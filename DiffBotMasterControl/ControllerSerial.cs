using System;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
using System.Threading;

namespace DiffBotMasterControl
{
	public static class ControllerSerial
	{
		public class ControllerPacket : DiffPacket
		{
			public byte id { get { return bytes[0]; } }
			public byte lsth { get { return bytes[1]; } }
			public byte lstv { get { return bytes[2]; } }
			public byte rsth { get { return bytes[3]; } }
			public byte rstv { get { return bytes[4]; } }
			public byte but { get { return bytes[5]; } }

			public ControllerPacket() : base(6) { }
		}

		class PacketHandler
		{
			public delegate bool Filter(ControllerPacket p);
			private Action<ControllerPacket> handler;
			private ManualResetEventSlim recieved = new ManualResetEventSlim();

			public ControllerPacket Wait(Filter filter, TimeSpan timeout) {
				if(handler != null) throw new Exception("Alreading waiting on a packet");

				ControllerPacket ret = null;
				recieved.Reset();
				handler = p => {
					if (!filter(p)) return;
					ret = p;
					recieved.Set();
					handler = null;
				};
				if(timeout > TimeSpan.Zero)
					recieved.Wait(timeout);
				handler = null;
				return ret;
			}

			public void Handle(ControllerPacket p) {
				if (handler != null) handler(p);
				else Log.Warn("Packet dropped");
			}
		}

		private static int[] signalStrength = new int[ControllerInput.ChannelCount];
		private static readonly int maxSignalStrength = 5;

		private static DiffBotSerial serial;
		private static PacketHandler handler = new PacketHandler();

		public static bool Connected(int remote) {
			return Connected() && signalStrength[remote] > 0;
		}

		public static readonly TimeSpan pollingInterval = TimeSpan.FromMilliseconds(100);
		public static void PollingThread(CancellationToken ct) {
			try {
				var stopwatch = new Stopwatch();
				while (!ct.IsCancellationRequested) {
					stopwatch.Restart();
					PollControllers();

					var wait = pollingInterval - stopwatch.Elapsed;
					if(wait > TimeSpan.Zero)
						ct.WaitHandle.WaitOne(wait);
				}
			}
			catch (Exception e) {
				Log.Error("Exception in Polling Thread", e);
			}
		}

		public static readonly TimeSpan responseInterval = pollingInterval - TimeSpan.FromMilliseconds(20);
		private static void PollControllers() {
			for(int i = 0; i < 10; i++)
				serial.SendPacket(new byte[] {250});

			var stopwatch = new Stopwatch();
			stopwatch.Start();

			var packets = new ControllerPacket[ControllerInput.ChannelCount];
			var responseTimes = new TimeSpan[ControllerInput.ChannelCount];

			while (packets.Any(a => a == null)) {
				var p = handler.Wait(f => f.id > 0 && f.id <= packets.Length, responseInterval-stopwatch.Elapsed);
				if (p == null) break;

				if (packets[p.id - 1] == null) {
					packets[p.id - 1] = p;
					responseTimes[p.id - 1] = stopwatch.Elapsed;
				}
				//serial.SendPacket();
			}

			for (int i = 0; i < packets.Length; i++) {
				var ch = i + 1;
				var p = packets[i];
				if (p != null) {
					if (signalStrength[i] == 0) {
						signalStrength[i] = maxSignalStrength;
						Log.Info(string.Format("#{0} connected", ch));
						MainForm.Instance.ConnectionChanged();
					} else if (signalStrength[i] < 5)
						signalStrength[i]++;

					Log.Info(string.Format("#{0} responded in {1}ms", ch, responseTimes[i].TotalMilliseconds));
					ControllerInput.Handle(p);

				} else if (signalStrength[i] > 0 && --signalStrength[i] == 0) {
					Log.Info(string.Format("#{0} lost connection", ch));
					MainForm.Instance.ConnectionChanged();
				} else if (signalStrength[i] > 0) {
					Log.Info(string.Format("#{0} missed in {1}ms", ch, stopwatch.ElapsedMilliseconds));
				}
			}
		}

		private static void ReadThread() {
			try {
				while (true) {
					if (serial.ReadByte() != 0xAA || serial.ReadByte() != 0x55) continue;
					var p = new ControllerPacket();
					serial.ReadBytes(p.bytes);
					if (p.CRC()) handler.Handle(p);
					else Log.Warn("CRC failed");
				}
			}
			catch (OperationCanceledException) {} 
			catch (Exception e) {
				Log.Error("Exception in Controller Read Thread", e);
			}
		}

		public static void Connect(string portName) {
			if (serial != null)
				throw new Exception("Already connected");

			Log.Info("Connecting controller serial on "+portName);
			try {
				serial = new DiffBotSerial(new SerialPort(portName, 57600, Parity.None, 8, StopBits.One));
				serial.AddThread(PollingThread);
				serial.AddThread(ReadThread);
				serial.Open();
			}
			catch (Exception e) {
				Log.Error("Exception connecting controller serial on "+portName, e);
			}
		}

		public static void Disconnect() {
			if (serial == null)
				throw new Exception("Not connected");

			Log.Info("Disconnecting controller serial");
			serial.Close();
			serial = null;
		}

		public static bool Connected() {
			return serial != null;
		}
	}
}

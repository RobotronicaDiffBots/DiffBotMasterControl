using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
using System.Threading;

namespace DiffBotMasterControl
{
	public static class ControllerHandler
	{
		class ControllerPacket : DiffPacket
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
				recieved.Wait(timeout);
				handler = null;
				return ret;
			}

			public void Handle(ControllerPacket p) {
				if (handler != null) handler(p);
			}
		}

		public static int[][] channels = new int[10][];
		public static int robotType;
		private static int[] signalStrength = new int[10];
		private static readonly int maxSignalStrength = 5;

		private static DiffBotSerial serial;
		private static CancellationTokenSource close;
		private static PacketHandler handler = new PacketHandler();
		private static List<Thread> threads = new List<Thread>(); 

		static ControllerHandler()  {
			for (int i = 0; i < channels.Length; i++)
				channels[i] = new[] { i + 1, i + 13, i + 23 };
		}

		public static bool Connected(int remote) {
			return Connected() && signalStrength[remote] > 0;
		}

		public static readonly TimeSpan pollingInterval = TimeSpan.FromMilliseconds(100);
		public static void PollingThread() {
			try {
				var ct = close.Token;
				var stopwatch = new Stopwatch();
				while (!ct.IsCancellationRequested) {
					stopwatch.Restart();
					PollControllers();
					ct.WaitHandle.WaitOne(pollingInterval - stopwatch.Elapsed);
				}
			}
			catch (Exception e) {
				Log.Error("Exception in Polling Thread", e);
			}
		}

		public static readonly TimeSpan responseInterval = pollingInterval - TimeSpan.FromMilliseconds(20);
		private static void PollControllers() {
			serial.SendPacket(new byte[] {250});
			var stopwatch = new Stopwatch();
			stopwatch.Start();

			var packets = new ControllerPacket[channels.Length];
			var responseTimes = new TimeSpan[channels.Length];

			while (packets.Any(a => a == null)) {
				var p = handler.Wait(f => f.id > 0 && f.id <= packets.Length, responseInterval-stopwatch.Elapsed);
				if (p == null) break;
				packets[p.id - 1] = p;
				responseTimes[p.id - 1] = stopwatch.Elapsed;
			}

			for (int i = 0; i < channels.Length; i++) {
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
					HandleControllerPacket(p);

				} else if (signalStrength[i] > 0 && --signalStrength[i] == 0) {
					Log.Info(string.Format("#{0} lost connection", ch));
					MainForm.Instance.ConnectionChanged();
				} else if (signalStrength[i] > 0) {
					Log.Info(string.Format("#{0} missed in {1}ms", ch, stopwatch.ElapsedMilliseconds));
				}
			}
		}

		private static void HandleControllerPacket(ControllerPacket p) {
			
		}

		private static void ReadThread() {
			try {
				while (true) {
					if (serial.ReadByte() != 0xAA) continue;
					if (serial.ReadByte() != 0x55) continue;
					var p = new ControllerPacket();
					serial.ReadBytes(p.bytes);
					if (p.CRC())
						handler.Handle(p);
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
				var port = new SerialPort(portName, 57600, Parity.None, 8, StopBits.One);
				port.Open();

				close = new CancellationTokenSource();
				serial = new DiffBotSerial(close.Token, port);

				threads.Add(new Thread(PollingThread));
				threads.Add(new Thread(ReadThread));
				foreach(var thread in threads)
					thread.Start();
			}
			catch (Exception e) {
				Log.Error("Exception connecting controller serial on "+portName, e);
			}
		}

		public static void Disconnect() {
			if (serial == null)
				throw new Exception("Not connected");

			Log.Info("Disconnecting controller serial");
			close.Cancel();
			foreach (var thread in threads)
				thread.Join();

			threads.Clear();
			serial.port.Close();
			serial = null;
		}

		public static bool Connected() {
			return serial != null;
		}
	}
}

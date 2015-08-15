using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;

namespace DiffBotMasterControl
{
	public class RobotSerial
	{
		private static DiffBotSerial serial;
		private static BlockingCollection<byte[]> packetQueue;
		private static byte seqno;

		public static void Connect(string portName) {
			if (serial != null)
				throw new Exception("Already connected");

			Log.Info("Connecting robot serial on "+portName);
			try {
				packetQueue = new BlockingCollection<byte[]>();
				serial = new DiffBotSerial(new SerialPort(portName, 57600, Parity.None, 8, StopBits.One));
				serial.AddThread(WriteThread);
				serial.AddThread(HeartbeatThread);
				serial.Open();
			} catch (Exception e) {
				Log.Error("Exception connecting controller serial on "+portName, e);
			}
		}

		public static void Disconnect() {
			if (serial == null)
				throw new Exception("Not connected");

			Log.Info("Disconnecting robot serial");
			ControllerInput.StopAlive(); //may not actually get through in time
			serial.Close();
			serial = null;
		}

		public static bool Connected() {
			return serial != null;
		}

		public static void AddPacket(IEnumerable<int> rIDs, byte type, byte d1, byte d2, byte d3, byte d4) {
			foreach (var rID in rIDs)
				AddPacket(rID, type, d1, d2, d3, d4);
		}

		public static void AddPacket(int rID, byte type, byte d1, byte d2, byte d3, byte d4) {
			if (Connected() && rID > 0 && rID < 255)
				packetQueue.Add(new[] { (byte)rID, type, d1, d2, d3, d4, ++seqno });
		}

		private static void WriteThread(DiffBotSerial serial) {
			try {
				var ct = serial.CancellationToken();
				while (true) serial.SendPacket(packetQueue.Take(ct));
			}
			catch (OperationCanceledException) {}
			catch (Exception e) {
				Log.Error("Exception in Write Thread", e);
			}
		}

		private static readonly TimeSpan heartbeatInterval = TimeSpan.FromSeconds(0.25);
		private static void HeartbeatThread(DiffBotSerial serial) {
			try {
				var ct = serial.CancellationToken();
				var stopwatch = new Stopwatch();
				ControllerInput.ResetAlive();
				while (!ct.IsCancellationRequested) {
					stopwatch.Restart();
					ControllerInput.SendKeepAlive();

					var wait = heartbeatInterval - stopwatch.Elapsed;
					if (wait > TimeSpan.Zero)
						ct.WaitHandle.WaitOne(wait);
				}
			} catch (Exception e) {
				Log.Error("Exception in Heartbeat Thread", e);
			}
		}
	}
}
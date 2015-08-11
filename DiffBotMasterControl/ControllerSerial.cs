using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DiffBotMasterControl
{
	public static class ControllerSerial
	{
		public class ControllerPacket : DiffPacket
		{
			public byte robotID { get { return bytes[0]; } set { bytes[0] = value; } }
			public byte type { get { return bytes[1]; } set { bytes[1] = value; } }
			public byte ldrive { get { return bytes[2]; } set { bytes[2] = value; } }
			public byte rdrive { get { return bytes[3]; } set { bytes[3] = value; } }
			public byte remoteID { get { return bytes[4]; } set { bytes[4] = value; } }
			public byte buttons { get { return bytes[5]; } set { bytes[5] = value; } }
			public byte i { get { return bytes[6]; } set { bytes[6] = value; } }

			public ControllerPacket() : base(7) { }
		}

		private static Dictionary<string, DiffBotSerial> serials = new Dictionary<string, DiffBotSerial>();

		private static void ReadThread(DiffBotSerial serial) {
			try {
				while (true) ControllerInput.Handle(ReadPacket(serial, true));
			}
			catch (OperationCanceledException) {}
			catch (InvalidOperationException e) {
				Log.Error(serial.port.PortName + " Read Thread: " + e.Message);
			} catch (IOException e) {
				Log.Error(serial.port.PortName + " Read Thread: " + e.Message);
			} catch (AccessViolationException e) {
				Log.Error(serial.port.PortName + " Read Thread: " + e.Message);
			} catch (Exception e) {
				Log.Error(serial.port.PortName + " Read Thread", e);
			} finally {
				Task.Run(() => ClosePort(serial));
			}
		}

		private static ControllerPacket ReadPacket(DiffBotSerial serial, bool warn) {
			while (true) {
				if (serial.ReadByte() != 0xAA || serial.ReadByte() != 0x55) continue;
				var p = new ControllerPacket();
				serial.ReadBytes(p.bytes);
				if (p.CRC()) return p;
				if (warn) Log.Warn("CRC failed");
			}
		}

		public static void ScanPort(string portName, TimeSpan timeout) {
			Log.Info("Scanning port "+portName);
			var serial = DiffBotSerial.Create(portName);
			try {
				serial.Open();
				var task = Task.Run(() => ReadPacket(serial, false));
				if (!task.Wait(timeout)) {
					serial.Close();
					Log.Info("Scan timed out " + portName);
				} else {
					serial.AddThread(ReadThread);
					_AddPort(serial);
				}
			} catch (Exception) {
				Log.Warn("Scanning failed "+portName);
			}
		}

		private static void _AddPort(DiffBotSerial serial) {
			lock (serials) {
				ClosePort(serial.port.PortName);
				serials[serial.port.PortName] = serial;
				ControllerPortForm.Instance.Add(serial.port.PortName);
			}
			Log.Info("Opened controller port " + serial.port.PortName);
		}

		public static void ClosePort(string portName) {
			lock (serials) {
				DiffBotSerial serial;
				if(serials.TryGetValue(portName, out serial))
					ClosePort(serial);
			}
		}

		public static void ClosePort(DiffBotSerial serial) {
			lock (serials) {
				DiffBotSerial stored;
				if(serials.TryGetValue(serial.port.PortName, out stored) && stored == serial) {
					serials.Remove(serial.port.PortName);
					ControllerPortForm.Instance.Remove(serial.port.PortName);
				}
			}
			try {
				serial.Close();
				Log.Info("Closed controller port " + serial.port.PortName);
			}
			catch (Exception e) {
				Log.Error("Exception closing controller port "+serial.port.PortName, e);
			}
		}

		public static void AddPort(string portName) {
			try {
				var serial = DiffBotSerial.Create(portName);
				serial.AddThread(ReadThread);
				serial.Open();
				_AddPort(serial);
			}
			catch (Exception e) {
				Log.Error("Exception opening controller port "+portName, e);
			}
		}
	}
}

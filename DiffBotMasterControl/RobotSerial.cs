using System;
using System.IO.Ports;

namespace DiffBotMasterControl
{
	public class RobotSerial
	{
		private static DiffBotSerial serial;

		public static void Connect(string portName) {
			if (serial != null)
				throw new Exception("Already connected");

			Log.Info("Connecting robot serial on "+portName);
			try {
				serial = new DiffBotSerial(new SerialPort(portName, 57600, Parity.None, 8, StopBits.One));
				//serial.AddThread(PollingThread);
				//serial.AddThread(ReadThread);
				serial.Open();
			} catch (Exception e) {
				Log.Error("Exception connecting controller serial on "+portName, e);
			}
		}

		public static void Disconnect() {
			if (serial == null)
				throw new Exception("Not connected");

			Log.Info("Disconnecting robot serial");
			serial.Close();
			serial = null;
		}

		public static bool Connected() {
			return serial != null;
		}
	}
}
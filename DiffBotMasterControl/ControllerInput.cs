namespace DiffBotMasterControl
{
	public class ControllerInput
	{
		private static bool enabled;

		public static void Enable() {
			enabled = true;
			Log.Info("Controller Input Enabled");
		}

		public static void Disable() {
			enabled = false;
			Log.Info("Controller Input Disabled");
		}

		public static void Handle(ControllerSerial.ControllerPacket p) {
			bool lt = (p.but & 0x80) != 0;
			bool rt = (p.but & 0x40) != 0;
			byte rID = (byte)ControllerSerial.channels[p.id - 1][ControllerSerial.robotType];
			if (lt) RobotSerial.AddPacket(rID, 120, 3, 0, 0, 40);
			if (rt) RobotSerial.AddPacket(rID, 120, 0, 0, 0, 40);
		}
	}
}

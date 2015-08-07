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

		public static void Handle(ControllerSerial.ControllerPacket controllerPacket) {
		}
	}
}

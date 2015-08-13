using System;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace DiffBotMasterControl
{
	public class ControllerInput
	{
		public static readonly int ControllerCount = 10;
		public static readonly int RobotTypeCount = 3;
		private static int[][] channels = new int[ControllerCount][];
		private static int robotType;
		private static bool enabled;

		private static DateTime[] recvTimes = new DateTime[ControllerCount];
		private static bool[] alive = new bool[ControllerCount];

		public static int RobotType {
			get { return robotType; }
			set {
				if (robotType != value) StopOldBots(robotType, value);
				robotType = value;
				Properties.Settings.Default.RobotType = value;
			}
		}

		static ControllerInput() {
			robotType = Properties.Settings.Default.RobotType;
			if (robotType < 0 || robotType >= RobotTypeCount)
				RobotType = 0;


			for (int i = 0; i < channels.Length; i++)
				channels[i] = new[] { i + 1, i + 13, i + 23 };

			//load channel config
			foreach (var entry in Properties.Settings.Default.Channels.Split(new[] {'|'}, StringSplitOptions.RemoveEmptyEntries)) {
				try {
					var a = entry.Split(',').Select(int.Parse).ToArray();
					channels[a[0]][a[1]] = a[2];
				} catch {}
			}
		}

		public static void Enable() {
			enabled = true;
			Log.Info("Controller Input Enabled");
		}

		public static void Disable() {
			enabled = false;
			Log.Info("Controller Input Disabled");
		}

		internal static int GetChannel(int controller, int robotType) {
			return channels[controller][robotType];
		}

		internal static void SetChannel(int controller, int robotType, int value) {
			channels[controller][robotType] = value;
			//save channel config
			var s = new StringBuilder();
			for(int i = 0; i < ControllerCount; i++)
				for (int j = 0; j < channels[i].Length; j++)
					s.Append("" + i + ',' + j + ',' + channels[i][j] + '|');

			Properties.Settings.Default.Channels = s.ToString();
		}

		internal static void PopulateChannelGrid(DataGridView grid) {
			foreach (var ch in channels)
				grid.Rows.Add(grid.RowCount + 1, ch[0], ch[1], ch[2]);
		}

		public static void Handle(ControllerSerial.ControllerPacket p) {
			if (p.remoteID < 1 || p.remoteID > ControllerCount) return;
			recvTimes[p.remoteID - 1] = DateTime.Now;

			p.robotID = (byte)channels[p.remoteID - 1][robotType];
			RobotSerial.AddPacket(p.robotID, p.type, p.ldrive, p.rdrive, p.remoteID, p.buttons);
		}

		public static bool Connected(int controller) {
			return DateTime.Now - recvTimes[controller] < TimeSpan.FromSeconds(.5);
		}

		public static void SendKeepAlive() {
			for (int i = 0; i < ControllerCount; i++) {
				var connected = Connected(i);
				if(connected)
					RobotSerial.AddPacket((byte)channels[i][robotType], 255, 0, 0, 0, 0);
				else if(alive[i])
					StopBot((byte)channels[i][robotType]);

				alive[i] = connected;
			}
		}

		private static void StopOldBots(int oldRobotType, int newRobotType) {
			for (int i = 0; i < ControllerCount; i++)
				if (alive[i] && channels[i][oldRobotType] != channels[i][newRobotType])
					StopBot((byte)channels[i][oldRobotType]);
		}

		private static void StopBot(byte robotID) {
			RobotSerial.AddPacket(robotID, 1, 100, 100, 0, 0);
		}

		public static void ResetAlive() {
			for (int i = 0; i < ControllerCount; i++)
				alive[i] = false;
		}
	}
}

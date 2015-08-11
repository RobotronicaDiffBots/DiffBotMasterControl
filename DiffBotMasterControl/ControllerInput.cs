using System;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace DiffBotMasterControl
{
	public class ControllerInput
	{
		public static readonly int ChannelCount = 10;
		public static readonly int RobotTypeCount = 3;
		private static int[][] channels = new int[ChannelCount][];
		private static int robotType;
		private static bool enabled;

		public static int RobotType {
			get { return robotType; }
			set {
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
			for(int i = 0; i < ChannelCount; i++)
				for (int j = 0; j < channels[i].Length; j++)
					s.Append("" + i + ',' + j + ',' + channels[i][j] + '|');

			Properties.Settings.Default.Channels = s.ToString();
		}

		internal static void PopulateChannelGrid(DataGridView grid) {
			foreach (var ch in channels)
				grid.Rows.Add(grid.RowCount + 1, ch[0], ch[1], ch[2]);
		}

		public static void Handle(ControllerSerial.ControllerPacket p) {
			bool lt = (p.but & 0x80) != 0;
			bool rt = (p.but & 0x40) != 0;
			byte rID = (byte)channels[p.id - 1][robotType];
			if (lt) RobotSerial.AddPacket(rID, 120, 3, 0, 0, 40);
			if (rt) RobotSerial.AddPacket(rID, 120, 0, 0, 0, 40);
		}
	}
}

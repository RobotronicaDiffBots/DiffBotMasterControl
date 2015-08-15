using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace DiffBotMasterControl
{
	public class ControllerInput
	{
		public static readonly int ControllerCount = 10;
		public static readonly int RobotTypeCount = 3;

		public class ChannelSetting
		{
			public readonly string ident;
			public readonly List<int> bots;

			public ChannelSetting(string ident, List<int> bots) {
				this.ident = ident;
				this.bots = bots;
			}

			public override string ToString() {
				return ident;
			}
		}

		private static List<Func<string, ChannelSetting>> channelParsers = new List<Func<string, ChannelSetting>>(); 
		private static ChannelSetting[][] channels = new ChannelSetting[ControllerCount][];
		private static int robotType;
		private static bool enabled = true;

		private static readonly TimeSpan ControllerTimeout = TimeSpan.FromMilliseconds(300);
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

			AddDefaultParsers();

			for (int i = 0; i < channels.Length; i++)
				channels[i] = new[] { SimpleChannel(i + 1), SimpleChannel(i + 13), SimpleChannel(i + 23) };

			//load channel config
			foreach (var entry in Properties.Settings.Default.Channels.Split(new[] {'|'}, StringSplitOptions.RemoveEmptyEntries)) {
				try {
					var a = entry.Split(',').ToArray();
					var ch = ParseChannel(a[2]);
					if(ch != null)
						channels[int.Parse(a[0])][int.Parse(a[1])] = ch;
				} catch {}
			}
		}

		private static void AddDefaultParsers() {
			channelParsers.Add(s => {
				int i;
				return int.TryParse(s, out i) && i > 0 && i < 256 ? SimpleChannel(i) : null;
			});

			channelParsers.Add(s => {
				var range = s.Split(':');
				if (range.Length != 2) return null;
				var i = new int[2];
				if (int.TryParse(range[0], out i[0]) && int.TryParse(range[1], out i[1]) &&
						i[0] > 0 && i[0] < 256 && 
						i[1] > 0 && i[1] < 256 && 
						i[0] <= i[1])
					return RangeChannel(i[0], i[1]);

				return null;
			});
		}

		public static void Enable() {
			enabled = true;
			Log.Info("Controller Input Enabled");
		}

		public static void Disable() {
			enabled = false;
			Log.Info("Controller Input Disabled");
		}

		public static void AddChannelParser(Func<string, ChannelSetting> parser) {
			channelParsers.Add(parser);
		}

		public static ChannelSetting ParseChannel(string text) {
			return channelParsers.Select(parser => parser(text)).FirstOrDefault(ch => ch != null);
		}

		public static ChannelSetting GetChannel(int controller, int robotType) {
			return channels[controller][robotType];
		}

		public static void SetChannel(int controller, int robotType, ChannelSetting ch) {
			var oldBots = new HashSet<int>();
			var newBots = new HashSet<int>();
			//add all live bots in the column to the old set
			for (int i = 0; i < ControllerCount; i++)
				if(alive[i])
					channels[i][robotType].bots.ForEach(id => oldBots.Add(id));
			
			//set the new channel
			_SetChannel(controller, robotType, ch);
			var chSet = new HashSet<int>(ch.bots);

			//set any channels that overlap with the new one to 255 (none)
			for (int i = 0; i < ControllerCount; i++)
				if (i != controller && channels[i][robotType].bots.Any(chSet.Contains))
					_SetChannel(i, robotType, SimpleChannel(255));

			//add all live bots in the column to the new set
			for (int i = 0; i < ControllerCount; i++)
				if (alive[i])
					channels[i][robotType].bots.ForEach(id => newBots.Add(id));

			//stop any old bots that aren't in the new config
			oldBots.ExceptWith(newBots);
			StopBots(oldBots);

			//save channel config
			var s = new StringBuilder();
			for(int i = 0; i < ControllerCount; i++)
				for (int j = 0; j < channels[i].Length; j++)
					s.Append("" + i + ',' + j + ',' + channels[i][j] + '|');

			Properties.Settings.Default.Channels = s.ToString();
		}

		private static void _SetChannel(int controller, int robotType, ChannelSetting ch) {
			channels[controller][robotType] = ch;
			MainForm.Instance.SetChannel(controller, robotType, ch.ToString());
		}

		public static ChannelSetting SimpleChannel(int rID) {
			return new ChannelSetting(rID.ToString(), new [] {rID}.ToList());
		}

		private static ChannelSetting RangeChannel(int first, int last) {
			return new ChannelSetting(first + ":" + last, Enumerable.Range(first, last-first+1).ToList());
		}

		internal static void PopulateChannelGrid(DataGridView grid) {
			foreach (var ch in channels)
				grid.Rows.Add(grid.RowCount + 1, ch[0], ch[1], ch[2]);
		}

		public static void Handle(ControllerSerial.ControllerPacket p) {
			if (p.remoteID < 1 || p.remoteID > ControllerCount) return;
			recvTimes[p.remoteID - 1] = DateTime.Now;

			if (!enabled) return;

			RobotSerial.AddPacket(channels[p.remoteID - 1][robotType].bots, p.type, p.ldrive, p.rdrive, p.remoteID, p.buttons);
		}

		public static bool Connected(int controller) {
			return DateTime.Now - recvTimes[controller] < ControllerTimeout;
		}

		public static void SendKeepAlive() {
			for (int i = 0; i < ControllerCount; i++) {
				var connected = Connected(i);
				if(connected)
					RobotSerial.AddPacket(channels[i][robotType].bots, 255, 0, 0, 0, 0);
				else if(alive[i])
					StopBots(channels[i][robotType].bots);

				alive[i] = connected;
			}
		}

		private static void StopOldBots(int oldRobotType, int newRobotType) {
			var oldBots = new HashSet<int>();
			var newBots = new HashSet<int>();
			for (int i = 0; i < ControllerCount; i++)
				if (alive[i]) {
					channels[i][oldRobotType].bots.ForEach(id => oldBots.Add(id));
					channels[i][newRobotType].bots.ForEach(id => newBots.Add(id));
				}

			oldBots.ExceptWith(newBots);
			StopBots(oldBots);
		}

		public static void StopBots(IEnumerable<int> bots) {
			if (bots.Any()) {
				Log.Info("Stopping " + string.Join(",", bots));
				RobotSerial.AddPacket(bots, 1, 100, 100, 0, 0);
			}
		}

		public static void ResetAlive() {
			for (int i = 0; i < ControllerCount; i++)
				alive[i] = false;
		}

		public static void StopAlive() {
			for (int i = 0; i < ControllerCount; i++) {
				if (alive[i])
					StopBots(channels[i][robotType].bots);

				alive[i] = false;
			}
		}
	}
}

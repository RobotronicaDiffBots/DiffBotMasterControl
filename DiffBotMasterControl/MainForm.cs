using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace DiffBotMasterControl
{
	public partial class MainForm : Form
	{
		public static MainForm Instance { get; private set; }
		private Keys commandKey;

		public MainForm() {
			InitializeComponent();

			textBoxKey_SetText("[ ]");

			ControllerInput.PopulateChannelGrid(dataGridChannels);
			RecolourChannelGrid();

			Instance = this;
		}

		private void MainForm_Load(object sender, EventArgs e) {
			ControllerPortForm.Init();
			Commands.Load();
		}

		private void dataGridChannels_CellValidating(object sender, DataGridViewCellValidatingEventArgs e) {
			if (e.ColumnIndex == 0)
				return;

			var ch = ControllerInput.ParseChannel(e.FormattedValue.ToString());
			if (ch != null)
				ControllerInput.SetChannel(e.RowIndex, e.ColumnIndex-1, ch);
			else
				dataGridChannels.EditingControl.Text = ControllerInput.GetChannel(e.RowIndex, e.ColumnIndex-1).ToString();
		}

		internal void SetChannel(int controller, int robotType, string ident) {
			Invoke(new Action(() => dataGridChannels[robotType + 1, controller].Value = ident));
		}

		private void dataGridChannels_Leave(object sender, EventArgs e) {
			dataGridChannels.ClearSelection();
		}

		private void textBoxKey_Enter(object sender, EventArgs e) {
			textBoxKey.BackColor = Color.DodgerBlue;
			listBoxCommands.ClearSelected();
		}

		private void textBoxKey_Leave(object sender, EventArgs e) {
			textBoxKey.BackColor = Color.FromArgb(255, 240, 240, 240);
		}

		private void textBoxKey_SelectionChanged(object sender, EventArgs e) {
			textBoxKey.DeselectAll();
		}

		[DllImport("user32.dll")]
		private static extern int HideCaret(IntPtr hwnd);
		private void textBoxKey_HideCaret(object sender, MouseEventArgs e) {
			HideCaret(textBoxKey.Handle);
		}

		private void textBoxKey_SetText(string text) {
			textBoxKey.Text = text;
			textBoxKey.SelectAll();
			textBoxKey.SelectionAlignment = HorizontalAlignment.Center;
			textBoxKey_HideCaret(null, null);
		}

		private void textBoxKey_KeyDown(object sender, KeyEventArgs e) {
			e.SuppressKeyPress = true;

			if (Commands.Running || commandKey == e.KeyCode) {
				if(e.KeyCode == Keys.Escape)
					Commands.Cancel();

				return;
			}
			
			textBoxKey.BackColor = Commands.KeyCommand(e.KeyCode) ? Color.SeaGreen : Color.IndianRed;
			textBoxKey_SetText("[" + e.KeyCode + "]");
			commandKey = e.KeyCode;
		}

		private void textBoxKey_KeyUp(object sender, KeyEventArgs e) {
			if (e.KeyCode == commandKey) {
				textBoxKey_SetText("[ ]");
				textBoxKey_Enter(sender, null);
				commandKey = Keys.None;
			}
		}

		private void listBoxCommands_SelectedIndexChanged(object sender, EventArgs e) {
			UpdateExecuteButton();
		}

		private void UpdateExecuteButton() {
			if (Commands.Running) {
				buttonExecute.Enabled = true;
				listBoxCommands.Enabled = false;
				buttonExecute.Text = "Cancel";
			} else {
				listBoxCommands.Enabled = true;
				buttonExecute.Enabled = listBoxCommands.SelectedItems.Count == 1;
				buttonExecute.Text = "Execute";
			}
		}

		private void buttonExecute_Click(object sender, EventArgs e) {
			if (Commands.Running)
				Commands.Cancel();
			else
				Commands.Run((Commands.Command) listBoxCommands.SelectedItem);
		}

		private void MainForm_MouseClick(object sender, MouseEventArgs e) {
			ActiveControl = null;
		}

		internal void AddCommandEntry(Commands.Command c) {
			Invoke(new Action(() => {
				listBoxCommands.Items.Add(c);
			}));
		}

		internal void ResetCommandList() {
			Invoke(new Action(() => {
				listBoxCommands.Items.Clear();
			}));
		}

		private void dataGridChannels_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e) {
			if (e.ColumnIndex > 0) {
				ControllerInput.RobotType = e.ColumnIndex - 1;
				dataGridChannels.EndEdit();
				dataGridChannels.CurrentCell = null;
				RecolourChannelGrid();
			}
		}

		private static readonly Color[] channelColors = {
			Color.LightGray, Color.White, Color.SteelBlue, Color.DeepSkyBlue
		};
		private void RecolourChannelGrid() {
			for (int r = 0; r < ControllerInput.ControllerCount; r++)
				for (int c = 0; c < ControllerInput.RobotTypeCount; c++) {
					var key = (ControllerInput.Connected(r) ? 1 : 0) + (ControllerInput.RobotType == c ? 2 : 0);
					dataGridChannels[c + 1, r].Style.BackColor = channelColors[key];
				}
		}

		public void CommandStarted() {
			Invoke(new Action(UpdateExecuteButton));
		}

		public void CommandFinished() {
			Invoke(new Action(UpdateExecuteButton));
		}
		
		private void buttonConnectRobots_Click(object sender, EventArgs e) {
			if (RobotSerial.Connected()) {
				RobotSerial.Disconnect();
				buttonConnectRobots.Text = "Connect Robots";
			} else {
				var port = PortSelectForm.FromConfig("Robot Serial Port", "RobotPort");
				if (!string.IsNullOrEmpty(port))
					RobotSerial.Connect(port);

				if (RobotSerial.Connected())
					buttonConnectRobots.Text = "Disconnect Robots";
			}
		}

		private void buttonControllerPorts_Click(object sender, EventArgs e) {
			ControllerPortForm.Show();
		}

		private void timerUpdateLog_Tick(object sender, EventArgs e) {
			Log.Update(textBoxLog);
		}

		private void timerUpdateGrid_Tick(object sender, EventArgs e) {
			RecolourChannelGrid();
		}

		private void listBoxCommands_MouseDoubleClick(object sender, MouseEventArgs e) {
			int index = listBoxCommands.IndexFromPoint(e.Location);
			if (index != ListBox.NoMatches && !Commands.Running)
				Commands.Run((Commands.Command) listBoxCommands.Items[index]);
		}
	}
}

using System;
using System.IO.Ports;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DiffBotMasterControl
{
	public partial class ControllerPortForm : Form
	{
		public static readonly ControllerPortForm Instance = new ControllerPortForm();
		private string[] serialPorts = SerialPort.GetPortNames();

		private static readonly TimeSpan ScanTimeout = TimeSpan.FromSeconds(1);
		private static readonly TimeSpan ConnectScanTimeout = TimeSpan.FromSeconds(3);

		public static void Init() {
			Show();
			Instance.Hide();
		}

		private ControllerPortForm() {
			InitializeComponent();
			UsbNotification.RegisterUsbDeviceNotification(Handle);
		}

		protected override void WndProc(ref Message m) {
			base.WndProc(ref m);
			if (m.Msg == UsbNotification.WmDevicechange) {
				switch ((int)m.WParam) {
					case UsbNotification.DbtDeviceremovecomplete:
					case UsbNotification.DbtDevicearrival:
						UpdatePortBox();
						ScanPortChanges();
						break;
				}
			}
		}

		private void ScanPortChanges() {
			var newPorts = SerialPort.GetPortNames();
			foreach (var port in newPorts.Where(port => !serialPorts.Contains(port)))
				Task.Run(() => ControllerSerial.ScanPort(port, ConnectScanTimeout));

			serialPorts = newPorts;
		}

		private void UpdatePortBox() {
			BeginInvoke(new Action(() => {
				var exclude = listBoxPorts.Items.Cast<string>().ToList();
				if (RobotSerial.Connected()) exclude.Add(Properties.Settings.Default.RobotPort);
				var ports = SerialPort.GetPortNames().Where(s => !exclude.Contains(s));

				comboBoxPort.Items.Clear();
				comboBoxPort.Items.AddRange(ports.ToArray<object>());
			}));
		}

		public new static void Show() {
			Instance.UpdatePortBox();
			((Control) Instance).Show();
		}

		public void Remove(string portName) {
			BeginInvoke(new Action(() => {
				listBoxPorts.Items.Remove(portName);
				UpdatePortBox();
			}));
		}

		public void Add(string portName) {
			BeginInvoke(new Action(() => {
				listBoxPorts.Items.Add(portName);
				UpdatePortBox();
			}));
		}

		private void ControllerPortForm_FormClosing(object sender, FormClosingEventArgs e) {
			if (e.CloseReason != CloseReason.UserClosing) return;
			e.Cancel = true;
			Hide();
		}

		private void buttonAdd_Click(object sender, EventArgs e) {
			ControllerSerial.AddPort(comboBoxPort.Text);
		}

		private void buttonSearch_Click(object sender, EventArgs e) {
			UpdatePortBox();
			var ports = comboBoxPort.Items.Cast<string>();
			Parallel.ForEach(ports, p => ControllerSerial.ScanPort(p, ScanTimeout));
			UpdatePortBox();
		}

		private void buttonRemove_Click(object sender, EventArgs e) {
			foreach(var port in listBoxPorts.SelectedItems.Cast<string>().ToArray())
				ControllerSerial.ClosePort(port);
		}

		private void buttonRemoveAll_Click(object sender, EventArgs e) {
			foreach (var port in listBoxPorts.Items.Cast<string>().ToArray())
				ControllerSerial.ClosePort(port);
		}

		private void ControllerPortForm_Load(object sender, EventArgs e) {
			Parallel.ForEach(serialPorts, p => ControllerSerial.ScanPort(p, ScanTimeout));
		}
	}
}

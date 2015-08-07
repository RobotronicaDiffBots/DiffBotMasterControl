using System.IO.Ports;
using System.Windows.Forms;

namespace DiffBotMasterControl
{
	public partial class PortSelectForm : Form
	{
		public string Port { get { return comboBoxPort.Text; } }

		public PortSelectForm(string initial) {
			InitializeComponent();

			comboBoxPort.Text = initial;

			foreach (var s in SerialPort.GetPortNames())
				comboBoxPort.Items.Add(s);
		}

		public static string ShowDialog(string title, string initial) {
			var dialog = new PortSelectForm(initial) {
				Text = title
			};
			if (dialog.ShowDialog() != DialogResult.OK)
				return null;

			return dialog.Port;
		}

		public static string FromConfig(string title, string key) {
			var s = ShowDialog(title, (string) Properties.Settings.Default[key]);
			if (!string.IsNullOrEmpty(s)) {
				Properties.Settings.Default[key] = s;
				Properties.Settings.Default.Save();
			}
			return s;
		}
	}
}

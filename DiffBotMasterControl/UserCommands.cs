using System;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace DiffBotMasterControl
{
	class UserCommands
	{
		public static void Wait(CancellationToken ct, TimeSpan time) {
			if(time > TimeSpan.Zero && ct.WaitHandle.WaitOne(time))
				throw new OperationCanceledException();
		}

		public static void DisableControllersFor(CancellationToken ct, TimeSpan time) {
			ControllerInput.Disable();
			Wait(ct, time);
			ControllerInput.Enable();
		}

		public static void Load() {
			Commands.Add("Enable Controllers", Keys.O, ControllerInput.Enable);
			Commands.Add("Disable Controllers", Keys.P, ControllerInput.Disable);
			Commands.Add("Fade on 1-10", Keys.G, () => RobotSerial.AddPacket(250, 122, 1, 0, 0, 40));
			Commands.Add("Fade off 1-10", Keys.H, () => RobotSerial.AddPacket(250, 122, 0, 0, 0, 40));
			Commands.Add("Osc", Keys.J, () => RobotSerial.AddPacket(250, 120, 2, 0, 0, 40));
			Commands.Add("Spin", Keys.S, () => RobotSerial.AddPacket(Enumerable.Range(1, 9), 1, 200, 0, 0, 0));
			Commands.Add("Stop", Keys.X, () => RobotSerial.AddPacket(Enumerable.Range(1, 9), 1, 100, 100, 0, 0));
			Commands.Add("Wait Test", Keys.W, ct => DisableControllersFor(ct, TimeSpan.FromSeconds(4)));
		}
	}
}
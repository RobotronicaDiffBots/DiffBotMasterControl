using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace DiffBotMasterControl
{
	class UserCommands
	{
		public static void Wait(CancellationToken ct, TimeSpan time) {
			if (time > TimeSpan.Zero && ct.WaitHandle.WaitOne(time))
				throw new OperationCanceledException();
		}

		public static void DisableControllersFor(CancellationToken ct, TimeSpan time) {
			ControllerInput.Disable();
			Wait(ct, time);
			ControllerInput.Enable();
		}

		public static void LoopFor(CancellationToken ct, TimeSpan limit, TimeSpan interval, Action action) {
			var limitT = new Stopwatch();
			var intervalT = new Stopwatch();

			limitT.Start();
			while (limitT.Elapsed < interval) {
				intervalT.Restart();
				action();
				Wait(ct, interval - intervalT.Elapsed);
			}
		}

		public static void Stop(int rID) {
			RobotSerial.AddPacket((byte) rID, 1, 100, 100, 0, 0);
		}

		//amount is 1 or 2
		public static void Spin(int rID, int amount, bool ccw) {
			var buttons = (byte) (amount << 5);
			if (ccw) buttons |= 0x80;
			RobotSerial.AddPacket((byte) rID, 2, 0, 0, 0, buttons);
		}

		public static void Load() {
			Commands.Add("Enable Controllers", Keys.O, ControllerInput.Enable);
			Commands.Add("Disable Controllers", Keys.P, () => { ControllerInput.Disable(); ControllerInput.StopAlive(); });

			Commands.Add("Default Cube Channels", Keys.None, () => {
				for(int i = 0; i < 10; i++) ControllerInput.SetChannel(i, 0, ControllerInput.SimpleChannel(i+1));
			});

			Commands.Add("Red", Keys.R, () => RobotSerial.AddPacket(250, 120, 2, 0, 100, 40));
			Commands.Add("Green", Keys.G, () => RobotSerial.AddPacket(250, 120, 3, 0, 100, 40));
			Commands.Add("Blue", Keys.B, () => RobotSerial.AddPacket(250, 120, 4, 0, 100, 40));
			Commands.Add("Lights On", Keys.L, () => RobotSerial.AddPacket(250, 122, 1, 0, 100, 40));
			Commands.Add("Lights Off", Keys.K, () => RobotSerial.AddPacket(250, 122, 0, 0, 100, 40));

			Commands.Add("Spin CW", Keys.S, () => Spin(250, 1, false));
			Commands.Add("Spin CCW", Keys.A, () => Spin(250, 1, true));
			Commands.Add("Spin 180 CW", Keys.W, () => Spin(250, 2, false));
			Commands.Add("Spin 180 CCW", Keys.Q, () => Spin(250, 2, true));

			Commands.Add("Stop", Keys.X, () => Stop(250));
		}
	}
}
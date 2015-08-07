using System;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace DiffBotMasterControl
{
	class UserCommands
	{
		public static void Wait(CancellationToken ct, TimeSpan time) {
			if(ct.WaitHandle.WaitOne(time))
				throw new OperationCanceledException();
		}

		public static void Load() {
			Commands.Add("Fade", Keys.G, () => PacketQueue.Add(new[] { 1, 4, 6, 11 }, 120, 3, 0, 0, 40));
			Commands.Add("Spin", Keys.S, () => PacketQueue.Add(Enumerable.Range(1, 9), 1, 200, 0, 0, 0));
			Commands.Add("Stop", Keys.X, () => PacketQueue.Add(Enumerable.Range(1, 9), 1, 100, 100, 0, 0));
			Commands.Add("Wait Test", Keys.W, ct => {
				Wait(ct, TimeSpan.FromSeconds(4));
				Log.Error("Task Done (Error test)");
			});
		}
	}
}
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.CSharp;

namespace DiffBotMasterControl
{
	public class Commands
	{
		public class Command
		{
			public readonly string name;
			public readonly Keys key;
			internal readonly Action<CancellationToken> action;

			public Command(string name, Keys key, Action<CancellationToken> action) {
				this.name = name;
				this.key = key;
				this.action = action;
			}

			public override string ToString() {
				return name + (key == Keys.None ? "" : " [" + key + "]");
			}
		}

		private static CancellationTokenSource cancelSource;
		private static Command runningCommand;
		public static bool Running { get { return runningCommand != null; } }
		private static IDictionary<string, Command> commandMap = new Dictionary<string, Command>();
		private static IDictionary<Keys, Command> keyMap = new Dictionary<Keys, Command>();

		public static void Add(string name, Keys key, Action action) {
			Add(new Command(name, key, ct => action()));
		}

		public static void Add(string name, Keys key, Action<CancellationToken> action) {
			Add(new Command(name, key, action));
		}

		public static void Add(Command c) {
			if (commandMap.ContainsKey(c.name)) {
				Log.Error("Command \""+c.name+"\" already registered.");
				return;
			}

			if (c.key != Keys.None) {
				if (keyMap.ContainsKey(c.key))
					Log.Error("Duplicate key binding '" + c.key + "'. Already assigned to \"" + keyMap[c.key].name + "\"");
				else
					keyMap[c.key] = c;
			}

			commandMap[c.name] = c;
			MainForm.Instance.AddCommandEntry(c);
		}

		public static bool KeyCommand(Keys key) {
			Command c;
			if (!keyMap.TryGetValue(key, out c))
				return false;

			Run(c);
			return true;
		}

		public static void Load() {
			MainForm.Instance.ResetCommandList();
			commandMap.Clear();
			keyMap.Clear();
			Add("Reload Commands", Keys.None, Load);

			try {
				var file = "UserCommands.cs";
				if (!File.Exists(file))
					File.WriteAllText(file, Properties.Resources.UserCommands);

				var provider = new CSharpCodeProvider();
				var options = new CompilerParameters {
					GenerateInMemory = true,
					IncludeDebugInformation = true,
					ReferencedAssemblies = {
						Assembly.GetExecutingAssembly().Location,
						"mscorlib.dll",
						"System.dll",
						"System.Core.dll",
						"System.Windows.Forms.dll",
					}
				};
				var results = provider.CompileAssemblyFromFile(options, file);
				if (results.Errors.HasErrors) {
					Log.Error("Errors Compiling UserCommands.cs");
					foreach (CompilerError error in results.Errors)
						Log.Error(string.Format("  at {0}: Error ({1}): {2}", error.Line, error.ErrorNumber, error.ErrorText));

					return;
				}

				var assembly = results.CompiledAssembly;
				assembly.GetType("DiffBotMasterControl.UserCommands").GetMethod("Load").Invoke(null, null);
			}
			catch (Exception e) {
				Log.Error(e.ToString());
			}
		}

		public static void Run(Command c) {
			if (runningCommand != null) {
				Log.Warn("Command \""+c.name+"\" already running. Cannot start a new command.");
				return;
			}

			cancelSource = new CancellationTokenSource();
			Task.Run(() => {
				try {
					runningCommand = c;
					Log.Info("Running Command \""+c.name+"\"");
					MainForm.Instance.CommandStarted();
					c.action(cancelSource.Token);
				}
				catch (OperationCanceledException) {
					throw;
				}
				catch (Exception e) {
					Log.Error("Error running command \"" + c.name + "\"", e);
				}
				finally {
					runningCommand = null;
					MainForm.Instance.CommandFinished();
				}
			}, cancelSource.Token);
		}

		public static void Cancel() {
			cancelSource.Cancel();
		}
	}
}

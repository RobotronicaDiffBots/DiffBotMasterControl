using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace DiffBotMasterControl
{
	public class Log
	{
		public class LogLine
		{
			public Color color;
			public string line;

			public LogLine(Color color, string line) {
				this.color = color;
				this.line = line;
			}
		}

		private static int maxLines = 5000;
		private static int lines = 0;
		private static ConcurrentQueue<LogLine> appendQueue = new ConcurrentQueue<LogLine>();
		private static StreamWriter writer = new StreamWriter("log.txt");

		private static void InternalLog(string s, Color c) {
			var timestamp = '['+DateTime.Now.ToString("HH:mm:ss.fff")+"] ";
			var lines = s.Split(new[] { "\r\n" }, StringSplitOptions.None);
			appendQueue.Enqueue(new LogLine(c, timestamp + lines[0]));
			for (int i = 1; i < lines.Length; i++)
				appendQueue.Enqueue(new LogLine(c, new string(' ', timestamp.Length) + lines[i]));
		}

		internal static void Update(RichTextBox box) {
			if (appendQueue.IsEmpty) return;

			var start = box.SelectionStart;
			var len = box.SelectionLength;
			var scroll = box.TextLength == start;

			LogLine result;
			box.Select(box.TextLength, 0);
			while (appendQueue.TryDequeue(out result)) {
				box.SelectionColor = result.color;
				box.AppendText(result.line+"\r\n");
				writer.WriteLine(result.line);
				lines++;
			}
			writer.FlushAsync();

			if (lines > maxLines) {
				box.ReadOnly = false;
				while (lines > maxLines) {
					var lineLen = box.Text.IndexOf('\n') + 1;
					box.Select(0, lineLen);
					box.Cut();
					start -= lineLen;
					lines--;
				}
				box.ReadOnly = true;
			}

			if (scroll) {
				box.Select(box.TextLength, 0);
				box.ScrollToCaret();
			} else if (start + len > 0) {
				if (start < 0) {
					len += start;
					start = 0;
				}
				box.Select(start, len);
			}
		}

		public static void Error(string s) {
			InternalLog(s, Color.Red);
		}

		public static void Error(string s, Exception e) {
			Error(s+"\r\n"+e);
		}

		public static void Warn(string s) {
			InternalLog(s, Color.YellowGreen);
		}

		public static void Info(string s) {
			InternalLog(s, Color.Black);
		}
	}
}

using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Threading;

namespace DiffBotMasterControl
{
	public class DiffBotSerial
	{
		/// <summary>
		/// Creates a new DiffBotSerial instance with the serial parameters for the project
		/// </summary>
		public static DiffBotSerial Create(string portName) {
			return new DiffBotSerial(new SerialPort(portName, 57600, Parity.None, 8, StopBits.One));
		}

		public readonly SerialPort port;
		private readonly CancellationTokenSource close;
		private readonly CancellationToken readCt;
		private readonly List<Thread> threads = new List<Thread>();
		private bool started;

		public DiffBotSerial(SerialPort port) {
			this.port = port;
			close = new CancellationTokenSource();
			readCt = close.Token;
			port.ReadTimeout = 5;
		}

		public CancellationToken CancellationToken() {
			return close.Token;
		}

		public byte ReadByte() {
			while (true) {
				readCt.ThrowIfCancellationRequested();
				try {
					return (byte) port.ReadByte();
				}
				catch (TimeoutException) {}
			}
		}

		public void SendPacket(byte[] bytes) {
			var pack = new byte[bytes.Length + 3];
			pack[0] = 0xAA;
			pack[1] = 0x55;
			Array.Copy(bytes, 0, pack, 2, bytes.Length);
			byte crc = 0;
			for (byte b = 0; b < pack.Length - 1; b++)
				crc ^= pack[b];
			pack[pack.Length - 1] = crc;
			port.Write(pack, 0, pack.Length);
		}

		public void ReadBytes(byte[] bytes) {
			for (int i = 0; i < bytes.Length; i++)
				bytes[i] = ReadByte();
		}

		public void Open() {
			port.Open();
			foreach(var thread in threads)
				thread.Start();

			started = true;
		}

		public void AddThread(Action<DiffBotSerial> action) {
			AddThread(() => action(this));
		}

		public void AddThread(Action action) {
			var thread = new Thread(() => action()) {
				IsBackground = true
			};
			threads.Add(thread);
			if(started)
				thread.Start();
		}

		public void Close() {
			if (close.IsCancellationRequested)
				return;

			close.Cancel();
			foreach (var thread in threads)
				if(thread.IsAlive)
					thread.Join();

			port.Close();
			close.Dispose();
		}
	}
}
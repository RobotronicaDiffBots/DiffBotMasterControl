using System;
using System.IO.Ports;
using System.Threading;

namespace DiffBotMasterControl
{
	public class DiffBotSerial
	{
		public readonly CancellationToken ct;
		public readonly SerialPort port;

		public DiffBotSerial(CancellationToken ct, SerialPort port) {
			this.ct = ct;
			this.port = port;
			port.ReadTimeout = 5;
		}

		public byte ReadByte() {
			while (true) {
				ct.ThrowIfCancellationRequested();
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
	}
}
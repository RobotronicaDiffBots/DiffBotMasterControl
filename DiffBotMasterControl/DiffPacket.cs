using System;

namespace DiffBotMasterControl
{
	public class DiffPacket
	{
		public byte[] bytes;
		public byte crc { get { return bytes[bytes.Length - 1]; } }

		public DiffPacket(int len) {
			bytes = new byte[len+1];
		}

		public bool CRC() {
			byte crc2 = 0xFF;
			for (int i = 0; i < bytes.Length - 1; i++)
				crc2 ^= bytes[i];

			return crc2 == crc;
		}

		public override string ToString() {
			return BitConverter.ToString(bytes);
		}
	}
}

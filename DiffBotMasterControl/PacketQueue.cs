using System;
using System.Collections.Generic;

namespace DiffBotMasterControl
{
	public class PacketQueue
	{
		public static void Add(IEnumerable<int> rIDs, byte type, byte d1, byte d2, byte d3, byte d4) {
			foreach(var rID in rIDs)
				Add((byte)rID, type, d1, d2, d3, d4);
		}

		public static void Add(byte rID, byte type, byte d1, byte d2, byte d3, byte d4) {

		}
	}
}

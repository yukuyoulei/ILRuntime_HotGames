using System;
using System.IO;

namespace LibPacket
{
	public abstract class PktBase
	{
		private MemoryStream _stream;
		protected MemoryStream stream
		{
			get
			{
				return _stream;
			}
			set
			{
				_stream = value;
				_reader = null;
				_writer = null;
			}
		}
		private BinaryReader _reader;
		protected BinaryReader reader
		{
			get
			{
				if (_reader == null)
					_reader = new BinaryReader(stream);
				return _reader;
			}
		}
		private BinaryWriter _writer;
		protected BinaryWriter writer
		{
			get
			{
				if (_writer == null)
					_writer = new BinaryWriter(stream);
				return _writer;
			}
		}
		public static int GetPktDef(Type pktType) { return CRC.CRC16(pktType.Name); }
		public int pktDef { get { return GetPktDef(GetType()); } }
		public abstract void Serialize(MemoryStream ms);
		public abstract void Deserialize(MemoryStream ms);
		public static T Deserialize<T>(byte[] bytes)
			where T : PktBase, new()
		{
			var t = new T();
			t.Deserialize(new MemoryStream(bytes));
			return t;
		}
	}
}
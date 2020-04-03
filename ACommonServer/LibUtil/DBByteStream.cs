using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

public class DBByteStream
{
	private static readonly int REALLOCATE_BUFFER_INC_SIZE = 1024;

	private Stream m_Stream;
	private byte[] m_Data;
	private int m_BytesRead;

	public byte[] Data
	{
		get { return m_Data; }
	}

	public Stream ContentStream
	{
		get { return m_Stream; }
	}

	//public int Position
	//{
	//    get { return Convert.ToInt32(m_Stream.Position); }
	//}

	//public int Length
	//{
	//    get { return Convert.ToInt32(m_Stream.Length); }
	//}

	//public int BytesRead
	//{
	//    get { return m_BytesRead; }
	//}

	public DBByteStream()
	{
		m_Data = new byte[REALLOCATE_BUFFER_INC_SIZE];
		m_Stream = new MemoryStream(m_Data, 0, REALLOCATE_BUFFER_INC_SIZE, true);
		//m_Stream.Position = 0;
		m_BytesRead = 0;
	}

	public byte ReadByte()
	{
		long iPos = m_Stream.Position;
		m_Stream.Position = m_BytesRead;
		byte[] vData = new byte[1];
		m_BytesRead += m_Stream.Read(vData, 0, 1);
		byte iValue = vData[0];
		m_Stream.Position = iPos;
		return iValue;
	}

	public string ReadString()
	{
		string sValue = string.Empty;
		Int16 iStringSize = ReadShort();
		if (0 < iStringSize)
		{
			long iPos = m_Stream.Position;
			m_Stream.Position = m_BytesRead;
			byte[] vData = new byte[iStringSize + 1];
			m_BytesRead += m_Stream.Read(vData, 0, iStringSize);
			sValue = Encoding.UTF8.GetString(vData, 0, iStringSize);
			m_Stream.Position = iPos;
		}

		return sValue;
	}

	public byte[] ReadBytesToEnd()
	{
		long iPos = m_Stream.Position;
		int iSize = (int)(m_Stream.Length - m_BytesRead);
		m_Stream.Position = m_BytesRead;
		byte[] vData = new byte[iSize];
		m_BytesRead += m_Stream.Read(vData, 0, iSize);
		m_Stream.Position = iPos;
		return vData;
	}

	public Guid ReadGuid()
	{
		return new Guid(ReadString());
	}

	public short ReadShort()
	{
		long iPos = m_Stream.Position;
		m_Stream.Position = m_BytesRead;
		byte[] vData = new byte[2];
		m_BytesRead += m_Stream.Read(vData, 0, 2);
		short iValue = BitConverter.ToInt16(vData, 0);
		m_Stream.Position = iPos;
		return iValue;
	}

	public float ReadFloat()
	{
		long iPos = m_Stream.Position;
		m_Stream.Position = m_BytesRead;
		byte[] vData = new byte[4];
		m_BytesRead += m_Stream.Read(vData, 0, 4);
		float fValue = BitConverter.ToSingle(vData, 0);
		m_Stream.Position = iPos;
		return fValue;
	}

	public int ReadInt()
	{
		long iPos = m_Stream.Position;
		m_Stream.Position = m_BytesRead;
		byte[] vData = new byte[4];
		m_BytesRead += m_Stream.Read(vData, 0, 4);
		int iValue = BitConverter.ToInt32(vData, 0);
		m_Stream.Position = iPos;
		return iValue;
	}

	public Int64 ReadInt64()
	{
		long iPos = m_Stream.Position;
		m_Stream.Position = m_BytesRead;
		byte[] vData = new byte[8];
		m_BytesRead += m_Stream.Read(vData, 0, 8);
		Int64 iValue = BitConverter.ToInt64(vData, 0);
		m_Stream.Position = iPos;
		return iValue;
	}

	public void WriteByte(byte value)
	{
		int iWriteSize = (int)(sizeof(byte) + m_Stream.Position - m_Stream.Length);
		if (iWriteSize > 0)
		{
			ReallocateBuffer(iWriteSize);
		}

		m_Stream.WriteByte(value);
	}

	public void WriteInt16(Int16 value)
	{
		byte[] buffer = new byte[2];
		Buffer.BlockCopy(BitConverter.GetBytes(value), 0, buffer, 0, 2);

		int iWriteSize = (int)(2 + m_Stream.Position - m_Stream.Length);
		if (iWriteSize > 0)
		{
			ReallocateBuffer(iWriteSize);
		}

		m_Stream.Write(buffer, 0, 2);
	}

	public void WriteInt(int value)
	{
		byte[] buffer = new byte[4];
		Buffer.BlockCopy(BitConverter.GetBytes(value), 0, buffer, 0, 4);

		int iWriteSize = (int)(4 + m_Stream.Position - m_Stream.Length);
		if (iWriteSize > 0)
		{
			ReallocateBuffer(iWriteSize);
		}

		m_Stream.Write(buffer, 0, 4);
	}

	public void WriteInt64(Int64 value)
	{
		byte[] buffer = new byte[8];
		Buffer.BlockCopy(BitConverter.GetBytes(value), 0, buffer, 0, 8);

		int iWriteSize = (int)(8 + m_Stream.Position - m_Stream.Length);
		if (iWriteSize > 0)
		{
			ReallocateBuffer(iWriteSize);
		}

		m_Stream.Write(buffer, 0, 8);
	}

	public void WriteFloat(float value)
	{
		byte[] vData = new byte[4];
		Buffer.BlockCopy(BitConverter.GetBytes(value), 0, vData, 0, 4);

		int iWriteSize = (int)(4 + m_Stream.Position - m_Stream.Length);
		if (iWriteSize > 0)
		{
			ReallocateBuffer(iWriteSize);
		}

		m_Stream.Write(vData, 0, 4);
	}

	public void WriteString(string sData)
	{
		UTF8Encoding utf8 = new UTF8Encoding();
		Byte[] encodedBytes = utf8.GetBytes(sData);

		if (encodedBytes.Length > 65535)
		{
			throw (new ArgumentException("String too Long >65535 bytes."));
		}

		WriteInt16((Int16)encodedBytes.Length);

		int iWriteSize = (int)(encodedBytes.Length + m_Stream.Position - m_Stream.Length);
		if (iWriteSize > 0)
		{
			ReallocateBuffer(iWriteSize);
		}

		m_Stream.Write(encodedBytes, 0, encodedBytes.Length);
	}

	public void WriteGuid(Guid guid)
	{
		WriteString(guid.ToString());
	}

	public void WriteByteArray(byte[] arrByte)
	{
		int iWriteSize = (int)(arrByte.Length + m_Stream.Position - m_Stream.Length);
		if (iWriteSize > 0)
		{
			ReallocateBuffer(iWriteSize);
		}
		m_Stream.Write(arrByte, 0, arrByte.Length);
	}

	public void ReallocateBuffer(int iSize)
	{
		if (iSize < REALLOCATE_BUFFER_INC_SIZE)
		{
			iSize = REALLOCATE_BUFFER_INC_SIZE;
		}

		int iLength = (int)(iSize + m_Stream.Length);
		byte[] vData = new byte[iLength];
		Buffer.BlockCopy(m_Data, 0, vData, 0, (int)(m_Stream.Position));
		m_Data = vData;
		int iPosition = (int)(m_Stream.Position);
		m_Stream = new MemoryStream(m_Data, 0, iLength, true);
		m_Stream.Position = iPosition;
	}

	public void Copy(DBByteStream dbStream)
	{
		int iLength = (int)dbStream.ContentStream.Length;
		int iPosition = (int)dbStream.ContentStream.Position;
		byte[] vData = new byte[iLength];
		Buffer.BlockCopy(dbStream.Data, 0, vData, 0, iPosition);
		m_Data = vData;
		m_Stream = new MemoryStream(m_Data, 0, iLength, true);
		m_Stream.Position = (int)dbStream.ContentStream.Position;
	}

}

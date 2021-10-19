using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace LibNet
{
    public class ByteStream
    {
        private static readonly int REALLOCATE_BUFFER_INC_SIZE = 256;

        protected Stream m_Stream;
        private byte[] m_Data;
		private int m_BytesRead;
        protected bool m_bReadOnly = false;

        public byte[] Data
        {
            get { return m_Data; }
        }

        public int Position
        {
            get { return Convert.ToInt32( m_Stream.Position ); }
        }

		public int Length
		{
			get { return Convert.ToInt32( m_Stream.Length ); }
		}

		public int BytesRead
		{
			get { return m_BytesRead; }
		}

        public ByteStream( int iLength, byte PacketType )
        {
			iLength += 5; // 2 bytes for MagicNum + 1 byte packet type + 2 bytes for packet size
            m_Data = new byte[ iLength ];
            m_Stream = new MemoryStream( m_Data, 0, iLength, true );
			m_BytesRead = 0;

            WriteByte(PacketType);
        }

        public ByteStream( Stream stream )
        {
            m_bReadOnly = true;
            m_Data = new byte[ 1024 ];
            m_Stream = stream;
			m_BytesRead = 0;
        }

        public bool CanReadBytes( uint iBytes )
		{
			return ( ( m_Stream.Length - m_Stream.Position ) >= iBytes );
		}

        public byte ReadByte()
        {
			m_BytesRead++;
            return (byte)m_Stream.ReadByte();
        }

        public string ReadString()
        {
            string sValue = string.Empty;
            Int16 iStringSize = ReadShort();
            if ( 0 < iStringSize )
            {
                byte[] vData = new byte[ iStringSize+1 ];
                m_BytesRead += m_Stream.Read( vData, 0, iStringSize );
                sValue = Encoding.UTF8.GetString( vData, 0, iStringSize );
            }

            return sValue;
        }
        public byte[] ReadBytesToEnd()
        {
            int iSize = this.Length - this.Position;
            byte[] vData = new byte[iSize];
            m_BytesRead += m_Stream.Read(vData, 0, iSize);
            return vData;
        }

        public Guid ReadGuid()
        {
            return new Guid( ReadString() );
        }

		public short ReadShort()
		{
            byte[] vData = new byte[2];
            m_BytesRead += m_Stream.Read( vData, 0, 2 );
            short iValue = BitConverter.ToInt16( vData, 0 );
			return iValue;
		}

		public float ReadFloat()
		{
			byte[] vData = new byte[4];
			m_BytesRead += m_Stream.Read( vData, 0, 4 );
			float fValue = BitConverter.ToSingle( vData, 0 );
			return fValue;
		}

        public int ReadInt()
        {
            byte[] vData = new byte[4];
            m_BytesRead += m_Stream.Read( vData, 0, 4 );
            int iValue = BitConverter.ToInt32( vData, 0 );
            return iValue;
        }

        public Int64 ReadInt64()
        {
            byte[] vData = new byte[ 8 ];
            m_BytesRead += m_Stream.Read( vData, 0, 8 );
            Int64 iValue = BitConverter.ToInt64( vData, 0 );
            return iValue;
        }

        public void WriteByte( byte value )
        {
            if ( m_bReadOnly )
            {
                throw ( new ArgumentException( "Read Only ByteStream had a write method called." ) );
            }

            int iWriteSize = (int)(sizeof(byte) + m_Stream.Position - m_Stream.Length);
            if ( iWriteSize > 0 )
            {
                ReallocateBuffer( iWriteSize );
            }

            m_Stream.WriteByte( value );
        }

        public void WriteInt16( Int16 value )
        {
            if ( m_bReadOnly )
            {
                throw ( new ArgumentException( "Read Only ByteStream had a write method called." ) );
            }
            byte[] buffer = new byte[ 2 ];
            Buffer.BlockCopy( BitConverter.GetBytes( value ), 0, buffer, 0, 2 );

            int iWriteSize = (int)(2 + m_Stream.Position - m_Stream.Length);
            if ( iWriteSize > 0 )
            {
                ReallocateBuffer(iWriteSize);
            }

            m_Stream.Write( buffer, 0, 2 );
        }

        public void WriteInt( int value )
        {
            if ( m_bReadOnly )
            {
                throw ( new ArgumentException( "Read Only ByteStream had a write method called." ) );
            }
            byte[] buffer = new byte[ 4 ];
            Buffer.BlockCopy( BitConverter.GetBytes( value ), 0, buffer, 0, 4 );

            int iWriteSize = (int)(4 + m_Stream.Position - m_Stream.Length);
            if ( iWriteSize > 0 )
            {
                ReallocateBuffer(iWriteSize);
            }

            m_Stream.Write( buffer, 0, 4 );
        }

        public void WriteInt64( Int64 value )
        {
            if ( m_bReadOnly )
            {
                throw ( new ArgumentException( "Read Only ByteStream had a write method called." ) );
            }
            byte[] buffer = new byte[ 8 ];
            Buffer.BlockCopy( BitConverter.GetBytes( value ), 0, buffer, 0, 8 );

            int iWriteSize = (int)( 8 + m_Stream.Position - m_Stream.Length );
            if ( iWriteSize > 0 )
            {
                ReallocateBuffer( iWriteSize );
            }

            m_Stream.Write( buffer, 0, 8 );
        }

		public void WriteFloat( float value )
		{
			if ( m_bReadOnly )
			{
                throw ( new ArgumentException( "Read Only ByteStream had a write method called." ) );
			}
			byte[] vData = new byte[ 4 ];
			Buffer.BlockCopy( BitConverter.GetBytes( value ), 0, vData, 0, 4 );

            int iWriteSize = (int)(4 + m_Stream.Position - m_Stream.Length);
            if ( iWriteSize > 0 )
            {
                ReallocateBuffer(iWriteSize);
            }

			m_Stream.Write( vData, 0, 4 );
		}

        public void WriteString( string sData )
        {
            if ( m_bReadOnly )
            {
                throw ( new ArgumentException( "Read Only ByteStream had a write method called." ) );
            }
            UTF8Encoding utf8 = new UTF8Encoding();
            Byte[] encodedBytes = utf8.GetBytes( sData );

            if (encodedBytes.Length>65535)
            {
                throw (new ArgumentException("String too Long >65535 bytes."));
            }

            WriteInt16( (Int16)encodedBytes.Length );

            int iWriteSize = (int)(encodedBytes.Length + m_Stream.Position - m_Stream.Length);
            if ( iWriteSize > 0 )
            {
                ReallocateBuffer(iWriteSize);
            }

            m_Stream.Write( encodedBytes, 0, encodedBytes.Length );
        }

        public void WriteGuid( Guid guid )
        {
            WriteString( guid.ToString() );
        }
        public void WriteByteArray(Byte[] arrByte)
        {
            int iWriteSize = (int)(arrByte.Length + m_Stream.Position - m_Stream.Length);
            if (iWriteSize > 0)
            {
                ReallocateBuffer(iWriteSize);
            }
            m_Stream.Write(arrByte, 0, arrByte.Length);
        }

        public void ReallocateBuffer( int iSize )
        {
            if ( iSize < REALLOCATE_BUFFER_INC_SIZE )
            {
                iSize = REALLOCATE_BUFFER_INC_SIZE;
            }

            int iLength = (int)(iSize + m_Stream.Length);
            byte[] vData = new byte[iLength];
            Buffer.BlockCopy( m_Data, 0, vData, 0, (int)(m_Stream.Position) );
            m_Data = vData;
            int iPosition = (int)(m_Stream.Position);
            m_Stream = new MemoryStream(m_Data, 0, iLength, true);
            m_Stream.Position = iPosition;
        }
    }
}

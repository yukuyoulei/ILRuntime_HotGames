using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace LibNet
{
    internal interface IPacketFactory
    {
        Packet Create( Stream stream, ClientConnection client );
    }

    internal class PacketFactory<T> : IPacketFactory where T : Packet, new()
    {
        public PacketFactory()
        {
        }

        #region IPacketFactory Members

        public Packet Create(Stream stream, ClientConnection client)
        {
            T ret = new T();
            ret.FactoryInit(new ByteStream(stream), client);
            return ret;
        }

        #endregion
    }

    internal class PacketFactory
    {
        static private PacketFactory s_Instance = null;
        private SortedList<eMessageType, IPacketFactory> m_Factories = new SortedList<eMessageType, IPacketFactory>();

        static public PacketFactory Instance
        {
            get
            {
                if ( null == s_Instance )
                {
                    s_Instance = new PacketFactory();
                }
                return s_Instance;
            }
        }

        private PacketFactory()
        {
            m_Factories.Add(eMessageType.PacketID_Common, new PacketFactory<RPCCommonPacket>());
        }

        public Packet ParseData( eMessageType eType, Stream stream, ClientConnection client )
        {
            if ( m_Factories.ContainsKey( eType ) )
            {
                return m_Factories[ eType ].Create( stream, client );
            }

            return new Packet();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Diagnostics;

namespace LibNet
{
    public static class ClientConnectionUtil
    {
        /// <summary>
        /// Processes an incoming packet to an ClientConnection interface
        /// </summary>
        /// <param name="messageType">Packet message identifier</param>
        /// <param name="packetStream">Packet stream data</param>
        /// <param name="client">Client interface to handle the packet</param>
        /// <returns>Packet to process or NULL if a control packet dispatched to the client</returns>
        public static Packet ProcessClientPacket( eMessageType messageType, Stream packetStream, ClientConnection client )
        {
            return ProcessClientPacket( messageType, packetStream, client, false );
        }

        /// <summary>
        /// Processes an incoming packet to an ClientConnection interface
        /// </summary>
        /// <param name="messageType">Packet message identifier</param>
        /// <param name="packetStream">Packet stream data</param>
        /// <param name="client">Client interface to handle the packet</param>
        /// <param name="bThrowOnError">True to throw an exception instead of logging</param>
        /// <returns>Packet to process or NULL if a control packet dispatched to the client</returns>
        public static Packet ProcessClientPacket( eMessageType messageType, Stream packetStream, ClientConnection client, bool bThrowOnError )
        {
           // string userName = ( client.ConnectionUser != null ) ? client.ConnectionUser.Login : "Unknown User";

            Packet packet = PacketFactory.Instance.ParseData( messageType, packetStream, client );
//             if( packet is LogoutPacket )
//             {
//                 client.SignalLogout();
//                 return null;
//             }
            //else 
            if( packet.IsValid )
            {
                return packet;
            }

            if ( bThrowOnError )
            {
                throw new InvalidOperationException( string.Format( @"Invalid Packet Data Received For Type '{0}'. {1}", messageType, "userName" ) );
            }
            else
            {
                AOutput.Log( string.Format( @"Invalid Packet Data Received For Type '{0}'. {1}", messageType, "userName" ) );
            }

            return null;
        }
    }
}

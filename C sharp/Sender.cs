using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;


namespace UDP_Connection
{

    class UDP_Sender
    {

        IPAddress remoteIP = null;
        IPEndPoint remoteIPEP = null;
        Socket mySocket = null;


		///// <summary>
		/// Obtains the IP address, and port number of the endpoin
		/// </summary>
		/// <param name="ip_address">Ip address.</param>
		/// <param name="port">Port.</param>
		public UDP_Sender(string ip_address, int port)
        {
            remoteIP = IPAddress.Parse(ip_address);
            remoteIPEP = new IPEndPoint(remoteIP, port);
            mySocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        }


		///<summary>
		///Sends messages of type Byte
		///</summary>
		public void sendMessage(byte[] msg)
		{
			mySocket.SendTo(msg, remoteIPEP);

		}

		///<summary>
		///Sends messages of type string
		///</summary>
		public void sendMessage(string msg)
        {
            mySocket.SendTo(Encoding.ASCII.GetBytes(msg), remoteIPEP);

        }


    }
}
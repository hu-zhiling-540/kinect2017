using System.Text;
using System.Net;
using System.Net.Sockets;
using System;

namespace Kinect_UDP_Sender
{

    class UDP_Sender
    {

        IPAddress remoteIP = null;
        IPEndPoint remoteIPEP = null;
        Socket mySocket = null;

        int upperLimit = 10000;


        /// <summary>
        /// Obtains the IP address, and port number of the endpoin
        /// </summary>
        public UDP_Sender(string hostName, int portNum)
        {
            remoteIP = IPAddress.Parse(hostName);
            remoteIPEP = new IPEndPoint(remoteIP, portNum);
            mySocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        }


        ///<summary>
        ///Sends messages of type Byte
        ///</summary>
        public void SendMessage(byte[] msg)
        {
            var maxData = new byte[upperLimit];
            int remaining = msg.Length;
            while(remaining > 0)
            {
                int size = Math.Min(remaining, upperLimit);
                Array.Copy(msg, msg.Length - remaining, maxData, 0, upperLimit);
                mySocket.SendTo(maxData, remoteIPEP);
                remaining -= size;
            }

        }

        ///<summary>
        ///Sends messages of type string
        ///</summary>
        public void SendMessage(string msg)
        {
            SendMessage(Encoding.ASCII.GetBytes(msg));
            //mySocket.SendTo(Encoding.ASCII.GetBytes(msg), remoteIPEP);
        }


    }
}
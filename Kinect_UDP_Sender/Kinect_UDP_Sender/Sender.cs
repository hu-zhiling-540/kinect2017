using System.Text;
using System.Net;
using System.Net.Sockets;

namespace Kinect_UDP_Sender
{

    class UDP_Sender
    {

        IPAddress remoteIP = null;
        IPEndPoint remoteIPEP = null;
        Socket mySocket = null;


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
            mySocket.SendTo(msg, remoteIPEP);
        }

        ///<summary>
        ///Sends messages of type string
        ///</summary>
        public void SendMessage(string msg)
        {
            mySocket.SendTo(Encoding.ASCII.GetBytes(msg), remoteIPEP);
        }


    }
}
using System.Text;
using System.Net;
using System.Net.Sockets;
using System;
using System.IO.Compression;
using System.IO;

namespace Kinect_UDP_Sender
{

    class UDP_Sender
    {

        IPAddress remoteIP = null;
        IPEndPoint remoteIPEP = null;
        Socket mySocket = null;

        int upperLimit = 64000;


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
        ///Sends messages of type string
        ///</summary>
        public void SendMessage(string msg)
        {
            //SendMessage(Encoding.ASCII.GetBytes(msg));
            mySocket.SendTo(Encoding.ASCII.GetBytes(msg), remoteIPEP);
        }


        public void SendMessage(byte[] msg, long timeStamp)
        {
            int offset = 0;
            int i = 0;
            int count = (int)Math.Ceiling((float)msg.Length / (float)upperLimit);
            while (offset < msg.Length)
            {
                int len = upperLimit;
                if (len + offset >= msg.Length)
                {
                    len = msg.Length - offset;
                }
                // data, timestamp, serial number, count
                byte[] packetData = new byte[len+ sizeof(long) + sizeof(int) + sizeof(int)];
                Buffer.BlockCopy(msg, offset, packetData, 0, len);
                Buffer.BlockCopy(msg, 0, packetData, len, sizeof(long));
                Buffer.BlockCopy(msg, 0, packetData, len + sizeof(long), sizeof(int));
                Buffer.BlockCopy(msg, 0, packetData, len + sizeof(long) + sizeof(int), sizeof(int));

                mySocket.SendTo(packetData, remoteIPEP);
                offset += len;
                i++;
            }
        }

        public byte[] Compress(byte[] bytes)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                GZipStream compress = new GZipStream(ms, CompressionMode.Compress);
                compress.Write(bytes, 0, bytes.Length);
                compress.Close();
                return ms.ToArray();
            }
        }

    }
}
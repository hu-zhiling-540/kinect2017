using System.Text;
using System.Net;
using System.Net.Sockets;
using System;
using System.Collections.Generic;
using System.Threading;

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
        ///Sends messages of type Byte
        ///</summary>
        //public void SendMessage(byte[] msg)
        //{
        //    var maxData = new byte[upperLimit];
        //    int remaining = msg.Length;
        //    while(remaining > 0)
        //    {
        //        int size = Math.Min(remaining, upperLimit);
        //        Array.Copy(msg, msg.Length - remaining, maxData, 0, upperLimit);
        //        mySocket.SendTo(maxData, remoteIPEP);
        //        remaining -= size;
        //    }

        //}

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
                byte[] packetData = new byte[len];
                Buffer.BlockCopy(msg, offset, packetData, 0, len);
                Packet packet = new Packet(timeStamp, i, count, packetData);
                mySocket.SendTo(packet.Serialize(), remoteIPEP);
                offset += len;
                i++;
            }
            /*
            return packets;
        }


            }

            // if the message exceeds over the maximum size
            if (msg.Length > upperLimit)
            {
                // count of packets to be divided into
                int count = msg.Length / upperLimit;
                // leftover message for the last packet
                int remainder = msg.Length % upperLimit;
                // if has remainder, add up one more packet
                if (remainder > 0)
                    count += 1;
                // check if more than once packet to be sent
                if (count > 1)
                {
                    //Console.WriteLine("sending");
                    ICollection<Packet> packets = SplitUpMsg(timeStamp, count, remainder, msg);
                    foreach (Packet packet in packets)
                    {
                        Console.WriteLine("hi");
                        Console.WriteLine(packet.Serialize());
                        mySocket.SendTo(packet.Serialize(), remoteIPEP);
                    }
                }
            }
            else
            {
                mySocket.SendTo(msg, remoteIPEP);
            }
        }*/
            /*
                public ICollection<Packet> SplitUpMsg(long timeStamp, int count, int remainder, byte[] msg)
                {
                    List<Packet> packets = new List<Packet>();

                    for (int i = 1; i <= count; i++)
                    {
                        byte[] packetData = new byte[upperLimit];
                        Buffer.BlockCopy(msg, (i - 1) * upperLimit, packetData, 0, upperLimit);
                        //Packet p = new Packet(timeStamp, i, count, packetData);
                        //Console.WriteLine("hi");
                        packets.Add(new Packet(timeStamp, i, count, packetData));
                    }

                    return packets;
                }
                */
        }

    }
}
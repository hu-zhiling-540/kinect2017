﻿using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Diagnostics;

namespace UDP_Connection
{

    class UDP_Receiver
    {

        // instances
        private IPEndPoint myIPEP = null;
        private EndPoint myEP = null;
        private Socket mySocket = null;
        private int myPort;

        bool isRunning = true;

        BlockingCollection<string> receivedMsgs = new BlockingCollection<string>(new ConcurrentQueue<string>());

		/// <summary>
		/// Obtains the port number of the endpoin.
		/// The receiver will receiver message regardless of IP address.
		/// </summary>
		/// <param name="port">Port.</param>
		public UDP_Receiver(int port)
        {
            // the IPAddress.Any address to use any network interface on the system
            myPort = port;
			myIPEP = new IPEndPoint(IPAddress.Any, port);
            myEP = (EndPoint)myIPEP;
            // specify the Dgram SocketType, Udp ProtocolType
            mySocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

            // to receive UDP data on a specific UDP port
            mySocket.Bind(myIPEP);      // will accept any incoming UDP packet on port
            mySocket.ReceiveTimeout = 500;
        }

        public void start()
        {
            Thread newThread = new Thread(new ThreadStart(this.onReceive));
            newThread.Start();
        }

        public void stop()
        {
            isRunning = false;
            mySocket.Dispose();
        }

        public BlockingCollection<string> getMsgQueue()
        {
            return receivedMsgs;
        }

        // Receive data from Client
        public void onReceive()
        {
            // not specify the ip address of the devices sending me the packets
            IPEndPoint sender = new IPEndPoint(IPAddress.Any, 0);
            EndPoint remote = (EndPoint)(sender);

            byte[] result = new byte[1024];
            int msgLength;

            while (isRunning)
            {
                try
                {
                    msgLength = mySocket.ReceiveFrom(result, ref remote);
                    string decodedMsg = Encoding.ASCII.GetString(result, 0, msgLength);
                    receivedMsgs.Add(decodedMsg);
                }
                catch (SocketException s)
                {
                   
                }
            }
        }

    }
}

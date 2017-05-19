using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Diagnostics;

namespace UDP_Connection  {

	class UDP_Receiver   {

		// instances
		IPEndPoint myIPEP = null;
		EndPoint myEP = null;
		Socket mySocket = null;

        ConcurrentQueue<string> recievedPackets = new ConcurrentQueue<string>();
        private int myPort;


		///<summary>
		///Obtain the port number of the endpoin
		/// </summary>
		public UDP_Receiver(int port)     {

            //look at making the receiver receive any UDP msg send to 
            // this port regardless of IP address 

            // Creates an IPEndPoint to record the IP Address and port number of the sender. 
            // allows you to read datagrams sent from any source.
            // the IPAddress.Any address to use any network interface on the system
            myPort = port;
			myIPEP = new IPEndPoint(IPAddress.Any, port);       
			myEP = (EndPoint)myIPEP;
            // specify the Dgram SocketType, Udp ProtocolType
			mySocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

			// if doesn't need to receive UDP data on a specific UDP port, don't have to bind the socket to a specific IPEndPoint
			// binds it to a set IPEndPoint object so it can wait for incoming packets
            mySocket.Bind(myIPEP);      // will accept any incoming UDP packet on port

		}

        public void start() {
            //ThreadStart receiverStart = new ThreadStart(this.receiveBroadcast);
            //Thread receiverThread = new Thread(receiverStart);
			Thread newThread =new Thread(new ThreadStart(this.receiveBroadcast));
			newThread.Start();
		}

        public void stop() {
            mySocket.Shutdown(SocketShutdown.Both);   
        }

        public ConcurrentQueue<string> getMsg()  {
            return recievedPackets;
        }

		// Receive data from Client
		public void receiveBroadcast()   {

			// not specify the ip address of the devices sending me the packets
			// dummy end-point
			IPEndPoint sender = new IPEndPoint(IPAddress.Any, 0);
			EndPoint remote = (EndPoint)(sender);

			byte[] result = new byte[1024];
            int msgLength;

			while (true)    {
                msgLength = mySocket.ReceiveFrom(result, ref remote);
                string decodedMsg = Encoding.ASCII.GetString(result, 0, msgLength);
				//TOOD: remove this in the future
				Console.WriteLine("Message Received: {0}",decodedMsg);

                if (decodedMsg == "exit" || decodedMsg == "quit")
                    stop();

			}
		}
	}
}

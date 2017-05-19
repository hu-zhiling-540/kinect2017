using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;
using System.Collections.Generic;
using System.Collections.Concurrent;

namespace UDP_Connection  {

	class UDP_Receiver   {

		// instances
		IPAddress myIP = null;      // the IP address
		IPEndPoint myIPEP = null;
		EndPoint myEP = null;
		Socket mySocket = null;

        ConcurrentQueue<string> recievedPackets = new ConcurrentQueue<string>();


		//Obtain the IP address, and port number of the endpoin
        public UDP_Receiver(int port)     {

			//look at making the receiver receive any UDP msg send to 
			// this port regardless of IP address 

			// Creates an IPEndPoint to record the IP Address and port number of the sender. 
			// allows you to read datagrams sent from any source.
			// the IPAddress.Any address to use any network interface on the system
			myIPEP = new IPEndPoint(IPAddress.Any, port);       
			myEP = (EndPoint)myIPEP;
            // specify the Dgram SocketType, Udp ProtocolType
			mySocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

			// if doesn't need to receive UDP data on a specific UDP port, don't have to bind the socket to a specific IPEndPoint
			// binds it to a set IPEndPoint object so it can wait for incoming packets
            mySocket.Bind(myIPEP);      // will accept any incoming UDP packet on port


			//TOOD: remove this in the future
			Console.WriteLine("This is a Receiver, IP address is {0}, " +
                              "host name is {1}", myIP.ToString(), Dns.GetHostName());
			Console.WriteLine("Receiver is waiting for a message");
		}

        public void start() {
            ThreadStart testThread1Start = new ThreadStart(this.receiveData);
		}

        public void stop() {
            mySocket.Shutdown(SocketShutdown.Both);   
        }

        public ConcurrentQueue<string> getMsg()  {
            return recievedPackets;
        }

		// Receive data from Client
		public void receiveData()   {
            
			// not specify the ip address of the devices sending me the packets
			IPEndPoint sender = new IPEndPoint(IPAddress.Any, 0);
			EndPoint remote = (EndPoint)(sender);

			byte[] result = new byte[1024];
            int msgLength;

			while (true)    {
                msgLength = mySocket.ReceiveFrom(result, ref remote);
                string decodedMsg = Encoding.ASCII.GetString(result, 0, msgLength);
				//TOOD: remove this in the future
				Console.WriteLine("Message Received: {0}",decodedMsg);
                //messageProcessor(decodedMsg);

                if (decodedMsg == "exit" || decodedMsg == "quit")
                    break;

			}
		}
	}
}

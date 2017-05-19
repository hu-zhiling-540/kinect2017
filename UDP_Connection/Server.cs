using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;
using System.Collections.Generic;
using System.Collections.Concurrent;

namespace UDP_Connection  {

	class UDP_Receiver   {

        public delegate void ProcessMessage(String msg);
		ProcessMessage messageProcessor;

		// instances
		IPAddress myIP = null;      // the IP address
		IPEndPoint myIPEP = null;
		EndPoint myEP = null;
		Socket mySocket = null;
        ConcurrentQueue<string> recievedPackets = new ConcurrentQueue<string>();


		//Obtain the IP address, and port number of the endpoin
        public UDP_Receiver(string ip_address, int port, ProcessMessage messageProcessor)     {
            //look at making the receiver receive any UDP msg send to 
            // this port regardless of IP address 
			myIP = IPAddress.Parse(ip_address);
			myIPEP = new IPEndPoint(myIP, port);
			myEP = (EndPoint)myIPEP;
			mySocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
           
            mySocket.Bind(myIPEP);

            Console.WriteLine("This is a Receiver, IP address is {0}, " +
                              "host name is {1}", myIP.ToString(), Dns.GetHostName());
			Console.WriteLine("Receiver is waiting for a message");
		}


            // two threads will be called for execution in parallel
   //         Thread thread = new Thread(receiveData);     // thread for receiving messages
			//thread.Start();
   //         Thread thread2 = new Thread(Sender);       // thread for sending message
			//thread2.Start();

        // Sending information to Client
		//public void sendData()   {
		//	//EndPoint point = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8001);
  //          IPEndPoint sender = new IPEndPoint(IPAddress.Any, 0);
		//	while (true)    {
		//		string msg = Console.ReadLine();
  //              mySocket.SendTo(Encoding.UTF8.GetBytes(msg), sender);
		//	}
		//}


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
            

			//save the ip address and port of Client
			IPEndPoint sender = new IPEndPoint(IPAddress.Any, 0);
			EndPoint remote = (EndPoint)(sender);


			byte[] result = new byte[1024];
            int msgLength;
//			int length = mySocket.ReceiveFrom(result, ref remote);

  //          Console.WriteLine("Message received from {0} .", remote.ToString());
	//		Console.WriteLine("Message received: {0} ", Encoding.ASCII.GetString(result, 0, length));

//			result = Encoding.ASCII.GetBytes("Connected.");
  //          mySocket.SendTo(result, result.Length, SocketFlags.None, remote);

			while (true)    {
                // refresh
                msgLength = mySocket.ReceiveFrom(result, ref remote);
                //TOOD: remove this in the future
                string decodedMsg = Encoding.ASCII.GetString(result, 0, msgLength);
                Console.WriteLine("Message Received: {0}",decodedMsg);
                messageProcessor(decodedMsg);

                // pass back the message I receive from Client
//                mySocket.SendTo(result, result.Length, SocketFlags.None, remote);
			}
		}
	}
}

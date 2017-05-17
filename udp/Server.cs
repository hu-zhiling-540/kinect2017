﻿using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;



namespace UDP_Receiver  {
	class Program   {
		
        static Socket serverSocket;
		//ip address  
		static IPAddress ip = IPAddress.Parse("127.0.0.1");
		static IPEndPoint ip_server = new IPEndPoint(ip, myPort);
		static EndPoint local = (EndPoint)ip_server;
		static int myPort = 8001;   //port

		static void Main(string[] args)     {
        	serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            serverSocket.Bind(ip_server);
			//serverSocket.Listen(10);    //at most 10 connection awaiting  

            Console.WriteLine("Message received from {0}: ", local.ToString());
			Console.WriteLine("This is a Server, host name is {0}", Dns.GetHostName());

			Console.WriteLine("Waiting for a client");

            // two threads will be called for execution in parallel
			Thread thread = new Thread(Receiver);     // thread for receiving messages
			thread.Start();
   //         Thread thread2 = new Thread(Sender);       // thread for sending message
			//thread2.Start();
		}

		//      // Sending information to Client
		//static void Sender(d)
		//{
		//	//EndPoint point = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8001);
		//	while (true)
		//	{
		//		string msg = Console.ReadLine();
		//              serverSocket.SendTo(Encoding.UTF8.GetBytes(msg), remote);
		//	}


		//}

		// Receive data from Client
		static void Receiver()
		{
            byte[] result = new byte[1024];
			//save the ip address and port of Client
			IPEndPoint sender = new IPEndPoint(IPAddress.Any, 0);
			EndPoint remote = (EndPoint)(sender);
			int length = serverSocket.ReceiveFrom(result, ref remote);
			Console.WriteLine("Message received from {0}: ", remote.ToString());
			Console.WriteLine(Encoding.ASCII.GetString(result, 0, length));

			// when connect succesfully
			string welcome = "Hello!";

			result = Encoding.ASCII.GetBytes(welcome);
			serverSocket.SendTo(result, result.Length, SocketFlags.None, remote);

			while (true)
			{
                // refresh
                result = new byte[1024];
                length = serverSocket.ReceiveFrom(result, ref remote);
                Console.WriteLine(Encoding.ASCII.GetString(result, 0, length));
                serverSocket.SendTo(result, result.Length, SocketFlags.None, remote);
			}
		}
	}
}

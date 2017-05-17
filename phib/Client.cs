using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace UDP_Sender      {
	class Program   {

		static Socket clientSocket;
		//ip address  
		static IPAddress ip = IPAddress.Parse("127.0.0.1");
		static IPEndPoint ip_client = new IPEndPoint(ip, myPort);
		static EndPoint remote = (EndPoint)ip_client;
		static int myPort = 8001;   //port

		static void Main(string[] args)     {
			clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            //clientSocket.Bind(ip_client);

            Thread thread = new Thread(Sender);
			thread.Start();
   //       Thread thread2 = new Thread(Receiver);
			//thread2.Start();

		}

		// Sending information to Server
		static void Sender()    {

            byte[] data = new byte[1024];
			string welcome = "Client up. ";
            data = Encoding.ASCII.GetBytes(welcome);
            clientSocket.SendTo(data, data.Length, SocketFlags.None, ip_client);

			IPEndPoint sender = new IPEndPoint(IPAddress.Any, 0);
			EndPoint remote = (EndPoint)sender;

			data = new byte[1024];

            int recv = clientSocket.ReceiveFrom(data, ref remote);
            Console.WriteLine("Message received from {0}: ", remote.ToString());
			Console.WriteLine(Encoding.ASCII.GetString(data, 0, recv));


			while (true)    {
				string msg = Console.ReadLine();
                if (msg == "exit")
					break;
                clientSocket.SendTo(Encoding.ASCII.GetBytes(msg), remote);
				data = new byte[1024];
                recv = clientSocket.ReceiveFrom(data, ref remote);
				string result = Encoding.ASCII.GetString(data, 0, recv);
                Console.WriteLine(result);

				
				//clientSocket.SendTo(Encoding.UTF8.GetBytes(msg), remote);
			}
			Console.WriteLine("Stopping Client.");
            clientSocket.Shutdown(SocketShutdown.Both);
            //clientSocket.Close();


		}

		//// Sending information from Client
		//static void Receiver()  {
		//	while (true)    {
		//		//to save the ip address and port for the destination
		//		EndPoint point = new IPEndPoint(IPAddress.Any, 0);
		//		byte[] result = new byte[1024];
  //              int length = clientSocket.ReceiveFrom(result, ref remote);
  //              string msg = Encoding.UTF8.GetString(result, 0, length);
  //              Console.WriteLine(point.ToString() + msg);
		//	}
		//}

	}
}
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;


namespace UDP_Connection.UDP_Receiver  {
	class Program   {


		IPAddress myIP = null;      // the IP address
		IPEndPoint myIPEP = null;
		EndPoint myEP = null;
		Socket mySocket = null;

		//Obtain the IP address, and port number of the endpoin
		public Program(String ip_address, int port)
		{
			myIP = IPAddress.Parse(ip_address);
			myIPEP = new IPEndPoint(myIP, port);
			myEP = (EndPoint)myIPEP;
			mySocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
           
            mySocket.Bind(myIPEP);

			Console.WriteLine("Message received from {0}: ", local.ToString());
			Console.WriteLine("This is a Server, host name is {0}", Dns.GetHostName());

			Console.WriteLine("Waiting for a client");
		}


            // two threads will be called for execution in parallel
   //         Thread thread = new Thread(receiveData);     // thread for receiving messages
			//thread.Start();
   //         Thread thread2 = new Thread(Sender);       // thread for sending message
			//thread2.Start();

        // Sending information to Client
		public void sendData(string data)   {
			//EndPoint point = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8001);
			while (true)    {
				//string msg = Console.ReadLine();
                mySocket.SendTo(Encoding.UTF8.GetBytes(data), myEP);
			}
		}

		// Receive data from Client
        public void receiveData()
		{
            byte[] result = new byte[1024];
			//save the ip address and port of Client
			IPEndPoint sender = new IPEndPoint(IPAddress.Any, 0);
			EndPoint remote = (EndPoint)(sender);
            int length = mySocket.ReceiveFrom(result, ref remote);
			Console.WriteLine("Message received from {0}: ", remote.ToString());
			Console.WriteLine(Encoding.ASCII.GetString(result, 0, length));

			// when connect succesfully
			string welcome = "Hello!";

			result = Encoding.ASCII.GetBytes(welcome);
            mySocket.SendTo(result, result.Length, SocketFlags.None, remote);

			while (true)
			{
                // refresh
                result = new byte[1024];
                length = mySocket.ReceiveFrom(result, ref remote);
                Console.WriteLine(Encoding.ASCII.GetString(result, 0, length));
                mySocket.SendTo(result, result.Length, SocketFlags.None, remote);
			}
		}
	}
}

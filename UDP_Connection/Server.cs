using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;


namespace UDP_Connection.UDP_Receiver  {
    
	class Program   {

        // instances
		IPAddress myIP = null;      // the IP address
		IPEndPoint myIPEP = null;
		EndPoint myEP = null;
		Socket mySocket = null;

		//Obtain the IP address, and port number of the endpoin
		public Program(String ip_address, int port)     {
            
			myIP = IPAddress.Parse(ip_address);
			myIPEP = new IPEndPoint(myIP, port);
			myEP = (EndPoint)myIPEP;
			mySocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
           
            mySocket.Bind(myIPEP);

            Console.WriteLine("This is a Server, IP address is {0}, " +
                              "host name is {1}", myIP.ToString(), Dns.GetHostName());
			Console.WriteLine("Server is waiting for a client");
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


		// Receive data from Client
        public void receiveData()   {
            
            byte[] result = new byte[1024];

			//save the ip address and port of Client
			IPEndPoint sender = new IPEndPoint(IPAddress.Any, 0);
			EndPoint remote = (EndPoint)(sender);

            int length = mySocket.ReceiveFrom(result, ref remote);
			Console.WriteLine("Message received from {0} .", remote.ToString());
			Console.WriteLine("Message received: {0} ", Encoding.ASCII.GetString(result, 0, length));

			result = Encoding.ASCII.GetBytes("Connected.");
            mySocket.SendTo(result, result.Length, SocketFlags.None, remote);

			while (true)    {
                // refresh
                result = new byte[1024];
                length = mySocket.ReceiveFrom(result, ref remote);
                Console.WriteLine("Message Received: {0}", Encoding.ASCII.GetString(result, 0, length));

                // pass back the message I receive from Client
                mySocket.SendTo(result, result.Length, SocketFlags.None, remote);
			}
		}
	}
}

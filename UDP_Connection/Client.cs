using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;

//namespace UDP_Connection
//{
namespace UDP_Connection.UDP_Sender
{
    class Program   {
        
        IPAddress myIP = null;      // the IP address
        IPEndPoint myIPEP = null;
        EndPoint myEP = null;          
        Socket mySocket = null;

		//Obtain the IP address, and port number of the endpoin
		public Program(string ip_address, int port)   {
            myIP = IPAddress.Parse(ip_address);
            myIPEP = new IPEndPoint(myIP, port);
            myEP = (EndPoint)myIPEP;
            mySocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
			
            Thread thread = new Thread(sendData);
			thread.Start();
			//Thread thread2 = new Thread(Receiver);
			//thread2.Start();
		}

       
        //ip address  
        //static 
        //static IPEndPoint ip_client = new IPEndPoint(ip, myPort);
        //static EndPoint remote = (EndPoint)ip_client;
        //static int myPort = 8001;   //port


        // Sending information to Server
        public void sendData()
        {

            byte[] data = new byte[1024];
            string welcome = "Client is here. ";
            data = Encoding.ASCII.GetBytes(welcome);
            mySocket.SendTo(data, data.Length, SocketFlags.None, myIPEP);

            IPEndPoint sender = new IPEndPoint(IPAddress.Any, 0);
            EndPoint remote = (EndPoint)sender;

            data = new byte[1024];

            int recv = mySocket.ReceiveFrom(data, ref remote);
            Console.WriteLine("Message received from {0}: ", remote.ToString());
            Console.WriteLine(Encoding.ASCII.GetString(data, 0, recv));


            while (true)    {
                string msg = Console.ReadLine();
                if (msg == "exit")
                    break;
                mySocket.SendTo(Encoding.ASCII.GetBytes(msg), remote);
                data = new byte[1024];
                recv = mySocket.ReceiveFrom(data, ref remote);
                string result = Encoding.ASCII.GetString(data, 0, recv);
                Console.WriteLine(result);


                //clientSocket.SendTo(Encoding.UTF8.GetBytes(msg), remote);
            }
            Console.WriteLine("Stopping Client.");
            mySocket.Shutdown(SocketShutdown.Both);
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
//}
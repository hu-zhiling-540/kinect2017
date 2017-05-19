using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;


namespace UDP_Connection    {
    
    class UDP_Sender   {
        
        IPAddress myIP = null;      // the IP address
        IPEndPoint myIPEP = null;
        EndPoint myEP = null;          
        Socket mySocket = null;

        //Obtain the IP address, and port number of the endpoin
        public UDP_Sender(string ip_address, int port)  {
            myIP = IPAddress.Parse(ip_address);
            myIPEP = new IPEndPoint(myIP, port);
            myEP = (EndPoint)myIPEP;
            mySocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

            //         Thread thread = new Thread(sendData);
            //thread.Start();
            //Thread thread2 = new Thread(Receiver);
            //thread2.Start();
        }



        // Sending information to Server
        public void sendData()  {

			IPEndPoint receiver = new IPEndPoint(IPAddress.Any, 0);
			EndPoint remote = (EndPoint)receiver;

			Console.WriteLine("Message sent to {0} .", remote.ToString());

			string welcome = "Client is here. ";
			byte[] data = new byte[1024];
			data = Encoding.ASCII.GetBytes(welcome);            
			mySocket.SendTo(data, data.Length, SocketFlags.None, myIPEP);

            // print out the message that been sent back to receiver
			int recv = mySocket.ReceiveFrom(data, ref remote);
			Console.WriteLine("Client is receiving message from {0} .", remote.ToString());
			Console.WriteLine(Encoding.ASCII.GetString(data, 0, recv));

			while (true)    {
			    string msg = Console.ReadLine();
			    if (msg == "exit" || msg == "quit")
			        break;
			    mySocket.SendTo(Encoding.ASCII.GetBytes(msg), remote);
			    data = new byte[1024];
                // get back the message I sent
			    recv = mySocket.ReceiveFrom(data, ref remote);  // bounce back
			    string result = Encoding.ASCII.GetString(data, 0, recv);
                Console.WriteLine("Message Sent: {0}", result);
			}
			Console.WriteLine("Stopping Client.");
			mySocket.Shutdown(SocketShutdown.Both);
            //mySocket.Close();


		}

        // Sending information from Client
   //     public void readReceipt()  {
			//byte[] result = new byte[1024];

			////save the ip address and port of Client
			//IPEndPoint receiver = new IPEndPoint(IPAddress.Any, 0);
    //        EndPoint remote = (EndPoint)(receiver);

    //    	while (true)    {
    //    		//to save the ip address and port for the destination
    //    		EndPoint point = new IPEndPoint(IPAddress.Any, 0);
    //    		result = new byte[1024];
				//int length = mySocket.ReceiveFrom(result, ref remote);
				//string msg = Encoding.UTF8.GetString(result, 0, length);
				//Console.WriteLine(point.ToString() + msg);
        //	}
        //}

    }
}
//}
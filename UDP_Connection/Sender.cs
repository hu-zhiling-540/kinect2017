using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;


namespace UDP_Connection    {
    
    class UDP_Sender   {
        
	    IPAddress remoteIP = null;      // the IP address
	    IPEndPoint remoteIPEP = null;     
	    Socket mySocket = null;


	    //Obtain the IP address, and port number of the endpoin
	    public UDP_Sender(string ip_address, int port)  {
	        remoteIP = IPAddress.Parse(ip_address);
	        remoteIPEP = new IPEndPoint(remoteIP, port);
	        mySocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
	    }

		///<summary>
		///Will start a thread
		///</summary>
		public void start() {
            Thread newThread = new Thread(new ThreadStart(this.sendData));
			newThread.Start();
        }

		///<summary>
		///Disable both sending and receiving on this Socket.
		///</summary>
		public void stop()  {
			mySocket.Shutdown(SocketShutdown.Both);
		}

	    // Sending information to Server
	    public void sendData()  {

			while (true)    {
			    string msg = Console.ReadLine();
			    if (msg == "exit" || msg == "quit")
			        break;
			    mySocket.SendTo(Encoding.ASCII.GetBytes(msg), remoteIPEP);
	            Console.WriteLine("Message Sent: {0}", msg);
			}
			Console.WriteLine("Stopping Client.");
            stop();

		}

    }
}
//}
using System;
using System.Net;
using System.Threading;

namespace UDP_Connection    {
    class Program   {
		//ip address  
		static IPAddress ip = IPAddress.Parse("127.0.0.1");
		static IPEndPoint ip_server = new IPEndPoint(ip, myPort);
		static EndPoint local = (EndPoint)ip_server;
		static int myPort = 8001;   //port
		
        static void Main(string[] args)     {

			ThreadStart testThread1Start = new ThreadStart(new UDP_Receiver.Program().receiveData);
            ThreadStart testThread2Start = new ThreadStart(new UDP_Sender.Program().sendData);

			Thread[] testThread = new Thread[2];
			testThread[0] = new Thread(testThread1Start);
			testThread[1] = new Thread(testThread2Start);

			foreach (Thread myThread in testThread)
			{
				myThread.Start();
			}

		}
    }
}

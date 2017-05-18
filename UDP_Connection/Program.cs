using System;
using System.Net;
using System.Threading;

namespace UDP_Connection    {
    class Program   {
        
        //ip address  
        static string ip_address = "127.0.0.1";
		static int port = 8008;   //port
		
        static void Main(string[] args)     {

            ThreadStart testThread1Start = new ThreadStart(new UDP_Receiver.Program(ip_address, port).receiveData);
            ThreadStart testThread2Start = new ThreadStart(new UDP_Sender.Program(ip_address, port).sendData);

			Thread[] testThread = new Thread[2];
			testThread[0] = new Thread(testThread1Start);
			testThread[1] = new Thread(testThread2Start);

			foreach (Thread myThread in testThread)     {
				myThread.Start();
			}

		}
    }
}

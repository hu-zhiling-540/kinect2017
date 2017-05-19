using System;
using System.Collections.Concurrent;
using System.Net;
using System.Threading;

namespace UDP_Connection    {
    class Program   {
        
        //ip address  
        static string ip_address = "127.0.0.1";
		static int port = 8008;   //port


        //bool running = true;

        //public static void echoMessage(string msg) {
        //    Console.WriteLine("Echo " + msg);
        //}


   //     public void receiverThread() {
			
			//ConcurrentQueue<string> q = receiver.getMsg();
        //    while(running){
                
        //        Console.WriteLine("Echo "  + q.
        //    }
            
        //}

        static void Main(string[] args)     {


            UDP_Receiver receiver = new UDP_Receiver(port);
            UDP_Sender sender = new UDP_Sender(ip_address, port);
            receiver.start();
            sender.start();

		}
    }


	// a value type that is typically used to 
    // encapsulate small groups of related variables
	//struct Data     {
	//	int length;
	//	byte[] data;

	//	public Data(int length, byte[] data)    {
 //           this.length = length;
	//		this.data = data;
	//	}
	//}
}

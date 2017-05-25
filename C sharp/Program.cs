using System;
using System.IO;
using SimpleJSON;

namespace UDP_Connection
{

    class Program
    {

        //static string ip_address = "127.0.0.1";
        //static int port = 8008;

       // static UDP_Receiver receiver = new UDP_Receiver(port);


        static public bool isRunning = true;

        /// <summary>
        /// Starts the receiver thread.
        /// </summary>
   //     public static void startReceiverThread()
   //     {
   //         Thread newThread = new Thread(ReceiverThread);
			//newThread.Start();
        //}

        /// <summary>
        /// Takes out message if any on the queue.
        /// </summary>
       

		//public static void ReceiverThread()
		//{
		//	string msg;
		//	BlockingCollection<string> msgs;

		//	msgs = receiver.getMsgQueue();

		//	while (isRunning)
		//	{
		//		if (msgs.TryTake(out msg, 1000))
		//		{
		//			Console.WriteLine("Message Received: {0}", msg);
		//		}
		//	}
		//}


        static void Main(string[] args)
        {
            string ipAddress = null;
            int port = -1;
            //receiver.start();
            //startReceiverThread();

            // json file path as command line argument
            if (args.Length >= 0)
            {
                //new StreamReader(path)
                string json_string = null;
                using (var fs = new FileStream(args[0], FileMode.Open, FileAccess.Read))
                {
                    using (var sr = new System.IO.StreamReader(fs))
                    {
                        json_string = sr.ReadToEnd();
                    }
                }
                var pref = JSON.Parse(json_string);
                port = pref["port"].AsInt;                 // versionString will be a int containing "8008"
                ipAddress = pref["ip address"].Value;      // will be a string containing "127.0.0.1"
            }

            else if (args.Length == 2)
            {
                port = Int32.Parse(args[0]);
                ipAddress = args[1];
            }

            else
                Console.WriteLine("Invalid arguments");

            UDP_Sender sender = new UDP_Sender(ipAddress, port);

            while (true)
            {
                string msg = Console.ReadLine();
                if (msg == "exit" || msg == "quit")
                    break;
                sender.sendMessage(msg);
                Console.WriteLine("Message Sent: {0}", msg);
            }
            Console.WriteLine("Stopping Client.");

//            receiver.stop();
              //isRunning = false;
  //          receiver.getMsgQueue().Add(null);

            Console.WriteLine("Everything is done!");

        }

    }



}

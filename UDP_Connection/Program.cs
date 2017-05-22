using System;
using System.Collections.Concurrent;
using System.Net;
using System.Threading;

namespace UDP_Connection
{

    class Program
    {

        static string ip_address = "127.0.0.1";
        static int port = 8008;

        static UDP_Receiver receiver = new UDP_Receiver(port);
        static UDP_Sender sender = new UDP_Sender(ip_address, port);

        static public bool isRunning = true;

        /// <summary>
        /// Starts the receiver thread.
        /// </summary>
        public static void startReceiverThread()
        {
            Thread newThread = new Thread(ReceiverThread);
			newThread.Start();
        }

        /// <summary>
        /// Takes out message if any on the queue.
        /// </summary>
		public static void ReceiverThread()
		{
			string msg;
			BlockingCollection<string> msgs;

			msgs = receiver.getMsgQueue();

			while (isRunning)
			{
				if (msgs.TryTake(out msg, 1000))
				{
					Console.WriteLine("Message Received: {0}", msg);
				}
			}
		}


        static void Main(string[] args)
        {
            receiver.start();
            startReceiverThread();

            while (true)
            {
                string msg = Console.ReadLine();
                if (msg == "exit" || msg == "quit")
                    break;
                sender.sendMessage(msg);
                Console.WriteLine("Message Sent: {0}", msg);
            }
            Console.WriteLine("Stopping Client.");

            receiver.stop();
            isRunning = false;
            receiver.getMsgQueue().Add(null);

            Console.WriteLine("Everything is done!");

        }

    }



}

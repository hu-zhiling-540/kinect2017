using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;
using System.Collections.Concurrent;

namespace Kinect_UDP_Sender
{

    class UDP_Receiver
    {

        // instances
        private IPEndPoint myIPEP = null;
        private EndPoint myEP = null;
        private Socket mySocket = null;
        private int myPort;

        bool isRunning = true;

        BlockingCollection<string> receivedMsgs = new BlockingCollection<string>(new ConcurrentQueue<string>());

		/// <summary>
		/// Obtains the port number of the endpoin.
		/// The receiver will receiver message regardless of IP address.
		/// </summary>
		/// <param name="port">Port.</param>
		public UDP_Receiver(int port)
        {
            // the IPAddress.Any address to use any network interface on the system
            myPort = port;
			myIPEP = new IPEndPoint(IPAddress.Any, port);
            myEP = (EndPoint)myIPEP;
            // specify the Dgram SocketType, Udp ProtocolType
            mySocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

            // to receive UDP data on a specific UDP port
            mySocket.Bind(myIPEP);      // will accept any incoming UDP packet on port
            mySocket.ReceiveTimeout = 500;
        }

        /// <summary>
        /// Starts the thread for Receiver.
        /// </summary>
        public void Start()
        {
            Thread newThread = new Thread(new ThreadStart(this.OnReceive));
            newThread.Start();
        }

        /// <summary>
        /// Stops the thread for Receiver
        /// </summary>
        public void Stop()
        {
            isRunning = false;
            mySocket.Dispose();     //finished using the Socket
        }

        /// <summary>
        /// Gets the message queue.
        /// </summary>
        /// <returns>The message queue.</returns>
        public BlockingCollection<string> GetMsgQueue()
        {
            return receivedMsgs;
        }

        /// <summary>
        /// When the Receiver is on, keep accepting message 
        /// and store them in the queue 
        /// </summary>
        public void OnReceive()
        {
            // not specify the ip address of the devices sending me the packets
            IPEndPoint sender = new IPEndPoint(IPAddress.Any, 0);
            EndPoint remote = (EndPoint)(sender);

            byte[] result = new byte[1024];
            int msgLength;

            while (isRunning)
            {
                try
                {
                    msgLength = mySocket.ReceiveFrom(result, ref remote);
                    string decodedMsg = Encoding.ASCII.GetString(result, 0, msgLength);
                    receivedMsgs.Add(decodedMsg);
                }
                catch (SocketException s)
                {
                   
                }
            }
        }

    }
}

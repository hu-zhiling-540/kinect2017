using System;
using System.IO;
using SimpleJSON;

namespace UDP_Connection
{

    class Program
    {
        public static UDP_Sender sender = null;
        static public bool isRunning = true;

        StreamType readingStream = StreamType.Body;
        public void SetStream(StreamType readingStream)
        {
            this.readingStream = readingStream;
        }

        static void Main(string[] args)
        {
            string ipAddress = null;
            int port = -1;

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

            sender = new UDP_Sender(ipAddress, port);

            KinectController kinect = new KinectController();

            kinect.BodyFrameReady += KinectBodyFrameReceived;
            //kinect.ColorFrameReceived += KinectColorFrameReceived;

            while (true)
            {
                // if hit enter
                if (Console.ReadKey().Key == ConsoleKey.Enter)
                {
                    kinect.CloseKinect();
                    //kinect.BodyFrameReceived -= KinectBodyFrameReceived;
                    //Console.WriteLine("Stopping Client.");
                    break;
                }

            }
        }

        static void KinectColorFrameReceived(object obj, ColorFrameReadyEventArgs c)
        {
            Console.WriteLine("Color Frame");
            sender.sendMessage(c.ColorFrameData);
        }

        static void KinectBodyFrameReceived(object obj, BodyFrameReadyEventArgs f)
        {
            Console.WriteLine(f.BodyFrameData);
            sender.sendMessage(f.BodyFrameData);
        }

		static void KinectDepthFrameReceived(object obj, DepthFrameReadyEventArgs f)
		{
			Console.WriteLine(f.DepthFrameData);
			sender.sendMessage(f.DepthFrameData);
		}

    }





}

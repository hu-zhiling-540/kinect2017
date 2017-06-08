using System;
using System.IO;
using SimpleJSON;
using System.Runtime.Serialization.Json;
using System.Text;

namespace Kinect_UDP_Sender
{

    class Program
    {
        public static UDP_Sender sender = null;
        static public bool isRunning = true;


        // Create a User object and serialize it to a JSON stream.  
        //public static string WriteFromObject(this Object obj)
        //{
        //    //Create a stream to serialize the object to.  
        //    MemoryStream ms = new MemoryStream();

        //    // Serializer the User object to the stream.  
        //    DataContractJsonSerializer ser = new DataContractJsonSerializer(obj.GetType());
        //    ser.WriteObject(ms, obj);
        //    byte[] json = ms.ToArray();
        //    Console.WriteLine(json.Length);
        //    ms.Close();
        //    return Encoding.UTF8.GetString(json, 0, json.Length);

        //}

        static void Main(string[] args)
        {
            string ipAddress = null;
            int port = -1;
            string stream = null;

            // json file path as command line argument
            if (args.Length == 1)
            {
                //new StreamReader(path)
                string json_string = null;
                using (var fs = new FileStream(args[0], FileMode.Open, FileAccess.Read))
                {
                    using (var sr = new StreamReader(fs))
                    {
                        json_string = sr.ReadToEnd();
                    }
                }
                var pref = JSON.Parse(json_string);
                port = pref["port"].AsInt;                 // versionString will be a int containing "8008"
                ipAddress = pref["ip address"].Value;      // will be a string containing "127.0.0.1"
                stream = pref["stream"].Value;
                Console.WriteLine("reading value in: " + stream);
            }

            else if (args.Length == 2)
            {
                port = Int32.Parse(args[0]);
                ipAddress = args[1];
                stream = "Body";
            }

            else if (args.Length == 3)
            {
                port = Int32.Parse(args[0]);
                ipAddress = args[1];
                stream = args[2];
            }

            else
                Console.WriteLine("Invalid arguments");


            sender = new UDP_Sender(ipAddress, port);

            KinectController kinect = new KinectController();
            kinect.SetStreamType(stream);

            kinect.ColorFrameReady += KinectColorFrameReceived;
            kinect.BodyFrameReady += KinectBodyFrameReceived;
            kinect.DepthFrameReady += KinectDepthFrameReceived;
            kinect.InfraredFrameReady += KinectInfraredFrameReceived;

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
            sender.sendMessage(c.ColorFrameData);
            Console.WriteLine(Encoding.UTF8.GetString(c.ColorFrameData));
        }

        static void KinectBodyFrameReceived(object obj, BodyFrameReadyEventArgs f)
        {
            Console.WriteLine(f.BodyFrameData);
            sender.sendMessage(f.BodyFrameData);
        }

		static void KinectDepthFrameReceived(object obj, DepthFrameReadyEventArgs d)
		{
			Console.WriteLine(d.DepthFrameData);
			sender.sendMessage(d.DepthFrameData);
		}
        
        static void KinectInfraredFrameReceived(object obj, InfraredFrameReadyEventArgs i)
        {
            sender.sendMessage(i.InfraredFrameData);
        }
    }





}

using System;
using System.IO;
using SimpleJSON;

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
            bool bodyStreamOn = false;
            bool colorStreamOn = false;
            bool depthStreamOn = false;
            bool infraredStreamOn = false;

            //string stream = null;

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
                port = pref["port number"].AsInt;                 // should be "8008"
                ipAddress = pref["host name"].Value;            // should be "127.0.0.1"

                bodyStreamOn = pref["Body Stream On"].AsBool;
                colorStreamOn = pref["Color Stream On"].AsBool;
                depthStreamOn = pref["Depth Stream On"].AsBool;
                infraredStreamOn = pref["Infrared Stream On"].AsBool;
            }

            else if (args.Length == 2)
            {
                port = Int32.Parse(args[0]);
                ipAddress = args[1];
                // default
                colorStreamOn = true;
            }

            else
                Console.WriteLine("Invalid arguments");


            sender = new UDP_Sender(ipAddress, port);

            KinectController kinect = new KinectController();
            kinect.EnableDataStreams(bodyStreamOn, colorStreamOn, depthStreamOn, infraredStreamOn);

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
                    //Console.WriteLine("Stopping Client.");
                    break;
                }
            }
        }

        static void KinectColorFrameReceived(object obj, ColorFrameReadyEventArgs c)
        {
            sender.SendMessage(c.ColorFrameData, c.TimeStamp);
        }

        static void KinectBodyFrameReceived(object obj, BodyFrameReadyEventArgs f)
        {
            sender.SendMessage(f.BodyFrameData);
        }

		static void KinectDepthFrameReceived(object obj, DepthFrameReadyEventArgs d)
		{
			sender.SendMessage(d.DepthFrameData, d.TimeStamp);
		}
        
        static void KinectInfraredFrameReceived(object obj, InfraredFrameReadyEventArgs i)
        {
            sender.SendMessage(i.InfraredFrameData, i.TimeStamp);
        }
    }





}

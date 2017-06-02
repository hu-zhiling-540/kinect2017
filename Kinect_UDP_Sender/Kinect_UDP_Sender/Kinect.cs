using System;
using System.Collections.Generic;

using Microsoft.Kinect;
using System.Runtime.InteropServices;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Newtonsoft.Json;

namespace UDP_Connection
{
    class KinectController
    {

        KinectSensor mySensor;
        // MultiSourceFrame myFrame;
        MultiSourceFrameReader myReader;
        Body[] bodies;
        //WriteableBitmap outputImg = null;
        //byte[] framePixels = null;
        List<Body> bdList = new List<Body>();
        StreamType readStream = StreamType.Body;

        public KinectController()
        {
            OpenKinect();

        }

        public void OpenKinect()
        {
            // select the default sensor
            mySensor = KinectSensor.GetDefault();

            // enable data streaming
            if (mySensor != null)
            {
                // open the sensor
                mySensor.Open();
                // open reader for frame source, specify which streams to be used
                myReader = mySensor.OpenMultiSourceFrameReader(FrameSourceTypes.Body | FrameSourceTypes.Depth | FrameSourceTypes.Color);
                // register an event that fires each time a frame is ready
                myReader.MultiSourceFrameArrived += MultiSouceFrameArrived;
            }
            else
                throw new Exception("Unable to connect to Kinect sensor");
        }


        public void CloseKinect()
        {
            if (myReader != null)
            {
                myReader.Dispose();
                myReader = null;
            }

            if (mySensor != null)
            {
                mySensor.Close();
                mySensor = null;
            }
        }

        /// <summary>
        /// each time the sensor has a new frame of data available, 
        /// implement an event handler, store the code 
        /// sender object is the KinectSensor that fired the event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MultiSouceFrameArrived(object sender, MultiSourceFrameArrivedEventArgs e)
        {
            var frame = e.FrameReference.AcquireFrame();
            bdList.Clear();

			#region Body
			// frame will be automatically disposed of when done using it
            using (BodyFrame bdFrame = frame.BodyFrameReference.AcquireFrame())
            {
                //&& readStream == StreamType.Body
                if (bdFrame != null )
                {
                    bodies = new Body[bdFrame.BodyFrameSource.BodyCount];
                    bdFrame.GetAndRefreshBodyData(bodies);

                    // bodies = new Body[mySensor.BodyFrameSource.BodyCount];
                    foreach (Body body in bodies)
                    {
                        if (body.IsTracked)
                        {
                            bdList.Add(body);
                        }
                    }

                    // if at least one body is tracked
                    if (bdList.Capacity != 0)
                    {
                        //// convert it to string
                        //string bodyList = JsonConvert.SerializeObject(bdList);
                        BodyFrameReady(this, new BodyFrameReadyEventArgs(bdList));
                    }
                }
            }
            #endregion  

            #region Depth
            using (DepthFrame dFrame = frame.DepthFrameReference.AcquireFrame())
            {
                if (dFrame != null)
                {
                   
                }
            }
            #endregion  


            #region Color
            using (ColorFrame cFrame = frame.ColorFrameReference.AcquireFrame())
            {
                if (cFrame != null)
                {
                    ColorFrameReady(this, new ColorFrameReadyEventArgs(ColorFrameProcessor(cFrame)));
                }
            }
            #endregion  
        }


		/// <summary>
		/// For the best performance, allocate the memory for the data outside the event handler, 
        /// since the event handler runs every frame
		/// </summary>
		private byte[] ColorFrameProcessor(ColorFrame frame)
        {
            FrameDescription fd = frame.FrameDescription;
			// store the pixel data and then allocate the memory array
			//  4 bytes will store the data from one pixel
			byte[] colorFramePixels = new byte[fd.Width * fd.Height * 4];
            // get the data
            // color image frame is in BGRA format; BGRA has 4 bytes of data per pixel
            frame.CopyConvertedFrameDataToArray(colorFramePixels, ColorImageFormat.Bgra);

            return colorFramePixels;
        }

        private byte[] DepthFrameProcessor(DepthFrame frame)
        {
            FrameDescription fd = frame.FrameDescription;
            byte[] depthFramePixels = new byte[fd.Width * fd.Height];

            return depthFramePixels;
        }

        private byte[] InfraredFrameProcessor(InfraredFrame frame)
        {
            FrameDescription fd = frame.FrameDescription;
            
            ushort[] temp = new ushort[fd.Width * fd.Height];
            //copy the infrared frame data to an unsigned short array
            frame.CopyFrameDataToArray(temp);
            // * bytes per pixel
            byte[] pixels = new byte[fd.Width * fd.Height * 4];
            
            return pixels;

        }

        



        // event that fires when a new body frame is available
        public event EventHandler<BodyFrameReadyEventArgs> BodyFrameReady;
        
        // event that fires when a new color frame is available
        public event EventHandler<ColorFrameReadyEventArgs> ColorFrameReady;

        // event that fires when a new depth frame is available
        public event EventHandler<DepthFrameReadyEventArgs> DepthFrameReady;
    }

    public enum StreamType
    {
        Body,
        Color,
        Depth,
        Infrared
    }

    public class BodyFrameReadyEventArgs: EventArgs
    {
        public string BodyFrameData {
            get;
            set;
        }
        public BodyFrameReadyEventArgs(List<Body> bdList)
        {
            // convert it to string
            this.BodyFrameData = JsonConvert.SerializeObject(bdList);
        }
    }

    public class ColorFrameReadyEventArgs: EventArgs
    {
        public byte[] ColorFrameData
        {
            get;
            set;
        }
        public ColorFrameReadyEventArgs(byte[] colorFramePixels)
        {
            this.ColorFrameData = colorFramePixels;
        }
    }

    public class DepthFrameReadyEventArgs : EventArgs
    {
        public byte[] DepthFrameData
        {
            get;
            set;
        }
        public DepthFrameReadyEventArgs(byte[] depthFramePixels)
        {
            this.DepthFrameData = depthFramePixels;
        }
    }
    }

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
        
        // each time the sensor has a new frame of data available, 
        // implement an event handler, store the code 
        // sender object is the KinectSensor that fired the event
        private void MultiSouceFrameArrived(object sender, MultiSourceFrameArrivedEventArgs e)
        {
            var frame = e.FrameReference.AcquireFrame();
            bdList.Clear();

            #region Body
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
                        BodyFrameReceived(this, new BodyFrameReceivedEventArgs(bdList));
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
                    ColorFrameReceived(this, new ColorFrameReceivedEventArgs(ColorDisplay(cFrame)));
                }
            }
            #endregion  
        }

        public byte[] ColorDisplay(ColorFrame frame)
        {
            FrameDescription fd = frame.FrameDescription;
            // declare a member variable to store the pixel data and then allocate the memory array
            byte[] colorFramePixels = new byte[fd.Width * fd.Height * 4];
            // get the data
            frame.CopyConvertedFrameDataToArray(colorFramePixels, ColorImageFormat.Bgra);

            return colorFramePixels;
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
        }


        public void CloseKinect(object sender, EventArgs e)
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

        public event EventHandler<BodyFrameReceivedEventArgs> BodyFrameReceived;
        public event EventHandler<ColorFrameReceivedEventArgs> ColorFrameReceived;
    }

    public enum StreamType
    {
        Body,
        Color,
        Depth,
        Infrared
    }

    public class BodyFrameReceivedEventArgs: EventArgs
    {
        public string BodyFrameData {
            get;
            set;
        }
        public BodyFrameReceivedEventArgs(List<Body> bdList)
        {
            // convert it to string
            this.BodyFrameData = JsonConvert.SerializeObject(bdList);
        }
    }

    public class ColorFrameReceivedEventArgs: EventArgs
    {
        public string ColorFrameData
        {
            get;
            set;
        }
        public ColorFrameReceivedEventArgs(byte[] colorFramePixels)
        {
            this.ColorFrameData = JsonConvert.SerializeObject(colorFramePixels);
        }
    }
}

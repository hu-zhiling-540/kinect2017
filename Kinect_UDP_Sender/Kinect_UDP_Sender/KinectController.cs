using System;
using System.Collections.Generic;
using Microsoft.Kinect;
using Newtonsoft.Json;

namespace Kinect_UDP_Sender
{
    public enum StreamType
    {
        Body,
        Color,
        Depth,
        Infrared
    }

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
                myReader = mySensor.OpenMultiSourceFrameReader(FrameSourceTypes.Body | FrameSourceTypes.Depth | FrameSourceTypes.Color | FrameSourceTypes.Infrared);
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
                if (bdFrame != null)
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
                    DepthFrameReady(this, new DepthFrameReadyEventArgs(dFrame.DepthFrameProcessor()));
                }
            }
            #endregion  


            #region Color
            using (ColorFrame cFrame = frame.ColorFrameReference.AcquireFrame())
            {
                if (cFrame != null)
                {
                    ColorFrameReady(this, new ColorFrameReadyEventArgs(cFrame.ColorFrameProcessor()));
                }
            }
            #endregion

            #region Infrared
            using (InfraredFrame iFrame = frame.InfraredFrameReference.AcquireFrame())
            {
                if (iFrame != null)
                {
                    InfraredFrameReady(this, new InfraredFrameReadyEventArgs(iFrame.InfraredFrameProcessor()));
                }
            }
            #endregion
        }



        // event that fires when a new body frame is available
        public event EventHandler<BodyFrameReadyEventArgs> BodyFrameReady;

        // event that fires when a new color frame is available
        public event EventHandler<ColorFrameReadyEventArgs> ColorFrameReady;

        // event that fires when a new depth frame is available
        public event EventHandler<DepthFrameReadyEventArgs> DepthFrameReady;

        // event that fires when a new infrared frame is available
        public event EventHandler<InfraredFrameReadyEventArgs> InfraredFrameReady;
    }



    public class BodyFrameReadyEventArgs : EventArgs
    {
        public string BodyFrameData
        {
            get;
            set;
        }
        public BodyFrameReadyEventArgs(List<Body> bdList)
        {
            // convert it to string
            this.BodyFrameData = JsonConvert.SerializeObject(bdList);
        }
    }

    public class ColorFrameReadyEventArgs : EventArgs
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

    public class InfraredFrameReadyEventArgs : EventArgs
    {
        public byte[] InfraredFrameData
        {
            get;
            set;
        }
        public InfraredFrameReadyEventArgs(byte[] infraredFramePixels)
        {
            this.InfraredFrameData = infraredFramePixels;
        }
    }
}

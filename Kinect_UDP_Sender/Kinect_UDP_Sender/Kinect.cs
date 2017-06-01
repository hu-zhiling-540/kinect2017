using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using Microsoft.Kinect;
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
                        //// send the data
                        //Console.WriteLine(bodyList);
                        frameReceived(this, new FrameReceivedEventArgs(bdList));
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
            //using (ColorFrame cFrame = frame.ColorFrameReference.AcquireFrame())
            //{
            //    if (cFrame != null)
            //    {
            //        if (streamChoice == Stream.Color)
            //        {
            //            // setting the image (defined in xaml file) to the color data
            //            //image.Source = cFrame.ToBitmap();
            //            image.Source = cFrame.colorDisplay();
            //        }
            //    }
            //}
            #endregion  
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

        public event EventHandler<FrameReceivedEventArgs> frameReceived;
    }

    public class FrameReceivedEventArgs: EventArgs
    {
        public string frameData {
            get;
            set;
        }
        public FrameReceivedEventArgs(List<Body> bdList)
        {
            // convert it to string
            this.frameData = JsonConvert.SerializeObject(bdList);
        }


    }
}

using System;
using System.Collections.Generic;
using Microsoft.Kinect;
using Newtonsoft.Json;
using System.Windows.Media;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Windows.Media.Imaging;

namespace Kinect_UDP_Sender
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
        FrameSourceTypes openStreams = FrameSourceTypes.None;

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
        /// Read bools from Preference file, and enable any or all of the data streams
        /// </summary>
        public void EnableDataStreams(bool bodyOn, bool colorOn, bool depthOn, bool infraredOn)
        {
            if(bodyOn)
                openStreams |= FrameSourceTypes.Body;
            if (colorOn)
                openStreams |= FrameSourceTypes.Color;
            if (depthOn)
                openStreams |= FrameSourceTypes.Depth;
            if (infraredOn)
                openStreams |= FrameSourceTypes.Infrared;
            
            // open reader for frame source, specify which streams to be used
            myReader = mySensor.OpenMultiSourceFrameReader(openStreams);
            // register an event that fires each time a frame is ready
            myReader.MultiSourceFrameArrived += MultiSouceFrameArrived;
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
            // millisecond component of the time represented by current time in local computer
            long timeInMillisec = DateTimeOffset.Now.Millisecond;
            byte[] timeStamp = BitConverter.GetBytes(timeInMillisec);

            bdList.Clear();

            #region Body
            if ((openStreams | FrameSourceTypes.Body) != 0)
            {
                // frame will be automatically disposed of when done using it
                using (BodyFrame bdFrame = frame.BodyFrameReference.AcquireFrame())
                {
                    if (bdFrame != null)
                    {
                        bodies = new Body[bdFrame.BodyFrameSource.BodyCount];
                        bdFrame.GetAndRefreshBodyData(bodies);
                        
                        foreach (Body body in bodies)
                        {
                            if (body.IsTracked)
                            {
                                Console.WriteLine("Object detected!");
                                bdList.Add(body);
                            }
                        }

                        // if at least one body is tracked
                        if (bdList.Capacity != 0)
                        {
                            BodyFrameReady(this, new BodyFrameReadyEventArgs(bdList));
                        }
                    }
                }
            }
            #endregion  

            #region Color
            if ((openStreams | FrameSourceTypes.Color) != 0)
            {
                using (ColorFrame cFrame = frame.ColorFrameReference.AcquireFrame())
                {
                    if (cFrame != null)
                    {
                        //FrameDescription fd = cFrame.FrameDescription;
                        //int bytesPerPixel = PixelFormats.Bgra32.BitsPerPixel / 8;

                        //var size = fd.Width * fd.Height * bytesPerPixel;
                        ////Console.WriteLine(fd.BytesPerPixel);
                        //Console.WriteLine(size);//434,176

                        //var storage = new byte[size + sizeof(long)];
                        //WriteableBitmap bit = new WriteableBitmap(fd.Width, fd.Height, 96.0, 96.0, PixelFormats.Bgr32, null);


                        ColorFrameReady(this, new ColorFrameReadyEventArgs(cFrame.ColorFrameProcessor()));
                    }
                }
            }
            #endregion

            #region Depth
            if ((openStreams | FrameSourceTypes.Depth) != 0)
            {
                using (DepthFrame dFrame = frame.DepthFrameReference.AcquireFrame())
                {
                    if (dFrame != null)
                    {
                        FrameDescription fd = dFrame.FrameDescription;
                        var size = fd.Width * fd.Height * fd.BytesPerPixel;
                        Console.WriteLine(fd.BytesPerPixel);
                        //Console.WriteLine(size);//434,176

                        // sizeof(long) = 8
                        var storage = new byte[size + sizeof(long)];
                        
                        // access to the underlying buffer used by the system to store this frame's data
                        using (var depthFrameBuffer = dFrame.LockImageBuffer())
                        {
                            Marshal.Copy(depthFrameBuffer.UnderlyingBuffer, storage, 0, (int)depthFrameBuffer.Size);
                        }
                        Buffer.BlockCopy(timeStamp, 0, storage, (int)(size), sizeof(long));

                        DepthFrameReady(this, new DepthFrameReadyEventArgs(storage));
                    }
                }
            }
            #endregion  
            
            #region Infrared
            if ((openStreams | FrameSourceTypes.Infrared) != 0)
            {
                using (InfraredFrame iFrame = frame.InfraredFrameReference.AcquireFrame())
                {
                    if (iFrame != null)
                    {
                        InfraredFrameReady(this, new InfraredFrameReadyEventArgs(iFrame.InfraredFrameProcessor()));
                    }
                }
            }
            #endregion
        }


        // event that fires when a new color frame is available
        public event EventHandler<ColorFrameReadyEventArgs> ColorFrameReady;

        // event that fires when a new body frame is available
        public event EventHandler<BodyFrameReadyEventArgs> BodyFrameReady;

        // event that fires when a new depth frame is available
        public event EventHandler<DepthFrameReadyEventArgs> DepthFrameReady;

        // event that fires when a new infrared frame is available
        public event EventHandler<InfraredFrameReadyEventArgs> InfraredFrameReady;

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
            Console.WriteLine(colorFramePixels.Length);
            this.ColorFrameData = colorFramePixels;
        }
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
            this.BodyFrameData = JsonConvert.SerializeObject(bdList);
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

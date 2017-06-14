using System;
using System.Collections.Generic;
using Microsoft.Kinect;
using Newtonsoft.Json;
using System.Windows.Media;
using System.Runtime.InteropServices;
using System.Threading;

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
            long timeStamp = DateTimeOffset.Now.Millisecond;
            //byte[] timeStamp = BitConverter.GetBytes(timeInMillisec);



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
                            BodyFrameReady(this, new BodyFrameReadyEventArgs(bdList,timeStamp));
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

                        //double fps = 1.0 / cFrame.ColorCameraSettings.FrameInterval.TotalSeconds;

                        //Console.WriteLine(cFrame.RawColorImageFormat); // return Yuy2 - 2 bytes per pixel
                        // should be 4 bytes store the data from one pixel
                        int bytesPerPixel = PixelFormats.Bgra32.BitsPerPixel / 8;

                        FrameDescription fd = cFrame.FrameDescription;
                        var size = fd.Width * fd.Height * bytesPerPixel;
                        // store the pixel data and then allocate the memory array
                        var buffer = new byte[size];

                        // want to return the color image frame in BGRA format
                        cFrame.CopyConvertedFrameDataToArray(buffer, ColorImageFormat.Bgra);
                        //string hi = BitConverter.ToString(pixels);
                        //Console.WriteLine(hi.Replace("-", ""));
                        //cFrame.CopyRawFrameDataToArray(pixels);

                        ColorFrameReady(this, new ColorFrameReadyEventArgs(buffer, timeStamp));
                    }
                }

                Thread.Sleep(100000);
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
                        // 512 * 424 * 2 = 434,176

                        // sizeof(long) = 8
                        var buffer = new byte[size];
                        
                        // access to the underlying buffer used by the system to store this frame's data
                        using (var dFrameBuffer = dFrame.LockImageBuffer())
                        {
                            Marshal.Copy(dFrameBuffer.UnderlyingBuffer, buffer, 0, (int)dFrameBuffer.Size);
                        }
                        //Buffer.BlockCopy(timeStamp, 0, buffer, (int)(size), sizeof(long));

                        DepthFrameReady(this, new DepthFrameReadyEventArgs(buffer, timeStamp));
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
                        FrameDescription fd = iFrame.FrameDescription;
                        var size = fd.Width * fd.Height * fd.BytesPerPixel;
                        //Console.WriteLine(size);//434,176

                        // sizeof(long) = 8
                        var buffer = new byte[size];

                        // access to the underlying buffer used by the system to store this frame's data
                        using (var iFrameBuffer = iFrame.LockImageBuffer())
                        {
                            Marshal.Copy(iFrameBuffer.UnderlyingBuffer, buffer, 0, (int)iFrameBuffer.Size);
                        }
                        //Buffer.BlockCopy(timeStamp, 0, buffer, (int)(size), sizeof(long));

                        InfraredFrameReady(this, new InfraredFrameReadyEventArgs(buffer, timeStamp));
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
        public byte[] ColorFrameData { get; set; }
        public long TimeStamp { get; set; }
        public ColorFrameReadyEventArgs(byte[] colorFramePixels, long timeStamp)
        {
            this.ColorFrameData = colorFramePixels;
            this.TimeStamp = timeStamp;
        }
    }

    public class BodyFrameReadyEventArgs : EventArgs
    {
        public string BodyFrameData { get; set; }
        public long TimeStamp { get; set; }
        public BodyFrameReadyEventArgs(List<Body> bdList, long timeStamp)
        {
            this.BodyFrameData = JsonConvert.SerializeObject(bdList);
            this.TimeStamp = timeStamp;
        }
    }

    

    public class DepthFrameReadyEventArgs : EventArgs
    {
        public byte[] DepthFrameData { get; set; }
        public long TimeStamp { get; set; }
        public DepthFrameReadyEventArgs(byte[] depthFramePixels, long timeStamp)
        {
            this.DepthFrameData = depthFramePixels;
            this.TimeStamp = timeStamp;
        }
    }

    public class InfraredFrameReadyEventArgs : EventArgs
    {
        public byte[] InfraredFrameData { get; set; }
        public long TimeStamp { get; set; }
        public InfraredFrameReadyEventArgs(byte[] infraredFramePixels, long timeStamp)
        {
            this.InfraredFrameData = infraredFramePixels;
            this.TimeStamp = timeStamp;
        }
    }
}

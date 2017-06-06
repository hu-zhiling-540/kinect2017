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
		private static WriteableBitmap ColorFrameProcessor(this ColorFrame frame)
		{
			FrameDescription fd = frame.FrameDescription;
			// create a bitmap to store the data
			WriteableBitmap outputImg = new WriteableBitmap(fd.Width, fd.Height, 96.0, 96.0, PixelFormats.Bgra32, null);
			// store the pixel data and then allocate the memory array
			//  4 bytes will store the data from one pixel
            byte[] colorFramePixels = new byte[fd.Width * fd.Height * 4];
			// get the data
			// color image frame is in BGRA format; BGRA has 4 bytes of data per pixel
			frame.CopyConvertedFrameDataToArray(framePixels, ColorImageFormat.Bgra);

			outputImg.Lock();           // reserve the back buffer for updates
										// write the pixel data into the bitmap
			Marshal.Copy(framePixels, 0, outputImg.BackBuffer, framePixels.Length);
			// specify the area of the bitmap that changed
			outputImg.AddDirtyRect(new Int32Rect(0, 0, fd.Width, fd.Height));
			outputImg.Unlock();         //  release the back buffer to make it available for display.
			
            return outputImg;
		}

        private byte[] DepthFrameProcessor(DepthFrame frame)
        {
            FrameDescription fd = frame.FrameDescription;
			// Each pixel of depth data is stored in a short; therefore the array in this method is an array of shorts.
			short[] temp = new ushort[fd.Width * fd.Height];
            byte[] depthFramePixels = new byte[fd.Width * fd.Height * 4];
			// create a bitmap to store the data
			WriteableBitmap outputImg = new WriteableBitmap(fd.Width, fd.Height, 96.0, 96.0, PixelFormats.Bgra32, null);

            frame.CopyPixelDataTo(temp);

			// Get the min and max reliable depth for the current frame
			int minDepth = frame.MinDepth;
			int maxDepth = frame.MaxDepth;

            DepthImagePixel[] depthPixels = new DepthImagePixel[mySensor.DepthStream.FramePixelDataLength];

			// Convert the depth to RGB
			int colorPixelIndex = 0;

            for (int depthIndex = 0, colorIndex = 0; depthIndex<temp.Length && colorIndex<depthFramePixels.Length; depthIndex++, colorIndex += 4)
			{
                // Get the depth for this pixel
                short depth = temp[depthIndex];

				// To convert to a byte, we're discarding the most-significant
				// rather than least-significant bits.
				// We're preserving detail, although the intensity will "wrap."
				// Values outside the reliable depth range are mapped to 0 (black).

				// Note: Using conditionals in this loop could degrade performance.
				// Consider using a lookup table instead when writing production code.
				// See the KinectDepthViewer class used by the KinectExplorer sample
				// for a lookup table example.
				byte intensity = (byte)(depth >= minDepth && depth <= maxDepth ? depth : 0);

				// Write out blue byte
                depthFramePixels[colorIndex++] = intensity;

				// Write out green byte
                depthFramePixels[colorIndex++] = intensity;

				// Write out red byte                        
                depthFramePixels[colorIndex++] = intensity;

				++colorPixelIndex;
			}

            int stride = fd.Width * FormatException.BitsPerPixel / 8;

			//  use the WriteableBitmap to display the mapped depth data.
			// Use the WriteableBitmap.WritePixels method to save the pixel data
			outputImg.WritePixels(new Int32Rect(0, 0, fd.Width, fd.Height), 
                                  depthFramePixels, this.colorBitmap.PixelWidth * sizeof(int),
                                  0);

			return depthFramePixels;
        }

        private byte[] InfraredFrameProcessor(InfraredFrame frame)
        {
            FrameDescription fd = frame.FrameDescription;
            
            ushort[] temp = new ushort[fd.Width * fd.Height];
            //copy the infrared frame data to an unsigned short array
            frame.CopyFrameDataToArray(temp);
            // * bytes per pixel
            byte[] pixels = new byte[fd.Width * fd.Height * (PixelFormats.Bgr32.BitsPerPixel + 7) / 8];

            frame.CopyFrameDataToArray(temp);

			int colorIndex = 0;
            for (int infraredIndex = 0; infraredIndex < temp.Length; ++infraredIndex)
			{
				ushort ir = temp[infraredIndex];
				byte intensity = (byte)(ir >> 8);

                pixels[colorIndex++] = intensity; // Blue
                pixels[colorIndex++] = intensity; // Green   
                pixels[colorIndex++] = intensity; // Red

				// ++colorIndex;
				// If we were outputting BGRA, we would write alpha here.
				pixelData[colorIndex++] = 255;       //Alpha 
			}

			int stride = width * format.BitsPerPixel / 8;

			// create a bitmap to store the data
			WriteableBitmap outputImg = new WriteableBitmap(fd.Width, fd.Height, 96.0, 96.0, PixelFormats.Bgra32, null);


			outputImg.WritePixels(new Int32Rect(0, 0, fd.Width, fd.Height),
								  depthFramePixels, this.colorBitmap.PixelWidth * sizeof(int),
								  0);
            return pixels;

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

    public class DepthFrameReadyEventArgs: EventArgs
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

    public class InfraredFrameReadyEventArgs: EventArgs
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

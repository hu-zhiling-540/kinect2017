using Microsoft.Kinect;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Kinect_UDP_Sender
{
    static class KinectExtensions
    {
        /// <summary>
		/// For the best performance, allocate the memory for the data outside the event handler, 
		/// since the event handler runs every frame
		/// </summary>
		public static byte[] ColorFrameProcessor(this ColorFrame frame)
        {
            //Console.WriteLine(frame.RawColorImageFormat); return Yuy2 - 2 bytes per pixel
            // should be 4 bytes store the data from one pixel
            int bytesPerPixel = PixelFormats.Bgra32.BitsPerPixel/8;

            FrameDescription fd = frame.FrameDescription;
            
            // store the pixel data and then allocate the memory array
            byte[] pixels = new byte[fd.Width * fd.Height * bytesPerPixel];
            //byte[] pixels = new byte[fd.LengthInPixels];

            // want to return the color image frame in BGRA format
            frame.CopyConvertedFrameDataToArray(pixels, ColorImageFormat.Bgra);

            //// create a bitmap to store the data
            //WriteableBitmap outputImg = new WriteableBitmap(fd.Width, fd.Height, 96.0, 96.0, PixelFormats.Bgr32, null);
            //outputImg.Lock();           // reserve the back buffer for updates
            //                            // write the pixel data into the bitmap
            //Marshal.Copy(pixels, 0, outputImg.BackBuffer, pixels.Length);
            //// specify the area of the bitmap that changed
            //outputImg.AddDirtyRect(new Int32Rect(0, 0, fd.Width, fd.Height));
            //outputImg.Unlock();         //  release the back buffer to make it available for display.
            //BitmapEncoder encoder = new JpegBitmapEncoder();
            //encoder.Frames.Add(BitmapFrame.Create(outputImg as BitmapSource));
            //using (var stream = new FileStream("temp.jpg", FileMode.Create))
            //{
            //    encoder.Save(stream);
            //}
            //using (FileStream stream = new FileStream("temp.jpg", FileMode.Open, FileAccess.Read))
            //{
            //    using (BinaryReader reader = new BinaryReader(stream))
            //    {
            //        return reader.ReadBytes((int)stream.Length);
            //    }
            //}

            return pixels;
        }

        public static byte[] DepthFrameProcessor(this DepthFrame frame, long timeStamp)
        {
            FrameDescription fd = frame.FrameDescription;
            var depthBuffer = new byte[fd.Width * fd.Height * fd.BytesPerPixel + sizeof(long)];

            using (var depthFrameBuffer = frame.LockImageBuffer())
            {
                Marshal.Copy(depthFrameBuffer.UnderlyingBuffer, depthBuffer, 0, (int)depthFrameBuffer.Size);
            }
            Buffer.BlockCopy(BitConverter.GetBytes(timeStamp), 0, depthBuffer, (int)(fd.Width * fd.Height * fd.BytesPerPixel), sizeof(long));

            return depthBuffer;
        }

        public static byte[] InfraredFrameProcessor(this InfraredFrame frame)
        {
            FrameDescription fd = frame.FrameDescription;

            ushort[] tempData = new ushort[fd.Width * fd.Height];
            // * bytes per pixel
            byte[] pixels = new byte[fd.Width * fd.Height * (PixelFormats.Bgr32.BitsPerPixel + 7) / 8];

            //copy the infrared frame data to an unsigned short array
            frame.CopyFrameDataToArray(tempData);

            int colorIndex = 0;
            for (int infraredIndex = 0; infraredIndex < tempData.Length; ++infraredIndex)
            {
                ushort ir = tempData[infraredIndex];
                byte intensity = (byte)(ir >> 8);

                pixels[colorIndex++] = intensity; // Blue
                pixels[colorIndex++] = intensity; // Green   
                pixels[colorIndex++] = intensity; // Red

                // If we were outputting BGRA, we would write alpha here.
                pixels[colorIndex++] = 255;       //Alpha 
            }

            int stride = fd.Width * PixelFormats.Bgr32.BitsPerPixel / 8;

            // create a bitmap to store the data
            WriteableBitmap outputImg = new WriteableBitmap(fd.Width, fd.Height, 96.0, 96.0, PixelFormats.Bgra32, null);


            outputImg.WritePixels(new Int32Rect(0, 0, fd.Width, fd.Height),
                                  pixels, outputImg.PixelWidth * sizeof(int),
                                  0);
            return pixels;

        }
    }
}

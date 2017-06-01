﻿using Microsoft.Kinect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace KinectSkeleton
{
    static class Extensions
    {

        #region camera
        public static WriteableBitmap ColorDisplay(this ColorFrame frame)
        {
            FrameDescription fd = frame.FrameDescription;
            // create a bitmap to store the data
            WriteableBitmap outputImg = new WriteableBitmap(fd.Width, fd.Height, 96.0, 96.0, PixelFormats.Bgra32, null);
            // declare a member variable to store the pixel data and then allocate the memory array
            byte[] framePixels = new byte[fd.Width * fd.Height * 4];
            // get the data
            frame.CopyConvertedFrameDataToArray(framePixels, ColorImageFormat.Bgra);

            outputImg.Lock();           // reserve the back buffer for updates
            // write the pixel data into the bitmap
            Marshal.Copy(framePixels, 0, outputImg.BackBuffer, framePixels.Length);
            // specify the area of the bitmap that changed
            outputImg.AddDirtyRect(new Int32Rect(0, 0, fd.Width, fd.Height));       
            outputImg.Unlock();         //  release the back buffer to make it available for display.
            return outputImg;
        }
        #endregion
    }
}

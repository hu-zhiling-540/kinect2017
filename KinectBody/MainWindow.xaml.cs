using Microsoft.Kinect;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace KinectSkeleton
{
        
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        KinectSensor mySensor;
        // MultiSourceFrame myFrame;
        MultiSourceFrameReader myReader;
        Body[] bodies;
        //WriteableBitmap outputImg = null;
        //byte[] framePixels = null;

        // default mode
        Stream streamChoice = Stream.Color;


        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // select the default sensor
            mySensor = KinectSensor.GetDefault();

            // enable data streaming
            if (mySensor != null)
            {
                // open the sensor
                mySensor.Open();
                // open reader for frame source, specify which streams to be used
                myReader = mySensor.OpenMultiSourceFrameReader(FrameSourceTypes.Body|FrameSourceTypes.Depth|FrameSourceTypes.Color);
                // register an event that fires each time a frame is ready
                myReader.MultiSourceFrameArrived += multiSouceFrameArrived;
            }
        }


        //private static string CovertToJSON(Body body)
        //{
        //    ulong id = Body.TrackingId;
        //    Body.Joints;
        //    JointType
        //    Position
        //        TrackingState
        //    return
        //}

        private JointType[] _JointType = new JointType[]
        {  
            // head
            JointType.SpineBase,JointType.SpineMid,  
            JointType.SpineMid,JointType.SpineShoulder,  
            JointType.SpineShoulder,JointType.Neck,  
            JointType.Neck,JointType.Head,  
          
            // left upper quadrant
            JointType.SpineShoulder,JointType.ShoulderLeft,  
            JointType.ShoulderLeft,JointType.ElbowLeft,  
            JointType.ElbowLeft,JointType.WristLeft,  
            JointType.WristLeft,JointType.HandLeft,  
            JointType.HandLeft,JointType.HandTipLeft,  
            JointType.HandLeft,JointType.ThumbLeft,  
          
            // right upper quardrant
            JointType.SpineShoulder,JointType.ShoulderRight,  
            JointType.ShoulderRight,JointType.ElbowRight,  
            JointType.ElbowRight,JointType.WristRight,  
            JointType.WristRight,JointType.HandRight,  
            JointType.HandRight,JointType.HandTipRight,  
            JointType.HandRight,JointType.ThumbRight,  
          
            // left lower quadrant
            JointType.SpineBase,JointType.HipLeft,  
            JointType.HipLeft,JointType.KneeLeft,  
            JointType.KneeLeft,JointType.AnkleLeft,  
            JointType.AnkleLeft,JointType.FootLeft,  
          
            // right lower quadrant
            JointType.SpineBase,JointType.HipRight,  
            JointType.HipRight,JointType.KneeRight,  
            JointType.KneeRight,JointType.AnkleRight,  
            JointType.AnkleRight,JointType.FootRight  
            };

        List<Body> bdList = new List<Body>();

        // each time the sensor has a new frame of data available, 
        // implement an event handler, store the code 
        // sender object is the KinectSensor that fired the event
        private void multiSouceFrameArrived(object sender, MultiSourceFrameArrivedEventArgs e)
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
                        // convert it to string
                        string bodyList = JsonConvert.SerializeObject(bdList);
                        // send the data
                        Console.WriteLine(bodyList);
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
                    if (streamChoice == Stream.Color)
                    {
                        // setting the image (defined in xaml file) to the color data
                        //image.Source = cFrame.ToBitmap();
                        image.Source = cFrame.colorDisplay();
                    }
                }
            }
            #endregion  
        }


        private void drawBodies()
        {
            foreach (var body in bodies)
            {
                if (body != null)
                {
                    if (body.IsTracked)
                    {
                        // Draw skeleton.
                        
                    }
                }
            }
        }

        private void Windowd_Close(object sender, EventArgs e)
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

        private void bodyButton(object sender, RoutedEventArgs e)
        {

        }
        private void colorButton(object sender, RoutedEventArgs e)
        {

        }
    }

    public enum Stream
    {
        Body,
        Color,
        Depth,
        Infrared
    }
}

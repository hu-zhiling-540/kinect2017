using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using Microsoft.Kinect;

namespace Kinect_UDP_Sender
{

    /// <summary>
    /// Contains all methods that help serialize Skeleton to Byte-Array
    /// </summary>
    public static class SkeletonSerializer
    {
//        "JointOrientations":
//{"SpineBase":{"JointType":0,"Orientation":{"X":-0.05233748,"Y":0.9205873,"Z":-0.0939059556,"W":0.375448316}},
//"SpineMid":{"JointType":1,"Orientation":{"X":-0.053282842,"Y":0.9167501,"Z":-0.0933728,"W":0.384722918}},
//"Neck":{"JointType":2,"Orientation":{"X":-0.0698294,"Y":0.907467246,"Z":-0.133008137,"W":0.39234665}},
        [DataContract]
		public class SkeletonJoint
        {
            // Print name is "JointType" instead of "Joint".
            [DataMember(Name = "JointType")]
            public string Name;

            public double X;
            public double Y;
            public double Z;
        }

        [DataContract]
        public class Skeleton
        {
            public string TrackingId;
            public List<SkeletonJoint> Joints;
        }

        [DataContract]
        public class Skeletons
        {
            public List<Skeleton> SkeletonsList;
        }

        public static Skeletons WriteSkeletons(this List<Body> bodiesList)
        {
            Skeletons skels = new Skeletons()
            {
                SkeletonsList = new List<Skeleton>()
            };
            foreach (var body in bodiesList)
            {
                Skeleton skel = new Skeleton()
                {
                    TrackingId = body.TrackingId.ToString(),
                    Joints = new List<SkeletonJoint>()
                };
                foreach (Joint j in body.Joints.Values)
                {
                    skel.Joints.Add(new SkeletonJoint
                    {
                        
                        Name = j.JointType.ToString(),
                        //X = Horizontal position measured as the distance, in meters from the Kinect along the X Axis
                        X = j.Position.X,
                        //Y = Vertical position measured as the distance, in meters from the Kinect along the Y Axis
                        Y = j.Position.Y,
                        //Z = Distance from Kinect measured in meters
                        Z = j.Position.Z

                    });
                    Console.WriteLine(skel.Joints[0].Name);
                    Console.WriteLine(skel.Joints[0].X);

                }
                skels.SkeletonsList.Add(skel);
            }
            return skels;
        }

		// Create a User object and serialize it to a JSON stream.  
		public static string WriteFromObject(this Object obj)
		{
			//Create a stream to serialize the object to.  
			MemoryStream ms = new MemoryStream();

			// Serializer the User object to the stream.  
            DataContractJsonSerializer ser = new DataContractJsonSerializer(obj.GetType());
			ser.WriteObject(ms, obj);
			byte[] json = ms.ToArray();
            Console.WriteLine(json.Length);
			ms.Close();
			return Encoding.UTF8.GetString(json, 0, json.Length);

		}
	}
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;

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
		public class JSONJoint
        {
            // Print name is "JointType" instead of "Joint".
            [DataMember(Name = "JointType")]
            public string Name;

            public double X;
            public double Y;
            public double Z;
        }

        [DataContract]
        public class JSONSkel
        {
            public string TrackingId;
            public List<JSONJoint> Joints;
        }

        [DataContract]
        public class JSONSkeletons
        {
            public List<JSONSkel> Skeletons { get; set; }
        }

        public static JSONSkeletons WriteSkeletons(List<JSONSkel> skeletons)
        {
            JSONSkeletons skels = new JSONSkeletons()
            {
                Skeletons = new List<JSONSkel>()
            };
            foreach (var s in skeletons)
            {
                JSONSkel skel = new JSONSkel()
                {
                    TrackingId = s.TrackingId,
                    Joints = new List<JSONJoint>()
                };
                foreach (JSONJoint j in skel.Joints)
                {
                    skel.Joints.Add(new JSONJoint
                    {
                        Name = j.JointType,
                        X = j.JointType.Orientation.X,
                        Y = j.JointType.Orientation.Y,
                        Z = j.JointType.Orientation.Z

                    });
                }
                skels.Skeletons.Add((skel));
            }

            return skels;
        }

		// Create a User object and serialize it to a JSON stream.  
		public static string WriteFromObject(JSONSkeletons skels)
		{
			//Create User object


			//Create a stream to serialize the object to.  
			MemoryStream ms = new MemoryStream();

			// Serializer the User object to the stream.  
            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(skels));
			ser.WriteObject(ms, skels);
			byte[] json = ms.ToArray();
			ms.Close();
			return Encoding.UTF8.GetString(json, 0, json.Length);

		}
	}
}

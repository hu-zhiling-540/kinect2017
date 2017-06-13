using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace Kinect_UDP_Sender
{
    class Packet
    {
        public Packet(long timeStamp, int serialNum, int totalCount, byte[] data)
        {
            this.TimeStamp = timeStamp;
            this.SerialNum = serialNum;
            this.TotalCount = totalCount;
            this.Data = data;
        }
        public string Name { get; set; }
        public byte[] Data { get; set; }
        public long TimeStamp { get; set; }
        public int SerialNum { get; set; }
        public int TotalCount { get; set; }
        
    }

    static class Serialization
    {
        /// <summary>
        /// Convert an object to a byte array
        /// </summary>
        /// <param name="packet"></param>
        /// <returns></returns>
        public static byte[] Serialize(this Packet packet)
        {
            // construct a BinaryFormatter and use it to serialize the data to the stream
            BinaryFormatter formatter = new BinaryFormatter();
            // dispose memory stream once done processing it
            using (var memstrm = new MemoryStream())
            {
                // serialize the packet into the stream
                formatter.Serialize(memstrm, packet);
                return memstrm.GetBuffer();
            }
        }
    }
}

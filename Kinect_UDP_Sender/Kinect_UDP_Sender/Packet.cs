using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

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
            //MemoryStream memstrm = new MemoryStream();
            //BinaryFormatter formatter = new BinaryFormatter();
            //formatter.Serialize(memstrm, packet);
            //memstrm.Position = 0;
            //byte[] data = memstrm.GetBuffer();
            //memstrm.Read(data, 0, data.Length);
            //memstrm.Close();
            ////string hi = BitConverter.ToString(data);
            //Console.WriteLine(System.Convert.ToString(data));
            ////Console.WriteLine(hi.Replace("-", ""));
            //return data;
            
            // dispose memory stream once done processing it
            using (var memstrm = new MemoryStream())
            {
                
                BinaryFormatter formatter = new BinaryFormatter();
                // serialize the packet into the stream
                formatter.Serialize(memstrm, packet);
                byte[] data = memstrm.ToArray();

                //string hi = BitConverter.ToString(data);
                Console.WriteLine(data);
                //Console.WriteLine(System.Convert.ToString(data));

                return data;
                //return memstrm.GetBuffer();
            }
        }
    }
}

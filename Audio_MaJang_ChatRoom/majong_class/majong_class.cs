using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace majong_class
{
    [Serializable]
    public class majong_card
    {
        public int[] allCard = new int[144];
        public int[] p1_card = new int[16];
        public int[] p2_card = new int[16];
        public int[] p3_card = new int[16];
        public int[] p4_card = new int[16];

        public Byte[] PacketToByte2()//將Packet轉換為Byte型態 方便傳送
        {
            BinaryFormatter bf2 = new BinaryFormatter();
            MemoryStream ms2 = new MemoryStream();

            bf2.Serialize(ms2, this);
            byte[] Bytes = ms2.ToArray();//將本類別序列化至ms
            ms2.Close();
            return Bytes;
        }
        public majong_card ByteToPacket2(Byte[] ByteOfPacket)
        {
            BinaryFormatter bf2 = new BinaryFormatter();
            MemoryStream ms2 = new MemoryStream(ByteOfPacket);
            majong_card TmpPkt = (majong_card)bf2.Deserialize(ms2);//將ms解序列化至TmpPkt
            ms2.Close();
            this.allCard = TmpPkt.allCard;
            this.p1_card = TmpPkt.p1_card;
            this.p2_card = TmpPkt.p2_card;
            this.p3_card = TmpPkt.p3_card;
            this.p4_card = TmpPkt.p4_card;
            return TmpPkt;
        }
    }
}

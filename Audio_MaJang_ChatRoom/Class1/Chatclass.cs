using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;

namespace Chatclass
{
    [Serializable]
    public class Packet
    {
        public String PlayerName;
        public String WordToSend;
        public int[] allCard = new int[144];
        public int[] p1_card = new int[17];
        public int[] p2_card = new int[17];
        public int[] p3_card = new int[17];
        public int[] p4_card = new int[17];
        public int[] p1_flower = new int[9];
        public int[] p2_flower = new int[9];
        public int[] p3_flower = new int[9];
        public int[] p4_flower = new int[9];
        public int sendedCard;
        public int send_player;
        public int someonePon;
        public int eaten_player;
        public int[] eated_card = new int[3];
        public int someoneWin;

        public Byte[] PacketToByte()//將Packet轉換為Byte型態 方便傳送
        {
            BinaryFormatter bf = new BinaryFormatter();
            MemoryStream ms = new MemoryStream();

            bf.Serialize(ms, this);
            byte[] Bytes = ms.ToArray();//將本類別序列化至ms
            ms.Close();
            return Bytes;
        }
        public Packet ByteToPacket(Byte[] ByteOfPacket)
        {
            BinaryFormatter bf = new BinaryFormatter();
            MemoryStream ms = new MemoryStream(ByteOfPacket);
            Packet TmpPkt = (Packet)bf.Deserialize(ms);//將ms解序列化至TmpPkt
            ms.Close();

            this.PlayerName = TmpPkt.PlayerName;
            this.WordToSend = TmpPkt.WordToSend;
            this.allCard = TmpPkt.allCard;
            this.p1_card = TmpPkt.p1_card; this.p1_flower = TmpPkt.p1_flower;
            this.p2_card = TmpPkt.p2_card; this.p2_flower = TmpPkt.p2_flower;
            this.p3_card = TmpPkt.p3_card; this.p3_flower = TmpPkt.p3_flower;
            this.p4_card = TmpPkt.p4_card; this.p4_flower = TmpPkt.p4_flower;
            this.sendedCard = TmpPkt.sendedCard;
            this.send_player = TmpPkt.send_player;
            this.eaten_player = TmpPkt.eaten_player;
            this.someonePon = TmpPkt.someonePon;
            this.eated_card = TmpPkt.eated_card;
            this.someoneWin = TmpPkt.someoneWin;
            return TmpPkt;
        }
    }

}

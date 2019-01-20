using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Chatclass;

namespace MahjongServer
{
    public partial class Form1 : Form
    {
        
        TcpListener Listener,Listener2;
        TcpClient[] Clients,clients2;
        NetworkStream[] Mystream,Mystream2;
        Thread _thread1, _thread2, _thread3, _thread4;
        Thread _threadPlayer1, _threadPlayer2, _threadPlayer3, _threadPlayer4;
        //Random random = new Random();
        Packet Info = new Packet();
        Packet RcvInfo = new Packet();
        int p1flower = 1, p2flower = 1, p3flower = 1,p4flower = 1;

        public Form1()
        {
            InitializeComponent();
            StartListener();
        }
        private void StartListener()
        {
            Listener = new TcpListener(new IPEndPoint(IPAddress.Any, 6666));
            Listener2 = new TcpListener(new IPEndPoint(IPAddress.Any, 5555));
            Listener.Start();
            Listener2.Start();
            AcceptClients();
        }
        private void AcceptClients()
        {
            Clients = new TcpClient[4];
            clients2 = new TcpClient[4];
            Mystream = new NetworkStream[4];
            Mystream2 = new NetworkStream[4];
            for (int i = 0; i < 4; i++)
            {
                Clients[i] = Listener.AcceptTcpClient();
                Mystream[i] = Clients[i].GetStream();
            }
            for (int i = 0; i < 4; i++)
            {
                clients2[i] = Listener2.AcceptTcpClient();
                Mystream2[i] = clients2[i].GetStream();
            }
            MessageBox.Show("Connected 4 clients");

            givePlayerOrder();
            giveInitialCard();

            _thread1 = new Thread(thread1);
            _thread1.Start();
            _thread2 = new Thread(thread2);
            _thread2.Start();
            _thread3 = new Thread(thread3);
            _thread3.Start();
            _thread4 = new Thread(thread4);
            _thread4.Start();
            _threadPlayer1 = new Thread(threadPlayer1);
            _threadPlayer1.Start();
            _threadPlayer2 = new Thread(threadPlayer2);
            _threadPlayer2.Start();
            _threadPlayer3 = new Thread(threadPlayer3);
            _threadPlayer3.Start();
            _threadPlayer4 = new Thread(threadPlayer4);
            _threadPlayer4.Start();
        }
        private void givePlayerOrder()
        {
            for(int i = 0; i < 4; i++)
            {
                Byte[] player = System.BitConverter.GetBytes(i+1);
                Mystream2[i].Flush();
                Mystream2[i].Write(player, 0, player.Length);
            }
        }
        private void giveInitialCard()
        {
            for (int i = 0; i < 144; i++) { Info.allCard[i] = 0; }
            for(int i = 0; i < 9; i++) { Info.p1_flower[i] = 0; Info.p2_flower[i] = 0; Info.p3_flower[i] = 0; Info.p4_flower[i] = 0; }
            // 隨機發送16張牌 給每個玩家 並判斷是否有花
            for (int i = 0; i < 64; i++)
            {
                Random random = new Random();
                int a = random.Next() % 144;
                if (Info.allCard[a] == 0)
                {
                    if (i < 16)
                    {
                        if (a < 136)
                        { Info.allCard[a] = 1; Info.p1_card[i] = a; }
                        else if (a >= 136) {
                            Info.allCard[a] = 1;
                            Info.p1_flower[0] = p1flower;
                            Info.p1_flower[p1flower] = a;
                            p1flower++;i--;
                        }
                    }
                    else if (i >= 16 && i < 32) {
                        if(a < 136)
                        { Info.allCard[a] = 2; Info.p2_card[i - 16] = a; }
                        else if( a >= 136)
                        {
                            Info.allCard[a] = 2;
                            Info.p2_flower[0] = p2flower;
                            Info.p2_flower[p2flower] = a;
                            p2flower++; i--;
                        }
                    }
                    else if (i >= 32 && i < 48) {
                        if (a < 136) { Info.allCard[a] = 3; Info.p3_card[i - 32] = a; }
                        else if (a >= 136)
                        {
                            Info.allCard[a] = 3;
                            Info.p3_flower[0] = p3flower;
                            Info.p3_flower[p3flower] = a;
                            p3flower++; i--;
                        }
                    }
                    else {
                        if (a < 136) { Info.allCard[a] = 4; Info.p4_card[i - 48] = a; }
                        else if (a >= 136)
                        {
                            Info.allCard[a] = 4;
                            Info.p4_flower[0] = p4flower;
                            Info.p4_flower[p4flower] = a;
                            p4flower++; i--;
                        }
                    }
                }
                else
                    i--;
            }
            for (int i = 0; i < 4; i++)
            {
                Byte[] SndBuf = new Byte[clients2[i].SendBufferSize];
                SndBuf = Info.PacketToByte();
                Mystream2[i].Flush();
                Mystream2[i].Write(SndBuf, 0, SndBuf.Length);
            }
        }

        private void thread1()
        {
            while (true)
            {
                Byte[] RcvBuf = new Byte[Clients[0].SendBufferSize];
                Mystream[0].Read(RcvBuf, 0, RcvBuf.Length);
                for (int i = 0; i < 4; i++)
                {
                    Byte[] SndBuf = new Byte[Clients[i].SendBufferSize];
                    SndBuf = RcvBuf;
                    Mystream[i].Flush();
                    Mystream[i].Write(SndBuf, 0, SndBuf.Length);
                }
            }
        }
        private void thread2()
        {
            while (true)
            {
                Byte[] RcvBuf = new Byte[Clients[1].SendBufferSize];
                Mystream[1].Read(RcvBuf, 0, RcvBuf.Length);
                for (int i = 0; i < 4; i++)
                {
                    Byte[] SndBuf = new Byte[Clients[i].SendBufferSize];
                    SndBuf = RcvBuf;
                    Mystream[i].Flush();
                    Mystream[i].Write(SndBuf, 0, SndBuf.Length);
                }
            }
        }
        private void thread3()
        {
            while (true)
            {
                Byte[] RcvBuf = new Byte[Clients[2].SendBufferSize];
                Mystream[2].Read(RcvBuf, 0, RcvBuf.Length);
                for (int i = 0; i < 4; i++)
                {
                    Byte[] SndBuf = new Byte[Clients[i].SendBufferSize];
                    SndBuf = RcvBuf;
                    Mystream[i].Flush();
                    Mystream[i].Write(SndBuf, 0, SndBuf.Length);
                }
            }
        }
        private void thread4()
        {
            while (true)
            {
                Byte[] RcvBuf = new Byte[Clients[3].SendBufferSize];
                Mystream[3].Read(RcvBuf, 0, RcvBuf.Length);
                for (int i = 0; i < 4; i++)
                {
                    Byte[] SndBuf = new Byte[Clients[i].SendBufferSize];
                    SndBuf = RcvBuf;
                    Mystream[i].Flush();
                    Mystream[i].Write(SndBuf, 0, SndBuf.Length);
                }
            }
        }
        /*------------------------------------------------------------------------------------------------------------------------------------*/
        private void threadPlayer1()
        {
            while (true)
            {
                Packet TmpPkt = new Packet();
                Byte[] RcvBuf = new Byte[clients2[0].SendBufferSize];
                Mystream2[0].Read(RcvBuf, 0, RcvBuf.Length);
                //TmpPkt = RcvInfo.ByteToPacket(RcvBuf);
                //Info.p1_card = RcvInfo.p1_card;
                for (int i = 0; i < 4; i++)
                {
                    Byte[] SndBuf = new Byte[clients2[i].SendBufferSize];
                    SndBuf = RcvBuf;
                    Mystream2[i].Flush();
                    Mystream2[i].Write(SndBuf, 0, SndBuf.Length);
                }
            }
        }
        private void threadPlayer2()
        {
            while (true)
            {
                Packet TmpPkt = new Packet();
                Byte[] RcvBuf = new Byte[clients2[1].SendBufferSize];
                Mystream2[1].Read(RcvBuf, 0, RcvBuf.Length);
                //TmpPkt = RcvInfo.ByteToPacket(RcvBuf);
                //Info.p2_card = RcvInfo.p2_card;
                for (int i = 0; i < 4; i++)
                {
                    Byte[] SndBuf = new Byte[clients2[i].SendBufferSize];
                    SndBuf = RcvBuf;
                    Mystream2[i].Flush();
                    Mystream2[i].Write(SndBuf, 0, SndBuf.Length);
                }
            }
        }
        private void threadPlayer3()
        {
            while (true)
            {
                Packet TmpPkt = new Packet();
                Byte[] RcvBuf = new Byte[clients2[2].SendBufferSize];
                Mystream2[2].Read(RcvBuf, 0, RcvBuf.Length);
                //TmpPkt = RcvInfo.ByteToPacket(RcvBuf);
                //Info.p3_card = RcvInfo.p3_card;
                for (int i = 0; i < 4; i++)
                {
                    Byte[] SndBuf = new Byte[clients2[i].SendBufferSize];
                    SndBuf = RcvBuf;
                    Mystream2[i].Flush();
                    Mystream2[i].Write(SndBuf, 0, SndBuf.Length);
                }
            }
        }
        private void threadPlayer4()
        {
            while (true)
            {
                Packet TmpPkt = new Packet();
                Byte[] RcvBuf = new Byte[clients2[3].SendBufferSize];
                Mystream2[3].Read(RcvBuf, 0, RcvBuf.Length);
                //TmpPkt = RcvInfo.ByteToPacket(RcvBuf);
                //Info.p4_card = RcvInfo.p4_card;
                for (int i = 0; i < 4; i++)
                {
                    Byte[] SndBuf = new Byte[clients2[i].SendBufferSize];
                    SndBuf = RcvBuf;
                    Mystream2[i].Flush();
                    Mystream2[i].Write(SndBuf, 0, SndBuf.Length);
                }
            }
        }
    }
}

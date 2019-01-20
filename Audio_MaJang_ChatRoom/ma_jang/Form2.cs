using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Chatclass;
using System.Net.Sockets;
using System.Speech;
using System.Speech.Recognition;
using System.Speech.Synthesis;
using System.Threading;

namespace ma_jang
{
    public partial class Form2 : Form
    {
        TcpClient clientsocket, clientsocket2;
        NetworkStream Mystream, Mystream2;
        Packet MyInfo, OthersInfo;
        Button[] Majong_button = new Button[16], flower = new Button[8];
        Button[] Front_Player = new Button[16], Front_sended = new Button[20];
        Button[] Right_Player = new Button[16], Right_sended = new Button[20];
        Button[] Left_Player = new Button[16], Left_sended = new Button[20];
        Button[] My_sended = new Button[20];
        Button[] Image_button = new Button[17];
        int player = 0, pre_player = 0, eatPlayer, PonOrNot;
        int WinOrNot = 0, p1Win = 0, p2Win = 0, p3Win = 0, p4Win = 0;
        int win_card = 0;
        int[] mycard = new int[17], myflower = new int[9];
        int[] allcard = new int[144];
        int[] p1_card = new int[17], p2_card = new int[17], p3_card = new int[17], p4_card = new int[17];
        string[] mycard_str = new string[17], myflower_str = new string[8];
        Layout_Info layoutInfo = new Layout_Info();

        private int myTurn = 0;
        string ReString;
        int ReNumber;
        int HandleNum = -1;
        int ListenNum = -1;


        public Form2(string IdFromForm1)
        {
            InitializeComponent();


            for (int i = 0; i < 17; i++)
            {
                Image_button[i] = new Button();
            }
            MyInfo = new Packet();
            MyInfo.PlayerName = IdFromForm1;
            //MessageBox.Show(MyInfo.PlayerName);
            OthersInfo = new Packet();

            connectToServer();

            set_MajongButton();
            set_FrontPlayer();
            set_RightPlayer();
            set_LeftPlayer();
            set_mysend();
            player = receiveOrder();
            receiveCard();
            //set_MajongButton();
            backgroundWorker1.RunWorkerAsync();
            backgroundWorker2.RunWorkerAsync();

            button17.Visible = false;
            button17.Text = "";
            button1.Visible = false;
            button2.Visible = false;
            button3.Visible = false;
            Pon_button.Visible = false;
            NotPon_button.Visible = false;
            eat_button.Visible = false;
            Noteat_button.Visible = false;
            Leat_button.Visible = false;
            Meat_button.Visible = false;
            Reat_button.Visible = false;
        }


        //---------------------------------------------------------------------------------
        //聲控宣告
        SpeechSynthesizer sSynth = new SpeechSynthesizer();
        PromptBuilder pBuilder = new PromptBuilder();
        SpeechRecognitionEngine sRecognize = new SpeechRecognitionEngine();
        //---------------------------------------------------------------------------------



        private void connectToServer()
        {
            clientsocket = new TcpClient();
            clientsocket.Connect("192.168.1.242", 6666);
            Mystream = clientsocket.GetStream();
            clientsocket2 = new TcpClient();
            clientsocket2.Connect("192.168.1.242", 5555);
            Mystream2 = clientsocket2.GetStream();
        }
        private int receiveOrder()
        {
            Byte[] order = new Byte[4];
            Mystream2.Read(order, 0, order.Length);
            return System.BitConverter.ToInt32(order, 0);
        }
        private void receiveCard()
        {
            Packet tmp = new Packet();
            Byte[] RcvBuf = new Byte[clientsocket2.SendBufferSize];
            Mystream2.Read(RcvBuf, 0, RcvBuf.Length);
            tmp = OthersInfo.ByteToPacket(RcvBuf);
            switch (player)
            {
                case 1:
                    myOrder.Text = "東";
                    allcard = OthersInfo.allCard;
                    mycard = OthersInfo.p1_card;
                    myflower = OthersInfo.p1_flower;
                    break;
                case 2:
                    myOrder.Text = "南";
                    allcard = OthersInfo.allCard;
                    mycard = OthersInfo.p2_card;
                    myflower = OthersInfo.p2_flower;
                    break;
                case 3:
                    myOrder.Text = "西";
                    allcard = OthersInfo.allCard;
                    mycard = OthersInfo.p3_card;
                    myflower = OthersInfo.p3_flower;
                    break;
                case 4:
                    myOrder.Text = "北";
                    allcard = OthersInfo.allCard;
                    mycard = OthersInfo.p4_card;
                    myflower = OthersInfo.p4_flower;
                    break;
            }
            for (int i = 15; i >= 0; i--)
            {
                for (int j = 0; j < i; j++)
                {
                    if (mycard[j] > mycard[j + 1])
                    {
                        int temp;
                        temp = mycard[j];
                        mycard[j] = mycard[j + 1];
                        mycard[j + 1] = temp;
                    }
                }
            }
            if (myflower[0] != 0)
            {
                for (int i = myflower[0]; i > 0; i--)
                {
                    for (int j = 1; j < i; j++)
                    {
                        if (myflower[j] > myflower[j + 1])
                        {
                            int temp;
                            temp = myflower[j];
                            myflower[j] = myflower[j + 1];
                            myflower[j + 1] = temp;
                        }
                    }
                }
            }
            for (int i = 0; i < 16; i++)
            {
                getcardstring(i);
            }
            for (int i = 1; i < (myflower[0] + 1); i++)
            {
                getflowerstring(i);
                int x = 80 + i * 50;
                flower[i - 1] = new Button();
                this.flower[i - 1].Location = new System.Drawing.Point(x, 650);
                this.flower[i - 1].Name = "flower" + i;
                this.flower[i - 1].Size = new System.Drawing.Size(46, 61);
                this.flower[i - 1].TabIndex = 32;
                this.flower[i - 1].Text = myflower_str[i];
                set_flower_picture(i - 1, myflower_str[i]);  //set flower picture

                this.flower[i - 1].UseVisualStyleBackColor = true;
                Controls.Add(flower[i - 1]);
                flower[i - 1].BringToFront();
            }
            for (int i = 0; i < 16; i++)
            {
                Majong_button[i].BackgroundImage = Image_button[i].BackgroundImage;
                Majong_button[i].BackgroundImageLayout = ImageLayout.Stretch;
                Majong_button[i].TabIndex = i + (i * 1000);
            }
        }                                     //new
        private void getcardstring(int i)
        {
            if (mycard[i] <= 35) { mycard_str[i] = Convert.ToString(mycard[i] / 4 + 1) + "萬"; set_picture(i, "_" + mycard_str[i]); }
            else if (mycard[i] > 35 && mycard[i] <= 71) { mycard_str[i] = Convert.ToString((mycard[i] - 36) / 4 + 1) + "筒"; set_picture(i, "_" + mycard_str[i]); }
            else if (mycard[i] > 71 && mycard[i] <= 107) { mycard_str[i] = Convert.ToString((mycard[i] - 72) / 4 + 1) + "條"; set_picture(i, "_" + mycard_str[i]); }
            else if (mycard[i] > 107)
            {
                if (mycard[i] > 107 && mycard[i] <= 111) { mycard_str[i] = "東風"; set_picture(i, "東風"); }
                else if (mycard[i] > 111 && mycard[i] <= 115) { mycard_str[i] = "南風"; set_picture(i, "南風"); }
                else if (mycard[i] > 115 && mycard[i] <= 119) { mycard_str[i] = "西風"; set_picture(i, "西風"); }
                else if (mycard[i] > 119 && mycard[i] <= 123) { mycard_str[i] = "北風"; set_picture(i, "北風"); }
                else if (mycard[i] > 123 && mycard[i] <= 127) { mycard_str[i] = "紅中"; set_picture(i, "紅中"); }
                else if (mycard[i] > 127 && mycard[i] <= 131) { mycard_str[i] = "發財"; set_picture(i, "發財"); }
                else if (mycard[i] > 131 && mycard[i] <= 135) { mycard_str[i] = "白板"; set_picture(i, "白板"); }
            }
            //else if (myflower[i] > 135 && myflower[i] <= 139) { myflower_str[i] = "紅" + Convert.ToString((myflower[i] - 135)) + "花"; }
            //else if (myflower[i] > 139 && myflower[i] <= 143) { myflower_str[i] = "黑" + Convert.ToString((myflower[i] - 139)) + "花"; }
        }                              //new
        private void getflowerstring(int i)
        {
            if (myflower[i] > 135 && myflower[i] <= 139) { myflower_str[i] = "紅" + Convert.ToString((myflower[i] - 135)); set_picture(i, "紅" + Convert.ToString((myflower[i] - 135)) + "花"); }
            else if (myflower[i] > 139 && myflower[i] <= 143) { myflower_str[i] = "黑" + Convert.ToString((myflower[i] - 139)); set_picture(i, "黑" + Convert.ToString((myflower[i] - 135)) + "花"); }
        }                            //new
        private void set_MajongButton()
        {
            for (int i = 0; i < 16; i++)
            {
                int x = 30 + i * 60;
                Majong_button[i] = new Button();
                this.Majong_button[i].Location = new System.Drawing.Point(x, 750);
                this.Majong_button[i].Name = "button" + i;
                this.Majong_button[i].Size = new System.Drawing.Size(53, 75);

                //this.Majong_button[i].TabIndex = mycard[i];
                this.Majong_button[i].UseVisualStyleBackColor = true;
                this.Majong_button[i].Click += new System.EventHandler(this.button1_Click);
                Controls.Add(Majong_button[i]);
                Majong_button[i].BringToFront();
                this.Majong_button[i].MouseLeave += new System.EventHandler(this.button1_MouseLeave);
                this.Majong_button[i].MouseMove += new System.Windows.Forms.MouseEventHandler(this.button1_MouseMove);
            }
        }
        private void set_FrontPlayer()
        {
            for (int i = 0; i < 16; i++)
            {
                int x = 190 + (i * 42);
                Front_Player[i] = new Button();
                this.Front_Player[i].Location = new System.Drawing.Point(x, 65);
                this.Front_Player[i].Name = "Front_Player" + i;
                this.Front_Player[i].Size = new System.Drawing.Size(42, 51);
                this.Front_Player[i].TabIndex = 32;
                this.Front_Player[i].UseVisualStyleBackColor = true;
                Front_Player[i].Text = "";
                Controls.Add(Front_Player[i]);
                Front_Player[i].BringToFront();
            }
            for (int i = 0; i < 20; i++)
            {
                if (i < 10)
                {
                    int x = 305 + (i * 36);
                    Front_sended[i] = new Button();
                    this.Front_sended[i].Location = new System.Drawing.Point(x, 170);
                    this.Front_sended[i].Name = "Front_sended" + i;
                    this.Front_sended[i].Size = new System.Drawing.Size(36, 39);
                    this.Front_sended[i].TabIndex = 48;
                    this.Front_sended[i].Text = "";
                    this.Front_sended[i].UseVisualStyleBackColor = true;
                    Front_sended[i].Visible = false;
                    Controls.Add(Front_sended[i]);
                    Front_sended[i].BringToFront();
                }
                else
                {
                    int x = 305 + ((i - 10) * 36);
                    Front_sended[i] = new Button();
                    this.Front_sended[i].Location = new System.Drawing.Point(x, 210);
                    this.Front_sended[i].Name = "Front_sended" + i;
                    this.Front_sended[i].Size = new System.Drawing.Size(36, 39);
                    this.Front_sended[i].TabIndex = 48;
                    this.Front_sended[i].Text = "";
                    this.Front_sended[i].UseVisualStyleBackColor = true;
                    Front_sended[i].Visible = false;
                    Controls.Add(Front_sended[i]);
                    Front_sended[i].BringToFront();
                }
            }
        }
        private void set_RightPlayer()
        {
            for (int i = 0; i < 16; i++)
            {
                int y = 70 + (i * 35);
                Right_Player[i] = new Button();
                this.Right_Player[i].Location = new System.Drawing.Point(975, y);
                this.Right_Player[i].Name = "Right_Player" + i;
                this.Right_Player[i].Size = new System.Drawing.Size(60, 34);
                this.Right_Player[i].TabIndex = 32;
                Right_Player[i].Text = "";
                this.Right_Player[i].UseVisualStyleBackColor = true;
                Controls.Add(Right_Player[i]);
                Right_Player[i].BringToFront();
            }
            for (int i = 0; i < 20; i++)
            {
                if (i < 7)
                {
                    int y = 160 + (i * 39);
                    Right_sended[i] = new Button();
                    this.Right_sended[i].Location = new System.Drawing.Point(860, y);
                    this.Right_sended[i].Name = "Right_sended" + i;
                    this.Right_sended[i].Size = new System.Drawing.Size(36, 39);
                    this.Right_sended[i].TabIndex = 48;
                    this.Right_sended[i].Text = "";
                    this.Right_sended[i].UseVisualStyleBackColor = true;
                    Controls.Add(Right_sended[i]);
                    Right_sended[i].Visible = false;
                    Right_sended[i].BringToFront();
                }
                else if (i >= 7 && i < 14)
                {
                    int y = 160 + ((i - 7) * 39);
                    Right_sended[i] = new Button();
                    this.Right_sended[i].Location = new System.Drawing.Point(824, y);
                    this.Right_sended[i].Name = "Right_sended" + i;
                    this.Right_sended[i].Size = new System.Drawing.Size(36, 39);
                    this.Right_sended[i].TabIndex = 48;
                    this.Right_sended[i].Text = "";
                    this.Right_sended[i].UseVisualStyleBackColor = true;
                    Controls.Add(Right_sended[i]);
                    Right_sended[i].Visible = false;
                    Right_sended[i].BringToFront();
                }
                else
                {
                    int y = 160 + ((i - 14) * 39);
                    Right_sended[i] = new Button();
                    this.Right_sended[i].Location = new System.Drawing.Point(788, y);
                    this.Right_sended[i].Name = "Right_sended" + i;
                    this.Right_sended[i].Size = new System.Drawing.Size(36, 39);
                    this.Right_sended[i].TabIndex = 48;
                    this.Right_sended[i].Text = "";
                    this.Right_sended[i].UseVisualStyleBackColor = true;
                    Controls.Add(Right_sended[i]);
                    Right_sended[i].Visible = false;
                    Right_sended[i].BringToFront();
                }
            }
        }
        private void set_LeftPlayer()
        {
            for (int i = 0; i < 16; i++)
            {
                int y = 80 + (i * 35);
                Left_Player[i] = new Button();
                this.Left_Player[i].Location = new System.Drawing.Point(65, y);
                this.Left_Player[i].Name = "Left_Player" + i;
                this.Left_Player[i].Size = new System.Drawing.Size(60, 34);
                this.Left_Player[i].TabIndex = 32;
                Left_Player[i].Text = "";
                this.Left_Player[i].UseVisualStyleBackColor = true;
                Controls.Add(Left_Player[i]);
                Left_Player[i].BringToFront();
            }
            for (int i = 0; i < 20; i++)
            {
                if (i < 7)
                {
                    int y = 270 + (i * 39);
                    Left_sended[i] = new Button();
                    this.Left_sended[i].Location = new System.Drawing.Point(165, y);
                    this.Left_sended[i].Name = "Left_sended" + i;
                    this.Left_sended[i].Size = new System.Drawing.Size(36, 39);
                    this.Left_sended[i].TabIndex = 49;
                    this.Left_sended[i].Text = "";
                    this.Left_sended[i].UseVisualStyleBackColor = true;
                    Controls.Add(Left_sended[i]);
                    Left_sended[i].Visible = false;
                    Left_sended[i].BringToFront();
                }
                else if (i >= 7 && i < 14)
                {
                    int y = 270 + ((i - 7) * 39);
                    Left_sended[i] = new Button();
                    this.Left_sended[i].Location = new System.Drawing.Point(201, y);
                    this.Left_sended[i].Name = "Left_sended" + i;
                    this.Left_sended[i].Size = new System.Drawing.Size(36, 39);
                    this.Left_sended[i].TabIndex = 49;
                    this.Left_sended[i].Text = "";
                    this.Left_sended[i].UseVisualStyleBackColor = true;
                    Controls.Add(Left_sended[i]);
                    Left_sended[i].Visible = false;
                    Left_sended[i].BringToFront();
                }
                else
                {
                    int y = 270 + ((i - 14) * 39);
                    Left_sended[i] = new Button();
                    this.Left_sended[i].Location = new System.Drawing.Point(237, y);
                    this.Left_sended[i].Name = "Left_sended" + i;
                    this.Left_sended[i].Size = new System.Drawing.Size(36, 39);
                    this.Left_sended[i].TabIndex = 49;
                    this.Left_sended[i].Text = "";
                    this.Left_sended[i].UseVisualStyleBackColor = true;
                    Controls.Add(Left_sended[i]);
                    Left_sended[i].Visible = false;
                    Left_sended[i].BringToFront();
                }
            }
        }
        private bool set_picture(int i, string channel)
        {
            object o = Properties.Resources.ResourceManager.GetObject(channel);

            if (o is Image)
            {
                if (i == 16) {
                    Image_button[16].BackgroundImage = o as Image;
                    Image_button[16].BackgroundImageLayout = ImageLayout.Stretch;
                    Image_button[16].Text = "";
                }
                else
                {
                    Image_button[i].BackgroundImage = o as Image;
                    Image_button[i].BackgroundImageLayout = ImageLayout.Stretch;
                    Image_button[i].Text = "";
                    return true;
                }

            }
            return false;
        }               //add
        private bool set_flower_picture(int i, string channel)
        {
            object o = Properties.Resources.ResourceManager.GetObject(channel);

            if (o is Image)
            {
                if (i == 16) { button17.Image = o as Image; }
                else
                {
                    this.flower[i].BackgroundImage = o as Image;
                    this.flower[i].BackgroundImageLayout = ImageLayout.Stretch;
                    flower[i].Text = "";
                    return true;
                }

            }
            return false;
        }        //add


        private void set_mysend()
        {
            for (int i = 0; i < 20; i++)
            {
                if (i < 10)
                {
                    int x = 305 + (i * 36);
                    My_sended[i] = new Button();
                    this.My_sended[i].Location = new System.Drawing.Point(x, 570);
                    this.My_sended[i].Name = "My_sended" + i;
                    this.My_sended[i].Size = new System.Drawing.Size(36, 39);
                    this.My_sended[i].TabIndex = 48;
                    this.My_sended[i].Text = "";
                    this.My_sended[i].UseVisualStyleBackColor = true;
                    My_sended[i].Visible = false;
                    Controls.Add(My_sended[i]);
                    My_sended[i].BringToFront();
                }
                else
                {
                    int x = 305 + ((i - 10) * 36);
                    My_sended[i] = new Button();
                    this.My_sended[i].Location = new System.Drawing.Point(x, 530);
                    this.My_sended[i].Name = "My_sended" + i;
                    this.My_sended[i].Size = new System.Drawing.Size(36, 39);
                    this.My_sended[i].TabIndex = 48;
                    this.My_sended[i].Text = "";
                    this.My_sended[i].UseVisualStyleBackColor = true;
                    My_sended[i].Visible = false;
                    Controls.Add(My_sended[i]);
                    My_sended[i].BringToFront();
                }
            }
        }



        private void button1_MouseMove(object sender, MouseEventArgs e)
        {
            int num = ((Button)(sender)).TabIndex / 1000;
            Majong_button[num].Location = new Point((30 + 60 * num), 730);
        }

        private void button1_MouseLeave(object sender, EventArgs e)
        {
            int num = ((Button)(sender)).TabIndex / 1000;
            Majong_button[num].Location = new Point((30 + 60 * num), 750);
        }

        /*  private void button4_MouseDown(object sender, MouseEventArgs e)
          {
              button4.Enabled = false;
              button4.Enabled = true;
              Choices sList = new Choices();
              sList.Add(new string[] {
                  "一條","二條","三條","四條","五條","六條","七條","八條","九條",
                  "一索","二索","三索","四索","五索","六索","七索","八索","九索",
                  "碰", "吃", "聽", "胡",
                  "一萬","二萬","三萬","四萬","五萬","六萬","七萬","八萬","九萬",
                  "一筒","二筒","三筒","四筒","五筒","六筒","七筒","八筒","九筒",
                  "紅中","東風","西風","南風","北風","白板","發財" });//"補花",
                 // "幹你娘","靠北","幹","幹你媽","你娘機掰","機掰","操你媽","你娘"});
              Grammar gr = new Grammar(new GrammarBuilder(sList));

              try
              {
                  sRecognize.RequestRecognizerUpdate();
                  sRecognize.LoadGrammar(gr);
                  sRecognize.SpeechRecognized += sRecognize_SpeechRecognized;
                  sRecognize.SetInputToDefaultAudioDevice();
                  sRecognize.RecognizeAsync(RecognizeMode.Multiple);
              }
              catch
              {
                  return;
              }
          }*/


        private void sRecognize_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            ReString = e.Result.Text.ToString();
            if (ReString == "一萬") ReNumber = 0;
            if (ReString == "二萬") ReNumber = 4;
            if (ReString == "三萬") ReNumber = 8;
            if (ReString == "四萬") ReNumber = 12;
            if (ReString == "五萬") ReNumber = 16;
            if (ReString == "六萬") ReNumber = 20;
            if (ReString == "七萬") ReNumber = 24;
            if (ReString == "八萬") ReNumber = 28;
            if (ReString == "九萬") ReNumber = 32;
            if (ReString == "一筒") ReNumber = 36;
            if (ReString == "二筒") ReNumber = 40;
            if (ReString == "三筒") ReNumber = 44;
            if (ReString == "四筒") ReNumber = 48;
            if (ReString == "五筒") ReNumber = 52;
            if (ReString == "六筒") ReNumber = 56;
            if (ReString == "七筒") ReNumber = 60;
            if (ReString == "八筒") ReNumber = 64;
            if (ReString == "九筒") ReNumber = 68;
            if (ReString == "一條" || ReString == "一索") ReNumber = 72;
            if (ReString == "二條" || ReString == "二索") ReNumber = 76;
            if (ReString == "三條" || ReString == "三索") ReNumber = 80;
            if (ReString == "四條" || ReString == "四索") ReNumber = 84;
            if (ReString == "五條" || ReString == "五索") ReNumber = 88;
            if (ReString == "六條" || ReString == "六索") ReNumber = 92;
            if (ReString == "七條" || ReString == "七索") ReNumber = 96;
            if (ReString == "八條" || ReString == "八索") ReNumber = 100;
            if (ReString == "九條" || ReString == "九索") ReNumber = 104;
            if (ReString == "東風") ReNumber = 108;
            if (ReString == "南風") ReNumber = 112;
            if (ReString == "西風") ReNumber = 116;
            if (ReString == "北風") ReNumber = 120;
            if (ReString == "紅中") ReNumber = 124;
            if (ReString == "發財") ReNumber = 128;
            if (ReString == "白板") ReNumber = 132;



            for (int i = 0; i < 16; i++)
            {
                if (mycard[i] == ReNumber||mycard[i]==ReNumber+1 || mycard[i] == ReNumber + 2 || mycard[i] == ReNumber + 3)
                {

                    ListenNum = i;
                    this.button1_Click(button1, EventArgs.Empty);
                    break;
                   
                }

            }

          /*  if (ListenNum != -1)
            {
                this.button1_Click(button1, EventArgs.Empty);
            }*/
        }
    
        private void button5_Click(object sender, EventArgs e)
        {
            sRecognize.RecognizeAsyncStop();
            button4.Enabled = true;
            button5.Enabled = false;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            button4.Enabled = false;
            button5.Enabled = true;
            Choices sList = new Choices();
            sList.Add(new string[] {
                "一條","二條","三條","四條","五條","六條","七條","八條","九條",
                "一索","二索","三索","四索","五索","六索","七索","八索","九索",
             //   "碰", "吃", "聽", "胡",
                "一萬","二萬","三萬","四萬","五萬","六萬","七萬","八萬","九萬",
                "一筒","二筒","三筒","四筒","五筒","六筒","七筒","八筒","九筒",
                "紅中","東風","西風","南風","北風","白板","發財" });//"補花",
                                                      // "幹你娘","靠北","幹","幹你媽","你娘機掰","機掰","操你媽","你娘"});
            Grammar gr = new Grammar(new GrammarBuilder(sList));

            try
            {
                sRecognize.RequestRecognizerUpdate();
                sRecognize.LoadGrammar(gr);
                sRecognize.SpeechRecognized += sRecognize_SpeechRecognized;
                sRecognize.SetInputToDefaultAudioDevice();
                sRecognize.RecognizeAsync(RecognizeMode.Multiple);
            }
            catch
            {
                return;
            }
        }


        //---------------------------------------------------------------------------------

        private void button1_Click(object sender, EventArgs e)
        {

            if (ListenNum != -1&&button4.Enabled==false)
            {
                if (mycard[ListenNum] == ReNumber || mycard[ListenNum] == ReNumber + 1 || mycard[ListenNum] == ReNumber + 2 || mycard[ListenNum] == ReNumber + 3)
                { 
                    int whoCanPon_Listen;
                    allcard[mycard[ListenNum]] += 10;

                    whoCanPon_Listen = PonCard(mycard[ListenNum]);

                    MyInfo.allCard = allcard;
                    MyInfo.sendedCard = mycard[ListenNum];
                    MyInfo.send_player = player;
                    MyInfo.someonePon = whoCanPon_Listen;
                    MyInfo.eaten_player = 0;
                    MyInfo.someoneWin = 0;

                    Byte[] SndBuf_Listen = new Byte[clientsocket2.SendBufferSize];
                    SndBuf_Listen = MyInfo.PacketToByte();
                    Mystream2.Flush();
                    Mystream2.Write(SndBuf_Listen, 0, SndBuf_Listen.Length);
                    //輸出打出的排在自己的前面
                    My_sended[layoutInfo.MySendNum].BackgroundImage = Image_button[ListenNum].BackgroundImage;
                    My_sended[layoutInfo.MySendNum].BackgroundImageLayout = ImageLayout.Stretch;
                    My_sended[layoutInfo.MySendNum].Visible = true;
                    layoutInfo.MySendNum++;
                    //sort 打完之後的手排
                    mycard[ListenNum] = 200;
                    for (int i = 16; i >= 0; i--)
                    {
                        for (int j = layoutInfo.MyLayoutNum; j < i; j++)
                        {
                            if (mycard[j] > mycard[j + 1])
                            {
                                int temp;
                                temp = mycard[j];
                                mycard[j] = mycard[j + 1];
                                mycard[j + 1] = temp;
                            }
                        }
                    }
                    for (int i = layoutInfo.MyLayoutNum; i < 16; i++)
                    {
                        getcardstring(i);
                        //MessageBox.Show("" + allcard[mycard[i]]);
                        Majong_button[i].BackgroundImage = Image_button[i].BackgroundImage;
                        Majong_button[i].BackgroundImageLayout = ImageLayout.Stretch;
                        Majong_button[i].TabIndex = i + (i * 1000);
                    }

                    button17.Visible = false;
                }


            }





           else  if(button5.Enabled==false)
            {

                int HandleNum = ((Button)(sender)).TabIndex % 1000;
                int whoCanPon;
                //MessageBox.Show("" + mycard[num]);
                allcard[mycard[HandleNum]] += 10;

                whoCanPon = PonCard(mycard[HandleNum]);

                MyInfo.allCard = allcard;
                MyInfo.sendedCard = mycard[HandleNum];
                MyInfo.send_player = player;
                MyInfo.someonePon = whoCanPon;
                MyInfo.eaten_player = 0;
                MyInfo.someoneWin = 0;

                Byte[] SndBuf = new Byte[clientsocket2.SendBufferSize];
                SndBuf = MyInfo.PacketToByte();
                Mystream2.Flush();
                Mystream2.Write(SndBuf, 0, SndBuf.Length);
                //輸出打出的排在自己的前面
                My_sended[layoutInfo.MySendNum].BackgroundImage = Image_button[HandleNum].BackgroundImage;
                My_sended[layoutInfo.MySendNum].BackgroundImageLayout = ImageLayout.Stretch;
                My_sended[layoutInfo.MySendNum].Visible = true;
                layoutInfo.MySendNum++;
                //sort 打完之後的手排
                mycard[HandleNum] = 200;
                for (int i = 16; i >= 0; i--)
                {
                    for (int j = layoutInfo.MyLayoutNum; j < i; j++)
                    {
                        if (mycard[j] > mycard[j + 1])
                        {
                            int temp;
                            temp = mycard[j];
                            mycard[j] = mycard[j + 1];
                            mycard[j + 1] = temp;
                        }
                    }
                }
                for (int i = layoutInfo.MyLayoutNum; i < 16; i++)
                {
                    getcardstring(i);
                    //MessageBox.Show("" + allcard[mycard[i]]);
                    Majong_button[i].BackgroundImage = Image_button[i].BackgroundImage;
                    Majong_button[i].BackgroundImageLayout = ImageLayout.Stretch;
                    Majong_button[i].TabIndex = i + (i * 1000);
                }

                button17.Visible = false;
            }

        }

    
        
        

        private void win_button_Click(object sender, EventArgs e)
        {
            int[] sort_array = new int[17];
            for(int i = 0; i < 16; i++)
            {
                sort_array[i] = mycard[i]; 
            }
            
            for (int i = 15; i >= 0; i--)
            {
                for (int j = 0; j < i; j++)
                {
                    if (sort_array[j] > sort_array[j + 1])
                    {
                        int temp;
                        temp = sort_array[j];
                        sort_array[j] = sort_array[j + 1];
                        sort_array[j + 1] = temp;
                    }
                }
            }
            //MessageBox.Show("" + win_card);
            

            if (win(win_card, sort_array) == 1)
            {
                MyInfo.someoneWin = 1;
                MyInfo.send_player = player;
                Byte[] SndBuf = new Byte[clientsocket2.SendBufferSize];
                SndBuf = MyInfo.PacketToByte();
                Mystream2.Flush();
                Mystream2.Write(SndBuf, 0, SndBuf.Length);
            }
            else if (win(win_card, sort_array) == 0)
            {
                MyInfo.someoneWin = 2;
                MyInfo.send_player = player;
                Byte[] SndBuf = new Byte[clientsocket2.SendBufferSize];
                SndBuf = MyInfo.PacketToByte();
                Mystream2.Flush();
                Mystream2.Write(SndBuf, 0, SndBuf.Length);
            }
        }

        private void Pon_button_Click(object sender, EventArgs e)
        {
            allcard[mycard[16]] = player + 10;
            MyInfo.sendedCard = mycard[16];
            MyInfo.send_player = player;
            MyInfo.someonePon = 5;
            MyInfo.someoneWin = 0;

            for(int i = layoutInfo.MyLayoutNum,j=layoutInfo.MyLayoutNum; i < 16; i++)
            {
                if( (mycard[i] / 4) == (mycard[16] / 4) )
                {
                    allcard[mycard[i]] += 10;
                    int temp;
                    temp = mycard[j];
                    mycard[j] = mycard[i];
                    mycard[i] = temp;
                    j++;
                }
                if (j == (layoutInfo.MyLayoutNum + 2))
                {
                    int temp;
                    temp = mycard[j];
                    mycard[j] = mycard[16];
                    mycard[16] = temp;
                    break;
                }
            }
            for(int i = layoutInfo.MyLayoutNum ; i < 16; i++)
            {
                getcardstring(i);
                Majong_button[i].BackgroundImage = Image_button[i].BackgroundImage;
                Majong_button[i].BackgroundImageLayout = ImageLayout.Stretch;
                Majong_button[i].TabIndex = i + (i * 1000);
            }
            getcardstring(16);
            button17.BackgroundImage = Image_button[16].BackgroundImage;
            button17.BackgroundImageLayout = ImageLayout.Stretch;
            button17.TabIndex = 16 + 1000 * 16;
            button17.Visible = true;
            for(int i = layoutInfo.MyLayoutNum; i < (layoutInfo.MyLayoutNum + 3); i++)
            {
                Majong_button[i].Enabled = false;
            }
            layoutInfo.MyLayoutNum += 3;

            MyInfo.allCard = allcard;
            Byte[] SndBuf = new Byte[clientsocket2.SendBufferSize];
            SndBuf = MyInfo.PacketToByte();
            Mystream2.Flush();
            Mystream2.Write(SndBuf, 0, SndBuf.Length);

            Pon_button.Visible = false;
            NotPon_button.Visible = false;
        }

        private void NotPon_button_Click(object sender, EventArgs e)
        {
            MyInfo.allCard = allcard;
            MyInfo.sendedCard = mycard[16];
            MyInfo.send_player = pre_player;
            MyInfo.someonePon = 0;
            MyInfo.eaten_player = 0;

            Byte[] SndBuf = new Byte[clientsocket2.SendBufferSize];
            SndBuf = MyInfo.PacketToByte();
            Mystream2.Flush();
            Mystream2.Write(SndBuf, 0, SndBuf.Length);

            Pon_button.Visible = false;
            NotPon_button.Visible = false;
        }

        private void eat_button_Click(object sender, EventArgs e)
        {
            int type,record,temp;
            int[] eatcard = new int[3];
            type = EatCard(mycard[16]);
            record = mycard[16];
            allcard[mycard[16]] = player + 10;
            if (type == 1 || type == 20 || type == 300)
            {
                if (type == 1)// 2"1"3
                {
                    //找筒子
                    if (record / 4 >= 18 && record / 4 < 27)
                    {
                        for (int i = layoutInfo.MyLayoutNum; i < 16; i++)
                        {
                            if (record / 4 == mycard[i] / 4 + 1) { if (mycard[i] / 4 >= 18) { eatcard[0] = mycard[i]; break; } }
                        }//MessageBox.Show("" + eatcard[0]);
                        for (int i = layoutInfo.MyLayoutNum; i < 16; i++)
                        {
                            if (record / 4 == mycard[i] / 4 + 2) { if (mycard[i] / 4 >= 18) { eatcard[2] = mycard[i]; break; } }
                        }//MessageBox.Show("" + eatcard[2]);
                    }
                    //找條子
                    if (record / 4 >= 9 && record / 4 < 18)
                    {
                        for (int i = layoutInfo.MyLayoutNum; i < 16; i++)
                        {
                            if (record / 4 == mycard[i] / 4 + 1) { if (mycard[i] / 4 >= 9) { eatcard[0] = mycard[i]; break; } }
                        }//MessageBox.Show("" + eatcard[0]);
                        for (int i = layoutInfo.MyLayoutNum; i < 16; i++)
                        {
                            if (record / 4 == mycard[i] / 4 + 2) { if (mycard[i] / 4 >= 9) { eatcard[2] = mycard[i]; break; } }
                        }
                        //MessageBox.Show("" + eatcard[2]);
                    }//找萬子
                    if (record / 4 >= 0 && record / 4 < 9)
                    {
                        for (int i = layoutInfo.MyLayoutNum; i < 16; i++)
                        {
                            if (record / 4 == mycard[i] / 4 + 1) { if (mycard[i] / 4 >= 0) { eatcard[0] = mycard[i]; break; } }
                        }
                        //MessageBox.Show("" + eatcard[0]);
                        for (int i = layoutInfo.MyLayoutNum; i < 16; i++)
                        {
                            if (record / 4 == mycard[i] / 4 + 2) { if (mycard[i] / 4 >= 0) { eatcard[2] = mycard[i]; break; } }
                        }
                        //MessageBox.Show("" + eatcard[2]);
                    }
                }
                else if (type == 20)//1"2"3
                {
                    //找筒子
                    if (record / 4 >= 18 && record / 4 < 27)
                    {
                        for (int i = layoutInfo.MyLayoutNum; i < 16; i++)
                        {
                            if (record / 4 == mycard[i] / 4 - 1) { if (mycard[i] / 4 >= 18) { eatcard[0] = mycard[i]; break; } }
                        }
                        //MessageBox.Show("" + eatcard[0]);
                        for (int i = layoutInfo.MyLayoutNum; i < 16; i++)
                        {
                            if (record / 4 == mycard[i] / 4 + 1) { if (mycard[i] / 4 >= 18) { eatcard[2] = mycard[i]; break; } }
                        }
                        //MessageBox.Show("" + eatcard[2]);
                    }
                    //找條子
                    if (record / 4 >= 9 && record / 4 < 18)
                    {
                        for (int i = layoutInfo.MyLayoutNum; i < 16; i++)
                        {
                            if (record / 4 == mycard[i] / 4 - 1) { if (mycard[i] / 4 >= 9) { eatcard[0] = mycard[i]; break; } }
                        }
                       // MessageBox.Show("" + eatcard[0]);
                        for (int i = layoutInfo.MyLayoutNum; i < 16; i++)
                        {
                            if (record / 4 == mycard[i] / 4 + 1) { if (mycard[i] / 4 >= 9) { eatcard[2] = mycard[i]; break; } }
                        }
                        //MessageBox.Show("" + eatcard[2]);
                    }//找萬子
                    if (record / 4 >= 0 && record / 4 < 9)
                    {
                        for (int i = layoutInfo.MyLayoutNum; i < 16; i++)
                        {
                            if (record / 4 == mycard[i] / 4 - 1) { if (mycard[i] / 4 >= 0) { eatcard[0] = mycard[i]; break; } }
                        }
                       // MessageBox.Show("" + eatcard[0]);
                        for (int i = layoutInfo.MyLayoutNum; i < 16; i++)
                        {
                            if (record / 4 == mycard[i] / 4 + 1) { if (mycard[i] / 4 >= 0) { eatcard[2] = mycard[i]; break; } }
                        }
                       // MessageBox.Show("" + eatcard[2]);
                    }
                }
                else if (type == 300)//1"3"2
                {
                    //找筒子
                    if (record / 4 >= 18 && record / 4 < 27)
                    {
                        for (int i = layoutInfo.MyLayoutNum; i < 16; i++)
                        {
                            if (record / 4 == mycard[i] / 4 - 2) { if (mycard[i] / 4 >= 18) { eatcard[0] = mycard[i]; break; } }
                        }
                        //MessageBox.Show("" + eatcard[0]);
                        for (int i = layoutInfo.MyLayoutNum; i < 16; i++)
                        {
                            if (record / 4 == mycard[i] / 4 - 1) { if (mycard[i] / 4 >= 18) { eatcard[2] = mycard[i]; break; } }
                        }
                        //MessageBox.Show("" + eatcard[2]);
                    }
                    //找條子
                    if (record / 4 >= 9 && record / 4 < 18)
                    {
                        for (int i = layoutInfo.MyLayoutNum; i < 16; i++)
                        {
                            if (record / 4 == mycard[i] / 4 - 2) { if (mycard[i] / 4 >= 9) { eatcard[0] = mycard[i]; break; } }
                        }
                        //MessageBox.Show("" + eatcard[0]);
                        for (int i = layoutInfo.MyLayoutNum; i < 16; i++)
                        {
                            if (record / 4 == mycard[i] / 4 - 1) { if (mycard[i] / 4 >= 9) { eatcard[2] = mycard[i]; break; } }
                        }
                        //MessageBox.Show("" + eatcard[2]);
                    }//找萬子
                    if (record / 4 >= 0 && record / 4 < 9)
                    {
                        for (int i = layoutInfo.MyLayoutNum; i < 16; i++)
                        {
                            if (record / 4 == mycard[i] / 4 - 2) { if (mycard[i] / 4 >= 0) { eatcard[0] = mycard[i]; break; } }
                        }
                        //MessageBox.Show("" + eatcard[0]);
                        for (int i = layoutInfo.MyLayoutNum; i < 16; i++)
                        {
                            if (record / 4 == mycard[i] / 4 - 1) { if (mycard[i] / 4 >= 0) { eatcard[2] = mycard[i]; break; } }
                        }
                        //MessageBox.Show("" + eatcard[2]);
                    }
                }
                eatcard[1] = record;

                for (int i = layoutInfo.MyLayoutNum; i < 16; i++)
                {
                    if (eatcard[0] == mycard[i])
                    {
                        temp = mycard[i];
                        mycard[i] = mycard[layoutInfo.MyLayoutNum];
                        mycard[layoutInfo.MyLayoutNum] = temp;
                        break;
                    }
                }
                temp = mycard[layoutInfo.MyLayoutNum + 1];
                mycard[layoutInfo.MyLayoutNum + 1] = mycard[16];
                mycard[16] = temp;
                for (int i = layoutInfo.MyLayoutNum + 2; i <= 16; i++)
                {
                    if (eatcard[2] == mycard[i])
                    {
                        temp = mycard[i];
                        mycard[i] = mycard[layoutInfo.MyLayoutNum + 2];
                        mycard[layoutInfo.MyLayoutNum + 2] = temp;
                        break;
                    }
                }
                for (int i = layoutInfo.MyLayoutNum; i < 16; i++)
                {
                    getcardstring(i);
                    Majong_button[i].BackgroundImage = Image_button[i].BackgroundImage;
                    Majong_button[i].BackgroundImageLayout = ImageLayout.Stretch;
                    Majong_button[i].TabIndex = i + (i * 1000);
                }
                getcardstring(16);
                button17.BackgroundImage = Image_button[16].BackgroundImage;
                button17.BackgroundImageLayout = ImageLayout.Stretch;
                button17.TabIndex = 16 + 1000 * 16;
                button17.Visible = true;
                for (int i = layoutInfo.MyLayoutNum; i < (layoutInfo.MyLayoutNum + 3); i++)
                {
                    Majong_button[i].Enabled = false;
                }
                layoutInfo.MyLayoutNum += 3;

                MyInfo.eaten_player = pre_player;
                MyInfo.send_player = player;
                MyInfo.eated_card = eatcard;
                MyInfo.allCard = allcard;
                MyInfo.someoneWin = 0;
                Byte[] SndBuf = new Byte[clientsocket2.SendBufferSize];
                SndBuf = MyInfo.PacketToByte();
                Mystream2.Flush();
                Mystream2.Write(SndBuf, 0, SndBuf.Length);

                eat_button.Visible = false;
                Noteat_button.Visible = false;
            }
            else if (type == 321)
            {
                eat_button.Visible = false;
                Noteat_button.Visible = false;
                Leat_button.Visible = true;
                Meat_button.Visible = true;
                Reat_button.Visible = true;
            }
            else if(type == 320)
            {
                eat_button.Visible = false;
                Noteat_button.Visible = false;
                Leat_button.Visible = true;
                Meat_button.Visible = true;
            }
            else if(type == 21)
            {
                eat_button.Visible = false;
                Noteat_button.Visible = false;
                Meat_button.Visible = true;
                Reat_button.Visible = true;
            }
            else if (type == 301)
            {
                eat_button.Visible = false;
                Noteat_button.Visible = false;
                Leat_button.Visible = true;
                Reat_button.Visible = true;
            }
        }


        private void Leat_button_Click(object sender, EventArgs e)
        {
            int type, record, temp;
            int[] eatcard = new int[3];
            record = mycard[16];
            allcard[mycard[16]] = player + 10;
            //找筒子
            if (record / 4 >= 18 && record / 4 < 27)
            {
                for (int i = layoutInfo.MyLayoutNum; i < 16; i++)
                {
                    if (record / 4 == mycard[i] / 4 + 1) { if (mycard[i] / 4 >= 18) { eatcard[0] = mycard[i]; break; } }
                }
                for (int i = 0; i < 16; i++)
                {
                    if (record / 4 == mycard[i] / 4 + 2) { if (mycard[i] / 4 >= 18) { eatcard[2] = mycard[i]; break; } }
                }
            }
            //找條子
            if (record / 4 >= 9 && record / 4 < 18)
            {
                for (int i = 0; i < 16; i++)
                {
                    if (record / 4 == mycard[i] / 4 + 1) { if (mycard[i] / 4 >= 9) { eatcard[0] = mycard[i]; break; } }
                }
                for (int i = 0; i < 16; i++)
                {
                    if (record / 4 == mycard[i] / 4 + 2) { if (mycard[i] / 4 >= 9) { eatcard[2] = mycard[i]; break; } }
                }
            }//找萬子
            if (record / 4 >= 0 && record / 4 < 9)
            {
                for (int i = 0; i < 16; i++)
                {
                    if (record / 4 == mycard[i] / 4 + 1) { if (mycard[i] / 4 >= 0) { eatcard[0] = mycard[i]; break; } }
                }
                for (int i = 0; i < 16; i++)
                {
                    if (record / 4 == mycard[i] / 4 + 2) { if (mycard[i] / 4 >= 0) { eatcard[2] = mycard[i]; break; } }
                }
            }
            eatcard[1] = record;

            for (int i = layoutInfo.MyLayoutNum; i < 16; i++)
            {
                if (eatcard[0] == mycard[i])
                {
                    temp = mycard[i];
                    mycard[i] = mycard[layoutInfo.MyLayoutNum];
                    mycard[layoutInfo.MyLayoutNum] = temp;
                    break;
                }
            }
            temp = mycard[layoutInfo.MyLayoutNum + 1];
            mycard[layoutInfo.MyLayoutNum + 1] = mycard[16];
            mycard[16] = temp;
            for (int i = layoutInfo.MyLayoutNum + 2; i <= 16; i++)
            {
                if (eatcard[2] == mycard[i])
                {
                    temp = mycard[i];
                    mycard[i] = mycard[layoutInfo.MyLayoutNum + 2];
                    mycard[layoutInfo.MyLayoutNum + 2] = temp;
                    break;
                }
            }
            for (int i = layoutInfo.MyLayoutNum; i < 16; i++)
            {
                getcardstring(i);
                Majong_button[i].BackgroundImage = Image_button[i].BackgroundImage;
                Majong_button[i].BackgroundImageLayout = ImageLayout.Stretch;
                Majong_button[i].TabIndex = i + (i * 1000);
            }
            getcardstring(16);
            button17.BackgroundImage = Image_button[16].BackgroundImage;
            button17.BackgroundImageLayout = ImageLayout.Stretch;
            button17.TabIndex = 16 + 1000 * 16;
            button17.Visible = true;
            for (int i = layoutInfo.MyLayoutNum; i < (layoutInfo.MyLayoutNum + 3); i++)
            {
                Majong_button[i].Enabled = false;
            }
            layoutInfo.MyLayoutNum += 3;

            MyInfo.eaten_player = pre_player;
            MyInfo.send_player = player;
            MyInfo.eated_card = eatcard;
            MyInfo.allCard = allcard;
            MyInfo.someoneWin = 0;
            Byte[] SndBuf = new Byte[clientsocket2.SendBufferSize];
            SndBuf = MyInfo.PacketToByte();
            Mystream2.Flush();
            Mystream2.Write(SndBuf, 0, SndBuf.Length);

            Leat_button.Visible = false;
            Meat_button.Visible = false;
            Reat_button.Visible = false;
        }
        private void Meat_button_Click(object sender, EventArgs e)
        {
            int type, record, temp;
            int[] eatcard = new int[3];
            record = mycard[16];
            allcard[mycard[16]] = player + 10;
            //找筒子
            if (record / 4 >= 18 && record / 4 < 27)
            {
                for (int i = 0; i < 16; i++)
                {
                    if (record / 4 == mycard[i] / 4 - 1) { if (mycard[i] / 4 >= 18) { eatcard[0] = mycard[i]; break; } }
                }
                for (int i = 0; i < 16; i++)
                {
                    if (record / 4 == mycard[i] / 4 + 1) { if (mycard[i] / 4 >= 18) { eatcard[2] = mycard[i]; break; } }
                }
            }
            //找條子
            if (record / 4 >= 9 && record / 4 < 18)
            {
                for (int i = 0; i < 16; i++)
                {
                    if (record / 4 == mycard[i] / 4 - 1) { if (mycard[i] / 4 >= 9) { eatcard[0] = mycard[i]; break; } }
                }
                for (int i = 0; i < 16; i++)
                {
                    if (record / 4 == mycard[i] / 4 + 1) { if (mycard[i] / 4 >= 9) { eatcard[2] = mycard[i]; break; } }
                }
            }//找萬子
            if (record / 4 >= 0 && record / 4 < 9)
            {
                for (int i = 0; i < 16; i++)
                {
                    if (record / 4 == mycard[i] / 4 - 1) { if (mycard[i] / 4 >= 0) { eatcard[0] = mycard[i]; break; } }
                }
                for (int i = 0; i < 16; i++)
                {
                    if (record / 4 == mycard[i] / 4 + 1) { if (mycard[i] / 4 >= 0) { eatcard[2] = mycard[i]; break; } }
                }
            }
            eatcard[1] = record;

            for (int i = layoutInfo.MyLayoutNum; i < 16; i++)
            {
                if (eatcard[0] == mycard[i])
                {
                    temp = mycard[i];
                    mycard[i] = mycard[layoutInfo.MyLayoutNum];
                    mycard[layoutInfo.MyLayoutNum] = temp;
                    break;
                }
            }
            temp = mycard[layoutInfo.MyLayoutNum + 1];
            mycard[layoutInfo.MyLayoutNum + 1] = mycard[16];
            mycard[16] = temp;
            for (int i = layoutInfo.MyLayoutNum + 2; i <= 16; i++)
            {
                if (eatcard[2] == mycard[i])
                {
                    temp = mycard[i];
                    mycard[i] = mycard[layoutInfo.MyLayoutNum + 2];
                    mycard[layoutInfo.MyLayoutNum + 2] = temp;
                    break;
                }
            }
            for (int i = layoutInfo.MyLayoutNum; i < 16; i++)
            {
                getcardstring(i);
                Majong_button[i].BackgroundImage = Image_button[i].BackgroundImage;
                Majong_button[i].BackgroundImageLayout = ImageLayout.Stretch;
                Majong_button[i].TabIndex = i + (i * 1000);
            }
            getcardstring(16);
            button17.BackgroundImage = Image_button[16].BackgroundImage;
            button17.BackgroundImageLayout = ImageLayout.Stretch;
            button17.TabIndex = 16 + 1000 * 16;
            button17.Visible = true;
            for (int i = layoutInfo.MyLayoutNum; i < (layoutInfo.MyLayoutNum + 3); i++)
            {
                Majong_button[i].Enabled = false;
            }
            layoutInfo.MyLayoutNum += 3;

            MyInfo.eaten_player = pre_player;
            MyInfo.send_player = player;
            MyInfo.eated_card = eatcard;
            MyInfo.allCard = allcard;
            MyInfo.someoneWin = 0;
            Byte[] SndBuf = new Byte[clientsocket2.SendBufferSize];
            SndBuf = MyInfo.PacketToByte();
            Mystream2.Flush();
            Mystream2.Write(SndBuf, 0, SndBuf.Length);

            Leat_button.Visible = false;
            Meat_button.Visible = false;
            Reat_button.Visible = false;
        }
        private void Reat_button_Click(object sender, EventArgs e)
        {
            int type, record, temp;
            int[] eatcard = new int[3];
            record = mycard[16];
            allcard[mycard[16]] = player + 10;
            //找筒子
            if (record / 4 >= 18 && record / 4 < 27)
            {
                for (int i = 0; i < 16; i++)
                {
                    if (record / 4 == mycard[i] / 4 - 2) { if (mycard[i] / 4 >= 18) { eatcard[0] = mycard[i]; break; } }
                }
                for (int i = 0; i < 16; i++)
                {
                    if (record / 4 == mycard[i] / 4 - 1) { if (mycard[i] / 4 >= 18) { eatcard[2] = mycard[i]; break; } }
                }
            }
            //找條子
            if (record / 4 >= 9 && record / 4 < 18)
            {
                for (int i = 0; i < 16; i++)
                {
                    if (record / 4 == mycard[i] / 4 - 2) { if (mycard[i] / 4 >= 9) { eatcard[0] = mycard[i]; break; } }
                }
                for (int i = 0; i < 16; i++)
                {
                    if (record / 4 == mycard[i] / 4 - 1) { if (mycard[i] / 4 >= 9) { eatcard[2] = mycard[i]; break; } }
                }
            }//找萬子
            if (record / 4 >= 0 && record / 4 < 9)
            {
                for (int i = 0; i < 16; i++)
                {
                    if (record / 4 == mycard[i] / 4 - 2) { if (mycard[i] / 4 >= 0) { eatcard[0] = mycard[i]; break; } }
                }
                for (int i = 0; i < 16; i++)
                {
                    if (record / 4 == mycard[i] / 4 - 1) { if (mycard[i] / 4 >= 0) { eatcard[2] = mycard[i]; break; } }
                }
            }
            eatcard[1] = record;

            for (int i = layoutInfo.MyLayoutNum; i < 16; i++)
            {
                if (eatcard[0] == mycard[i])
                {
                    temp = mycard[i];
                    mycard[i] = mycard[layoutInfo.MyLayoutNum];
                    mycard[layoutInfo.MyLayoutNum] = temp;
                    break;
                }
            }
            temp = mycard[layoutInfo.MyLayoutNum + 1];
            mycard[layoutInfo.MyLayoutNum + 1] = mycard[16];
            mycard[16] = temp;
            for (int i = layoutInfo.MyLayoutNum + 2; i <= 16; i++)
            {
                if (eatcard[2] == mycard[i])
                {
                    temp = mycard[i];
                    mycard[i] = mycard[layoutInfo.MyLayoutNum + 2];
                    mycard[layoutInfo.MyLayoutNum + 2] = temp;
                    break;
                }
            }
            for (int i = layoutInfo.MyLayoutNum; i < 16; i++)
            {
                getcardstring(i);
                Majong_button[i].BackgroundImage = Image_button[i].BackgroundImage;
                Majong_button[i].BackgroundImageLayout = ImageLayout.Stretch;
                Majong_button[i].TabIndex = i + (i * 1000);
            }
            getcardstring(16);
            button17.BackgroundImage = Image_button[16].BackgroundImage;
            button17.BackgroundImageLayout = ImageLayout.Stretch;
            button17.TabIndex = 16 + 1000 * 16;
            button17.Visible = true;
            for (int i = layoutInfo.MyLayoutNum; i < (layoutInfo.MyLayoutNum + 3); i++)
            {
                Majong_button[i].Enabled = false;
            }
            layoutInfo.MyLayoutNum += 3;

            MyInfo.eaten_player = pre_player;
            MyInfo.send_player = player;
            MyInfo.eated_card = eatcard;
            MyInfo.allCard = allcard;
            MyInfo.someoneWin = 0;
            Byte[] SndBuf = new Byte[clientsocket2.SendBufferSize];
            SndBuf = MyInfo.PacketToByte();
            Mystream2.Flush();
            Mystream2.Write(SndBuf, 0, SndBuf.Length);

            Leat_button.Visible = false;
            Meat_button.Visible = false;
            Reat_button.Visible = false;
        }
        private void Noteat_button_Click(object sender, EventArgs e)
        {
            while (true)
            {
                Random random = new Random();
                int a = random.Next() % 136;
                if (allcard[a] == 0)
                {
                    allcard[a] = player;
                    mycard[16] = a;
                    win_card = a;
                    getcardstring(16);
                    this.button17.Invoke(new MethodInvoker(delegate
                    {
                        button17.BackgroundImage = Image_button[16].BackgroundImage;
                        button17.BackgroundImageLayout = ImageLayout.Stretch;
                        button17.TabIndex = 16 + 1000 * 16;
                        button17.Visible = true;
                    }));
                    break;
                }
            }
            eat_button.Visible = false;
            Noteat_button.Visible = false;
        }
        //---------------------------------------------------------------------------------
        public void turn()
        {
            myTurn = (myTurn + 1) % 4;
        }

        private void sendButton_Click(object sender, EventArgs e)
        {
            sendMsg();
        }

        private void textBox2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                sendMsg();
        }
        private void sendMsg()
        {
            if (myMessage.Text != null)
            {

                MyInfo.WordToSend = myMessage.Text;
                //MessageBox.Show(MyInfo.PlayerName);

                Byte[] SndBuf = new Byte[clientsocket.SendBufferSize];
                SndBuf = MyInfo.PacketToByte();
                Mystream.Flush();
                Mystream.Write(SndBuf, 0, SndBuf.Length);

                myMessage.Text = "";
            }
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            Packet TmpPkt = new Packet();
            while (true)
            {
                Byte[] RcvBuf = new Byte[clientsocket.SendBufferSize];
                Mystream.Read(RcvBuf, 0, RcvBuf.Length);
                TmpPkt = OthersInfo.ByteToPacket(RcvBuf);
                this.richTextBox1.Invoke(new MethodInvoker(delegate
                {
                    richTextBox1.AppendText(OthersInfo.PlayerName + " : " + OthersInfo.WordToSend);
                    richTextBox1.AppendText("\n");
                }));
            }
        }
        private void backgroundWorker2_DoWork(object sender, DoWorkEventArgs e)
        {
            //判斷是否為玩家1 否，則進入迴圈
            if (player == 1)
            {
                while (true)
                {
                    Random random = new Random();
                    int a = random.Next() % 136;
                    if (allcard[a] == 0)
                    {
                        allcard[a] = 1;
                        mycard[16] = a;
                        getcardstring(16);
                        this.button17.Invoke(new MethodInvoker(delegate
                        {
                            button17.BackgroundImage = Image_button[16].BackgroundImage;
                            button17.BackgroundImageLayout = ImageLayout.Stretch;
                            button17.TabIndex = 16 + 1000 * 16;
                            button17.Visible = true;
                        }));
                        break;
                    }
                }
            }
            else if (player == 2)
            {
                this.button3.Invoke(new MethodInvoker(delegate
                {
                    button3.Visible = true;
                }));
            }
            else if (player == 3)
            {
                this.button1.Invoke(new MethodInvoker(delegate
                {
                    button1.Visible = true;
                }));
            }
            else if (player == 4)
            {
                this.button2.Invoke(new MethodInvoker(delegate
                {
                    button2.Visible = true;
                }));
            }
            while (true)
            {
                Byte[] RcvBuf = new Byte[clientsocket2.SendBufferSize];
                Mystream2.Read(RcvBuf, 0, RcvBuf.Length);
                OthersInfo.ByteToPacket(RcvBuf);

                pre_player = OthersInfo.send_player;
                eatPlayer = OthersInfo.eaten_player;
                //mycard[16] = OthersInfo.sendedCard;
                win_card = OthersInfo.sendedCard;
                PonOrNot = OthersInfo.someonePon;
                WinOrNot = OthersInfo.someoneWin;
                //getcardstring(16);
                allcard = OthersInfo.allCard;
                //p1_card = OthersInfo.p1_card;
                //p2_card = OthersInfo.p2_card;
                //p3_card = OthersInfo.p3_card;
                //p4_card = OthersInfo.p4_card;

                //p1Win = win(OthersInfo.sendedCard, 1);
                //p2Win = win(OthersInfo.sendedCard, 2);
                //p3Win = win(OthersInfo.sendedCard, 3);
                //p4Win = win(OthersInfo.sendedCard, 4);
                //if (p1Win == 1 || p2Win == 1 || p3Win == 1 || p4Win == 1) { WinOrNot = 1; }
                /*if(WinOrNot == 1)
                {
                    if (p1Win == 1) { MessageBox.Show("p1 Win "); }
                    else if (p2Win == 1) { MessageBox.Show("p2 Win "); }
                    else if (p3Win == 1) { MessageBox.Show("p3 Win "); }
                    else if (p4Win == 1) { MessageBox.Show("p3 Win "); }
                }*/
                if (WinOrNot != 0)
                {
                    if (WinOrNot == 1)
                    {
                        if (player - pre_player == 0)
                        {
                            MessageBox.Show("You Win");
                        }
                        else
                        {
                            if (pre_player == 1) { MessageBox.Show("東風位玩家勝"); }
                            else if (pre_player == 2) { MessageBox.Show("南風位玩家勝"); }
                            else if (pre_player == 3) { MessageBox.Show("西風位玩家勝"); }
                            else if (pre_player == 4) { MessageBox.Show("北風位玩家勝"); }
                        }
                    }
                    else if (WinOrNot == 2)
                    {
                        if (player - pre_player == 0)
                        {
                            MessageBox.Show("你詐胡");
                        }
                        else
                        {
                            if (pre_player == 1) { MessageBox.Show("東風位玩家詐胡"); }
                            else if (pre_player == 2) { MessageBox.Show("南風位玩家詐胡"); }
                            else if (pre_player == 3) { MessageBox.Show("西風位玩家詐胡"); }
                            else if (pre_player == 4) { MessageBox.Show("北風位玩家詐胡"); }
                        }
                    }
                }
                else if (PonOrNot != 0 && PonOrNot != 5)
                {
                    if (player == PonOrNot && PonOrNot != pre_player)
                    {
                        mycard[16] = OthersInfo.sendedCard;
                        getcardstring(16);
                        if ((player - pre_player) == -3 || (player - pre_player) == 1)
                        {
                            this.Left_sended[layoutInfo.LeftSendNum].Invoke(new MethodInvoker(delegate
                            {
                                Left_sended[layoutInfo.LeftSendNum].BackgroundImage = Image_button[16].BackgroundImage;
                                Left_sended[layoutInfo.LeftSendNum].BackgroundImageLayout = ImageLayout.Stretch;
                                Left_sended[layoutInfo.LeftSendNum].Visible = true;
                            }));
                        }
                        else if ((player - pre_player) == -2 || (player - pre_player) == 2)
                        {
                            this.Front_sended[layoutInfo.FrontSendNum].Invoke(new MethodInvoker(delegate
                            {
                                Front_sended[layoutInfo.FrontSendNum].BackgroundImage = Image_button[16].BackgroundImage;
                                Front_sended[layoutInfo.FrontSendNum].BackgroundImageLayout = ImageLayout.Stretch;
                                Front_sended[layoutInfo.FrontSendNum].Visible = true;
                            }));
                        }
                        else if ((player - pre_player) == -1 || (player - pre_player) == 3)
                        {

                            this.Right_sended[layoutInfo.RightSendNum].Invoke(new MethodInvoker(delegate
                            {
                                Right_sended[layoutInfo.RightSendNum].BackgroundImage = Image_button[16].BackgroundImage;
                                Right_sended[layoutInfo.RightSendNum].BackgroundImageLayout = ImageLayout.Stretch;
                                Right_sended[layoutInfo.RightSendNum].Visible = true;
                            }));
                        }
                        this.Pon_button.Invoke(new MethodInvoker(delegate
                        {
                            Pon_button.Visible = true;
                        }));
                        this.NotPon_button.Invoke(new MethodInvoker(delegate
                        {
                            NotPon_button.Visible = true;
                        }));
                    }

                }
                else if (PonOrNot == 5)
                {
                    if ((player - pre_player) == -3 || (player - pre_player) == 1)
                    {
                        mycard[16] = OthersInfo.sendedCard;
                        getcardstring(16);
                        for (int i = layoutInfo.LeftLayoutNum; i < (layoutInfo.LeftLayoutNum + 3); i++)
                        {
                            this.Left_Player[i].Invoke(new MethodInvoker(delegate
                            {
                                Left_Player[i].BackgroundImage = Image_button[16].BackgroundImage;
                                Left_Player[i].BackgroundImageLayout = ImageLayout.Stretch;
                            }));
                        }
                        layoutInfo.LeftLayoutNum += 3;
                    }
                    else if ((player - pre_player) == -2 || (player - pre_player) == 2)
                    {
                        mycard[16] = OthersInfo.sendedCard;
                        getcardstring(16);
                        for (int i = layoutInfo.FrontLayoutNum; i < (layoutInfo.FrontLayoutNum + 3); i++)
                        {
                            this.Front_Player[i].Invoke(new MethodInvoker(delegate
                            {
                                Front_Player[i].BackgroundImage = Image_button[16].BackgroundImage;
                                Front_Player[i].BackgroundImageLayout = ImageLayout.Stretch;
                            }));
                        }
                        layoutInfo.FrontLayoutNum += 3;
                    }
                    else if ((player - pre_player) == -1 || (player - pre_player) == 3)
                    {
                        mycard[16] = OthersInfo.sendedCard;
                        getcardstring(16);
                        for (int i = layoutInfo.RightLayoutNum; i < (layoutInfo.RightLayoutNum + 3); i++)
                        {
                            this.Right_Player[i].Invoke(new MethodInvoker(delegate
                            {
                                Right_Player[i].BackgroundImage = Image_button[16].BackgroundImage;
                                Right_Player[i].BackgroundImageLayout = ImageLayout.Stretch;
                            }));
                        }
                        layoutInfo.RightLayoutNum += 3;
                    }
                }
                else if (eatPlayer != 0)
                {
                    int[] eat = new int[3];
                    eat = OthersInfo.eated_card;
                    if ((player - eatPlayer) == 0)
                    {
                        layoutInfo.MySendNum--;
                        this.My_sended[layoutInfo.MySendNum].Invoke(new MethodInvoker(delegate
                        {
                            My_sended[layoutInfo.MySendNum].Visible = false;
                        }));
                    }
                    else if ((player - eatPlayer) == -3 || (player - eatPlayer) == 1)
                    {
                        if (layoutInfo.LeftSendNum != 0) { layoutInfo.LeftSendNum--; }
                        this.Left_sended[layoutInfo.LeftSendNum].Invoke(new MethodInvoker(delegate
                        {
                            Left_sended[layoutInfo.LeftSendNum].Visible = false;
                        }));
                    }
                    else if ((player - eatPlayer) == -2 || (player - eatPlayer) == 2)
                    {
                        if (layoutInfo.FrontSendNum != 0) { layoutInfo.FrontSendNum--; }
                        this.Front_sended[layoutInfo.FrontSendNum].Invoke(new MethodInvoker(delegate
                        {
                            Front_sended[layoutInfo.FrontSendNum].Visible = false;
                        }));
                    }
                    else if ((player - eatPlayer) == -1 || (player - eatPlayer) == 3)
                    {
                        if (layoutInfo.RightSendNum != 0) { layoutInfo.RightSendNum--; }
                        this.Right_sended[layoutInfo.RightSendNum].Invoke(new MethodInvoker(delegate
                        {
                            Right_sended[layoutInfo.RightSendNum].Visible = false;
                        }));
                    }
                    //輸出吃的那三張牌
                    
                    if ((player - pre_player) == -3 || (player - pre_player) == 1)
                    {
                        int[] temp = new int[17];
                        for(int i=0; i < 16; i++)
                        {
                            temp[i] = mycard[i];
                        }
                        
                        mycard[0] = eat[0]; mycard[1] = eat[1]; mycard[2] = eat[2];
                        getcardstring(0); getcardstring(1); getcardstring(2);

                        for (int i = layoutInfo.LeftLayoutNum,j = 0; i < (layoutInfo.LeftLayoutNum + 3); i++,j++)
                        {
                            this.Left_Player[i].Invoke(new MethodInvoker(delegate
                            {
                                Left_Player[i].BackgroundImage = Image_button[j].BackgroundImage;
                                Left_Player[i].BackgroundImageLayout = ImageLayout.Stretch;
                            }));
                        }
                        layoutInfo.LeftLayoutNum += 3;

                        for (int i = 0; i < 16; i++)
                        {
                            mycard[i] = temp[i];
                            getcardstring(i);
                        }
                    }
                    else if ((player - pre_player) == -2 || (player - pre_player) == 2)
                    {
                        int[] temp = new int[17];
                        for (int i = 0; i < 16; i++)
                        {
                            temp[i] = mycard[i];
                        }
                        mycard[0] = eat[0]; mycard[1] = eat[1]; mycard[2] = eat[2];
                        getcardstring(0); getcardstring(1); getcardstring(2);

                        for (int i = layoutInfo.FrontLayoutNum,j = 0; i < (layoutInfo.FrontLayoutNum + 3); i++,j++)
                        {
                            this.Front_Player[i].Invoke(new MethodInvoker(delegate
                            {
                                Front_Player[i].BackgroundImage = Image_button[j].BackgroundImage;
                                Front_Player[i].BackgroundImageLayout = ImageLayout.Stretch;
                            }));
                        }
                        layoutInfo.FrontLayoutNum += 3;

                        for (int i = 0; i < 16; i++)
                        {
                            mycard[i] = temp[i];
                            getcardstring(i);
                        }
                    }
                    else if ((player - pre_player) == -1 || (player - pre_player) == 3)
                    {
                        int[] temp = new int[17];
                        for(int i=0; i < 16; i++)
                        {
                            temp[i] = mycard[i];
                        }
                        mycard[0] = eat[0]; mycard[1] = eat[1]; mycard[2] = eat[2];
                        getcardstring(0); getcardstring(1); getcardstring(2);

                        for (int i = layoutInfo.RightLayoutNum,j = 0; i < (layoutInfo.RightLayoutNum + 3); i++,j++)
                        {
                            this.Right_Player[i].Invoke(new MethodInvoker(delegate
                            {
                                Right_Player[i].BackgroundImage = Image_button[j].BackgroundImage;
                                Right_Player[i].BackgroundImageLayout = ImageLayout.Stretch;
                            }));
                        }
                        layoutInfo.RightLayoutNum += 3;

                        for (int i = 0; i < 16; i++)
                        {
                            mycard[i] = temp[i];
                            getcardstring(i);
                        }
                    }
                    
                }
                else
                {
                    if ((player - pre_player) == 0)
                    {

                        this.button2.Invoke(new MethodInvoker(delegate
                        {
                            button2.Visible = true;
                        }));
                    }
                    //right
                    else if ((player - pre_player) == -3 || (player - pre_player) == 1)
                    {
                        mycard[16] = OthersInfo.sendedCard;
                        getcardstring(16);
                        this.Left_sended[layoutInfo.LeftSendNum].Invoke(new MethodInvoker(delegate
                        {
                            Left_sended[layoutInfo.LeftSendNum].BackgroundImage = Image_button[16].BackgroundImage;
                            Left_sended[layoutInfo.LeftSendNum].BackgroundImageLayout = ImageLayout.Stretch;
                            Left_sended[layoutInfo.LeftSendNum].Visible = true;
                            layoutInfo.LeftSendNum++;
                        }));
                        this.button3.Invoke(new MethodInvoker(delegate
                        {
                            button3.Visible = false;
                        }));
                        if (EatCard(mycard[16]) != 0)
                        {
                            this.eat_button.Invoke(new MethodInvoker(delegate
                            {
                                eat_button.Visible = true;
                            }));
                            this.Noteat_button.Invoke(new MethodInvoker(delegate
                            {
                                Noteat_button.Visible = true;
                            }));
                        }
                        else
                        {
                            while (true)
                            {
                                Random random = new Random();
                                int a = random.Next() % 136;
                                if (allcard[a] == 0)
                                {
                                    allcard[a] = player;
                                    mycard[16] = a;
                                    win_card = a;
                                    getcardstring(16);
                                    this.button17.Invoke(new MethodInvoker(delegate
                                    {
                                        button17.BackgroundImage = Image_button[16].BackgroundImage;
                                        button17.BackgroundImageLayout = ImageLayout.Stretch;
                                        button17.TabIndex = 16 + 1000 * 16;
                                        button17.Visible = true;
                                    }));
                                    break;
                                }
                            }
                        }

                    }
                    //front
                    else if ((player - pre_player) == -2 || (player - pre_player) == 2)
                    {
                        mycard[16] = OthersInfo.sendedCard;
                        getcardstring(16);
                        this.Front_sended[layoutInfo.FrontSendNum].Invoke(new MethodInvoker(delegate
                        {
                            Front_sended[layoutInfo.FrontSendNum].BackgroundImage = Image_button[16].BackgroundImage;
                            Front_sended[layoutInfo.FrontSendNum].BackgroundImageLayout = ImageLayout.Stretch;
                            Front_sended[layoutInfo.FrontSendNum].Visible = true;
                            layoutInfo.FrontSendNum++;
                        }));
                        this.button3.Invoke(new MethodInvoker(delegate
                        {
                            button3.Visible = true;
                        }));
                        this.button1.Invoke(new MethodInvoker(delegate
                        {
                            button1.Visible = false;
                        }));
                    }
                    //left
                    else if ((player - pre_player) == -1 || (player - pre_player) == 3)
                    {
                        mycard[16] = OthersInfo.sendedCard;
                        getcardstring(16);
                        this.Right_sended[layoutInfo.RightSendNum].Invoke(new MethodInvoker(delegate
                        {
                            Right_sended[layoutInfo.RightSendNum].BackgroundImage = Image_button[16].BackgroundImage;
                            Right_sended[layoutInfo.RightSendNum].BackgroundImageLayout = ImageLayout.Stretch;
                            Right_sended[layoutInfo.RightSendNum].Visible = true;
                            layoutInfo.RightSendNum++;
                        }));
                        this.button1.Invoke(new MethodInvoker(delegate
                        {
                            button1.Visible = true;
                        }));
                        this.button2.Invoke(new MethodInvoker(delegate
                        {
                            button2.Visible = false;
                        }));
                    }
                }
                //end of while(true)
            }
        }
        private int win(int k,int[] a)///k=新牌
        {
            int Straightnum = 0, three = 0; int[] ans = new int[20];


            for (int n = 0; n < 16; n++)
            {
                ans[16] = k;
                for (int i = 0; i < 16; i++)
                {
                    ans[i] = a[i];
                }
                for (int p = 0; p < 17; p++)
                {
                    for (int q = p + 1; q < 17; q++)
                    {
                        if (ans[p] > ans[q])
                        {
                            int temp = 0;
                            temp = ans[p];
                            ans[p] = ans[q];
                            ans[q] = temp;
                        }
                    }
                }
                three = 0; Straightnum = 0;
                if (ans[n] / 4 == ans[n + 1] / 4)///eye
                {
                    //  MessageBox.Show("n = "+Convert.ToString(n));
                    ans[n] += 200; ans[n + 1] += 200;
                    for (int p = 0; p < 17; p++)
                    {
                        for (int q = p + 1; q < 17; q++)
                        {
                            if (ans[p] > ans[q])
                            {
                                int temp = 0;
                                temp = ans[p];
                                ans[p] = ans[q];
                                ans[q] = temp;
                            }
                        }
                    }
                    for (int i = 0; i < 13 - three * 3;)
                    {
                        if (ans[i] / 4 == ans[i + 1] / 4 && ans[i + 1] / 4 == ans[i + 2] / 4)
                        {
                            three++; ans[i] += 200; ans[i + 1] += 200; ans[i + 2] += 200;
                            for (int p = 0; p < 17; p++)
                            {
                                for (int q = p + 1; q < 17; q++)
                                {
                                    if (ans[p] > ans[q])
                                    {
                                        int temp = 0;
                                        temp = ans[p];
                                        ans[p] = ans[q];
                                        ans[q] = temp;
                                    }
                                }
                            }

                            //  i += 3;
                        }
                        else { i++; }

                    }
                    //  MessageBox.Show("three = " + Convert.ToString(three));
                    ////////////////////////////////////////////////////////////////////////////

                    for (int i = 0; i < 15 - 3 * three; i++)
                    {
                        if (ans[i] / 4 <= 6)
                        {
                            for (int j = i + 1; j < 17; j++)
                            {
                                if (ans[i] / 4 == ans[j] / 4 - 1)
                                {
                                    for (int t = j + 1; t < 17; t++)
                                    {
                                        if (ans[j] / 4 == ans[t] / 4 - 1)
                                        {
                                            Straightnum++; ans[i] += 200; ans[j] += 200; ans[t] += 200; //MessageBox.Show("str=" + Convert.ToString(Straightnum));
                                            for (int p = 0; p < 17; p++)
                                            {
                                                for (int q = p + 1; q < 16; q++)
                                                {
                                                    if (ans[p] > ans[q])
                                                    {
                                                        int temp = 0;
                                                        temp = ans[p];
                                                        ans[p] = ans[q];
                                                        ans[q] = temp;
                                                    }
                                                }
                                            }
                                            i--;
                                            break;
                                        }
                                    }
                                    break;
                                }
                            }
                        }
                        else if (ans[i] / 4 >= 9 && ans[i] / 4 <= 15)
                        {
                            for (int j = i + 1; j < 17; j++)
                            {
                                if (ans[i] / 4 == ans[j] / 4 - 1)
                                {
                                    for (int t = j + 1; t < 17; t++)
                                    {
                                        if (ans[j] / 4 == ans[t] / 4 - 1)
                                        {
                                            Straightnum++; ans[i] += 200; ans[j] += 200; ans[t] += 200; //MessageBox.Show("str=" + Convert.ToString(Straightnum));
                                            for (int p = 0; p < 17; p++)
                                            {
                                                for (int q = p + 1; q < 16; q++)
                                                {
                                                    if (ans[p] > ans[q])
                                                    {
                                                        int temp = 0;
                                                        temp = ans[p];
                                                        ans[p] = ans[q];
                                                        ans[q] = temp;
                                                    }
                                                }
                                            }
                                            i--;
                                            break;
                                        }
                                    }
                                    break;
                                }
                            }
                        }
                        else if (ans[i] / 4 >= 18 && ans[i] / 4 <= 24)
                        {
                            for (int j = i + 1; j < 17; j++)
                            {
                                if (ans[i] / 4 == ans[j] / 4 - 1)
                                {
                                    for (int t = j + 1; t < 17; t++)
                                    {
                                        if (ans[j] / 4 == ans[t] / 4 - 1)
                                        {
                                            Straightnum++; ans[i] += 200; ans[j] += 200; ans[t] += 200;// MessageBox.Show("str=" + Convert.ToString(Straightnum));
                                            for (int p = 0; p < 17; p++)
                                            {
                                                for (int q = p + 1; q < 16; q++)
                                                {
                                                    if (ans[p] > ans[q])
                                                    {
                                                        int temp = 0;
                                                        temp = ans[p];
                                                        ans[p] = ans[q];
                                                        ans[q] = temp;
                                                    }
                                                }
                                            }
                                            i--;
                                            break;
                                        }
                                    }
                                    break;
                                }
                            }
                        }
                        /*  else{
                              i++;
                          }*/
                    }
                } ///eye
                if (three + Straightnum == 5) { return 1; }
            }
            return 0;
        }
        private int PonCard (int card)
        {
            int p1 = 0, p2 = 0, p3 = 0, p4 = 0;
            for(int i = 0; i < 144; i++)
            {
                if(i / 4 == card / 4)
                {
                    if (allcard[i] == 1) { p1++; }
                    else if (allcard[i] == 2) { p2++; }
                    else if (allcard[i] == 3) { p3++; }
                    else if (allcard[i] == 4) { p4++; }
                }
            }
            if (p1 >= 2) { return 1; }
            else if (p2 >= 2) { return 2; }
            else if (p3 >= 2) { return 3; }
            else if (p4 >= 2) { return 4; }
            else { return 0; }
        }
        private int EatCard(int card)
        {
            int count1 = 0, count2 = 0, count3 = 0;
            int which = 0;
            for (int i = layoutInfo.MyLayoutNum; i < 16; i++)
            {
                if (card / 4 >= 18 && card / 4 < 27)
                {
                    if (card / 4 == mycard[i] / 4 + 2) { if (mycard[i] / 4 >= 18) { count1 = 1; } }
                    if (card / 4 == mycard[i] / 4 + 1 && card / 4 < 27) { if (mycard[i] / 4 >= 18) { if (count1 == 1) { which = 1; } count2 = 1; } }     //1     
                    if (card / 4 == mycard[i] / 4 - 1 && card / 4 < 27) { if (mycard[i] / 4 < 27) { if (count2 == 1) { which += 20; } count3 = 1; } }  //2
                    if (card / 4 == mycard[i] / 4 - 2 && card / 4 < 27) { if (mycard[i] / 4 < 27) { if (count3 == 1) { which += 300; } } } //3
                }
                if (card / 4 >= 0 && card / 4 < 9)
                {
                    if (card / 4 == mycard[i] / 4 + 2) { if (mycard[i] / 4 >= 0) { count1 = 1; } }
                    if (card / 4 == mycard[i] / 4 + 1 && card / 4 < 27) { if (mycard[i] / 4 >= 0) { if (count1 == 1) { which = 1; } count2 = 1; } }     //1     
                    if (card / 4 == mycard[i] / 4 - 1 && card / 4 < 27) { if (mycard[i] / 4 < 9) { if (count2 == 1) { which += 20; } count3 = 1; } }  //2
                    if (card / 4 == mycard[i] / 4 - 2 && card / 4 < 27) { if (mycard[i] / 4 < 9) { if (count3 == 1) { which += 300; } } } //3
                }
                if (card / 4 >= 9 && card / 4 < 18)
                {
                    if (card / 4 == mycard[i] / 4 + 2) { if (mycard[i] / 4 >= 9) { count1 = 1; } }
                    if (card / 4 == mycard[i] / 4 + 1 && card / 4 < 27) { if (mycard[i] / 4 >= 9) { if (count1 == 1) { which = 1; } count2 = 1; } }     //1     
                    if (card / 4 == mycard[i] / 4 - 1 && card / 4 < 27) { if (mycard[i] / 4 < 18) { if (count2 == 1) { which += 20; } count3 = 1; } }  //2
                    if (card / 4 == mycard[i] / 4 - 2 && card / 4 < 27) { if (mycard[i] / 4 < 18) { if (count3 == 1) { which += 300; } } } //3
                }
            }
            return which;
        }
        class Layout_Info
        {
            public int MySendNum = 0;
            public int RightSendNum = 0;
            public int FrontSendNum = 0;
            public int LeftSendNum = 0;
            public int MyLayoutNum = 0;
            public int RightLayoutNum = 0;
            public int FrontLayoutNum = 0;
            public int LeftLayoutNum = 0;
        }

    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO.Ports;
using System.Threading;
using System.IO;
using System.Runtime.InteropServices;

namespace HKDemo
{
    public partial class Form1 : Form
    {
       private  HKSDK.MSGCallBack m_call = null; 
        
        public Form1()
        {
            InitializeComponent();
         //   pictureBox1.Image = Image.FromFile(@"D:\卡口\20150416\20150416163914627.jpg");
         //   MessageBox.Show(Thread.CurrentThread.ManagedThreadId.ToString());
            comboBox1.SelectedIndex = 0;
            filepath.Text = Form1.savepath;


            
            string test="";
            System.IO.StreamReader fileread;
            fileread = new StreamReader(logpath,System.Text.Encoding.Default);
            
            byte[] tembyte = new byte[40];
            byte a;
            while(!fileread.EndOfStream)
            {
               test  =  fileread.ReadLine();
                for (int i=0;i<test.Length/2;i++)
                {
                    tembyte[i] = Convert.ToByte(test.Substring(i*2, 2),16);
                    
                }
                comdedata(tembyte, 15);                
            }      
        }
        private bool m_bInitSDK = false;
        private Int32 m_lUserID = -1;//用户ID
        private HKSDK.NET_DVR_PREVIEWINFO lpPreviewInfo = new HKSDK.NET_DVR_PREVIEWINFO();
        private int m_lRealHandle;//预览
        //注册
        private void RegButClick(object sender, EventArgs e)
        {
            if (ipaddr.Text == null||port.Text==null)
            {
                MessageBox.Show("请输入设备IP地址和端口号");
                return;
            }
            m_bInitSDK = HKSDK.NET_DVR_Init();
            if (!m_bInitSDK)
            {
              
                textBox1.AppendText("\r\n初始化失败");
            }
            else
                textBox1.AppendText("\r\n初始化成功");//  MessageBox.Show("初始化成功");
             
            HKSDK.NET_DVR_DEVICEINFO_V30 struDeviceInfo = new HKSDK.NET_DVR_DEVICEINFO_V30();
            m_lUserID = HKSDK.NET_DVR_Login_V30(ipaddr.Text, int.Parse(port.Text), "admin", "12345", ref struDeviceInfo);
            if (m_lUserID < 0)
            {
                
                textBox1.AppendText("\r\n注册失败");
            }
            else
                textBox1.AppendText("\r\n注册成功");//   MessageBox.Show("注册成功");
        }
        //预览关闭
        private void previewCloseClick(object sender, EventArgs e)
        {
          
          //  savepic.Abort();
        }
        //预览开启
        private void previewOpenClick(object sender, EventArgs e)
        {
            if (m_lUserID >= 0)
            {
                if (button2.Text == @"开启预览")
                {
                    HKSDK.NET_DVR_PREVIEWINFO lpPreviewInfo = new HKSDK.NET_DVR_PREVIEWINFO();
                    lpPreviewInfo.hPlayWnd = panel1.Handle;//预览窗口
                    lpPreviewInfo.lChannel = 1;//预te览的设备通道
                    lpPreviewInfo.dwStreamType = 0;//码流类型：0-主码流，1-子码流，2-码流3，3-码流4，以此类推
                    lpPreviewInfo.dwLinkMode = 0;//连接方式：0- TCP方式，1- UDP方式，2- 多播方式，3- RTP方式，4-RTP/RTSP，5-RSTP/HTTP 
                    lpPreviewInfo.bBlocked = true; //0- 非阻塞取流，1- 阻塞取流
                    IntPtr pUser = new IntPtr();//用户数据                
                    m_lRealHandle = HKSDK.NET_DVR_RealPlay_V40(m_lUserID, ref lpPreviewInfo, null/*RealData*/, pUser);
                    if (m_lRealHandle < 0)
                    {
                        textBox1.AppendText("\r\n开启预览失败");
                      //  MessageBox.Show("error " + HKSDK.NET_DVR_GetLastError());
                        return;
                    }
                    //    savepic = new Thread(new ThreadStart(savePicThread));
                    m_call = new HKSDK.MSGCallBack(MsgCallback);
                    if (HKSDK.NET_DVR_SetDVRMessageCallBack_V30(m_call, IntPtr.Zero))                        
                        textBox1.AppendText("\r\n开启预览成功");
                    button2.Text = @"关闭预览";
                }
                else
                {
                    HKSDK.NET_DVR_StopRealPlay(m_lRealHandle);
                    button2.Text = @"关闭预览";
                    textBox1.AppendText("\r\n关闭预览");
                }
              

            }
        }
        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern IntPtr GetForegroundWindow();
        //最大化窗口
        static bool maxororgin;
        static Point orignalpos;
        static Size orignalsize;
        static IntPtr pareloc  ;
        private void maxClick(object sender, EventArgs e)
        {
            if (maxororgin)
            {
                
                maxororgin = !maxororgin;
                this.panel1.Location = new System.Drawing.Point(orignalpos.X,orignalpos.Y); 
                this.panel1.Size = new System.Drawing.Size(orignalsize.Width,orignalsize.Height);
              //  SetParent(this.panel1.Handle, pareloc);
                FillScreenDisplay(this.panel1, pareloc, false);
            }
            else
            {   
                pareloc = GetForegroundWindow();
                maxororgin = !maxororgin;
                orignalpos.X = this.panel1.Location.X;
                orignalpos.Y = this.panel1.Location.Y;
                orignalsize.Width = this.panel1.Width;
                orignalsize.Height = this.panel1.Height;
                FillScreenDisplay(this.panel1,IntPtr.Zero, true);
                panel1.Size = new System.Drawing.Size(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);
                this.panel1.Location = new System.Drawing.Point(0, 0);
            }
        }
        private void FillScreenDisplay(Control control,IntPtr parentcon, bool fill)
        {
            if (fill)
            {
                cotrol = control;
                control.Dock = DockStyle.None;
                control.Left = 0;
                control.Top = 0;
                control.Width = Screen.PrimaryScreen.Bounds.Width;
                control.Height = Screen.PrimaryScreen.Bounds.Height;
                parentcontrol = control.Parent.Handle;
                SetParent(control.Handle, parentcon);
             //   AddEventKeyUp(control);
              //  base.Parent.Hide();
            }
            else
            {
                SetParent(control.Handle, parentcon);
              //  base.Parent.Show();
            }
        }
        private void AddEventKeyUp(Control control)
        {
            if (control != null)
            {
                control.KeyUp += new KeyEventHandler(control_KeyUp);
                //foreach (Control c in control.Controls)
                //{// 需要给子控件也添加上，否则有可能取不到。
                //    AddEventKeyUp(c);
                //}
            }
        }
        static Control cotrol;
        static IntPtr parentcontrol;
        void control_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                 if (cotrol != null)
                {
                    SetParent(cotrol.Handle, parentcontrol);
                    cotrol.Dock = DockStyle.Fill;
                }
            }
        }
        //抓拍图片
        static int picname = 0;
        Thread savepic = null;
        private int capparam = 0;
        private void CapturePic(object sender, EventArgs e)
        {
            if (button4.Text == @"开启抓拍")
            {

                HKSDK.tagNET_DVR_SETUPALARM_PARAM starttag = new HKSDK.tagNET_DVR_SETUPALARM_PARAM();
             //   starttag.dwSize = (uint)Marshal.SizeOf(typeof(HKSDK.NET_DVR_SETUPALARM_PARAM));
                starttag.byAlarmInfoType = 1;
                starttag.byRetAlarmTypeV40 = 1;
                capparam =  HKSDK.NET_DVR_SetupAlarmChan_V41(m_lUserID,ref starttag);
                textBox1.AppendText("\r\n开启抓拍");
                button4.Text = @"关闭抓拍";
            }
            else
            {

                HKSDK.NET_DVR_CloseAlarmChan_V30(capparam);
                textBox1.AppendText("\r\n关闭抓拍");
                button4.Text = @"开启抓拍";
            }
           
            if (m_lRealHandle < 0)
            {                
                return;
            }  
        }
        public delegate void UPdatePicBox(string picpath);
        //保存图片线程
        public  void savePicThread( )
        {
            try
            {
           
            picname++;
            string sBmpPicFileName = savepath + "pic\\" + picname.ToString() + ".jpg"; 
            if (!HKSDK.NET_DVR_CapturePicture(m_lRealHandle, sBmpPicFileName))
            {

            }
            else
            {
                 
                if (this.pictureBox1.InvokeRequired)
                {
                    
                    pictureBox1.BeginInvoke(new UPdatePicBox(showpic),sBmpPicFileName);
                }
                else
                {
                    showpic(sBmpPicFileName);
                }
            }
            }
            catch (System.Exception ex)
            {
                
            }
        }
     
        private void showpic(string picpath)
        {
           try
           {
               pictureBox1.Image = Image.FromFile(picpath);
           }
           catch (System.Exception ex)
           {
           	
           }         
           
        }
        //选择图片保存路径
        private void SelPicPath(object sender, EventArgs e)
        {
            folderBrowserDialog1.SelectedPath = System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
            folderBrowserDialog1.Description = @"选择保存文件的文件夹";
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                filepath.Text = folderBrowserDialog1.SelectedPath;            

            }
            System.IO.FileStream file1;

            string file1path = folderBrowserDialog1.SelectedPath +"\\"+ DateTime.Now.ToString("yyy-MM-dd hhmmss")+".txt";
            file1 = System.IO.File.Open(file1path, FileMode.OpenOrCreate);
            StreamWriter filewrite = new StreamWriter(file1);
            filewrite.WriteLine(" 序号   车道      长      宽         高 ");
            for (int i = 0; i < listView1.Items.Count;i++ )
            {
                string info = string.Format(" {0}        {1}        {2}       {3}       {4}", listView1.Items[i].SubItems[0].Text, listView1.Items[i].SubItems[1].Text, listView1.Items[i].SubItems[2].Text, listView1.Items[i].SubItems[3].Text, listView1.Items[i].SubItems[4].Text);
                filewrite.WriteLine(info);            
            }
            filewrite.Flush();
            filewrite.Close();
           
        }
        
        SerialPort comport = new SerialPort();
        Thread recvthread = null;

        //打开 关闭端口
        private void ComClick(object sender, EventArgs e)
        {
            try
            {
            
            if (comboBox1.Text!="")
            {
                 if (comport.IsOpen)
                 {
                     combutton.Text = "打开端口";
                     comport.Close();
                     recvthread.Abort();
                     comboBox1.Enabled = true;
                     file.Close();
                     textBox1.AppendText("\r\n关闭端口成功");
                 }
                 else
                 {
                     comport.PortName = comboBox1.Text;
                     comport.BaudRate = 115200;
                     comport.DataBits = 8;
                     comport.StopBits = StopBits.One;
                     comport.Parity = Parity.None;
                     comport.ReadTimeout = 20;
                     comport.Open();

                     if (comport.IsOpen)
                     {
                         textBox1.AppendText("\r\n打开端口成功");
                         combutton.Text = "关闭端口";
                         comboBox1.Enabled = false;
              //           MessageBox.Show(Thread.CurrentThread.ManagedThreadId.ToString());
                         ThreadStart method = new ThreadStart(ReceiveData);//在线程上执行此方法
                         recvthread = new Thread(new ThreadStart(method));
                         recvthread.Start();
                         file = System.IO.File.Open(logpath, FileMode.OpenOrCreate | FileMode.Append);
                      }
                      
                 }
            }
            else
            {

                MessageBox.Show("请选择串口");
                return;
            }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            
        }
        //串口接收线程
        //不断循环接收数据
        private void ReceiveData()
        {
            while (true)
            {
                Thread.Sleep(50);
               
                this.SynReceiveData();
            }
        }
        public delegate void UpdateListBoxCallback(byte [] a,int len );

         //通过串口取数据
         private void SynReceiveData()
         {
             byte[] recvbyte = new byte[400];
             int len = comport.BytesToRead; 
             try
             {
                 if (len>0)
                 {
                     comport.Read(recvbyte, 0, len);
                     savedata(recvbyte, len);
                    
               //      MessageBox.Show(Thread.CurrentThread.ManagedThreadId.ToString());
                     if (listView1.InvokeRequired)
                     {
                         byte[] paras = new byte[100];
                         Array.Copy(recvbyte, paras, len);
                         listView1.BeginInvoke(new UpdateListBoxCallback(comdedata), paras,len); 
                     }
                     else { 
                         comdedata(recvbyte, len); 
                     }
                     
                 }
             }
             catch (Exception ex)
             {
                 MessageBox.Show(ex.Message);
             }
         }
         static int vehnum=0;
         private void comdedata(byte [] a,int len)
         {
             int lane;
             int length;
             int width;
             int higth;
             int i = 0;
            // @ 0x40
            
             while(len/15 >i)
             {
                lane = a[i*15+1];
                length = a[i * 15 + 2] * 1000 + a[i * 15 + 3] * 100 + a[i * 15 + 4] * 10 + a[i * 15 + 5];
                width = a[i * 15 + 6]*1000 + a[i * 15 + 7] *100+ a[i * 15 + 8]*10+ a[i * 15 + 9];
                higth = a[i * 15 + 10]*1000+ a[i * 15 + 11]*100+a[i * 15 + 12]*10+ a[i * 15 + 13];
                vehnum++;
                ListViewItem item = new ListViewItem(vehnum.ToString());
                item.SubItems.Add(lane.ToString());
                item.SubItems.Add(length.ToString());
                item.SubItems.Add(width.ToString());
                item.SubItems.Add(higth.ToString());
                item.Focused = true;
                 
                listView1.Items.Add(item);
               
                i++;
             //   listView1.Items.Add(item);
             }


         }
        //得到当前路径
         private static string savepath = System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
         private static string logpath = savepath + "过车记录.txt";
         private static System.IO.FileStream file; 
        
        private void savedata( byte[] a,int len)
        {
            
            StringBuilder strb = new StringBuilder();
            if (savepath != "")
            {

                for (int i = 0; i < len;i++ )
                {
                    strb.Append(a[i].ToString("X2"));
                }
                string hexstring = strb.ToString();
                StreamWriter filewrite = new StreamWriter(file);
                filewrite.WriteLine(hexstring);
                filewrite.Flush();


            }
           
        }

        private void SelectPic(object sender, EventArgs e)
        {
            //ListView item = (ListView)sender;
             
            //MessageBox.Show(sender.ToString());
        }
        //长加
        private void lengadd(object sender, EventArgs e)
        {
            int num = listView1.Items.Count;
            for (int i = 0; i < num ; i++)
            {
                if (listView1.Items[i].Selected)
                {

                      int aa = System.Int32.Parse(listView1.Items[i].SubItems[2].Text) + 10;
                      listView1.Items[i].SubItems[2].Text = string.Format("{0}", aa);
                }
                 
            }
        }
        //长减
        private void lengsub(object sender, EventArgs e)
        {
            int num = listView1.Items.Count;
            for (int i = 0; i < num; i++)
            {
                if (listView1.Items[i].Selected)
                {

                    int aa = System.Int32.Parse(listView1.Items[i].SubItems[2].Text) - 10;
                    listView1.Items[i].SubItems[2].Text = string.Format("{0}", aa);
                }

            }
        }
        //宽加
        private void widthadd(object sender, EventArgs e)
        {
            int num = listView1.Items.Count;
            for (int i = 0; i < num; i++)
            {
                if (listView1.Items[i].Selected)
                {

                    int aa = System.Int32.Parse(listView1.Items[i].SubItems[3].Text) + 5;
                    listView1.Items[i].SubItems[3].Text = string.Format("{0}", aa);
                }

            }
        }
        //宽减
        private void widthsub(object sender, EventArgs e)
        {
            int num = listView1.Items.Count;
            for (int i = 0; i < num; i++)
            {
                if (listView1.Items[i].Selected)
                {

                    int aa = System.Int32.Parse(listView1.Items[i].SubItems[3].Text) - 5;
                    listView1.Items[i].SubItems[3].Text = string.Format("{0}", aa);
                }

            }
        }
        //高加
        private void highadd(object sender, EventArgs e)
        {
            int num = listView1.Items.Count;
            for (int i = 0; i < num; i++)
            {
                if (listView1.Items[i].Selected)
                {

                    int aa = System.Int32.Parse(listView1.Items[i].SubItems[4].Text) + 5;
                    listView1.Items[i].SubItems[4].Text = string.Format("{0}", aa);
                }

            }
        }
        //高减
        private void highsub(object sender, EventArgs e)
        {
            int num = listView1.Items.Count;
            for (int i = 0; i < num; i++)
            {
                if (listView1.Items[i].Selected)
                {

                    int aa = System.Int32.Parse(listView1.Items[i].SubItems[4].Text) - 5;
                    listView1.Items[i].SubItems[4].Text = string.Format("{0}", aa);
                }

            }

        }

       
        private void SureClose(object sender, FormClosingEventArgs e)
        {
            DialogResult res = MessageBox.Show("确定关闭窗口","确认关闭" ,MessageBoxButtons.OKCancel);
            if (res == DialogResult.OK)
            {

            }
            else
            {
                e.Cancel = true;
            }
        }

       
       
      
    }
}

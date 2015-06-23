using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

using System.Runtime.InteropServices;

namespace HKDemo
{
    public partial class Form1
    {
         /// <summary>  
         /// 
    //     public delegate void UpdateListBoxCallback(byte[] a, int len);
    //     public delegate void UpdateListBoxCallback(string strAlarmTime, string strDevIP, string strAlarmMsg);
        [DllImport("user32.dll")]
        private static extern int SetParent(IntPtr hWndChild, IntPtr hWndParent);
         private static bool picflag = true;
         public static void savePic()
         {
             while (true) {
                
                 if (picflag)
                 {
                 }
             
             }
         }
         public void MsgCallback(int lCommand, ref HKSDK.NET_DVR_ALARMER pAlarmer, IntPtr pAlarmInfo, uint dwBufLen, IntPtr pUser)
         {
             //通过lCommand来判断接收到的报警信息类型，不同的lCommand对应不同的pAlarmInfo内容
             switch (lCommand)
             {                       
                 case HKSDK.COMM_UPLOAD_PLATE_RESULT://旧上传
                     ProcessCommAlarm_Plate(ref pAlarmer, pAlarmInfo, dwBufLen, pUser);
                     break;
                 case HKSDK.COMM_ITS_PLATE_RESULT://新上传
                     ProcessCommAlarm_ITSPlate(ref pAlarmer, pAlarmInfo, dwBufLen, pUser);
                     break;
                 default:
                     break;
             }

         }

         ///  新上传处理函数      
         private void ProcessCommAlarm_ITSPlate(ref HKSDK.NET_DVR_ALARMER pAlarmer, IntPtr pAlarmInfo, uint dwBufLen, IntPtr pUser)
         {
             HKSDK.NET_ITS_PLATE_RESULT struITSPlateResult = new HKSDK.NET_ITS_PLATE_RESULT();
             uint dwSize = (uint)Marshal.SizeOf(struITSPlateResult);
             //PtrToStructure返回托管对象  将非托管对象封送到新分配的指定类型的托管
             struITSPlateResult = (HKSDK.NET_ITS_PLATE_RESULT)Marshal.PtrToStructure(pAlarmInfo, typeof(HKSDK.NET_ITS_PLATE_RESULT));
             //保存抓拍图片
             for (int i = 0; i < struITSPlateResult.dwPicNum; i++)
             {
                 if (struITSPlateResult.struPicInfo[i].dwDataLen != 0)
                 {
                     pictureBox1.Image = Image.FromFile("");
                     if (InvokeRequired)//非当前线程
                     {
                         pictureBox1.BeginInvoke(new UpdateListBoxCallback(UpdateClientList));
                     }
                     else
                     {   //当前线程
                           
                      //   UpdateClientList(0,0);
                     }
                //    pictureBox1.Image = Image.FromFile(@"D:\卡口\20150416\20150416163914627.jpg"); 
                 }
             }
         }

         private void ProcessCommAlarm_Plate(ref HKSDK.NET_DVR_ALARMER pAlarmer, IntPtr pAlarmInfo, uint dwBufLen, IntPtr pUser)
         {
             try
             {
                 
                 HKSDK.NET_DVR_PLATE_RESULT struPlateResultInfo = new HKSDK.NET_DVR_PLATE_RESULT();

                 uint dwSize = (uint)Marshal.SizeOf(struPlateResultInfo);
            
                 struPlateResultInfo = (HKSDK.NET_DVR_PLATE_RESULT)Marshal.PtrToStructure(pAlarmInfo, typeof(HKSDK.NET_DVR_PLATE_RESULT));
                 if (struPlateResultInfo.byResultType == 1&&struPlateResultInfo.dwPicLen>0)
                {

                    byte[] by = new byte[struPlateResultInfo.dwPicLen];
                    Marshal.Copy(struPlateResultInfo.pBuffer1, by, 0, (int)struPlateResultInfo.dwPicLen);


                    MemoryStream ms = new MemoryStream(by, 0, (int)struPlateResultInfo.dwPicLen);
                    Image returnImage = Image.FromStream(ms);
                    pictureBox1.Image = returnImage;
                    if (filepath.Text != "")
                     {
                         returnImage.Save(filepath.Text+vehnum.ToString()+".jpg");
                         vehnum++;
                     }
                   
                }

             }
             catch (System.Exception ex)
             {

             }

         }
         public void UpdateClientList(byte [] a,int b)
         {

         }
         //手动抓拍测试
         public HKSDK.tagNET_DVR_SNAPCFG captag;
        private void manualCap(object sender, EventArgs e)
        {
            //captag = new HKSDK.tagNET_DVR_SNAPCFG();
            //captag.bySnapTimes = 1;
            //captag.wSnapWaitTime = 1;
            //ushort[] capnum = new ushort[4];
            //capnum[0] = 1;
            //capnum[1] = 1;
            //capnum[2] = 2;
            //capnum[3] = 3;
            //captag.wIntervalTime = capnum;
            //captag.byRelatedDriveWay = 0;
             
            //captag.dwSize = (uint)Marshal.SizeOf(typeof(HKSDK.tagNET_DVR_SNAPCFG));
            //HKSDK.NET_DVR_ContinuousShoot(m_lUserID, ref captag);
            HKSDK.tagNET_DVR_MANUALSNAP msnap = new HKSDK.tagNET_DVR_MANUALSNAP();
            HKSDK.tagNET_DVR_PLATE_RESULT plateres = new HKSDK.tagNET_DVR_PLATE_RESULT();

            HKSDK.NET_DVR_ManualSnap(m_lUserID,ref msnap,ref plateres);
        }
    }
}

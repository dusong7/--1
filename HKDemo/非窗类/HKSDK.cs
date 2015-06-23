using System;
using System.Runtime.InteropServices;

namespace HKDemo
{
 
   public class HKSDK
    {
        public HKSDK()
        {//构造函数

        }
        
        public const int  MAX_INTERVAL_NUM = 4;    //最大时间间隔个数
        public const int SERIALNO_LEN = 48;//序列号长度
        public const int STREAM_ID_LEN = 32;
        public const int NAME_LEN = 32;//用户名长度
        public const int PASSWD_LEN = 16;//密码长度
        public const int MACADDR_LEN = 6;//mac地址长度
        public const int MAX_LICENSE_LEN = 16;//车牌号最大长度
       //
        public const int COMM_UPLOAD_PLATE_RESULT = 0x2800;	 //上传车牌信息(老)
        public const int COMM_ITS_PLATE_RESULT = 0x3050;	 //上传车牌信息(新)
    //    public const int COMM_UPLOAD_PLATE_RESULT = 0x2800;	 //上传车牌信息
        //车牌识别结果子结构
        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct NET_DVR_PLATE_INFO
        {
            public byte byPlateType; //车牌类型
            public byte byColor; //车牌颜色
            public byte byBright; //车牌亮度
            public byte byLicenseLen;	//车牌字符个数
            public byte byEntireBelieve;//整个车牌的置信度，0-100
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 35, ArraySubType = UnmanagedType.I1)]
            public byte[] byRes;//保留
            public NET_VCA_RECT struPlateRect;	//车牌位置
            //指示在托管和非托管代码间封送数据
            [MarshalAsAttribute(UnmanagedType.ByValTStr, SizeConst = MAX_LICENSE_LEN)]
            public string sLicense;	//车牌号码 
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = MAX_LICENSE_LEN, ArraySubType = UnmanagedType.I1)]
            public byte[] byBelieve; //各个识别字符的置信度，如检测到车牌"浙A12345", 置信度为,20,30,40,50,60,70，则表示"浙"字正确的可能性只有%，"A"字的正确的可能性是%
        }
        //区域框结构
        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct NET_VCA_RECT
        {
            public float fX;//边界框左上角点的X轴坐标, 0.001~1
            public float fY;//边界框左上角点的Y轴坐标, 0.001~1
            public float fWidth;//边界框的宽度, 0.001~1
            public float fHeight;//边界框的高度, 0.001~1
        }
        //NET_DVR_Login_V30()参数结构
        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct NET_DVR_DEVICEINFO_V30
        {
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = SERIALNO_LEN, ArraySubType = UnmanagedType.I1)]
            public byte[] sSerialNumber;  //序列号
            public byte byAlarmInPortNum;		        //报警输入个数
            public byte byAlarmOutPortNum;		        //报警输出个数
            public byte byDiskNum;				    //硬盘个数
            public byte byDVRType;				    //设备类型, 1:DVR 2:ATM DVR 3:DVS ......
            public byte byChanNum;				    //模拟通道个数
            public byte byStartChan;			        //起始通道号,例如DVS-1,DVR - 1
            public byte byAudioChanNum;                //语音通道数
            public byte byIPChanNum;					//最大数字通道个数，低位  
            public byte byZeroChanNum;			//零通道编码个数 //2010-01-16
            public byte byMainProto;			//主码流传输协议类型 0-private, 1-rtsp,2-同时支持private和rtsp
            public byte bySubProto;				//子码流传输协议类型0-private, 1-rtsp,2-同时支持private和rtsp
            public byte bySupport;        //能力，位与结果为0表示不支持，1表示支持，

            public byte bySupport1;        // 能力集扩充，位与结果为0表示不支持，1表示支持
            public byte bySupport2; /*能力，位与结果为0表示不支持，非0表示支持							
							bySupport2 & 0x1, 表示解码器是否支持通过URL取流解码
							bySupport2 & 0x2,  表示支持FTPV40
							bySupport2 & 0x4,  表示支持ANR
							bySupport2 & 0x8,  表示支持CCD的通道参数配置
							bySupport2 & 0x10,  表示支持布防报警回传信息（仅支持抓拍机报警 新老报警结构）
							bySupport2 & 0x20,  表示是否支持单独获取设备状态子项
							bySupport2 & 0x40,  表示是否是码流加密设备*/
            public ushort wDevType;              //设备型号
            public byte bySupport3; //能力集扩展，位与结果为0表示不支持，1表示支持
            //bySupport3 & 0x1, 表示是否多码流
            // bySupport3 & 0x4 表示支持按组配置， 具体包含 通道图像参数、报警输入参数、IP报警输入、输出接入参数、
            // 用户参数、设备工作状态、JPEG抓图、定时和时间抓图、硬盘盘组管理 
            //bySupport3 & 0x8为1 表示支持使用TCP预览、UDP预览、多播预览中的"延时预览"字段来请求延时预览（后续都将使用这种方式请求延时预览）。而当bySupport3 & 0x8为0时，将使用 "私有延时预览"协议。
            //bySupport3 & 0x10 表示支持"获取报警主机主要状态（V40）"。
            //bySupport3 & 0x20 表示是否支持通过DDNS域名解析取流

            public byte byMultiStreamProto;//是否支持多码流,按位表示,0-不支持,1-支持,bit1-码流3,bit2-码流4,bit7-主码流，bit-8子码流
            public byte byStartDChan;		//起始数字通道号,0表示无效
            public byte byStartDTalkChan;	//起始数字对讲通道号，区别于模拟对讲通道号，0表示无效
            public byte byHighDChanNum;		//数字通道个数，高位
            public byte bySupport4;
            public byte byLanguageType;// 支持语种能力,按位表示,每一位0-不支持,1-支持  
            //  byLanguageType 等于0 表示 老设备
            //  byLanguageType & 0x1表示支持中文
            //  byLanguageType & 0x2表示支持英文
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 9, ArraySubType = UnmanagedType.I1)]
            public byte[] byRes2;		//保留
        }
        //预览V40接口
        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct NET_DVR_PREVIEWINFO
        {
            public Int32 lChannel;//通道号
            public uint dwStreamType;	// 码流类型，0-主码流，1-子码流，2-码流3，3-码流4 等以此类推
            public uint dwLinkMode;// 0：TCP方式,1：UDP方式,2：多播方式,3 - RTP方式，4-RTP/RTSP,5-RSTP/HTTP 
            public IntPtr hPlayWnd;//播放窗口的句柄,为NULL表示不播放图象
            public bool bBlocked;  //0-非阻塞取流, 1-阻塞取流, 如果阻塞SDK内部connect失败将会有5s的超时才能够返回,不适合于轮询取流操作.
            public bool bPassbackRecord; //0-不启用录像回传,1启用录像回传
            public byte byPreviewMode;//预览模式，0-正常预览，1-延迟预览
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = STREAM_ID_LEN, ArraySubType = UnmanagedType.I1)]
            public byte[] byStreamID;//流ID，lChannel为0xffffffff时启用此参数
            public byte byProtoType; //应用层取流协议，0-私有协议，1-RTSP协议
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 222, ArraySubType = UnmanagedType.I1)]
            public byte[] byRes;
        }
        public struct NET_DVR_CLIENTINFO
        {
            public Int32 lChannel;//通道号
            public Int32 lLinkMode;//最高位(31)为0表示主码流，为1表示子码流，0－30位表示码流连接方式: 0：TCP方式,1：UDP方式,2：多播方式,3 - RTP方式，4-音视频分开(TCP)
            public IntPtr hPlayWnd;//播放窗口的句柄,为NULL表示不播放图象
            public string sMultiCastIP;//多播组地址
        }
        //报警设备信息
        [StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct NET_DVR_ALARMER 
        {
            public byte byUserIDValid;/* userid是否有效 0-无效，1-有效 */
            public byte bySerialValid;/* 序列号是否有效 0-无效，1-有效 */
            public byte byVersionValid;/* 版本号是否有效 0-无效，1-有效 */
            public byte byDeviceNameValid;/* 设备名字是否有效 0-无效，1-有效 */
            public byte byMacAddrValid; /* MAC地址是否有效 0-无效，1-有效 */
            public byte byLinkPortValid;/* login端口是否有效 0-无效，1-有效 */
            public byte byDeviceIPValid;/* 设备IP是否有效 0-无效，1-有效 */
            public byte bySocketIPValid;/* socket ip是否有效 0-无效，1-有效 */
            public int lUserID; /* NET_DVR_Login()返回值, 布防时有效 */
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = SERIALNO_LEN, ArraySubType = UnmanagedType.I1)]
            public byte[] sSerialNumber;/* 序列号 */
            public uint dwDeviceVersion;/* 版本信息 高16位表示主版本，低16位表示次版本*/
            [MarshalAsAttribute(UnmanagedType.ByValTStr, SizeConst = NAME_LEN)]
            public string sDeviceName;/* 设备名字 */
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = MACADDR_LEN, ArraySubType = UnmanagedType.I1)]
            public byte[] byMacAddr;/* MAC地址 */
            public ushort wLinkPort; /* link port */
            [MarshalAsAttribute(UnmanagedType.ByValTStr, SizeConst = 128)]
            public string sDeviceIP;/* IP地址 */
            [MarshalAsAttribute(UnmanagedType.ByValTStr, SizeConst = 128)]
            public string sSocketIP;/* 报警主动上传时的socket IP地址 */
            public byte byIpProtocol; /* Ip协议 0-IPV4, 1-IPV6 */
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 11, ArraySubType = UnmanagedType.I1)]
            public byte[] byRes2;
        }
       //车辆信息结构体
        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct NET_DVR_PLATE_RESULT
        {
            public uint dwSize;
            public byte byResultType;
            public byte byChanIndex;
            public ushort wAlarmRecordID;	//报警录像ID(用于查询录像，仅当byResultType为2时有效)
            public uint dwRelativeTime;
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 32, ArraySubType = UnmanagedType.I1)]
            public byte[] byAbsTime;
            public uint dwPicLen;
            public uint dwPicPlateLen;
            public uint dwVideoLen;
            public byte byTrafficLight;
            public byte byPicNum;
            public byte byDriveChan;
            public byte byVehicleType; //0- 未知，1- 客车，2- 货车，3- 轿车，4- 面包车，5- 小货车
            public uint dwBinPicLen;
            public uint dwCarPicLen;
            public uint dwFarCarPicLen;
            public IntPtr pBuffer3;
            public IntPtr pBuffer4;
            public IntPtr pBuffer5;
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 8, ArraySubType = UnmanagedType.I1)]
            public byte[] byRes3;
            public NET_DVR_PLATE_INFO struPlateInfo;
            public NET_DVR_VEHICLE_INFO struVehicleInfo;
            public IntPtr pBuffer1;
            public IntPtr pBuffer2;

            public void Init()
            {
                byAbsTime = new byte[32];
                byRes3 = new byte[8];
            }
        }
       //抓拍结构体
        
        [StructLayoutAttribute(LayoutKind.Sequential)]
       public struct tagNET_DVR_SNAPCFG
        {
            public uint   dwSize;
            public byte byRelatedDriveWay;//触发IO关联的车道号
            public byte bySnapTimes; //线圈抓拍次数，0-不抓拍，非0-连拍次数，目前最大5次  
            public ushort wSnapWaitTime;  //抓拍等待时间，单位ms，取值范围[0,60000]
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = MAX_INTERVAL_NUM)]
            public ushort[] wIntervalTime;//连拍间隔时间，ms
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 24)]
            public byte[] byRes2;//保留字节
        }
        //图片信息
        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct NET_ITS_PICTURE_INFO
        {
            public uint dwDataLen;
            public byte byType;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
            public byte[] byRes1;
            public uint dwRedLightTime;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
            public byte[] byAbsTime;
            public NET_VCA_RECT struPlateRect;
            public NET_VCA_RECT struPlateRecgRect;
            public IntPtr pBuffer;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 12)]
            public byte[] byRes2;
        }
       //时间结构体
        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct NET_DVR_TIME_V30
        {
            public ushort wYear;
            public byte byMonth;
            public byte byDay;
            public byte byHour;
            public byte byMinute;
            public byte bySecond;
            public byte byRes;
            public ushort wMilliSec;
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 2, ArraySubType = UnmanagedType.I1)]
            public byte[] byRes1;
        }

       //
       //车牌检测结果
        public struct tagNET_DVR_PLATE_RESULT
        {
            uint   dwSize;	        //结构长度
            byte	byResultType;	//0-视频识别结果，1图像识别结果 2 大于10M时走下载路线
	        byte    byChanIndex;	//通道号
            ushort    wAlarmRecordID;	//报警录像ID(用于查询录像，仅当byResultType为2时有效)
            uint   dwRelativeTime;	//相对时间点  
             [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 32)]
            byte[]    byAbsTime;	//绝对时间点,yyyymmddhhmmssxxx,e.g.20090810235959999（毫秒）
            uint   dwPicLen;	    //图片长度(近景图)
            uint	dwPicPlateLen;  //车牌小图片长度
            uint   dwVideoLen;	    //录像内容长度
            byte	byTrafficLight;	//0-非红绿灯抓拍，1-绿灯时抓拍；2-红灯时抓拍
            byte    byPicNum;       //连拍的图片序号
            byte	byDriveChan;    //触发车道号
            byte	 byVehicleType; //车辆类型，参考VTR_RESULT
            uint   dwBinPicLen;    //车牌二值图长度
	        uint   dwCarPicLen;    //车辆原图长度
	        uint   dwFarCarPicLen; //远景图长度
            IntPtr pBuffer3;      //车牌二值图
	        IntPtr pBuffer4;      //车辆原图
	        IntPtr pBuffer5;      //远景图
             [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 8)]
	        byte[]    byRes3;      //保留 
            NET_DVR_PLATE_INFO  struPlateInfo;    //车牌信息结构
            NET_DVR_VEHICLE_INFO struVehicleInfo; //车辆信息
            IntPtr pBuffer1;   		        // 当上传的是图片(近景图)，指针指向图片信息，当上传的是视频，指针指向视频信息，如果不想获得图片或视频信息，传NULL(DVS车辆近景图)
            IntPtr pBuffer2;                  // 当上传的是图片(车牌图)时，指向车牌图片的指针（DVS车牌彩图）
        }
       //新上传结构体
        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct NET_ITS_PLATE_RESULT
        {
            public uint dwSize;
            public uint dwMatchNo;
            public byte byGroupNum;
            public byte byPicNo;
            public byte bySecondCam;    //是否第二相机抓拍（如远近景抓拍的远景相机，或前后抓拍的后相机，特殊项目中会用到）
            public byte byFeaturePicNo; //闯红灯电警，取第几张图作为特写图,0xff-表示不取
            public byte byDriveChan;                //触发车道号
            public byte byVehicleType;     //0- 未知，1-客车，2-货车，3-轿车，4-面包车，5-小货车
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 2, ArraySubType = UnmanagedType.I1)]
            public byte[] byRes1;                        //保留
            public ushort wIllegalType;       //违章类型采用国标定义
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 8, ArraySubType = UnmanagedType.I1)]
            public byte[] byIllegalSubType;   //违章子类型
            public byte byPostPicNo;    //违章时取第几张图片作为卡口图,0xff-表示不取
            public byte byChanIndex;                //通道号（保留）
            public ushort wSpeedLimit;            //限速上限（超速时有效）
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 2, ArraySubType = UnmanagedType.I1)]
            public byte[] byRes2;
            public NET_DVR_PLATE_INFO struPlateInfo;       //车牌信息结构
            public NET_DVR_VEHICLE_INFO struVehicleInfo;        //车辆信息
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 48, ArraySubType = UnmanagedType.I1)]
            public byte[] byMonitoringSiteID;          //监测点编号
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 48, ArraySubType = UnmanagedType.I1)]
            public byte[] byDeviceID;                                   //设备编号
            public byte byDir;                //监测方向，1-上行，2-下行，3-双向，4-由东向西，5-由南向北,6-由西向东，7-由北向南，8-其它
            public byte byDetectType;    //检测方式,1-地感触发，2-视频触发，3-多帧识别，4-雷达触发
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 22, ArraySubType = UnmanagedType.I1)]
            public byte[] byRes3; //保留
            public NET_DVR_TIME_V30 struSnapFirstPicTime;//端点时间(ms)（抓拍第一张图片的时间）
            public uint dwIllegalTime;//违法持续时间（ms） = 抓拍最后一张图片的时间 - 抓拍第一张图片的时间
            public uint dwPicNum;            //图片数量（与picGroupNum不同，代表本条信息附带的图片数量，图片信息由struVehicleInfoEx定义   
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 6, ArraySubType = UnmanagedType.Struct)]
            public NET_ITS_PICTURE_INFO[] struPicInfo;                //图片信息,单张回调，最多6张图，由序号区分            
        }
        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct NET_DVR_VEHICLE_INFO
        {
            public uint dwIndex;
            public byte byVehicleType;
            public byte byColorDepth;
            public byte byColor;
            public byte byRes1;
            public ushort wSpeed;
            public ushort wLength;
            public byte byIllegalType;
            public byte byVehicleLogoRecog; //参考枚举类型 VLR_VEHICLE_CLASS
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 2, ArraySubType = UnmanagedType.I1)]
            public byte[] byRes2; //保留
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 16, ArraySubType = UnmanagedType.I1)]
            public byte[] byCustomInfo;  //自定义信息
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 16, ArraySubType = UnmanagedType.I1)]
            public byte[] byRes;

            public void Init()
            {
                byRes2 = new byte[2];
                byCustomInfo = new byte[16];
                byRes = new byte[16];
            }
        }

        //报警布防参数结构体旧
        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct NET_DVR_SETUPALARM_PARAM
        {
            public uint dwSize;
            public byte byLevel;//布防优先级：0- 一等级（高），1- 二等级（中），2- 三等级（低，保留）
            public byte byAlarmInfoType;//上传报警信息类型（智能交通摄像机支持）：0- 老报警信息（NET_DVR_PLATE_RESULT），1- 新报警信息(NET_ITS_PLATE_RESULT) 
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 14, ArraySubType = UnmanagedType.I1)]
            public byte[] byRes;//这里保留音频的压缩参数 
        }
       //报警布防参数结构体新
        public struct tagNET_DVR_SETUPALARM_PARAM
        {
           public uint dwSize;
           public byte byLevel; //布防优先级，0-一等级（高），1-二等级（中），2-三等级（低）
           public byte byAlarmInfoType; //上传报警信息类型（抓拍机支持），0-老报警信息（NET_DVR_PLATE_RESULT），1-新报警信息(NET_ITS_PLATE_RESULT)2012-9-28
           public byte byRetAlarmTypeV40; //0--返回NET_DVR_ALARMINFO_V30或NET_DVR_ALARMINFO, 1--设备支持NET_DVR_ALARMINFO_V40则返回NET_DVR_ALARMINFO_V40，不支持则返回NET_DVR_ALARMINFO_V30或NET_DVR_ALARMINFO
           public byte byRetDevInfoVersion; //CVR上传报警信息回调结构体版本号 0-COMM_ALARM_DEVICE， 1-COMM_ALARM_DEVICE_V40
           public byte byRetVQDAlarmType; //VQD报警上传类型，0-上传报报警NET_DVR_VQD_DIAGNOSE_INFO，1-上传报警NET_DVR_VQD_ALARM
	        [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 11)]
           public byte[] byRes;
        }
       //抓拍触发请求结构(保留)
       public struct tagNET_DVR_MANUALSNAP
        {
	      public  byte   byOSDEnable;//0-不关闭(默认)，1-关闭
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 23)]
          public  byte[]   byRes; //保留
        }

        public delegate void REALDATACALLBACK(Int32 lRealHandle, UInt32 dwDataType, ref byte pBuffer, UInt32 dwBufSize, IntPtr pUser);
        //初始化函数
        [DllImport(@"HCNetSDK.dll")]
        public static extern bool NET_DVR_Init();
        //注册函数
        [DllImport(@"HCNetSDK.dll")]
        public static extern Int32 NET_DVR_Login_V30(string sDVRIP, Int32 wDVRPort, string sUserName, string sPassword, ref NET_DVR_DEVICEINFO_V30 lpDeviceInfo);
        //注销函数     
        [DllImport(@"HCNetSDK.dll")]
        public static extern bool NET_DVR_Logout_V30(Int32 lUserID);

        //打开预览    
        [DllImport(@"HCNetSDK.dll")]
        public static extern int NET_DVR_RealPlay_V40(int iUserID, ref NET_DVR_PREVIEWINFO lpPreviewInfo, REALDATACALLBACK fRealDataCallBack_V30, IntPtr pUser);
        [DllImport(@"HCNetSDK.dll")]
        public static extern uint NET_DVR_GetLastError();
        //关闭预览 
        [DllImport(@"HCNetSDK.dll")]
        public static extern bool NET_DVR_StopRealPlay(int iRealHandle);
        //配置报警条件。。。
        //委托        
        public delegate void MSGCallBack(int lCommand, ref NET_DVR_ALARMER pAlarmer, IntPtr pAlarmInfo, uint dwBufLen, IntPtr pUser);
        //设置报警回调 函数
        [DllImport(@"HCNetSDK.dll")]
        public static extern bool NET_DVR_SetDVRMessageCallBack_V30(MSGCallBack fMessageCallBack, IntPtr pUser);
        //报警布防 旧
    //    [DllImport(@"HCNetSDK.dll")]
    //    public static extern int    NET_DVR_SetupAlarmChan_V41(int lUserID, ref NET_DVR_SETUPALARM_PARAM lpSetupParam);
        ////报警扯防
        //[DllImport(@"HCNetSDK.dll")]
        //public static extern bool NET_DVR_CloseAlarmChan_V30(int lAlarmHandle);
        //释放SDK资源
        [DllImport(@"HCNetSDK.dll")]
        public static extern bool NET_DVR_Cleanup();
        //抓拍图片
        [DllImport(@"HCNetSDK.dll")]
        public static extern bool NET_DVR_CapturePicture(Int32 lRealHandle, string sPicFileName);
       //设置抓拍模式  
       [DllImport(@"HCNetSDK.dll")]
        public static extern bool NET_DVR_SetCapturePictureMode(long dwCaptureMode);
       //网络抓拍
       [DllImport(@"HCNetSDK.dll")]
       public static extern bool  NET_DVR_ContinuousShoot(long lUserID,ref tagNET_DVR_SNAPCFG lpInter);
       //网络布防 新
       [DllImport(@"HCNetSDK.dll")]
       public static extern int NET_DVR_SetupAlarmChan_V41(long lUserID, ref tagNET_DVR_SETUPALARM_PARAM lpSetupParam);
       //报警扯防
       [DllImport(@"HCNetSDK.dll")]
       public static extern bool NET_DVR_CloseAlarmChan_V30(int lAlarmHandle);
       //手动抓拍
       [DllImport(@"HCNetSDK.dll")]
       public static extern bool NET_DVR_ManualSnap(long lUserID, ref tagNET_DVR_MANUALSNAP lpInter,ref tagNET_DVR_PLATE_RESULT lpOuter);

    }
}

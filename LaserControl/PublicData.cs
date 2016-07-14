using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LaserControl
{

    /// <summary>
    /// 公用数据区
    /// </summary>
    class PublicData
    {
        public static int RunTrajIndex = 0;
        /// <summary>
        /// 产品名
        /// </summary>
        static public string ProductName = "";
        /// <summary>
        /// 工艺名
        /// </summary>
        static public string PresetName = "";
        /// <summary>
        /// 安全点
        /// </summary>
        static public double[] Home = new double[6];

        /// <summary>
        /// 工具
        /// </summary>
        static public double[] Tcp = new double[6];


        /// <summary>
        /// 用户坐标系
        /// </summary>
        static public double[] Frame = new double[6];

        /// <summary>
        /// 轨迹总个数
        /// </summary>
        public static int TrajCount = 0;

        /// <summary>
        /// 激光功率
        /// </summary>
        public static double LaserPower = 0;

        /// <summary>
        /// 送粉量
        /// </summary>
        public static double Powder = 0;

        /// <summary>
        /// 机器人速度比
        /// </summary>
        public static double Speed=1;


        /// <summary>
        /// 当前的机器人joint
        /// </summary>
        public static double[] Joint=new double[6];

        /// <summary>
        /// 当前的Cart坐标
        /// </summary>
        public static double[] Cart=new double[6];

        /// <summary>
        /// 当前的IP
        /// </summary>
        public static string Ip = "127.0.0.1";


        /// <summary>
        /// 当前机器人类型
        /// </summary>
        public static string RobotType;


        /// <summary>
        /// 机器人模式，远程，自动，手动，测试
        /// </summary>
        public static int RobotMode;

        /// <summary>
        /// 通讯状态
        /// </summary>
        public static bool SoapStatus = false;

        /// <summary>
        /// 是否上电
        /// </summary>
        public static bool PowerOn = false;
        
        /// <summary>
        /// 机械手臂是否空闲
        /// </summary>
        public static bool Idle=true;
        /// <summary>
        /// 机械手臂是否回零
        /// </summary>
        public static bool IsHome = false;


        /// <summary>
        /// 机器人是否静止
        /// </summary>
        public static bool RobotStatic = false;
    }







    public class Traj
    {
        public int [] PointCount=new int [3];

        /// <summary>
        /// 轨迹坐标系
        /// </summary>
        public double[] TrajFrame=new double[6];


        /// <summary>
        /// 表示MOVEJ/MOVEL
        /// </summary>
        public bool[] Before=new bool[10000];



        /// <summary>
        /// 表示MOVEJ/MOVEL
        /// </summary>
        public bool[] Mid=new bool[10000];


        /// <summary>
        /// 表示MOVEJ/MOVEL
        /// </summary>
        public bool[] After=new bool[10000];


        /// <summary>
        /// 开始点的点的信息
        /// </summary>
        public double[][] Beforepoint=new double[10000][];
        /// <summary>
        /// 切割点的信息
        /// </summary>
        public double[][] Midpoint=new double[10000][];

        public double[][] MidMC = new double[10000][];
        /// <summary>
        /// 离开点的信息
        /// </summary>
        public double[][] Afterpoint=new double[10000][];

        //参数包括 速度，加速度，blend,动作，工艺

        /// <summary>
        /// 开始点的参数
        /// </summary>
        public double[][] BeforePara=new double[10000][];
        /// <summary>
        /// 切割点的参数
        /// </summary>
        public double[][] MidPara=new double[10000][];
     
        /// <summary>
        /// 离开点的参数
        /// </summary>
        public double[][] AfterPara=new double[10000][];


        /// <summary>
        /// 延时
        /// </summary>
        public double Delay;


    }







}

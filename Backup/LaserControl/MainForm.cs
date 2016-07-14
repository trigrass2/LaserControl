using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using StaubliRobot;
using System.Threading;


namespace LaserControl
{



    public partial class MainForm : Form
    {
        /// <summary>
        /// 声明路径信息
        /// </summary>
        Traj[] TrajInform;
        Thread newThread = null;

        public MainForm()
        {
            InitializeComponent();
            TB_ipSetting.Text = PublicData.Ip;

        }



        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BT_LoginOK_Click(object sender, EventArgs e)
        {
            if (TB_Name.Text.Trim() == "" || TB_Password.Text.Trim() == "")
            {
                MessageBox.Show("登录名或者密码不能为空!");
                return;


            }
            if (TB_Name.Text.Trim() == "admin" && TB_Password.Text.Trim() == "admin")
            {
                if (TC_Main.TabPages.Count<=1)
                {
                    TC_Main.TabPages.Add(TP_Monitor);
                    TC_Main.TabPages.Add(TP_Recipe);
                    TC_Main.TabPages.Add(TP_Product);
     
                }
                MessageBox.Show("登录成功!");
                return;
            }
            else
            {
                MessageBox.Show("登录失败!");
                return;
            }

        }



        bool IsMoved = false;
        /// <summary>
        /// 状态显示
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timer_Status_Tick(object sender, EventArgs e)
        {
            if (!Res)
            {
                LB_Status.Text = "运行状态:错误 路径" + PublicData.RunTrajIndex.ToString() + "序号:" + TrajIndex.ToString();
            }
            else
            {
                LB_Status.Text = "运行状态:正常 路径" + PublicData.RunTrajIndex.ToString() + "序号:" + TrajIndex.ToString();
            }
            LL_RunTrajIndex.Text = "生产路径：" + PublicData.RunTrajIndex.ToString();
            CK_Static.Checked = PublicData.RobotStatic;
            CK_Home.Checked = PublicData.IsHome;
            ck_Soapstatus.Checked = PublicData.SoapStatus;
            CK_Power.Checked = PublicData.PowerOn;
            CK_Idle.Checked = PublicData.Idle;
            LL_IP.Text = "IP:" + PublicData.Ip;
            TB_J1.Text = PublicData.Joint[0].ToString();
            TB_J2.Text = PublicData.Joint[1].ToString();
            TB_J3.Text = PublicData.Joint[2].ToString();
            TB_J4.Text = PublicData.Joint[3].ToString();
            TB_J5.Text = PublicData.Joint[4].ToString();
            TB_J6.Text = PublicData.Joint[5].ToString();

            TB_X.Text = PublicData.Cart[0].ToString();
            TB_Y.Text = PublicData.Cart[1].ToString();
            TB_Z.Text = PublicData.Cart[2].ToString();
            TB_RX.Text = PublicData.Cart[3].ToString();
            TB_RY.Text = PublicData.Cart[4].ToString();
            TB_RZ.Text = PublicData.Cart[5].ToString();

            PB_Process.Maximum = PublicData.TrajCount;
            if (PublicData.RunTrajIndex<=PB_Process.Maximum)
            {
                PB_Process.Value = PublicData.RunTrajIndex;
            }
            if (BT_Start.Text=="停止")
            {
                if (!PublicData.IsHome)
                {
                    IsMoved = true;
                }
                
                if (PublicData.IsHome && IsMoved)
                {

                    BT_Start_Click(this, null);
                }
            }

            tssl_date.Text = "       日期:"+DateTime.Now.ToLongDateString()+DateTime.Now.ToLongTimeString();

        }

        private void TP_Monitor_Click(object sender, EventArgs e)
        {

        }




        /// <summary>
        /// 打开文件并且进行读取
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BT_ProductImport_Click(object sender, EventArgs e)
        {
            if (ofd_ProductFile.ShowDialog() == DialogResult.OK)
            {
                if (ReadFile(ofd_ProductFile.FileName))
                {
                    MessageBox.Show("导入成功");


                    ProductInformShow();
                    NUD_ValueChanged(this, null);
                    return;


                }
                else
                {
                    MessageBox.Show("导入错误!");
                    return;
                }

            }

        }



        private void ProductInformShow()
        {
            NUD.Maximum = PublicData.TrajCount;
            TB_Frame.Text = "0,0,0,0,0,0";
            TB_TrajFrame.Text = "0,0,0,0,0,0";
            TB_ProductName.Text = PublicData.ProductName;
            TB_TrajCount.Text = PublicData.TrajCount.ToString();
            TB_Tool.Text = PublicData.Tcp[0].ToString() + "," + PublicData.Tcp[1].ToString() + "," + PublicData.Tcp[2].ToString() + "," + PublicData.Tcp[3].ToString() + "," + PublicData.Tcp[4].ToString() + "," + PublicData.Tcp[5].ToString();
            TB_Home.Text = PublicData.Home[0].ToString() + "," + PublicData.Home[1].ToString() + "," + PublicData.Home[2].ToString() + "," + PublicData.Home[3].ToString() + "," + PublicData.Home[4].ToString() + "," + PublicData.Home[5].ToString();
            TB_Frame.Text = PublicData.Frame[0].ToString() + "," + PublicData.Frame[1].ToString() + "," + PublicData.Frame[2].ToString() + "," + PublicData.Frame[3].ToString() + "," + PublicData.Frame[4].ToString() + "," + PublicData.Frame[5].ToString();
            if (PublicData.TrajCount >= 1)
            {
                TB_TrajFrame.Text = TrajInform[0].TrajFrame[0].ToString() + "," + TrajInform[0].TrajFrame[1].ToString() + "," + TrajInform[0].TrajFrame[2].ToString() + "," +
                    TrajInform[0].TrajFrame[3].ToString() + "," + TrajInform[0].TrajFrame[4].ToString() + "," + TrajInform[0].TrajFrame[5].ToString();                //  TB_TrajFrame.Text = TrajInform
            }
        }




        private void Str2double(string[] source, ref double[] dest, int FormIndex)
        {
            if (source == null || dest == null || FormIndex > source.Length - 1)
            {
                return;
            }
            int J = 0;
            for (int i = FormIndex; i < source.Length; i++)
            {
                try
                {
                    if (J >= dest.Length)
                    {
                        return;
                    }

                    dest[J] = double.Parse(source[i]);
                    J++;
                }
                catch (System.Exception ex)
                {

                }


            }
        }

        private void Str2double(string[] source, ref double[] dest)
        {
            Str2double(source, ref dest, 0);
        }

        /// <summary>
        /// 读取文件
        /// </summary>
        /// <param name="FileName"></param>
        /// <returns></returns>
        private bool ReadFile(string FileName)
        {
            //参数定义
            FileStream fs = null;
            StreamReader sr = null;
            //每行字符串
            string StrLine;
            //行数
            int LineCount = 0;
            //文本中一共多少个字符
            int StrCharCount = 0;
            int TrajIndex = -1;
            int PointType = 0;

            try
            {
                if (FileName == null || !File.Exists(FileName))
                {
                    MessageBox.Show("文件不存在!");
                    return false;
                }


                fs = new FileStream(FileName, FileMode.Open, FileAccess.Read);
                sr = new StreamReader(fs);  //使用StreamReader类来读取文件 
                sr.BaseStream.Seek(0, SeekOrigin.Begin);

                while ((StrLine = sr.ReadLine()) != null)
                {

                    LineCount++;
                    StrLine = StrLine.Trim();
                    if (StrLine == "" || StrLine.IndexOf("//") >= 0)
                    {
                        continue;

                    }
                    StrCharCount = StrLine.Length;
                    //读取productname
                    if (StrLine.IndexOf("PRODUCTID=") >= 0)
                    {
                        PublicData.ProductName = StrLine.Substring("PRODUCTID=".Length, StrCharCount - "PRODUCTID=".Length);

                    }


                    if (StrLine.IndexOf("PRESET=") >= 0)
                    {
                        PublicData.PresetName = StrLine.Substring("PRESET=".Length, StrCharCount - "PRESET=".Length);

                    }

                    if (StrLine.IndexOf("TOTALELEMENTS=") >= 0)
                    {
                        PublicData.TrajCount = int.Parse(StrLine.Substring("TOTALELEMENTS=".Length, StrCharCount - "TOTALELEMENTS=".Length));
                        TrajInform = new Traj[PublicData.TrajCount];
                    }

                    if (StrLine.IndexOf("=BEGIN") >= 0)
                    {
                        TrajIndex++;
                        PointType = 0;
                        TrajInform[TrajIndex] = new Traj();
                    }
                    //轨迹坐标系
                    if (StrLine.IndexOf("FRAME=") == 0)
                    {



                        string[] Data = (StrLine.Substring("FRAME=".Length, StrCharCount - "FRAME=".Length)).Split(',');


                        if (Data == null || Data.Length < 6)
                        {
                            return false;
                        }
                        Str2double(Data, ref  TrajInform[TrajIndex].TrajFrame);



                    }
                    //产品坐标系
                    if (StrLine.IndexOf("RECIPEFRAME=") == 0)
                    {



                        string[] Data = (StrLine.Substring("RECIPEFRAME=".Length, StrCharCount - "RECIPEFRAME=".Length)).Split(',');


                        if (Data == null || Data.Length < 6)
                        {
                            return false;
                        }
                        Str2double(Data, ref  PublicData.Frame);



                    }

                    if (StrLine.IndexOf("HOME=") >= 0)
                    {



                        string[] Data = (StrLine.Substring("HOME=".Length, StrCharCount - "HOME=".Length)).Split(',');


                        if (Data == null || Data.Length < 6)
                        {
                            return false;
                        }
                        Str2double(Data, ref  PublicData.Home);



                    }

                    if (StrLine.IndexOf("TOOL=") >= 0)
                    {



                        string[] Data = (StrLine.Substring("TOOL=".Length, StrCharCount - "TOOL=".Length)).Split(',');


                        if (Data == null || Data.Length < 6)
                        {
                            return false;
                        }
                        Str2double(Data, ref  PublicData.Tcp);



                    }

                    if (StrLine.IndexOf("LASER=OFF") >= 0)
                    {
                        PointType = 2;
                    }

                    if (StrLine.IndexOf("LASER=ON") >= 0)
                    {
                        PointType = 1;
                    }


                    if (StrLine.IndexOf("MOVEL=") >= 0)
                    {
                        string[] Data = (StrLine.Substring("MOVEL=".Length, StrCharCount - "MOVEL=".Length)).Split(new char[] { ',', '/' });
                        switch (PointType)
                        {

                            //趋近点
                            case 0:
                                {
                                    TrajInform[TrajIndex].Beforepoint[TrajInform[TrajIndex].PointCount[PointType]] = new double[6];
                                    TrajInform[TrajIndex].BeforePara[TrajInform[TrajIndex].PointCount[PointType]] = new double[2];
                                    Str2double(Data, ref TrajInform[TrajIndex].Beforepoint[TrajInform[TrajIndex].PointCount[PointType]]);
                                    Str2double(Data, ref TrajInform[TrajIndex].BeforePara[TrajInform[TrajIndex].PointCount[PointType]], 6);
                                    TrajInform[TrajIndex].Before[TrajInform[TrajIndex].PointCount[PointType]] = true;
                                    break;
                                }
                            //切割点
                            case 1:
                                {
                                    TrajInform[TrajIndex].Midpoint[TrajInform[TrajIndex].PointCount[PointType]] = new double[6];
                                    TrajInform[TrajIndex].MidPara[TrajInform[TrajIndex].PointCount[PointType]] = new double[4];
                                    Str2double(Data, ref TrajInform[TrajIndex].Midpoint[TrajInform[TrajIndex].PointCount[PointType]]);
                                    Str2double(Data, ref TrajInform[TrajIndex].MidPara[TrajInform[TrajIndex].PointCount[PointType]], 6);
                                    TrajInform[TrajIndex].Mid[TrajInform[TrajIndex].PointCount[PointType]] = false;


                                    break;
                                }

                            //切出点
                            case 2:
                                {
                                    TrajInform[TrajIndex].Afterpoint[TrajInform[TrajIndex].PointCount[PointType]] = new double[6];
                                    TrajInform[TrajIndex].AfterPara[TrajInform[TrajIndex].PointCount[PointType]] = new double[2];
                                    Str2double(Data, ref TrajInform[TrajIndex].Afterpoint[TrajInform[TrajIndex].PointCount[PointType]]);
                                    Str2double(Data, ref TrajInform[TrajIndex].AfterPara[TrajInform[TrajIndex].PointCount[PointType]], 6);
                                    TrajInform[TrajIndex].After[TrajInform[TrajIndex].PointCount[PointType]] = true;

                                    break;
                                }

                        }
                        TrajInform[TrajIndex].PointCount[PointType]++;
                    }
                    //
                    if (StrLine.IndexOf("MOVEC=") >= 0)
                    {
                        string[] Data = (StrLine.Substring("MOVEC=".Length, StrCharCount - "MOVEC=".Length)).Split(new char[] { ',', '/' });
                        if (PointType == 0 || PointType == 2)
                        {
                            return false;
                        }

                        TrajInform[TrajIndex].Midpoint[TrajInform[TrajIndex].PointCount[PointType]] = new double[6];
                        TrajInform[TrajIndex].MidMC[TrajInform[TrajIndex].PointCount[PointType]] = new double[6];
                        TrajInform[TrajIndex].MidPara[TrajInform[TrajIndex].PointCount[PointType]] = new double[4];

                        Str2double(Data, ref TrajInform[TrajIndex].Midpoint[TrajInform[TrajIndex].PointCount[PointType]]);
                        Str2double(Data, ref TrajInform[TrajIndex].MidMC[TrajInform[TrajIndex].PointCount[PointType]], 6);
                        Str2double(Data, ref TrajInform[TrajIndex].MidPara[TrajInform[TrajIndex].PointCount[PointType]], 12);
                        TrajInform[TrajIndex].Mid[TrajInform[TrajIndex].PointCount[PointType]] = true;





                        TrajInform[TrajIndex].PointCount[PointType]++;
                    }


                    //movej
                    if (StrLine.IndexOf("MOVEJ=") >= 0)
                    {
                        string[] Data = (StrLine.Substring("MOVEJ=".Length, StrCharCount - "MOVEJ=".Length)).Split(new char[] { ',', '/' });
                        if (PointType == 1)
                        {
                            return false;
                        }
                        switch (PointType)
                        {

                            //趋近点
                            case 0:
                                {
                                    TrajInform[TrajIndex].Beforepoint[TrajInform[TrajIndex].PointCount[PointType]] = new double[6];
                                    TrajInform[TrajIndex].BeforePara[TrajInform[TrajIndex].PointCount[PointType]] = new double[2];
                                    Str2double(Data, ref TrajInform[TrajIndex].Beforepoint[TrajInform[TrajIndex].PointCount[PointType]]);
                                    Str2double(Data, ref TrajInform[TrajIndex].BeforePara[TrajInform[TrajIndex].PointCount[PointType]], 6);
                                    TrajInform[TrajIndex].Before[TrajInform[TrajIndex].PointCount[PointType]] = false;
                                    break;
                                }


                            //切出点
                            case 2:
                                {
                                    TrajInform[TrajIndex].Afterpoint[TrajInform[TrajIndex].PointCount[PointType]] = new double[6];
                                    TrajInform[TrajIndex].AfterPara[TrajInform[TrajIndex].PointCount[PointType]] = new double[2];
                                    Str2double(Data, ref TrajInform[TrajIndex].Afterpoint[TrajInform[TrajIndex].PointCount[PointType]]);
                                    Str2double(Data, ref TrajInform[TrajIndex].AfterPara[TrajInform[TrajIndex].PointCount[PointType]], 6);
                                    TrajInform[TrajIndex].After[TrajInform[TrajIndex].PointCount[PointType]] = false;

                                    break;
                                }

                        }
                        TrajInform[TrajIndex].PointCount[PointType]++;
                    }


                }


                return true;
            }
            catch
            {
                // MessageBox.Show(ex.Message + LineCount.ToString());
                return false;
            }
            finally
            {
                if (fs != null)
                {
                    fs.Close();
                }
                if (sr != null)
                {
                    sr.Close();
                }


            }
        }


        bool Res = true;
        int TrajIndex;
        /// <summary>
        /// 运动函数
        /// </summary>
        private void Move()
        {
            Res = true;
            PublicData.Speed = Math.Max(0.01, PublicData.Speed);

            for (int TrajIndex = 0; TrajIndex < PublicData.TrajCount; TrajIndex++)
            {
                PublicData.RunTrajIndex = TrajIndex + 1;

                //切入点
                for (int i = 0; i < TrajInform[TrajIndex].PointCount[0]; i++)
                {

                    if (!TrajInform[TrajIndex].Before[i])
                    {

                        Res = SoapInstance.SoapWrite.MoveJ(TrajInform[TrajIndex].Beforepoint[i], TrajInform[TrajIndex].BeforePara[i][0] * PublicData.Speed, TrajInform[TrajIndex].BeforePara[i][1]);
                    }
                    else
                    {
                        Res = SoapInstance.SoapWrite.MoveL(TrajInform[TrajIndex].Beforepoint[i], PublicData.Tcp, TrajInform[TrajIndex].TrajFrame, TrajInform[TrajIndex].BeforePara[i][0] * PublicData.Speed, TrajInform[TrajIndex].BeforePara[i][1], 0);

                    }
                    if (!Res)
                    {
                        TrajIndex = i + 1000;
                        return;
                    }
                }


                while (!PublicData.RobotStatic)
                {
                    Thread.Sleep(10);
                }

                Thread.Sleep(1000);


                //切割

                for (int i = 0; i < TrajInform[TrajIndex].PointCount[1]; i++)
                {

                    if (TrajInform[TrajIndex].Mid[i])
                    {
                        Res = SoapInstance.SoapWrite.MoveC(TrajInform[TrajIndex].Midpoint[i], TrajInform[TrajIndex].MidMC[i], PublicData.Tcp, TrajInform[TrajIndex].TrajFrame, TrajInform[TrajIndex].MidPara[i][0] * PublicData.Speed, TrajInform[TrajIndex].MidPara[i][1], 0);

                    }
                    else
                    {
                        Res = SoapInstance.SoapWrite.MoveL(TrajInform[TrajIndex].Midpoint[i], PublicData.Tcp, TrajInform[TrajIndex].TrajFrame, TrajInform[TrajIndex].MidPara[i][0] * PublicData.Speed, TrajInform[TrajIndex].MidPara[i][1], 20);

                    }
                    //if (!Res)
                    //{
                    //    TrajIndex = i + 2000;
                    //    return;
                    //}

                }
                while (!PublicData.RobotStatic)
                {
                    Thread.Sleep(10);
                }

                Thread.Sleep(1000);


                //切出
                for (int i = 0; i < TrajInform[TrajIndex].PointCount[2]; i++)
                {


                    if (!TrajInform[TrajIndex].After[i])
                    {
                        Res = SoapInstance.SoapWrite.MoveJ(TrajInform[TrajIndex].Afterpoint[i], TrajInform[TrajIndex].AfterPara[i][0] * PublicData.Speed, TrajInform[TrajIndex].AfterPara[i][1]);
                    }
                    else
                    {
                        Res = SoapInstance.SoapWrite.MoveL(TrajInform[TrajIndex].Afterpoint[i], PublicData.Tcp, TrajInform[TrajIndex].TrajFrame, TrajInform[TrajIndex].AfterPara[i][0] * PublicData.Speed, TrajInform[TrajIndex].AfterPara[i][1], 10);


                    }
                    if (!Res)
                    {
                        TrajIndex = i + 3000;
                        return;
                    }

                }
                while (!PublicData.RobotStatic)
                {
                    Thread.Sleep(10);
                }

                Thread.Sleep(1000);

            }


            SoapInstance.SoapWrite.MoveJ(PublicData.Home);







        }






        //设置IP
        private void BT_IPSetting_Click(object sender, EventArgs e)
        {

        }

        private void BT_Login_Click(object sender, EventArgs e)
        {
            if (TB_ipSetting.Text.Trim() == "" || TB_ipSetting.Text.IndexOf(".") <= 0)
            {
                MessageBox.Show("ip不能为空!");
                return;
            }


            PublicData.Ip = TB_ipSetting.Text.Trim();

            StaubliRobot.SoapInstance.SoapRead = new StaubliRobot.SoapClient(PublicData.Ip);
            StaubliRobot.SoapInstance.SoapWrite = new StaubliRobot.SoapClient(PublicData.Ip);

            if (StaubliRobot.SoapInstance.SoapRead.Login() && StaubliRobot.SoapInstance.SoapWrite.Login())
            {
                PublicData.SoapStatus = true;
                MessageBox.Show("登录成功！");
                ThreadPool.QueueUserWorkItem(new WaitCallback(Soap), null);
                BT_LogOut.Enabled = true;
                BT_Login.Enabled = false;
            }
            else
            {
                PublicData.SoapStatus = false;
                MessageBox.Show("登录失败!");
            }


        }


        double[] Buff = new double[6];
        private void Soap(object target)
        {

            while (true)
            {

                if (PublicData.SoapStatus)
                {
                    PublicData.Joint = SoapInstance.SoapRead.GetJointPos();
                    PublicData.Cart = SoapInstance.SoapRead.GetCartesianPos();

                    if (PublicData.Cart != null && PublicData.Cart != null && PublicData.Home != null)
                    {
                        double err = 0;
                        for (int i = 0; i < 6; i++)
                        {
                            err = err + Math.Abs(PublicData.Joint[i] - PublicData.Home[i]);
                        }
                        PublicData.IsHome = err < 0.1;
                        err = 0;
                        for (int i = 0; i < 6; i++)
                        {
                            err = err + Math.Abs(PublicData.Joint[i] - Buff[i]);
                        }

                        PublicData.RobotStatic = err < 0.01;
                        for (int i = 0; i < 6; i++)
                        {
                            Buff[i] = PublicData.Joint[i];
                        }
                    }
                }





                Thread.Sleep(100);
            }




        }




        private void BT_LogOut_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("确定需要登出吗?", "") == DialogResult.Yes)
            {

                StaubliRobot.SoapInstance.SoapWrite.Logout();
                StaubliRobot.SoapInstance.SoapRead.Logout();
                PublicData.SoapStatus = false;
                BT_Login.Enabled = true;
            }
        }

        private void BT_Power_Click(object sender, EventArgs e)
        {
            if (!PublicData.SoapStatus)
            {
                MessageBox.Show("请确认通讯！");
                return;
            }
            if (BT_Power.Text == "上电")
            {
                PublicData.PowerOn = StaubliRobot.SoapInstance.SoapWrite.Poweron();
                if (PublicData.PowerOn)
                {
                    BT_Power.Text = "下电";
                    MessageBox.Show("上电成功！");
                }
                else
                {
                    MessageBox.Show("失败：检查模式或者急停按钮！");

                }
            }
            else
            {
                PublicData.PowerOn = StaubliRobot.SoapInstance.SoapWrite.Poweroff();
                if (PublicData.PowerOn)
                {
                    BT_Power.Text = "上电";
                    MessageBox.Show("下电成功！");
                }
                PublicData.PowerOn = !PublicData.PowerOn;
            }

        }

        private void BT_Start_Click(object sender, EventArgs e)
        {
            IsMoved = false;
            if (!PublicData.SoapStatus)
            {
                MessageBox.Show("请确认通讯！");
                return;
            }
            if (PublicData.ProductName == "")
            {
                MessageBox.Show("请先设置产品！");
                return;
            }
            BT_Pause.Text = "暂停";


            //******************************************
            if (BT_Start.Text == "开始")
            {



                if (!PublicData.PowerOn)
                {
                    MessageBox.Show("请先上电！");
                    return;
                }


                if (!PublicData.IsHome)
                {
                    MessageBox.Show("请先回零！");
                    return;
                }
                SoapInstance.SoapWrite.ResetMotion();
                SoapInstance.SoapWrite.RestartMotion();

                newThread = new Thread(new ThreadStart(Move));
                newThread.Start();

                BT_Start.Text = "停止";
                BT_Pause.Enabled = true;
            }

            else
            {
                SoapInstance.SoapWrite.StopMotion();
                if (newThread != null && newThread.IsAlive)
                {
                    newThread.Abort();
                }

                SoapInstance.SoapWrite.ResetMotion();




                BT_Start.Text = "开始";
                BT_Pause.Enabled = false;



            }



        }

        private void BT_Home_Click(object sender, EventArgs e)
        {
            //添加询问按钮

            if (!PublicData.PowerOn)
            {
                MessageBox.Show("请先上电！");
                return;
            }
            if (!PublicData.SoapStatus)
            {
                MessageBox.Show("请确认通讯！");
                return;
            }
            SoapInstance.SoapWrite.ResetMotion();
            SoapInstance.SoapWrite.RestartMotion();
            SoapInstance.SoapWrite.MoveJ(PublicData.Home);
            PublicData.IsHome = true;

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BT_Pause_Click(object sender, EventArgs e)
        {
            if (!PublicData.PowerOn)
            {
                MessageBox.Show("请先上电！");
                return;
            }
            if (!PublicData.SoapStatus)
            {
                MessageBox.Show("请确认通讯！");
                return;
            }
            if (BT_Pause.Text.Trim() == "暂停")
            {
                SoapInstance.SoapWrite.StopMotion();
                BT_Pause.Text = "继续";
            }
            else
            {
                SoapInstance.SoapWrite.RestartMotion();
                BT_Pause.Text = "暂停";
            }
        }

        private void GB_Recipe_Enter(object sender, EventArgs e)
        {

        }

        private void NUD_ValueChanged(object sender, EventArgs e)
        {
            string[] Data = new string[1000];
            int Index = 0;
            int TrajIndex = (int)(NUD.Value) - 1;
            string Buff;
            TB_TrajFrame.Text = TrajInform[TrajIndex].TrajFrame[0].ToString() + "," + TrajInform[TrajIndex].TrajFrame[1].ToString() + "," + TrajInform[TrajIndex].TrajFrame[2].ToString() + "," +
    TrajInform[TrajIndex].TrajFrame[3].ToString() + "," + TrajInform[TrajIndex].TrajFrame[4].ToString() + "," + TrajInform[TrajIndex].TrajFrame[5].ToString();

            Data[Index] = "趋近点：";

            for (int i = 0; i < TrajInform[TrajIndex].PointCount[0]; i++)
            {
                Index++;
                if (TrajInform[TrajIndex].Before[i])
                {
                    Buff = "MOVEL=";
                }
                else
                {
                    Buff = "MOVEJ=";
                }
                Buff = Buff + TrajInform[TrajIndex].Beforepoint[i][0].ToString() + "," + TrajInform[TrajIndex].Beforepoint[i][1].ToString() + "," + TrajInform[TrajIndex].Beforepoint[i][2].ToString() + "," +
                    TrajInform[TrajIndex].Beforepoint[i][3].ToString() + "," + TrajInform[TrajIndex].Beforepoint[i][4].ToString() + "," + TrajInform[TrajIndex].Beforepoint[i][5].ToString() + "/"
                        + TrajInform[TrajIndex].BeforePara[i][0].ToString() + "," + TrajInform[TrajIndex].BeforePara[i][1].ToString();
                Data[Index] = Buff;
            }


            Data[Index] = "切割点：";

            for (int i = 0; i < TrajInform[TrajIndex].PointCount[0]; i++)
            {
                Index++;
                if (TrajInform[TrajIndex].Mid[i])
                {
                    Buff = "MOVEC=";




                    Buff = Buff + TrajInform[TrajIndex].Midpoint[i][0].ToString() + "," + TrajInform[TrajIndex].Midpoint[i][1].ToString() + "," + TrajInform[TrajIndex].Midpoint[i][2].ToString() + "," +
                    TrajInform[TrajIndex].Midpoint[i][3].ToString() + "," + TrajInform[TrajIndex].Midpoint[i][4].ToString() + "," + TrajInform[TrajIndex].Midpoint[i][5].ToString() + "/"

           + TrajInform[TrajIndex].MidMC[i][0].ToString() + "," + TrajInform[TrajIndex].MidMC[i][1].ToString() + "," + TrajInform[TrajIndex].MidMC[i][2].ToString() + "," +
                    TrajInform[TrajIndex].MidMC[i][3].ToString() + "," + TrajInform[TrajIndex].MidMC[i][4].ToString() + "," + TrajInform[TrajIndex].MidMC[i][5].ToString() + "/"

                     + TrajInform[TrajIndex].MidPara[i][0].ToString() + "," + TrajInform[TrajIndex].MidPara[i][1].ToString() + "/"
                     + TrajInform[TrajIndex].MidPara[i][2].ToString() + "," + TrajInform[TrajIndex].MidPara[i][3].ToString();



                }
                else
                {
                    Buff = "MOVEL=";

                    Buff = Buff + TrajInform[TrajIndex].Midpoint[i][0].ToString() + "," + TrajInform[TrajIndex].Midpoint[i][1].ToString() + "," + TrajInform[TrajIndex].Midpoint[i][2].ToString() + "," +
                    TrajInform[TrajIndex].Midpoint[i][3].ToString() + "," + TrajInform[TrajIndex].Midpoint[i][4].ToString() + "," + TrajInform[TrajIndex].Midpoint[i][5].ToString() + "/"


                     + TrajInform[TrajIndex].MidPara[i][0].ToString() + "," + TrajInform[TrajIndex].MidPara[i][1].ToString() + "/"
                     + TrajInform[TrajIndex].MidPara[i][2].ToString() + "," + TrajInform[TrajIndex].MidPara[i][3].ToString();



                }




                Data[Index] = Buff;
            }



            Data[Index] = "离开点：";

            for (int i = 0; i < TrajInform[TrajIndex].PointCount[2]; i++)
            {
                Index++;
                if (TrajInform[TrajIndex].After[i])
                {
                    Buff = "MOVEL=";
                }
                else
                {
                    Buff = "MOVEJ=";
                }
                Buff = Buff + TrajInform[TrajIndex].Afterpoint[i][0].ToString() + "," + TrajInform[TrajIndex].Afterpoint[i][1].ToString() + "," + TrajInform[TrajIndex].Afterpoint[i][2].ToString() + "," +
                    TrajInform[TrajIndex].Afterpoint[i][3].ToString() + "," + TrajInform[TrajIndex].Afterpoint[i][4].ToString() + "," + TrajInform[TrajIndex].Afterpoint[i][5].ToString() + "/"
                        + TrajInform[TrajIndex].AfterPara[i][0].ToString() + "," + TrajInform[TrajIndex].AfterPara[i][1].ToString();
                Data[Index] = Buff;
            }





            LB_Traj.DataSource = Data;
        }
        bool Las = false;
        private void BT_SetOK_Click(object sender, EventArgs e)
        {
            PublicData.Speed = ((double)NUD_Speed.Value) / 100;
            //  SoapInstance.SoapWrite.rea
            SoapInstance.SoapWrite.WriteIO(@"ModbusSrv-0\Modbus-Bit\O1_bLaserOn", Las);
            SoapInstance.SoapWrite.WriteIO(@"ModbusSrv-0\Modbus-Word\O1_aLasPwr", double.Parse(TB_SetPower.Text.Trim()));
            SoapInstance.SoapWrite.WriteIO(@"ModbusSrv-0\Modbus-Word\O1_aFPWM", double.Parse(TB_SetPowder.Text.Trim()));
            Lb_aPWM.Text = "aPWM:" + SoapInstance.SoapWrite.ReadIOvalue(@"ModbusSrv-0\Modbus-Word\In4_a7CurPosH");
            Las = !Las;




        }

        private void button1_Click(object sender, EventArgs e)
        {
            // SoapInstance.SoapWrite.MoveJ(PublicData.Home);
            SoapInstance.SoapWrite.MoveJ(new double[] { 4.179, 14.715, 93.877, 42.225, 59.447, -80.529 });
            bool Res = SoapInstance.SoapWrite.MoveL(new double[] { 1310.635, 273.271, 286.791, 177.545, -6.375, 55.863 }, new double[] { 0, 0, 0, 0, 0, 0 }, new double[] { 0, 0, 0, 0, 0, 0 }, 100, 20, 0);
            if (!Res)
            {
                MessageBox.Show("Error");
                return;

            }




        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            if (TC_Main.TabPages.Count >= 1)
            {
                TC_Main.TabPages.Remove(TP_Monitor);
                TC_Main.TabPages.Remove(TP_Product);
                TC_Main.TabPages.Remove(TP_Recipe);
            }
            TB_Name.Focus();
        }

        private void TB_Name_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData==Keys.Enter)
            {
                TB_Password.Focus();
            }

        }

        private void TB_Password_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                BT_LoginOK_Click(this,null);
            }
        }
    }
}

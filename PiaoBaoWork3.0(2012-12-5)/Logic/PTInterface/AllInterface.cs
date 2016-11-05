using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using PbProject.Logic.ControlBase;

namespace PbProject.Logic.PTInterface
{
    public class AllInterface
    {
        BaseDataManage Manage = new BaseDataManage();
        /// <summary>
        /// 517线程数据
        /// </summary>
        public Thread Get517thread;
        private bool Data517Ok = false;
        private string errorData517 = "0";
        /// <summary>
        /// Baituo线程
        /// </summary>
        public Thread GetBaiTuothread;
        private bool DataBaiTuoOk = false;
        private string errorDataBaiTuo = "0";
        /// <summary>
        /// PM线程
        /// </summary>
        public Thread GetPMthread;
        private bool DataPMOk = false;
        private string errorDataPM = "0";
        /// <summary>
        /// 51book线程
        /// </summary>
        public Thread Get51bookthread;
        private bool Data51bookOk = false;
        private string errorData51book = "0";
        /// <summary>
        /// 8000yi线程
        /// </summary>
        public Thread Get8000Ythread;
        private bool Data8000YOk = false;
        private string errorData8000Y = "0";
        /// <summary>
        /// 今日线程
        /// </summary>
        public Thread GetTodaythread;
        private bool DataTodayOk = false;
        private string errorDataToday = "0";

        /// <summary>
        /// 易行线程
        /// </summary>
        public Thread GetYeeXingthread;
        private bool DataYeeXingOk = false;
        private string errorDataYeeXing = "0";

        //所有线程处理完DataOk变成false
        private bool DataOk = true;

        /// <summary>
        /// Order
        /// </summary>
        public PbProject.Model.Tb_Ticket_Order _order;

        /// <summary>
        /// 采购员（支付员）
        /// </summary>
        public PbProject.Model.User_Employees _mUser;

        /// <summary>
        /// 供应公司
        /// </summary>
        public PbProject.Model.User_Company _mTopcom;

        /// <summary>
        /// 采购公司（支付公司）
        /// </summary>
        public PbProject.Model.User_Company _mCom;

        //是否换编码出票
        private bool _changePnr = true;

        //总实体
        List<PbProject.Model.Tb_Ticket_Policy> _allPolicy ;

        /// <summary>
        /// 参数实例
        /// </summary>
        /// <param name="Order"></param>
        /// <param name="mUser">买家帐号</param>
        public AllInterface(PbProject.Model.Tb_Ticket_Order Order, PbProject.Model.User_Employees mUser)
        {
            _order = Order;
            _mUser = mUser;
            _changePnr = Order.AllowChangePNRFlag;
            string sqlWhere = " UninCode='" + _mUser.CpyNo.Substring(0, 12) + "'";
            List<PbProject.Model.User_Company> objList = Manage.CallMethod("User_Company", "GetList", null, new object[] { sqlWhere }) as List<PbProject.Model.User_Company>;
            if (objList != null && objList.Count > 0)
            {
                _mTopcom = objList[0];
            }
            //_mTopcom = objList[0];
            sqlWhere = " UninCode='" + _mUser.CpyNo + "'"; ;
            objList = Manage.CallMethod("User_Company", "GetList", null, new object[] { sqlWhere }) as List<PbProject.Model.User_Company>;
            if (objList != null && objList.Count > 0)
            {
                _mCom = objList[0];
            }
            _allPolicy = new List<Model.Tb_Ticket_Policy>();
            //_mCom = objList[0];
        }

        //Data517Ok && Data51bookOk && DataBaiTuoOk && DataPMOk && Data8000YOk && DataTodayOk
        #region 517
        /// <summary>
        /// 开始查询517线程
        /// </summary>
        private void StartGet517Thread()
        {
            Get517thread = new Thread(new ThreadStart(Get517Pol));
            Get517thread.Start();
        }

        public void Get517Pol()
        {
            PTBy517 na517 = new PTBy517(_order, _mUser,_mTopcom,_mCom);
            _allPolicy.AddRange(na517.GetPolicy(_changePnr));
            Data517Ok = true;
        }
        #endregion

        #region 百拓
        /// <summary>
        /// 开始查询百拓线程
        /// </summary>
        private void StartGetBaiTuoThread()
        {
            GetBaiTuothread = new Thread(new ThreadStart(GetBaiTuoPol));
            GetBaiTuothread.Start();
        }
        public void GetBaiTuoPol()
        {
            PbProject.Model.definitionParam.BaseSwitch BS = new PbProject.Model.definitionParam.BaseSwitch();
            
            List<PbProject.Model.Bd_Base_Parameters> mBP = new PbProject.Logic.ControlBase.BaseDataManage().
                 CallMethod("Bd_Base_Parameters", "GetList", null, new Object[] { "CpyNo='" + _order.OwnerCpyNo.Substring(0, 12) + "'" }) as List<PbProject.Model.Bd_Base_Parameters>;
            
            BS = PbProject.WebCommon.Utility.BaseParams.getParams(mBP);
            PTBybaituo baituo = new PTBybaituo(_order, BS);
            _allPolicy.AddRange(baituo.GetPolicy(_changePnr));
            DataBaiTuoOk = true;
        }
        #endregion

        #region 票盟
        /// <summary>
        /// 开始查询票盟线程
        /// </summary>
        private void StartGetPMThread()
        {
            GetPMthread = new Thread(new ThreadStart(GetPMPol));
            GetPMthread.Start();
        }
        public void GetPMPol()
        {
            PTByPiaomeng piaoMeng = new PTByPiaomeng(_order, _mUser, _mTopcom, _mCom);
            List<PbProject.Model.Tb_Ticket_Policy> sss = piaoMeng.GetPolicy(_changePnr);
            _allPolicy.AddRange(piaoMeng.GetPolicy(_changePnr));
            DataPMOk = true;
        }
        #endregion

        #region 51book
        /// <summary>
        /// 开始查询51book线程
        /// </summary>
        private void StartGet51BookThread()
        {
            Get51bookthread = new Thread(new ThreadStart(Get51BookPol));
            Get51bookthread.Start();
        }
        public void Get51BookPol()
        {
            PTBy51book book = new PTBy51book(_order, _mUser, _mTopcom, _mCom);
            _allPolicy.AddRange(book.GetPolicy(_changePnr));
            Data51bookOk = true;
        }
        #endregion

        #region 8000yi
        /// <summary>
        /// 开始查询8000yi线程
        /// </summary>
        private void StartGet8000YThread()
        {
            Get8000Ythread = new Thread(new ThreadStart(Get8000YPol));
            Get8000Ythread.Start();
        }
        public void Get8000YPol()
        {
            PTBy8000yi yi = new PTBy8000yi(_order, _mUser, _mTopcom, _mCom);
            _allPolicy.AddRange(yi.GetPolicy(_changePnr));
            Data8000YOk = true;
        }
        #endregion

        #region 今日
        /// <summary>
        /// 开始查询今日线程
        /// </summary>
        private void StartGetTodayThread()
        {
            GetTodaythread = new Thread(new ThreadStart(GetTodayPol));
            GetTodaythread.Start();
        }
        public void GetTodayPol()
        {
            PTByJinri jinri = new PTByJinri(_order, _mUser, _mTopcom, _mCom);
            _allPolicy.AddRange(jinri.GetPolicy());
            DataTodayOk = true;
        }
        #endregion


        #region 易行
        /// <summary>
        /// 开始查询易行线程
        /// </summary>
        private void StartGetYeeXingThread()
        {
            GetYeeXingthread = new Thread(new ThreadStart(GetYeeXingPol));
            GetYeeXingthread.Start();
        }
        public void GetYeeXingPol()
        {
            PTByYeeXing YeeXing = new PTByYeeXing(_order, _mUser, _mTopcom, _mCom);
            _allPolicy.AddRange(YeeXing.GetPolicy(_changePnr));
            DataYeeXingOk = true;
        }
        #endregion



        //判断是否在工作时间内和工作时间内判断是否开启
        private bool isInWorkTime(PbProject.Model.User_Company Company)
        {
            bool isOk = true;//默认读取接口政策
            //bool isOpen = false;//是否开启时间判断
            int time0 = 0;//上班时间
            int time1 = 0;//下班时间

            if (Company.WorkTime == null || Company.WorkTime == "")
            {
                return true;
            }
            string[] time = Company.WorkTime.Split('-');

            //isOpen = (string.IsNullOrEmpty(Company.A47) || Company.A47 == "1") ? true : false;
            try
            {
                time0 = int.Parse(time[0].Trim().Replace(":", "").Replace(":", ""));
                time1 = int.Parse(time[1].Trim().Replace(":", "").Replace(":", ""));
                int nowTime = DateTime.Now.Hour * 100 + DateTime.Now.Minute;
                if (nowTime >= time0 && nowTime <= time1)
                {
                    isOk = true;
                }
                else
                {
                    isOk = false;
                }
            }
            catch (Exception)
            {
                isOk = false;//有转换错误直接认为未开启,仅仅是此处逻辑判断,不会修改数据库
            }
            return isOk;
        }


        /// <summary>
        /// 已重载.计算两个日期的时间间隔,返回的是时间间隔的日期差的秒数.
        /// </summary>
        /// <param name="DateTime1">第一个日期和时间</param>
        /// <param name="DateTime2">第二个日期和时间</param>
        /// <returns></returns>
        private int DateDiff(DateTime DateTime1, DateTime DateTime2)
        {
            //string dateDiff=null;
            int rs = 0;
            try
            {
                TimeSpan ts1 = new TimeSpan(DateTime1.Ticks);
                TimeSpan ts2 = new TimeSpan(DateTime2.Ticks);
                TimeSpan ts = ts1.Subtract(ts2).Duration();

                rs = ts.Minutes * 60 + ts.Seconds;
                rs = Math.Abs(rs);
            }
            catch
            {
                rs = 10;//因为调用处是判断是否大于3秒,出问题就默认大于3秒,外面就可自动退出判断
            }
            return rs;
        }

        public List<PbProject.Model.Tb_Ticket_Policy> GetPolicyAll()
        {

            DateTime begintime = DateTime.Now;
            DateTime endtime = DateTime.Now;


            PbProject.Model.definitionParam.BaseSwitch BS = new Model.definitionParam.BaseSwitch();

            PbProject.Logic.ControlBase.Bd_Base_ParametersBLL Bp = new ControlBase.Bd_Base_ParametersBLL();
            List<PbProject.Model.Bd_Base_Parameters> ParList = Bp.GetParametersListByCpyNo(_mTopcom.UninCode.ToString());
            BS = WebCommon.Utility.BaseParams.getParams(ParList);

            if (isInWorkTime(_mTopcom))//判断是否在工作时间内和工作时间内判断是否开启
            {
                begintime = DateTime.Now;
                if (BS.KongZhiXiTong.Contains("|84|") && BS.yunYingQuanXian.Contains("|84|"))
                {
                    StartGet517Thread();
                }
                else
                {
                    Data517Ok = true;
                }
                if (BS.KongZhiXiTong.Contains("|89|") && BS.yunYingQuanXian.Contains("|89|"))
                {
                    StartGet51BookThread();
                }
                else
                {
                    Data51bookOk = true;
                }
                if (BS.KongZhiXiTong.Contains("|85|") && BS.yunYingQuanXian.Contains("|85|"))
                {
                    StartGetBaiTuoThread();
                }
                else
                {
                    DataBaiTuoOk = true;
                }
                if (BS.KongZhiXiTong.Contains("|88|") && BS.yunYingQuanXian.Contains("|88|"))
                {
                    StartGetPMThread();
                }
                else
                {
                    DataPMOk = true;
                }
                if (BS.KongZhiXiTong.Contains("|86|") && BS.yunYingQuanXian.Contains("|86|"))
                {
                    //  8000yi线程
                    StartGet8000YThread();
                }
                else
                {
                    Data8000YOk = true;
                }
                if (BS.KongZhiXiTong.Contains("|87|") && BS.yunYingQuanXian.Contains("|87|"))
                {
                    //  今日线程
                    StartGetTodayThread();
                }
                else
                {
                    DataTodayOk = true;
                }

                if (BS.KongZhiXiTong.Contains("|93|") && BS.yunYingQuanXian.Contains("|93|"))
                {
                    //  易行线程
                    StartGetYeeXingThread();
                }
                else
                {
                    DataYeeXingOk = true;
                }
                //else
                //{
                //    Data517Ok = true;
                //    Data51bookOk = true;
                //    DataBaiTuoOk = true;
                //    DataPMOk = true;
                //    Data8000YOk = true;
                //    DataTodayOk = true;
                //}

                while (DataOk)
                {
                    endtime = DateTime.Now;
                    if (DateDiff(endtime, begintime) > 200)
                    {
                        DataOk = false;
                        if (Get517thread != null)
                        {
                            if (Get517thread.IsAlive)
                            {
                                Get517thread.Abort();
                            }

                        }
                        if (Get51bookthread != null)
                        {
                            if (Get51bookthread.IsAlive)
                            {
                                Get51bookthread.Abort();
                            }

                        }
                        if (GetBaiTuothread != null)
                        {
                            if (GetBaiTuothread.IsAlive)
                            {
                                GetBaiTuothread.Abort();
                            }

                        }
                        if (GetPMthread != null)
                        {
                            if (GetPMthread.IsAlive)
                            {
                                GetPMthread.Abort();
                            }
                        }
                        //  8000yi
                        if (Get8000Ythread != null)
                        {
                            if (Get8000Ythread.IsAlive)
                                Get8000Ythread.Abort();
                        }
                        //  今日
                        if (GetTodaythread != null)
                        {
                            if (GetTodaythread.IsAlive)
                                GetTodaythread.Abort();
                        }
                        //  易行
                        if (GetYeeXingthread != null)
                        {
                            if (GetYeeXingthread.IsAlive)
                                GetYeeXingthread.Abort();
                        }
                    }
                    if (Data517Ok && Data51bookOk && DataBaiTuoOk && DataPMOk && Data8000YOk && DataTodayOk && DataYeeXingOk)
                    {
                        DataOk = false;
                    }
                }
            }
            return _allPolicy;
        }
    }
}

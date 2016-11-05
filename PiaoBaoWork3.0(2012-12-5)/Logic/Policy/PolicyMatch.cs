using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PbProject.Model;
using PbProject.Logic.ControlBase;
using DataBase.Data;
using System.Data;
using System.Threading;
using PbProject.Logic.Pay;
using System.Threading.Tasks;

namespace PbProject.Logic.Policy
{
    public class PolicyMatch
    {
        BaseDataManage Manage = new BaseDataManage();
        /// <summary>
        /// 获取接口政策
        /// </summary>
        /// <param name="CpyNo"></param>
        /// <param name="StartCityNameCode"></param>
        /// <param name="TargetCityNameCode"></param>
        /// <param name="TravelType"></param>
        /// <returns></returns>
        public List<Tb_Ticket_Policy> getInterFacePolicy(string OrderID, User_Employees mUser,
            bool IsOrGetPolicy, string fccode, string tccode, string FromDate, string TravType, bool IsINF)
        {
            List<Tb_Ticket_Policy> list = null;
            try
            {
                if (IsOrGetPolicy)
                {
                    string sqlwhere1 = " 1=1 ";
                    sqlwhere1 = sqlwhere1 + CityConvert(" and (FromCity like '%" + fccode.Trim().ToUpper() + "%'", fccode, "'%" + fccode.Trim().ToUpper() + "%'");//.Replace("like", "=").Replace("%", "");
                    if ("SHA,PVG,PEK,NAY".Contains(fccode.ToUpper()))
                        sqlwhere1 = sqlwhere1 + " or FromCity='ALL'))";
                    else
                        sqlwhere1 = sqlwhere1 + " or FromCity='ALL')";
                    //到达城市
                    sqlwhere1 = sqlwhere1 + CityConvert(" and (ToCity like '%" + tccode.Trim().ToUpper() + "%'", tccode, "'%" + tccode.Trim().ToUpper() + "%'");//.Replace("like", "=").Replace("%", "");

                    //  zym issue comment：如果存在多个机场替换就加双括号结尾，否则单括号结尾
                    if ("SHA,PVG,PEK,NAY".Contains(tccode.ToUpper()))
                        sqlwhere1 = sqlwhere1 + " or ToCity='ALL'))";
                    else
                        sqlwhere1 = sqlwhere1 + " or ToCity='ALL')";

                    //if (PolicySource == "5")//票盟要判断打票日期
                    //{
                    //    //打票日期
                    //    sqlwhere1 = sqlwhere1 + " and dbo.split(GTicketTimeE,'-',0)+'-'+ dbo.split(GTicketTimeE,'-',1)+'-'+ dbo.split(GTicketTimeE,'-',2) <= '" + FromDate + "' and '" + FromDate + "' <= dbo.split(GTicketTimeE,'-',3)+'-'+ dbo.split(GTicketTimeE,'-',4)+'-'+ dbo.split(GTicketTimeE,'-',5)";
                    //}
                    //乘机日期
                    sqlwhere1 = sqlwhere1 + " and EffectiveDate <= '" + FromDate + "' and '" + FromDate + "' <= ExpiryDate";
                    //已审政策
                    sqlwhere1 = sqlwhere1 + " and PolicyState=1";
                    if (TravType == "3")
                    {
                        TravType = "2";
                    }
                    sqlwhere1 = sqlwhere1 + " and TripType=" + TravType;
                    //51book政策
                    //_51bookSql = _51bookSql + " and PolicySource=" + PolicySource;
                    List<Tb_Ticket_BookPolicy> objList = Manage.CallMethod("Tb_Ticket_BookPolicy", "GetList", null, new object[] { sqlwhere1 }) as List<Tb_Ticket_BookPolicy>;
                    //objList[i].


                    PbProject.Model.User_Company mTopcom = new User_Company();
                    string sqlWhere = " UninCode='" + mUser.CpyNo.Substring(0, 12) + "'";
                    List<PbProject.Model.User_Company> mTopcomList = Manage.CallMethod("User_Company", "GetList", null, new object[] { sqlWhere }) as List<PbProject.Model.User_Company>;
                    if (mTopcomList != null && mTopcomList.Count > 0)
                    {
                        mTopcom = mTopcomList[0];
                    }
                    if (objList.Count > 0)
                    {
                        list = new List<Tb_Ticket_Policy>();
                    }

                    for (int i = 0; i < objList.Count; i++)
                    {
                        PbProject.Model.Tb_Ticket_Policy mPolicy = new Tb_Ticket_Policy();
                        bool getPolicyIsOk = false;
                        try
                        {
                            mPolicy.CpyNo = "0" + objList[i].PolicySource + mTopcom.UninCode;
                            mPolicy.PolicyKind = 0;
                            mPolicy.GenerationType = 1;
                            mPolicy.CarryCode = objList[i].Airlines;
                            if (objList[i].TripType == 1)
                            {
                                mPolicy.TravelType = 1;
                            }
                            else if (objList[i].TripType == 2)
                            {
                                mPolicy.TravelType = 3;
                            }
                            else
                            {
                                mPolicy.TravelType = 4;
                            }

                            if (objList[i].TicketType == 1)
                            {
                                mPolicy.PolicyType = 1;
                            }
                            else
                            {
                                mPolicy.PolicyType = 2;
                            }
                            mPolicy.TeamFlag = 0;
                            mPolicy.StartCityNameCode = objList[i].FromCity;
                            mPolicy.StartCityNameSame = 2;
                            mPolicy.TargetCityNameCode = objList[i].ToCity;
                            mPolicy.TargetCityNameSame = 2;
                            mPolicy.ApplianceFlight = objList[i].Flight;
                            mPolicy.ApplianceFlightType = 2;
                            if (objList[i].A1 == 0)
                            {
                                mPolicy.PolicyKind = 0;
                                mPolicy.GenerationType = 1;
                            }
                            else
                            {
                                mPolicy.PolicyKind = 2;
                                mPolicy.GenerationType = 2;
                            }



                            mPolicy.ScheduleConstraints = objList[i].EtcLimit;
                            mPolicy.ShippingSpace = objList[i].Shipping;

                            mPolicy.FlightStartDate = objList[i].EffectiveDate;
                            mPolicy.FlightEndDate = objList[i].ExpiryDate;
                            try
                            {
                                string GTtime1 = objList[i].GTicketTimeE.Substring(0, 19);
                                string GTtime2 = objList[i].GTicketTimeE.Substring(20, 19);
                                mPolicy.PrintStartDate = Convert.ToDateTime(GTtime1);
                                mPolicy.PrintEndDate = Convert.ToDateTime(GTtime2);
                            }
                            catch (Exception)
                            {
                                mPolicy.PrintStartDate = DateTime.Parse("1900-01-01 00:00:00");
                                mPolicy.PrintEndDate = DateTime.Parse("1900-01-01 00:00:01");
                            }


                            mPolicy.AuditDate = DateTime.Now;
                            mPolicy.AuditType = 1;
                            mPolicy.Remark = objList[i].Remark;
                            mPolicy.IsApplyToShareFlight = 0;
                            mPolicy.ShareAirCode = "";
                            mPolicy.IsLowerOpen = 0;
                            mPolicy.DownPoint = objList[i].PReturn * 100;
                            mPolicy.InterPolicyId = objList[i].PolicyId;
                            mPolicy._WorkTime = WorkTimeConvert(objList[i].ProviderWorkTime, mTopcom.WorkTime);
                            getPolicyIsOk = true;
                        }
                        catch (Exception)
                        {
                            getPolicyIsOk = false;
                        }
                        if (getPolicyIsOk)
                        {
                            list.Add(mPolicy);
                        }

                    }
                }
                else
                {
                    if (!IsINF)//有婴儿不享有接口订单
                    {
                        string sqlwhere = " 1=1 "
                                        + "and OrderId='" + OrderID + "'";
                        List<Tb_Ticket_Order> objList = Manage.CallMethod("Tb_Ticket_Order", "GetList", null, new object[] { sqlwhere }) as List<Tb_Ticket_Order>;
                        if (objList != null)
                        {
                            if (objList.Count > 0)
                            {
                                PbProject.Logic.PTInterface.AllInterface allInterface = new PTInterface.AllInterface(objList[0], mUser);
                                list = allInterface.GetPolicyAll();
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                list = null;
            }
            return list;
        }




        /// <summary>
        /// 获取匹配后的政策
        /// </summary>
        /// <param name="CpyNo">公司ID</param>
        /// <param name="StartCityNameCode">出发城市</param>
        /// <param name="MiddCityNameCode">中转城市</param>
        /// <param name="TargetCityNameCode">到达城市</param>
        /// <param name="FromDate">出发时间</param>
        /// <param name="ReturnDate">到达时间</param>
        /// <param name="TravelType">行程类型</param>
        /// <param name="chaceNameByGUID">缓存ID</param>
        /// <param name="IsOrGetPolicy">白屏还是获取政策(true白false获取)</param>
        /// <param name="GroupId">扣点分组ID</param>
        /// <param name="OrderID">订单ID</param>
        /// <param name="mUser">登录账号信息</param>
        /// <param name="HavaChild">是否有儿童(true有,false没有)</param>
        /// <param name="IsINF">是否有嬰兒(true有,false没有)</param>
        /// <returns></returns>
        public DataSet getMatchingPolicy(string CpyNo, string StartCityNameCode, string MiddCityNameCode, string TargetCityNameCode,
            string FromDate, string ReturnDate,
            string TravelType, string chaceNameByGUID, bool IsOrGetPolicy, string GroupId,
            string OrderID, User_Employees mUser, bool HavaChild, bool IsINF)
        {
            return getMatchingPolicy(CpyNo, StartCityNameCode, MiddCityNameCode, TargetCityNameCode, FromDate, ReturnDate, TravelType, chaceNameByGUID, IsOrGetPolicy, GroupId, OrderID, mUser, HavaChild, IsINF, true);
        }


        /// <summary>
        /// 获取匹配后的政策
        /// </summary>
        /// <param name="CpyNo">公司ID</param>
        /// <param name="StartCityNameCode">出发城市</param>
        /// <param name="MiddCityNameCode">中转城市</param>
        /// <param name="TargetCityNameCode">到达城市</param>
        /// <param name="FromDate">出发时间</param>
        /// <param name="ReturnDate">到达时间</param>
        /// <param name="TravelType">行程类型</param>
        /// <param name="chaceNameByGUID">缓存ID</param>
        /// <param name="IsOrGetPolicy">白屏还是获取政策(true白false获取)</param>
        /// <param name="GroupId">扣点分组ID</param>
        /// <param name="OrderID">订单ID</param>
        /// <param name="mUser">登录账号信息</param>
        /// <param name="HavaChild">是否有儿童(true有,false没有)</param>
        /// <param name="IsINF">是否有嬰兒(true有,false没有)</param>
        /// <param name="isGetShare">是否取共享(对外接口不取共享政策)</param>
        /// <returns></returns>
        public DataSet getMatchingPolicy(string CpyNo, string StartCityNameCode, string MiddCityNameCode, string TargetCityNameCode,
            string FromDate, string ReturnDate,
            string TravelType, string chaceNameByGUID, bool IsOrGetPolicy, string GroupId,
            string OrderID, User_Employees mUser, bool HavaChild, bool IsINF, bool isGetShare)
        {
            List<Tb_Ticket_Policy> listLocalPolicy = null;
            List<Tb_Ticket_Policy> listLocalPolicyDefault = null;
            List<Tb_Ticket_Policy> listSharePolicy = null;
            List<Tb_Ticket_Policy> listInterFacePolicy = null;
            List<Tb_Ticket_Policy> listAirB2BPolicy = null;

            #region b2b参数
            string Pnr = string.Empty, BigPnr = string.Empty, CarryCode = string.Empty, Space = string.Empty, Office = string.Empty, Msg = string.Empty;
            bool IsValidate = false;
            Pnr = System.Web.HttpContext.Current.Request["Pnr"] != null ? System.Web.HttpContext.Current.Request["Pnr"].ToString() : "";
            BigPnr = System.Web.HttpContext.Current.Request["BigPnr"] != null ? System.Web.HttpContext.Current.Request["BigPnr"].ToString() : "";
            CarryCode = System.Web.HttpContext.Current.Request["CarrayCode"] != null ? System.Web.HttpContext.Current.Request["CarrayCode"].ToString() : "";
            Office = System.Web.HttpContext.Current.Request["Office"] != null ? System.Web.HttpContext.Current.Request["Office"].ToString() : "";
            Space = System.Web.HttpContext.Current.Request["Space"] != null ? System.Web.HttpContext.Current.Request["Space"].ToString() : "";
            PolicyParamEx B2BAirParam = null;
            if (!string.IsNullOrEmpty(Pnr) && !string.IsNullOrEmpty(BigPnr) && !string.IsNullOrEmpty(CarryCode))
            {
                IsValidate = true;
                B2BAirParam = new PolicyParamEx();
                B2BAirParam.CpCpyNo = CpyNo;
                B2BAirParam.Pnr = Pnr;
                B2BAirParam.BigPnr = BigPnr;
                B2BAirParam.CarryCode = CarryCode;
                B2BAirParam.TravelType = TravelType;
                B2BAirParam.Office = Office;
                B2BAirParam.StartCityNameCode = StartCityNameCode;
                B2BAirParam.MiddleCityNameCode = MiddCityNameCode;
                B2BAirParam.TargetCityNameCode = TargetCityNameCode;
                B2BAirParam.Space = Space;
            }
            #endregion


            if (!HavaChild)//成人政策数据
            {
                //获取数据并行计算
                Parallel.Invoke(
                    () => listLocalPolicy = new LocalPolicy().getLocalPolicy(CpyNo, StartCityNameCode, MiddCityNameCode, TargetCityNameCode, FromDate, ReturnDate, TravelType),
                    () => listLocalPolicyDefault = new LocalPolicy().getLocalPolicyDefault(CpyNo),
                    () =>
                    {
                        if (isGetShare)
                        {
                            listSharePolicy = new LocalPolicy().getSharePolicy(CpyNo, StartCityNameCode, MiddCityNameCode, TargetCityNameCode, FromDate, ReturnDate, TravelType, IsINF);
                        }
                    },
                    () => listInterFacePolicy = getInterFacePolicy(OrderID, mUser, IsOrGetPolicy, StartCityNameCode, TargetCityNameCode, FromDate, TravelType, IsINF),
                    () =>
                    {
                        if (!IsOrGetPolicy && IsValidate)
                        {
                            listAirB2BPolicy = new AirB2BPolicy().getAirB2BPolicy(B2BAirParam, out Msg);
                        }
                    }
             );
                orderPolicy(listLocalPolicy, 20);//同样点数,本地永远默认最高等级
                orderPolicy(listLocalPolicyDefault, 20);//同样点数,本地永远默认最高等级
                orderPolicy(listSharePolicy, 19);//同样点数,本地永远默认最高等级


                ////获取本地政策(普通)
                //listLocalPolicy = new LocalPolicy().getLocalPolicy(CpyNo, StartCityNameCode, MiddCityNameCode, TargetCityNameCode, FromDate, ReturnDate, TravelType);
                //orderPolicy(listLocalPolicy, 20);//同样点数,本地永远默认最高等级
                ////获取本地政策(成人默认)
                //listLocalPolicyDefault = new LocalPolicy().getLocalPolicyDefault(CpyNo);
                //orderPolicy(listLocalPolicyDefault, 20);//同样点数,本地永远默认最高等级
                //if (isGetShare)
                //{
                //    //获取共享政策
                //    listSharePolicy = new LocalPolicy().getSharePolicy(CpyNo, StartCityNameCode, MiddCityNameCode, TargetCityNameCode, FromDate, ReturnDate, TravelType, IsINF);
                //    orderPolicy(listSharePolicy, 19);//同样点数,本地永远默认最高等级
                //}
                ////获取接口政策
                //listInterFacePolicy = getInterFacePolicy(OrderID, mUser, IsOrGetPolicy, StartCityNameCode, TargetCityNameCode, FromDate, TravelType, IsINF);

                if (listSharePolicy != null)
                {
                    //接口排序暂缓
                    //共享政策的共享点数赋值于下级分销返点,页面统一显示下级分销返点
                    for (int i = 0; i < listSharePolicy.Count; i++)
                    {
                        listSharePolicy[i].DownPoint = listSharePolicy[i].SharePoint;
                        listSharePolicy[i].DownReturnMoney = listSharePolicy[i].SharePointReturnMoney;
                    }
                }
                //共享政策下级分销返点取共享政策返点
                //组合政策
                CombinationPolicy(ref listLocalPolicy, listInterFacePolicy);
                CombinationPolicy(ref listLocalPolicy, listLocalPolicyDefault);
                CombinationPolicy(ref listLocalPolicy, listSharePolicy);
                //组合航空公司B2B政策
                if (listAirB2BPolicy != null && listAirB2BPolicy.Count > 0)
                {
                    CombinationPolicy(ref listLocalPolicy, listAirB2BPolicy);
                }
            }
            else
            {
                //获取儿童默认政策,当前儿童政策只能默认,直接复制给listLocalPolicy即可,不用再组合
                listLocalPolicy = new LocalPolicy().getLocalPolicyDefaultChild(CpyNo);
            }
            if (!HavaChild)//成人才扣点和补点
            {
                //本地政策补点
                LocalPatchCalculate(listLocalPolicy, CpyNo, StartCityNameCode, TargetCityNameCode, GroupId);
            }
            //查询当前政策里,非接口政策的公司数据,用以获取工作业务时间
            List<User_Company> listUser_Company = getWorkTimeInNowPolicy(listLocalPolicy);
            //记录原始政策现返以及时间 ,所有扣点操作必须写在这个后面
            for (int i = 0; i < listLocalPolicy.Count; i++)
            {
                listLocalPolicy[i]._OldPolicyPoint = listLocalPolicy[i].DownPoint.ToString();
                listLocalPolicy[i]._OldreturnMoney = listLocalPolicy[i].DownReturnMoney.ToString();
                listLocalPolicy[i]._PolicyPoint = listLocalPolicy[i].DownPoint.ToString();
                listLocalPolicy[i]._returnMoney = listLocalPolicy[i].DownReturnMoney.ToString();



                //获取当前政策的对应公司的所有工作退费票时间,接口的时间政策里面已经赋值
                string time = WorkTimeOrBusinessTime(listLocalPolicy[i].CpyNo, listUser_Company);
                if (time != "")
                {
                    listLocalPolicy[i]._WorkTime = time.Split(',')[0];
                    listLocalPolicy[i]._PolicyCancelTime = time.Split(',')[1];
                    listLocalPolicy[i]._PolicyReturnTime = time.Split(',')[1];
                    listLocalPolicy[i]._FPGQTime = time.Split(',')[1];
                }
            }
            if (!HavaChild)//成人才扣点和补点
            {
                //扣点计算
                TakeOffCalculate(listLocalPolicy, CpyNo, StartCityNameCode, TargetCityNameCode, GroupId);
                //补点计算
                PatchCalculate(listLocalPolicy, CpyNo, StartCityNameCode, TargetCityNameCode);
            }

            //政策排序
            //按利润(政策+现返排序),因为政策没有舱位价,默认给一个10000,只是用于排序
            var q =
            from p in listLocalPolicy
            orderby p.DownPoint * 10000 + p.DownReturnMoney descending, p._orderByPolicy descending
            select p;
            listLocalPolicy = q.ToList<Tb_Ticket_Policy>();
            //获取缓存中的基础航班数据
            PbProject.WebCommon.Utility.Cache.CacheByNet pwucc = new WebCommon.Utility.Cache.CacheByNet();
            DataSet dsCacheData = pwucc.GetCacheData(chaceNameByGUID);
            DataSet dsMatchdPolicy = new DataSet();//匹配后的数据
            if (dsCacheData == null)
            {
                dsMatchdPolicy = null;
            }
            else
            {
                if (dsCacheData.Tables.Count == 0)
                {
                    dsMatchdPolicy = null;
                }
                else
                {
                    bool isUnite = false;
                    if ((TravelType == "3" || TravelType == "2") && !IsOrGetPolicy)
                    {
                        isUnite = true;
                    }
                    dsMatchdPolicy = MatchingPolicy(dsCacheData, listLocalPolicy, CpyNo, IsOrGetPolicy, HavaChild, isUnite);
                }
            }
            return dsMatchdPolicy;
        }
        /// <summary>
        /// 匹配政策数据
        /// </summary>
        /// <param name="dsCacheData">基础数据</param>
        /// <param name="dsPolicy">原始政策数据</param>
        /// <param name="CpyNo">当前登录账号公司ID</param>
        /// <param name="IsOrGetPolicy">是白屏预订还是获取政策(白屏多航线取一条,获取单航线取5条)</param>
        /// <param name="HavaChild">是否有儿童(true是,fals否)</param>
        /// <param name="isUnite">是否联乘往返(true是,false否)(只对在获取页面有效,白屏不管是否联成,都直接标记为false即可)</param>
        /// <returns></returns>
        private DataSet MatchingPolicy(DataSet dsCacheData, List<Tb_Ticket_Policy> listLocalPolicy,
            string CpyNo, bool IsOrGetPolicy, bool HavaChild, bool isUnite)
        {
            DataSet dsMatchdPolicy = new DataSet();//匹配后的数据

            int dsCacheDataTablesCount = dsCacheData.Tables.Count;

            int policyCountALL = 0;//满5条政策的计数器(只针对获取政策页面使用)
            int UniteFirst = 0;//联乘时需要判断两乘是否都已经匹配到合适政策,主要是因为两乘舱位可能不一样
            bool haveLocalPolicy = false;//是否有本地政策,false为没有(业务需要,即使比接口政策的低,政策里也必须有一条本地的政)
            string cachePolicyInfo = ",";//往返联成存储第一程已经匹配到的政策
            //获取本地政策保护的数据
            string protectSqlwhere = " 1=1 and CpyNo='" + CpyNo.Substring(0, 12) + "' and state=1";
            List<Tb_Policy_Protect> protectList = Manage.CallMethod("Tb_Policy_Protect", "GetList", null, new object[] { protectSqlwhere }) as List<Tb_Policy_Protect>;
            //获取控制的参数信息
            string paramsStr = " 1=1 and CpyNo='" + CpyNo.Substring(0, 12) + "'";
            List<Bd_Base_Parameters> paramsList = Manage.CallMethod("Bd_Base_Parameters", "GetList", null, new object[] { paramsStr }) as List<Bd_Base_Parameters>;
            PbProject.Model.definitionParam.BaseSwitch BS = PbProject.WebCommon.Utility.BaseParams.getParams(paramsList);

            //循环以承运人+航班号(如:3U8881)为表名的航班信息
            for (int i = 0; i < dsCacheDataTablesCount; i++)
            {
                dsMatchdPolicy.Tables.Add(createMatchPolicyTable(dsCacheData.Tables[i].TableName));
                int dsCacheDataTablesiRowsCount = dsCacheData.Tables[i].Rows.Count;
                //循环对应的具体的舱位 
                for (int j = 0; j < dsCacheDataTablesiRowsCount; j++)
                {
                    int policyCount = 0;//单条航线政策条数计数器(一条航线的一个舱位满5条)
                    int listLocalPolicyCount = listLocalPolicy.Count;
                    int noPolicy = 0;//是否没有匹配到政策  
                    //循环政策
                    for (int k = 0; k < listLocalPolicyCount; k++)
                    {
                        #region 排除不符合条件的政策
                        //如果是联乘且航班信息为第二乘的且第一乘匹配失败,则不匹配政策
                        if (isUnite && i == 1 && UniteFirst == 0)
                        {
                            continue;
                        }
                        //特价政策筛选
                        //当前舱位是特价的,但是政策不是特价政策,则剔除
                        if (dsCacheData.Tables[i].Rows[j]["SpecialType"].ToString() == "1")
                        {
                            //非通用非特价则不匹配
                            if (listLocalPolicy[k].PolicyKind != 0 && listLocalPolicy[k].PolicyKind != 2)
                            {
                                continue;
                            }
                        }
                        //共享航班筛选(注意:共享航班和共享政策是完全不同的概念)
                        //当前舱位是共享航班的,但是政策不是匹配共享政策,则剔除
                        if (dsCacheData.Tables[i].Rows[j]["FlightNo"].ToString().Contains("*"))
                        {
                            //不适用共享航班,则不匹配
                            if (listLocalPolicy[k].IsApplyToShareFlight == 0)
                            {
                                continue;
                            }
                        }


                        //承运人(是否可以选ALL?)
                        if (!listLocalPolicy[k].CarryCode.ToString().Contains(dsCacheData.Tables[i].Rows[j]["CarrCode"].ToString())
                           && !listLocalPolicy[k].CarryCode.ToString().Contains("ALL"))
                        {
                            continue;
                        }
                        //舱位(是否可以选ALL?)
                        if (!listLocalPolicy[k].ShippingSpace.ToString().Contains(dsCacheData.Tables[i].Rows[j]["Space"].ToString())
                            && !listLocalPolicy[k].ShippingSpace.ToString().Contains("ALL"))
                        {
                            continue;
                        }
                        //政策来源判定
                        int policySource = 0;
                        policySource = getPolicySource(listLocalPolicy, CpyNo, k);
                        if (policySource == 0)
                        {
                            continue;
                        }
                        //本地政策保护限制
                        //非本地政策的,如果再筛选条件里,则不获取
                        if (policySource != 1 && policySource != 2)
                        {
                            if (protectedPolicy(protectList, dsCacheData.Tables[i].Rows[j]["CarrCode"].ToString(),
                                dsCacheData.Tables[i].Rows[j]["StartCityCode"].ToString(),
                                dsCacheData.Tables[i].Rows[j]["ToCityCode"].ToString())
                                )
                            {
                                continue;
                            }
                        }
                        //共享政策 超过晚上8点到次日9点 过滤
                        if (policySource == 9)
                        {
                            if (DateTime.Now.Hour >= 20 || DateTime.Now.Hour <= 9)
                            {
                                continue;
                            }
                        }

                        //航班限制判断
                        if (listLocalPolicy[k].ApplianceFlightType != 1)//全部适用无须判断
                        {

                            if (listLocalPolicy[k].ApplianceFlightType == 2)
                            {
                                if (!CompareFlight(listLocalPolicy[k].ApplianceFlight,
                                   "",
                                    dsCacheData.Tables[i].Rows[j]["FlightNo"].ToString(),
                                    policySource))
                                {
                                    continue;
                                }
                            }
                            if (listLocalPolicy[k].ApplianceFlightType == 3)
                            {
                                if (!CompareFlight("",
                                    listLocalPolicy[k].UnApplianceFlight,
                                    dsCacheData.Tables[i].Rows[j]["FlightNo"].ToString(),
                                    policySource))
                                {
                                    continue;
                                }
                            }
                        }

                        //班期限制判断
                        if (!CompareSchedule(dsCacheData.Tables[i].Rows[j]["StartDate"].ToString(),
                               listLocalPolicy[k].ScheduleConstraints,
                               policySource))
                        {
                            continue;
                        }
                        //供应工作时间判断
                        string workTime = "";

                        workTime = listLocalPolicy[k]._WorkTime;
                        if (workTime == "" || workTime == "-")
                        {
                            workTime = "00:00-23:59";
                        }
                        if (!IsOkTime(workTime))
                        {
                            continue;
                        }
                        //离起飞时间少于两小时的不获取接口政策
                        if (policySource != 1 && policySource != 2 && policySource != 9)
                        {
                            try
                            {
                                string flyTime = dsCacheData.Tables[i].Rows[j]["StartDate"].ToString() + " " + dsCacheData.Tables[i].Rows[j]["StartTime"].ToString();
                                if (flyTime != "")
                                {
                                    DateTime dtFlyTime = DateTime.Parse("2100-01-01 01:01:01");
                                    dtFlyTime = DateTime.Parse(flyTime);
                                    DateTime dtNowTime = DateTime.Now;
                                    TimeSpan ts = dtFlyTime.Subtract(dtNowTime);
                                    if (ts.Hours <= 1 && ts.Days == 0)
                                    {
                                        continue;
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                //OnError("离起飞时间少于两小时的不获取接口政策报错:" + ex.Message, "ThanPol");
                            }
                        }
                        //如果 是获取政策并且已经获取了4条政策,并且还没有本地政策,并且当前这条仍然不是本地政策,则不匹配
                        if (!IsOrGetPolicy && policyCountALL == 4 && !haveLocalPolicy && !(policySource == 1 || policySource == 2))
                        {
                            continue;
                        }



                        #endregion
                        //如果是联乘往返且航班信息为第一乘的,则不匹配政策,只是直接标注第一乘成功匹配到政策
                        //因为这样会匹配出两条航段的政策,政策显示时就会有两条
                        if (isUnite && i == 0)
                        {

                            cachePolicyInfo += listLocalPolicy[k].id.ToString() + ",";
                            //dsCacheData.Tables[i].Rows[j]["GUID"].ToString()//航班GUID
                            //listLocalPolicy[k].id.ToString()//政策GUID
                            UniteFirst = 1;
                            continue;
                        }
                        //如果是往返联成的,第二程匹配到了政策,还的确定第一程的政策是否匹配上,都匹配上,才可以
                        if (isUnite && i == 1)
                        {

                            bool cachePolicyInfoTrue = cachePolicyInfo.Contains("," + listLocalPolicy[k].id.ToString() + ",");
                            if (!cachePolicyInfoTrue)
                            {
                                continue;
                            }

                        }


                        noPolicy++;
                        //如果选取过本地政策,则标注本地政策为已经添加
                        if ((policySource == 1 || policySource == 2))
                        {
                            haveLocalPolicy = true;
                        }


                        DataRow dr = dsMatchdPolicy.Tables[i].NewRow();
                        string plo = "";//政策
                        string commission = "";//佣金
                        string xf = "";//现返

                        string tempXSFee = "";
                        string tempABFee = "";
                        string tempFuelAdultFee = "";
                        string tempDiscountRate = "";

                        string MsgErr = "";
                        //白屏查询直接用IBE舱位价格的数据
                        tempXSFee = dsCacheData.Tables[i].Rows[j]["XSFee"].ToString();
                        tempABFee = dsCacheData.Tables[i].Rows[j]["ABFee"].ToString();
                        tempFuelAdultFee = dsCacheData.Tables[i].Rows[j]["FuelAdultFee"].ToString();
                        tempDiscountRate = dsCacheData.Tables[i].Rows[j]["DiscountRate"].ToString();

                        //如果是固定特价需要得到特价舱位价
                        if (listLocalPolicy[k].GenerationType == 3)
                        {
                            tempXSFee = listLocalPolicy[k].SpacePrice.ToString("F2");
                            tempDiscountRate = "-1";//固定特价
                        }
                        else
                        {
                            if (!IsOrGetPolicy)//获取政策时需要根据高开低开从PAT内容中获取高价低价 
                            {
                                //PAT内容
                                string PatContent = dsCacheData.Tables[i].Rows[j]["PatContent"].ToString();
                                //儿童默认政策
                                if (listLocalPolicy[k].A1 == 2)
                                {
                                    //儿童PAT内容
                                    PatContent = dsCacheData.Tables[i].Rows[j]["CHDPatContent"].ToString();
                                }

                                //Y舱价格
                                string YFareFee = dsCacheData.Tables[i].Rows[j]["FareFee"].ToString();
                                decimal _YFareFee = 0m;
                                decimal.TryParse(YFareFee, out _YFareFee);
                                //是否儿童出成人票 true 是 现在只是白屏预订编码有
                                bool IsChdToAdultTK = System.Web.HttpContext.Current.Request["IsChdToAdultTK"] != null && System.Web.HttpContext.Current.Request["IsChdToAdultTK"].ToString() == "1";
                                PnrAnalysis.FormatPNR pnrFormat = new PnrAnalysis.FormatPNR();
                                //解析出来的PAT价格列表
                                PnrAnalysis.PatModel pat = pnrFormat.GetPATInfo(PatContent, out MsgErr);
                                PnrAnalysis.PatInfo price = null;
                                //编码中有多个价格进行处理
                                if (pat != null && pat.UninuePatList.Count > 0)
                                {
                                    //子舱位是否取低价
                                    if (BS.KongZhiXiTong.Contains("72") && dsCacheData.Tables[i].Rows[j]["Space"].ToString().Length == 2 && pat.ChildPat != null)
                                    {
                                        price = pat.ChildPat;
                                    }
                                    else//其他舱位
                                    {
                                        #region 取高价还是低价
                                        bool LowerOrHight = false;//取高或低价格,默认高价
                                        //本地和共享取高低价依据政策中设置
                                        if (policySource == 1 || policySource == 2 || policySource == 9)
                                        {
                                            //从PAT内容解析并根据政策判断取高价还是低价
                                            if (listLocalPolicy[k].IsLowerOpen == 1)//低开
                                            {
                                                LowerOrHight = true;
                                            }
                                            else//高开
                                            {
                                                LowerOrHight = false;

                                            }
                                        }
                                        else
                                        {
                                            //接口的开关开启了取低价
                                            if (BS.KongZhiXiTong.Contains("60"))
                                            {
                                                LowerOrHight = true;
                                            }
                                            else//反之取高价
                                            {
                                                LowerOrHight = false;
                                            }
                                        }
                                        if (LowerOrHight)
                                        {
                                            price = pat.UninuePatList[0];
                                        }
                                        else
                                        {
                                            price = pat.UninuePatList[pat.UninuePatList.Count - 1];
                                        }
                                        #endregion
                                    }
                                    tempXSFee = price.Fare.ToString();
                                    tempABFee = price.TAX.ToString();
                                    tempFuelAdultFee = price.RQFare.ToString();
                                    tempDiscountRate = _YFareFee != 0m ? PnrAnalysis.FormatPNR.GetZk(YFareFee, tempXSFee).ToString() : "0";

                                    //-------------邓积远2013-4-24添加--------------------
                                    //如果为儿童默认政策 且是儿童出成人票 修改基建和燃油
                                    if (IsChdToAdultTK && listLocalPolicy[k].A1 == 2)
                                    {
                                        //儿童出成人票时儿童PAT内容
                                        PatContent = dsCacheData.Tables[i].Rows[j]["ChdToAdultPatCon"].ToString();
                                        pat = pnrFormat.GetPATInfo(PatContent, out MsgErr);
                                        if (pat != null && pat.UninuePatList.Count > 0)
                                        {
                                            //过滤规则：1 G5 BK 东航集团(MU\FM\KN)的 都跟成人一样购买机建和燃油。2 其他都是没得机建费，燃油费为儿童PAT出来的燃油。
                                            //航空公司二字码
                                            string tempCarrCode = "|G5|BK|MU|FM|KN|";
                                            string strCarrCode = dsCacheData.Tables[i].Rows[j]["CarrCode"].ToString().ToUpper();
                                            if (!tempCarrCode.Contains("|" + strCarrCode + "|"))
                                            {
                                                //修改基建和燃油
                                                tempABFee = "0";
                                                tempFuelAdultFee = pat.UninuePatList[0].RQFare;
                                            }
                                        }
                                    }
                                    //------------邓积远2013-4-24---------------------
                                }
                                else
                                {
                                    //-------------邓积远2013-4-27修改--------------------
                                    //如果为儿童政策，不是儿童出成人票且没有PAT出价格 儿童为Y舱价格的一半
                                    if (listLocalPolicy[k].A1 == 2 && !IsChdToAdultTK)
                                    {
                                        DataAction d = new DataAction();
                                        tempXSFee = (d.FourToFiveNum((0.5m * _YFareFee) / 10, 0) * 10).ToString();
                                        tempDiscountRate = "50";
                                    }
                                    //-------------邓积远2013-4-27--------------------
                                }
                            }
                        }




                        plo = listLocalPolicy[k].DownPoint.ToString("F2");
                        commission = new PbProject.Logic.Pay.Data(CpyNo).CreateCommissionCG(decimal.Parse(tempXSFee), decimal.Parse(plo)).ToString("F2");
                        xf = listLocalPolicy[k].DownReturnMoney * 1 == 0 ? "" : listLocalPolicy[k].DownReturnMoney.ToString();
                        decimal sj = new PbProject.Logic.Pay.Data(CpyNo).CreatePassengerPayFee(decimal.Parse(tempXSFee),
                            decimal.Parse(tempABFee),
                            decimal.Parse(tempFuelAdultFee),
                            decimal.Parse(plo),
                            listLocalPolicy[k].DownReturnMoney * -1,
                            1);
                        decimal tempXF = 0m;
                        if (xf != "")
                        {
                            decimal.TryParse(xf, out tempXF);
                            if (tempXF > 0)
                            {
                                xf = "+" + tempXF.ToString("f2");
                            }
                        }


                        //实际支付价格=票面价-先返-佣金
                        //= Math.Round(decimal.Parse(dsCacheData.Tables[i].Rows[j]["XSFee"].ToString()) * (1 - (decimal.Parse(plo)/100)), 2) - decimal.Parse(dsCacheData.Tables[i].Rows[j]["ReturnMoney"].ToString());
                        //sj += Math.Round(decimal.Parse(dsCacheData.Tables[i].Rows[j]["ABFee"].ToString()) + decimal.Parse(dsCacheData.Tables[i].Rows[j]["FuelAdultFee"].ToString()), 2);
                        dr["GUID"] = dsCacheData.Tables[i].Rows[j]["GUID"].ToString();
                        dr["GenerationType"] = listLocalPolicy[k].GenerationType;
                        //dr["SpacePrice"] = spacePriceTemp;
                        dr["Policy"] = plo;
                        dr["ReturnMoney"] = xf;
                        dr["Commission"] = (commission != "" ? decimal.Parse(commission) + tempXF : tempXF).ToString("f2");
                        dr["SJFee"] = sj.ToString("f2");


                        dr["CarryCode"] = dsCacheData.Tables[i].Rows[j]["CarrCode"].ToString();//承运人
                        string drCpyNo = listLocalPolicy[k].CpyNo;
                        if (listLocalPolicy[k].CpyNo.Length == 14)
                        {
                            try
                            {
                                drCpyNo = listLocalPolicy[k].CpyNo.Substring(2, 12);
                            }
                            catch (Exception)
                            {
                                drCpyNo = "";
                            }
                        }

                        dr["CpyNo"] = drCpyNo;//发布政策公司编号

                        dr["Space"] = dsCacheData.Tables[i].Rows[j]["Space"].ToString();//舱位
                        dr["DiscountRate"] = tempDiscountRate;//折扣
                        //本地政策用的自带的GUID,接口的单独用InterPolicyId
                        if (policySource == 1 || policySource == 2 || policySource == 9)
                        {
                            if (listLocalPolicy[k].A13 == "1")
                            {
                                dr["PolicyId"] = "b2bpolicy|" + listLocalPolicy[k]._AirPayMoney;
                            }
                            else
                            {
                                dr["PolicyId"] = listLocalPolicy[k].id;
                            }
                        }
                        else
                        {
                            dr["PolicyId"] = listLocalPolicy[k].InterPolicyId;//政策ID
                        }
                        dr["AirPoint"] = listLocalPolicy[k].AirReBate;//航空公司返点
                        dr["AirReturnMoney"] = listLocalPolicy[k].AirReBateReturnMoney;//航空公司现返

                        dr["PolicyPoint"] = listLocalPolicy[k]._PolicyPoint;//出票政策
                        dr["ReturnMoney2"] = listLocalPolicy[k]._returnMoney;//出票现返

                        dr["OldPolicyPoint"] = listLocalPolicy[k]._OldPolicyPoint;//原始政策
                        dr["OldReturnMoney"] = listLocalPolicy[k]._OldreturnMoney;//原始现返
                        if (policySource == 1 || policySource == 2)//本地政策才记录后返,共享和接口直接赋值为0
                        {
                            dr["LaterPoint"] = listLocalPolicy[k].LaterPoint;//后返      
                        }
                        else
                        {
                            dr["LaterPoint"] = 0;//后返    
                        }
                        dr["SeatPrice"] = tempXSFee;
                        dr["ABFare"] = tempABFee;
                        dr["RQFare"] = tempFuelAdultFee;
                        dr["DiscountDetail"] = listLocalPolicy[k]._DiscountDetail;
                        dr["PolicyRemark"] = listLocalPolicy[k].Remark;
                        dr["PolicyType"] = listLocalPolicy[k].PolicyType;
                        dr["PolicyKind"] = listLocalPolicy[k].PolicyKind;
                        dr["AutoPrintFlag"] = listLocalPolicy[k].AutoPrintFlag;
                        dr["PolicySource"] = policySource.ToString();
                        dr["PolicyOffice"] = listLocalPolicy[k].Office;
                        dr["DefaultType"] = listLocalPolicy[k].A1;//
                        dr["HighPolicyFlag"] = listLocalPolicy[k].HighPolicyFlag;//高返
                        dr["WorkTime"] = listLocalPolicy[k]._WorkTime;//正常上班时间00:00-00:00
                        dr["PolicyCancelTime"] = listLocalPolicy[k]._PolicyCancelTime;//废票时间
                        dr["PolicyReturnTime"] = listLocalPolicy[k]._PolicyReturnTime;//退票时间
                        dr["FPGQTime"] = listLocalPolicy[k]._FPGQTime;//废票改签时间 00:00-00:00
                        dr["ChuPiaoShiJian"] = BS.chuPiaoShiJian;//出票时间
                        dr["PatchPonit"] = listLocalPolicy[k]._patchPonit;//补点值
                        dr["JinriGYCode"] = listLocalPolicy[k].A16;//今日供应商ID
                        dr["AirPayMoney"] = listLocalPolicy[k]._AirPayMoney;//B2B航空公司政策支付金额                    
                        dsMatchdPolicy.Tables[i].Rows.Add(dr);
                        //如果是白屏查询,则匹配一条最高的
                        //如果是选择政策界面,则匹配政策的前5条
                        policyCount++;
                        policyCountALL++;
                        if (IsOrGetPolicy && policyCount == 1 && !HavaChild)//如果是 白屏查询且有一条政策且非儿童,则匹配结束退出
                        {
                            break;
                        }
                        if (!IsOrGetPolicy && policyCountALL == 1 && HavaChild)//如果是 获取政策页面且满1条政策且是儿童政策,则匹配结束退出
                        {
                            break;
                        }

                        if (!IsOrGetPolicy && policyCountALL == 5 && !HavaChild)//如果是 获取政策页面且满5条政策且非儿童,则匹配结束退出
                        {
                            break;
                        }
                    }
                }
            }

            return dsMatchdPolicy;
        }
        /// <summary>
        /// 获取当前政策里面的政策来源
        /// </summary>
        /// <param name="listLocalPolicy">政策集合</param>
        /// <param name="CpyNo">当前登录的ID</param>
        /// <param name="k">索引</param>
        /// <returns></returns>
        private int getPolicySource(List<Tb_Ticket_Policy> listLocalPolicy, string CpyNo, int k)
        {

            int policySource = 0;
            if (listLocalPolicy[k].CpyNo.Length == 12)//首先判断是接口政策还是平台的政策
            {
                string GCpyNo = CpyNo.Substring(0, 12);//获取上级供应商或落地运营商的ID
                if (GCpyNo == listLocalPolicy[k].CpyNo)//政策是上级公司发出的,就是本地
                {
                    if (listLocalPolicy[k].PolicyType == 1)//B2B
                    {
                        policySource = 1;
                    }
                    if (listLocalPolicy[k].PolicyType == 2)//BSP
                    {
                        policySource = 2;
                    }
                    //BSP/B2B这样的政策,现在直接默认为BSP
                    if (listLocalPolicy[k].PolicyType == 3)//B2B
                    {
                        //后面显示,所以类型也直接改为2
                        listLocalPolicy[k].PolicyType = 2;
                        policySource = 2;
                    }
                }
                else//不是上级公司发布的就是共享
                {
                    policySource = 9;
                }
            }
            else
            {
                try
                {
                    policySource = int.Parse(listLocalPolicy[k].CpyNo.Substring(0, 2));
                }
                catch (Exception)
                {
                    policySource = 0;
                }
            }
            return policySource;
        }
        /// <summary>
        /// 构建匹配后的table
        /// </summary>
        /// <returns></returns>
        private DataTable createMatchPolicyTable(string TableName)
        {

            DataTable dt = new DataTable(TableName);
            //航线对应GUID
            DataColumn dcGUID = new DataColumn("GUID");
            dt.Columns.Add(dcGUID);
            //政策发布公司CPYNO
            DataColumn dcCpyNo = new DataColumn("CpyNo");
            dt.Columns.Add(dcCpyNo);

            //票价生成方式 1.正常价格，2.动态特价，3.固定特价
            DataColumn dcGenerationType = new DataColumn("GenerationType");
            dt.Columns.Add(dcGenerationType);
            /// 政策种类 0.通用， 1.普通，2.特价
            DataColumn dcPolicyKind = new DataColumn("PolicyKind");
            dt.Columns.Add(dcPolicyKind);
            //显示的政策
            DataColumn dcPolicy = new DataColumn("Policy");
            dt.Columns.Add(dcPolicy);
            //显示的佣金
            DataColumn dcCommission = new DataColumn("Commission");
            dt.Columns.Add(dcCommission);
            //显示的现返
            DataColumn dcReturnMoney = new DataColumn("ReturnMoney");
            dt.Columns.Add(dcReturnMoney);
            //显示的实际支付价格
            DataColumn dcSJFee = new DataColumn("SJFee");
            dt.Columns.Add(dcSJFee);
            ////固定特价舱位价
            //DataColumn dcSpacePrice = new DataColumn("SpacePrice");
            //dt.Columns.Add(dcSpacePrice);

            //承运人
            DataColumn dcCarryCode = new DataColumn("CarryCode");
            dt.Columns.Add(dcCarryCode);
            //舱位
            DataColumn dcSpace = new DataColumn("Space");
            dt.Columns.Add(dcSpace);
            //折扣
            DataColumn dcDiscountRate = new DataColumn("DiscountRate");
            dt.Columns.Add(dcDiscountRate);
            //政策编号
            DataColumn dcPolicyId = new DataColumn("PolicyId");
            dt.Columns.Add(dcPolicyId);
            //航空公司返点
            DataColumn dcAirPoint = new DataColumn("AirPoint");
            dt.Columns.Add(dcAirPoint);
            //航空公司现返
            DataColumn dcAirReturnMoney = new DataColumn("AirReturnMoney");
            dt.Columns.Add(dcAirReturnMoney);

            //原始政策返点
            DataColumn dcOldPolicyPoint = new DataColumn("OldPolicyPoint");
            dt.Columns.Add(dcOldPolicyPoint);
            //原始现返
            DataColumn dcOldReturnMoney = new DataColumn("OldReturnMoney");
            dt.Columns.Add(dcOldReturnMoney);

            //出票政策返点
            DataColumn dcPolicyPoint = new DataColumn("PolicyPoint");
            dt.Columns.Add(dcPolicyPoint);
            //出票现返
            DataColumn dcReturnMoney2 = new DataColumn("ReturnMoney2");
            dt.Columns.Add(dcReturnMoney2);

            //政策后返
            DataColumn dcLaterPoint = new DataColumn("LaterPoint");
            dt.Columns.Add(dcLaterPoint);

            //基建
            DataColumn dcABFare = new DataColumn("ABFare");
            dt.Columns.Add(dcABFare);
            //燃油
            DataColumn dcRQFare = new DataColumn("RQFare");
            dt.Columns.Add(dcRQFare);

            //舱位价
            DataColumn dcSeatPrice = new DataColumn("SeatPrice");
            dt.Columns.Add(dcSeatPrice);

            //扣点明细
            DataColumn dcDiscountDetail = new DataColumn("DiscountDetail");
            dt.Columns.Add(dcDiscountDetail);

            //政策备注
            DataColumn dcPolicyRemark = new DataColumn("PolicyRemark");
            dt.Columns.Add(dcPolicyRemark);

            //政策类型
            DataColumn dcPolicyType = new DataColumn("PolicyType");
            dt.Columns.Add(dcPolicyType);
            //自动出票方式 手动(0或者null空)， 半自动1， 自动2
            DataColumn dcAutoPrintFlag = new DataColumn("AutoPrintFlag");
            dt.Columns.Add(dcAutoPrintFlag);
            //默认1 政策来源 0 本地BSP, 1 本地B2B， 2 百拓, 3 517 ，4 今日天下通，5，票盟 6，51book ,7 共享
            DataColumn dcPolicySource = new DataColumn("PolicySource");
            dt.Columns.Add(dcPolicySource);

            //默认1 政策来源 0 本地BSP, 1 本地B2B， 2 百拓, 3 517 ，4 今日天下通，5，票盟 6，51book ,7 共享
            DataColumn dcPolicyOffice = new DataColumn("PolicyOffice");
            dt.Columns.Add(dcPolicyOffice);
            //默认0不是默认政策 1成人默认政策 2儿童默认政策
            DataColumn dcDefaultType = new DataColumn("DefaultType");
            dt.Columns.Add(dcDefaultType);
            //是否高返政策 1是 0否 默认0
            DataColumn dcHighPolicyFlag = new DataColumn("HighPolicyFlag");
            dt.Columns.Add(dcHighPolicyFlag);
            //正常上班时间00:00-00:00
            DataColumn dcWorkTime = new DataColumn("WorkTime");
            dt.Columns.Add(dcWorkTime);
            //废票时间
            DataColumn dcPolicyCancelTime = new DataColumn("PolicyCancelTime");
            dt.Columns.Add(dcPolicyCancelTime);
            //退票时间
            DataColumn dcPolicyReturnTime = new DataColumn("PolicyReturnTime");
            dt.Columns.Add(dcPolicyReturnTime);
            //废票改签时间 00:00-00:00
            DataColumn dcFPGQTime = new DataColumn("FPGQTime");
            dt.Columns.Add(dcFPGQTime);
            //出票效率值
            DataColumn dcChuPiaoShiJian = new DataColumn("ChuPiaoShiJian");
            dt.Columns.Add(dcChuPiaoShiJian);
            //出票效率值
            DataColumn dcPatchPonit = new DataColumn("PatchPonit");
            dt.Columns.Add(dcPatchPonit);
            //今日供应商ID
            DataColumn dcJinriGYCode = new DataColumn("JinriGYCode");
            dt.Columns.Add(dcJinriGYCode);
            //B2B航空公司政策支付金额
            DataColumn dcAirPayMoney = new DataColumn("AirPayMoney");
            dt.Columns.Add(dcAirPayMoney);

            ////原始政策
            //// dcOldPolicy = new DataColumn("OldPolicy");
            ////dt.Columns.Add(dcOldPolicy);
            ////供应政策
            //DataColumn dcGYPolicy = new DataColumn("GYPolicy");
            //dt.Columns.Add(dcGYPolicy);
            ////分销政策
            //DataColumn dcFXPolicy = new DataColumn("FXPolicy");
            //dt.Columns.Add(dcFXPolicy);
            ////下级分销现返金额
            //DataColumn dcDownReturnMoney = new DataColumn("DownReturnMoney");
            //dt.Columns.Add(dcDownReturnMoney);
            ////下级分销后返现返金额
            //DataColumn dcLaterReturnMoney = new DataColumn("LaterReturnMoney");
            //dt.Columns.Add(dcLaterReturnMoney);
            ////共享政策现返金额
            //DataColumn dcShareReturnMoney = new DataColumn("ShareReturnMoney");
            //dt.Columns.Add(dcShareReturnMoney);

            return dt;
        }
        /// <summary>
        /// 组合政策
        /// </summary>
        /// <param name="sourcePolicy">源政策</param>
        /// <param name="targerPolicy">目标政策</param>
        private void CombinationPolicy(ref List<Tb_Ticket_Policy> listSourcePolicy, List<Tb_Ticket_Policy> listTargerPolicy)
        {
            try
            {
                if (listSourcePolicy != null && listTargerPolicy != null)
                {
                    listSourcePolicy.AddRange(listTargerPolicy);
                }
                else
                {
                    if (listSourcePolicy == null && listTargerPolicy != null && listTargerPolicy.Count > 0)
                    {
                        listSourcePolicy = new List<Tb_Ticket_Policy>();
                        listSourcePolicy.AddRange(listTargerPolicy);
                    }
                }
            }
            catch (Exception)
            {

                //政策组合错误
            }

        }
        /// <summary>
        /// 政策排序设置
        /// </summary>
        /// <param name="sourcePolicy">源政策</param>
        /// <param name="orderNo">排序号</param>
        private void orderPolicy(List<Tb_Ticket_Policy> listSourcePolicy, int orderNo)
        {
            try
            {
                if (listSourcePolicy != null)
                {
                    for (int i = 0; i < listSourcePolicy.Count; i++)
                    {
                        listSourcePolicy[i]._orderByPolicy = orderNo;
                    }
                }
            }
            catch (Exception)
            {

                //政策组合错误
            }

        }
        /// <summary>
        /// 政策航班号匹配(重载)
        /// </summary>
        /// <param name="Flight">政策适用航班号</param>
        /// <param name="NoFlight">政策不适用航班号</param>
        /// <param name="NowFlight">当前航班号</param>
        /// <param name="PolicySorce">政策来源编号</param>
        /// <returns>返回是否成功标识</returns>
        private bool CompareFlight(string Flight, string NoFlight, string NowFlight, int PolicySorce)
        {
            //旧版本
            //订单来源：0 BSP, 1 B2B， 2 百拓, 3 517 ，4 今日天下通，5，票盟 6，51book ,7 共享，8 8000yi，
            //新版本
            //订单来源：1 本地B2B, 2 本地BSP,3 517,4 百拓,5 8000翼,6 今日,7 票盟,8 51book ,9 共享
            bool IsSuccess = false;
            try
            {
                if (Flight.Contains("*") || NoFlight.Contains("*"))
                {
                    char zf = '/';
                    if (PolicySorce == 4)
                    {
                        zf = ',';
                    }
                    string[] fs = Flight.Replace("*", "").Split(zf);
                    string[] nfs = NoFlight.Replace("*", "").Split(zf);
                    if (fs.Length > 0)
                    {
                        for (int i = 0; i < fs.Length; i++)
                        {
                            string fly1 = NowFlight.Substring(0, fs[i].Length);
                            if (fly1 == fs[i])
                            {
                                IsSuccess = true;
                                break;
                            }
                        }
                    }
                    else
                    {
                        int err = 0;
                        for (int i = 0; i < nfs.Length; i++)
                        {
                            string fly2 = NowFlight.Substring(0, nfs[i].Length);
                            if (fly2 == nfs[i])
                            {
                                err++;
                            }
                        }
                        if (err == 0)
                        {
                            IsSuccess = true;
                        }
                    }
                }
                else if (Flight.ToString() != "")
                {
                    string[] Flights = null;
                    if (PolicySorce == 1 || PolicySorce == 2 || PolicySorce == 3)
                    {
                        Flights = Flight.Replace(",", "/").Replace("^", "/").Split('/');
                    }
                    else if (PolicySorce == 4)
                    {
                        Flights = Flight.Replace(",", "/").Replace("^", "/").Split('/');
                    }
                    else
                    {
                        Flights = Flight.Replace(",", "/").Replace("^", "/").Split('/');
                    }
                    for (int y = 0; y < Flights.Length; y++)
                    {
                        if (Flights[y].ToString().Trim() == NowFlight.ToString().Trim())
                        {
                            IsSuccess = true;
                            break;
                        }
                    }
                    if (!IsSuccess)
                    {
                        if (NoFlight != "")
                        {
                            string[] NoFlights = null;
                            if (PolicySorce == 1 || PolicySorce == 2 || PolicySorce == 3)
                            {
                                NoFlights = NoFlight.Replace(",", "/").Replace("^", "/").Split('/');
                            }
                            else if (PolicySorce == 4)
                            {
                                NoFlights = NoFlight.Replace(",", "/").Replace("^", "/").Split('/');
                            }
                            else
                            {
                                NoFlights = NoFlight.Replace(",", "/").Replace("^", "/").Split('/');
                            }
                            for (int c = 0; c < NoFlights.Length; c++)
                            {
                                if (NoFlights[c].ToString().Trim() == NowFlight.Trim())
                                {
                                    IsSuccess = false;
                                    break;
                                }
                                else
                                {
                                    IsSuccess = true;
                                    continue;
                                }
                            }
                        }
                    }
                }
                else
                {
                    if (NoFlight.ToString() != "")
                    {
                        string[] NoFlights = null;
                        if (PolicySorce == 1 || PolicySorce == 2 || PolicySorce == 3)
                        {
                            NoFlights = NoFlight.ToString().Replace(",", "/").Replace("^", "/").Split('/');
                        }
                        else if (PolicySorce == 4)
                        {
                            NoFlights = NoFlight.ToString().Replace(",", "/").Replace("^", "/").Split('/');
                        }
                        else
                        {
                            NoFlights = NoFlight.ToString().Replace(",", "/").Replace("^", "/").Split('/');
                        }
                        for (int c = 0; c < NoFlights.Length; c++)
                        {
                            if (NoFlights[c].ToString().Trim() == NowFlight.Trim())
                            {
                                IsSuccess = false;
                                break;
                            }
                            else
                            {
                                IsSuccess = true;
                                continue;
                            }
                        }
                    }
                    else
                    {
                        IsSuccess = true;
                    }
                }
            }
            catch
            {

            }
            return IsSuccess;
        }

        /// <summary>
        /// 工作时间转换
        /// </summary>
        /// <param name="OldTime">原始时间</param>
        /// <param name="GYTime">供应时间</param>
        /// <returns></returns>
        public string WorkTimeConvert(string OldTime, string GYTime)
        {
            string[] symbols = new string[]{
                "/",
                "^",
                ".",
                "~",
                "#",
                "&",
                "*",
                "|"
            };
            if (GYTime == null || GYTime == "")
            {
                GYTime = "08:00-23:00";
            }
            foreach (string symbol in symbols)
            {
                OldTime = OldTime.Replace(symbol, "-");
                GYTime = GYTime.Replace(symbol, "-");
            }
            string[] OldTimeList = OldTime.Split('-');
            string[] GYTimeList = GYTime.Split('-');
            string[] NewTimeList = new string[2];
            try
            {
                if (OldTimeList[0].IndexOf(":") < 0)
                {
                    OldTimeList[0] = OldTimeList[0].Substring(0, 2) + ":" + OldTimeList[0].Substring(2, 2);
                }
                if (GYTimeList[0].IndexOf(":") < 0)
                {
                    GYTimeList[0] = GYTimeList[0].Substring(0, 2) + ":" + GYTimeList[0].Substring(2, 2);
                }
                if (OldTimeList[1].IndexOf(":") < 0)
                {
                    OldTimeList[1] = OldTimeList[1].Substring(0, 2) + ":" + OldTimeList[1].Substring(2, 2);
                }
                if (GYTimeList[1].IndexOf(":") < 0)
                {
                    GYTimeList[1] = GYTimeList[1].Substring(0, 2) + ":" + GYTimeList[1].Substring(2, 2);
                }
                if (DateTime.Compare(DateTime.Parse(OldTimeList[0]), DateTime.Parse(GYTimeList[0])) <= 0)
                {
                    NewTimeList[0] = GYTimeList[0];
                }
                else
                {
                    NewTimeList[0] = OldTimeList[0];
                }
                if (DateTime.Compare(DateTime.Parse(OldTimeList[1]), DateTime.Parse(GYTimeList[1])) >= 0)
                {
                    NewTimeList[1] = GYTimeList[1];
                }
                else
                {
                    NewTimeList[1] = OldTimeList[1];
                }
            }
            catch (Exception e)
            {
                //OnError("类：PolicyMatching中方法WorkTimeConvert报错：" + e.ToString(), "WorkTimeConvert");
            }
            return NewTimeList[0] + "-" + NewTimeList[1];
        }

        /// <summary>
        /// 对日期进行判断
        /// </summary>
        /// <param name="drNow">当前传入的列值</param>
        /// <param name="Schedule">当前查询出的班期限制</param>
        /// <returns>返回是否成功标识</returns>
        private bool CompareSchedule(string StartDate, string Schedule, int Type)
        {
            //声明变量
            bool IsSuccess = false;

            //对当前乘机日期进行转换并获得乘机日期的星期值
            DateTime Time = Convert.ToDateTime(StartDate);
            //根据日期获取日期的星期值
            DayOfWeek WeekDay = Time.DayOfWeek;

            //判断查询得到的班期限制是否为空
            if (Schedule != "//" && Schedule != "")
            {
                string Days = "";
                #region  日期判断
                if (WeekDay == DayOfWeek.Monday)
                {
                    Days = "1";
                }
                else if (WeekDay == DayOfWeek.Tuesday)
                {
                    Days = "2";
                }
                else if (WeekDay == DayOfWeek.Wednesday)
                {
                    Days = "3";
                }
                else if (WeekDay == DayOfWeek.Thursday)
                {
                    Days = "4";
                }
                else if (WeekDay == DayOfWeek.Friday)
                {
                    Days = "5";
                }
                else if (WeekDay == DayOfWeek.Saturday)
                {
                    Days = "6";
                }
                else if (WeekDay == DayOfWeek.Sunday)
                {
                    Days = "7";
                }
                #endregion

                if (Type == 1 || Type == 2 || Type == 9)
                {
                    bool Isok = false;
                    if (Days == "7")
                    {
                        if (Schedule.Replace(",", "/").Replace("^", "/").Contains(Days) || Schedule.Replace(",", "/").Replace("^", "/").Contains("0"))
                        {
                            Isok = true;
                        }
                    }
                    else
                    {
                        if (Schedule.Replace(",", "/").Replace("^", "/").Contains(Days))
                        {
                            Isok = true;
                        }
                    }
                    if (Isok)
                    {
                        IsSuccess = false;
                    }
                    else
                    {
                        IsSuccess = true;
                    }
                }
                else
                {
                    bool Isok = false;
                    if (Days == "7")
                    {
                        if (Schedule.Replace(",", "/").Replace("^", "/").Contains(Days) || Schedule.Replace(",", "/").Replace("^", "/").Contains("0"))
                        {
                            Isok = true;
                        }
                    }
                    else
                    {
                        if (Schedule.Replace(",", "/").Replace("^", "/").Contains(Days))
                        {
                            Isok = true;
                        }
                    }
                    if (Isok)
                    {
                        IsSuccess = true;
                    }
                    else
                    {
                        IsSuccess = false;
                    }
                }
            }
            else
            {
                IsSuccess = true;
            }

            return IsSuccess;
        }

        private string CityConvert(string sqlstr, string CityCode, string ColName)
        {
            string newsqlstr = sqlstr;
            string doublecity = "SHA,PVG,PEK,NAY";
            string allstr = "";
            if (doublecity.Contains(CityCode.ToUpper()))
            {
                sqlstr = sqlstr.Replace("and", " and(");
                if (CityCode.ToUpper() == "SHA")
                {
                    newsqlstr = newsqlstr.Replace(ColName, "'%PVG%'").Replace(" and", " or ") + ")"; ;
                }
                if (CityCode.ToUpper() == "PVG")
                {
                    newsqlstr = newsqlstr.Replace(ColName, "'%SHA%'").Replace(" and", " or ") + ")";
                }
                if (CityCode.ToUpper() == "PEK")
                {
                    newsqlstr = newsqlstr.Replace(ColName, "'%NAY%'").Replace(" and", " or ") + ")"; ;
                }
                if (CityCode.ToUpper() == "NAY")
                {
                    newsqlstr = newsqlstr.Replace(ColName, "'%PEK%'").Replace(" and", " or ") + ")";
                }
                allstr = sqlstr + newsqlstr;
            }
            else
            {
                allstr = sqlstr;
            }
            return allstr;
        }
        /// <summary>
        ///获取当前政策的对应公司的所有工作退费票时间
        /// </summary>
        /// <param name="listLocalPolicy"></param>
        /// <returns></returns>
        private List<User_Company> getWorkTimeInNowPolicy(List<Tb_Ticket_Policy> listLocalPolicy)
        {
            StringBuilder sbCpyNo = new StringBuilder("('-1'");
            for (int i = 0; i < listLocalPolicy.Count; i++)
            {
                //接口的时间不在这里处理
                if (listLocalPolicy[i].CpyNo.Length != 14)
                {
                    //没有添加过的公司就添加
                    if (!sbCpyNo.ToString().Contains(listLocalPolicy[i].CpyNo))
                    {
                        sbCpyNo.Append(",'" + listLocalPolicy[i].CpyNo + "'");
                    }
                }
            }
            sbCpyNo.Append(")");
            string sqlwhere = " UninCode in " + sbCpyNo.ToString();
            //获取当前政策的本地工作时间和退费票时间
            List<User_Company> objList = Manage.CallMethod("User_Company", "GetList", null, new object[] { sqlwhere }) as List<User_Company>;
            return objList;

        }
        /// <summary>
        /// 根据公司编号获取工作时间和退费票时间
        /// </summary>
        /// <param name="cpyNo"></param>
        /// <param name="listUser_Company"></param>
        /// <returns></returns>
        private string WorkTimeOrBusinessTime(string cpyNo, List<User_Company> listUser_Company)
        {
            string workTimeOrBusinessTime = "";
            for (int i = 0; i < listUser_Company.Count; i++)
            {
                if (listUser_Company[i].UninCode == cpyNo)
                {
                    workTimeOrBusinessTime = listUser_Company[i].WorkTime + "," + listUser_Company[i].BusinessTime;
                }
            }
            return workTimeOrBusinessTime;
        }
        /// <summary>
        /// 供应商上下班时间
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public bool IsOkTime(string time)
        {
            bool isok = false;
            try
            {
                if (time == "00:00-23:59")
                {
                    return true;
                }
                if (time != "")
                {
                    string[] times1 = time.Split('-')[0].ToString().Split(':');
                    string timeOld = "";
                    if (DateTime.Parse(time.Split('-')[1]).ToShortTimeString() == "0:00")
                    {
                        timeOld = "23:59";
                    }
                    else
                    {
                        timeOld = DateTime.Parse(time.Split('-')[1]).ToShortTimeString();
                    }
                    string[] times2 = timeOld.Split(':');//.AddMinutes(-30)
                    string[] nowtime = DateTime.Now.ToShortTimeString().Split(':');
                    if (int.Parse(nowtime[0]) > int.Parse(times1[0]) && int.Parse(nowtime[0]) < int.Parse(times2[0]))
                    {
                        isok = true;
                    }
                    else if (int.Parse(nowtime[0]) == int.Parse(times1[0]) && int.Parse(nowtime[1]) > int.Parse(times1[1]))
                    {
                        isok = true;
                    }
                    else if (int.Parse(nowtime[0]) == int.Parse(times2[0]) && int.Parse(nowtime[1]) < int.Parse(times2[1]))
                    {
                        isok = true;
                    }
                }
                else
                {
                    isok = true;
                }
            }
            catch (Exception)
            {

                isok = false;
            }
            return isok;
        }
        /// <summary>
        /// 计算扣点
        /// </summary>
        /// <param name="list">政策集合</param>
        /// <param name="cpyNo">当前登录账号的公司ID</param>
        /// <param name="fromCity">出发城市</param>
        /// <param name="toCity">到达城市</param>
        /// <returns></returns>
        private string TakeOffCalculate(List<Tb_Ticket_Policy> list, string cpyNo, string fromCity, string toCity, string GroupId)
        {

            string errorMsg = "";
            try
            {

                int nowClass = cpyNo.Length / 6;//当前账号共有几级
                string cpyNo1 = "";//落地ID
                string cpyNo2 = "";//分销ID
                string cpyNo3 = "";//二级分销ID               
                string sqlwhere = "";
                string strGroupIdSQL = "";
                switch (nowClass)
                {
                    case 1://平台
                    case 2://落地供应
                        sqlwhere = "1<>1";
                        break;
                    case 3://分销或同级采购
                        cpyNo1 = "'" + cpyNo.Substring(0, 12) + "'";
                        strGroupIdSQL = "select GroupId from User_Company where unincode in('" + cpyNo + "') and rtrim(ltrim(GroupId))<>'' and GroupId is not  null";
                        sqlwhere = " 1=1 and cpyNo =" + cpyNo1
                              + " and GroupId in(" + strGroupIdSQL + ")"
                              + " and (FromCityCode like'%" + fromCity + "%' or FromCityCode  like '%ALL%' ) "
                              + " and (ToCityCode like'%" + toCity + "%' or ToCityCode  like '%ALL%' )";
                        break;
                    case 4://二级分销或同级采购
                        cpyNo1 = "'" + cpyNo.Substring(0, 12) + "'";
                        cpyNo2 = "'" + cpyNo.Substring(0, 18) + "'";
                        strGroupIdSQL = "select GroupId from User_Company where unincode in('" + cpyNo + "'," + cpyNo2 + ") and rtrim(ltrim(GroupId))<>'' and GroupId is not  null";
                        sqlwhere = " 1=1 and cpyNo =" + cpyNo1
                             + " and GroupId in(" + strGroupIdSQL + ") "
                             + " and (FromCityCode like'%" + fromCity + "%' or FromCityCode  like '%ALL%' ) "
                             + " and (ToCityCode like'%" + toCity + "%' or ToCityCode  like '%ALL%' )"
                             + " union all "

                             + " (select * from Tb_Ticket_TakeOffDetail  where  cpyNo =" + cpyNo2
                             + " and GroupId in(" + strGroupIdSQL + ") "
                             + " and (FromCityCode like'%" + fromCity + "%' or FromCityCode  like '%ALL%' ) "
                             + " and (ToCityCode like'%" + toCity + "%' or ToCityCode  like '%ALL%' ) )";
                        break;
                    case 5://二级分销开的采购
                        cpyNo1 = "'" + cpyNo.Substring(0, 12) + "'";
                        cpyNo2 = "'" + cpyNo.Substring(0, 18) + "'";
                        cpyNo3 = "'" + cpyNo.Substring(0, 24) + "'";
                        strGroupIdSQL = "select GroupId from User_Company where unincode in('" + cpyNo + "'," + cpyNo3 + "," + cpyNo2 + ") and rtrim(ltrim(GroupId))<>'' and GroupId is not  null";
                        sqlwhere = " 1=1 and cpyNo =" + cpyNo1
                             + " and GroupId in(" + strGroupIdSQL + ")"
                             + " and (FromCityCode like'%" + fromCity + "%' or FromCityCode  like '%ALL%' ) "
                             + " and (ToCityCode like'%" + toCity + "%' or ToCityCode  like '%ALL%' )"
                             + " union all "

                             + " (select  * from Tb_Ticket_TakeOffDetail where cpyNo =" + cpyNo2
                             + " and GroupId in(" + strGroupIdSQL + ")"
                             + " and (FromCityCode like'%" + fromCity + "%' or FromCityCode  like '%ALL%' ) "
                             + " and (ToCityCode like'%" + toCity + "%' or ToCityCode  like '%ALL%' ) )"
                             + " union all "

                             + "  (select  * from Tb_Ticket_TakeOffDetail where cpyNo =" + cpyNo3
                             + " and GroupId in(" + strGroupIdSQL + ")"
                             + " and (FromCityCode like'%" + fromCity + "%' or FromCityCode  like '%ALL%' ) "
                             + " and (ToCityCode like'%" + toCity + "%' or ToCityCode  like '%ALL%' ) )";
                        break;
                    default://账号解析出问题
                        sqlwhere = "1<>1";
                        break;
                }
                List<Tb_Ticket_TakeOffDetail> objList = Manage.CallMethod("Tb_Ticket_TakeOffDetail", "GetList", null, new object[] { sqlwhere }) as List<Tb_Ticket_TakeOffDetail>;
                for (int i = 0; i < objList.Count; i++)//扣点集合
                {
                    DateTime dtStartDate;
                    DateTime dtEndDate;
                    try
                    {
                        dtStartDate = DateTime.Parse(objList[i].TimeScope.Split('|')[0].ToString());
                        dtEndDate = DateTime.Parse(objList[i].TimeScope.Split('|')[1].ToString());
                    }
                    catch (Exception)
                    {
                        //格式发生错误,直接给默认时间
                        dtStartDate = DateTime.Now;
                        dtEndDate = DateTime.Now;
                    }
                    //时间在范围内,或者BaseType为0(0即为全部,仅适合分销和二级分销)
                    if (DateTime.Now >= dtStartDate && DateTime.Now <= dtEndDate)
                    {
                        for (int j = 0; j < list.Count; j++)//政策集合
                        {
                            //有可能空数据会导致异常
                            if (list[j].CpyNo.Trim() == "")
                            {
                                continue;
                            }
                            //扣本地政策的
                            if (list[j].CpyNo.Substring(0, 12) == cpyNo.Substring(0, 12) && (objList[i].BaseType == 1 || objList[i].BaseType == 0))
                            {
                                //承运人相等或承运人等于ALL
                                if (list[j].CarryCode.Contains(objList[i].CarryCode) || list[j].CarryCode.Contains("ALL") || objList[i].CarryCode.Contains("ALL"))
                                {
                                    if (checkCityIsOK(list[j].StartCityNameCode, list[j].TargetCityNameCode, objList[i].FromCityCode, objList[i].ToCityCode))
                                    {
                                        string DeductPolicyAndReturnMoney = AnalysisOfDeductScope(list[j].DownPoint, list[j].DownReturnMoney, objList[i].PointScope, objList[i].SelectType);
                                        decimal DeductPolicy = decimal.Parse(DeductPolicyAndReturnMoney.Split('^')[0]);
                                        decimal DeductReturnMoney = decimal.Parse(DeductPolicyAndReturnMoney.Split('^')[1]);
                                        decimal tempDownPoint = list[j].DownPoint;//如果扣超了扣为负数,则扣点记录为此数值
                                        decimal tempReturnMoney = list[j].DownReturnMoney;
                                        list[j].DownPoint = list[j].DownPoint - DeductPolicy;//政策扣点
                                        list[j].DownReturnMoney = list[j].DownReturnMoney - DeductReturnMoney;//现返扣点

                                        if (list[j].DownPoint < 0)//如果扣点扣为负数了,则直接为0
                                        {
                                            list[j].DownPoint = 0.0m;
                                            DeductPolicy = tempDownPoint;
                                        }

                                        //list[j].DownReturnMoney = 0.0m;
                                        //DeductReturnMoney = tempReturnMoney;

                                        //运营商扣一级分销的这条扣点关系不记录入_DiscountDetail明细字段,
                                        //而是记录进出票政策
                                        if (objList[i].CpyNo.Length == 12)
                                        {
                                            list[j]._PolicyPoint = list[j].DownPoint.ToString();
                                            list[j]._returnMoney = list[j].DownReturnMoney.ToString();
                                        }
                                        else
                                        {
                                            list[j]._DiscountDetail = list[j]._DiscountDetail + objList[i].CpyNo + "^" + DeductPolicy.ToString() + "^" + DeductReturnMoney.ToString() + "|";//扣点明细
                                        }
                                    }
                                }
                            }
                            //扣接口政策的
                            if (list[j].CpyNo.Substring(0, 12) != cpyNo.Substring(0, 12) && list[j].CpyNo.Length > 12 && (objList[i].BaseType == 2 || objList[i].BaseType == 0))
                            {
                                if (objList[i].BaseType == 0 || checkInterfaceIsOk(list[j].CpyNo.Substring(0, 2), objList[i].PolicySource))//当前政策是否是接口政策
                                {
                                    //承运人相等或承运人等于ALL
                                    if (list[j].CarryCode.Contains(objList[i].CarryCode) || list[j].CarryCode.Contains("ALL") || objList[i].CarryCode.Contains("ALL"))
                                    {
                                        if (checkCityIsOK(list[j].StartCityNameCode, list[j].TargetCityNameCode, objList[i].FromCityCode, objList[i].ToCityCode))
                                        {
                                            string DeductPolicyAndReturnMoney = AnalysisOfDeductScope(list[j].DownPoint, list[j].DownReturnMoney, objList[i].PointScope, objList[i].SelectType);
                                            decimal DeductPolicy = decimal.Parse(DeductPolicyAndReturnMoney.Split('^')[0]);
                                            decimal DeductReturnMoney = decimal.Parse(DeductPolicyAndReturnMoney.Split('^')[1]);
                                            decimal tempDownPoint = list[j].DownPoint;//如果扣超了扣为负数,则扣点记录为此数值
                                            decimal tempReturnMoney = list[j].DownReturnMoney;
                                            list[j].DownPoint = list[j].DownPoint - DeductPolicy;//政策扣点
                                            list[j].DownReturnMoney = list[j].DownReturnMoney - DeductReturnMoney;//现返扣点
                                            if (list[j].DownPoint < 0)//如果扣点扣为负数了,则直接为0
                                            {
                                                list[j].DownPoint = 0.0m;
                                                DeductPolicy = tempDownPoint;
                                            }
                                            //list[j].DownReturnMoney = 0.0m;
                                            //DeductReturnMoney = tempReturnMoney;
                                            //运营商扣一级分销的这条扣点关系不记录入_DiscountDetail明细字段,
                                            //而是记录进出票政策
                                            if (objList[i].CpyNo.Length == 12)
                                            {
                                                list[j]._PolicyPoint = list[j].DownPoint.ToString();
                                                list[j]._returnMoney = list[j].DownReturnMoney.ToString();
                                            }
                                            else
                                            {
                                                list[j]._DiscountDetail = list[j]._DiscountDetail + objList[i].CpyNo + "^" + DeductPolicy.ToString() + "^" + DeductReturnMoney.ToString() + "|";//扣点明细
                                            }

                                        }
                                    }
                                }
                            }
                            //扣共享政策的
                            if (list[j].CpyNo.Substring(0, 12) != cpyNo.Substring(0, 12) && list[j].CpyNo.Length == 12 && (objList[i].BaseType == 3 || objList[i].BaseType == 0))
                            {
                                //承运人相等或承运人等于ALL
                                if (list[j].CarryCode.Contains(objList[i].CarryCode) || list[j].CarryCode.Contains("ALL") || objList[i].CarryCode.Contains("ALL"))
                                {
                                    if (checkCityIsOK(list[j].StartCityNameCode, list[j].TargetCityNameCode, objList[i].FromCityCode, objList[i].ToCityCode))
                                    {
                                        string DeductPolicyAndReturnMoney = AnalysisOfDeductScope(list[j].DownPoint, list[j].DownReturnMoney, objList[i].PointScope, objList[i].SelectType);
                                        decimal DeductPolicy = decimal.Parse(DeductPolicyAndReturnMoney.Split('^')[0]);
                                        decimal DeductReturnMoney = decimal.Parse(DeductPolicyAndReturnMoney.Split('^')[1]);
                                        decimal tempDownPoint = list[j].DownPoint;//如果扣超了扣为负数,则扣点记录为此数值
                                        decimal tempReturnMoney = list[j].DownReturnMoney;
                                        list[j].DownPoint = list[j].DownPoint - DeductPolicy;//政策扣点
                                        list[j].DownReturnMoney = list[j].DownReturnMoney - DeductReturnMoney;//现返扣点
                                        if (list[j].DownPoint < 0)//如果扣点扣为负数了,则直接为0
                                        {
                                            list[j].DownPoint = 0.0m;
                                            DeductPolicy = tempDownPoint;
                                        }
                                        //list[j].DownReturnMoney = 0.0m;
                                        //DeductReturnMoney = tempReturnMoney;
                                        list[j]._DiscountDetail = list[j]._DiscountDetail + objList[i].CpyNo + "^" + DeductPolicy.ToString() + "^" + DeductReturnMoney.ToString() + "|";//扣点明细
                                    }
                                }
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                errorMsg = ex.Message;
            }
            return errorMsg;
        }
        /// <summary>
        /// 平台补点计算
        /// </summary>
        /// <param name="list">政策数据</param>
        /// <param name="cpyNo">当前登录账号的公司</param>
        /// <param name="fromCity">出发城市</param>
        /// <param name="toCity">到达城市</param>
        /// <returns></returns>
        private string PatchCalculate(List<Tb_Ticket_Policy> list, string cpyNo, string fromCity, string toCity)
        {
            string errorMsg = "";
            try
            {
                //获取补点信息
                string sqlwhere = " 1=1  and cpyNo='" + cpyNo.Substring(0, 12) + "' and state=1 and "
                    + " (FromCityCode like '%" + fromCity + "%' or FromCityCode like '%ALL%' ) and "
                     + " (ToCityCode like '%" + toCity + "%' or ToCityCode like '%ALL%' )";
                List<Tb_Policy_Supply> objList = Manage.CallMethod("Tb_Policy_Supply", "GetList", null, new object[] { sqlwhere }) as List<Tb_Policy_Supply>;
                if (list != null && objList != null)
                {
                    for (int i = 0; i < list.Count; i++)
                    {
                        int policySource = 0;
                        policySource = getPolicySource(list, cpyNo, i);
                        if (policySource == 0)
                        {
                            continue;
                        }
                        //现在默认一个公司一条,承运人出发城市,到达城市都是ALL,不判断,只判断补点的政策来源
                        for (int j = 0; j < objList.Count; j++)
                        {
                            //补点设置的政策来源可能有多个,每个用,分隔开
                            if (objList[j].A7.Contains("," + policySource + ","))
                            {
                                list[i]._patchPonit = objList[j].PolicyPoint;//记录补点值
                                list[i].DownPoint = list[i].DownPoint + list[i]._patchPonit;//政策加上补丁值
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                errorMsg = ex.Message;
            }
            return errorMsg;
        }
        /// <summary>
        /// 本地补点
        /// </summary>
        /// <param name="list"></param>
        /// <param name="cpyNo"></param>
        /// <param name="fromCity"></param>
        /// <param name="toCity"></param>
        /// <returns></returns>
        private string LocalPatchCalculate(List<Tb_Ticket_Policy> list, string cpyNo, string fromCity, string toCity,string GroupId)
        {
            string errorMsg = "";
            try
            {
                string sqlwhere = " 1=1 and selectType=3 and cpyNo='" + cpyNo.Substring(0, 12) + "' and GroupId='" + GroupId + "' and"
                  + " (FromCityCode like '%" + fromCity + "%' or FromCityCode like '%ALL%' ) and "
                   + " (ToCityCode like '%" + toCity + "%' or ToCityCode like '%ALL%' )";
                List<Tb_Ticket_TakeOffDetail> objList = Manage.CallMethod("Tb_Ticket_TakeOffDetail", "GetList", null, new object[] { sqlwhere }) as List<Tb_Ticket_TakeOffDetail>;
                if (list != null && objList != null)
                {
                    for (int i = 0; i < list.Count; i++)
                    {
                        for (int j = 0; j < objList.Count; j++)
                        {
                            //只有本地才补点
                            string localpoint = AnalysisOfDeductScope(list[i].DownPoint, 0, objList[j].PointScope, 1);//根据扣点规则得到对应补点
                            if ((getPolicySource(list, cpyNo, i) == 1 || getPolicySource(list, cpyNo, i)==2) && localpoint != "0^0")
                            {
                                //list[i]._patchPonit = decimal.Parse(localpoint.Split('^')[0]);//记录补点值
                                list[i].DownPoint = list[i].DownPoint + decimal.Parse(localpoint.Split('^')[0]);//政策加上补点值
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                errorMsg = ex.Message;
            }
            return errorMsg;
        }
        /// <summary>
        /// 解析扣点范围
        /// </summary>
        /// <param name="oldPolicy">当前政策点数</param>
        /// <param name="DeductScopeStr">扣点范围字符串</param>
        /// <param name="SelectType">扣点还是留点 1扣2留</param>
        /// <returns>扣点和扣除的现返</returns>
        private string AnalysisOfDeductScope(decimal oldPolicy, decimal oldReturnMoney, string DeductScopeStr, int SelectType)
        {
            string rsPolicy = "0^0";//默认0扣点0现返
            try
            {

                //字符串例如: 0-3^0.5^0|3.1-5^0.8^0|5.1-8^1^0|8.1-100^2^5
                string[] DeductScopeStrAll = DeductScopeStr.Split('|');//分隔出来的范围数组
                int DeductScopeStrLen = DeductScopeStrAll.Length;//数组长度
                for (int i = 0; i < DeductScopeStrLen; i++)
                {
                    string[] ScopesAndNumber = DeductScopeStrAll[i].Split('^');//分隔范围和点数 如  0-3^0.5^0
                    string[] Scopes = ScopesAndNumber[0].Split('-');//分隔范围 如 1-3
                    string deductPolicy = ScopesAndNumber[1];//扣除的点数
                    string deductreturnMoney = ScopesAndNumber[2];//扣除的现返
                    decimal ScopesFront = decimal.Parse(Scopes[0]);//范围前段
                    decimal ScopesBack = decimal.Parse(Scopes[1]);//范围后段

                    if (oldPolicy >= ScopesFront && oldPolicy <= ScopesBack)
                    {
                        if (SelectType == 1)//扣点
                        {
                            rsPolicy = deductPolicy + "^" + deductreturnMoney;//找到匹配的范围就赋值并且跳出循环
                            break;
                        }
                        //留点模式输出值也采用扣点方式(既直接计算留点后应该在原来基础上扣多少点)
                        else if (SelectType == 2)
                        {
                            string keepPolicy = "";
                            string keepReturnMoney = "";
                            if (oldPolicy >= decimal.Parse(deductPolicy))//当前点数大于等于留点,则当前点数-留点
                            {
                                keepPolicy = (oldPolicy - decimal.Parse(deductPolicy)).ToString();
                            }
                            else//留点政策小于当前,则不扣
                            {
                                keepPolicy = "0";
                            }
                            if (oldReturnMoney >= decimal.Parse(deductreturnMoney))//当前现返大于等于留点,则当前现返-留点
                            {
                                keepReturnMoney = (oldReturnMoney - decimal.Parse(deductreturnMoney)).ToString();
                            }
                            else//留点先返小于当前,则不扣
                            {
                                keepReturnMoney = "0";
                            }
                            rsPolicy = keepPolicy + "^" + keepReturnMoney;//找到匹配的范围就赋值并且跳出循环
                            break;

                        }
                       
                    }

                }
            }
            catch (Exception)
            {
                rsPolicy = "0^0";
            }
            return rsPolicy;
        }
        /// <summary>
        /// 匹配城市
        /// </summary>
        /// <param name="policyFromCity">政策出发城市集合</param>
        /// <param name="policyToCity">政策到达城市集合</param>
        /// <param name="DeductFromCity">扣点出发城市集合</param>
        /// <param name="DeductToCity">扣点到达城市集合</param>
        /// <returns></returns>
        private bool checkCityIsOK(string policyFromCity, string policyToCity, string DeductFromCity, string DeductToCity)
        {
            bool rs = false;
            string[] policyFromCitys = policyFromCity.Split('/');
            string[] policyToCitys = policyToCity.Split('/');
            string[] DeductFromCitys = DeductFromCity.Split('/');
            string[] DeductToCitys = DeductToCity.Split('/');
            for (int i = 0; i < policyFromCitys.Length; i++)
            {
                if (policyFromCitys[i] == "")
                {
                    continue;
                }
                for (int j = 0; j < DeductFromCitys.Length; j++)
                {
                    if (DeductFromCitys[j] == "")
                    {
                        continue;
                    }
                    if (policyFromCitys[i].ToUpper() == "ALL" || DeductFromCitys[j].ToUpper() == "ALL")
                    {
                        rs = true;
                        break;
                    }
                    if (policyFromCitys[i].ToUpper() == DeductFromCitys[j].ToUpper())
                    {
                        rs = true;
                        break;
                    }
                }
                if (rs)
                {
                    break;
                }
            }
            if (rs)
            {
                for (int i = 0; i < policyToCitys.Length; i++)
                {
                    if (policyToCitys[i] == "")
                    {
                        continue;
                    }
                    for (int j = 0; j < DeductToCitys.Length; j++)
                    {
                        if (DeductToCitys[j] == "")
                        {
                            continue;
                        }
                        if (policyToCitys[i].ToUpper() == "ALL" || DeductToCitys[j].ToUpper() == "ALL")
                        {
                            rs = true;
                            break;
                        }
                        if (policyToCitys[i].ToUpper() == DeductToCitys[j].ToUpper())
                        {
                            rs = true;
                            break;
                        }
                    }
                    if (rs)
                    {
                        break;
                    }

                }
            }
            if (!rs)
            {
                rs = true;
            }
            return rs;
        }
        /// <summary>
        /// 匹配接口扣点
        /// </summary>
        /// <param name="nowPolicySourse">当前的接口政策来源</param>
        /// <param name="DeductPolicySourse">扣点的接口来源</param>
        /// <returns></returns>
        private bool checkInterfaceIsOk(string nowPolicySourse, string DeductPolicySourse)
        {
            bool rs = false;
            nowPolicySourse = int.Parse(nowPolicySourse).ToString();
            string[] DeductPolicySourses = DeductPolicySourse.Split(',');
            for (int i = 0; i < DeductPolicySourses.Length; i++)
            {
                if (DeductPolicySourses[i] == nowPolicySourse)
                {
                    rs = true;
                    break;
                }
            }
            return rs;
        }

        /// <summary>
        /// 是否受本地政策保护
        /// </summary>
        /// <param name="list">本地保护数据集</param>
        /// <param name="airCode">航空公司</param>
        /// <param name="fromCity">出发城市</param>
        /// <param name="toCity">到达城市</param>
        /// <returns></returns>
        private bool protectedPolicy(List<Tb_Policy_Protect> list, string airCode, string fromCity, string toCity)
        {
            bool result = false;
            int listCount = list.Count;
            for (int i = 0; i < listCount; i++)
            {
                if (list[i].CarryCode.ToUpper().Contains(airCode) || airCode.ToUpper().Contains("ALL") || list[i].CarryCode.ToUpper().Contains("ALL"))
                {
                    if (list[i].FromCityCode.ToUpper().Contains(fromCity) || fromCity.ToUpper().Contains("ALL") || list[i].FromCityCode.ToUpper().Contains("ALL"))
                    {
                        if (list[i].ToCityCode.ToUpper().Contains(toCity) || toCity.ToUpper().Contains("ALL") || list[i].ToCityCode.ToUpper().Contains("ALL"))
                        {
                            result = true;
                            break;
                        }
                    }
                }
            }
            return result;
        }



    }
}

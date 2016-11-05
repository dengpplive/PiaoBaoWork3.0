using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PbProject.Model;
using PbProject.Dal.Mapping;
using PnrAnalysis.Model;
using DataBase.Data;
using PbProject.Logic.Pay;
using System.Data;
using PnrAnalysis;
namespace PbProject.Logic.Order
{
    public class Tb_Ticket_OrderBLL
    {
        PbProject.Logic.ControlBase.BaseDataManage baseDataManage = new PbProject.Logic.ControlBase.BaseDataManage();
        private SqlHelper sqlHelper = new SqlHelper();

        #region 获取订单

        /// <summary>
        /// 订单信息信息
        /// </summary>
        /// <param name="sqlWhere">查询条件</param>
        /// <returns></returns>
        public List<Tb_Ticket_Order> GetListBySqlWhere(string sqlWhere)
        {
            List<Tb_Ticket_Order> reList = baseDataManage.CallMethod("Tb_Ticket_Order", "GetList", null, new Object[] { sqlWhere }) as List<Tb_Ticket_Order>;
            return reList;
        }

        /// <summary>
        /// 通过订单号：获取订单信息
        /// </summary>
        /// <param name="orderId">订单号</param>
        /// <returns></returns>
        public Tb_Ticket_Order GetTicketOrderByOrderId(string orderId)
        {
            Tb_Ticket_Order order = null;

            try
            {
                if (!string.IsNullOrEmpty(orderId))
                {
                    string sqlWhere = "OrderId='" + orderId + "'";

                    List<Tb_Ticket_Order> reList = GetListBySqlWhere(sqlWhere);

                    if (reList != null && reList.Count > 0)
                    {
                        order = reList[0];
                    }
                }
            }
            catch (Exception ex)
            {
                PnrAnalysis.LogText.LogWrite("【时间:" + System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ff") + " 方法:Tb_Ticket_Order GetTicketOrderByOrderId(string orderId)】================================================================\r\n 异常信息:" + ex.Message + "\r\n订单号:" + orderId, "GetTicketOrderByOrderId");
            }
            return order;
        }

        /// <summary>
        /// 通过id：获取订单信息
        /// </summary>
        /// <param name="orderId">订单号</param>
        /// <returns></returns>
        public Tb_Ticket_Order GetTicketOrderById(string id)
        {
            Tb_Ticket_Order order = null;

            try
            {
                if (!string.IsNullOrEmpty(id))
                {
                    string sqlWhere = "id='" + id + "'";

                    List<Tb_Ticket_Order> reList = GetListBySqlWhere(sqlWhere);

                    if (reList != null && reList.Count > 0)
                    {
                        order = reList[0];
                    }
                }
            }
            catch (Exception ex)
            {
                PnrAnalysis.LogText.LogWrite("【时间:" + System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ff") + " 方法:Tb_Ticket_Order GetTicketOrderById(string id)】================================================================\r\n 异常信息:" + ex.Message + "\r\nID:" + id, "GetTicketOrderById");
            }
            return order;
        }
        /// <summary>
        /// 通过Sql语句获取数据
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public DataTable GetOrderBySql(string sql)
        {
            return new PbProject.Dal.SQLEXDAL.SQLEXDAL_Base().ExecuteStrSQL(sql);
        }

        #endregion

        #region 取消订单

        /// <summary>
        /// 取消订单
        /// </summary>
        /// <param name="mOrder">订单Model</param>
        /// <param name="mUser">用户Model</param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public bool CancelOrder(Tb_Ticket_Order mOrder, User_Employees mUser, User_Company mCompany, string msg)
        {
            bool result = false;
            List<string> sqlList = new List<string>();
            try
            {
                #region 1.修改订单状态
                string tempSql = "update Tb_Ticket_Order set A1=1,OrderStatusCode=2,TicketStatus=6  where id='" + mOrder.id + "'";
                sqlList.Add(tempSql);//1.添加订单日志
                #endregion

                #region 2.修改乘机人状态
                tempSql = "update Tb_Ticket_Passenger set TicketStatus=6  where OrderId='" + mOrder.OrderId + "'";
                sqlList.Add(tempSql);//1.添加订单日志
                #endregion

                #region 3.订单日志

                Log_Tb_AirOrder OrderLog = new Log_Tb_AirOrder();
                OrderLog.id = Guid.NewGuid();
                OrderLog.OperContent = msg;
                OrderLog.OperLoginName = mUser.LoginName;
                OrderLog.OperTime = DateTime.Now;
                OrderLog.OperType = "修改";
                OrderLog.OperUserName = mUser.UserName;
                OrderLog.OrderId = mOrder.OrderId;
                OrderLog.WatchType = mCompany.RoleType;
                OrderLog.CpyName = mCompany.UninAllName;
                OrderLog.CpyNo = mCompany.UninCode;
                OrderLog.CpyType = mCompany.RoleType;
                OrderLog.WatchType = mCompany.RoleType;
                tempSql = Dal.Mapping.MappingHelper<Log_Tb_AirOrder>.CreateInsertModelSql(OrderLog);
                sqlList.Add(tempSql);//3.添加订单日志

                #endregion

                //添加到数据
                result = new Dal.ControlBase.BaseData<Tb_Ticket_Order>().ExecuteSqlTran(sqlList);
            }
            catch (Exception ex)
            {
                PnrAnalysis.LogText.LogWrite("【时间:" + System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ff") + " 方法:bool CancelOrder(Tb_Ticket_Order mOrder, User_Employees mUser, User_Company mCompany, string msg)】================================================================\r\n 异常信息:" + ex.Message + "\r\nSQL:" + string.Join("\r\n", sqlList.ToArray()), "CancelOrder");
            }
            return result;
        }

        #endregion

        #region  锁定/解锁 订单

        /// <summary>
        /// 锁定/解锁 订单
        /// </summary>
        /// <param name="type">类型:true 锁定，false 解锁</param>
        /// <param name="id">订单id</param>
        /// <param name="mUser"></param>
        /// <param name="mCompany"></param>
        /// <returns></returns>
        public bool LockOrder(bool type, string id, User_Employees mUser, User_Company mCompany)
        {
            bool reuslt = false;
            string ErrMsg = "";
            try
            {
                reuslt = LockOrder(type, id, mUser.LoginName, mCompany.UninCode, out ErrMsg);
            }
            catch (Exception ex)
            {
                ErrMsg = ex.Message;
                PnrAnalysis.LogText.LogWrite("【时间:" + System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ff") + " 方法:bool LockOrder(bool type, string id, User_Employees mUser, User_Company mCompany)】================================================================\r\n 异常信息:" + ErrMsg + "\r\n", "LockOrder");
            }
            return reuslt;
        }
        /// <summary>
        /// 订单锁定 解锁 操作
        /// </summary>
        /// <param name="lockType">0加锁 1解锁</param>
        /// <param name="OrderId">订单id</param>
        /// <param name="LockLoginName">锁定账号</param>
        /// <param name="LockCpyNo">锁定公司编号</param>
        /// <returns></returns>
        public bool LockOrder(bool lockType, string Id, string LockLoginName, string LockCpyNo, out string ErrMsg)
        {
            bool IsSuc = false;
            //记录参数日志
            List<string> sbParam = new List<string>();
            ErrMsg = "";
            try
            {
                sbParam.Add("lockType=" + lockType);
                sbParam.Add("Id=" + Id);
                sbParam.Add("LockLoginName=" + LockLoginName);
                sbParam.Add("LockCpyNo=" + LockCpyNo);

                IHashObject hash = new HashObject();
                hash.Add("id", Id);
                if (lockType)
                {
                    hash.Add("LockLoginName", LockLoginName);
                    hash.Add("LockCpyNo", LockCpyNo);
                    hash.Add("LockTime", System.DateTime.Now);
                }
                else
                {
                    hash.Add("LockLoginName", "");
                    hash.Add("LockCpyNo", "");
                    hash.Add("LockTime", System.DateTime.Parse("1900-01-01 00:00:00"));
                }
                IsSuc = (bool)this.baseDataManage.CallMethod("Tb_Ticket_Order", "Update", null, new object[] { hash });
            }
            catch (Exception ex)
            {
                ErrMsg = ex.Message;
                IsSuc = false;
                PnrAnalysis.LogText.LogWrite("【时间:" + System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ff") + " 方法:bool LockOrder(bool lockType, string Id, string LockLoginName, string LockCpyNo, out string ErrMsg)】================================================================\r\n 异常信息:" + ex.Message + "\r\n参数信息:\r\n" + string.Join("\r\n", sbParam.ToArray()), "GetTicketOrderById");
            }
            return IsSuc;
        }
        /// <summary>
        /// 批量锁定与解锁 
        /// </summary>
        /// <param name="lockType">true加锁 false解锁</param>
        /// <param name="Ids">订单</param>
        /// <param name="LockLoginName">锁定账号</param>
        /// <param name="LockCpyNo">锁定公司编号</param>
        /// <param name="ErrMsg">内部错误</param>
        /// <returns></returns>
        public bool PatchLockOrder(bool lockType, List<string> Ids, string LockLoginName, string LockCpyNo, out string ErrMsg)
        {
            string UpdateFileds = "";
            bool IsSuc = false;
            ErrMsg = "";
            //记录参数日志
            List<string> sbParam = new List<string>();
            try
            {
                //记录日志
                sbParam.Add("lockType=" + lockType);
                sbParam.Add("Ids=" + string.Join(",", Ids));
                sbParam.Add("LockLoginName=" + LockLoginName);
                sbParam.Add("LockCpyNo=" + LockCpyNo);

                if (lockType)
                {
                    UpdateFileds = string.Format("LockLoginName='{0}',LockCpyNo='{1}',LockTime='{2}'", LockLoginName, LockCpyNo, System.DateTime.Now);
                }
                else
                {
                    UpdateFileds = string.Format("LockLoginName='',LockCpyNo='',LockTime='{0}'", System.DateTime.Parse("1900-01-01 00:00:00"));
                }
                IsSuc = (bool)this.baseDataManage.CallMethod("Tb_Ticket_Order", "UpdateByIds", null, new object[] { Ids, UpdateFileds });
            }
            catch (Exception ex)
            {
                ErrMsg = ex.Message;
                IsSuc = false;
                PnrAnalysis.LogText.LogWrite("【时间:" + System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ff") + " 方法: bool PatchLockOrder(bool lockType, List<string> Ids, string LockLoginName, string LockCpyNo, out string ErrMsg)】================================================================\r\n 异常信息:" + ex.Message + "\r\n参数信息:\r\n" + string.Join("\r\n", sbParam.ToArray()), "PatchLockOrder");
            }
            return IsSuc;
        }
        #endregion

        #region 创建 订单号、内部流水号

        /// <summary>
        /// 创建订单号
        /// </summary>
        /// <param name="no">类型:0 机票支付 ,1 充值支付 ，2 短信支付,3预存款充值代扣</param>
        /// <returns></returns>
        public string GetOrderId(string no)
        {
            return new PbProject.Logic.SQLEXBLL.SQLEXBLL_Base().GetNewOrderId(no);
        }

        /// <summary>
        /// 创建内部流水号 / 本地内部流水号
        /// </summary>
        /// <returns></returns>
        public string GetIndexId()
        {
            return DateTime.Now.Ticks.ToString();
        }

        #endregion

        #region 订单创建 更新 获取 操作

        /// <summary>
        /// 是否存在指定条件的订单
        /// </summary>
        /// <param name="pnr"></param>
        /// <param name="TongDao"></param>
        /// <returns></returns>
        public bool IsExist(string sqlWhere)
        {
            return (bool)baseDataManage.CallMethod("Tb_Ticket_Order", "IsExist", null, new object[] { sqlWhere });
        }

        /// <summary>
        /// 生成订单方法
        /// </summary>
        /// <param name="skyList"></param>
        /// <param name="pasList"></param>
        /// <param name="order"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public bool CreateOrder(ref OrderInputParam paramList, out string ErrMsg)
        {
            bool IsSuc = false;
            ErrMsg = "";
            try
            {
                Bill bill = new Bill();
                List<string> RobList = new List<string>();
                for (int k = 0; k < paramList.OrderParamModel.Count; k++)
                {
                    OrderMustParamModel item = paramList.OrderParamModel[k];
                    item.Order.id = Guid.NewGuid();
                    //设置订单号
                    item.Order.OrderId = GetOrderId("0");
                    //创建内部流水号
                    item.Order.InPayNo = GetIndexId();
                    //设置编码
                    if (item.Order.IsChdFlag)
                    {
                        item.Order.PNR = paramList.PnrInfo.childPnr.ToUpper();
                        item.Order.BigCode = paramList.PnrInfo.childPnrToBigPNR.ToUpper();
                        //儿童出成人票 儿童PAT价格内容
                        if (item.Order.IsCHDETAdultTK == 1)
                        {
                            item.Order.CHDToAdultPat = paramList.PnrInfo.CHDToAdultPatCon;
                        }
                    }
                    else
                    {
                        item.Order.PNR = paramList.PnrInfo.AdultPnr.ToUpper();
                        item.Order.BigCode = paramList.PnrInfo.AdultPnrToBigPNR.ToUpper();
                        //有婴儿时存储婴儿PAT数据
                        if (item.Order.HaveBabyFlag)
                        {
                            item.Order.BabyPatContent = paramList.PnrInfo.PatList[2];
                        }
                    }
                    //设置预订Office
                    item.Order.Office = paramList.PnrInfo.Office.ToUpper();
                    //航空公司出票Office
                    item.Order.PrintOffice = paramList.PnrInfo.PrintOffice.ToUpper();
                    item.Order.CreateTime = System.DateTime.Now;
                    //乘机人sql
                    for (int i = 0; i < item.PasList.Count; i++)
                    {
                        Tb_Ticket_Passenger pas = item.PasList[i];
                        pas.id = Guid.NewGuid();
                        //设置订单号
                        pas.OrderId = item.Order.OrderId;
                        paramList.ExecSQLList.Add(PbProject.Dal.Mapping.MappingHelper<Tb_Ticket_Passenger>.CreateInsertModelSql(pas));
                    }
                    //航段sql
                    for (int j = 0; j < item.SkyList.Count; j++)
                    {
                        Tb_Ticket_SkyWay SkyWay = item.SkyList[j];
                        SkyWay.id = Guid.NewGuid();
                        //设置订单号
                        SkyWay.OrderId = item.Order.OrderId;
                        if (item.Order.IsChdFlag)
                        {
                            //儿童RT内容   
                            SkyWay.PnrContent = paramList.PnrInfo.childPnrRTContent;
                            //非PNR内容导入
                            if (item.Order.OrderSourceType != 3 && item.Order.OrderSourceType != 7 && item.Order.OrderSourceType != 9)
                            {
                                SkyWay.NewPnrContent = paramList.PnrInfo.childPnrRTContent;
                            }
                            else
                            {
                                SkyWay.NewPnrContent = paramList.PnrInfo.HandleRTContent;
                            }
                            //儿童Pat内容
                            SkyWay.Pat = paramList.PnrInfo.PatList[1];

                            #region 儿童基建或者燃油为0时处理
                            if (SkyWay.Pat != "" && paramList.PnrInfo.PatModelList.Length >= 1 && paramList.PnrInfo.PatModelList[1] != null)
                            {
                                List<PatInfo> PatList = paramList.PnrInfo.PatModelList[1].UninuePatList;
                                decimal m_Fare = 0m, m_TAX = 0m, m_RQFare = 0m;
                                bool IsUpdatePAT = false;
                                foreach (PatInfo pat in PatList)
                                {
                                    decimal.TryParse(pat.Fare, out m_Fare);
                                    decimal.TryParse(pat.TAX, out m_TAX);
                                    decimal.TryParse(pat.RQFare, out m_RQFare);
                                    //基建或者燃油为0
                                    if (m_Fare != 0 && (m_TAX == 0 || m_RQFare == 0))
                                    {
                                        IsUpdatePAT = true;
                                        break;
                                    }
                                }
                                if (IsUpdatePAT)
                                {
                                    decimal new_TAX = 0m, new_RQFare = 0m;
                                    //儿童
                                    string sqlWhere = string.Format(" FromCityCode='{0}' and ToCityCode='{1}' and PersonType=2 ", SkyWay.FromCityCode, SkyWay.ToCityCode);
                                    List<Tb_Air_BuildOilInfo> lstBuildOI = baseDataManage.CallMethod("Tb_Air_BuildOilInfo", "GetList", null, new object[] { sqlWhere }) as List<Tb_Air_BuildOilInfo>;
                                    //有设置生僻航线时的基建和燃油
                                    if (lstBuildOI != null && lstBuildOI.Count > 0)
                                    {
                                        item.Order.IsUpdatePAT = 1;
                                        Tb_Air_BuildOilInfo TBBI = lstBuildOI[0];
                                        new_TAX = TBBI.BuildPrice;
                                        new_RQFare = TBBI.OilPrice;
                                        SkyWay.OldPAT = paramList.PnrInfo.PatList[1];
                                        //原来的基建燃油
                                        SkyWay.Pat = paramList.PnrInfo.PatModelList[1].UpdatePrice(new_TAX.ToString(), new_RQFare.ToString());
                                    }
                                }
                            }
                            #endregion

                            //航站楼
                            if (paramList.PnrInfo.PnrList[1] != null && paramList.PnrInfo.PnrList[1]._LegList.Count == item.SkyList.Count && string.IsNullOrEmpty(SkyWay.Terminal))
                            {
                                SkyWay.Terminal = paramList.PnrInfo.PnrList[1]._LegList[j].FromCityT1 + paramList.PnrInfo.PnrList[1]._LegList[j].ToCityT2;
                            }
                        }
                        else
                        {
                            //成人RT内容    
                            SkyWay.PnrContent = paramList.PnrInfo.AdultPnrRTContent;
                            //非PNR内容导入
                            if (item.Order.OrderSourceType != 3 && item.Order.OrderSourceType != 7 && item.Order.OrderSourceType != 9)
                            {
                                SkyWay.NewPnrContent = paramList.PnrInfo.AdultPnrRTContent;
                            }
                            else
                            {
                                SkyWay.NewPnrContent = paramList.PnrInfo.HandleRTContent;
                            }
                            //成人Pat内容
                            SkyWay.Pat = paramList.PnrInfo.PatList[0];
                            #region 成人基建或者燃油为0时处理
                            if (SkyWay.Pat != "" && paramList.PnrInfo.PatModelList.Length >= 1 && paramList.PnrInfo.PatModelList[0] != null)
                            {
                                List<PatInfo> PatList = paramList.PnrInfo.PatModelList[0].UninuePatList;
                                decimal m_Fare = 0m, m_TAX = 0m, m_RQFare = 0m;
                                bool IsUpdatePAT = false;
                                foreach (PatInfo pat in PatList)
                                {
                                    decimal.TryParse(pat.Fare, out m_Fare);
                                    decimal.TryParse(pat.TAX, out m_TAX);
                                    decimal.TryParse(pat.RQFare, out m_RQFare);
                                    //基建或者燃油为0
                                    if (m_Fare != 0 && (m_TAX == 0 || m_RQFare == 0))
                                    {
                                        IsUpdatePAT = true;
                                        break;
                                    }
                                }
                                if (IsUpdatePAT)
                                {
                                    decimal new_TAX = 0m, new_RQFare = 0m;
                                    //成人
                                    string sqlWhere = string.Format(" FromCityCode='{0}' and ToCityCode='{1}' and PersonType=1", SkyWay.FromCityCode, SkyWay.ToCityCode);
                                    List<Tb_Air_BuildOilInfo> lstBuildOI = baseDataManage.CallMethod("Tb_Air_BuildOilInfo", "GetList", null, new object[] { sqlWhere }) as List<Tb_Air_BuildOilInfo>;
                                    //有设置生僻航线时的基建和燃油
                                    if (lstBuildOI != null && lstBuildOI.Count > 0)
                                    {
                                        item.Order.IsUpdatePAT = 1;
                                        Tb_Air_BuildOilInfo TBBI = lstBuildOI[0];
                                        new_TAX = TBBI.BuildPrice;
                                        new_RQFare = TBBI.OilPrice;
                                        SkyWay.OldPAT = paramList.PnrInfo.PatList[0];
                                        //原来的基建燃油
                                        SkyWay.Pat = paramList.PnrInfo.PatModelList[0].UpdatePrice(new_TAX.ToString(), new_RQFare.ToString());
                                    }
                                }
                            }
                            #endregion
                            //航站楼
                            if (paramList.PnrInfo.PnrList[0] != null && paramList.PnrInfo.PnrList[0]._LegList.Count == item.SkyList.Count && string.IsNullOrEmpty(SkyWay.Terminal))
                            {
                                SkyWay.Terminal = paramList.PnrInfo.PnrList[0]._LegList[j].FromCityT1 + paramList.PnrInfo.PnrList[0]._LegList[j].ToCityT2;
                            }
                        }
                        paramList.ExecSQLList.Add(PbProject.Dal.Mapping.MappingHelper<Tb_Ticket_SkyWay>.CreateInsertModelSql(SkyWay));
                    }
                    item.LogOrder.id = Guid.NewGuid();
                    //设置订单号
                    item.LogOrder.OrderId = item.Order.OrderId;
                    //订单日志sql
                    paramList.ExecSQLList.Add(PbProject.Dal.Mapping.MappingHelper<Log_Tb_AirOrder>.CreateInsertModelSql(item.LogOrder));
                    //添加订单账单明细sql                       
                    List<string> sqlList = bill.CreateOrderAndTicketPayDetailNew(item.Order, item.PasList);
                    //订单sql
                    paramList.ExecSQLList.Add(PbProject.Dal.Mapping.MappingHelper<Tb_Ticket_Order>.CreateInsertModelSql(item.Order));
                    if (paramList.IsCreatePayDetail == 1)
                    {
                        if (sqlList.Count > 0)
                        {
                            paramList.ExecSQLList.AddRange(sqlList.ToArray());
                        }
                    }
                    //抢票订单号 更新到原订单中
                    if (paramList.IsRobTicketOrder && paramList.OldRobTicketOrderId != "")
                    {
                        RobList.Add(string.Format(" update Tb_Ticket_Order set RobOrderId='{0}' where OrderId='{1}' ", paramList.OldRobTicketOrderId, item.Order.OrderId));
                    }
                }
                //执行事务
                IsSuc = sqlHelper.ExecuteSqlTran(paramList.ExecSQLList, out ErrMsg);
                if (IsSuc && paramList.IsRobTicketOrder && RobList.Count > 0)
                {
                    bool RobIsSuc = sqlHelper.ExecuteSqlTran(RobList, out ErrMsg);
                    if (!RobIsSuc)
                    {
                        PnrAnalysis.LogText.LogWrite("【时间:" + System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ff") + " 方法: public bool CreateOrder(ref OrderInputParam paramList, out string ErrMsg)】================================================================\r\n 抢票订单异常信息:" + ErrMsg + "\r\nSQL:" + string.Join("\r\n", RobList.ToArray()), "SQL\\CreateOrder");
                    }
                }
            }
            catch (Exception ex)
            {
                IsSuc = false;
                ErrMsg += ex.Message;
                PnrAnalysis.LogText.LogWrite("【时间:" + System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ff") + " 方法: public bool CreateOrder(ref OrderInputParam paramList, out string ErrMsg)】================================================================\r\n 异常信息:" + ErrMsg + "\r\nSQL:" + string.Join("\r\n", paramList.ExecSQLList.ToArray()), "SQL\\CreateOrder");
            }
            return IsSuc;
        }

        /// <summary>
        /// 修改和订单有关的实体信息 实体中必须都含有OrderId
        /// </summary>
        /// <param name="paramList"></param>
        /// <param name="ErrMsg"></param>
        /// <returns></returns>
        public bool UpdateOrder(ref OrderInputParam paramList, out string ErrMsg)
        {
            bool IsSuc = false;
            ErrMsg = "";
            try
            {
                for (int k = 0; k < paramList.OrderParamModel.Count; k++)
                {
                    OrderMustParamModel item = paramList.OrderParamModel[k];
                    //更新订单sql
                    paramList.ExecSQLList.Add(PbProject.Dal.Mapping.MappingHelper<Tb_Ticket_Order>.CreateUpdateModelSql(item.Order, item.UpdateOrderFileds, new string[] { "OrderId", "id" }));
                    //乘机人sql
                    for (int i = 0; i < item.PasList.Count; i++)
                    {
                        Tb_Ticket_Passenger pas = item.PasList[i];
                        paramList.ExecSQLList.Add(PbProject.Dal.Mapping.MappingHelper<Tb_Ticket_Passenger>.CreateUpdateModelSql(pas, item.UpdateOrderFileds, new string[] { "OrderId", "id" }));
                    }
                    //航段sql
                    for (int j = 0; j < item.SkyList.Count; j++)
                    {
                        Tb_Ticket_SkyWay SkyWay = item.SkyList[j];
                        paramList.ExecSQLList.Add(PbProject.Dal.Mapping.MappingHelper<Tb_Ticket_SkyWay>.CreateUpdateModelSql(SkyWay, item.UpdateOrderFileds, new string[] { "OrderId", "id" }));
                    }
                }
                //执行事务
                IsSuc = sqlHelper.ExecuteSqlTran(paramList.ExecSQLList, out ErrMsg);
            }
            catch (Exception ex)
            {
                IsSuc = false;
                ErrMsg += ex.Message;
                PnrAnalysis.LogText.LogWrite("【时间:" + System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ff") + " 方法:  public bool UpdateOrder(ref OrderInputParam paramList, out string ErrMsg)】================================================================\r\n 异常信息:" + ErrMsg + "\r\nSQL:" + string.Join("\r\n", paramList.ExecSQLList.ToArray()), "SQL\\UpdateOrder");
            }
            return IsSuc;
        }

        /// <summary>
        /// 获取有关该订单号的所有实体信息
        /// </summary>
        /// <param name="OrderId"></param>
        /// <param name="?"></param>
        /// <returns></returns>
        public OrderInputParam GetOrder(string OrderId, OrderInputParam orderinputparam, out string ErrMsg)
        {
            ErrMsg = "";
            if (orderinputparam == null)
            {
                orderinputparam = new OrderInputParam();
            }
            try
            {
                OrderMustParamModel ordermustparammodel = new OrderMustParamModel();
                List<Tb_Ticket_Order> OrderList = baseDataManage.CallMethod("Tb_Ticket_Order", "GetList", null, new object[] { string.Format("OrderId='{0}'", OrderId) }) as List<Tb_Ticket_Order>;
                ordermustparammodel.PasList = baseDataManage.CallMethod("Tb_Ticket_Passenger", "GetList", null, new object[] { string.Format(" OrderId='{0}' order by PassengerType ", OrderId) }) as List<Tb_Ticket_Passenger>;
                ordermustparammodel.SkyList = baseDataManage.CallMethod("Tb_Ticket_SkyWay", "GetList", null, new object[] { string.Format("OrderId='{0}' order by FromDate ", OrderId) }) as List<Tb_Ticket_SkyWay>;
                List<Log_Tb_AirOrder> LogList = baseDataManage.CallMethod("Log_Tb_AirOrder", "GetList", null, new object[] { string.Format("OrderId='{0}'", OrderId) }) as List<Log_Tb_AirOrder>;
                if (OrderList != null && OrderList.Count > 0)
                {
                    Tb_Ticket_Order Order = OrderList[0];
                    ordermustparammodel.Order = Order;
                    if (orderinputparam.PnrInfo == null)
                    {
                        orderinputparam.PnrInfo = new RePnrObj();
                    }
                    //Office
                    orderinputparam.PnrInfo.Office = Order.Office;
                    if (ordermustparammodel.SkyList != null && ordermustparammodel.SkyList.Count > 0)
                    {
                        foreach (Tb_Ticket_SkyWay skyway in ordermustparammodel.SkyList)
                        {
                            if (ordermustparammodel.Order.IsChdFlag)
                            {
                                //儿童订单
                                orderinputparam.PnrInfo.childPnr = Order.PNR;
                                orderinputparam.PnrInfo.childPnrToBigPNR = Order.BigCode;
                                //内容
                                orderinputparam.PnrInfo.childPnrRTContent = skyway.PnrContent;
                                if (ordermustparammodel.Order.OrderSourceType != 3)
                                {
                                    orderinputparam.PnrInfo.HandleRTContent = skyway.PnrContent;
                                }
                                else
                                {
                                    orderinputparam.PnrInfo.HandleRTContent = skyway.NewPnrContent;
                                }
                                orderinputparam.PnrInfo.PatList[1] = skyway.Pat;
                                //是否儿童出成人票
                                if (ordermustparammodel.Order.IsCHDETAdultTK == 1)
                                {
                                    orderinputparam.PnrInfo.CHDToAdultPatCon = ordermustparammodel.Order.CHDToAdultPat;
                                }
                            }
                            else
                            {
                                //成人订单
                                orderinputparam.PnrInfo.AdultPnr = Order.PNR;
                                orderinputparam.PnrInfo.AdultPnrToBigPNR = Order.BigCode;
                                //内容
                                orderinputparam.PnrInfo.AdultPnrRTContent = skyway.PnrContent;
                                if (ordermustparammodel.Order.OrderSourceType != 3)
                                {
                                    orderinputparam.PnrInfo.HandleRTContent = skyway.PnrContent;
                                }
                                else
                                {
                                    orderinputparam.PnrInfo.HandleRTContent = skyway.NewPnrContent;
                                }
                                orderinputparam.PnrInfo.PatList[0] = skyway.Pat;
                                if (ordermustparammodel.Order.HaveBabyFlag)
                                {
                                    //婴儿
                                    orderinputparam.PnrInfo.PatList[2] = ordermustparammodel.Order.BabyPatContent;
                                }
                            }
                        }
                    }
                    //转换到实体
                    PnrAnalysis.SendNewPID.ConToModel(orderinputparam.PnrInfo, out ErrMsg);
                }
                if (LogList != null && LogList.Count > 0)
                {
                    ordermustparammodel.LogOrder = LogList[0];
                }
                bool IsAdd = true;
                for (int i = 0; i < orderinputparam.OrderParamModel.Count; i++)
                {
                    if (orderinputparam.OrderParamModel[i].Order.OrderId.Trim() == OrderId.Trim())
                    {
                        IsAdd = false;
                        break;
                    }
                }
                if (IsAdd)
                {
                    orderinputparam.OrderParamModel.Add(ordermustparammodel);
                }
            }
            catch (Exception ex)
            {
                ErrMsg += ex.Message;
                PnrAnalysis.LogText.LogWrite("【时间:" + System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ff") + " 方法:OrderInputParam GetOrder(string OrderId, OrderInputParam orderinputparam, out string ErrMsg)】================================================================\r\n 异常信息:" + ex.Message + "\r\n订单号:\r\n" + OrderId, "GetOrder");
            }
            orderinputparam.ErrMsg = ErrMsg;
            return orderinputparam;
        }

        /// <summary>
        /// 修改订单信息：平台修改订单数据使用
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="logAirOrder"></param>
        /// <returns></returns>
        public bool UpdateOrder(List<string> sqlList, Log_Tb_AirOrder logAirOrder)
        {
            bool result = false;
            try
            {
                string tempSql = "";
                tempSql = Dal.Mapping.MappingHelper<Log_Tb_AirOrder>.CreateInsertModelSql(logAirOrder);
                sqlList.Add(tempSql);

                //添加到数据
                result = new Dal.ControlBase.BaseData<Tb_Ticket_Order>().ExecuteSqlTran(sqlList);
            }
            catch (Exception ex)
            {
                PnrAnalysis.LogText.LogWrite("【时间:" + System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ff") + " 方法: bool UpdateOrder(string sql, Log_Tb_AirOrder logAirOrder)】================================================================\r\n 异常信息:" + ex.Message + "\r\nSQL:\r\n" + string.Join("\r\n", sqlList.ToArray()), "UpdateOrder");
            }
            return result;
        }

        #endregion

        #region 退废处理、改签处理

        /// <summary>
        /// 申请退、废、改
        /// </summary>
        /// <param name="newOrder">新订单 model</param>
        /// <param name="newPassengerList">新乘机人 model list</param>
        /// <param name="passengerId">旧乘机人 model list</param>
        /// <param name="newSkyWayList">新航段 model list</param>
        /// <param name="mUser">用户 model</param>
        /// <param name="mCompany">公司 model</param>
        /// <param name="typeVal">退票时:自愿、非自愿</param>
        /// <param name="content">保留编码日志</param>
        /// <param name="ErrMsg"></param>
        /// <returns></returns>
        public bool CreateOrderTFG(Tb_Ticket_Order newOrder, List<Tb_Ticket_Passenger> newPassengerList, List<string> passengerId, List<Tb_Ticket_SkyWay> newSkyWayList,
            User_Employees mUser, User_Company mCompany, string typeVal, string content, out string ErrMsg)
        {
            bool result = false;
            ErrMsg = "";

            #region 验证重复提交

            if (newPassengerList != null && newPassengerList.Count > 0)
            {
                string pasSqlWhere = " ID in (";
                foreach (var item in passengerId)
                {
                    pasSqlWhere += string.Format("'{0}',", item.ToString());
                }
                pasSqlWhere = pasSqlWhere.TrimEnd(',') + ") and IsBack=0";
                var list = baseDataManage.CallMethod("Tb_Ticket_Passenger", "GetList", null, new Object[] { pasSqlWhere }) as List<Tb_Ticket_Passenger>;
                if (list == null || list.Count == 0)
                {
                    ErrMsg = "申请失败,不能重复提交!";
                    return result;
                }
            }

            #endregion

            //添加 sql 语句
            List<string> sqlList = new List<string>();
            try
            {
                string newOrderId = newOrder.OrderId; //新订单号             
                // 1.添加新订单信息
                string tempSql = "";

                Data data = new Data(mCompany.UninCode);

                //newOrder.PayMoney = data.CreateOrderPayMoney(newOrder, newPassengerList);  //
                //newOrder.OrderMoney = data.CreateOrderOrderMoney(newOrder, newPassengerList);  //
                Bill bill = new Bill();
                List<string> ticketPayDetailList = bill.CreateOrderAndTicketPayDetailNew(newOrder, newPassengerList);



                newOrder.A3 = 0;
                newOrder.A13 = data.CreateOrderHFMoney(newOrder, newPassengerList);  //

                tempSql = Dal.Mapping.MappingHelper<PbProject.Model.Tb_Ticket_Order>.CreateInsertModelSql(newOrder);
                sqlList.Add(tempSql);

                // 2.添加新乘机人信息
                foreach (Tb_Ticket_Passenger passenger in newPassengerList)
                {
                    passenger.OrderId = newOrderId;
                    passenger.TicketStatus = newOrder.TicketStatus;

                    tempSql = Dal.Mapping.MappingHelper<PbProject.Model.Tb_Ticket_Passenger>.CreateInsertModelSql(passenger);
                    sqlList.Add(tempSql);
                }
                // 3.添加新航段信息
                foreach (Tb_Ticket_SkyWay skyWay in newSkyWayList)
                {
                    skyWay.OrderId = newOrderId;

                    tempSql = Dal.Mapping.MappingHelper<PbProject.Model.Tb_Ticket_SkyWay>.CreateInsertModelSql(skyWay);
                    sqlList.Add(tempSql);
                }


                if (newOrder.TicketStatus == 3)
                {
                    if (string.IsNullOrEmpty(typeVal))
                        content = "申请退票" + content;
                    else
                        content = "申请退票" + "(" + typeVal + ")" + content;
                }
                else if (newOrder.TicketStatus == 4)
                    content = "申请废票" + content;
                else if (newOrder.TicketStatus == 5)
                    content = "申请改签" + content;

                #region 4.添加订单日志

                Log_Tb_AirOrder OrderLog = new Log_Tb_AirOrder();
                OrderLog.id = Guid.NewGuid();
                OrderLog.OrderId = newOrder.OrderId;
                OrderLog.OperType = "修改";
                OrderLog.OperTime = DateTime.Now;
                OrderLog.OperLoginName = mUser.LoginName;
                OrderLog.OperUserName = mUser.UserName;
                OrderLog.CpyNo = mCompany.UninCode;
                OrderLog.CpyType = mCompany.RoleType;
                OrderLog.CpyName = mCompany.UninAllName;
                OrderLog.OperContent = content;
                OrderLog.WatchType = mCompany.RoleType;

                tempSql = Dal.Mapping.MappingHelper<Log_Tb_AirOrder>.CreateInsertModelSql(OrderLog);

                sqlList.Add(tempSql);

                #endregion

                #region 5.修改原乘机人 isback  改签不修改 isback , 修改时间：2013-06-18 修改改签也只能提交一次

                //if (newOrder.OrderStatusCode != 6)
                //{
                string ids = "";
                for (int i = 0; i < passengerId.Count; i++)
                {
                    ids += "'" + passengerId[i] + "',";
                }
                ids = ids.TrimEnd(',');
                tempSql = "update dbo.Tb_Ticket_Passenger set IsBack=1 where id in(" + ids + ")";

                sqlList.Add(tempSql);
                //}

                #endregion

                #region 6.生成 Tb_Order_PayDetail 数据 、Tb_Ticket_Order 数据

                // Bill bill = new Bill();
                // List<string> ticketPayDetailList = bill.CreateOrderAndTicketPayDetail(newOrder, newPassengerList);
                // List<string> ticketPayDetailList = bill.CreateOrderAndTicketPayDetailNew(newOrder, newPassengerList);

                if (ticketPayDetailList != null && ticketPayDetailList.Count > 0)
                {
                    foreach (string item in ticketPayDetailList)
                    {
                        sqlList.Add(item);
                    }
                }

                #endregion

                //添加到数据
                result = new Dal.ControlBase.BaseData<Tb_Ticket_Order>().ExecuteSqlTran(sqlList);
            }
            catch (Exception ex)
            {
                ErrMsg += ex.Message;
                PnrAnalysis.LogText.LogWrite("【时间:" + System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ff") + " 方法:bool CreateOrderTFG(Tb_Ticket_Order newOrder, List<Tb_Ticket_Passenger> newPassengerList, List<string> passengerId, List<Tb_Ticket_SkyWay> newSkyWayList,User_Employees mUser, User_Company mCompany, string typeVal, out string ErrMsg)】================================================================\r\n 异常信息:" + ErrMsg + "\r\nSQL:\r\n" + string.Join("\r\n", sqlList.ToArray()), "CreateOrderTFG");
            }
            return result;
        }


        /// <summary>
        /// 处理退、废、改 
        /// </summary>
        /// <param name="mOrder">订单 model</param>
        /// <param name="PassengerList">乘机人 model list</param>
        /// <param name="mUser">用户 model</param>
        /// <param name="mCompany">公司 model<</param>
        /// <returns></returns>
        public bool OperOrderTFG(Tb_Ticket_Order mOrder, List<Tb_Ticket_Passenger> PassengerList, User_Employees mUser, User_Company mCompany, string content)
        {
            bool result = false;
            //SQL语句列表
            List<string> sqlList = new List<string>();
            Tb_Ticket_Order OldOrder = null;
            try
            {


                string Content = new PbProject.Logic.ControlBase.Bd_Base_DictionaryBLL().GetDictionaryName("1", mOrder.OrderStatusCode.ToString());

                if (!string.IsNullOrEmpty(content))
                    Content += "." + content;

                #region 参数

                PbProject.Logic.ControlBase.BaseDataManage baseDataManage = new PbProject.Logic.ControlBase.BaseDataManage();
                DataAction data = new DataAction();
                Data d = new Data(mOrder.OwnerCpyNo);
                Tb_Order_PayDetail tempOrderPayDetail = null;
                Tb_Order_PayDetail tempOrderPayDetailSXF = null;

                Tb_Ticket_PayDetail tempTicketPayDetail = null;
                Tb_Ticket_PayDetail tempTicketPayDetailSXF = null;

                string tempSql = "";
                string sqlWhere = " OrderId='" + mOrder.OrderId + "'";

                decimal tempPrice = 0;
                decimal Rate = mOrder.HandlingRate; //支付手续费费率
                decimal allFzMoney = 0; //分账金额之和

                decimal tempKouDian = 0;//扣点数
                decimal tempXianFan = 0;//现返

                decimal AllPayMoney = 0;
                decimal AllRealPayMoney = 0;
                decimal orderRealPayMoney = 0;

                int watchType = 0;

                #endregion

                mOrder.LockCpyNo = "";
                mOrder.LockLoginName = "";
                mOrder.LockTime = DateTime.Parse("1900-1-1");


                if (mOrder.OrderStatusCode == 9 || mOrder.OrderStatusCode == 19)
                {
                    //改签
                    watchType = 5;
                    if (mOrder.OrderStatusCode == 19)
                    { 
                        #region 改签成功还原 原支付订单ISback
                        
                        string PassengerNameS = "";

                        foreach (Tb_Ticket_Passenger item in PassengerList)
                        {
                            PassengerNameS += "'" + item.PassengerName + "',";
                        }
                        PassengerNameS = PassengerNameS.TrimEnd(',');

                        if (PassengerNameS != "")
                        {
                            tempSql = "  update Tb_Ticket_Passenger set IsBack=0 where OrderId = '" + mOrder.OldOrderId + "' and PassengerName in(" + PassengerNameS + ")";

                            sqlList.Add(tempSql);
                        }

                        #endregion
                    }
                }
                else
                {
                    watchType = mCompany.RoleType;

                    #region 重新计算订单金额（必须重新计算） （问题：处理中间状态时，会导致多次减去手续费）

                    new PbProject.Logic.Pay.Bill().CreateOrderAndTicketPayDetailNew(mOrder, PassengerList);

                    #endregion

                    mOrder.PayMoney = mOrder.PayMoney - mOrder.TGQHandlingFee;
                    mOrder.OrderMoney = mOrder.OrderMoney - mOrder.TGQHandlingFee;

                    mOrder.HandlingMoney = data.FourToFiveNum(mOrder.PayMoney * Rate, 2); //手续费
                }


                //修改订单 状态 和 添加日志
                #region 1.修改订单

                StringBuilder updateOrder = new StringBuilder();
                updateOrder.Append(" update Tb_Ticket_Order set ");
                updateOrder.Append(" LockCpyNo='',");//锁定帐号所属公司编号
                updateOrder.Append(" LockLoginName='',");//锁定帐户
                updateOrder.Append(" LockTime='1900-1-1',");//锁定时间

                updateOrder.Append(" CPTime='" + DateTime.Now.ToString() + "',");//时间 处理时间
                updateOrder.Append(" CPLoginName='" + mUser.LoginName + "',");//出票人登录帐号
                updateOrder.Append(" CPName='" + mUser.UserName + "',");//出票人姓名
                //updateOrder.Append(" CPCpyNo='" + mCompany.UninCode + "',");//出票人公司编号
                updateOrder.Append(" CPCpyName='" + mCompany.UninAllName + "',");//出票公司名称
               
                if (mOrder.TicketStatus == 3 || mOrder.TicketStatus == 4)
                {
                    updateOrder.Append(" A4='" + mOrder.A4.ToString() + "',");//航空公司退款时间
                    updateOrder.Append(" A6='" + mOrder.A6 + "',");//航空公司退款时间
                }

                updateOrder.Append(" HandlingMoney='" + mOrder.HandlingMoney + "',");
                updateOrder.Append(" TGQRefusalReason='" + mOrder.TGQRefusalReason + "',");
                updateOrder.Append(" TGQHandlingFee=" + mOrder.TGQHandlingFee + ",");//退改签手续费
                updateOrder.Append(" PayMoney=" + mOrder.PayMoney + ",");//实付金额
                updateOrder.Append(" OrderMoney=" + mOrder.OrderMoney + ",");//

                updateOrder.Append(" OrderStatusCode=" + mOrder.OrderStatusCode);  //操作类型即要修改的订单状态
                updateOrder.Append(" where " + sqlWhere);

                tempSql = updateOrder.ToString();
                sqlList.Add(tempSql);

                #endregion

                #region 2.修改手续费

                foreach (Tb_Ticket_Passenger item in PassengerList)
                {
                    tempSql = "update Tb_Ticket_Passenger set TGQHandlingFee=" + item.TGQHandlingFee + " where id='" + item.id + "'";
                    sqlList.Add(tempSql);
                }

                #endregion

                #region 3.添加订单日志

                Log_Tb_AirOrder OrderLog = new Log_Tb_AirOrder();
                OrderLog.id = Guid.NewGuid();
                OrderLog.OrderId = mOrder.OrderId;
                OrderLog.OperType = "修改";
                OrderLog.OperTime = DateTime.Now;
                OrderLog.OperLoginName = mUser.LoginName;
                OrderLog.OperUserName = mUser.UserName;
                OrderLog.CpyNo = mCompany.UninCode;
                OrderLog.CpyType = mCompany.RoleType;
                OrderLog.CpyName = mCompany.UninAllName;
                OrderLog.OperContent = Content;
                OrderLog.WatchType = watchType;

                tempSql = Dal.Mapping.MappingHelper<Log_Tb_AirOrder>.CreateInsertModelSql(OrderLog);
                sqlList.Add(tempSql);

                #endregion

                List<Tb_Order_PayDetail> OrderPayDetailList = null;
                List<Tb_Ticket_PayDetail> TicketPayDetailList = null;

                #region 快钱手续费加载信息

                if (mOrder.PayWay == 2 || mOrder.PayWay == 6)
                {
                    string sqlW = string.Format(" OrderID='{0}'", mOrder.OldOrderId);
                    // 原订单 GetById 错误，GetById 必须用id

                    //OldOrder = baseDataManage.CallMethod("Tb_Ticket_Order", "GetById", null, new object[] { sqlW }) as Tb_Ticket_Order;

                    OldOrder = (baseDataManage.CallMethod("Tb_Ticket_Order", "GetList", null, new Object[] { sqlW }) as List<Tb_Ticket_Order>)[0];
                }

                #endregion

                if (mOrder.OrderStatusCode == 10 || mOrder.OrderStatusCode == 12 || mOrder.OrderStatusCode == 14 || mOrder.OrderStatusCode == 18 || mOrder.OrderStatusCode == 24)
                {
                    #region 拒绝处理 ,还原原乘机人 isback

                    string PassengerNameS = "";

                    foreach (Tb_Ticket_Passenger item in PassengerList)
                    {
                        PassengerNameS += "'" + item.PassengerName + "',";
                    }
                    PassengerNameS = PassengerNameS.TrimEnd(',');

                    if (PassengerNameS != "")
                    {
                        tempSql = "  update Tb_Ticket_Passenger set IsBack=0 where OrderId = '" + mOrder.OldOrderId + "' and PassengerName in(" + PassengerNameS + ")";
                        sqlList.Add(tempSql);
                    }

                    #endregion
                }
                else if (mOrder.OrderStatusCode == 9 || mOrder.OrderStatusCode == 19)
                {
                    #region 改签 OrderPayDetailList
                    OrderPayDetailList = baseDataManage.CallMethod("Tb_Order_PayDetail", "GetList", null, new Object[] { sqlWhere }) as List<Tb_Order_PayDetail>;


                    for (int i = 0; i < OrderPayDetailList.Count; i++)
                    {
                        tempOrderPayDetail = OrderPayDetailList[i];
                        tempSql = "";

                        if (tempOrderPayDetail.PayType == "付款")
                        {
                            #region 付款
                            tempOrderPayDetail.PayMoney = mOrder.PayMoney;
                            tempOrderPayDetail.BuyPoundage = data.FourToFiveNum(tempOrderPayDetail.PayMoney * Rate, 2); //手续费
                            tempOrderPayDetail.RealPayMoney = tempOrderPayDetail.PayMoney - tempOrderPayDetail.BuyPoundage;//实际交易金额（实收实付）

                            #endregion
                        }
                        else if (tempOrderPayDetail.PayType == "收款")
                        {
                            #region 收款
                            tempOrderPayDetail.PayMoney = mOrder.PayMoney;
                            tempOrderPayDetail.BuyPoundage = data.FourToFiveNum(tempOrderPayDetail.PayMoney * Rate, 2); //手续费
                            tempOrderPayDetail.RealPayMoney = tempOrderPayDetail.PayMoney - tempOrderPayDetail.BuyPoundage;//实际交易金额（实收实付）

                            orderRealPayMoney = tempOrderPayDetail.RealPayMoney;

                            #endregion
                        }
                        else if (tempOrderPayDetail.PayType == "分账")
                        {
                            #region 分账
                            tempOrderPayDetail.PayMoney = 0;
                            tempOrderPayDetail.BuyPoundage = 0;
                            tempOrderPayDetail.RealPayMoney = 0;
                            #endregion
                        }
                        else if (tempOrderPayDetail.PayType == "手续费")
                        {
                            #region 手续费
                            tempOrderPayDetailSXF = tempOrderPayDetail;
                            tempOrderPayDetailSXF.PayMoney = 0;
                            continue;
                            #endregion
                        }

                        tempSql = " update Tb_Order_PayDetail set ";
                        tempSql += " PayMoney='" + tempOrderPayDetail.PayMoney + "',";
                        tempSql += " BuyPoundage='" + tempOrderPayDetail.BuyPoundage + "',";
                        tempSql += " RealPayMoney='" + tempOrderPayDetail.RealPayMoney + "'";
                        tempSql += " where id='" + tempOrderPayDetail.id.ToString() + "'";

                        sqlList.Add(tempSql);
                    }

                    //手续费
                    tempOrderPayDetailSXF.BuyPoundage = data.FourToFiveNum(mOrder.PayMoney * 0.001M, 2); //手续费
                    tempOrderPayDetailSXF.RealPayMoney = mOrder.PayMoney - tempOrderPayDetail.BuyPoundage - orderRealPayMoney;//实际交易金额（实收实付）

                    tempSql = " update Tb_Order_PayDetail set ";
                    tempSql += " PayMoney='" + tempOrderPayDetailSXF.PayMoney + "',";
                    tempSql += " BuyPoundage='" + tempOrderPayDetailSXF.BuyPoundage + "',";
                    tempSql += " RealPayMoney='" + tempOrderPayDetailSXF.RealPayMoney + "'";
                    tempSql += " where id='" + tempOrderPayDetailSXF.id.ToString() + "'";

                    sqlList.Add(tempSql);


                    #endregion

                    #region 改签  TicketPayDetailList

                    TicketPayDetailList = baseDataManage.CallMethod("Tb_Ticket_PayDetail", "GetList", null, new Object[] { sqlWhere }) as List<Tb_Ticket_PayDetail>;
                    List<string> strTemp = new PbProject.Logic.Pay.Bill().GetTicketIdByTicketPayDetail(TicketPayDetailList);

                    foreach (string ticketId in strTemp)
                    {
                        orderRealPayMoney = 0;

                        for (int i = 0; i < TicketPayDetailList.Count; i++)
                        {
                            tempTicketPayDetail = TicketPayDetailList[i];

                            if (tempTicketPayDetail.TicketId != ticketId)
                            {
                                continue;
                            }

                            tempSql = "";

                            if (tempTicketPayDetail.PayType == "付款")
                            {
                                // 付款
                                tempTicketPayDetail.PayMoney = mOrder.PayMoney;
                                tempTicketPayDetail.BuyPoundage = data.FourToFiveNum(tempOrderPayDetail.PayMoney * Rate, 2); //手续费
                                tempTicketPayDetail.RealPayMoney = tempTicketPayDetail.PayMoney - tempTicketPayDetail.BuyPoundage;//实际交易金额（实收实付）
                            }
                            else if (tempTicketPayDetail.PayType == "收款")
                            {
                                // 收款
                                tempTicketPayDetail.PayMoney = mOrder.PayMoney;
                                tempTicketPayDetail.BuyPoundage = data.FourToFiveNum(tempTicketPayDetail.PayMoney * Rate, 2); //手续费
                                tempTicketPayDetail.RealPayMoney = tempTicketPayDetail.PayMoney - tempTicketPayDetail.BuyPoundage;//实际交易金额（实收实付）
                                orderRealPayMoney = tempTicketPayDetail.RealPayMoney;
                            }
                            else if (tempTicketPayDetail.PayType == "分账")
                            {
                                // 分账
                                tempTicketPayDetail.PayMoney = 0;
                                tempTicketPayDetail.BuyPoundage = 0;
                                tempTicketPayDetail.RealPayMoney = 0;
                            }
                            else if (tempTicketPayDetail.PayType == "手续费")
                            {
                                // 手续费
                                tempTicketPayDetailSXF = tempTicketPayDetail;
                                tempTicketPayDetailSXF.PayMoney = 0;
                                continue;
                            }

                            tempSql = " update Tb_Order_PayDetail set ";
                            tempSql += " PayMoney='" + tempTicketPayDetail.PayMoney + "',";
                            tempSql += " BuyPoundage='" + tempTicketPayDetail.BuyPoundage + "',";
                            tempSql += " RealPayMoney='" + tempTicketPayDetail.RealPayMoney + "'";
                            tempSql += " where id='" + tempTicketPayDetail.id.ToString() + "'";

                            sqlList.Add(tempSql);
                        }

                        //手续费
                        tempTicketPayDetailSXF.BuyPoundage = data.FourToFiveNum(mOrder.PayMoney * 0.001M, 2); //手续费
                        tempTicketPayDetailSXF.RealPayMoney = mOrder.PayMoney - tempOrderPayDetail.BuyPoundage - orderRealPayMoney;//实际交易金额（实收实付）

                        tempSql = " update Tb_Ticket_PayDetail set ";
                        tempSql += " PayMoney='" + tempTicketPayDetailSXF.PayMoney + "',";
                        tempSql += " BuyPoundage='" + tempTicketPayDetailSXF.BuyPoundage + "',";
                        tempSql += " RealPayMoney='" + tempTicketPayDetailSXF.RealPayMoney + "'";
                        tempSql += " where id='" + tempTicketPayDetailSXF.id.ToString() + "'";

                        sqlList.Add(tempSql);
                    }

                    #endregion
                }
                else if (mOrder.PayWay == 15 && (mOrder.OrderStatusCode == 16 || mOrder.OrderStatusCode == 17))
                {

                }
                else
                {
                    #region 4.同意 退、废处理 修改账单数据 Tb_Order_PayDetail

                    OrderPayDetailList = baseDataManage.CallMethod("Tb_Order_PayDetail", "GetList", null, new Object[] { sqlWhere }) as List<Tb_Order_PayDetail>;

                    for (int i = 0; i < OrderPayDetailList.Count; i++)
                    {
                        tempPrice = 0;
                        tempOrderPayDetail = OrderPayDetailList[i];
                        tempSql = "";

                        if (tempOrderPayDetail.PayType == "付款")
                        {
                            // 付款
                            tempOrderPayDetail.PayMoney = mOrder.PayMoney; // 已经减去手续费
                            if (mOrder.PayWay == 2 || mOrder.PayWay == 6)
                            {
                                tempOrderPayDetail.BuyPoundage = data.FourToFiveNum((tempOrderPayDetail.PayMoney / OldOrder.PayMoney) * OldOrder.HandlingMoney, 2);
                            }
                            else
                            {
                                tempOrderPayDetail.BuyPoundage = data.FourToFiveNum(tempOrderPayDetail.PayMoney * Rate, 2); //手续费
                            }
                            //
                            tempOrderPayDetail.RealPayMoney = tempOrderPayDetail.PayMoney - tempOrderPayDetail.BuyPoundage;//实际交易金额（实收实付）
                            orderRealPayMoney = tempOrderPayDetail.RealPayMoney;//实际交易金额（实收实付）
                        }
                        else if (tempOrderPayDetail.PayType == "收款")
                        {
                            #region 收款

                            if (mOrder.PayWay == 14)
                            {
                                tempOrderPayDetailSXF = tempOrderPayDetail;
                                continue;
                            }
                            else
                            {
                                tempOrderPayDetail.PayMoney = mOrder.OrderMoney;
                                if (mOrder.PayWay == 2 || mOrder.PayWay == 6)
                                {
                                    tempOrderPayDetail.BuyPoundage = data.FourToFiveNum((tempOrderPayDetail.PayMoney / OldOrder.OrderMoney) * OldOrder.HandlingMoney, 2);
                                }
                                else
                                {
                                    tempOrderPayDetail.BuyPoundage = data.FourToFiveNum(tempOrderPayDetail.PayMoney * Rate, 2); //手续费
                                }

                                //#region 处理 1 分钱的问题 临时方法
                                //tempFFQ = mOrder.TGQHandlingFee * Rate;
                                //if (tempFFQ != 0 && tempFFQ < 0.01M)
                                //{
                                //    tempFFQ = 0.01M;
                                //}
                                //tempOrderPayDetail.BuyPoundage = tempOrderPayDetail.BuyPoundage - tempFFQ;
                                //#endregion

                                tempOrderPayDetail.RealPayMoney = tempOrderPayDetail.PayMoney - tempOrderPayDetail.BuyPoundage;//实际交易金额（实收实付）
                                //tempOrderPayDetail.RealPayMoney += data.FourToFiveNum(mOrder.TGQHandlingFee * Rate, 2);   //收款 手续费 的手续费
                            }

                            #endregion
                        }
                        else if (tempOrderPayDetail.PayType == "分账")
                        {
                            #region 分账

                            if (!string.IsNullOrEmpty(mOrder.DiscountDetail) && mOrder.DiscountDetail != "")
                            {
                                string[] discountDetails = mOrder.DiscountDetail.Split('|');

                                for (int j = 0; j < discountDetails.Length; j++)
                                {
                                    if (!string.IsNullOrEmpty(discountDetails[j]))
                                    {
                                        string[] values = discountDetails[j].Split('^');

                                        if (values[0] == tempOrderPayDetail.CpyNo)
                                        {
                                            tempKouDian = decimal.Parse(values[1]);

                                            tempXianFan = data.FourToFiveNum(decimal.Parse(values[2]), 2);

                                            if (PassengerList != null && PassengerList.Count > 0)
                                            {
                                                tempPrice = 0;

                                                //通过扣点计算金额
                                                foreach (Tb_Ticket_Passenger passenger in PassengerList)
                                                {
                                                    tempPrice += d.CreateCommissionFX(passenger.PMFee, tempKouDian, tempXianFan, passenger.PassengerType);
                                                }
                                            }
                                            allFzMoney += tempPrice; //分账金额之和

                                            break;
                                        }
                                    }
                                }

                                if (tempPrice == 0)
                                {
                                    tempOrderPayDetail.PayMoney = 0;
                                    tempOrderPayDetail.BuyPoundage = 0;
                                    tempOrderPayDetail.RealPayMoney = 0;
                                }
                                else
                                {
                                    if (mOrder.PayWay == 14)
                                    {
                                        tempOrderPayDetail.PayMoney = tempPrice;
                                        tempOrderPayDetail.BuyPoundage = 0;
                                        tempOrderPayDetail.RealPayMoney = tempPrice;
                                    }
                                    else
                                    {
                                        tempOrderPayDetail.PayMoney = tempPrice;
                                        tempOrderPayDetail.BuyPoundage = data.FourToFiveNum(tempOrderPayDetail.PayMoney * Rate, 1); //手续费

                                        if (tempOrderPayDetail.BuyPoundage <= 0.1M)
                                            tempOrderPayDetail.BuyPoundage = 0.1M;  //不足1毛按 1 毛计算

                                        tempOrderPayDetail.RealPayMoney = tempOrderPayDetail.PayMoney - tempOrderPayDetail.BuyPoundage;//实际交易金额（实收实付）

                                        if (tempOrderPayDetail.RealPayMoney < 0)
                                            tempOrderPayDetail.RealPayMoney = 0;
                                    }
                                }
                            }
                            #endregion
                        }
                        else if (tempOrderPayDetail.PayType == "手续费")
                        {
                            //手续费
                            if (mOrder.PayWay == 14)
                            {
                                //tempOrderPayDetail.PayMoney = tempOrderPayDetail.PayMoney;
                                tempOrderPayDetail.BuyPoundage = 0; //手续费
                                //tempOrderPayDetail.RealPayMoney = tempOrderPayDetail.PayMoney;//实际交易金额（实收实付）
                            }
                            else
                            {
                                tempOrderPayDetailSXF = tempOrderPayDetail;
                                continue;
                            }
                        }

                        if (mOrder.PayWay == 14)
                        {

                        }
                        else
                        {
                            if (tempOrderPayDetail.PayType == "收款" || tempOrderPayDetail.PayType == "分账")
                            {
                                AllPayMoney += tempOrderPayDetail.PayMoney;
                                AllRealPayMoney += tempOrderPayDetail.RealPayMoney;
                            }
                        }

                        tempSql = " update Tb_Order_PayDetail set ";
                        tempSql += " PayMoney='" + tempOrderPayDetail.PayMoney + "',";
                        tempSql += " BuyPoundage='" + tempOrderPayDetail.BuyPoundage + "',";
                        tempSql += " RealPayMoney='" + tempOrderPayDetail.RealPayMoney + "'";
                        tempSql += " where id='" + tempOrderPayDetail.id.ToString() + "'";

                        sqlList.Add(tempSql);
                    }

                    #region 手续费 最后计算

                    if (mOrder.PayWay == 14)
                    {
                        tempOrderPayDetailSXF.PayMoney = mOrder.OrderMoney;
                        tempOrderPayDetailSXF.BuyPoundage = 0;
                        tempOrderPayDetailSXF.RealPayMoney = tempOrderPayDetailSXF.PayMoney;
                    }
                    else
                    {
                        //在线支付手续费
                        if (mOrder.PayWay == 2 || mOrder.PayWay == 6)
                        {
                            tempOrderPayDetailSXF.BuyPoundage = data.FourToFiveNum((mOrder.PayMoney / OldOrder.PayMoney) * OldOrder.HandlingMoney, 2);
                        }
                        else
                        {
                            tempOrderPayDetailSXF.BuyPoundage = data.FourToFiveNum(mOrder.PayMoney * 0.001M, 2);
                        }

                        tempOrderPayDetailSXF.RealPayMoney = mOrder.PayMoney - AllRealPayMoney - tempOrderPayDetailSXF.BuyPoundage;
                        tempOrderPayDetailSXF.PayMoney = mOrder.PayMoney - AllPayMoney;


                        //财付通特殊处理
                        if (mOrder.PayWay == 4 || mOrder.PayWay == 8 || mOrder.PayWay == 40) //财付通特殊处理
                        {
                            tempOrderPayDetailSXF.RealPayMoney = mOrder.PayMoney - AllRealPayMoney;//实际交易金额（实收实付）
                        }
                    }

                    tempSql = " update Tb_Order_PayDetail set ";
                    tempSql += " PayMoney='" + tempOrderPayDetailSXF.PayMoney + "',";
                    tempSql += " BuyPoundage='" + tempOrderPayDetailSXF.BuyPoundage + "',";
                    tempSql += " RealPayMoney='" + tempOrderPayDetailSXF.RealPayMoney + "'";
                    tempSql += " where id='" + tempOrderPayDetailSXF.id.ToString() + "'";

                    sqlList.Add(tempSql);

                    #endregion

                    #endregion

                    #region 处理 TicketPayDetailList

                    TicketPayDetailList = baseDataManage.CallMethod("Tb_Ticket_PayDetail", "GetList", null, new Object[] { sqlWhere }) as List<Tb_Ticket_PayDetail>;

                    decimal policyPoint = mOrder.PolicyPoint;//政策原始返点
                    decimal returnPoint = mOrder.ReturnPoint; //最终返点
                    decimal returnMoney = mOrder.ReturnMoney;//政策现返金额
                    decimal FKtempReturnMoney = d.CreateXianFanPayFee(mOrder.DiscountDetail, returnMoney);

                    foreach (Tb_Ticket_Passenger passenger in PassengerList)
                    {
                        AllPayMoney = 0;
                        AllRealPayMoney = 0;
                        tempTicketPayDetailSXF = null;

                        for (int i = 0; i < TicketPayDetailList.Count; i++)
                        {
                            #region 4.同意 退、废处理 修改账单数据 Tb_Order_PayDetail

                            tempTicketPayDetail = TicketPayDetailList[i];

                            tempPrice = 0;
                            tempSql = "";

                            if (passenger.id.ToString() != tempTicketPayDetail.TicketId)
                            {
                                continue;
                            }

                            if (tempTicketPayDetail.PayType == "付款")
                            {
                                #region 付款

                                // 重新计算金额
                                tempTicketPayDetail.PayMoney = d.CreateOrderPayFeeGY(passenger.PMFee, passenger.ABFee, passenger.FuelFee, policyPoint, returnMoney, passenger.PassengerType);

                                tempTicketPayDetail.PayMoney = tempTicketPayDetail.PayMoney - passenger.TGQHandlingFee;
                                tempTicketPayDetail.BuyPoundage = data.FourToFiveNum(tempTicketPayDetail.PayMoney * Rate, 2); //手续费

                                tempTicketPayDetail.RealPayMoney = tempTicketPayDetail.PayMoney - tempTicketPayDetail.BuyPoundage;//实际交易金额（实收实付）

                                #endregion
                            }
                            else if (tempTicketPayDetail.PayType == "收款")
                            {
                                #region 收款

                                // 重新计算金额
                                tempTicketPayDetail.PayMoney = d.CreatePassengerPayFee(passenger.PMFee, passenger.ABFee, passenger.FuelFee, returnPoint, FKtempReturnMoney, passenger.PassengerType);

                                tempTicketPayDetail.PayMoney = tempTicketPayDetail.PayMoney - passenger.TGQHandlingFee;
                                tempTicketPayDetail.BuyPoundage = data.FourToFiveNum(tempTicketPayDetail.PayMoney * Rate, 2); //手续费
                                tempTicketPayDetail.RealPayMoney = tempTicketPayDetail.PayMoney - tempTicketPayDetail.BuyPoundage;//实际交易金额（实收实付）

                                #endregion
                            }
                            else if (tempTicketPayDetail.PayType == "分账")
                            {
                                #region 分账

                                // 分账
                                if (mOrder.PayWay == 14)
                                {
                                    //tempTicketPayDetail.PayMoney = tempTicketPayDetail.PayMoney;
                                    //tempTicketPayDetail.BuyPoundage = tempTicketPayDetail.BuyPoundage;
                                    //tempTicketPayDetail.RealPayMoney = tempTicketPayDetail.RealPayMoney;
                                }
                                else
                                {
                                    //tempTicketPayDetail.PayMoney = tempTicketPayDetail.PayMoney;
                                    //tempTicketPayDetail.BuyPoundage = tempTicketPayDetail.BuyPoundage;
                                    //tempTicketPayDetail.RealPayMoney = tempTicketPayDetail.RealPayMoney;
                                }
                                #endregion
                            }
                            else if (tempTicketPayDetail.PayType == "手续费")
                            {
                                #region 手续费

                                if (mOrder.PayWay == 14)
                                {
                                    tempTicketPayDetail.PayMoney = 0;
                                    tempTicketPayDetail.BuyPoundage = 0; //手续费
                                    tempTicketPayDetail.RealPayMoney = 0;//实际交易金额（实收实付）
                                }
                                else
                                {
                                    tempTicketPayDetailSXF = tempTicketPayDetail;
                                    continue;
                                }

                                #endregion
                            }

                            if (mOrder.PayWay != 14 && (tempOrderPayDetail.PayType == "收款" || tempOrderPayDetail.PayType == "分账"))
                            {
                                AllPayMoney += tempTicketPayDetail.PayMoney;
                                AllRealPayMoney += tempTicketPayDetail.RealPayMoney;
                            }

                            tempSql = " update Tb_Ticket_PayDetail set ";
                            tempSql += " PayMoney='" + tempTicketPayDetail.PayMoney + "',";
                            tempSql += " BuyPoundage='" + tempTicketPayDetail.BuyPoundage + "',";
                            tempSql += " RealPayMoney='" + tempTicketPayDetail.RealPayMoney + "'";
                            tempSql += " where id='" + tempTicketPayDetail.id.ToString() + "'";

                            sqlList.Add(tempSql);

                            #endregion  在线支付手续费 最后计算
                        }

                        if (mOrder.PayWay != 14)
                        {
                            #region 在线支付手续费

                            //单人金额

                            tempTicketPayDetailSXF.BuyPoundage = data.FourToFiveNum(mOrder.PayMoney * 0.001M, 2);
                            tempTicketPayDetailSXF.RealPayMoney = orderRealPayMoney - AllRealPayMoney - tempTicketPayDetailSXF.BuyPoundage;

                            if (tempTicketPayDetailSXF.RealPayMoney < 0)
                                tempTicketPayDetailSXF.RealPayMoney = 0;

                            tempSql = " update Tb_Ticket_PayDetail set ";
                            tempSql += " PayMoney='" + tempTicketPayDetailSXF.PayMoney + "',";
                            tempSql += " BuyPoundage='" + tempTicketPayDetailSXF.BuyPoundage + "',";
                            tempSql += " RealPayMoney='" + tempTicketPayDetailSXF.RealPayMoney + "'";
                            tempSql += " where id='" + tempTicketPayDetailSXF.id.ToString() + "'";

                            sqlList.Add(tempSql);

                            #endregion
                        }
                    }
                    #endregion
                }

                //添加到数据
                result = new Dal.ControlBase.BaseData<Tb_Ticket_Order>().ExecuteSqlTran(sqlList);
            }
            catch (Exception ex)
            {
                PnrAnalysis.LogText.LogWrite("【时间:" + System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ff") + " 方法: bool OperOrderTFG(Tb_Ticket_Order mOrder, List<Tb_Ticket_Passenger> PassengerList, User_Employees mUser, User_Company mCompany, string content)】================================================================\r\n 异常信息:" + ex.Message + "\r\nSQL:\r\n" + string.Join("\r\n", sqlList.ToArray()), "OperOrderTFG");
            }

            return result;
        }

        ///// <summary>
        ///// 审核中 处理
        ///// </summary>
        ///// <param name="mOrder"></param>
        ///// <param name="mUser"></param>
        ///// <param name="mCompany"></param>
        ///// <returns></returns>
        //public bool OperOrderTFGSHZ(Tb_Ticket_Order mOrder, User_Employees mUser, User_Company mCompany, string content)
        //{
        //    bool result = false;
        //    //SQL语句列表
        //    List<string> sqlList = new List<string>();
        //    try
        //    {
        //        string Content = new PbProject.Logic.ControlBase.Bd_Base_DictionaryBLL().GetDictionaryName("1", mOrder.OrderStatusCode.ToString());

        //        if (!string.IsNullOrEmpty(content))
        //            Content += "." + content;

        //        string tempSql = "";
        //        string sqlWhere = " OrderId='" + mOrder.OrderId + "'";

        //        //修改订单 状态 和 添加日志
        //        #region 1.修改订单

        //        StringBuilder updateOrder = new StringBuilder();
        //        updateOrder.Append(" update Tb_Ticket_Order set ");
        //        updateOrder.Append(" LockCpyNo='',");//锁定帐号所属公司编号
        //        updateOrder.Append(" LockLoginName='',");//锁定帐户
        //        updateOrder.Append(" LockTime='1900-1-1',");//锁定时间

        //        updateOrder.Append(" OrderStatusCode=" + mOrder.OrderStatusCode);  //操作类型即要修改的订单状态
        //        updateOrder.Append(" where " + sqlWhere);

        //        tempSql = updateOrder.ToString();
        //        sqlList.Add(tempSql);

        //        #endregion


        //        #region 3.添加订单日志

        //        Log_Tb_AirOrder OrderLog = new Log_Tb_AirOrder();
        //        OrderLog.id = Guid.NewGuid();
        //        OrderLog.OrderId = mOrder.OrderId;
        //        OrderLog.OperType = "修改";
        //        OrderLog.OperTime = DateTime.Now;
        //        OrderLog.OperLoginName = mUser.LoginName;
        //        OrderLog.OperUserName = mUser.UserName;
        //        OrderLog.CpyNo = mCompany.UninCode;
        //        OrderLog.CpyType = mCompany.RoleType;
        //        OrderLog.CpyName = mCompany.UninAllName;
        //        OrderLog.OperContent = Content;
        //        OrderLog.WatchType = 5;

        //        tempSql = Dal.Mapping.MappingHelper<Log_Tb_AirOrder>.CreateInsertModelSql(OrderLog);
        //        sqlList.Add(tempSql);

        //        #endregion

        //        //添加到数据
        //        result = new Dal.ControlBase.BaseData<Tb_Ticket_Order>().ExecuteSqlTran(sqlList);
        //    }
        //    catch (Exception ex)
        //    {
        //        PnrAnalysis.LogText.LogWrite("【时间:" + System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ff") + " 方法: bool OperOrderTFGSHZ(Tb_Ticket_Order mOrder, User_Employees mUser, User_Company mCompany, string content)】================================================================\r\n 异常信息:" + ex.Message + "\r\nSQL:\r\n" + string.Join("\r\n", sqlList.ToArray()), "OperOrderTFGSHZ");
        //    }

        //    return result;
        //}

        /// <summary>
        /// 改签处理
        /// </summary>
        /// <param name="opType">
        /// 操作类型：要修改的订单状态值
        /// 1 确认改签 、
        /// 2 拒绝改签
        /// </param>
        /// <param name="mOrder">订单 Model</param>
        /// <param name="PassengerList"></param>
        /// <param name="mUser">操作人 Model</param>
        /// <param name="mCompany">操作公司 Model</param>
        /// <returns></returns>
        public bool OperOrderGQ(Tb_Ticket_Order mOrder, List<Tb_Ticket_Passenger> PassengerList, User_Employees mUser, User_Company mCompany)
        {
            bool result = false;
            List<string> sqlList = new List<string>();
            try
            {
                //机票订单状态	6	申请改签，等待审核	申请改签
                //机票订单状态	10	审核失败，拒绝改签	改签审核失败
                //机票订单状态	18	拒绝补差，改签失败	改签失败
                //机票订单状态	19	改签成功，交易结束	改签成功
                //机票订单状态	23	拒绝改签，退款中	拒绝改签
                //机票订单状态	24	拒绝改签，已退款	拒绝改签          
                string tempSql = "";


                string Content = new PbProject.Logic.ControlBase.Bd_Base_DictionaryBLL().GetDictionaryName("1", mOrder.OrderStatusCode.ToString());

                mOrder.LockCpyNo = "";
                mOrder.LockLoginName = "";
                mOrder.LockTime = DateTime.Parse("1900-1-1");

                //修改订单 状态 和 添加日志
                #region 1.修改订单

                StringBuilder updateOrder = new StringBuilder();
                updateOrder.Append(" update Tb_Ticket_Order set ");
                updateOrder.Append(" LockCpyNo='',");//锁定帐号所属公司编号
                updateOrder.Append(" LockLoginName='',");//锁定帐户
                updateOrder.Append(" LockTime='1900-1-1',");//锁定时间


                updateOrder.Append(" CPTime='" + DateTime.Now.ToString() + "',");//时间

                updateOrder.Append(" OrderStatusCode=" + mOrder.OrderStatusCode);  //操作类型即要修改的订单状态
                updateOrder.Append(" where OrderId='" + mOrder.OrderId + "'");

                tempSql = updateOrder.ToString();
                sqlList.Add(tempSql);

                #endregion

                #region 2.添加订单日志

                Log_Tb_AirOrder OrderLog = new Log_Tb_AirOrder();
                OrderLog.id = Guid.NewGuid();
                OrderLog.OrderId = mOrder.OrderId;
                OrderLog.OperType = "修改";
                OrderLog.OperTime = DateTime.Now;
                OrderLog.OperLoginName = mUser.LoginName;
                OrderLog.OperUserName = mUser.UserName;
                OrderLog.CpyNo = mCompany.UninCode;
                OrderLog.CpyType = mCompany.RoleType;
                OrderLog.CpyName = mCompany.UninAllName;
                OrderLog.OperContent = Content;
                OrderLog.WatchType = mCompany.RoleType;

                tempSql = Dal.Mapping.MappingHelper<Log_Tb_AirOrder>.CreateInsertModelSql(OrderLog);
                sqlList.Add(tempSql);
                #endregion

                #region 3.还原原乘机人 isback

                if (mOrder.OrderStatusCode == 10 || mOrder.OrderStatusCode == 12 || 
                    mOrder.OrderStatusCode == 14 || mOrder.OrderStatusCode == 18 ||
                     mOrder.OrderStatusCode == 19 || mOrder.OrderStatusCode == 24)
                {

                    string PassengerNameS = "";

                    foreach (Tb_Ticket_Passenger item in PassengerList)
                    {
                        PassengerNameS += "'" + item.PassengerName + "',";
                    }
                    PassengerNameS = PassengerNameS.TrimEnd(',');

                    if (PassengerNameS != "")
                    {
                        tempSql = "  update Tb_Ticket_Passenger set IsBack=0 where OrderId = '" + mOrder.OldOrderId + "' and PassengerName in(" + PassengerNameS + ")";

                        sqlList.Add(tempSql);
                    }
                }

                #endregion

                // 添加到数据
                result = new Dal.ControlBase.BaseData<Tb_Ticket_Order>().ExecuteSqlTran(sqlList);
            }
            catch (Exception ex)
            {
                PnrAnalysis.LogText.LogWrite("【时间:" + System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ff") + " 方法:  bool OperOrderGQ(Tb_Ticket_Order mOrder, List<Tb_Ticket_Passenger> PassengerList, User_Employees mUser, User_Company mCompany)】================================================================\r\n 异常信息:" + ex.Message + "\r\nSQL:\r\n" + string.Join("\r\n", sqlList.ToArray()), "OperOrderGQ");
            }
            return result;
        }

        #endregion

        #region 出票处理

        /// <summary>
        /// 出票处理
        /// </summary>
        /// <param name="mOrder">订单 model</param>
        /// <param name="passengerList">乘机人 model</param>
        /// <param name="mUser">登录用户 model</param>
        /// <param name="mCompany">登录公司 model</param>
        /// <param name="mCompany">出票修改日志 contentLog </param>
        /// <returns></returns>
        public bool OperOrderCP(Tb_Ticket_Order mOrder, List<Tb_Ticket_Passenger> passengerList, User_Employees mUser, User_Company mCompany, string contentLog)
        {

            bool result = false;
            List<string> sqlList = new List<string>();
            try
            {
                string tempSql = "";
                string Content = new PbProject.Logic.ControlBase.Bd_Base_DictionaryBLL().GetDictionaryName("1", mOrder.OrderStatusCode.ToString());

                if (!string.IsNullOrEmpty(contentLog))
                    Content += "." + contentLog;

                mOrder.LockCpyNo = "";
                mOrder.LockLoginName = "";
                mOrder.LockTime = DateTime.Parse("1900-1-1");

                //修改订单 状态 和 添加日志
                #region 1.修改订单

                StringBuilder updateOrder = new StringBuilder();
                updateOrder.Append(" update Tb_Ticket_Order set ");
                updateOrder.Append(" LockCpyNo='',");//锁定帐号所属公司编号
                updateOrder.Append(" LockLoginName='',");//锁定帐户
                updateOrder.Append(" LockTime='1900-1-1',");//锁定时间
                updateOrder.Append(" TicketStatus=2,");//机票状态

                if (mOrder.AllowChangePNRFlag == true)
                {
                    updateOrder.Append(" ChangePNR='" + mOrder.ChangePNR + "',");//可以换编码
                }
                updateOrder.Append(" OutOrderPayMoney=" + mOrder.OutOrderPayMoney + ",");//代付金额
                updateOrder.Append(" A7=" + mOrder.A7 + ",");//代付返点

                updateOrder.Append(" PolicySource=" + mOrder.PolicySource + ",");//政策来源
                updateOrder.Append(" CPRemark='" + mOrder.CPRemark + "',");//出票备注

                updateOrder.Append(" CPTime='" + DateTime.Now.ToString() + "',");//时间 处理时间
                updateOrder.Append(" CPLoginName='" + mUser.LoginName + "',");//出票人登录帐号
                updateOrder.Append(" CPName='" + mUser.UserName + "',");//出票人姓名
                //updateOrder.Append(" CPCpyNo='" + mCompany.UninCode + "',");//出票人公司编号
                updateOrder.Append(" CPCpyName='" + mCompany.UninAllName + "'");//出票公司名称


                if (!contentLog.Contains("婴儿票请手动处理"))
                {
                    updateOrder.Append(" ,OrderStatusCode=4");  //操作类型即要修改的订单状态
                }
                updateOrder.Append(" where OrderId='" + mOrder.OrderId + "'");
                sqlList.Add(updateOrder.ToString());

                #endregion

                #region 2.修改乘机人状态

                if (passengerList == null || passengerList.Count == 0)
                {
                    StringBuilder updatepPssenger1 = new StringBuilder();
                    updatepPssenger1.Append(" update Tb_Ticket_Passenger set ");
                    updatepPssenger1.Append(" TicketStatus=2 ");//机票状态
                    updatepPssenger1.Append(" where OrderId='" + mOrder.OrderId + "'");

                    sqlList.Add(updatepPssenger1.ToString());
                }
                else
                {

                    for (int i = 0; i < passengerList.Count; i++)
                    {
                        StringBuilder updatepPssenger = new StringBuilder();
                        updatepPssenger.Append(" update Tb_Ticket_Passenger set ");
                        updatepPssenger.Append(" TicketStatus=2,");//机票状态
                        updatepPssenger.Append(" TicketNumber='" + passengerList[i].TicketNumber + "' ,");//票号
                        updatepPssenger.Append(" Cid='" + passengerList[i].Cid + "'");//证件号
                        updatepPssenger.Append(" where id='" + passengerList[i].id + "'");

                        sqlList.Add(updatepPssenger.ToString());
                    }
                }



                #endregion

                #region 3.添加订单日志

                Log_Tb_AirOrder OrderLog = new Log_Tb_AirOrder();
                OrderLog.id = Guid.NewGuid();
                OrderLog.OrderId = mOrder.OrderId;
                OrderLog.OperType = "出票";
                OrderLog.OperTime = DateTime.Now;
                OrderLog.OperLoginName = mUser.LoginName;
                OrderLog.OperUserName = mUser.UserName;
                OrderLog.CpyNo = mCompany.UninCode;
                OrderLog.CpyType = mCompany.RoleType;
                OrderLog.CpyName = mCompany.UninAllName;
                OrderLog.OperContent = Content;
                OrderLog.WatchType = mCompany.RoleType;

                tempSql = Dal.Mapping.MappingHelper<Log_Tb_AirOrder>.CreateInsertModelSql(OrderLog);
                sqlList.Add(tempSql);


                #endregion

                #region 添加到数据

                //添加到数据
                result = new Dal.ControlBase.BaseData<Tb_Ticket_Order>().ExecuteSqlTran(sqlList);

                #endregion

            }
            catch (Exception ex)
            {
                //DataBase.LogCommon.Log.Error("方法:OperOrderCP() 出票时间:" + System.DateTime.Now + " SQL语句:" + string.Join("\r\n ", sqlList.ToArray()), ex);
                PnrAnalysis.LogText.LogWrite("【时间:" + System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ff") + " 方法: bool OperOrderCP(Tb_Ticket_Order mOrder, List<Tb_Ticket_Passenger> passengerList, User_Employees mUser, User_Company mCompany, string contentLog)】================================================================\r\n 异常信息:" + ex.Message + "\r\nSQL:\r\n" + string.Join("\r\n", sqlList.ToArray()), "OperOrderCP");
            }
            return result;
        }

        /// <summary>
        /// 出票页面，保存并解锁
        /// </summary>
        /// <param name="mOrder"></param>
        /// <param name="mUser"></param>
        /// <param name="mCompany"></param>
        /// <param name="contentLog">保存修改修改日志 contentLog</param>
        /// <returns></returns>
        public bool OperOrderCPSave(Tb_Ticket_Order mOrder, User_Employees mUser, User_Company mCompany, string contentLog)
        {

            bool result = false;
            List<string> sqlList = new List<string>();
            try
            {
                string tempSql = "";

                mOrder.LockCpyNo = "";
                mOrder.LockLoginName = "";
                mOrder.LockTime = DateTime.Parse("1900-1-1");

                //修改订单 状态 和 添加日志
                #region 1.修改订单

                StringBuilder updateOrder = new StringBuilder();
                updateOrder.Append(" update Tb_Ticket_Order set ");
                updateOrder.Append(" LockCpyNo='',");//锁定帐号所属公司编号
                //updateOrder.Append(" LockLoginName='',");//锁定帐户
                updateOrder.Append(" LockTime='1900-1-1',");//锁定时间

                if (mOrder.AllowChangePNRFlag == true)
                {
                    updateOrder.Append(" ChangePNR='" + mOrder.ChangePNR + "',");//可以换编码
                }
                updateOrder.Append(" OutOrderPayMoney=" + mOrder.OutOrderPayMoney + ",");//代付金额
                updateOrder.Append(" A7=" + mOrder.A7 + ",");//代付返点
                updateOrder.Append(" OutOrderId='" + mOrder.OutOrderId + "',");//外部订单号
                updateOrder.Append(" PolicySource=" + mOrder.PolicySource + ",");//政策来源
                updateOrder.Append(" CPRemark='" + mOrder.CPRemark + "',");//出票备注

                updateOrder.Append(" OrderStatusCode=" + mOrder.OrderStatusCode);  //
                updateOrder.Append(" where OrderId='" + mOrder.OrderId + "'");
                sqlList.Add(updateOrder.ToString());

                #endregion

                string Content = "出票信息保存";
                if (!string.IsNullOrEmpty(contentLog))
                    Content += "." + contentLog;

                #region 2.添加订单日志

                Log_Tb_AirOrder OrderLog = new Log_Tb_AirOrder();
                OrderLog.id = Guid.NewGuid();
                OrderLog.OrderId = mOrder.OrderId;
                OrderLog.OperType = "修改";
                OrderLog.OperTime = DateTime.Now;
                OrderLog.OperLoginName = mUser.LoginName;
                OrderLog.OperUserName = mUser.UserName;
                OrderLog.CpyNo = mCompany.UninCode;
                OrderLog.CpyType = mCompany.RoleType;
                OrderLog.CpyName = mCompany.UninAllName;
                OrderLog.OperContent = Content;
                OrderLog.WatchType = mCompany.RoleType;

                tempSql = Dal.Mapping.MappingHelper<Log_Tb_AirOrder>.CreateInsertModelSql(OrderLog);
                sqlList.Add(tempSql);


                #endregion

                #region 添加到数据

                //添加到数据
                result = new Dal.ControlBase.BaseData<Tb_Ticket_Order>().ExecuteSqlTran(sqlList);

                #endregion

            }
            catch (Exception ex)
            {
                //DataBase.LogCommon.Log.Error("方法:OperOrderCPSave() 出票时间:" + System.DateTime.Now + " SQL语句:" + string.Join("\r\n ", sqlList.ToArray()), ex);
                PnrAnalysis.LogText.LogWrite("【时间:" + System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ff") + " 方法:bool OperOrderCPSave(Tb_Ticket_Order mOrder, User_Employees mUser, User_Company mCompany, string contentLog)】================================================================\r\n 异常信息:" + ex.Message + "\r\nSQL:\r\n" + string.Join("\r\n", sqlList.ToArray()), "OperOrderCPSave");
            }
            return result;
        }
        public bool OrderUnLock(string orderid)
        {
            PbProject.Dal.SQLEXDAL.SQLEXDAL_Base service = new Dal.SQLEXDAL.SQLEXDAL_Base();
            string sql = string.Format(" update Tb_Ticket_Order set  LockLoginName='' where OrderId='{0}'", orderid);
            return service.ExecuteNonQuerySQLInfo(sql);
        }
        /// <summary>
        /// 确定收银,单条处理 
        /// </summary>
        /// <param name="mOrder">订单 model</param>
        /// <param name="passengerList">乘机人 model</param>
        /// <param name="mUser">登录用户 model</param>
        /// <param name="mCompany">登录公司 model</param>
        /// <returns></returns>
        public bool OperOrderCashier(Tb_Ticket_Order mOrder, User_Employees mUser, User_Company mCompany)
        {
            bool result = false;
            List<string> sqlList = new List<string>();
            try
            {
                sqlList = OperOrderCashier(mOrder.id.ToString(), mOrder.OrderId, mUser, mCompany);
                if (sqlList.Count == 2)
                {
                    //添加到数据
                    result = new Dal.ControlBase.BaseData<Tb_Ticket_Order>().ExecuteSqlTran(sqlList);
                }
            }
            catch (Exception ex)
            {
                PnrAnalysis.LogText.LogWrite("【时间:" + System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ff") + " 方法:bool OperOrderCashier(Tb_Ticket_Order mOrder, User_Employees mUser, User_Company mCompany)】================================================================\r\n 异常信息:" + ex.Message + "\r\nSQL:\r\n" + string.Join("\r\n", sqlList.ToArray()), "OperOrderCashier");
            }
            return result;
        }

        /// <summary>
        /// 确定收银,批量处理 
        /// </summary>
        /// <param name="str_Ids_OrderIds">ids 和 OrderIds</param>
        /// <param name="mUser">登录用户 model</param>
        /// <param name="mCompany">登录公司 model</param>
        /// <returns></returns>
        public bool OperOrderCashiers(List<string> str_Ids_OrderIds, User_Employees mUser, User_Company mCompany)
        {
            bool result = false;
            List<string> sqlList = new List<string>();
            List<string> strTemp = new List<string>();
            try
            {
                if (str_Ids_OrderIds != null && str_Ids_OrderIds.Count > 0)
                {
                    foreach (string item in str_Ids_OrderIds)
                    {
                        string[] strArr = item.Split(new string[] { "##" }, StringSplitOptions.None);
                        strTemp = OperOrderCashier(strArr[0], strArr[1], mUser, mCompany);
                        if (strTemp.Count == 2)
                            sqlList.AddRange(strTemp);
                    }
                    int temp = str_Ids_OrderIds.Count * 2;
                    if (temp == sqlList.Count)
                    {
                        //添加到数据
                        result = new Dal.ControlBase.BaseData<Tb_Ticket_Order>().ExecuteSqlTran(sqlList);
                    }
                }
            }
            catch (Exception ex)
            {
                PnrAnalysis.LogText.LogWrite("【时间:" + System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ff") + " 方法: bool OperOrderCashiers(List<string> str_Ids_OrderIds, User_Employees mUser, User_Company mCompany)】================================================================\r\n 异常信息:" + ex.Message + "\r\nSQL:\r\n" + string.Join("\r\n", sqlList.ToArray()), "OperOrderCashiers");
            }
            return result;
        }

        /// <summary>
        /// 确定收银 sql 语句
        /// </summary>
        /// <param name="id">订单 id</param>
        ///  <param name="OrderID">订单 OrderID</param>
        /// <param name="mUser">登录用户 model</param>
        /// <param name="mCompany">登录公司 model</param>
        /// <returns></returns>
        private List<string> OperOrderCashier(string id, string OrderID, User_Employees mUser, User_Company mCompany)
        {
            List<string> sqlList = new List<string>();

            try
            {
                string tempSql = "";
                string Content = "确认收银成功";

                //修改订单 状态 和 添加日志
                #region 1.修改订单

                StringBuilder updateOrder = new StringBuilder();
                updateOrder.Append(" update Tb_Ticket_Order set ");
                updateOrder.Append(" LockCpyNo='',");//锁定帐号所属公司编号
                updateOrder.Append(" LockLoginName='',");//锁定帐户
                updateOrder.Append(" LockTime='1900-1-1',");//锁定时间
                updateOrder.Append(" TicketStatus=2,");//机票状态
                updateOrder.Append(" PayTime='" + DateTime.Now + "',");//出票时间  根据实际情况处理

                //updateOrder.Append(" CPTime='" + DateTime.Now + "',");//出票时间  根据实际情况处理
                //updateOrder.Append(" CPName='" + mUser.UserName + "',");//出票人姓名
                //updateOrder.Append(" CPCpyNo='" + mUser.CpyNo + "',");//出票公司编号
                //updateOrder.Append(" CPCpyName='" + mCompany.UninAllName + "',");//出票公司名称
                updateOrder.Append(" PayStatus=1 ");  //操作类型即要修改的订单状态
                updateOrder.Append(" where Id='" + id + "'");
                sqlList.Add(updateOrder.ToString());

                #endregion

                #region 2.添加订单日志

                Log_Tb_AirOrder OrderLog = new Log_Tb_AirOrder();
                OrderLog.id = Guid.NewGuid();
                OrderLog.OrderId = OrderID;
                OrderLog.OperType = "修改";
                OrderLog.OperTime = DateTime.Now;
                OrderLog.OperLoginName = mUser.LoginName;
                OrderLog.OperUserName = mUser.UserName;
                OrderLog.CpyNo = mCompany.UninCode;
                OrderLog.CpyType = mCompany.RoleType;
                OrderLog.CpyName = mCompany.UninAllName;
                OrderLog.OperContent = Content;
                OrderLog.WatchType = mCompany.RoleType;

                tempSql = Dal.Mapping.MappingHelper<Log_Tb_AirOrder>.CreateInsertModelSql(OrderLog);
                sqlList.Add(tempSql);


                #endregion

                #region 添加到数据

                //添加到数据
                //result = new Dal.ControlBase.BaseData<Tb_Ticket_Order>().ExecuteSqlTran(sqlList);

                #endregion

            }
            catch (Exception ex)
            {
                PnrAnalysis.LogText.LogWrite("【时间:" + System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ff") + " 方法: List<string> OperOrderCashier(string id, string OrderID, User_Employees mUser, User_Company mCompany)】================================================================\r\n 异常信息:" + ex.Message + "\r\nSQL:\r\n" + string.Join("\r\n", sqlList.ToArray()), "OperOrderCashier");
            }
            return sqlList;
        }

        #endregion

        #region 拒绝出票处理

        /// <summary>
        /// 拒绝出票处理
        /// </summary>
        /// <param name="mOrder">订单 model</param>
        /// <param name="passengerList">乘机人 model</param>
        /// <param name="mUser">登录用户 model</param>
        /// <param name="mCompany">登录公司 model</param>
        /// <returns></returns>
        public bool OperOrderJJCP(Tb_Ticket_Order mOrder, List<Tb_Ticket_Passenger> passengerList, User_Employees mUser, User_Company mCompany)
        {

            bool result = false;
            List<string> sqlList = new List<string>();
            try
            {
                string tempSql = "";

                string Content = new PbProject.Logic.ControlBase.Bd_Base_DictionaryBLL().GetDictionaryName("1", mOrder.OrderStatusCode.ToString());

                mOrder.LockCpyNo = "";
                mOrder.LockLoginName = "";
                mOrder.LockTime = DateTime.Parse("1900-1-1");

                //修改订单 状态 和 添加日志
                #region 1.修改订单

                StringBuilder updateOrder = new StringBuilder();
                updateOrder.Append(" update Tb_Ticket_Order set ");
                updateOrder.Append(" LockCpyNo='',");//锁定帐号所属公司编号
                updateOrder.Append(" LockLoginName='',");//锁定帐户
                updateOrder.Append(" LockTime='1900-1-1',");//锁定时间
                updateOrder.Append(" TicketStatus=6,");//机票状态
                updateOrder.Append(" CPRemark='" + mOrder.CPRemark + "',");//出票备注
                updateOrder.Append(" TGQRefusalReason='" + mOrder.TGQRefusalReason + "',");//退改签拒绝理由
                updateOrder.Append(" CPTime='" + DateTime.Now + "',");//出票时间
                updateOrder.Append(" CPName='" + mUser.UserName + "',");//出票人姓名
                updateOrder.Append(" CPCpyNo='" + mUser.CpyNo + "',");//出票公司编号
                updateOrder.Append(" CPCpyName='" + mCompany.UninAllName + "',");//出票公司名称
                updateOrder.Append(" OrderStatusCode=20");  //操作类型即要修改的订单状态  取消出票，退款中
                updateOrder.Append(" where OrderId='" + mOrder.OrderId + "'");
                sqlList.Add(updateOrder.ToString());

                #endregion

                #region 2.修改乘机人状态

                for (int i = 0; i < passengerList.Count; i++)
                {
                    StringBuilder updatepPssenger = new StringBuilder();
                    updatepPssenger.Append(" update Tb_Ticket_Passenger set ");
                    updatepPssenger.Append(" TicketStatus=6");//机票状态
                    updatepPssenger.Append(" where id='" + passengerList[i].id + "'");

                    sqlList.Add(updatepPssenger.ToString());
                }



                #endregion

                #region 3.添加订单日志

                Log_Tb_AirOrder OrderLog = new Log_Tb_AirOrder();
                OrderLog.id = Guid.NewGuid();
                OrderLog.OrderId = mOrder.OrderId;
                OrderLog.OperType = "取消";
                OrderLog.OperTime = DateTime.Now;
                OrderLog.OperLoginName = mUser.LoginName;
                OrderLog.OperUserName = mUser.UserName;
                OrderLog.CpyNo = mCompany.UninCode;
                OrderLog.CpyType = mCompany.RoleType;
                OrderLog.CpyName = mCompany.UninAllName;
                OrderLog.OperContent = Content;
                OrderLog.WatchType = mCompany.RoleType;

                tempSql = Dal.Mapping.MappingHelper<Log_Tb_AirOrder>.CreateInsertModelSql(OrderLog);
                sqlList.Add(tempSql);


                #endregion

                #region 添加到数据

                //添加到数据
                result = new Dal.ControlBase.BaseData<Tb_Ticket_Order>().ExecuteSqlTran(sqlList);

                #endregion

            }
            catch (Exception ex)
            {
                PnrAnalysis.LogText.LogWrite("【时间:" + System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ff") + " 方法:bool OperOrderJJCP(Tb_Ticket_Order mOrder, List<Tb_Ticket_Passenger> passengerList, User_Employees mUser, User_Company mCompany)】================================================================\r\n 异常信息:" + ex.Message + "\r\nSQL:\r\n" + string.Join("\r\n", sqlList.ToArray()), "OperOrderJJCP");
            }
            return result;
        }

        #endregion

        #region 销账处理： 欠款提醒

        /// <summary>
        /// 支付时:看有无未还清的账单
        /// </summary>
        /// <param name="uCompany">登录功能</param>
        /// <returns> true:提醒 有未还清的账单，false：不提醒 欠款天数为0 也不提醒。</returns>
        public bool AutomaticRemind(User_Company uCompany, out decimal payDebtsMoney)
        {
            bool result = false;
            payDebtsMoney = 0;

            try
            {
                if (uCompany.AccountMoney < 0 && uCompany.MaxDebtDays != 0)
                {
                    #region 获取账单所有欠款订单

                    DateTime nowDt = DateTime.Now; //当前时间
                    StringBuilder sbSql = new StringBuilder();
                    sbSql.Append(" OwnerCpyNo='" + uCompany.UninCode + "' ");
                    sbSql.Append(" and PayDebtsMoney<>0 ");

                    sbSql.Append(" and PayWay=14 "); //支付方式
                    sbSql.Append(" and PayTime < '" + nowDt.ToString("yyyy-MM-dd") + " 23:59:59' ");
                    sbSql.Append(" and OrderStatusCode=4 "); //出票成功状态
                    sbSql.Append(" and PayStatus=1 "); //支付成功状态

                    List<Tb_Ticket_Order> orderList = baseDataManage.CallMethod("Tb_Ticket_Order", "GetList", null, new object[] { sbSql.ToString() }) as List<Tb_Ticket_Order>;

                    foreach (Tb_Ticket_Order item in orderList)
                    {
                        if (nowDt > item.PayTime.AddDays(uCompany.MaxDebtDays))
                        {
                            //账单期外的单子
                            payDebtsMoney += item.PayDebtsMoney;

                            result = true; // 提醒 有未还清的账单
                        }
                        else
                        {
                            //账单期内的单子，忽略
                        }
                    }

                    #endregion
                }
                else
                {
                    result = false;
                }
            }
            catch (Exception ex)
            {
                PnrAnalysis.LogText.LogWrite("【时间:" + System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ff") + " 方法:bool AutomaticRemind(User_Company uCompany, out decimal payDebtsMoney)】================================================================\r\n 异常信息:" + ex.Message + "\r\n", "AutomaticRemind");
            }
            return result;
        }

        /// <summary>
        /// 获取所有未销账的订单
        /// </summary>
        /// <param name="UninCode">公司编号</param>
        /// <returns></returns>
        public List<Tb_Ticket_Order> GetListOrder(string UninCode)
        {
            List<Tb_Ticket_Order> orderList = null;
            try
            {
                StringBuilder sbSql = new StringBuilder();

                sbSql.Append(" OwnerCpyNo='" + UninCode + "' ");
                sbSql.Append(" and DebtsPayFlag=1 ");
                sbSql.Append(" and PayDebtsMoney<>0 ");
                sbSql.Append(" and PayWay=14 "); //支付方式
                sbSql.Append(" and OrderStatusCode=4 "); //出票成功状态
                sbSql.Append(" and PayStatus=1 "); //支付成功状态
                sbSql.Append(" order by PayTime "); //支付时间排序

                orderList = GetListBySqlWhere(sbSql.ToString());
            }
            catch (Exception ex)
            {
                PnrAnalysis.LogText.LogWrite("【时间:" + System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ff") + " 方法:List<Tb_Ticket_Order> GetListOrder(string UninCode)】================================================================\r\n 异常信息:" + ex.Message + "\r\n参数:UninCode=" + UninCode, "GetListOrder");
            }
            return orderList;
        }

        /// <summary>
        /// 充值、调账 自动销账
        /// </summary>
        /// <param name="UninCode"></param>
        /// <param name="payMoney"></param>
        /// <returns></returns>
        public List<string> AutomaticOperXZ(string UninCode, decimal payMoney)
        {
            List<string> sqlList = new List<string>();

            string tempSql = "";

            try
            {
                List<Tb_Ticket_Order> ticketList = GetListOrder(UninCode);

                #region 收款公司信息


                string tempCpyNo = UninCode.Substring(0, 12);

                string tempSqlWhere = " UninCode='" + tempCpyNo + "' or UninCode='" + UninCode + "'";

                List<User_Company> RecCompanyList = new PbProject.Logic.User.User_CompanyBLL().GetListBySqlWhere(tempSqlWhere);

                //收款方 tempCpyNo
                User_Company RecCompany = null;
                //付款方 UninCode
                User_Company payCompany = null;

                foreach (User_Company item in RecCompanyList)
                {
                    if (item.UninCode == tempCpyNo)
                    {
                        RecCompany = item;
                    }
                    else if (item.UninCode == UninCode)
                    {
                        payCompany = item;
                    }
                }
                #endregion

                foreach (Tb_Ticket_Order ticketOrder in ticketList)
                {
                    if (payMoney >= ticketOrder.PayDebtsMoney)
                    {
                        tempSql = "update Tb_Ticket_Order set DebtsPayFlag=0,PayDebtsMoney=0.00 where id='" + ticketOrder.id + "'";
                        payMoney = payMoney - ticketOrder.PayDebtsMoney;
                        sqlList.Add(tempSql);

                        tempSql = GetSql(payMoney, ticketOrder, null, payCompany, RecCompany);
                        sqlList.Add(tempSql);

                    }
                    else
                    {
                        tempSql = "update Tb_Ticket_Order set PayDebtsMoney=PayDebtsMoney-" + payMoney + " where id='" + ticketOrder.id + "'";
                        payMoney = 0;
                        sqlList.Add(tempSql);

                        tempSql = GetSql(payMoney, ticketOrder, null, payCompany, RecCompany);
                        sqlList.Add(tempSql);
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                PnrAnalysis.LogText.LogWrite("【时间:" + System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ff") + " 方法: List<string> AutomaticOperXZ(string UninCode, decimal payMoney)】================================================================\r\n 异常信息:" + ex.Message + "\r\n参数:UninCode=" + UninCode + " payMoney=" + payMoney.ToString(), "AutomaticOperXZ");
            }
            return sqlList;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tempMoney"></param>
        /// <param name="mOrder"></param>
        /// <param name="payUserEmployees"></param>
        /// <param name="payCompany"></param>
        /// <param name="RecCompany"></param>
        /// <returns></returns>
        public string GetSql(decimal tempMoney, Tb_Ticket_Order mOrder, User_Employees payUserEmployees, User_Company payCompany, User_Company RecCompany)
        {
            string tempSql = "";

            try
            {
                #region

                Log_MoneyDetail logMoneyDetail = new Log_MoneyDetail();

                logMoneyDetail.id = Guid.NewGuid();
                logMoneyDetail.InPayNo = mOrder.InPayNo;//内部流水号
                logMoneyDetail.OrderId = mOrder.OrderId + "_" + DateTime.Now.ToString("yyMMddHHmmss");//订单编号
                logMoneyDetail.PayNo = mOrder.InPayNo;//支付流水号

                //logMoneyDetail.OperLoginName = payUserEmployees.LoginName;
                //logMoneyDetail.OperUserName = payUserEmployees.UserName;

                logMoneyDetail.OperLoginName = mOrder.CreateLoginName;
                logMoneyDetail.OperUserName = mOrder.CreateUserName;

                logMoneyDetail.OperReason = "欠款明细记录";
                logMoneyDetail.OperTime = DateTime.Now;

                logMoneyDetail.PayCpyName = payCompany.UninAllName;
                logMoneyDetail.PayCpyNo = payCompany.UninCode;
                logMoneyDetail.PayCpyType = payCompany.RoleType;
                logMoneyDetail.PayType = 21;
                logMoneyDetail.RecCpyName = RecCompany.UninAllName;
                logMoneyDetail.RecCpyNo = RecCompany.UninCode;
                logMoneyDetail.RecCpyType = RecCompany.RoleType;
                logMoneyDetail.Remark = "欠款明细记录";//内部流水号

                //logMoneyDetail.PayMoney = mOrder.PayMoney;
                //logMoneyDetail.PreRemainMoney = oldAccountMoney;
                //logMoneyDetail.RemainMoney = newAccountMoney;

                logMoneyDetail.PayMoney = tempMoney;
                logMoneyDetail.PreRemainMoney = 0;
                logMoneyDetail.RemainMoney = 0;

                tempSql = Dal.Mapping.MappingHelper<Log_MoneyDetail>.CreateInsertModelSql(logMoneyDetail);

                #endregion
            }
            catch (Exception ex)
            {
                PnrAnalysis.LogText.LogWrite("【时间:" + System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ff") + " 方法:string GetSql(decimal tempMoney, Tb_Ticket_Order mOrder, User_Employees payUserEmployees, User_Company payCompany, User_Company RecCompany)】================================================================\r\n 异常信息:" + ex.Message + "\r\nSQL:" + tempSql, "GetSql");
            }
            return tempSql;
        }

        /// <summary>
        /// 退票成功 自动销账
        /// </summary>
        /// <param name="mOrder"></param>
        public List<string> AutomaticVirtualPayXZ(Tb_Ticket_Order mOrder)
        {
            List<string> sqlList = new List<string>();
            try
            {
                if (mOrder.PayWay == 14)
                {
                    sqlList = AutomaticOperXZ(mOrder.OwnerCpyName, mOrder.PayMoney);
                }
            }
            catch (Exception ex)
            {
                PnrAnalysis.LogText.LogWrite("【时间:" + System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ff") + " 方法:string GetSql(decimal tempMoney, Tb_Ticket_Order mOrder, User_Employees payUserEmployees, User_Company payCompany, User_Company RecCompany)】================================================================\r\n 异常信息:" + ex.Message + "\r\nSQL:" + string.Join("\r\n", sqlList.ToArray()), "AutomaticVirtualPayXZ");
            }
            return sqlList;
        }

        #endregion


        /// <summary>
        /// 订单日志,单独操作
        /// </summary>
        /// <param name="uEmployees"></param>
        /// <param name="uCompany"></param>
        /// <param name="orderId"></param>
        /// <param name="WatchType"></param>
        /// <param name="Content"></param>
        /// <returns></returns>
        public bool CreateOrderLog(User_Employees uEmployees, User_Company uCompany, string orderId, int WatchType, string Content)
        {
            bool result = false;
            try
            {
                #region 添加订单日志

                //2	OrderId	varchar	50	0	订单编号
                //3	OperType	varchar	10	0	操作类型：预订、支付、出票、修改等。
                //4	OperTime	datetime	23	3	操作时间
                //5	OperLoginName	varchar	50	0	操作员登录名
                //6	OperUserName	varchar	100	0	操作员名称
                //7	CpyNo	varchar	50	0	公司编号
                //8	CpyType	int	4	0	公司类型
                //9	CpyName	varchar	100	0	公司名称
                //10	OperContent	text	4	0	操作内容描述
                //11	WatchType	int	4	0	查看权限（1.平台 2.运营 3.供应 4.分销 5.采购）

                Log_Tb_AirOrder OrderLog = new Log_Tb_AirOrder();
                OrderLog.id = Guid.NewGuid();
                OrderLog.OrderId = orderId;
                OrderLog.OperType = "修改";
                OrderLog.OperTime = DateTime.Now.AddSeconds(1);
                OrderLog.OperLoginName = (uEmployees != null) ? uEmployees.LoginName : "";
                OrderLog.OperUserName = (uEmployees != null) ? uEmployees.UserName : "";
                OrderLog.CpyNo = (uEmployees != null) ? uEmployees.CpyNo : "";
                OrderLog.CpyType = 0;
                OrderLog.CpyName = "";
                OrderLog.OperContent = Content;
                OrderLog.WatchType = WatchType;

                PbProject.Logic.ControlBase.BaseDataManage baseDataManage = new PbProject.Logic.ControlBase.BaseDataManage();
                result = (bool)baseDataManage.CallMethod("Log_Tb_AirOrder", "Insert", null, new Object[] { OrderLog });
                #endregion
            }
            catch (Exception ex)
            {
                PnrAnalysis.LogText.LogWrite("【时间:" + System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ff") + " 方法:bool CreateOrderLog(User_Employees uEmployees, User_Company uCompany, string orderId, int WatchType, string Content)】================================================================\r\n 异常信息:" + ex.Message + "\r\n", "CreateOrderLog");
            }

            return result;
        }

        #region 手动修改证件号和票号


        /// <summary>
        /// 手动修改订单详情信息
        /// </summary>
        /// <param name="mOrder">操作类型 0修改证件号 1修改票号,2修改出票备注 3修改政策来源</param>
        /// <param name="mOrder">订单 model</param>
        /// <param name="passenger">乘机人 model</param>
        /// <param name="mUser">用户 model</param>
        /// <param name="mCompany">公司 model</param>
        /// <param name="content">修改日志内容</param>
        /// <returns></returns>
        public bool UpdateOrderDetailInfo(int mType, Tb_Ticket_Order mOrder, Tb_Ticket_Passenger passenger, User_Employees mUser, User_Company mCompany, string content)
        {
            bool result = false;
            //添加 sql 语句
            List<string> sqlList = new List<string>();
            try
            {
                // 1.添加新订单信息
                string tempSql = "";

                #region 1.添加订单日志

                Log_Tb_AirOrder OrderLog = new Log_Tb_AirOrder();
                OrderLog.id = Guid.NewGuid();
                OrderLog.OrderId = mOrder.OrderId;
                OrderLog.OperType = "修改";
                OrderLog.OperTime = DateTime.Now;
                OrderLog.OperLoginName = mUser.LoginName;
                OrderLog.OperUserName = mUser.UserName;
                OrderLog.CpyNo = mCompany.UninCode;
                OrderLog.CpyType = mCompany.RoleType;
                OrderLog.CpyName = mCompany.UninAllName;
                OrderLog.OperContent = content;
                //修改出票备注
                if (mType == 2 || mType == 3)
                {
                    OrderLog.WatchType = mCompany.RoleType;
                }
                else
                {
                    OrderLog.WatchType = 5;
                }
                tempSql = Dal.Mapping.MappingHelper<Log_Tb_AirOrder>.CreateInsertModelSql(OrderLog);
                sqlList.Add(tempSql);

                #endregion

                #region 2.修改乘机人证件号和票号
                //修改证件号
                if (mType == 0)
                {
                    tempSql = "update Tb_Ticket_Passenger set Cid='" + passenger.Cid + "',Remark='" + passenger.Remark + "' where id='" + passenger.id + "'";
                    sqlList.Add(tempSql);
                }
                //修改票号
                else if (mType == 1)
                {
                    tempSql = "update Tb_Ticket_Passenger set TicketNumber='" + passenger.TicketNumber + "' where id='" + passenger.id + "'";
                    sqlList.Add(tempSql);
                }
                //修改修改出票备注
                else if (mType == 2)
                {
                    tempSql = "update Tb_Ticket_Order set CPRemark='" + mOrder.CPRemark + "' where id='" + mOrder.id + "'";
                    sqlList.Add(tempSql);
                }
                //修改政策来源
                else if (mType == 3)
                {
                    tempSql = "update Tb_Ticket_Order set PolicySource='" + mOrder.PolicySource + "' where id='" + mOrder.id + "'";
                    sqlList.Add(tempSql);
                }
                #endregion

                //添加到数据
                result = new Dal.ControlBase.BaseData<Tb_Ticket_Order>().ExecuteSqlTran(sqlList);
            }
            catch (Exception ex)
            {
                //DataBase.LogCommon.Log.Error("UpdatePassengerCidAndTicketNumber", ex);
                PnrAnalysis.LogText.LogWrite("【时间:" + System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ff") + " 方法: bool UpdateOrderDetailInfo(int mType, Tb_Ticket_Order mOrder, Tb_Ticket_Passenger passenger, User_Employees mUser, User_Company mCompany, string content)】================================================================\r\n 异常信息:" + ex.Message + "\r\nSQL:" + string.Join("\r\n", sqlList), "UpdateOrderDetailInfo");
            }
            return result;
        }
        /// <summary>
        /// 修改 (支付/退废)验证检查
        /// </summary>
        /// <param name="id"></param>
        /// <param name="Status"></param>
        /// <returns></returns>
        public bool UpdatePayRefounedChecked(Tb_Ticket_Order mOrder, User_Employees mUser, User_Company mCompany, string Status)
        {
            bool IsUpdate = false;
            List<string> lstSQL = new List<string>();
            try
            {
                Log_Tb_AirOrder OrderLog = new Log_Tb_AirOrder();
                OrderLog.id = Guid.NewGuid();
                OrderLog.OrderId = mOrder.OrderId;
                OrderLog.OperType = "修改";
                OrderLog.OperTime = DateTime.Now;
                OrderLog.OperLoginName = mUser.LoginName;
                OrderLog.OperUserName = mUser.UserName;
                OrderLog.CpyNo = mCompany.UninCode;
                OrderLog.CpyType = mCompany.RoleType;
                OrderLog.CpyName = mCompany.UninAllName;
                OrderLog.OperContent = "(支付/退废)检查=>原来" + (mOrder.A9 == "1" ? "不检查" : "要检查") + "修改为:" + (Status == "1" ? "不检查" : "要检查");
                OrderLog.WatchType = mCompany.RoleType;
                string sql = Dal.Mapping.MappingHelper<Log_Tb_AirOrder>.CreateInsertModelSql(OrderLog);
                lstSQL.Add(sql);

                string id = mOrder.id.ToString();
                string updateSql = string.Format(" update Tb_Ticket_Order set A9='{0}' where id='{1}'", Status, id);
                lstSQL.Add(updateSql);
                string ErrMsg = "";

                IsUpdate = baseDataManage.ExecuteSqlTran(lstSQL, out ErrMsg);
            }
            catch (Exception ex)
            {
                PnrAnalysis.LogText.LogWrite("【时间:" + System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ff") + " 方法: bool UpdatePayRefounedChecked(Tb_Ticket_Order mOrder, User_Employees mUser, User_Company mCompany, string Status)】================================================================\r\n 异常信息:" + ex.Message + "\r\nSQL:" + string.Join("\r\n", lstSQL), "UpdatePayRefounedChecked");
            }
            return IsUpdate;
        }

        #endregion
    }
}
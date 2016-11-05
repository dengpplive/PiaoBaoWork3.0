using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PbProject.Model;
using PbProject.Logic.User;
using PbProject.Logic.Order;
using DataBase.Data;
using System.Data;
using PbProject.WebCommon.Utility;
using System.Web;
using System.Web.UI;
using PbProject.Logic.ControlBase;
using PbProject.Logic.SQLEXBLL;

namespace PbProject.Logic.Pay
{
    /// <summary>
    /// 2.94 数据同步到3.0接收通知参数
    /// </summary>
    public class AcceptParam
    {
        public string m_Pnr = string.Empty;
        public string m_Office = string.Empty;
        public string m_OrderId = string.Empty;
        public string m_LoginName = string.Empty;
        public string m_PassengerList = string.Empty;
        public decimal m_Price = 0m;
        public string m_CompanyName = string.Empty;
        public int Cpyid = 0;
        /// <summary>
        /// 记录参数 //记录执行步骤日志
        /// </summary>
        public StringBuilder sbLog = new StringBuilder();
    }
    /// <summary>
    /// 账单处理
    /// </summary>
    public class Bill
    {
        /// <summary>
        /// 锁定变量
        /// </summary>
        public static object lockobject = new object();
        /// <summary>
        ///  公共操作
        /// </summary>
        public PbProject.Logic.ControlBase.BaseDataManage baseDataManage = new PbProject.Logic.ControlBase.BaseDataManage();

        #region 生成 订单账单明细、机票账单明细

        /*
        /// <summary>
        /// 确定订单时:生成订单账单明细 Tb_Order_PayDetail、机票账单明细  Tb_Ticket_PayDetail
        /// </summary>
        /// <param name="mOrder">订单mOrder</param>
        /// <param name="mPassenger">乘机人mOrder</param>
        /// <returns>返回sql语句</returns>
        public List<string> CreateOrderAndTicketPayDetail(Tb_Ticket_Order mOrder, List<Tb_Ticket_Passenger> mPassenger)
        {
            lock (lockobject)
            {
                List<string> sqlList = new List<string>();

                try
                {
                    if ((mOrder.PayMoney + mOrder.PayMoney * 0.003m) >= mOrder.OrderMoney) //判断金额是否正确
                    {
                        List<Tb_Order_PayDetail> oldPayDetailCount = null;

                        if (mOrder.OrderStatusCode != 1 && !string.IsNullOrEmpty(mOrder.OldOrderId))
                            oldPayDetailCount = new Dal.ControlBase.BaseData<Tb_Order_PayDetail>().GetList(" OrderId='" + mOrder.OldOrderId + "' ");

                        List<Tb_Order_PayDetail> PayDetailCount = new Dal.ControlBase.BaseData<Tb_Order_PayDetail>().GetList(" OrderId='" + mOrder.OrderId + "' ");

                        if (PayDetailCount == null || PayDetailCount.Count == 0) // 判断订单是否存在
                        {
                            string tempSqlWhere = ""; //查询条件

                            string ownerCpyNo = mOrder.OwnerCpyNo; //订单方:付款
                            string cPCpyNo = mOrder.CPCpyNo;//出票方：收款
                            string PtCpyNo = mOrder.OwnerCpyNo.Substring(0, 6);//手续费：手续费
                            decimal policyPoint = mOrder.PolicyPoint;//政策原始返点
                            decimal returnPoint = mOrder.ReturnPoint; //最终返点
                            decimal returnMoney = mOrder.ReturnMoney;//政策现返金额

                            decimal tempPoint = 0;//扣点:临时变量
                            decimal tempReturnMoney = 0; //现返:临时变量
                            decimal tempMoney = 0;//交易金额:临时变量

                            decimal allPayMoney = 0;//累计金额

                            Data d = new Data();

                            List<Tb_Order_PayDetail> orderPayDetailList = new List<Tb_Order_PayDetail>();
                            List<Tb_Ticket_PayDetail> ticketPayDetailList = new List<Tb_Ticket_PayDetail>();

                            Tb_Order_PayDetail orderPayDetail = null;
                            Tb_Ticket_PayDetail ticketPayDetail = null;
                            User_Company uCompany = null;

                            #region 获取所有收款公司信息

                            string CompanyNoS = ""; //参与交易所有公司信息
                            CompanyNoS += "'" + ownerCpyNo + "',";//订单方
                            if (!CompanyNoS.Contains("'" + cPCpyNo + "'"))
                                CompanyNoS += "'" + cPCpyNo + "',";//出票方
                            if (!CompanyNoS.Contains("'" + PtCpyNo + "'"))
                                CompanyNoS += "'" + PtCpyNo + "',"; //手续费公司

                            //其它收款公司
                            if (!string.IsNullOrEmpty(mOrder.DiscountDetail) && mOrder.DiscountDetail != "")
                            {
                                string[] discountDetails = mOrder.DiscountDetail.Split('|');

                                for (int i = 0; i < discountDetails.Length; i++)
                                {
                                    if (!string.IsNullOrEmpty(discountDetails[i]))
                                    {
                                        string[] values = discountDetails[i].Split('^');
                                        if (!CompanyNoS.Contains("'" + values[0] + "'"))
                                            CompanyNoS += "'" + values[0] + "',";
                                    }
                                }
                            }
                            CompanyNoS = CompanyNoS.TrimEnd(',');

                            tempSqlWhere = " UninCode in(" + CompanyNoS + ")";
                            List<User_Company> uCompanys = new PbProject.Logic.User.User_CompanyBLL().GetListBySqlWhere(tempSqlWhere);

                            #endregion

                            #region Tb_Order_PayDetail 处理

                            #region 付款

                            //订单账单明细
                            orderPayDetail = new Tb_Order_PayDetail();
                            foreach (User_Company item in uCompanys)
                            {
                                if (item.UninCode == ownerCpyNo) //订单方:付款
                                {
                                    uCompany = item;
                                    break;
                                }
                            }

                            orderPayDetail.id = Guid.NewGuid();
                            orderPayDetail.OrderId = mOrder.OrderId;//订单编号
                            orderPayDetail.PayType = "付款";//支付类型“付款”“收款”“分账”
                            orderPayDetail.PayMode = 0;//支付方式（见字典表）
                            orderPayDetail.CpyNo = uCompany.UninCode;//公司编号
                            orderPayDetail.CpyType = uCompany.RoleType;//公司类型
                            orderPayDetail.CpyName = uCompany.UninAllName;//公司名称
                            orderPayDetail.BuyPoundage = 0M;//	交易手续费
                            orderPayDetail.PayMoney = mOrder.PayMoney;//交易金额（应收应付）
                            orderPayDetail.RealPayMoney = 0;//实际交易金额（实收实付）
                            orderPayDetail.InPayNo = mOrder.InPayNo;//内部流水号
                            orderPayDetail.PayNo = "";//支付交易流水号
                            orderPayDetail.ReturnPayNo = "";//退款交易流水号
                            orderPayDetail.PayAccount = GetPayAccount(uCompany.UninCode, oldPayDetailCount);//收支帐号
                            orderPayDetailList.Add(orderPayDetail);

                            #endregion

                            #region 收款方
                            // 订单账单明细
                            //tempMoney = 0;//通过扣点计算金额

                            //foreach (Tb_Ticket_Passenger passenger in mPassenger)
                            //{
                            //    tempMoney += d.CreateOrderPayFeeGY(passenger.PMFee, passenger.ABFee, passenger.FuelFee, policyPoint, returnMoney, passenger.PassengerType);
                            //}
                            //allPayMoney += tempMoney;

                            foreach (User_Company item in uCompanys)
                            {
                                if (item.UninCode == mOrder.CPCpyNo)
                                {
                                    uCompany = item;
                                    break;
                                }
                            }

                            orderPayDetail = new Tb_Order_PayDetail();
                            orderPayDetail.id = Guid.NewGuid();
                            orderPayDetail.OrderId = mOrder.OrderId;//订单编号
                            orderPayDetail.PayType = "收款";
                            orderPayDetail.PayMode = 0;
                            orderPayDetail.CpyNo = uCompany.UninCode;//公司编号
                            orderPayDetail.CpyType = uCompany.RoleType;//公司类型
                            orderPayDetail.CpyName = uCompany.UninAllName;//公司名称
                            orderPayDetail.BuyPoundage = 0M; //交易手续费
                            orderPayDetail.PayMoney = mOrder.OrderMoney;//交易金额（应收应付）
                            orderPayDetail.RealPayMoney = 0;//实际交易金额（实收实付）
                            orderPayDetail.InPayNo = mOrder.InPayNo; //内部流水号
                            orderPayDetail.PayNo = "";//支付交易流水号
                            orderPayDetail.ReturnPayNo = "";//退款交易流水号
                            orderPayDetail.PayAccount = GetPayAccount(uCompany.UninCode, oldPayDetailCount);//收支帐号
                            orderPayDetailList.Add(orderPayDetail);

                            #endregion

                            // 金额之差 = 支付金额 - 收款金额 
                            tempMoney = mOrder.PayMoney - mOrder.OrderMoney;

                            #region 分账

                            if (!string.IsNullOrEmpty(mOrder.DiscountDetail) && mOrder.DiscountDetail != "")
                            {
                                string[] discountDetails = mOrder.DiscountDetail.Trim('|').Split('|');

                                for (int i = 0; i < discountDetails.Length; i++)
                                {
                                    if (!string.IsNullOrEmpty(discountDetails[i]))
                                    {
                                        string[] values = discountDetails[i].Split('^');
                                        tempPoint = decimal.Parse(values[1]);
                                        tempReturnMoney = d.FourToFiveNum(decimal.Parse(values[2]), 2); //现返

                                        foreach (User_Company item in uCompanys)
                                        {
                                            if (item.UninCode == values[0])
                                            {
                                                uCompany = item;
                                                break;
                                            }
                                        }

                                        decimal tempMoneyNew = 0;
                                        if (mPassenger != null && mPassenger.Count > 0)
                                        {
                                            //通过扣点计算金额
                                            foreach (Tb_Ticket_Passenger passenger in mPassenger)
                                            {
                                                tempMoneyNew += d.CreateCommissionFX(passenger.PMFee, tempPoint, tempReturnMoney, passenger.PassengerType);
                                            }
                                        }

                                        if (tempMoney >= tempMoneyNew)
                                        {
                                            tempMoney = tempMoney - tempMoneyNew;
                                        }
                                        else
                                        {
                                            tempMoneyNew = tempMoney;
                                            tempMoney = 0;
                                        }


                                        orderPayDetail = new Tb_Order_PayDetail();
                                        orderPayDetail.id = Guid.NewGuid();
                                        orderPayDetail.OrderId = mOrder.OrderId;//订单编号
                                        orderPayDetail.PayType = "分账";//支付类型“付款”“收款”“分账”
                                        orderPayDetail.PayMode = 0;//支付方式（见字典表）
                                        orderPayDetail.CpyNo = uCompany.UninCode;//公司编号
                                        orderPayDetail.CpyType = uCompany.RoleType;//公司类型
                                        orderPayDetail.CpyName = uCompany.UninAllName;//公司名称
                                        orderPayDetail.BuyPoundage = 0M;// 交易手续费
                                        orderPayDetail.PayMoney = tempMoneyNew;//交易金额（应收应付）
                                        orderPayDetail.RealPayMoney = 0;//实际交易金额（实收实付）
                                        orderPayDetail.InPayNo = mOrder.InPayNo;//内部流水号
                                        orderPayDetail.PayNo = "";//支付交易流水号
                                        orderPayDetail.ReturnPayNo = "";//退款交易流水号
                                        orderPayDetail.PayAccount = GetPayAccount(uCompany.UninCode, oldPayDetailCount);//收支帐号
                                        orderPayDetailList.Add(orderPayDetail);
                                    }
                                }
                            }
                            #endregion

                            #region 特殊处理

                            if (tempMoney >= 0)
                            {
                                orderPayDetailList[orderPayDetailList.Count - 1].PayMoney += tempMoney;
                                tempMoney = 0;
                            }

                            #endregion

                            #region 手续费

                            //订单账单明细
                            foreach (User_Company item in uCompanys)
                            {
                                if (item.UninCode == PtCpyNo) //手续费：手续费
                                {
                                    uCompany = item;
                                    break;
                                }
                            }

                            orderPayDetail = new Tb_Order_PayDetail();
                            orderPayDetail.id = Guid.NewGuid();
                            orderPayDetail.OrderId = mOrder.OrderId;//订单编号
                            orderPayDetail.PayType = "手续费";//支付类型“付款”“收款”“分账”
                            orderPayDetail.PayMode = 0;//支付方式（见字典表）
                            orderPayDetail.CpyNo = uCompany.UninCode;//公司编号
                            orderPayDetail.CpyType = uCompany.RoleType;//公司类型
                            orderPayDetail.CpyName = uCompany.UninAllName;//公司名称
                            orderPayDetail.BuyPoundage = 0M;//	交易手续费
                            orderPayDetail.PayMoney = 0;//交易金额（应收应付）
                            orderPayDetail.RealPayMoney = 0;//实际交易金额（实收实付）
                            orderPayDetail.InPayNo = mOrder.InPayNo;//内部流水号
                            orderPayDetail.PayNo = "";//支付交易流水号
                            orderPayDetail.ReturnPayNo = "";//退款交易流水号
                            orderPayDetail.PayAccount = GetPayAccount(uCompany.UninCode, oldPayDetailCount);//收支帐号
                            orderPayDetailList.Add(orderPayDetail);

                            #endregion

                            #endregion

                            #region Tb_Ticket_PayDetail 处理

                            //单人金额处理
                            decimal tempPayPayMoneyZF = 0;
                            decimal tempPayPayMoneySK = 0;
                            decimal tempPayPayMoneyFZ = 0;

                            decimal tempPayPayMoneyTemp = 0; //金额之差

                            // 机票账单明细
                            decimal FKtempReturnMoney = d.CreateXianFanPayFee(mOrder.DiscountDetail, returnMoney);

                            foreach (Tb_Ticket_Passenger passenger in mPassenger)
                            {
                                tempPayPayMoneyZF = 0;
                                tempPayPayMoneySK = 0;

                                tempPayPayMoneyZF = d.CreatePassengerPayFee(passenger.PMFee, passenger.ABFee, passenger.FuelFee, returnPoint, FKtempReturnMoney, passenger.PassengerType);
                                tempPayPayMoneySK = d.CreateOrderPayFeeGY(passenger.PMFee, passenger.ABFee, passenger.FuelFee, policyPoint, returnMoney, passenger.PassengerType);

                                #region 付款

                                //订单账单明细
                                foreach (User_Company item in uCompanys)
                                {
                                    if (item.UninCode == ownerCpyNo) //手续费：手续费
                                    {
                                        uCompany = item;
                                        break;
                                    }
                                }

                                ticketPayDetail = new Tb_Ticket_PayDetail();
                                ticketPayDetail.id = Guid.NewGuid();
                                ticketPayDetail.BuyPoundage = 0;
                                ticketPayDetail.CpyNo = uCompany.UninCode;
                                ticketPayDetail.CpyType = uCompany.RoleType;
                                ticketPayDetail.CpyName = uCompany.UninAllName;
                                ticketPayDetail.OrderId = mOrder.OrderId;
                                ticketPayDetail.PayAccount = GetPayAccount(uCompany.UninCode, oldPayDetailCount);
                                ticketPayDetail.PayMode = 0;
                                ticketPayDetail.PayMoney = tempPayPayMoneyZF;
                                ticketPayDetail.PayNo = "";
                                ticketPayDetail.PayType = "付款";
                                ticketPayDetail.RealPayMoney = 0;
                                ticketPayDetail.ReturnPayNo = "";
                                ticketPayDetail.TicketId = passenger.id.ToString();
                                ticketPayDetailList.Add(ticketPayDetail);

                                #endregion

                                #region 收款

                                foreach (User_Company item in uCompanys)
                                {
                                    if (item.UninCode == mOrder.CPCpyNo)
                                    {
                                        uCompany = item;
                                        break;
                                    }
                                }

                                ticketPayDetail = new Tb_Ticket_PayDetail();
                                ticketPayDetail.id = Guid.NewGuid();
                                ticketPayDetail.BuyPoundage = 0;
                                ticketPayDetail.CpyNo = uCompany.UninCode;//
                                ticketPayDetail.CpyType = uCompany.RoleType;//
                                ticketPayDetail.CpyName = uCompany.UninAllName;//
                                ticketPayDetail.OrderId = mOrder.OrderId;
                                ticketPayDetail.PayAccount = GetPayAccount(uCompany.UninCode, oldPayDetailCount);
                                ticketPayDetail.PayMode = 0;
                                ticketPayDetail.PayMoney = tempPayPayMoneySK;
                                ticketPayDetail.PayNo = "";
                                ticketPayDetail.PayType = "收款";
                                ticketPayDetail.RealPayMoney = 0;
                                ticketPayDetail.ReturnPayNo = "";
                                ticketPayDetail.TicketId = passenger.id.ToString();

                                ticketPayDetailList.Add(ticketPayDetail);

                                #endregion

                                // 单人金额之差
                                tempPayPayMoneyTemp = 0;
                                tempPayPayMoneyTemp = tempPayPayMoneyZF - tempPayPayMoneySK;

                                #region 分账

                                if (!string.IsNullOrEmpty(mOrder.DiscountDetail) && mOrder.DiscountDetail != "")
                                {
                                    string[] discountDetails = mOrder.DiscountDetail.Split('|');

                                    for (int i = 0; i < discountDetails.Length; i++)
                                    {
                                        if (!string.IsNullOrEmpty(discountDetails[i]))
                                        {
                                            string[] values = discountDetails[i].Split('^');
                                            tempPoint = decimal.Parse(values[1]);
                                            tempReturnMoney = d.FourToFiveNum(decimal.Parse(values[2]), 2); //现返

                                            foreach (User_Company item in uCompanys)
                                            {
                                                if (item.UninCode == values[0])
                                                {
                                                    uCompany = item;
                                                    break;
                                                }
                                            }

                                            tempPayPayMoneyFZ = 0;
                                            tempPayPayMoneyFZ = d.CreateCommissionFX(passenger.PMFee, tempPoint, tempReturnMoney, passenger.PassengerType);

                                            if (tempPayPayMoneyTemp >= tempPayPayMoneyFZ)
                                            {
                                                tempPayPayMoneyTemp = tempPayPayMoneyTemp - tempPayPayMoneyFZ;
                                            }
                                            else
                                            {
                                                tempPayPayMoneyFZ = tempPayPayMoneyTemp;
                                                tempPayPayMoneyTemp = 0;
                                            }


                                            #region 分账 机票账单明细

                                            ticketPayDetail = new Tb_Ticket_PayDetail();
                                            ticketPayDetail.id = Guid.NewGuid();
                                            ticketPayDetail.BuyPoundage = 0;
                                            ticketPayDetail.CpyNo = uCompany.UninCode;//公司编号
                                            ticketPayDetail.CpyType = uCompany.RoleType;//公司类型
                                            ticketPayDetail.CpyName = uCompany.UninAllName;//公司名称
                                            ticketPayDetail.OrderId = mOrder.OrderId;
                                            ticketPayDetail.PayAccount = GetPayAccount(uCompany.UninCode, oldPayDetailCount);
                                            ticketPayDetail.PayMode = 0;
                                            ticketPayDetail.PayMoney = tempPayPayMoneyFZ;
                                            ticketPayDetail.PayNo = "";
                                            ticketPayDetail.PayType = "分账";
                                            ticketPayDetail.RealPayMoney = 0;
                                            ticketPayDetail.ReturnPayNo = "";
                                            ticketPayDetail.TicketId = passenger.id.ToString();
                                            ticketPayDetailList.Add(ticketPayDetail);

                                            #endregion

                                        }
                                    }
                                }

                                #endregion

                                #region 特殊处理

                                if (tempPayPayMoneyTemp >= 0)
                                {
                                    ticketPayDetailList[ticketPayDetailList.Count - 1].PayMoney += tempPayPayMoneyTemp;
                                    tempPayPayMoneyTemp = 0;
                                }

                                #endregion

                                #region 手续费

                                //订单账单明细
                                foreach (User_Company item in uCompanys)
                                {
                                    if (item.UninCode == PtCpyNo) //手续费：手续费
                                    {
                                        uCompany = item;
                                        break;
                                    }
                                }

                                ticketPayDetail = new Tb_Ticket_PayDetail();
                                ticketPayDetail.id = Guid.NewGuid();
                                ticketPayDetail.BuyPoundage = 0;
                                ticketPayDetail.CpyNo = uCompany.UninCode;//公司编号
                                ticketPayDetail.CpyType = uCompany.RoleType;//公司类型
                                ticketPayDetail.CpyName = uCompany.UninAllName;//公司名称
                                ticketPayDetail.OrderId = mOrder.OrderId;
                                ticketPayDetail.PayAccount = GetPayAccount(uCompany.UninCode, oldPayDetailCount);
                                ticketPayDetail.PayMode = 0;
                                ticketPayDetail.PayMoney = 0;
                                ticketPayDetail.PayNo = mOrder.PayNo;
                                ticketPayDetail.PayType = "手续费";
                                ticketPayDetail.RealPayMoney = 0;
                                ticketPayDetail.ReturnPayNo = "";
                                ticketPayDetail.TicketId = passenger.id.ToString();
                                ticketPayDetailList.Add(ticketPayDetail);

                                #endregion
                            }

                            #endregion

                            #region 生成订单账单明细表、机票账单明细 sql 语句
                            try
                            {
                                string strSql = "";
                                foreach (Tb_Ticket_PayDetail tPayDetail in ticketPayDetailList)
                                {
                                    strSql = Dal.Mapping.MappingHelper<PbProject.Model.Tb_Ticket_PayDetail>.CreateInsertModelSql(tPayDetail);
                                    sqlList.Add(strSql);
                                }

                                foreach (Tb_Order_PayDetail oPayDetail in orderPayDetailList)
                                {
                                    strSql = Dal.Mapping.MappingHelper<PbProject.Model.Tb_Order_PayDetail>.CreateInsertModelSql(oPayDetail);
                                    sqlList.Add(strSql);
                                }
                            }
                            catch (Exception)
                            {

                            }

                            #endregion
                        }
                    }
                }
                catch (Exception ex)
                {

                }
                return sqlList;
            }
        }
        */

        /// <summary>
        /// (新) 确定订单时:生成订单账单明细 Tb_Order_PayDetail、机票账单明细  Tb_Ticket_PayDetail
        /// </summary>
        /// <param name="mOrder">订单mOrder</param>
        /// <param name="mPassenger">乘机人mOrder</param>
        /// <returns>返回sql语句 , out decimal OrderPayMoney,out decimal OrderOrderMoney</returns>
        public List<string> CreateOrderAndTicketPayDetailNew(Tb_Ticket_Order mOrder,
            List<Tb_Ticket_Passenger> mPassenger)
        {

            decimal OrderPayMoney = 0; // 支付金额
            decimal OrderOrderMoney = 0; // 收款金额

            lock (lockobject)
            {
                List<string> sqlList = new List<string>();

                try
                {
                    string OwnerCpyNo = mOrder.OwnerCpyNo; //订单方:付款
                    string CPCpyNo = mOrder.CPCpyNo;//出票方：收款
                    string PtCpyNo = mOrder.OwnerCpyNo.Substring(0, 6);//手续费：手续费

                    decimal policyPoint = mOrder.PolicyPoint;//政策原始返点
                    decimal returnPoint = mOrder.ReturnPoint; //最终返点
                    decimal returnMoney = mOrder.ReturnMoney;//政策现返金额

                    decimal tempPoint = 0;//扣点:临时变量
                    decimal tempReturnMoney = 0; //现返:临时变量

                    string tempSqlWhere = ""; //查询条件

                    List<Tb_Order_PayDetail> oldPayDetailCount = null;

                    if (mOrder.OrderStatusCode != 1 && !string.IsNullOrEmpty(mOrder.OldOrderId))
                        oldPayDetailCount = new Dal.ControlBase.BaseData<Tb_Order_PayDetail>().GetList(" OrderId='" + mOrder.OldOrderId + "' ");

                    List<Tb_Order_PayDetail> PayDetailCount = new Dal.ControlBase.BaseData<Tb_Order_PayDetail>().GetList(" OrderId='" + mOrder.OrderId + "' ");

                    DataAction d = new DataAction();

                    Data data = new Data(mOrder.OwnerCpyNo);

                    List<Tb_Order_PayDetail> orderPayDetailList = new List<Tb_Order_PayDetail>();
                    List<Tb_Ticket_PayDetail> ticketPayDetailList = new List<Tb_Ticket_PayDetail>();

                    Tb_Order_PayDetail orderPayDetail = null;
                    Tb_Ticket_PayDetail ticketPayDetail = null;
                    User_Company uCompany = null;

                    #region 获取所有收款公司信息

                    string CompanyNoS = ""; //参与交易所有公司信息
                    CompanyNoS += "'" + OwnerCpyNo + "',";//订单方
                    if (!CompanyNoS.Contains("'" + CPCpyNo + "'"))
                        CompanyNoS += "'" + CPCpyNo + "',";//出票方
                    if (!CompanyNoS.Contains("'" + PtCpyNo + "'"))
                        CompanyNoS += "'" + PtCpyNo + "',"; //手续费公司

                    //其它收款公司
                    if (!string.IsNullOrEmpty(mOrder.DiscountDetail) && mOrder.DiscountDetail != "")
                    {
                        string[] discountDetails = mOrder.DiscountDetail.Split('|');

                        for (int i = 0; i < discountDetails.Length; i++)
                        {
                            if (!string.IsNullOrEmpty(discountDetails[i]))
                            {
                                string[] values = discountDetails[i].Split('^');
                                if (!CompanyNoS.Contains("'" + values[0] + "'"))
                                    CompanyNoS += "'" + values[0] + "',";
                            }
                        }
                    }
                    CompanyNoS = CompanyNoS.TrimEnd(',');

                    tempSqlWhere = " UninCode in(" + CompanyNoS + ")";
                    List<User_Company> uCompanys = new PbProject.Logic.User.User_CompanyBLL().GetListBySqlWhere(tempSqlWhere);

                    #endregion

                    #region Tb_Ticket_PayDetail 处理

                    decimal FKtempReturnMoney = data.CreateXianFanPayFee(mOrder.DiscountDetail, returnMoney);


                    string tempCpyNo = ""; //临时变量 公司编码
                    string tempPayType = "";

                    decimal PayMoney = 0; //临时变量   金额 
                    decimal sxfMoney = 0;

                    decimal passengerPayMoney = 0; //单人支付价格
                    decimal passengerOrderMoney = 0;//单人收款价格
                    decimal passengerFZMoney = 0; //单人分账金额

                    decimal cgyoujin = 0;//采购佣金
                    decimal gyyoujin = 0;//采购佣金
                    decimal tempYoujin = 0;

                    foreach (Tb_Ticket_Passenger passenger in mPassenger)
                    {
                        //单人金额处理

                        passengerPayMoney = data.CreatePassengerPayFee(passenger.PMFee, passenger.ABFee, passenger.FuelFee, returnPoint, FKtempReturnMoney, passenger.PassengerType);
                        passengerOrderMoney = data.CreateOrderPayFeeGY(passenger.PMFee, passenger.ABFee, passenger.FuelFee, policyPoint, returnMoney, passenger.PassengerType);


                        cgyoujin = data.CreateCommissionCG(passenger.PMFee, returnPoint); ////采购佣金
                        gyyoujin = data.CreateCommissionCG(passenger.PMFee, policyPoint); ////供应佣金

                        passengerFZMoney = 0;
                        PayMoney = 0;
                        sxfMoney = 0;
                        tempCpyNo = "";
                        tempPayType = "";

                        #region 分账

                        if (!string.IsNullOrEmpty(mOrder.DiscountDetail) && mOrder.DiscountDetail != "")
                        {
                            string[] discountDetails = mOrder.DiscountDetail.Trim('|').Split('|');

                            for (int i = 0; i < discountDetails.Length; i++)
                            {
                                PayMoney = 0;

                                if (!string.IsNullOrEmpty(discountDetails[i]))
                                {
                                    string[] values = discountDetails[i].Split('^');
                                    tempPoint = decimal.Parse(values[1]);
                                    tempReturnMoney = d.FourToFiveNum(decimal.Parse(values[2]), 2); //现返

                                    foreach (User_Company item in uCompanys)
                                    {
                                        if (item.UninCode == values[0])
                                        {
                                            uCompany = item;
                                            break;
                                        }
                                    }

                                    tempPayType = "分账";
                                    PayMoney = data.CreateCommissionFX(passenger.PMFee, tempPoint, tempReturnMoney, passenger.PassengerType);

                                    passengerFZMoney += PayMoney;

                                    #region 分账 机票账单明细

                                    ticketPayDetail = new Tb_Ticket_PayDetail();
                                    ticketPayDetail.id = Guid.NewGuid();
                                    ticketPayDetail.BuyPoundage = 0;
                                    ticketPayDetail.CpyNo = uCompany.UninCode;//公司编号
                                    ticketPayDetail.CpyType = uCompany.RoleType;//公司类型
                                    ticketPayDetail.CpyName = uCompany.UninAllName;//公司名称
                                    ticketPayDetail.OrderId = mOrder.OrderId;
                                    ticketPayDetail.PayAccount = GetPayAccount(uCompany.UninCode, oldPayDetailCount);
                                    ticketPayDetail.PayMode = 0;
                                    ticketPayDetail.PayMoney = PayMoney;
                                    ticketPayDetail.PayNo = "";
                                    ticketPayDetail.PayType = tempPayType;
                                    ticketPayDetail.RealPayMoney = 0;
                                    ticketPayDetail.ReturnPayNo = "";
                                    ticketPayDetail.TicketId = passenger.id.ToString();




                                    ticketPayDetailList.Add(ticketPayDetail);

                                    #endregion

                                }
                            }
                        }

                        #endregion

                        #region 付款、收款、手续费

                        for (int i = 0; i < 3; i++)
                        {
                            #region

                            tempPayType = "";

                            switch (i)
                            {
                                case 0:
                                    tempPayType = "付款";
                                    tempCpyNo = mOrder.OwnerCpyNo;
                                    PayMoney = passengerPayMoney;
                                    OrderPayMoney += PayMoney; //计算订单支付金额
                                    tempYoujin = cgyoujin;
                                    break;

                                case 1:
                                    tempPayType = "手续费";
                                    tempCpyNo = mOrder.OwnerCpyNo.Substring(0, 6);
                                    //PayMoney = passengerPayMoney - passengerOrderMoney - passengerFZMoney; //手续费
                                    PayMoney = 0;
                                    break;

                                case 2:
                                    tempPayType = "收款";
                                    tempCpyNo = mOrder.CPCpyNo;
                                    PayMoney = passengerOrderMoney;
                                    sxfMoney = passengerPayMoney - passengerOrderMoney - passengerFZMoney; //手续费

                                    tempYoujin = gyyoujin;

                                    if (sxfMoney >= 0)
                                    {
                                        PayMoney += sxfMoney;
                                    }
                                    OrderOrderMoney += PayMoney; //计算订单收款金额
                                    break;

                                default:
                                    break;
                            }

                            #endregion

                            //订单账单明细
                            foreach (User_Company item in uCompanys)
                            {
                                if (item.UninCode == tempCpyNo) //手续费：手续费
                                {
                                    uCompany = item;
                                    break;
                                }
                            }

                            ticketPayDetail = new Tb_Ticket_PayDetail();
                            ticketPayDetail.id = Guid.NewGuid();
                            ticketPayDetail.BuyPoundage = 0;
                            ticketPayDetail.CpyNo = uCompany.UninCode;
                            ticketPayDetail.CpyType = uCompany.RoleType;
                            ticketPayDetail.CpyName = uCompany.UninAllName;
                            ticketPayDetail.OrderId = mOrder.OrderId;
                            ticketPayDetail.PayAccount = GetPayAccount(uCompany.UninCode, oldPayDetailCount);
                            ticketPayDetail.PayMode = 0;
                            ticketPayDetail.PayMoney = PayMoney;
                            ticketPayDetail.PayNo = "";
                            ticketPayDetail.PayType = tempPayType;
                            ticketPayDetail.RealPayMoney = 0;
                            ticketPayDetail.ReturnPayNo = "";
                            ticketPayDetail.TicketId = passenger.id.ToString();
                            ticketPayDetail.A3 = tempYoujin; //佣金

                            
                            ticketPayDetailList.Add(ticketPayDetail);
                        }

                        #endregion
                    }

                    #endregion

                    #region Tb_Order_PayDetail 多人累计

                    #region 付款、收款、手续费

                    for (int i = 0; i < 3; i++)
                    {
                        tempPayType = "";
                        PayMoney = 0;
                        tempCpyNo = "";

                        switch (i)
                        {
                            case 0:
                                tempCpyNo = mOrder.OwnerCpyNo;
                                tempPayType = "付款";
                                PayMoney = OrderPayMoney;
                                break;


                            case 1:
                                tempCpyNo = mOrder.OwnerCpyNo.Substring(0, 6);
                                tempPayType = "手续费";
                                break;

                            case 2:
                                tempCpyNo = mOrder.CPCpyNo;
                                tempPayType = "收款";
                                PayMoney = OrderOrderMoney;
                                break;

                            default:
                                break;
                        }

                        #region 获取公司信息

                        foreach (User_Company item in uCompanys)
                        {
                            if (item.UninCode == tempCpyNo)
                            {
                                uCompany = item;
                                break;
                            }
                        }

                        #endregion

                        orderPayDetail = new Tb_Order_PayDetail();
                        orderPayDetail.id = Guid.NewGuid();
                        orderPayDetail.OrderId = mOrder.OrderId;//订单编号
                        orderPayDetail.PayType = tempPayType;
                        orderPayDetail.PayMode = 0;//支付方式（见字典表）
                        orderPayDetail.CpyNo = uCompany.UninCode;//公司编号
                        orderPayDetail.CpyType = uCompany.RoleType;//公司类型
                        orderPayDetail.CpyName = uCompany.UninAllName;//公司名称
                        orderPayDetail.BuyPoundage = 0M;//	交易手续费
                        orderPayDetail.PayMoney = PayMoney;//交易金额（应收应付）
                        orderPayDetail.RealPayMoney = 0;//实际交易金额（实收实付）
                        orderPayDetail.InPayNo = mOrder.InPayNo;//内部流水号
                        orderPayDetail.PayNo = "";//支付交易流水号
                        orderPayDetail.ReturnPayNo = "";//退款交易流水号
                        orderPayDetail.PayAccount = GetPayAccount(uCompany.UninCode, oldPayDetailCount);//收支帐号
                        orderPayDetailList.Add(orderPayDetail);
                    }

                    #endregion

                    #region 分账

                    if (!string.IsNullOrEmpty(mOrder.DiscountDetail) && mOrder.DiscountDetail != "")
                    {
                        string[] discountDetails = mOrder.DiscountDetail.Trim('|').Split('|');

                        for (int i = 0; i < discountDetails.Length; i++)
                        {
                            if (!string.IsNullOrEmpty(discountDetails[i]))
                            {
                                string[] values = discountDetails[i].Split('^');
                                tempPoint = decimal.Parse(values[1]);
                                tempReturnMoney = d.FourToFiveNum(decimal.Parse(values[2]), 2); //现返

                                foreach (User_Company item in uCompanys)
                                {
                                    if (item.UninCode == values[0])
                                    {
                                        uCompany = item;
                                        break;
                                    }
                                }

                                PayMoney = 0;
                                if (mPassenger != null && mPassenger.Count > 0)
                                {
                                    //通过扣点计算金额
                                    foreach (Tb_Ticket_Passenger passenger in mPassenger)
                                    {
                                        PayMoney += data.CreateCommissionFX(passenger.PMFee, tempPoint, tempReturnMoney, passenger.PassengerType);
                                    }
                                }

                                orderPayDetail = new Tb_Order_PayDetail();
                                orderPayDetail.id = Guid.NewGuid();
                                orderPayDetail.OrderId = mOrder.OrderId;//订单编号
                                orderPayDetail.PayType = "分账";//支付类型“付款”“收款”“分账”
                                orderPayDetail.PayMode = 0;//支付方式（见字典表）
                                orderPayDetail.CpyNo = uCompany.UninCode;//公司编号
                                orderPayDetail.CpyType = uCompany.RoleType;//公司类型
                                orderPayDetail.CpyName = uCompany.UninAllName;//公司名称
                                orderPayDetail.BuyPoundage = 0M;// 交易手续费
                                orderPayDetail.PayMoney = PayMoney;//交易金额（应收应付）
                                orderPayDetail.RealPayMoney = 0;//实际交易金额（实收实付）
                                orderPayDetail.InPayNo = mOrder.InPayNo;//内部流水号
                                orderPayDetail.PayNo = "";//支付交易流水号
                                orderPayDetail.ReturnPayNo = "";//退款交易流水号
                                orderPayDetail.PayAccount = GetPayAccount(uCompany.UninCode, oldPayDetailCount);//收支帐号
                                orderPayDetailList.Add(orderPayDetail);
                            }
                        }
                    }
                    #endregion

                    #endregion

                    if (PayDetailCount == null || PayDetailCount.Count == 0) // 判断订单是否存在
                    {
                        #region 生成订单账单明细表、机票账单明细 sql 语句
                        try
                        {
                            string strSql = "";
                            foreach (Tb_Ticket_PayDetail tPayDetail in ticketPayDetailList)
                            {
                                strSql = Dal.Mapping.MappingHelper<PbProject.Model.Tb_Ticket_PayDetail>.CreateInsertModelSql(tPayDetail);
                                sqlList.Add(strSql);
                            }

                            foreach (Tb_Order_PayDetail oPayDetail in orderPayDetailList)
                            {
                                strSql = Dal.Mapping.MappingHelper<PbProject.Model.Tb_Order_PayDetail>.CreateInsertModelSql(oPayDetail);
                                sqlList.Add(strSql);
                            }
                        }
                        catch (Exception)
                        {

                        }

                        #endregion
                    }

                    mOrder.PayMoney = OrderPayMoney;// 支付金额
                    mOrder.OrderMoney = OrderOrderMoney;// 收款金额


                }
                catch (Exception ex)
                {

                }
                return sqlList;
            }
        }

        /// <summary>
        /// 切换支付方式：修改 订单账单明细 Tb_Order_PayDetail、机票账单明细  Tb_Ticket_PayDetail
        /// </summary>
        /// <param name="mOrder">订单mOrder</param>
        /// <returns></returns>
        public bool UpdateOrderAndTicketPayDetail(Tb_Ticket_Order mOrder)
        {
            lock (lockobject)
            {
                bool result = false;
                try
                {
                    if (mOrder != null)
                    {
                        List<string> sqlUpdateList = new List<string>(); //sql语句

                        StringBuilder tempSql = null; //sql
                        string tempSqlWhere = "";//查询条件
                        DataAction d = new DataAction();
                        decimal Rate = 0;//承担手续费费率
                        string PTAct = ""; //手续费账号 

                        if (mOrder.PayWay == 0 || mOrder.PayWay == 15)
                        {
                            #region 收银

                            //BuyPoundage  手续费
                            //PayMoney  收支金额
                            //RealPayMoney  实际收支金额

                            tempSql = new StringBuilder();
                            tempSql.Append(" update Tb_Order_PayDetail set ");
                            tempSql.Append(" PayMode=" + mOrder.PayWay + ", ");
                            tempSql.Append(" BuyPoundage=0, ");
                            tempSql.Append(" PayAccount='', ");
                            tempSql.Append(" RealPayMoney=PayMoney ");
                            tempSql.Append(" where OrderId='" + mOrder.OrderId + "' ");
                            tempSql.Append(" and (PayType='付款' or PayType='收款') ");
                            sqlUpdateList.Add(tempSql.ToString());

                            tempSql = new StringBuilder();
                            tempSql.Append(" update Tb_Order_PayDetail set ");
                            tempSql.Append(" PayMode=" + mOrder.PayWay + ", ");
                            tempSql.Append(" BuyPoundage=0, ");
                            tempSql.Append(" PayAccount='', ");
                            if (mOrder.OrderStatusCode == 9)//改签
                            {
                                tempSql.Append(" PayMoney=0, ");
                            }
                            tempSql.Append(" RealPayMoney=PayMoney ");
                            tempSql.Append(" where OrderId='" + mOrder.OrderId + "' ");
                            tempSql.Append(" and (PayType='手续费' or PayType='分账') ");
                            sqlUpdateList.Add(tempSql.ToString());

                            tempSql = new StringBuilder();
                            tempSql.Append(" update Tb_Ticket_PayDetail set ");
                            tempSql.Append(" PayMode=" + mOrder.PayWay + ", ");
                            tempSql.Append(" BuyPoundage=0, ");
                            tempSql.Append(" PayAccount='', ");
                            tempSql.Append(" RealPayMoney=PayMoney ");
                            tempSql.Append(" where OrderId='" + mOrder.OrderId + "' ");
                            tempSql.Append(" and (PayType='付款' or PayType='收款') ");
                            sqlUpdateList.Add(tempSql.ToString());

                            tempSql = new StringBuilder();
                            tempSql.Append(" update Tb_Ticket_PayDetail set ");
                            tempSql.Append(" PayMode=" + mOrder.PayWay + ", ");
                            tempSql.Append(" BuyPoundage=0, ");
                            tempSql.Append(" PayAccount='', ");
                            if (mOrder.OrderStatusCode == 9) //改签
                            {
                                tempSql.Append(" PayMoney=0, ");
                            }
                            tempSql.Append(" RealPayMoney=PayMoney ");
                            tempSql.Append(" where OrderId='" + mOrder.OrderId + "' ");
                            tempSql.Append(" and (PayType='手续费' or PayType='分账') ");

                            sqlUpdateList.Add(tempSql.ToString());

                            #endregion
                        }
                        else if (mOrder.PayWay == 14)
                        {
                            #region 账号支付

                            //BuyPoundage  手续费
                            //PayMoney  收支金额
                            //RealPayMoney  实际收支金额

                            tempSql = new StringBuilder();
                            tempSql.Append(" update Tb_Order_PayDetail set ");
                            tempSql.Append(" PayMode=" + mOrder.PayWay + ", ");
                            tempSql.Append(" BuyPoundage=0, ");
                            tempSql.Append(" PayAccount='', ");
                            tempSql.Append(" RealPayMoney=PayMoney ");
                            tempSql.Append(" where OrderId='" + mOrder.OrderId + "' ");
                            tempSql.Append(" and (PayType='付款' or PayType='收款') ");
                            sqlUpdateList.Add(tempSql.ToString());

                            tempSql = new StringBuilder();
                            tempSql.Append(" update Tb_Order_PayDetail set ");
                            tempSql.Append(" PayMode=" + mOrder.PayWay + ", ");
                            tempSql.Append(" BuyPoundage=0, ");
                            tempSql.Append(" PayAccount='', ");

                            if (mOrder.OrderStatusCode == 9)//改签
                            {
                                tempSql.Append(" PayMoney=0, ");
                                tempSql.Append(" RealPayMoney=0 ");
                            }
                            else
                            {
                                tempSql.Append(" RealPayMoney=PayMoney ");
                            }

                            tempSql.Append(" where OrderId='" + mOrder.OrderId + "' ");
                            tempSql.Append(" and (PayType='手续费' or PayType='分账') ");
                            sqlUpdateList.Add(tempSql.ToString());

                            tempSql = new StringBuilder();
                            tempSql.Append(" update Tb_Ticket_PayDetail set ");
                            tempSql.Append(" PayMode=" + mOrder.PayWay + ", ");
                            tempSql.Append(" BuyPoundage=0, ");
                            tempSql.Append(" PayAccount='', ");
                            tempSql.Append(" RealPayMoney=PayMoney ");
                            tempSql.Append(" where OrderId='" + mOrder.OrderId + "' ");
                            tempSql.Append(" and (PayType='付款' or PayType='收款') ");
                            sqlUpdateList.Add(tempSql.ToString());

                            tempSql = new StringBuilder();
                            tempSql.Append(" update Tb_Ticket_PayDetail set ");
                            tempSql.Append(" PayMode=" + mOrder.PayWay + ", ");
                            tempSql.Append(" BuyPoundage=0, ");
                            tempSql.Append(" PayAccount='', ");

                            if (mOrder.OrderStatusCode == 9) //改签
                            {
                                tempSql.Append(" PayMoney=0, ");
                                tempSql.Append(" RealPayMoney=0 ");
                            }
                            else
                            {
                                tempSql.Append(" RealPayMoney=PayMoney ");
                            }

                            tempSql.Append(" where OrderId='" + mOrder.OrderId + "' ");
                            tempSql.Append(" and (PayType='手续费' or PayType='分账') ");

                            sqlUpdateList.Add(tempSql.ToString());
                            #endregion
                        }
                        else
                        {
                            #region 在线交易

                            // 支付方式:1 支付宝、2 快钱 、3 汇付、4 财付通、5 支付宝网银、5 快钱网银 、7 汇付网银、8 财付通网银、 9 支付宝pos、10 快钱pos 、11 汇付pos、12 财付通pos、  13 易宝pos 、14、账户支付

                            // 获取供应收款账号 和 承担手续费
                            tempSqlWhere = " CpyNo in(select CpyNo from Tb_Order_PayDetail where OrderId='" + mOrder.OrderId + "')";
                            tempSqlWhere += "  and SetName='" + PbProject.Model.definitionParam.paramsName.wangYinZhangHao + "'";

                            List<Bd_Base_Parameters> bParametersList = new PbProject.Logic.ControlBase.Bd_Base_ParametersBLL().GetParametersListByWhere(tempSqlWhere);
                            if (bParametersList != null && bParametersList.Count > 0 && !string.IsNullOrEmpty(bParametersList[0].SetValue))
                            {
                                //截取支付方式
                                int tempNo = mOrder.PayWay % 4 - 1;
                                tempNo = (tempNo < 0) ? tempNo + 4 : tempNo;

                                tempSqlWhere = "OrderId='" + mOrder.OrderId + "' ";
                                List<Tb_Order_PayDetail> orderPayDetail = new Dal.ControlBase.BaseData<Tb_Order_PayDetail>().GetList(tempSqlWhere);
                                List<Tb_Ticket_PayDetail> ticketPayDetail = new Dal.ControlBase.BaseData<Tb_Ticket_PayDetail>().GetList(tempSqlWhere);

                                if (orderPayDetail != null && orderPayDetail.Count > 0 && ticketPayDetail != null && ticketPayDetail.Count > 0)
                                {
                                    #region

                                    decimal AllPayMoney = 0; // 收款累计
                                    decimal AllRealPayMoney = 0; // 收款累计
                                    decimal orderRealPayMoney = 0; // 订单交易手续费

                                    // 在线交易手续费
                                    foreach (Bd_Base_Parameters item in bParametersList)
                                    {
                                        if (item.CpyNo == mOrder.CPCpyNo)
                                        {
                                            if (mOrder.CPCpyNo == mOrder.OwnerCpyNo.Substring(0, 12))
                                            {
                                                // 本地用户
                                                Rate = decimal.Parse(item.SetValue.Split('|')[tempNo].Split('^')[2]);//承担手续费
                                            }
                                            else
                                            {
                                                // 共享用户 
                                                Rate = decimal.Parse(item.SetValue.Split('|')[tempNo].Split('^')[3]);//承担手续费
                                            }
                                            break;
                                        }
                                    }

                                    string ptCpyNo = mOrder.CPCpyNo.Substring(0, 6); // 平台公司编号
                                    // 获取平台收款账号
                                    int jCount = bParametersList.Count;
                                    for (int j = 0; j < jCount; j++)
                                    {
                                        if (bParametersList[j] != null && !string.IsNullOrEmpty(bParametersList[j].SetValue) && ptCpyNo == bParametersList[j].CpyNo)
                                        {
                                            PTAct = bParametersList[j].SetValue.Split('|')[tempNo].Split('^')[0]; //平台收款账号
                                            break;
                                        }
                                    }

                                    #endregion

                                    #region 处理  Tb_Order_PayDetail

                                    int iCount = orderPayDetail.Count;
                                    Tb_Order_PayDetail ptOrderPayDetail = null;//临时变量：处理手续费
                                    Tb_Order_PayDetail tempOrderPayDetail = null; //临时变量

                                    for (int i = 0; i < iCount; i++)
                                    {
                                        tempOrderPayDetail = orderPayDetail[i];

                                        #region  参数表 获取收款账号
                                        // 获取收款账号
                                        for (int j = 0; j < jCount; j++)
                                        {
                                            if (tempOrderPayDetail.CpyNo == bParametersList[j].CpyNo &&
                                                bParametersList[j] != null && !string.IsNullOrEmpty(bParametersList[j].SetValue))
                                            {
                                                tempOrderPayDetail.PayAccount = bParametersList[j].SetValue.Split('|')[tempNo].Split('^')[0]; //收支帐号
                                                break;
                                            }
                                        }
                                        //分账时没有绑定收款账号 默认平台收款账号
                                        if (string.IsNullOrEmpty(tempOrderPayDetail.PayAccount) && (tempOrderPayDetail.PayType == "分账"))
                                        {
                                            tempOrderPayDetail.PayAccount = PTAct;
                                            tempOrderPayDetail.A1 = 1; //暂时没用
                                        }

                                        #endregion

                                        if (mOrder.OrderStatusCode == 9)
                                        {
                                            #region 手续费 处理改签流程

                                            if (tempOrderPayDetail.PayType == "分账")
                                            {
                                                tempOrderPayDetail.PayAccount = "";
                                                tempOrderPayDetail.BuyPoundage = 0;
                                                tempOrderPayDetail.RealPayMoney = 0;
                                            }
                                            else if (tempOrderPayDetail.PayType == "收款")
                                            {
                                                tempOrderPayDetail.BuyPoundage = d.FourToFiveNum(tempOrderPayDetail.PayMoney * Rate, 2);
                                                tempOrderPayDetail.RealPayMoney = tempOrderPayDetail.PayMoney - tempOrderPayDetail.BuyPoundage;
                                                AllRealPayMoney = tempOrderPayDetail.RealPayMoney;
                                            }
                                            else if (tempOrderPayDetail.PayType == "付款")
                                            {
                                                tempOrderPayDetail.PayAccount = "";
                                                tempOrderPayDetail.BuyPoundage = d.FourToFiveNum(tempOrderPayDetail.PayMoney * Rate, 2);
                                                tempOrderPayDetail.RealPayMoney = tempOrderPayDetail.PayMoney - tempOrderPayDetail.BuyPoundage;
                                            }
                                            else if (tempOrderPayDetail.PayType == "手续费")
                                            {
                                                ptOrderPayDetail = tempOrderPayDetail;
                                                ptOrderPayDetail.PayAccount = PTAct;
                                                continue;
                                            }

                                            tempSql = new StringBuilder();
                                            tempSql.Append(" update Tb_Order_PayDetail set ");
                                            tempSql.Append(" PayMode=" + mOrder.PayWay + ", ");
                                            if (tempOrderPayDetail.PayType == "分账")
                                                tempSql.Append(" PayMoney=0, ");
                                            tempSql.Append(" PayAccount='" + tempOrderPayDetail.PayAccount + "', ");
                                            tempSql.Append(" BuyPoundage='" + tempOrderPayDetail.BuyPoundage + "', ");
                                            tempSql.Append(" RealPayMoney='" + tempOrderPayDetail.RealPayMoney + "' ");
                                            tempSql.Append(" where id='" + tempOrderPayDetail.id.ToString() + "' ");

                                            sqlUpdateList.Add(tempSql.ToString());

                                            #endregion
                                        }
                                        else
                                        {
                                            #region 普通支付 手续费

                                            if (tempOrderPayDetail.PayType == "付款")
                                            {
                                                tempOrderPayDetail.PayAccount = ""; //付款账号
                                                tempOrderPayDetail.BuyPoundage = d.FourToFiveNum(tempOrderPayDetail.PayMoney * Rate, 2); //手续费
                                                orderRealPayMoney = tempOrderPayDetail.PayMoney - tempOrderPayDetail.BuyPoundage;//实际交易金额（实收实付）
                                            }
                                            else if (tempOrderPayDetail.PayType == "收款")
                                            {
                                                tempOrderPayDetail.BuyPoundage = d.FourToFiveNum(tempOrderPayDetail.PayMoney * Rate, 2); //手续费
                                            }
                                            else if (tempOrderPayDetail.PayType == "分账")
                                            {
                                                //分账
                                                for (int j = 0; j < jCount; j++)
                                                {
                                                    // 获取收款账号
                                                    if (tempOrderPayDetail.CpyNo == bParametersList[j].CpyNo && tempOrderPayDetail.PayMoney > 0)
                                                    {
                                                        tempOrderPayDetail.BuyPoundage = d.FourToFiveNum(tempOrderPayDetail.PayMoney * Rate, 1); //手续费

                                                        if (tempOrderPayDetail.BuyPoundage <= 0.1M)
                                                            tempOrderPayDetail.BuyPoundage = 0.1M;  //不足1毛按 1 毛计算
                                                    }
                                                }
                                            }
                                            else if (tempOrderPayDetail.PayType == "手续费")
                                            {
                                                ptOrderPayDetail = tempOrderPayDetail;
                                                ptOrderPayDetail.PayAccount = PTAct;//收款账号
                                                continue; //先不计算平台手续费
                                            }

                                            tempOrderPayDetail.RealPayMoney = tempOrderPayDetail.PayMoney - tempOrderPayDetail.BuyPoundage;//实际交易金额（实收实付）

                                            if (tempOrderPayDetail.RealPayMoney < 0)
                                                tempOrderPayDetail.RealPayMoney = 0;

                                            if (tempOrderPayDetail.PayType == "收款" || tempOrderPayDetail.PayType == "分账")
                                            {
                                                AllPayMoney += tempOrderPayDetail.PayMoney;
                                                AllRealPayMoney += tempOrderPayDetail.RealPayMoney;
                                            }

                                            tempSql = new StringBuilder();
                                            tempSql.Append(" update Tb_Order_PayDetail set ");
                                            tempSql.Append(" A1=" + tempOrderPayDetail.A1 + ", ");
                                            tempSql.Append(" PayMode=" + mOrder.PayWay + ", ");
                                            tempSql.Append(" PayAccount='" + tempOrderPayDetail.PayAccount + "', ");
                                            tempSql.Append(" BuyPoundage='" + tempOrderPayDetail.BuyPoundage + "', ");
                                            tempSql.Append(" RealPayMoney='" + tempOrderPayDetail.RealPayMoney + "' ");
                                            tempSql.Append(" where id='" + tempOrderPayDetail.id.ToString() + "' ");

                                            sqlUpdateList.Add(tempSql.ToString());

                                            #endregion
                                        }
                                    }

                                    #region  手续费

                                    if (ptOrderPayDetail != null)
                                    {
                                        if (mOrder.OrderStatusCode == 9)
                                        {
                                            //改签
                                            ptOrderPayDetail.PayMoney = 0;
                                            ptOrderPayDetail.BuyPoundage = d.FourToFiveNum(mOrder.PayMoney * 0.001M, 2); //手续费
                                            ptOrderPayDetail.RealPayMoney = mOrder.PayMoney - tempOrderPayDetail.BuyPoundage - AllRealPayMoney;//实际交易金额（实收实付）


                                            if (mOrder.PayWay == 4 || mOrder.PayWay == 8 || mOrder.PayWay==40) //财付通特殊处理
                                            {
                                                ptOrderPayDetail.RealPayMoney = mOrder.PayMoney - AllRealPayMoney;//实际交易金额（实收实付）
                                            }
                                            else if (mOrder.PayWay == 2 || mOrder.PayWay == 6)
                                            {
                                                ptOrderPayDetail.RealPayMoney = 0;
                                            }
                                        }
                                        else
                                        {
                                            //手续费
                                            ptOrderPayDetail.BuyPoundage = d.FourToFiveNum(mOrder.PayMoney * 0.001M, 2);
                                            ptOrderPayDetail.PayMoney = mOrder.PayMoney - AllPayMoney;
                                            ptOrderPayDetail.RealPayMoney = mOrder.PayMoney - AllRealPayMoney - ptOrderPayDetail.BuyPoundage;

                                            if (mOrder.PayWay == 4 || mOrder.PayWay == 8 || mOrder.PayWay == 40) //财付通特殊处理
                                            {
                                                ptOrderPayDetail.RealPayMoney = mOrder.PayMoney - AllRealPayMoney;
                                            }
                                        }

                                        tempSql = new StringBuilder();
                                        tempSql.Append(" update Tb_Order_PayDetail set ");
                                        tempSql.Append(" PayMode=" + mOrder.PayWay + ", ");
                                        tempSql.Append(" PayAccount='" + ptOrderPayDetail.PayAccount.ToString() + "', ");
                                        tempSql.Append(" PayMoney='" + ptOrderPayDetail.PayMoney + "', ");
                                        tempSql.Append(" BuyPoundage='" + ptOrderPayDetail.BuyPoundage + "', ");
                                        tempSql.Append(" RealPayMoney='" + ptOrderPayDetail.RealPayMoney + "' ");
                                        tempSql.Append(" where id='" + ptOrderPayDetail.id.ToString() + "' ");

                                        sqlUpdateList.Add(tempSql.ToString());


                                    }
                                    #endregion

                                    #endregion

                                    #region 处理  Tb_Ticket_PayDetail

                                    List<string> strTemp = new PbProject.Logic.Pay.Bill().GetTicketIdByTicketPayDetail(ticketPayDetail);

                                    foreach (string ticketId in strTemp)
                                    {
                                        orderRealPayMoney = 0;

                                        AllPayMoney = 0; // 收款累计
                                        AllRealPayMoney = 0; // 收款累计
                                        orderRealPayMoney = 0; // 

                                        int kCount = ticketPayDetail.Count;
                                        Tb_Ticket_PayDetail ptTicketPayDetail = null;//临时变量：处理手续费
                                        Tb_Ticket_PayDetail tempTicketPayDetail = null; //临时变量

                                        for (int k = 0; k < kCount; k++)
                                        {
                                            tempTicketPayDetail = ticketPayDetail[k];

                                            if (tempTicketPayDetail.TicketId != ticketId)
                                            {
                                                continue;
                                            }

                                            #region  参数表 获取收款账号
                                            // 获取收款账号
                                            for (int j = 0; j < jCount; j++)
                                            {
                                                if (tempTicketPayDetail.CpyNo == bParametersList[j].CpyNo && bParametersList[j] != null && !string.IsNullOrEmpty(bParametersList[j].SetValue))
                                                {
                                                    tempTicketPayDetail.PayAccount = bParametersList[j].SetValue.Split('|')[tempNo].Split('^')[0]; //收支帐号
                                                    break;
                                                }
                                            }
                                            //分账时没有绑定收款账号 默认平台收款账号
                                            if (string.IsNullOrEmpty(tempTicketPayDetail.PayAccount) && (tempTicketPayDetail.PayType == "分账"))
                                                tempTicketPayDetail.PayAccount = PTAct;

                                            #endregion

                                            if (mOrder.OrderStatusCode == 9)
                                            {
                                                #region 手续费 处理改签流程

                                                if (tempTicketPayDetail.PayType == "分账")
                                                {
                                                    tempTicketPayDetail.PayAccount = "";
                                                    tempTicketPayDetail.BuyPoundage = 0;
                                                    tempTicketPayDetail.RealPayMoney = 0;
                                                }
                                                else if (tempTicketPayDetail.PayType == "收款")
                                                {
                                                    tempTicketPayDetail.BuyPoundage = d.FourToFiveNum(tempTicketPayDetail.PayMoney * Rate, 2);
                                                    tempTicketPayDetail.RealPayMoney = tempTicketPayDetail.PayMoney - tempTicketPayDetail.BuyPoundage;
                                                    AllRealPayMoney = tempTicketPayDetail.RealPayMoney;
                                                }
                                                else if (tempTicketPayDetail.PayType == "付款")
                                                {
                                                    tempTicketPayDetail.PayAccount = "";
                                                    tempTicketPayDetail.BuyPoundage = d.FourToFiveNum(tempTicketPayDetail.PayMoney * Rate, 2);
                                                    tempTicketPayDetail.RealPayMoney = tempTicketPayDetail.PayMoney - tempTicketPayDetail.BuyPoundage;
                                                }
                                                else if (tempTicketPayDetail.PayType == "手续费")
                                                {
                                                    ptTicketPayDetail = tempTicketPayDetail;
                                                    ptTicketPayDetail.PayAccount = PTAct;
                                                    continue;
                                                }

                                                tempSql = new StringBuilder();
                                                tempSql.Append(" update Tb_Ticket_PayDetail set ");
                                                tempSql.Append(" PayMode=" + mOrder.PayWay + ", ");
                                                if (tempTicketPayDetail.PayType == "分账")
                                                    tempSql.Append(" PayMoney=0, ");
                                                tempSql.Append(" PayAccount='" + tempTicketPayDetail.PayAccount + "', ");
                                                tempSql.Append(" BuyPoundage='" + tempTicketPayDetail.BuyPoundage + "', ");
                                                tempSql.Append(" RealPayMoney='" + tempTicketPayDetail.RealPayMoney + "' ");
                                                tempSql.Append(" where id='" + tempTicketPayDetail.id.ToString() + "' ");

                                                sqlUpdateList.Add(tempSql.ToString());

                                                #endregion
                                            }
                                            else
                                            {
                                                #region 普通支付 手续费

                                                if (tempTicketPayDetail.PayType == "付款")
                                                {
                                                    tempTicketPayDetail.PayAccount = ""; //付款账号
                                                    tempTicketPayDetail.BuyPoundage = d.FourToFiveNum(tempTicketPayDetail.PayMoney * Rate, 2); //手续费
                                                    orderRealPayMoney = tempTicketPayDetail.PayMoney - tempTicketPayDetail.BuyPoundage;//实际交易金额（实收实付）
                                                }
                                                else if (tempTicketPayDetail.PayType == "收款")
                                                {
                                                    tempTicketPayDetail.BuyPoundage = d.FourToFiveNum(tempTicketPayDetail.PayMoney * Rate, 2); //手续费
                                                }
                                                else if (tempTicketPayDetail.PayType == "分账")
                                                {
                                                    //分账
                                                    for (int j = 0; j < jCount; j++)
                                                    {
                                                        // 获取收款账号
                                                        if (tempTicketPayDetail.CpyNo == bParametersList[j].CpyNo && tempTicketPayDetail.PayMoney > 0)
                                                        {
                                                            tempTicketPayDetail.BuyPoundage = d.FourToFiveNum(tempTicketPayDetail.PayMoney * Rate, 1); //手续费

                                                            if (tempTicketPayDetail.BuyPoundage <= 0.1M)
                                                                tempTicketPayDetail.BuyPoundage = 0.1M;  //不足1毛按 1 毛计算
                                                        }
                                                    }
                                                }
                                                else if (tempTicketPayDetail.PayType == "手续费")
                                                {
                                                    ptTicketPayDetail = tempTicketPayDetail;
                                                    ptTicketPayDetail.PayAccount = PTAct;//收款账号
                                                    continue; //先不计算平台手续费
                                                }

                                                tempTicketPayDetail.RealPayMoney = tempTicketPayDetail.PayMoney - tempTicketPayDetail.BuyPoundage;//实际交易金额（实收实付）

                                                if (tempTicketPayDetail.RealPayMoney < 0)
                                                    tempTicketPayDetail.RealPayMoney = 0;

                                                if (tempTicketPayDetail.PayType == "收款" || tempTicketPayDetail.PayType == "分账")
                                                {
                                                    AllPayMoney += tempTicketPayDetail.PayMoney;
                                                    AllRealPayMoney += tempTicketPayDetail.RealPayMoney;
                                                }

                                                tempSql = new StringBuilder();
                                                tempSql.Append(" update Tb_Ticket_PayDetail set ");
                                                tempSql.Append(" PayMode=" + mOrder.PayWay + ", ");
                                                tempSql.Append(" PayAccount='" + tempTicketPayDetail.PayAccount + "', ");
                                                tempSql.Append(" BuyPoundage='" + tempTicketPayDetail.BuyPoundage + "', ");
                                                tempSql.Append(" RealPayMoney='" + tempTicketPayDetail.RealPayMoney + "' ");
                                                tempSql.Append(" where id='" + tempTicketPayDetail.id.ToString() + "' ");

                                                sqlUpdateList.Add(tempSql.ToString());

                                                #endregion
                                            }
                                        }

                                        #region  手续费

                                        if (ptTicketPayDetail != null)
                                        {
                                            if (mOrder.OrderStatusCode == 9)
                                            {
                                                //改签
                                                ptTicketPayDetail.PayMoney = 0;
                                                ptTicketPayDetail.BuyPoundage = d.FourToFiveNum(mOrder.PayMoney * 0.001M, 2); //手续费
                                                ptTicketPayDetail.RealPayMoney = mOrder.PayMoney - tempTicketPayDetail.BuyPoundage - AllRealPayMoney;//实际交易金额（实收实付）
                                            }
                                            else
                                            {
                                                //在线支付手续费
                                                ptTicketPayDetail.PayMoney = mOrder.PayMoney - AllPayMoney;
                                                ptTicketPayDetail.BuyPoundage = d.FourToFiveNum(mOrder.PayMoney * 0.001M, 2);
                                                ptTicketPayDetail.RealPayMoney = mOrder.PayMoney - AllRealPayMoney - ptTicketPayDetail.BuyPoundage;
                                            }

                                            tempSql = new StringBuilder();
                                            tempSql.Append(" update Tb_Ticket_PayDetail set ");
                                            tempSql.Append(" PayMode=" + mOrder.PayWay + ", ");
                                            tempSql.Append(" PayAccount='" + ptTicketPayDetail.PayAccount.ToString() + "', ");
                                            tempSql.Append(" PayMoney='" + ptTicketPayDetail.PayMoney + "', ");
                                            tempSql.Append(" BuyPoundage='" + ptTicketPayDetail.BuyPoundage + "', ");
                                            tempSql.Append(" RealPayMoney='" + ptTicketPayDetail.RealPayMoney + "' ");
                                            tempSql.Append(" where id='" + ptTicketPayDetail.id.ToString() + "' ");

                                            sqlUpdateList.Add(tempSql.ToString());

                                        }
                                        #endregion
                                    }

                                    #endregion
                                }
                            }
                            #endregion
                        }

                        // 修改订单账单明细数据
                        if (sqlUpdateList.Count > 0)
                        {
                            #region 修改订单支付方式

                            decimal HandlingMoney = d.FourToFiveNum(mOrder.PayMoney * Rate, 2); //手续费

                            tempSql = new StringBuilder();
                            tempSql.Append(" update Tb_Ticket_Order set ");
                            tempSql.Append(" HandlingRate='" + Rate + "', ");//手续费费率
                            tempSql.Append(" HandlingMoney='" + HandlingMoney.ToString("f2") + "', ");//总交易手续费
                            tempSql.Append(" PayWay=" + mOrder.PayWay + " ");//支付方式
                            tempSql.Append(" where id='" + mOrder.id.ToString() + "' ");

                            sqlUpdateList.Add(tempSql.ToString());

                            #endregion

                            result = new Dal.ControlBase.BaseData<Tb_Order_PayDetail>().ExecuteSqlTran(sqlUpdateList);
                        }
                    }
                }
                catch (Exception ex)
                {

                }

                return result;
            }
        }

        /// <summary>
        /// 组合分账明细 在线支付
        /// </summary>
        /// <param name="payWay">支付方式</param>
        /// <param name="OrderId">订单号</param>
        /// <returns></returns>
        public string PayDetails(string payWay, string OrderId)
        {
            string msg = "";

            try
            {
                #region 获取支付分账信息

                string tempSqlWhere = " OrderId='" + OrderId + "' and PayType<>'付款'";
                List<Tb_Order_PayDetail> payDetailList = new PbProject.Logic.Order.Tb_Order_PayDetailBLL().GetListBySqlWhere(tempSqlWhere);

                payDetailList = OnLinePayDetails(payDetailList);

                #endregion

                decimal tempRealPayMoney = 0;

                foreach (Tb_Order_PayDetail item in payDetailList)
                {
                    if (item.RealPayMoney == 0)
                        continue;
                    if (payWay == "1" || payWay == "5")//支付宝
                    {
                        // act + "^" + actPrice.ToString("F2") + "^充值收款";
                        //if (item.PayType == "手续费")
                        //    tempRealPayMoney = item.BuyPoundage;
                        //else
                        tempRealPayMoney = item.RealPayMoney;
                        msg += item.PayAccount + "^" + tempRealPayMoney.ToString("F2") + "^" + item.PayType + "|";
                    }
                    else if (payWay == "2" || payWay == "6")//快钱
                    {
                        //"1^" + act + "^" + actPriceInt.ToString() + "^0^充值收款";
                        //if (item.PayType == "手续费")
                        //    tempRealPayMoney = item.PayMoney * 100;
                        //else 
                        tempRealPayMoney = item.RealPayMoney * 100;
                        msg += "1^" + item.PayAccount + "^" + tempRealPayMoney.ToString("F0") + "^0^" + item.PayType + "|";
                    }
                    else if (payWay == "3" || payWay == "7")//汇付天下
                    {
                        //"Agent:" + act + ":" + actPrice;
                        msg += "Agent:" + item.PayAccount + ":" + item.RealPayMoney + ";";
                    }
                    else if (payWay == "4" || payWay == "8" || payWay=="40")// 财付通
                    {
                        /**
                        * 业务参数
                        * 帐号^分帐金额^角色
                        * 角色说明:	1:供应商(收款) 2:平台服务方(手续费) 4:独立分润方（分账）
                        * 帐号1^分帐金额1^角色1|帐号2^分帐金额2^角色2|...
                        */
                        tempRealPayMoney = item.RealPayMoney * 100;
                        string roleid = string.Empty;

                        if (item.PayType.Equals("收款") && (item.CpyType == 2 || item.CpyType == 3))
                        {
                            roleid = "1";
                        }
                        else if (item.PayType.Equals("手续费"))
                        {
                            roleid = "2";
                        }
                        //else if (item.PayType.Equals("分账"))
                        else
                        {
                            roleid = "4";
                        }



                        msg += string.Format("{0}^{1}^{2}|", item.PayAccount, tempRealPayMoney.ToString("F0"), roleid);


                    }
                }

                msg = msg.TrimEnd('|').TrimEnd(';');
            }
            catch (Exception)
            {

            }
            return msg;
        }

        /// <summary>
        /// 过滤重复数据: 在线支付 通过收款账号合并
        /// </summary>
        /// <param name="OrderPayDetailList"></param>
        /// <returns></returns>
        public List<Tb_Order_PayDetail> OnLinePayDetails(List<Tb_Order_PayDetail> payDetailList)
        {
            List<Tb_Order_PayDetail> payDetailListNew = new List<Tb_Order_PayDetail>();
            try
            {
                Tb_Order_PayDetail temp = null;
                bool result = true;

                for (int i = 0; i < payDetailList.Count; i++)
                {
                    result = true;
                    temp = payDetailList[i];

                    if (temp.PayType == "付款" || temp.RealPayMoney == 0)
                        continue;

                    if (payDetailListNew == null || payDetailListNew.Count == 0)
                    {
                        payDetailListNew.Add(temp);
                        continue;
                    }

                    for (int j = 0; j < payDetailListNew.Count; j++)
                    {
                        //if (payDetailListNew[j].CpyNo == temp.CpyNo)
                        if (payDetailListNew[j].PayAccount == temp.PayAccount)
                        {
                            if (payDetailListNew[j].PayType != temp.PayType)
                                payDetailListNew[j].PayType = "收款";

                            //累计金额
                            payDetailListNew[j].BuyPoundage += temp.BuyPoundage;
                            payDetailListNew[j].PayMoney += temp.PayMoney;
                            payDetailListNew[j].RealPayMoney += temp.RealPayMoney;
                            result = false;
                            break;
                        }
                    }
                    if (result)
                        payDetailListNew.Add(temp);
                }
            }
            catch (Exception)
            {

            }

            return payDetailListNew;
        }

        /// <summary>
        /// 过滤重复数据: 账户余额支付 通过收款公司合并 
        /// </summary>
        /// <param name="OrderPayDetailList"></param>
        /// <returns>返回:分账信息</returns>
        public List<Tb_Order_PayDetail> VirtualPayDetails(List<Tb_Order_PayDetail> payDetailList)
        {
            List<Tb_Order_PayDetail> detailList = new List<Tb_Order_PayDetail>();
            try
            {
                //string tempSqlWhere = " OrderId='01316184022500198'";
                //List<Tb_Order_PayDetail> payDetailList = new PbProject.Logic.Order.Tb_Order_PayDetailBLL().GetListBySqlWhere(tempSqlWhere);

                Tb_Order_PayDetail temp = null;
                List<Tb_Order_PayDetail> payDetailListNew = new List<Tb_Order_PayDetail>();
                bool result = true;

                for (int i = 0; i < payDetailList.Count; i++)
                {
                    result = true;
                    temp = payDetailList[i];

                    if (payDetailListNew == null || payDetailListNew.Count == 0)
                    {
                        payDetailListNew.Add(temp);
                        continue;
                    }

                    for (int j = 0; j < payDetailListNew.Count; j++)
                    {
                        if (payDetailListNew[j].CpyNo == temp.CpyNo)
                        // if (payDetailListNew[j].PayAccount == temp.PayAccount)
                        {
                            if (payDetailListNew[j].PayType != temp.PayType)
                                payDetailListNew[j].PayType = "收款";

                            //累计金额
                            payDetailListNew[j].BuyPoundage += temp.BuyPoundage;
                            payDetailListNew[j].PayMoney += temp.PayMoney;
                            payDetailListNew[j].RealPayMoney += temp.RealPayMoney;
                            result = false;
                            break;
                        }
                    }

                    if (result)
                        payDetailListNew.Add(temp);
                }

                #region 筛选分账

                if (payDetailListNew.Count > 0)
                {
                    for (int k = 0; k < payDetailListNew.Count; k++)
                    {
                        temp = payDetailListNew[k];
                        if (temp.PayType == "分账")
                            detailList.Add(temp);
                    }
                }

                #endregion
            }
            catch (Exception)
            {

            }

            return detailList;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ticketPayDetail"></param>
        /// <returns></returns>
        public List<string> GetTicketIdByTicketPayDetail(List<Tb_Ticket_PayDetail> ticketPayDetail)
        {
            List<string> list = new List<string>();

            try
            {
                if (ticketPayDetail != null && ticketPayDetail.Count > 0)
                {
                    foreach (Tb_Ticket_PayDetail item in ticketPayDetail)
                    {
                        if (list == null || list.Count == 0)
                        {
                            list.Add(item.TicketId);
                        }
                        else
                        {
                            if (!list.Contains(item.TicketId))
                            {
                                list.Add(item.TicketId);
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {


            }
            return list;
        }

        /// <summary>
        /// 获取收支账号
        /// </summary>
        /// <param name="payNo"></param>
        /// <param name="payDetail"></param>
        /// <returns></returns>
        public string GetPayAccount(string payNo, List<Tb_Order_PayDetail> payDetail)
        {
            string act = "";
            try
            {
                if (!string.IsNullOrEmpty(payNo) && payDetail != null && payDetail.Count > 0)
                {
                    foreach (Tb_Order_PayDetail item in payDetail)
                    {
                        if (item.CpyNo == payNo)
                        {
                            act = item.PayAccount;
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }

            return act;
        }

        #endregion

        #region 在线充值、POS机充值

        /// <summary>
        /// 充值成功,生成充值账单
        /// </summary>
        /// <param name="orderId">订单编号</param>
        /// <param name="payNo">交易号</param>
        /// <param name="price">交易金额</param>
        /// <param name="payWay"> //payWay支付方式:1支付宝/2快钱/3汇付/4/财付通/5支付宝网银/6快钱网银/7汇付网银/8财付通网银/
        /// 9支付宝pos/10快钱pos/11汇付pos/12财付通/13易宝pos/14账户支付</param>
        /// <param name="useId">付款人id 或者 POS机编号</param>
        /// <param name="operReason">原因或操作描述</param>
        /// <param name="remark">备注</param>
        public bool CreateLogMoneyDetail(string orderId, string payNo, string price, int payWay, string useId, string operReason, string remark)
        {
            lock (lockobject)
            {
                bool result = false;

                try
                {
                    #region 判断是否已经存在的交易，不能重复交易

                    List<Log_MoneyDetail> moneyDetailList = baseDataManage.CallMethod("Log_MoneyDetail", "GetList", null, new Object[] { " PayNo='" + payNo + "'" }) as List<Log_MoneyDetail>;

                    #endregion

                    if (moneyDetailList == null || moneyDetailList.Count == 0)
                    {
                        Tb_Ticket_OrderBLL ticket_Order = new Tb_Ticket_OrderBLL();
                        string indexId = ticket_Order.GetIndexId();//内部流水号
                        orderId = string.IsNullOrEmpty(orderId) ? ticket_Order.GetOrderId("1") : orderId;//订单编号
                        User_CompanyBLL ucBll = new User_CompanyBLL();

                        string payCpyNo = ""; //付款公司编号
                        string tempWhere = "";
                        User_Company payCompany = null; //付款公司
                        User_Employees payUserEmployees = null;//付款账号
                        User_Company RecCompany = null;//收款公司

                        DataAction data = new DataAction();
                        decimal tempPrice = decimal.Parse(price);

                        //decimal Rate = 0.001M;  //在线充值默认手续费费率
                        decimal Rate = 0M;  //在线充值默认手续费费率

                        List<string> sqlList = new List<string>();

                        #region 处理POS机

                        if (payWay == 9 || payWay == 10 || payWay == 11 || payWay == 12 || payWay == 13)
                        {
                            //通过POS机编号查询：公司编号
                            tempWhere = " PosNo='" + useId + "'";

                            List<Tb_PosInfo> posInfoList = baseDataManage.CallMethod("Tb_PosInfo", "GetList", null, new Object[] { tempWhere }) as List<Tb_PosInfo>;
                            Tb_PosInfo posInfo = (posInfoList != null && posInfoList.Count > 0) ? posInfoList[0] : null;

                            if (posInfo != null)
                            {
                                payCpyNo = posInfo.CpyNo;
                                //费率处理
                                Rate = decimal.Parse(posInfo.PosRate.ToString());
                            }
                        }

                        #endregion

                        #region 付款账号信息

                        if (!string.IsNullOrEmpty(payCpyNo))
                        {
                            // pos 机
                            tempWhere = " CpyNo='" + payCpyNo + "' and IsAdmin=0";
                        }
                        else
                        {
                            tempWhere = " id='" + useId + "'";
                        }

                        List<User_Employees> payUserEmployeesList = new User_EmployeesBLL().GetBySQLList(tempWhere);
                        payUserEmployees = (payUserEmployeesList != null && payUserEmployeesList.Count > 0) ? payUserEmployeesList[0] : null;

                        if (string.IsNullOrEmpty(payCpyNo))
                            payCpyNo = payUserEmployees != null ? payUserEmployees.CpyNo : "";

                        #endregion

                        #region 手续费处理
                        decimal sxfPrice = data.FourToFiveNum((tempPrice * Rate), 2);
                        price = data.FourToFiveNum((tempPrice - sxfPrice), 2).ToString("F2");
                        #endregion

                        #region 付款公司信息
                        tempWhere = " UninCode='" + payCpyNo + "' ";
                        List<User_Company> payCompanyList = ucBll.GetListBySqlWhere(tempWhere);
                        payCompany = (payCompanyList != null && payCompanyList.Count > 0) ? payCompanyList[0] : null;

                        #endregion

                        #region 收款公司信息

                        string gYcpyNo = payCpyNo.Substring(0, 12);
                        tempWhere = " UninCode='" + gYcpyNo + "' ";
                        List<User_Company> RecCompanyList = ucBll.GetListBySqlWhere(tempWhere);
                        RecCompany = (RecCompanyList != null && RecCompanyList.Count > 0) ? RecCompanyList[0] : null;

                        #endregion

                        if (payCompany != null && payUserEmployees != null && RecCompany != null)
                        {
                            #region 1.修改充值前金额

                            decimal oldAccountMoney = payCompany.AccountMoney; // 充值前金额
                            decimal newAccountMoney = oldAccountMoney + decimal.Parse(price);// 充值成功后的金额
                            string sqlUpdateUserCompany = "update User_Company set AccountMoney=" + newAccountMoney + " where UninCode='" + payCpyNo + "'";

                            sqlList.Add(sqlUpdateUserCompany);//1.修改充值前金额

                            #endregion

                            #region 2.添加交易日志

                            Log_MoneyDetail logMoneyDetail = new Log_MoneyDetail();
                            logMoneyDetail.id = Guid.NewGuid();
                            logMoneyDetail.InPayNo = indexId;//内部流水号
                            logMoneyDetail.OrderId = orderId;//订单编号
                            logMoneyDetail.PayNo = payNo;//支付流水号
                            logMoneyDetail.OperLoginName = payUserEmployees.LoginName;
                            logMoneyDetail.OperReason = operReason;
                            logMoneyDetail.OperTime = DateTime.Now;
                            logMoneyDetail.OperUserName = payUserEmployees.UserName;
                            logMoneyDetail.PayCpyName = payCompany.UninAllName;
                            logMoneyDetail.PayCpyNo = payCompany.UninCode;
                            logMoneyDetail.PayCpyType = payCompany.RoleType;
                            logMoneyDetail.PayMoney = decimal.Parse(price);
                            logMoneyDetail.PayType = payWay;
                            logMoneyDetail.PreRemainMoney = oldAccountMoney;
                            logMoneyDetail.RecCpyName = RecCompany.UninAllName;
                            logMoneyDetail.RecCpyNo = RecCompany.UninCode;
                            logMoneyDetail.RecCpyType = RecCompany.RoleType;
                            logMoneyDetail.RemainMoney = newAccountMoney;
                            logMoneyDetail.Remark = remark;
                            logMoneyDetail.A1 = 0;
                            logMoneyDetail.A2 = 3;

                            string strSql = Dal.Mapping.MappingHelper<Log_MoneyDetail>.CreateInsertModelSql(logMoneyDetail);
                            sqlList.Add(strSql); //2.添加交易日志


                            #endregion

                            #region 3.自动处理销账

                            List<string> sqlListTemp = new Logic.Order.Tb_Ticket_OrderBLL().AutomaticOperXZ(payCpyNo, decimal.Parse(price));

                            if (sqlListTemp != null && sqlListTemp.Count > 0)
                            {
                                foreach (string sqlTemp in sqlListTemp)
                                {
                                    sqlList.Add(sqlTemp);
                                }
                            }

                            #endregion

                            #region 添加到数据


                            //添加到数据
                            result = new Dal.ControlBase.BaseData<Log_MoneyDetail>().ExecuteSqlTran(sqlList);

                            #endregion
                        }
                    }
                    else
                    {
                        //数据库已经存在了，不处理
                    }
                }
                catch (Exception ex)
                {

                }

                return result;
            }
        }


        /// <summary>
        /// 撤销充值成功的单子 （POS 机撤销充值成功通知）
        /// </summary>
        /// <param name="orderId">订单编号</param>
        /// <param name="payNo">交易号</param>
        /// <param name="price">交易金额</param>
        /// <param name="payWay"> //payWay支付方式:1支付宝/2快钱/3汇付/4/财付通/5支付宝网银/6快钱网银/7汇付网银/8财付通网银/
        /// 9支付宝pos/10快钱pos/11汇付pos/12财付通/13易宝pos/14账户支付</param>
        /// <param name="useId">付款人id 或者 POS机编号</param>
        /// <param name="operReason">原因或操作描述</param>
        /// <param name="remark">备注</param>
        public bool CreateCancelLogMoneyDetail(string orderId, string payNo, string price, int payWay, string useId, string operReason, string remark)
        {
            lock (lockobject)
            {
                bool result = false;

                try
                {
                    #region 判断是否已经存在的交易，不能重复交易

                    List<Log_MoneyDetail> moneyDetailList = baseDataManage.CallMethod("Log_MoneyDetail", "GetList", null, new Object[] { " PayNo='" + payNo + "' and A1=1 " }) as List<Log_MoneyDetail>;

                    #endregion

                    if (moneyDetailList == null || moneyDetailList.Count == 0)
                    {
                        Tb_Ticket_OrderBLL ticket_Order = new Tb_Ticket_OrderBLL();
                        string indexId = ticket_Order.GetIndexId();//内部流水号
                        orderId = string.IsNullOrEmpty(orderId) ? ticket_Order.GetOrderId("1") : orderId;//订单编号
                        User_CompanyBLL ucBll = new User_CompanyBLL();

                        string payCpyNo = ""; //付款公司编号
                        string tempWhere = "";
                        User_Company payCompany = null; //付款公司
                        User_Employees payUserEmployees = null;//付款账号
                        User_Company RecCompany = null;//收款公司

                        DataAction data = new DataAction();
                        decimal tempPrice = decimal.Parse(price);
                        decimal Rate = 0.001M;//在线充值默认手续费费率

                        List<string> sqlList = new List<string>();

                        #region 处理POS机

                        switch (payWay)
                        {
                            case 9:
                            case 10:
                            case 11:
                            case 12:
                            case 13:
                                //通过POS机编号查询：公司编号
                                tempWhere = " PosNo='" + useId + "'";

                                List<Tb_PosInfo> posInfoList = baseDataManage.CallMethod("Tb_PosInfo", "GetList", null, new Object[] { tempWhere }) as List<Tb_PosInfo>;
                                Tb_PosInfo posInfo = (posInfoList != null && posInfoList.Count > 0) ? posInfoList[0] : null;

                                if (posInfo != null)
                                {
                                    payCpyNo = posInfo.CpyNo;
                                    //费率处理
                                    Rate = decimal.Parse(posInfo.PosRate.ToString());
                                }
                                break;

                            default:
                                break;
                        }

                        #endregion

                        #region 手续费处理
                        decimal sxfPrice = data.FourToFiveNum((tempPrice * Rate), 2);
                        price = data.FourToFiveNum((tempPrice - sxfPrice), 2).ToString("F2");
                        #endregion

                        #region 付款账号信息

                        if (!string.IsNullOrEmpty(payCpyNo))
                        {
                            // pos 机
                            tempWhere = " CpyNo='" + payCpyNo + "' and IsAdmin=0";
                        }
                        else
                        {
                            tempWhere = " id='" + useId + "'";
                        }

                        price = data.FourToFiveNum((tempPrice - Rate), 2).ToString("F2");

                        List<User_Employees> payUserEmployeesList = new User_EmployeesBLL().GetBySQLList(tempWhere);
                        payUserEmployees = (payUserEmployeesList != null && payUserEmployeesList.Count > 0) ? payUserEmployeesList[0] : null;

                        if (string.IsNullOrEmpty(payCpyNo))
                            payCpyNo = payUserEmployees != null ? payUserEmployees.CpyNo : "";

                        #endregion

                        #region 付款公司信息

                        tempWhere = " UninCode='" + payCpyNo + "' ";
                        List<User_Company> payCompanyList = ucBll.GetListBySqlWhere(tempWhere);
                        payCompany = (payCompanyList != null && payCompanyList.Count > 0) ? payCompanyList[0] : null;

                        #endregion

                        #region 收款公司信息

                        string gYcpyNo = payCpyNo.Substring(0, 12);
                        tempWhere = " UninCode='" + gYcpyNo + "' ";
                        List<User_Company> RecCompanyList = ucBll.GetListBySqlWhere(tempWhere);
                        RecCompany = (RecCompanyList != null && RecCompanyList.Count > 0) ? RecCompanyList[0] : null;

                        #endregion

                        if (payCompany != null && payUserEmployees != null && RecCompany != null)
                        {
                            #region 1.修改撤销金额

                            decimal oldAccountMoney = payCompany.AccountMoney; // 充值前金额
                            decimal newAccountMoney = oldAccountMoney - decimal.Parse(price);// 充值成功后的金额
                            string sqlUpdateUserCompany = "update User_Company set AccountMoney=" + newAccountMoney + " where UninCode='" + payCpyNo + "'";

                            sqlList.Add(sqlUpdateUserCompany);//1.修改充值前金额

                            #endregion

                            #region 2.添加交易日志

                            Log_MoneyDetail logMoneyDetail = new Log_MoneyDetail();
                            logMoneyDetail.id = Guid.NewGuid();
                            logMoneyDetail.InPayNo = indexId;//内部流水号
                            logMoneyDetail.OrderId = orderId;//订单编号
                            logMoneyDetail.PayNo = payNo;//支付流水号
                            logMoneyDetail.OperLoginName = payUserEmployees.LoginName;
                            logMoneyDetail.OperReason = operReason;
                            logMoneyDetail.OperTime = DateTime.Now;
                            logMoneyDetail.OperUserName = payUserEmployees.UserName;
                            logMoneyDetail.PayCpyName = payCompany.UninAllName;
                            logMoneyDetail.PayCpyNo = payCompany.UninCode;
                            logMoneyDetail.PayCpyType = payCompany.RoleType;
                            logMoneyDetail.PayMoney = decimal.Parse(price);
                            logMoneyDetail.PayType = payWay;
                            logMoneyDetail.PreRemainMoney = oldAccountMoney;
                            logMoneyDetail.RecCpyName = RecCompany.UninAllName;
                            logMoneyDetail.RecCpyNo = RecCompany.UninCode;
                            logMoneyDetail.RecCpyType = RecCompany.RoleType;
                            logMoneyDetail.RemainMoney = newAccountMoney;
                            logMoneyDetail.Remark = remark;
                            logMoneyDetail.A1 = 1;
                            logMoneyDetail.A2 = 9;

                            string strSql = Dal.Mapping.MappingHelper<Log_MoneyDetail>.CreateInsertModelSql(logMoneyDetail);
                            sqlList.Add(strSql); //2.添加交易日志


                            #endregion

                            #region 添加到数据

                            //添加到数据
                            result = new Dal.ControlBase.BaseData<Log_MoneyDetail>().ExecuteSqlTran(sqlList);

                            #endregion
                        }
                    }
                    else
                    {
                        //数据库已经存在了，不处理
                    }
                }
                catch (Exception ex)
                {

                }

                return result;
            }
        }

        #endregion

        #region V2.94 退款同步调用3.0
        /// <summary>
        /// 进行同步数据
        /// </summary>
        /// <param name="?"></param>
        /// <returns></returns>
        public bool TongBuData(AcceptParam Param)
        {
            string strOutPut = "";
            bool result = false;
            BaseDataManage Manage = new BaseDataManage();
            try
            {
                string sqlWhere = string.Format(" LoginName='{0}'  ", Param.m_LoginName);
                List<User_Employees> empList = Manage.CallMethod("User_Employees", "GetList", null, new object[] { sqlWhere }) as List<User_Employees>;
                if (empList != null && empList.Count > 0)
                {
                    User_Employees m_UserEmployees = empList[0];
                    User_Company m_UserCompany = null;
                    sqlWhere = string.Format(" UninAllName='{0}' and UninCode='{1}'", Param.m_CompanyName, m_UserEmployees.CpyNo);
                    List<User_Company> comList = Manage.CallMethod("User_Company", "GetList", null, new object[] { sqlWhere }) as List<User_Company>;
                    if (comList != null && comList.Count > 0)
                    {
                        m_UserCompany = comList[0];
                    }
                    sqlWhere = string.Format(" OrderId='{0}' ", Param.m_OrderId);
                    bool IsExist = (bool)Manage.CallMethod("Log_MoneyDetail", "IsExist", null, new object[] { sqlWhere });
                    if (IsExist)
                    {
                        Param.sbLog.AppendFormat("\r\n找到账号:{0},和公司编号：{1},订单号{2}已存在!\r\n", Param.m_LoginName, m_UserCompany.UninCode, Param.m_OrderId);
                    }
                    else
                    {
                        //当前登录的公司和用户信息
                        if (m_UserCompany != null && m_UserEmployees != null)
                        {
                            Param.sbLog.AppendFormat("\r\n找到账号:{0},和公司编号：{1}", Param.m_LoginName, m_UserCompany.UninCode);
                            Tb_Ticket_OrderBLL ticket_Order = new Tb_Ticket_OrderBLL();
                            string indexId = ticket_Order.GetIndexId();//内部流水号
                            string payNo = Param.m_OrderId;
                            string orderId = Param.m_OrderId;//订单编号

                            string payCpyNo = m_UserCompany.UninCode; //付款公司编号                 
                            User_Company payCompany = null; //付款公司
                            User_Employees payUserEmployees = null;//付款账号
                            User_Company RecCompany = null;//收款公司
                            // Data data = new Data();
                            User_CompanyBLL ucBll = new User_CompanyBLL();
                            #region 付款账号信息
                            if (!string.IsNullOrEmpty(payCpyNo))
                            {
                                sqlWhere = " CpyNo='" + payCpyNo + "' and IsAdmin=0";
                            }
                            else
                            {
                                sqlWhere = " id='" + m_UserEmployees.id + "'";
                            }
                            //价格
                            string price = Param.m_Price.ToString();

                            List<User_Employees> payUserEmployeesList = new User_EmployeesBLL().GetBySQLList(sqlWhere);
                            payUserEmployees = (payUserEmployeesList != null && payUserEmployeesList.Count > 0) ? payUserEmployeesList[0] : null;
                            if (string.IsNullOrEmpty(payCpyNo))
                                payCpyNo = payUserEmployees != null ? payUserEmployees.CpyNo : "";

                            #endregion
                            //日志
                            Param.sbLog.AppendFormat("\r\n付款账号信息：payCpyNo={0}", payCpyNo);


                            #region 付款公司信息
                            sqlWhere = " UninCode='" + payCpyNo + "' ";
                            List<User_Company> payCompanyList = ucBll.GetListBySqlWhere(sqlWhere);
                            payCompany = (payCompanyList != null && payCompanyList.Count > 0) ? payCompanyList[0] : null;

                            #endregion

                            #region 收款公司信息
                            sqlWhere = " UninCode=left('" + payCpyNo + "',12) ";
                            List<User_Company> RecCompanyList = ucBll.GetListBySqlWhere(sqlWhere);
                            RecCompany = (RecCompanyList != null && RecCompanyList.Count > 0) ? RecCompanyList[0] : null;

                            #endregion

                            if (payCompany != null && payUserEmployees != null && RecCompany != null)
                            {
                                //sql列表
                                List<string> sqlList = new List<string>();
                                #region 1.修改充值前金额

                                decimal oldAccountMoney = payCompany.AccountMoney; // 充值前金额
                                decimal newAccountMoney = oldAccountMoney + decimal.Parse(price);// 充值成功后的金额
                                string sqlUpdateUserCompany = "update User_Company set AccountMoney=" + newAccountMoney + " where UninCode='" + payCpyNo + "'";

                                sqlList.Add(sqlUpdateUserCompany);//1.修改充值前金额

                                #endregion

                                #region 2.添加交易日志

                                Log_MoneyDetail logMoneyDetail = new Log_MoneyDetail();
                                logMoneyDetail.id = Guid.NewGuid();
                                logMoneyDetail.InPayNo = indexId;//内部流水号
                                logMoneyDetail.OrderId = orderId;//订单编号
                                logMoneyDetail.PayNo = payNo;//支付流水号
                                logMoneyDetail.OperLoginName = payUserEmployees.LoginName;
                                logMoneyDetail.OperReason = "历史订单退款";
                                logMoneyDetail.OperTime = DateTime.Now;
                                logMoneyDetail.OperUserName = payUserEmployees.UserName;
                                logMoneyDetail.PayCpyName = payCompany.UninAllName;
                                logMoneyDetail.PayCpyNo = payCompany.UninCode;
                                logMoneyDetail.PayCpyType = payCompany.RoleType;
                                logMoneyDetail.PayMoney = decimal.Parse(price);
                                logMoneyDetail.PayType = 14;
                                logMoneyDetail.PreRemainMoney = oldAccountMoney;
                                logMoneyDetail.RecCpyName = RecCompany.UninAllName;
                                logMoneyDetail.RecCpyNo = RecCompany.UninCode;
                                logMoneyDetail.RecCpyType = RecCompany.RoleType;
                                logMoneyDetail.RemainMoney = newAccountMoney;
                                logMoneyDetail.Remark = string.Format("同步账号:{0},订单号：{1},金额:{2},PNR:{3},乘机人:{4}", Param.m_LoginName, Param.m_OrderId, Param.m_Price, Param.m_Pnr, Param.m_PassengerList);
                                logMoneyDetail.A1 = 0;
                                logMoneyDetail.A2 = 2;

                                string strSql = PbProject.Dal.Mapping.MappingHelper<Log_MoneyDetail>.CreateInsertModelSql(logMoneyDetail);
                                sqlList.Add(strSql); //2.添加交易日志


                                #endregion

                                #region 3.自动处理销账

                                List<string> sqlListTemp = new PbProject.Logic.Order.Tb_Ticket_OrderBLL().AutomaticOperXZ(payCpyNo, decimal.Parse(price));
                                if (sqlListTemp != null && sqlListTemp.Count > 0)
                                {
                                    foreach (string sqlTemp in sqlListTemp)
                                    {
                                        sqlList.Add(sqlTemp);
                                    }
                                }

                                #endregion

                                #region 添加到数据
                                //日志
                                Param.sbLog.Append("\r\n进入执行sql语句:" + string.Join("\r\n", sqlList.ToArray()) + "\r\n");
                                //添加到数据
                                result = new PbProject.Dal.ControlBase.BaseData<Log_MoneyDetail>().ExecuteSqlTran(sqlList);
                                #endregion
                            }
                            else
                            {
                                //日志
                                Param.sbLog.Append("\r\npayCompany  payUserEmployees  RecCompany 为空！\r\n");
                            }
                        }
                    }
                }
                else
                {
                    Param.sbLog.AppendFormat("\r\n没有找到这个账号:{0}", Param.m_LoginName);
                }
            }
            catch (Exception ex)
            {
                Param.sbLog.Append(ex.ToString());
                PbProject.WebCommon.Log.Log.RecordLog("AddPayMoney.aspx", Param.sbLog.ToString(), false, HttpContext.Current.Request);
            }
            finally
            {
                Param.sbLog.Append("结束==========");
                PbProject.WebCommon.Log.Log.RecordLog("AddPayMoney.aspx", Param.sbLog.ToString(), false, HttpContext.Current.Request);
            }
            return result;
        }



        #endregion

        #region 账户余额支付:虚拟支付账单处理

        /// <summary>
        /// 账户余额支付:生成虚拟支付账单
        /// </summary>
        /// <param name="mOrder">订单mOrder</param>
        /// <param name="mUser">当前操作员model</param>
        /// <param name="msg">错误消息</param> 
        public bool CreateVirtualPayBill(Tb_Ticket_Order mOrder, User_Employees payUserEmployees, out string msg)
        {
            lock (lockobject)
            {
                msg = "";
                bool result = false;

                try
                {
                    List<string> sqlList = new List<string>();
                    string tempSql = ""; //临时变量 Sql
                    string tempSqlWhere = "";//临时变量 tempSqlWhere

                    string tempCpyNo = "";//临时变量公司编号

                    User_Company tempCompany = null; //分账公司
                    List<User_Company> uCompanys = null; //分账公司信息
                    SortedList<string, string> tempList = null;

                    PbProject.Logic.User.User_CompanyBLL uCompanyBLL = new PbProject.Logic.User.User_CompanyBLL();

                    #region 收款公司信息

                    tempCpyNo = (mOrder != null && mOrder.CPCpyNo != null) ? mOrder.CPCpyNo.Substring(0, 12) : "";

                    tempSqlWhere = " UninCode='" + tempCpyNo + "' ";
                    List<User_Company> RecCompanyList = uCompanyBLL.GetListBySqlWhere(tempSqlWhere);
                    User_Company RecCompany = (RecCompanyList != null && RecCompanyList.Count > 0) ? RecCompanyList[0] : null;

                    #endregion

                    #region 付款公司信息

                    tempCpyNo = (payUserEmployees != null && payUserEmployees.CpyNo != null) ? payUserEmployees.CpyNo : "";
                    tempSqlWhere = " UninCode='" + tempCpyNo + "' ";
                    List<User_Company> payCompanyList = uCompanyBLL.GetListBySqlWhere(tempSqlWhere);
                    User_Company payCompany = (payCompanyList != null && payCompanyList.Count > 0) ? payCompanyList[0] : null;

                    #endregion

                    #region 分账信息

                    tempSqlWhere = " OrderId='" + mOrder.OrderId + "' and RealPayMoney!=0 ";
                    List<Tb_Order_PayDetail> payDetailList = new PbProject.Logic.Order.Tb_Order_PayDetailBLL().GetListBySqlWhere(tempSqlWhere);

                    #endregion

                    if (mOrder != null && payCompany != null && payUserEmployees != null && RecCompany != null && payDetailList != null && payDetailList.Count > 0)
                    {
                        Log_MoneyDetail logMoneyDetail = null;
                        decimal tempMoney = 0;

                        #region 1.修改支付账号余额

                        decimal oldAccountMoney = payCompany.AccountMoney; // 支付前
                        decimal newAccountMoney = oldAccountMoney - mOrder.PayMoney;// 支付后

                        tempSql = "update User_Company set AccountMoney=AccountMoney-" + mOrder.PayMoney + " where id='" + payCompany.id + "'";
                        sqlList.Add(tempSql);//1.修改账号余额
                        #endregion

                        #region 欠款处理

                        if (oldAccountMoney <= 0) //支付前
                        {
                            tempMoney = mOrder.PayMoney; // 记录欠款
                        }
                        else
                        {
                            if (newAccountMoney < 0)
                            {
                                tempMoney = newAccountMoney * -1; //记录欠款
                            }
                            else
                            {
                                tempMoney = 0;
                            }
                        }

                        #endregion

                        decimal maxDebtMoney = payCompany.MaxDebtMoney; //最大欠款额度

                        if (newAccountMoney > 0 || newAccountMoney + maxDebtMoney >= 0)
                        {
                            #region 2.添加订单日志

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

                            string Content = "账户余额支付";

                            Log_Tb_AirOrder OrderLog = new Log_Tb_AirOrder();
                            OrderLog.id = Guid.NewGuid();
                            OrderLog.OrderId = mOrder.OrderId;
                            OrderLog.OperType = "支付";
                            OrderLog.OperTime = DateTime.Now;
                            OrderLog.OperLoginName = payUserEmployees.LoginName;
                            OrderLog.OperUserName = payUserEmployees.UserName;
                            OrderLog.CpyNo = payCompany.UninCode;
                            OrderLog.CpyType = payCompany.RoleType;
                            OrderLog.CpyName = payCompany.UninAllName;
                            OrderLog.OperContent = Content;
                            OrderLog.WatchType = 5;

                            #endregion

                            tempSql = Dal.Mapping.MappingHelper<Log_Tb_AirOrder>.CreateInsertModelSql(OrderLog);
                            sqlList.Add(tempSql);//2.添加订单日志

                            #region 3.添加支付日志

                            logMoneyDetail = new Log_MoneyDetail();
                            logMoneyDetail.id = Guid.NewGuid();
                            logMoneyDetail.InPayNo = mOrder.InPayNo;//内部流水号
                            logMoneyDetail.OrderId = mOrder.OrderId;//订单编号
                            logMoneyDetail.PayNo = mOrder.InPayNo;//支付流水号
                            logMoneyDetail.OperLoginName = payUserEmployees.LoginName;
                            logMoneyDetail.OperReason = "订单付款";
                            logMoneyDetail.OperTime = DateTime.Now;
                            logMoneyDetail.OperUserName = payUserEmployees.UserName;
                            logMoneyDetail.PayCpyName = payCompany.UninAllName;
                            logMoneyDetail.PayCpyNo = payCompany.UninCode;
                            logMoneyDetail.PayCpyType = payCompany.RoleType;
                            logMoneyDetail.PayMoney = mOrder.PayMoney;
                            logMoneyDetail.PayType = 14;
                            logMoneyDetail.PreRemainMoney = oldAccountMoney;
                            logMoneyDetail.RecCpyName = RecCompany.UninAllName;
                            logMoneyDetail.RecCpyNo = RecCompany.UninCode;
                            logMoneyDetail.RecCpyType = RecCompany.RoleType;
                            //logMoneyDetail.RemainMoney = newAccountMoney;
                            logMoneyDetail.Remark = "订单付款";
                            logMoneyDetail.A2 = 1;

                            #endregion

                            tempList = new SortedList<string, string>();
                            string sqlRemainMoney = "(select AccountMoney from User_Company where id='" + payCompany.id + "')";
                            tempList.Add("RemainMoney", sqlRemainMoney);
                            tempSql = Dal.Mapping.MappingHelper<Log_MoneyDetail>.CreateInsertModelSql(logMoneyDetail, tempList);
                            sqlList.Add(tempSql); //3.添加支付日志

                            #region 4.修改订单状态

                            StringBuilder updateOrder = new StringBuilder();
                            updateOrder.Append(" update Tb_Ticket_Order set ");
                            updateOrder.Append(" PayTime='" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "', ");//支付时间
                            updateOrder.Append(" PayStatus=1, "); //支付状态
                            updateOrder.Append(" PayWay=14, ");//支付方式

                            if (tempMoney > 0)
                            {
                                updateOrder.Append(" DebtsPayFlag=1, ");//（1=未还清，0=已还清）
                                updateOrder.Append(" PayDebtsMoney=" + tempMoney + ", "); //欠款还款金额

                                #region 记录 欠款明细

                                logMoneyDetail = new Log_MoneyDetail();
                                logMoneyDetail.id = Guid.NewGuid();
                                logMoneyDetail.InPayNo = mOrder.InPayNo;//内部流水号
                                logMoneyDetail.OrderId = mOrder.OrderId + "_1";//订单编号
                                logMoneyDetail.PayNo = mOrder.InPayNo;//支付流水号
                                logMoneyDetail.OperLoginName = payUserEmployees.LoginName;
                                logMoneyDetail.OperReason = "欠款明细记录";
                                logMoneyDetail.OperTime = DateTime.Now;
                                logMoneyDetail.OperUserName = payUserEmployees.UserName;
                                logMoneyDetail.PayCpyName = payCompany.UninAllName;
                                logMoneyDetail.PayCpyNo = payCompany.UninCode;
                                logMoneyDetail.PayCpyType = payCompany.RoleType;
                                logMoneyDetail.PayType = 20;
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

                                logMoneyDetail.A2 = 7;

                                tempSql = Dal.Mapping.MappingHelper<Log_MoneyDetail>.CreateInsertModelSql(logMoneyDetail);
                                sqlList.Add(tempSql); // 欠款明细

                                #endregion
                            }


                            if (mOrder.TicketStatus == 5)
                            {
                                updateOrder.Append(" OrderStatusCode=15 ");//订单状态,改签支付
                            }
                            else
                            {
                                updateOrder.Append(" OrderStatusCode=3 ");//订单状态 普通支付
                            }

                            updateOrder.Append(" where id='" + mOrder.id + "' ");

                            #endregion

                            tempSql = updateOrder.ToString();
                            sqlList.Add(tempSql); //4.修改订单状态



                            #region 5.多级分账处理

                            string tempFzCpyNoS = "";

                            foreach (Tb_Order_PayDetail item in payDetailList)
                            {
                                if (item.PayType == "分账" && item.RealPayMoney != 0 && item.CpyNo.Length > 12)
                                {
                                    tempFzCpyNoS += "'" + item.CpyNo + "',";
                                }
                            }

                            if (!string.IsNullOrEmpty(tempFzCpyNoS))
                            {
                                tempFzCpyNoS = tempFzCpyNoS.TrimEnd(',');
                                tempFzCpyNoS = " UninCode in(" + tempFzCpyNoS + ")";

                                uCompanys = uCompanyBLL.GetListBySqlWhere(tempFzCpyNoS);
                            }

                            string tempRemainMoney = "";

                            for (int i = 0; i < payDetailList.Count; i++)
                            {
                                if (payDetailList[i].PayType == "分账" && payDetailList[i].RealPayMoney != 0 && payDetailList[i].CpyNo.Length > 12)
                                {
                                    tempCompany = null;

                                    #region 分账公司信息
                                    for (int k = 0; k < uCompanys.Count; k++)
                                    {
                                        if (uCompanys[k].UninCode == payDetailList[i].CpyNo)
                                        {
                                            tempCompany = uCompanys[k];
                                            break;
                                        }
                                    }
                                    #endregion

                                    tempSql = "update User_Company set AccountMoney=AccountMoney+" + payDetailList[i].RealPayMoney + " where UninCode='" + payDetailList[i].CpyNo + "'";
                                    sqlList.Add(tempSql); //

                                    #region 添加支付日志

                                    logMoneyDetail = new Log_MoneyDetail();

                                    logMoneyDetail.id = Guid.NewGuid();
                                    logMoneyDetail.InPayNo = mOrder.InPayNo;//内部流水号
                                    logMoneyDetail.OrderId = mOrder.OrderId;//订单编号
                                    logMoneyDetail.PayNo = mOrder.InPayNo + i;//支付流水号
                                    logMoneyDetail.OperLoginName = payUserEmployees.LoginName;
                                    logMoneyDetail.OperReason = "分账收款";
                                    logMoneyDetail.OperTime = DateTime.Now;
                                    logMoneyDetail.OperUserName = payUserEmployees.UserName;
                                    logMoneyDetail.PayCpyName = RecCompany.UninAllName;
                                    logMoneyDetail.PayCpyNo = RecCompany.UninCode;
                                    logMoneyDetail.PayCpyType = RecCompany.RoleType;
                                    logMoneyDetail.PayMoney = payDetailList[i].RealPayMoney;//发生金额
                                    logMoneyDetail.PayType = 14;
                                    logMoneyDetail.PreRemainMoney = tempCompany.AccountMoney; //之前账户余额
                                    logMoneyDetail.RecCpyName = tempCompany.UninAllName;
                                    logMoneyDetail.RecCpyNo = tempCompany.UninCode;
                                    logMoneyDetail.RecCpyType = tempCompany.RoleType;
                                    //logMoneyDetail.RemainMoney = newAccountMoney;
                                    logMoneyDetail.Remark = "分账收款";
                                    logMoneyDetail.A2 = 6;
                                    #endregion

                                    tempList = new SortedList<string, string>();
                                    tempRemainMoney = "(select AccountMoney from User_Company where id='" + tempCompany.id + "')";
                                    tempList.Add("RemainMoney", tempRemainMoney);
                                    tempSql = Dal.Mapping.MappingHelper<Log_MoneyDetail>.CreateInsertModelSql(logMoneyDetail, tempList);
                                    sqlList.Add(tempSql); //添加支付日志
                                }
                            }

                            #endregion

                            //添加到数据
                            result = new Dal.ControlBase.BaseData<Tb_Order_PayDetail>().ExecuteSqlTran(sqlList);

                            if (result)
                                msg = "支付成功";
                        }
                        else
                        {
                            msg = "账户余额不足！";
                        }
                    }
                    else
                    {
                        msg = "信息错误！";
                    }
                }
                catch (Exception ex)
                {
                    msg = "支付错误！";
                }
                return result;
            }
        }

        #endregion

        #region 账户余额退款:虚拟退款账单处理

        /// <summary>
        /// 账户余额退款:生成虚拟退款账单
        /// </summary>
        /// <param name="mOrder">订单mOrder</param>
        /// <param name="mUser">当前操作员model</param>
        /// <param name="msg">错误消息</param> 
        public bool CreateVirtualRefundBill(Tb_Ticket_Order mOrder, User_Employees payUserEmployees, User_Company payUserCompany, out string msg)
        {
            lock (lockobject)
            {
                msg = "";
                bool result = false;

                try
                {
                    #region 参数

                    List<string> sqlList = new List<string>();
                    string tempSqlWhere = "";//临时变量 tempSqlWhere
                    string tempSql = ""; //临时变量 Sql
                    SortedList<string, string> tempList = null;
                    Log_MoneyDetail logMoneyDetail = null;
                    bool tempBool = false;

                    decimal tempAccountMoney = 0;
                    decimal tempMaxDebtMoney = 0;

                    #endregion

                    #region 订单状态处理

                    int tempOrderStatusCode = mOrder.OrderStatusCode;

                    string Content = "订单退款成功";

                    switch (tempOrderStatusCode)
                    {
                        case 20://取消出票，退款中
                            tempOrderStatusCode = 5; //	取消出票，已退款 
                            Content = "取消出票,退款成功";
                            break;
                        case 21://退票成功，退款中
                            tempOrderStatusCode = 16;//退票成功，交易结束
                            break;
                        case 22://废票成功，退款中
                            tempOrderStatusCode = 17;//废票成功，交易结束
                            break;
                        case 23://拒绝改签，退款中
                            tempOrderStatusCode = 24;//拒绝改签，已退款
                            break;
                        default:
                            tempOrderStatusCode = mOrder.OrderStatusCode;
                            break;
                    }
                    #endregion

                    #region 1.修改订单信息 ,修改订单状态

                    StringBuilder updateOrder = new StringBuilder();

                    updateOrder.Append(" update Tb_Ticket_Order set ");
                    updateOrder.Append(" PayTime='" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "', ");//
                    updateOrder.Append(" PayDebtsMoney=0, ");//
                    updateOrder.Append(" OrderStatusCode=" + tempOrderStatusCode);//订单状态
                    updateOrder.Append(" where id='" + mOrder.id + "' ");

                    tempSql = updateOrder.ToString();
                    sqlList.Add(tempSql);

                    //修改原订单欠款金额
                    if (mOrder.PayDebtsMoney > 0)
                    {
                        tempSql = "update Tb_Ticket_Order set PayDebtsMoney=dbo.GetPayDebtsMoney(PayDebtsMoney-" + mOrder.PayMoney + ") where OrderId='" + mOrder.OldOrderId + "'";
                        sqlList.Add(tempSql);
                    }

                    #endregion

                    #region 获取分账信息

                    tempSqlWhere = " OrderId='" + mOrder.OrderId + "'";
                    List<Tb_Order_PayDetail> payDetailList = new PbProject.Logic.Order.Tb_Order_PayDetailBLL().GetListBySqlWhere(tempSqlWhere);

                    if (payDetailList == null || payDetailList.Count == 0)
                    {
                        msg = "退款数据有误！";
                        return false;
                    }

                    #endregion

                    #region 获取所有收支账号

                    tempSqlWhere = "";

                    foreach (Tb_Order_PayDetail item in payDetailList)
                    {
                        if (item.RealPayMoney == 0)
                            continue;
                        else
                        {
                            tempSqlWhere += "'" + item.CpyNo + "',";
                        }
                    }

                    tempSqlWhere = tempSqlWhere.TrimEnd(',');

                    StringBuilder sbSql = new StringBuilder();

                    sbSql.Append(" select * from User_Company ");
                    sbSql.Append(" left join User_Employees ");
                    sbSql.Append(" on User_Company.UninCode = User_Employees.CpyNo ");
                    sbSql.Append(" where UninCode in (" + tempSqlWhere + ") ");
                    sbSql.Append(" and User_Employees.IsAdmin=0 ");

                    DataTable dsUser = new PbProject.Logic.SQLEXBLL.SQLEXBLL_Base().ExecuteStrSQL(sbSql.ToString());

                    #endregion

                    #region 2.修改用户金额、 3.添加 Log_MoneyDetail 日志


                    string operLoginName = "";//操作人登录名
                    string operUserName = "";//操作人名称
                    string payCpyName = "";//付款方公司名称
                    string payCpyNo = "";//付款方公司编号
                    int payCpyType = 0;//付款方公司类型
                    string recCpyName = "";//收款方公司名称
                    string recCpyNo = "";//收款方公司编号
                    int recCpyType = 0;//收款方公司类型

                    //1付款，2 收款 3 分账 4 手续费

                    #region foreach

                    Tb_Order_PayDetail itemPayDetail = null;

                    for (int n = 0; n < payDetailList.Count; n++)
                    {
                        itemPayDetail = payDetailList[n];

                        #region 参数

                        if (itemPayDetail.RealPayMoney == 0 || itemPayDetail.PayType == "收款" || itemPayDetail.PayType == "手续费")
                            continue;
                        else if (itemPayDetail.PayType == "付款" || itemPayDetail.PayType == "分账")
                        {
                            if (itemPayDetail.CpyNo == mOrder.CPCpyNo)
                                continue;
                        }
                        else
                        {
                            continue;
                        }

                        tempAccountMoney = 0;
                        tempMaxDebtMoney = 0;

                        operLoginName = "";//操作人登录名
                        operUserName = "";//操作人名称
                        payCpyName = "";//付款方公司名称
                        payCpyNo = "";//付款方公司编号
                        payCpyType = 0;//付款方公司类型
                        recCpyName = "";//收款方公司名称
                        recCpyNo = "";//收款方公司编号
                        recCpyType = 0;//收款方公司类型

                        if (payUserCompany.RoleType == 1)
                        {
                            operLoginName = "系统";//操作人登录名
                            operUserName = "系统";//操作人名称
                        }
                        else
                        {
                            //可以退款
                            operLoginName = payUserEmployees.LoginName;//操作人登录名
                            operUserName = payUserEmployees.UserName;//操作人名称
                        }

                        #endregion

                        #region 赋值  判断分账退款用户是否有钱  修改用户金额、 添加 Log_MoneyDetail 日志

                        for (int i = 0; i < dsUser.Rows.Count; i++)
                        {
                            if (dsUser.Rows[i]["UninCode"].ToString().Trim() == itemPayDetail.CpyNo.Trim())
                            {

                                tempAccountMoney = decimal.Parse(dsUser.Rows[i]["AccountMoney"].ToString().Trim());
                                tempMaxDebtMoney = decimal.Parse(dsUser.Rows[i]["MaxDebtMoney"].ToString().Trim());

                                if (itemPayDetail.PayType == "分账")
                                {
                                    #region 分账退款处理

                                    if (tempAccountMoney >= itemPayDetail.RealPayMoney || tempAccountMoney + tempMaxDebtMoney >= itemPayDetail.RealPayMoney)
                                    {
                                        payCpyName = itemPayDetail.CpyName;//付款方公司名称
                                        payCpyNo = itemPayDetail.CpyNo;//付款方公司编号
                                        payCpyType = itemPayDetail.CpyType;//付款方公司类型
                                        recCpyName = mOrder.CPCpyName;//收款方公司名称
                                        recCpyNo = mOrder.CPCpyNo;//收款方公司编号
                                        recCpyType = payUserCompany.RoleType;//收款方公司类型
                                    }
                                    else
                                    {
                                        //不能退
                                        tempBool = true;
                                    }

                                    // 修改用户金额
                                    tempSql = "update User_Company set AccountMoney=AccountMoney-" + itemPayDetail.RealPayMoney + " where UninCode='" + itemPayDetail.CpyNo + "'";
                                    sqlList.Add(tempSql); //

                                    #endregion

                                    break;
                                }
                                else if (itemPayDetail.PayType == "付款")
                                {
                                    #region 收款退款处理

                                    payCpyName = mOrder.CPCpyName;//付款方公司名称
                                    payCpyNo = mOrder.CPCpyNo;//付款方公司编号
                                    payCpyType = payUserCompany.RoleType;//付款方公司类型  2  3
                                    recCpyName = itemPayDetail.CpyName;//收款方公司名称
                                    recCpyNo = itemPayDetail.CpyNo;//收款方公司编号
                                    recCpyType = itemPayDetail.CpyType;//收款方公司类型

                                    // 修改用户金额
                                    tempSql = "update User_Company set AccountMoney=AccountMoney+" + itemPayDetail.RealPayMoney + " where UninCode='" + itemPayDetail.CpyNo + "'";
                                    sqlList.Add(tempSql); //

                                    #endregion

                                    break;
                                }
                            }
                        }

                        if (tempBool == true)
                        {
                            msg = "收款方余额不足不能退款！";
                            break;
                        }

                        #endregion

                        #region 添加 Log_MoneyDetail 日志

                        logMoneyDetail = new Log_MoneyDetail();
                        logMoneyDetail.id = Guid.NewGuid();

                        logMoneyDetail.InPayNo = mOrder.InPayNo;//内部流水号
                        logMoneyDetail.OrderId = mOrder.OrderId;//订单编号
                        logMoneyDetail.PayNo = mOrder.InPayNo + n;//支付流水号
                        logMoneyDetail.OperLoginName = operLoginName;
                        logMoneyDetail.OperReason = "退款";
                        logMoneyDetail.OperTime = DateTime.Now;
                        logMoneyDetail.OperUserName = operUserName;
                        logMoneyDetail.PayCpyName = payCpyName;
                        logMoneyDetail.PayCpyNo = payCpyNo;
                        logMoneyDetail.PayCpyType = payCpyType;
                        logMoneyDetail.PayMoney = itemPayDetail.RealPayMoney;//发生金额
                        logMoneyDetail.PayType = 14;
                        logMoneyDetail.PreRemainMoney = tempAccountMoney; //之前账户余额
                        logMoneyDetail.RecCpyName = recCpyName;
                        logMoneyDetail.RecCpyNo = recCpyNo;
                        logMoneyDetail.RecCpyType = recCpyType;
                        //logMoneyDetail.RemainMoney = 0;
                        logMoneyDetail.Remark = "退款";
                        logMoneyDetail.A2 = 2;

                        tempList = new SortedList<string, string>();

                        tempSql = "(select AccountMoney from User_Company where UninCode='" + itemPayDetail.CpyNo + "')";

                        tempList.Add("RemainMoney", tempSql);
                        tempSql = Dal.Mapping.MappingHelper<Log_MoneyDetail>.CreateInsertModelSql(logMoneyDetail, tempList);
                        sqlList.Add(tempSql); //3.添加支付日志

                        #endregion
                    }

                    #endregion foreach 结束


                    #endregion

                    if (tempBool == true)
                    {
                        return false;
                    }

                    #region 4.添加 Tb_Ticket_Order 日志

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
                    OrderLog.OrderId = mOrder.OrderId;
                    OrderLog.OperType = "退款";
                    OrderLog.OperTime = DateTime.Now.AddSeconds(1);
                    OrderLog.OperLoginName = payUserEmployees.LoginName;
                    OrderLog.OperUserName = payUserEmployees.UserName;
                    OrderLog.CpyNo = mOrder.CPCpyNo;
                    OrderLog.CpyType = payUserCompany.RoleType;
                    OrderLog.CpyName = mOrder.CPCpyName;
                    OrderLog.OperContent = Content;
                    OrderLog.WatchType = 5;

                    tempSql = Dal.Mapping.MappingHelper<Log_Tb_AirOrder>.CreateInsertModelSql(OrderLog);
                    sqlList.Add(tempSql); //添加订单日志

                    #endregion

                    #region 5. 暂时不处理 Tb_Ticket_PayDetail  Tb_Order_PayDetail

                    #endregion

                    #region 6. 退款成功自动销账
                    List<string> strList = new PbProject.Logic.Order.Tb_Ticket_OrderBLL().AutomaticVirtualPayXZ(mOrder);

                    if (strList != null && strList.Count > 0)
                    {
                        foreach (string strItem in strList)
                        {
                            sqlList.Add(strItem); //添加订单日志
                        }
                    }

                    #endregion

                    //添加到数据
                    result = new Dal.ControlBase.BaseData<Tb_Order_PayDetail>().ExecuteSqlTran(sqlList);

                    msg = result ? "退款成功！" : "操作失败！";
                }
                catch (Exception ex)
                {
                    msg = "支付错误！";
                }
                return result;
            }
        }

        #endregion

        #region 在线退款 退款成功订单处理

        /// <summary>
        /// 在线退款 :退款成功订单处理
        /// </summary>
        /// <param name="orderid">订单号</param>
        /// <param name="payNo">交易号</param>
        ///  <param name="payWay">支付方式:PayWay 支付方式：1支付宝、2快钱、3汇付、4财付通、5支付宝网银、6快钱网银 、
        ///  7汇付网银、8财付通网银、9支付宝pos、10快钱pos、11汇付pos、12财付通pos、13易宝pos、14账户支付、15收银</param>
        ///  <param name="operReason">说明</param> 
        ///  <param name="remark">备注</param>  
        ///  <param name="returnContent">退款数据</param>  
        public bool CreateBillRefund(string orderid, string payNo, int payWay, string operReason, string remark, string returnContent)
        {
            lock (lockobject)
            {
                bool result = false;
                try
                {
                    string tempWhere = "";
                    string cpyNo = "";//付款公司
                    string RecNo = "";//收款公司

                    int orderStatusCode = 0;

                    User_CompanyBLL ucBll = new User_CompanyBLL();
                    User_Company payCompany = null; //付款公司
                    User_Employees payUserEmployees = null;//付款账号
                    User_Company RecCompany = null;//收款公司

                    #region 获取订单信息

                    tempWhere = " OrderId='" + orderid + "'";
                    Tb_Ticket_OrderBLL ticket_Order = new Tb_Ticket_OrderBLL();
                    List<Tb_Ticket_Order> reList = ticket_Order.GetListBySqlWhere(tempWhere);
                    Tb_Ticket_Order mOrderId = (reList != null && reList.Count > 0) ? reList[0] : null;

                    #endregion

                    if (mOrderId != null)
                    {
                        cpyNo = mOrderId.OwnerCpyNo; //付款公司
                        RecNo = mOrderId.CPCpyNo; //收款公司
                    }

                    #region 订单状态处理

                    //1	新订单，等待支付	新订单
                    //2	订单取消	订单取消
                    //3	已经支付，等待出票	等待出票
                    //4	已经出票，交易结束	出票成功
                    //5	取消出票，已退款	拒绝出票
                    //6	申请改签，等待审核	申请改签
                    //7	申请退票，等待审核	申请退票
                    //8	申请废票，等待审核	申请废票
                    //9	审核成功，等待补差	改签审核成功
                    //10	审核失败，拒绝改签	改签审核失败
                    //11	审核成功，等待退票	退票审核成功
                    //12	审核失败，拒绝退票	退票审核失败
                    //13	审核成功，等待废票	废票审核成功
                    //14	审核失败，拒绝废票	废票审核失败
                    //15	补差成功，等待确认	等待改签确认
                    //16	退票成功，交易结束	退票成功
                    //17	废票成功，交易结束	废票成功
                    //18	拒绝补差，改签失败	改签失败
                    //19	改签成功，交易结束	改签成功
                    //20	取消出票，退款中	提交退款成功,等待支付公司返回通知
                    //21	退票成功，退款中	提交退款成功,等待支付公司返回通知
                    //22	废票成功，退款中	提交退款成功,等待支付公司返回通知
                    //23	团订单申请,等待处理	团订单申请
                    //24	拒绝团订单申请,申请失败	拒绝团订单申请
                    //25	拒绝改签，退款中	拒绝改签
                    //26	拒绝改签，已退款	拒绝改签
                    //27	线下订单申请,等待处理	线下订单申请,等待处理
                    //28	拒绝线下订单申请,申请失败	拒绝线下订单申请,申请失败

                    orderStatusCode = mOrderId.OrderStatusCode;
                    string Content = "订单退款成功";

                    switch (mOrderId.OrderStatusCode)
                    {
                        case 20://取消出票，退款中
                            orderStatusCode = 5; //	取消出票，已退款 
                            Content = "取消出票,退款成功";
                            break;
                        case 21://退票成功，退款中
                            orderStatusCode = 16;//退票成功，交易结束
                            break;
                        case 22://废票成功，退款中
                            orderStatusCode = 17;//废票成功，交易结束
                            break;
                        case 23://拒绝改签，退款中，退款中
                            orderStatusCode = 24;//拒绝改签，已退款
                            break;
                        default:
                            orderStatusCode = mOrderId.OrderStatusCode;
                            break;
                    }

                    #endregion

                    #region 付款公司信息
                    tempWhere = " UninCode='" + cpyNo + "' ";

                    List<User_Company> payCompanyList = ucBll.GetListBySqlWhere(tempWhere);
                    payCompany = (payCompanyList != null && payCompanyList.Count > 0) ? payCompanyList[0] : null;

                    #endregion

                    #region 付款账号信息

                    tempWhere = " CpyNo='" + mOrderId.OwnerCpyNo + "' and IsAdmin=0 ";

                    List<User_Employees> payUserEmployeesList = new User_EmployeesBLL().GetBySQLList(tempWhere);
                    payUserEmployees = (payUserEmployeesList != null && payUserEmployeesList.Count > 0) ? payUserEmployeesList[0] : null;
                    #endregion

                    #region 收款公司信息

                    string gYcpyNo = RecNo.Substring(0, 12);
                    tempWhere = " UninCode='" + gYcpyNo + "' ";
                    List<User_Company> RecCompanyList = ucBll.GetListBySqlWhere(tempWhere);
                    RecCompany = (RecCompanyList != null && RecCompanyList.Count > 0) ? RecCompanyList[0] : null;

                    #endregion

                    List<string> sqlList = new List<string>();

                    string tempSql = "";

                    if (mOrderId != null && payCompany != null && payUserEmployees != null && RecCompany != null)
                    {
                        #region 1.添加交易日志

                        Log_MoneyDetail logMoneyDetail = new Log_MoneyDetail();
                        logMoneyDetail.id = Guid.NewGuid();
                        logMoneyDetail.InPayNo = mOrderId.InPayNo;//内部流水号
                        logMoneyDetail.OrderId = mOrderId.OrderId;//订单编号
                        logMoneyDetail.PayNo = payNo;//支付流水号
                        logMoneyDetail.OperLoginName = payUserEmployees.LoginName;
                        logMoneyDetail.OperReason = operReason;
                        logMoneyDetail.OperTime = DateTime.Now;
                        logMoneyDetail.OperUserName = payUserEmployees.UserName;
                        logMoneyDetail.PayCpyName = payCompany.UninAllName;
                        logMoneyDetail.PayCpyNo = payCompany.UninCode;
                        logMoneyDetail.PayCpyType = payCompany.RoleType;
                        logMoneyDetail.PayMoney = mOrderId.PayMoney;
                        logMoneyDetail.PayType = payWay;
                        logMoneyDetail.PreRemainMoney = payCompany.AccountMoney;
                        logMoneyDetail.RecCpyName = RecCompany.UninAllName;
                        logMoneyDetail.RecCpyNo = RecCompany.UninCode;
                        logMoneyDetail.RecCpyType = RecCompany.RoleType;
                        logMoneyDetail.RemainMoney = payCompany.AccountMoney;
                        logMoneyDetail.Remark = remark;
                        logMoneyDetail.A2 = 2;

                        tempSql = Dal.Mapping.MappingHelper<Log_MoneyDetail>.CreateInsertModelSql(logMoneyDetail);
                        sqlList.Add(tempSql);//1.添加交易日志

                        #endregion

                        #region 2.添加订单日志

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
                        OrderLog.OrderId = mOrderId.OrderId;
                        OrderLog.OperType = "退款";
                        OrderLog.OperTime = DateTime.Now;
                        OrderLog.OperLoginName = payUserEmployees.LoginName;
                        OrderLog.OperUserName = payUserEmployees.UserName;
                        OrderLog.CpyNo = payCompany.UninCode;
                        OrderLog.CpyType = payCompany.RoleType;
                        OrderLog.CpyName = payCompany.UninAllName;
                        OrderLog.OperContent = Content;
                        OrderLog.WatchType = payCompany.RoleType;

                        tempSql = Dal.Mapping.MappingHelper<Log_Tb_AirOrder>.CreateInsertModelSql(OrderLog);
                        sqlList.Add(tempSql); //2.添加交易日志

                        #endregion

                        #region 2.添加订单日志 单独记录退款数据

                        if (!string.IsNullOrEmpty(returnContent))
                        {

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

                            OrderLog = new Log_Tb_AirOrder();
                            OrderLog.id = Guid.NewGuid();
                            OrderLog.OrderId = mOrderId.OrderId;
                            OrderLog.OperType = "退款";
                            OrderLog.OperTime = DateTime.Now.AddSeconds(1);
                            OrderLog.OperLoginName = payUserEmployees.LoginName;
                            OrderLog.OperUserName = payUserEmployees.UserName;
                            OrderLog.CpyNo = payCompany.UninCode;
                            OrderLog.CpyType = payCompany.RoleType;
                            OrderLog.CpyName = payCompany.UninAllName;
                            OrderLog.OperContent = Content + ",退款明细:" + returnContent;
                            OrderLog.WatchType = 1;

                            tempSql = Dal.Mapping.MappingHelper<Log_Tb_AirOrder>.CreateInsertModelSql(OrderLog);
                            sqlList.Add(tempSql); //2.添加交易日志
                        }

                        #endregion

                        #region 3.修改订单状态

                        StringBuilder updateOrder = new StringBuilder();
                        updateOrder.Append(" update Tb_Ticket_Order set ");
                        updateOrder.Append(" PayTime='" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "', ");//支付时间

                        if (payWay == 2 || payWay == 6 || payWay == 3 || payWay == 7)
                            updateOrder.Append(" PayNo= '" + payNo + "', ");//订单状态

                        updateOrder.Append(" OrderStatusCode= " + orderStatusCode);//订单状态
                        updateOrder.Append(" where id='" + mOrderId.id + "' ");

                        tempSql = updateOrder.ToString();
                        sqlList.Add(tempSql);//3.修改订单状态

                        #endregion

                        #region 4. 处理  Tb_Ticket_PayDetail

                        tempWhere = " OrderId='" + mOrderId.OrderId + "' ";
                        List<Tb_Ticket_Passenger> PassengerList = baseDataManage.CallMethod("Tb_Ticket_Passenger", "GetList", null, new Object[] { tempWhere }) as List<Tb_Ticket_Passenger>;
                        //List<string> PassengerListSql = CreateOrderAndTicketPayDetail(mOrderId, PassengerList);

                        List<string> PassengerListSql = CreateOrderAndTicketPayDetailNew(mOrderId, PassengerList);

                        
                        foreach (string item in PassengerListSql)
                        {
                            sqlList.Add(item);//4.处理  Tb_Ticket_PayDetail
                        }

                        #endregion

                        //添加到数据
                        result = new Dal.ControlBase.BaseData<Log_MoneyDetail>().ExecuteSqlTran(sqlList);
                    }
                }
                catch (Exception ex)
                {

                }

                return result;
            }
        }

        /// <summary>
        /// 退款失败日志
        /// </summary>
        /// <returns></returns>
        public bool CreateBillRefundFailedLog(User_Employees uEmployees, string orderId, string details)
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
                OrderLog.OperType = "退款";
                OrderLog.OperTime = DateTime.Now.AddSeconds(1);
                OrderLog.OperLoginName = (uEmployees != null) ? uEmployees.LoginName : "";
                OrderLog.OperUserName = (uEmployees != null) ? uEmployees.UserName : "";
                OrderLog.CpyNo = (uEmployees != null) ? uEmployees.CpyNo : "";
                OrderLog.CpyType = 0;
                OrderLog.CpyName = "";
                OrderLog.OperContent = details;
                OrderLog.WatchType = 1;

                PbProject.Logic.ControlBase.BaseDataManage baseDataManage = new PbProject.Logic.ControlBase.BaseDataManage();
                result = (bool)baseDataManage.CallMethod("Log_Tb_AirOrder", "Insert", null, new Object[] { OrderLog });
                #endregion
            }
            catch (Exception)
            {

            }

            return result;
        }

        #endregion

        #region 在线支付:支付成功订单处理

        /// <summary>
        /// 生成在线支付账单
        /// </summary>
        /// <param name="orderid">订单号</param>
        /// <param name="payNo">交易号</param>
        ///  <param name="payWay">支付方式:PayWay 支付方式：1支付宝、2快钱、3汇付、4财付通、5支付宝网银、6快钱网银 、
        ///  7汇付网银、8财付通网银、9支付宝pos、10快钱pos、11汇付pos、12财付通pos、13易宝pos、14账户支付、15收银</param>
        /// <param name="MerPriv">自定义字段：支付人id</param>  
        ///  <param name="operReason">说明</param>  
        ///   <param name="remark">备注</param>  
        ///    <param name="payAct">支付账号</param>  
        public bool CreateBillPayBill(string orderid, string payNo, int payWay, string MerPriv, string operReason, string remark, string payAct)
        {
            lock (lockobject)
            {
                bool result = false;
                try
                {
                    string tempWhere = "";
                    string cpyNo = "";//付款公司
                    string RecNo = "";//收款公司

                    User_CompanyBLL ucBll = new User_CompanyBLL();
                    User_Company payCompany = null; //付款公司
                    User_Employees payUserEmployees = null;//付款账号
                    User_Company RecCompany = null;//收款公司

                    #region 获取订单信息

                    tempWhere = " OrderId='" + orderid + "'";
                    Tb_Ticket_OrderBLL ticket_Order = new Tb_Ticket_OrderBLL();
                    List<Tb_Ticket_Order> reList = ticket_Order.GetListBySqlWhere(tempWhere);
                    Tb_Ticket_Order mOrderId = (reList != null && reList.Count > 0) ? reList[0] : null;

                    #endregion

                    if (mOrderId != null && (mOrderId.OrderStatusCode == 1 || mOrderId.OrderStatusCode == 9))
                    {
                        cpyNo = mOrderId.OwnerCpyNo; //付款公司
                        RecNo = mOrderId.CPCpyNo; //收款公司

                        #region 付款公司信息
                        tempWhere = " UninCode='" + cpyNo + "' ";

                        List<User_Company> payCompanyList = ucBll.GetListBySqlWhere(tempWhere);
                        payCompany = (payCompanyList != null && payCompanyList.Count > 0) ? payCompanyList[0] : null;

                        #endregion

                        #region 付款账号信息

                        if (!string.IsNullOrEmpty(MerPriv))
                            tempWhere = " id='" + MerPriv + "'";
                        else
                            tempWhere = " CpyNo='" + mOrderId.OwnerCpyNo + "' and IsAdmin=0 ";

                        List<User_Employees> payUserEmployeesList = new User_EmployeesBLL().GetBySQLList(tempWhere);
                        payUserEmployees = (payUserEmployeesList != null && payUserEmployeesList.Count > 0) ? payUserEmployeesList[0] : null;
                        #endregion

                        #region 收款公司信息

                        string gYcpyNo = RecNo.Substring(0, 12);
                        tempWhere = " UninCode='" + gYcpyNo + "' ";
                        List<User_Company> RecCompanyList = ucBll.GetListBySqlWhere(tempWhere);
                        RecCompany = (RecCompanyList != null && RecCompanyList.Count > 0) ? RecCompanyList[0] : null;

                        #endregion

                        string tempSql = "";

                        List<string> sqlList = new List<string>();

                        if (mOrderId != null && payCompany != null && payUserEmployees != null && RecCompany != null)
                        {
                            #region 1.添加交易日志

                            Log_MoneyDetail logMoneyDetail = new Log_MoneyDetail();
                            logMoneyDetail.id = Guid.NewGuid();
                            logMoneyDetail.InPayNo = mOrderId.InPayNo;//内部流水号
                            logMoneyDetail.OrderId = mOrderId.OrderId;//订单编号
                            logMoneyDetail.PayNo = payNo;//支付流水号
                            logMoneyDetail.OperLoginName = payUserEmployees.LoginName;
                            logMoneyDetail.OperReason = operReason;
                            logMoneyDetail.OperTime = DateTime.Now;
                            logMoneyDetail.OperUserName = payUserEmployees.UserName;
                            logMoneyDetail.PayCpyName = payCompany.UninAllName;
                            logMoneyDetail.PayCpyNo = payCompany.UninCode;
                            logMoneyDetail.PayCpyType = payCompany.RoleType;
                            logMoneyDetail.PayMoney = mOrderId.PayMoney;
                            logMoneyDetail.PayType = payWay;
                            logMoneyDetail.PreRemainMoney = payCompany.AccountMoney;
                            logMoneyDetail.RecCpyName = RecCompany.UninAllName;
                            logMoneyDetail.RecCpyNo = RecCompany.UninCode;
                            logMoneyDetail.RecCpyType = RecCompany.RoleType;
                            logMoneyDetail.RemainMoney = payCompany.AccountMoney;
                            logMoneyDetail.Remark = remark;
                            logMoneyDetail.A2 = 1;

                            tempSql = Dal.Mapping.MappingHelper<Log_MoneyDetail>.CreateInsertModelSql(logMoneyDetail);
                            sqlList.Add(tempSql);//1.添加交易日志

                            #endregion

                            #region 2.添加订单日志

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

                            string Content = "订单支付成功";

                            Log_Tb_AirOrder OrderLog = new Log_Tb_AirOrder();
                            OrderLog.id = Guid.NewGuid();
                            OrderLog.OrderId = mOrderId.OrderId;
                            OrderLog.OperType = "支付";
                            OrderLog.OperTime = DateTime.Now;
                            OrderLog.OperLoginName = payUserEmployees.LoginName;
                            OrderLog.OperUserName = payUserEmployees.UserName;
                            OrderLog.CpyNo = payCompany.UninCode;
                            OrderLog.CpyType = payCompany.RoleType;
                            OrderLog.CpyName = payCompany.UninAllName;
                            OrderLog.OperContent = Content;
                            OrderLog.WatchType = payCompany.RoleType;

                            tempSql = Dal.Mapping.MappingHelper<Log_Tb_AirOrder>.CreateInsertModelSql(OrderLog);
                            sqlList.Add(tempSql); //2.添加交易日志

                            #endregion

                            #region 3.修改订单状态

                            StringBuilder updateOrder = new StringBuilder();
                            updateOrder.Append(" update Tb_Ticket_Order set ");
                            updateOrder.Append(" PayTime='" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "', ");//支付时间
                            updateOrder.Append(" PayStatus=1, "); //支付状态
                            updateOrder.Append(" PayWay=" + payWay + ", ");//支付方式
                            updateOrder.Append(" PayNo='" + payNo + "', ");//交易号

                            if (mOrderId.OrderStatusCode == 9)
                            {
                                updateOrder.Append(" OrderStatusCode=15 ");//订单状态
                            }
                            else
                            {
                                updateOrder.Append(" OrderStatusCode=3 ");//订单状态
                            }

                            updateOrder.Append(" where id='" + mOrderId.id + "' ");

                            tempSql = updateOrder.ToString();
                            sqlList.Add(tempSql);//3.修改订单状态

                            #endregion

                            #region 4.修改 Tb_Order_PayDetail 交易号

                            tempWhere = " OrderId='" + mOrderId.OrderId + "'";
                            tempSql = " update Tb_Order_PayDetail set PayNo='" + payNo + "' where " + tempWhere;
                            sqlList.Add(tempSql);//
                            #endregion

                            #region 5.修改 Tb_Ticket_PayDetail  交易号

                            tempWhere = " OrderId='" + mOrderId.OrderId + "'";
                            tempSql = "update Tb_Ticket_PayDetail set PayNo='" + payNo + "' where " + tempWhere;
                            sqlList.Add(tempSql);//
                            #endregion

                            #region 6.修改 Tb_Order_PayDetail 支付账号

                            if (!string.IsNullOrEmpty(payAct))
                            {
                                tempWhere = " OrderId='" + mOrderId.OrderId + "' and PayType='付款'";
                                tempSql = " update Tb_Order_PayDetail set PayAccount='" + payAct + "' where " + tempWhere;
                                sqlList.Add(tempSql);//
                            }

                            #endregion

                            //添加到数据
                            result = new Dal.ControlBase.BaseData<Log_MoneyDetail>().ExecuteSqlTran(sqlList);
                        }
                    }
                    else
                    {
                        result = false;  // 订单信息不正确

                    }
                }
                catch (Exception ex)
                {

                }

                return result;
            }
        }

        /// <summary>
        /// 已经确定支付成功但是状态未改变，手动支付补单
        /// </summary>
        /// <param name="orderId">订单号</param>
        /// <param name="payNo">交易号</param>
        /// <param name="payWay">支付方式</param>
        /// <returns></returns>
        public bool PayFillOrder(string orderId, string payNo, int payWay)
        {
            bool reuslt = false;

            try
            {
                reuslt = CreateBillPayBill(orderId, payNo, payWay, null, "支付成功未返回支付通知,系统补单!", "支付成功未返回支付通知,系统补单！", "");
            }
            catch (Exception)
            {

            }
            return reuslt;
        }

        #endregion

        #region 短信购买成功处理
        public bool MakeSMS(string Smsorderid, string PayNo, int PayWay)
        {

            bool rs = false;
            IHashObject parameter = new HashObject();
            IHashObject parametersmsuser = new HashObject();
            try
            {
                string tempSQl = "";
                List<string> sqlList = new List<string>();
                //充值记录
                List<Tb_Sms_ReCharge> Listrecharge = (baseDataManage.CallMethod("Tb_Sms_ReCharge", "GetList", null, new Object[] { "OrderId='" + Smsorderid + "'" }) as List<Tb_Sms_ReCharge>);
                string RechargeCpyNo = Listrecharge[0].CpyNo;
                //购买方公司信息
                List<User_Company> listcpy = (baseDataManage.CallMethod("User_Company", "GetList", null, new Object[] { "UninCode='" + RechargeCpyNo + "'" }) as List<User_Company>);
                if (PayWay == 14)
                {
                    #region 0.修改账户余额
                    tempSQl = "update User_Company set AccountMoney=AccountMoney-" + Listrecharge[0].ReChargeMoney + " where UninCode='" + RechargeCpyNo + "'";
                    sqlList.Add(tempSQl);//1.修改账号余额

                    #endregion
                }
                #region 1.修改短信用户信息(增加剩余条数)
                tempSQl = "Update Tb_Sms_User set SmsCount=SmsCount+" + Listrecharge[0].ReChargeCount + ",SmsRemainCount=SmsRemainCount+" + Listrecharge[0].ReChargeCount + " where CpyNo='" + RechargeCpyNo + "'";
                sqlList.Add(tempSQl);//1.修改充值前金额
                #endregion

                #region 2.修改运营商剩余条数（减少）
                if (RechargeCpyNo.Length > 12)
                {
                    tempSQl = "Update Tb_Sms_User set SmsRemainCount=SmsRemainCount-" + Listrecharge[0].ReChargeCount + " where CpyNo='" + RechargeCpyNo.Substring(0, 12) + "'";
                    sqlList.Add(tempSQl);//1.修改剩余条数
                }
                #endregion

                #region 3.修改充值记录信息
                tempSQl = "update Tb_Sms_ReCharge set ReChargeState = 2,PayDate='" + DateTime.Now + "',PayNo='" + PayNo + "',PayType=" + PayWay + " where id='" + Listrecharge[0].id + "'";
                sqlList.Add(tempSQl);
                #endregion

                #region 4.添加流水账
                List<User_Employees> payUserEmployeesList = new User_EmployeesBLL().GetBySQLList(" CpyNo='" + RechargeCpyNo + "' and IsAdmin=0");
                Log_MoneyDetail logmoney = new Log_MoneyDetail();
                logmoney.id = Guid.NewGuid();
                logmoney.OrderId = Smsorderid;
                logmoney.InPayNo = Listrecharge[0].InPayNo;
                logmoney.PayNo = PayNo;
                logmoney.PayType = PayWay;
                logmoney.PayCpyNo = RechargeCpyNo;
                logmoney.PayCpyType = listcpy[0].RoleType;
                logmoney.PayCpyName = listcpy[0].UninAllName;
                if (RechargeCpyNo.Length > 12)
                {
                    //销售方公司信息
                    List<User_Company> listcpy1 = (baseDataManage.CallMethod("User_Company", "GetList", null, new Object[] { "UninCode='" + RechargeCpyNo.Substring(0, 12) + "'" }) as List<User_Company>);
                    logmoney.RecCpyNo = listcpy1[0].UninCode;
                    logmoney.RecCpyName = listcpy1[0].UninAllName;
                    logmoney.RecCpyType = listcpy1[0].RoleType;
                }
                else
                {
                    logmoney.RecCpyNo = "100001";
                    logmoney.RecCpyName = "成都票宝网络有限公司";
                    logmoney.RecCpyType = 1;
                }
                logmoney.OperTime = DateTime.Now;
                logmoney.OperLoginName = payUserEmployeesList[0].LoginName;
                logmoney.OperUserName = payUserEmployeesList[0].UserName;
                logmoney.PreRemainMoney = listcpy[0].AccountMoney;
                logmoney.PayMoney = Listrecharge[0].ReChargeMoney;
                if (PayWay == 14)
                {
                    logmoney.RemainMoney = listcpy[0].AccountMoney - Listrecharge[0].ReChargeMoney;
                }
                else
                {
                    logmoney.RemainMoney = listcpy[0].AccountMoney;
                }
                logmoney.Remark = "短信充值";
                logmoney.OperReason = "短信充值";
                logmoney.A2 = 1;

                tempSQl = Dal.Mapping.MappingHelper<Log_MoneyDetail>.CreateInsertModelSql(logmoney);
                sqlList.Add(tempSQl);
                #endregion
                //添加到数据
                rs = new Dal.ControlBase.BaseData<Log_MoneyDetail>().ExecuteSqlTran(sqlList);
            }
            catch (Exception)
            {
                rs = false;
            }
            return rs;
        }

        #endregion

        #region 财付通分账回退更新
        public void UpdateBackState(string orderid, int state)
        {
            string sql = string.Format("update Tb_Order_PayDetail set A1={0} where OrderId='{1}' and PayType='付款'", state, orderid);
            SQLEXBLL_Base service = new SQLEXBLL_Base();
            service.ExecuteNonQuerySQLInfo(sql);
        }
        #endregion
    }
}

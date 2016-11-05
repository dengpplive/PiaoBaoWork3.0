using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PbProject.Model;

namespace PbProject.Logic.Pay
{
    /// <summary>
    /// 账户余额支付/收银
    /// </summary>
    public class VirtualPay
    {
        /// <summary>
        /// 锁定变量
        /// </summary>
        public static object lockobject = new object();

        /// <summary>
        /// 账户余额支付
        /// </summary>
        /// <param name="mOrder">订单mOrder</param>
        /// <param name="mUser">当前操作员model</param>
        /// <param name="messge">消息</param> 
        public bool DepositPay(Tb_Ticket_Order mOrder, User_Employees mUser, out string messge)
        {
            lock (lockobject)
            {
                bool result = false;
                messge = "";
                try
                {
                    result = new Bill().CreateVirtualPayBill(mOrder, mUser, out messge);
                }
                catch (Exception)
                {
                    messge = "支付异常！";
                }
                return result;
            }
        }

        /// <summary>
        /// 账户余额退款
        /// </summary>
        /// <param name="mOrder">订单mOrder<</param>
        /// <param name="mUser">当前操作员model</param>
        /// <param name="mCompany">当前操作员公司model</param>
        /// <param name="Page">当前页面对象</param>
        /// <param name="messge">消息</param>
        /// <returns></returns>
        public bool DepositRefund(Tb_Ticket_Order mOrder, User_Employees mUser, User_Company mCompany, out string messge)
        {
            lock (lockobject)
            {
                bool result = false;
                messge = "";
                try
                {
                    result = new Bill().CreateVirtualRefundBill(mOrder, mUser, mCompany, out messge);

                    if (!result)
                    {
                        //退款失败记录日志
                        new Bill().CreateBillRefundFailedLog(mUser,mOrder.OrderId, messge);
                    }
                }
                catch (Exception)
                {
                    messge = "退款异常！";
                }
                return result;
            }
        }

        /// <summary>
        /// 收银支付
        /// </summary>
        /// <param name="mOrder">订单mOrder</param>
        /// <param name="mUser">当前操作员model</param>
        /// <param name="mCompany">当前公司model</param>
        /// <param name="messge">消息</param>
        /// <returns></returns>
        public bool CashRegisterPay(Tb_Ticket_Order mOrder, User_Employees mUser, User_Company mCompany, out string messge)
        {
            lock (lockobject)
            {
                bool result = false;
                messge = "";
                try
                {
                    //修改订单 状态 和 添加日志
                    #region 1.修改订单

                    StringBuilder updateOrder = new StringBuilder();
                    updateOrder.Append(" update Tb_Ticket_Order set ");
                    updateOrder.Append(" PayWay=15,");
                    updateOrder.Append(" PayTime='" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "', ");//支付时间
                    updateOrder.Append(" OrderStatusCode=3 ");
                    updateOrder.Append(" where OrderId='" + mOrder.OrderId + "'");

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

                    string Content = "线下收银";

                    Log_Tb_AirOrder OrderLog = new Log_Tb_AirOrder();
                    OrderLog.id = Guid.NewGuid();
                    OrderLog.OrderId = mOrder.OrderId;
                    OrderLog.OperType = "支付";
                    OrderLog.OperTime = DateTime.Now;
                    OrderLog.OperLoginName = mUser.LoginName;
                    OrderLog.OperUserName = mUser.UserName;
                    OrderLog.CpyNo = mCompany.UninCode;
                    OrderLog.CpyType = mCompany.RoleType;
                    OrderLog.CpyName = mCompany.UninAllName;
                    OrderLog.OperContent = Content;
                    OrderLog.WatchType = 5;

                    #endregion

                    #region 添加到数据

                    List<string> sqlList = new List<string>();
                    sqlList.Add(updateOrder.ToString());

                    string LogTbAirOrder = Dal.Mapping.MappingHelper<Log_Tb_AirOrder>.CreateInsertModelSql(OrderLog);
                    sqlList.Add(LogTbAirOrder);

                    //添加到数据
                    result = new Dal.ControlBase.BaseData<Tb_Order_PayDetail>().ExecuteSqlTran(sqlList);

                    #endregion
                }
                catch (Exception)
                {
                    messge = "支付异常！";
                }
                return result;
            }
        }
    }
}

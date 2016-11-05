using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PiaoBao.Arch.RestfulWebServices;
using PbProject.Logic.Order;
using PbProject.Model;
using PnrAnalysis;
using PiaoBao.API.Common;
using PbProject.Logic.Pay;
using PbProject.Logic.PID;
using System.Text;
using PbProject.Logic.Policy;
using PnrAnalysis.Model;

namespace PiaoBao.API.Services
{
    public class OrderServices : BaseServices
    {

        //获取订单
        public override void Query(ResponseWriter writer, System.Collections.Specialized.NameValueCollection parames)
        {
            var orderID = parames["orderid"];
            var pnr = parames["pnr"];
            Tb_Ticket_OrderBLL bll = new Tb_Ticket_OrderBLL();
            var order = bll.GetTicketOrderByOrderId(orderID);
            if (order.PNR != pnr)
            {
                writer.WriteEx(541, "this Pnr is not match with order", "PNR编码与订单号不匹配");
            }
            else
            {
                writer.Write(order);
            }
        }
        //取消订单
        public override void Delete(ResponseWriter writer, System.Collections.Specialized.NameValueCollection parames)
        {
            var u = AuthLogin.GetUserInfo(Username);

            var orderID = parames["orderid"];
            var pnr = parames["pnr"];
            Tb_Ticket_OrderBLL bll = new Tb_Ticket_OrderBLL();
            var order = bll.GetTicketOrderByOrderId(orderID);
            if (order.PNR != pnr)
            {
                writer.WriteEx(541, "this Pnr is not match with order", "PNR编码与订单号不匹配");
            }
            else
            {
                string msg = string.Format(" 取消订单  订单号:{0}", orderID);

                if (bll.CancelOrder(order, u.User, u.Company, msg))
                {
                    writer.Write("取消成功");
                }
                else
                {
                    writer.WriteEx(541, "Cancel order operation failure", "取消订单操作失败");
                }
            }
        }
        //支付订单
        public override void Update(ResponseWriter writer, System.Collections.Specialized.NameValueCollection parames)
        {
            var u = AuthLogin.GetUserInfo(Username);

            var orderID = parames["orderid"];
            var pnr = parames["pnr"];
            Tb_Ticket_OrderBLL bll = new Tb_Ticket_OrderBLL();
            var order = bll.GetTicketOrderByOrderId(orderID);
            if (order.PNR != pnr)
            {
                writer.WriteEx(541, "this Pnr is not match with order", "PNR编码与订单号不匹配");
            }
            else if (string.IsNullOrEmpty(order.CPCpyNo))
            {
                writer.WriteEx(541, "this Order is not match with policy", "订单未匹配政策，请先确认订单");
            }
            else
            {
                if (order.PayWay != 14)
                {
                    order.PayWay = 14;

                    var result = new PbProject.Logic.Pay.Bill().UpdateOrderAndTicketPayDetail(order);
                    if (!result)
                    {
                        writer.WriteEx(542, "cutover pay method failure", "切换支付方式时失败");
                    }
                }
                PbProject.Logic.Pay.VirtualPay virtualPay = new PbProject.Logic.Pay.VirtualPay();
                string msgShow = "";
                if (virtualPay.DepositPay(order, u.User, out msgShow))
                {
                    writer.Write("支付成功");
                }
                else
                {
                    if (string.IsNullOrEmpty(msgShow))
                        msgShow = "支付失败";
                    writer.WriteEx(541, "Pay failure", msgShow);
                }

            }

        }

    }
}
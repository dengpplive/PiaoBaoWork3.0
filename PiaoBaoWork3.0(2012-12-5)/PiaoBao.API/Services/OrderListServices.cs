using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PiaoBao.Arch.RestfulWebServices;
using DataBase.Data;
using PbProject.Model;
using System.Text;
using PbProject.WebCommon.Utility;
using System.Text.RegularExpressions;
using PnrAnalysis;
using PnrAnalysis.Model;
using PbProject.Logic.Order;
using PbProject.Logic.Policy;
using PiaoBao.API.Common;

namespace PiaoBao.API.Services
{
    public class OrderListServices : BaseServices
    {
        /// <summary>
        /// 查询订单
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="parames"></param>
        public override void Query(ResponseWriter writer, System.Collections.Specialized.NameValueCollection parames)
        {
            var pager = parames["pager"].Split(';');
            int pageSize = int.Parse(pager[0]);
            int currPage = int.Parse(pager[1]);
            string con = SelWhere(parames);
            string orderby = "  CreateTime desc";

            int totalCount = 0;
            IHashObject outParams = new HashObject();
            //指定参数类型 第一个参数为out输出类型
            //key 为参数索引从1开始 value为引用类型 out ref
            outParams.Add("1", "out");
            PbProject.Logic.ControlBase.BaseDataManage baseDataManage = new PbProject.Logic.ControlBase.BaseDataManage();
            List<Tb_Ticket_Order> list = baseDataManage.CallMethod("Tb_Ticket_Order", "GetBasePager", outParams,
                new object[] { totalCount, pageSize, currPage, "*,dbo.GetCpyName(CPCpyNo) NewCpCpyName ", con, orderby }) as List<Tb_Ticket_Order>;

            totalCount = outParams.GetValue<int>("1");

            writer.Write(new
            {
                TotalCount = totalCount,
                PageSize = pageSize,
                List = list
            });
        }

        private string SelWhere(System.Collections.Specialized.NameValueCollection parames)
        {
            var loginUser = AuthLogin.GetUserInfo(Username);
            StringBuilder StrWhere = new StringBuilder(" A1=1 ");

            if (loginUser.Company.RoleType == 1)
            {
                if (parames["GYList"] != "")//出票公司编码
                {
                    StrWhere.AppendFormat(" and (CPCpyNo='{0}' or left(OwnerCpyNo,12)= '{0}' ) ", parames["GYList"]); //可查询共享
                }
            }
            else if (loginUser.Company.RoleType == 2)
            {
                StrWhere.Append(" and (CPCpyNo='" + loginUser.User.CpyNo + "' or left(OwnerCpyNo,12)= '" + loginUser.User.CpyNo + "' ) "); //可查询共享
            }
            else if (loginUser.Company.RoleType == 3)
            {
                StrWhere.Append(" and CPCpyNo='" + loginUser.User.CpyNo + "' ");
            }
            else if (loginUser.Company.RoleType == 4 || loginUser.Company.RoleType == 5)
            {
                StrWhere.Append(" and OwnerCpyNo='" + loginUser.User.CpyNo + "' ");
            }
            try
            {
                //订单号或者票号
                if (!string.IsNullOrEmpty(CommonManage.TrimSQL(parames["orderid"])))
                {
                    string OrderIdAndTicketNumber = CommonManage.TrimSQL(parames["orderid"]);
                    string pattern = @"^\d{3,4}(\-?|\s+)\d{10}$";
                    if (Regex.Match(OrderIdAndTicketNumber, pattern, RegexOptions.IgnoreCase).Success)
                    {
                        //票号
                        StrWhere.AppendFormat(" and  dbo.GetTicketNumber(OrderId) like '%|{0}|%' ", OrderIdAndTicketNumber);
                    }
                    else
                    {
                        //订单号
                        StrWhere.AppendFormat(" and OrderId='{0}' ", OrderIdAndTicketNumber);
                    }
                }
                //pnr
                if (!string.IsNullOrEmpty(CommonManage.TrimSQL(parames["pnr"])))
                    StrWhere.Append(" and PNR='" + CommonManage.TrimSQL(parames["pnr"]) + "' ");
                //乘机人
                if (!string.IsNullOrEmpty(parames["passengerName"]))
                    StrWhere.Append(" and PassengerName like'%" + CommonManage.TrimSQL(parames["passengerName"]) + "%' ");
                //客户名称
                if (!string.IsNullOrEmpty(CommonManage.TrimSQL(parames["corporationName"])))
                    StrWhere.Append(" and CreateCpyName like'%" + CommonManage.TrimSQL(parames["corporationName"]) + "%' ");
                //航班号
                if (!string.IsNullOrEmpty(CommonManage.TrimSQL(parames["flightCode"])))
                    StrWhere.Append(" and FlightCode ='" + CommonManage.TrimSQL(parames["flightCode"]) + "' ");
                //航空公司
                if (!string.IsNullOrEmpty(CommonManage.TrimSQL(parames["airCode"])))
                    StrWhere.Append(" and CarryCode ='" + CommonManage.TrimSQL(parames["airCode"]) + "' ");

                //订单状态
                if (!string.IsNullOrEmpty(parames["status"]) && parames["status"] != "0")
                    StrWhere.Append(" and OrderStatusCode= " + CommonManage.TrimSQL(parames["status"]));


                //乘机日期
                if (!string.IsNullOrEmpty(CommonManage.TrimSQL(parames["airFTimeDate"])))
                    StrWhere.Append(" and AirTime >'" + CommonManage.TrimSQL(parames["airFTimeDate"]) + " 00:00:00'");
                //乘机日期
                if (!string.IsNullOrEmpty(CommonManage.TrimSQL(parames["airETimeDate"])))
                    StrWhere.Append(" and AirTime <'" + CommonManage.TrimSQL(parames["airETimeDate"]) + " 23:59:59'");

                //创建日期
                if (!string.IsNullOrEmpty(CommonManage.TrimSQL(parames["createFTimeDate"])))
                    StrWhere.Append(" and CreateTime >'" + CommonManage.TrimSQL(parames["createFTimeDate"]) + " 00:00:00'");
                //创建日期
                if (!string.IsNullOrEmpty(CommonManage.TrimSQL(parames["createETimeDate"])))
                    StrWhere.Append(" and CreateTime <'" + CommonManage.TrimSQL(parames["createETimeDate"]) + " 23:59:59'");

                ////城市控件
                //if (txtFromCity.Value.Trim() != "" && txtFromCity.Value.Trim() != "中文/拼音" && txtFromCity.Value.Trim() != "中文/英文")
                //    StrWhere.Append(" and Travel like '" + CommonManage.TrimSQL(txtFromCity.Value.Trim()) + "%'");
                //if (txtToCity.Value.Trim() != "" && txtToCity.Value.Trim() != "中文/拼音" && txtToCity.Value.Trim() != "中文/英文")
                //    StrWhere.Append(" and Travel like '%" + CommonManage.TrimSQL(txtToCity.Value.Trim()) + "'");

                ////城市控件
                if (!string.IsNullOrEmpty(parames["fcity"]))
                    StrWhere.Append(" and TravelCode like '" + CommonManage.TrimSQL(parames["fcity"]) + "%'");
                if (!string.IsNullOrEmpty(parames["tcity"]))
                    StrWhere.Append(" and TravelCode like '%" + CommonManage.TrimSQL(parames["tcity"]) + "'");

            }
            catch (Exception)
            {

            }
            return StrWhere.ToString();

        }



        
    }
}
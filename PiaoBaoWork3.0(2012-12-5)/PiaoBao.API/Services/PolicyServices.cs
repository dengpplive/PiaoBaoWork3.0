using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PiaoBao.Arch.RestfulWebServices;
using PbProject.Model;
using PbProject.Logic.Policy;
using System.Text.RegularExpressions;
using PbProject.Logic.Order;
using System.Data;
using PiaoBao.API.Common;
using System.Web.Caching;

namespace PiaoBao.API.Services
{
    public class PolicyServices : BaseServices
    {
        //查询实时政策 http://ip/api/policy?orderId=...
        public override void Query(ResponseWriter writer, System.Collections.Specialized.NameValueCollection parames)
        {
            UserLoginInfo user = AuthLogin.GetUserInfo(Username);

            PbProject.Logic.ControlBase.BaseDataManage baseDataManage = new PbProject.Logic.ControlBase.BaseDataManage();
            string orderId = parames["orderId"];
            List<Tb_Ticket_Order> reList = baseDataManage.CallMethod("Tb_Ticket_Order", "GetList", null, new Object[] { string.Format("orderID='{0}'", orderId) }) as List<Tb_Ticket_Order>;
            if (reList == null && reList.Count == 0)
            {
                writer.WriteEx(541, "Can't Found Order by OrderID", "通过OrderID找不到订单信息");
            }
            else
            {
                Tb_Ticket_Order order = reList[0];
                PolicyMatch pm = new PolicyMatch();
                Regex reg = new Regex(@"(\w{3})-(\w{3})(/\2-(\w{3}))?");
                if (reg.IsMatch(order.TravelCode))
                {
                    var ma = reg.Match(order.TravelCode);
                    var fcity = ma.Groups[1].Value;
                    var mcity = ma.Groups[3].Success ? ma.Groups[2].Value : "";
                    var tcity = ma.Groups[3].Success ? ma.Groups[4].Value : ma.Groups[2].Value;

                    OrderInputParam InputParam = new OrderInputParam();
                    string ErrMsg = "";
                    Tb_Ticket_OrderBLL orderBLL = new Tb_Ticket_OrderBLL();
                    InputParam = orderBLL.GetOrder(orderId, InputParam, out ErrMsg);
                    PbProject.Logic.Buy.AirQurey airqurey = new PbProject.Logic.Buy.AirQurey(user.BaseParametersList, user.User, user.Company);
                    var catchID = airqurey.SkyListSaveCache(InputParam,"0");//PNR导入不能确定舱位是否是特价舱位,暂时默认写死为普通 YYY 2013-05-22
                    var swList = baseDataManage.CallMethod("Tb_Ticket_SkyWay", "GetList", null, new Object[] { string.Format("orderID='{0}'", orderId) }) as List<Tb_Ticket_SkyWay>;

                    if (swList == null && swList.Count == 0)
                    {
                        writer.WriteEx(541, "Can't Found Ticket_SkyWay by OrderID", "通过OrderID找不到航程信息");
                    }
                    swList = swList.OrderBy(p => p.FromDate).ToList();


                    var policy = pm.getMatchingPolicy(user.Company.UninCode, fcity, mcity, tcity, swList[0].FromDate.ToString(), swList.Count > 1 ? swList[1].FromDate.ToString() : swList[0].ToDate.ToString(), order.TravelType.ToString(), catchID, false, user.Company.GroupId, orderId, user.User, order.IsChdFlag, order.HaveBabyFlag, false);

                    var lst = dataSetToList(policy,true);

                    

                    writer.Write(lst);
                }
                else
                {
                    writer.WriteEx(541, "Error:TravelCode's format have error", "航班Code格式错误");
                }

            }


        }
        
        private List<PolicyParamForAPI> dataSetToList(DataSet ds,bool setCache)
        {
            List<PolicyParamForAPI> listAjAxPolicyParam = new List<PolicyParamForAPI>();

            for (int i = 0; i < ds.Tables.Count; i++)
            {
                for (int j = 0; j < ds.Tables[i].Rows.Count; j++)
                {

                    PolicyParamForAPI pmap = new PolicyParamForAPI();

                    pmap.CarryCode = ds.Tables[i].Rows[j]["CarryCode"].ToString();
                    pmap.CPCpyNo = ds.Tables[i].Rows[j]["CpyNo"].ToString();
                    pmap.Space = ds.Tables[i].Rows[j]["Space"].ToString();
                    pmap.DiscountRate = changeDecimal(ds.Tables[i].Rows[j]["DiscountRate"].ToString());
                    pmap.PolicyId = ds.Tables[i].Rows[j]["PolicyId"].ToString();
                    pmap.AirPoint = changeDecimal(ds.Tables[i].Rows[j]["AirPoint"].ToString());
                    pmap.AirReturnMoney = changeDecimal(ds.Tables[i].Rows[j]["AirReturnMoney"].ToString());

                    pmap.OldPolicyPoint = changeDecimal(ds.Tables[i].Rows[j]["OldPolicyPoint"].ToString());
                    pmap.OldPolicyReturnMoney = changeDecimal(ds.Tables[i].Rows[j]["OldReturnMoney"].ToString());
                    pmap.PolicyPoint = changeDecimal(ds.Tables[i].Rows[j]["PolicyPoint"].ToString());
                    pmap.PolicyReturnMoney = changeDecimal(ds.Tables[i].Rows[j]["ReturnMoney2"].ToString());

                    pmap.ReturnPoint = changeDecimal(ds.Tables[i].Rows[j]["Policy"].ToString());
                    pmap.PolicyYongJin = changeDecimal(ds.Tables[i].Rows[j]["Commission"].ToString());
                    pmap.PolicyShiFuMoney = changeDecimal(ds.Tables[i].Rows[j]["SJFee"].ToString());
                    pmap.LaterPoint = changeDecimal(ds.Tables[i].Rows[j]["LaterPoint"].ToString());
                    //pmap.PolicyReturnMoney = changeDecimal(ds.Tables[i].Rows[j]["ReturnMoney"].ToString());
                    pmap.SeatPrice = changeDecimal(ds.Tables[i].Rows[j]["SeatPrice"].ToString());//舱位价
                    pmap.ABFare = changeDecimal(ds.Tables[i].Rows[j]["ABFare"].ToString());//基建
                    pmap.RQFare = changeDecimal(ds.Tables[i].Rows[j]["RQFare"].ToString());//燃油
                    pmap.DiscountDetail = ds.Tables[i].Rows[j]["DiscountDetail"].ToString();
                    pmap.PolicyRemark = ds.Tables[i].Rows[j]["PolicyRemark"].ToString();
                    pmap.PolicyType = ds.Tables[i].Rows[j]["PolicyType"].ToString();
                    pmap.PolicyKind = changeInt(ds.Tables[i].Rows[j]["PolicyKind"].ToString());
                    pmap.AutoPrintFlag = ds.Tables[i].Rows[j]["AutoPrintFlag"].ToString();
                    pmap.PolicySource = ds.Tables[i].Rows[j]["PolicySource"].ToString();
                    pmap.PolicyOffice = ds.Tables[i].Rows[j]["PolicyOffice"].ToString();
                    pmap.DefaultType = ds.Tables[i].Rows[j]["DefaultType"].ToString();
                    pmap.HighPolicyFlag = ds.Tables[i].Rows[j]["HighPolicyFlag"].ToString();
                    pmap.WorkTime = ds.Tables[i].Rows[j]["WorkTime"].ToString();
                    pmap.PolicyCancelTime = ds.Tables[i].Rows[j]["PolicyCancelTime"].ToString();
                    pmap.PolicyReturnTime = ds.Tables[i].Rows[j]["PolicyReturnTime"].ToString();
                    pmap.FPGQTime = ds.Tables[i].Rows[j]["FPGQTime"].ToString();
                    pmap.chuPiaoShiJian = ds.Tables[i].Rows[j]["ChuPiaoShiJian"].ToString();


                    pmap.Guid = ds.Tables[i].Rows[j]["Guid"].ToString();

                    if (setCache)
                    {
                        PolicyCacheManager.Set(pmap.Guid, pmap);
                    }

                    listAjAxPolicyParam.Add(pmap);

                }
            }
            return listAjAxPolicyParam;
        }


        
        private decimal changeDecimal(string str)
        {
            decimal rs = 0m;
            decimal.TryParse(str, out rs);
            return rs;
        }
        private int changeInt(string str)
        {
            int rs = 0;
            int.TryParse(str, out rs);
            return rs;
        }
    }
}
<%@ WebHandler Language="C#" Class="GetPolicyInfo" %>

using System;
using System.Web;
using PbProject.Model;
using System.Collections.Generic;
using System.Data;
public class GetPolicyInfo : HttpHandle
{
    /// <summary>
    /// 响应客户端结果数据
    /// </summary>
    /// <param name="result"></param>
    public void OutPut(string result)
    {
        Ajax_Context.Response.ContentType = "text/plain";
        Ajax_Context.Response.Clear();
        Ajax_Context.Response.Write(result);
        Ajax_Context.Response.Flush();
    }
    /// <summary>
    /// 请求数据处理
    /// </summary>
    /// <param name="context"></param>
    /// <summary>  
    /// </summary>
    public override void DoHandleWork()
    {
        string result = GetPolicy(Context);
        //编码
        OutPut(result);
    }
    /// <summary>
    /// 获取匹配好的政策
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public string GetPolicy(HttpContext context)
    {
        string returnJson = "";
        string cpyno = mCompany.UninCode;
        string StartCityNameCode = checkRequest(context, "StartCityNameCode");//出发城市
        string MiddCityNameCode = checkRequest(context, "MiddCityNameCode");//中转城市
        string TargetCityNameCode = checkRequest(context, "TargetCityNameCode");//到达城市
        string FromDate = checkRequest(context, "FromDate");//出发时间
        string ReturnDate = checkRequest(context, "ReturnDate");//到达时间
        string TravelType = checkRequest(context, "TravelType");//行程类型
        string chaceNameByGUID = checkRequest(context, "chaceNameByGUID");//航班信息缓存ID
        bool IsOrGetPolicy = bool.Parse(checkRequest(context, "IsOrGetPolicy"));//是白屏还是获取政策
        string GroupId = checkRequest(context, "GroupId");//分组ID
        string OrderID = checkRequest(context, "OrderID");//订单ID
        bool HavaChild = bool.Parse(checkRequest(context, "HavaChild"));//是否有儿童
        bool IsINF = bool.Parse(checkRequest(context, "IsINF"));//是否有嬰兒

        PbProject.Logic.Policy.PolicyMatch plpp = new PbProject.Logic.Policy.PolicyMatch();
        DataSet ds = plpp.getMatchingPolicy(cpyno, StartCityNameCode, MiddCityNameCode, TargetCityNameCode, FromDate, ReturnDate, TravelType, chaceNameByGUID, IsOrGetPolicy, GroupId, OrderID, mUser, HavaChild, IsINF);
        if (ds != null)
        {
            if (IsOrGetPolicy)//ture是白屏查询
            {
                returnJson = PbProject.WebCommon.Utility.Encoding.JsonHelper.DataSetToJson(ds);
            }
            else
            {
                List<AjAxPolicyParam> listAjAxPolicyParam = new List<AjAxPolicyParam>();
                PbProject.Model.AjaxPolicyMatchOutData pma = new PbProject.Model.AjaxPolicyMatchOutData();
                for (int i = 0; i < ds.Tables.Count; i++)
                {
                    for (int j = 0; j < ds.Tables[i].Rows.Count; j++)
                    {

                        PbProject.Model.AjAxPolicyParam pmap = new PbProject.Model.AjAxPolicyParam();

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
                        pmap.PatchPonit = changeDecimal(ds.Tables[i].Rows[j]["PatchPonit"].ToString());
                        pmap.JinriGYCode = ds.Tables[i].Rows[j]["JinriGYCode"].ToString();
                        //B2B航空公司政策支付金额
                        pmap.AirPayMoney = changeDecimal(ds.Tables[i].Rows[j]["AirPayMoney"].ToString());
                        listAjAxPolicyParam.Add(pmap);
                    }
                }
                pma.OutPutPolicyList = listAjAxPolicyParam;
                pma.PolicyErrMsg = "";
                //添加支付方式
                PbProject.Logic.ControlBase.Bd_Base_ParametersBLL m_ParametersBLL = new PbProject.Logic.ControlBase.Bd_Base_ParametersBLL();
                pma = m_ParametersBLL.GetAjaxPolicyMatchOutDataNew(pma, baseParametersList, supBaseParametersList);

                returnJson = PbProject.WebCommon.Utility.Encoding.JsonHelper.ObjToJson(pma);
                PnrAnalysis.LogText.LogWrite(pma.ToString(OrderID), "PolicyData");
            }

        }
        return returnJson;
    }
    /// <summary>
    /// 字符串转换成decimal
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    private decimal changeDecimal(string str)
    {
        decimal rs = 0m;
        decimal.TryParse(str, out rs);
        return rs;
    }
    /// <summary>
    /// 字符串转换成int
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    private int changeInt(string str)
    {
        int rs = 0;
        int.TryParse(str, out rs);
        return rs;
    }
    /// <summary>
    /// 验证传入参数不为NULL
    /// </summary>
    /// <param name="context"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    private string checkRequest(HttpContext context, string name)
    {
        string result = "";
        if (context.Request[name] != null)
        {
            if (context.Request[name].ToString().Trim() != "")
            {
                result = context.Request[name].ToString().Trim();
            }
        }
        return result;

    }
}
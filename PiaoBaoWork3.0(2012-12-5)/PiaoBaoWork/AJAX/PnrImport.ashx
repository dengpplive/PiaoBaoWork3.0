<%@ WebHandler Language="C#" Class="PnrImport" %>

using System;
using System.Web;
using PbProject.Model;
using System.IO;
using System.Collections.Generic;
using DataBase.Data;
using PbProject.WebCommon.Utility.Encoding;
using System.Text;
using PbProject.WebCommon.Utility;
using PbProject.Logic.Buy;
using PnrAnalysis;
using PbProject.Logic.PID;
public class PnrImport : HttpHandle
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
       // Ajax_Context.Response.End();
    }
    /// <summary>
    /// 获取请求值
    /// </summary>
    /// <param name="Name">请求key名称</param>
    /// <param name="DefaultVal">数据默认值</param>
    /// <param name="DefaultVal">是否取的默认值 true是 false否</param>
    /// <returns></returns>
    public string GetVal(string Name, string DefaultVal)
    {
        if (Ajax_Request[Name] != null)
        {
            DefaultVal = HttpUtility.UrlDecode(Ajax_Request[Name].ToString(), Encoding.Default);
        }
        return DefaultVal;
    }
    /// <summary>
    /// 请求数据处理
    /// </summary>
    /// <param name="context"></param>
    /// <summary>  
    /// </summary>
    public override void DoHandleWork()
    {
        //this.ResetEncoding(System.Text.Encoding.UTF8);
        string result = PnrImportHandle(Context);
        //编码
        OutPut(result);
    }
    /// <summary>
    /// 分支
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public string PnrImportHandle(HttpContext context)
    {
        string result = "";
        string OPPage = Ajax_Request["OPPage"] != null ? Ajax_Request["OPPage"].ToString() : "";
        switch (OPPage)
        {
            case "Import":
                {
                    result = PnrImportHandle();
                    break;
                }
            case "KeHu":
                {
                    string UninCode = Ajax_Request["UninCode"] != null ? Ajax_Request["UninCode"].ToString() : "";
                    string ErrMsg = "";
                    //获取客户
                    result = GetKeHu(UninCode, out ErrMsg);
                }
                break;
            case "GetPnrInfo"://获取编码信息
                {
                    result = GetPnrInfo();
                }
                break;
            default:
                break;
        }
        return result;
    }
    /// <summary>
    /// 获取编码或者PNR内容信息
    /// </summary>
    /// <returns></returns>
    public string GetPnrInfo()
    {
        string result = "";
        string ErrMsg = "";
        //编码
        string PNR = GetVal("PNR", "");
        string PNRCon = GetVal("PNRCon", "");
        string OrderFangShi = GetVal("OrderFangShi", "0");
        string CpyNo = GetVal("CpyNo", "");
        string LoginName = GetVal("LoginName", "");
        PbProject.Model.ConfigParam config = null;
        FormatPNR format = new PnrAnalysis.FormatPNR();
        string strKongZhiXiTong = this.KongZhiXiTong;
        if (mCompany.RoleType == 1)
        {
            List<Bd_Base_Parameters> baseParamList = baseDataManage.CallMethod("Bd_Base_Parameters", "GetList", null, new Object[] { "CpyNo='" + CpyNo + "'" }) as List<Bd_Base_Parameters>;
            config = PbProject.Logic.ControlBase.Bd_Base_ParametersBLL.GetConfigParam(baseParamList);
            strKongZhiXiTong =  BaseParams.getParams(baseParamList).KongZhiXiTong;
        }
        if (this.configparam != null)
        {
            config = this.configparam;
        }
        PnrModel PM = null;
        //扩展参数
        ParamEx pe = new ParamEx();
        pe.UsePIDChannel = strKongZhiXiTong != null && strKongZhiXiTong.Contains("|48|") ? 2 : 0;
        PbProject.Logic.PID.SendInsManage SendManage = new PbProject.Logic.PID.SendInsManage(mUser.LoginName, mCompany.UninCode, pe, config);
        if (OrderFangShi == "1")
        {
            PM = SendManage.GetPnr(PNR, out ErrMsg);
        }
        else if (OrderFangShi == "2")
        {
            //解析出编码
            string pnr = format.GetPNR(PNRCon, out ErrMsg);
            if (string.IsNullOrEmpty(pnr))
            {
                ErrMsg = "该Pnr内容格式错误,未能解析出编码！";
            }
            else
            {
                PM = format.GetPNRInfo(pnr, PNRCon, false, out ErrMsg);
            }
        }
        if (PM != null)
        {
            result = "1@@@####" + JsonHelper.ObjToJson<PnrModel>(PM);
        }
        else
        {
            result = "0@@@####" + ErrMsg;
        }
        return result;
    }



    /// <summary>
    /// 获取客户账号
    /// </summary>
    /// <returns></returns>
    public string GetKeHu(string UniCode, out string ErrMsg)
    {
        ErrMsg = "";
        StringBuilder sbKH = new StringBuilder();
        try
        {
            PbProject.Dal.SQLEXDAL.SQLEXDAL_Base sqlexdal_base = new PbProject.Dal.SQLEXDAL.SQLEXDAL_Base();
            System.Data.DataTable table = sqlexdal_base.GetGYLowerEmpolyees(UniCode);
            sbKH.Append(" <option>---请选择客户名称---</option>");
            foreach (System.Data.DataRow dr in table.Rows)
            {
                sbKH.AppendFormat("<option value=\"{0}\">{1}</option>", dr["UninCode"].ToString() + "@" + dr["LoginName"].ToString() + "@" + dr["UninAllName"].ToString() + "@" + dr["Uid"].ToString() + "@" + dr["Cid"].ToString(), dr["LoginName"].ToString() + "-" + dr["UninAllName"].ToString());
            }
        }
        catch (Exception ex)
        {
            ErrMsg = ex.Message;
        }
        return sbKH.ToString();
    }
    /// <summary>
    /// 初始化Pnr导入参数
    /// </summary>
    /// <returns></returns>
    public PnrImportParam InitImportParam()
    {
        //导入通道
        string ImportTongDao = Ajax_Request["ImportTongDao"].ToString();
        string RoleType = Ajax_Request["RoleType"].ToString();
        string IsMerge = Ajax_Request["IsMerge"].ToString();
        string IsSecond = Ajax_Request["IsSecond"].ToString();
        string Pnr = Ajax_Request["Pnr"].ToString().Replace("'","");
        string BigPnr = Ajax_Request["BigPnr"].ToString().Replace("'", "");
        string OrderId = Ajax_Request["OrderId"].ToString().Replace("'", "");
        string RTAndPAT = HttpUtility.UrlDecode(Ajax_Request["RTAndPAT"].ToString(), Encoding.UTF8).Replace("'", "");
        string RTData = HttpUtility.UrlDecode(Ajax_Request["RTData"].ToString(), Encoding.UTF8).Replace("'", "");
        string PATDATA = HttpUtility.UrlDecode(Ajax_Request["PATDATA"].ToString(), Encoding.UTF8).Replace("'", "");
        string AllowChangePNR = Ajax_Request["AllowChangePNR"].ToString();
        string Office = Ajax_Request["Office"].ToString();
        string PasList = Ajax_Request["PasList"] != null ? HttpUtility.UrlDecode(Ajax_Request["PasList"].ToString(), Encoding.Default).Replace("'", "") : "";
        SortedList<string, string> sortList = new SortedList<string, string>();
        if (!string.IsNullOrEmpty(PasList))
        {
            if (IsSecond != "0")
            {
                string[] strArr = PasList.Split(new string[] { "@@@" }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string item in strArr)
                {
                    if (item.Split('|').Length == 2)
                    {
                        sortList.Add(item.Split('|')[0], item.Split('|')[1]);
                    }
                }
            }
        }
        PnrImportParam Param = new PnrImportParam();
        //Pnr导入参数
        Param.RoleType = RoleType;
        Param.Pnr = Pnr;
        Param.BigPnr = BigPnr;
        Param.ImportTongDao = int.Parse(ImportTongDao);
        Param.IsMerge = int.Parse(IsMerge);
        Param.IsSecond = int.Parse(IsSecond);
        Param.OrderId = OrderId;
        Param.PATData = PATDATA;
        Param.RTAndPatData = RTAndPAT;
        Param.RTData = RTData;
        Param.Office = Office;
        Param.AllowChangePNR = int.Parse(AllowChangePNR);
        Param.SecondPM.PATContent = PATDATA;
        Param.SecondPM.PnrContent = RTData;
        Param.SecondPM.Office = Office;
        Param.sortList = sortList;
        Param.m_LoginCompany = mCompany;
        Param.m_LoginUser = mUser;
        return Param;
    }

    /// <summary>
    /// pnr导入处理
    /// </summary>   
    /// <returns></returns>
    public string PnrImportHandle()
    {
        string result = "";
        PnrImportParam Param = InitImportParam();
        if (Param.RoleType != "")
        {
            Param.SecondPM.Msg = Param.TipMsg;
            //提示
            Param.SecondPM.OpType = "0";
            bool IsNext = true;
            // string spData = "!@!";
            if (Param.IsSecond == 0)
            {
                if (Param.Pnr.Trim() != "")
                {
                    string sqlWhere = string.Format(" PNR='{0}' ", Param.Pnr);
                    bool IsExist = (bool)baseDataManage.CallMethod("Tb_Ticket_Order", "IsExist", null, new object[] { sqlWhere });
                    if (IsExist)
                    {
                        Param.SecondPM.Msg = "此编码已导入过,是否需要再次导入";
                        Param.SecondPM.OpType = "1";
                        Param.SecondPM.ErrCode = "0";
                        Param.SecondPM.DataFlag = "0";
                        //result = string.Format("0{0}继续{0}1{0}此编码已导入过,是否需要再次导入", spData);
                        IsNext = false;
                    }
                }
            }
            if (IsNext)
            {
                //处理
                string strKongZhiXiTong = BaseParams.getParams(supBaseParametersList).KongZhiXiTong;
                string strGongYingKongZhiFenXiao = BaseParams.getParams(baseParametersList).GongYingKongZhiFenXiao;
                PnrImportManage Mange = new PnrImportManage(mUser, mCompany, mSupCompany, strKongZhiXiTong, strGongYingKongZhiFenXiao, this.configparam);
                bool IsSuc = Mange.GetImportPnrInfo(Param);
                if (IsSuc)
                {
                    //生成的订单
                    Tb_Ticket_Order Order = Param.OrderParam.OrderParamModel[0].Order;
                    string Url = "";
                    if (Order.IsChdFlag)
                    {
                        Url = "Confirmation.aspx?ChildOrderId=" + Order.OrderId;
                    }
                    else
                    {
                        Url = "Confirmation.aspx?AdultOrderId=" + Order.OrderId;
                    }
                    if (Param.ImportTongDao == 5)
                    {
                        Url = "PayMent.aspx?id=" + Order.id;
                    }
                    Param.SecondPM.ErrCode = "1";
                    Param.SecondPM.DataFlag = "2";
                    Param.SecondPM.OpType = "0";
                    Param.SecondPM.GoUrl = Url;
                    Param.SecondPM.Msg = "成功";
                    //Response.Redirect(Url, false);
                    //OutPut("1@@成功@@0@@" + data);
                    //result = string.Format("1{0}成功{0}0{0}{1}", spData, Url);

                }
                else
                {
                    Param.SecondPM.ErrCode = "0";
                    if (Param.IsNextOK == 1)
                    {
                        //继续操作
                        Param.SecondPM.OpType = "1";
                    }
                    else
                    {
                        Param.SecondPM.Msg = Param.TipMsg;
                        //提示
                        Param.SecondPM.OpType = "0";
                    }
                }
            }
        }
        else
        {
            Param.SecondPM.Msg = "当前用户无效,请重新登录！";
            //提示
            Param.SecondPM.OpType = "0";
        }
        //返回数据格式
        //错误代码@@错误提示@@操作类型@@接收数据
        result = JsonHelper.ObjToJson<ImportTipParam>(Param.SecondPM);
        return result;
    }
}
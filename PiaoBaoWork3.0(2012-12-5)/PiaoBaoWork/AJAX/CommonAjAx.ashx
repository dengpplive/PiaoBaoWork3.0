<%@ WebHandler Language="C#" Class="CommonAjAx" %>

using System;
using System.Web;
using System.Web.SessionState;
using PbProject.Logic.ControlBase;
using System.Text;
using PbProject.Model;
using System.IO;
using System.Collections.Generic;
using DataBase.Data;
using PbProject.WebCommon.Utility.Encoding;
using PbProject.Logic.Order;
using PbProject.Logic.Pay;
using PbProject.WebCommon.Utility;
using PbProject.Logic.PID;
public class CommonAjAx : HttpHandle
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
        Ajax_Context.Response.End();
    }
    /// <summary>
    /// 请求数据处理
    /// </summary>
    /// <param name="context"></param>
    /// <summary>  
    /// </summary>
    public override void DoHandleWork()
    {
        string result = ProcessData(Context);
        //编码
        OutPut(result);
    }
    /// <summary>
    /// 数据分支
    /// </summary>
    /// <param name="context"></param>
    /// <returns>状态##错误描述##返回数据</returns>
    public string ProcessData(HttpContext context)
    {
        string state = "-1", Msg = "", result = "";
        if (mUser == null) return string.Format("{0}##{1}##{2}", state, Msg, escape(result));
        bool IsDefault = false;
        //操作功能
        string OpFunction = GetVal("OpFunction", "-1", out IsDefault);
        //操作页面
        string OpPage = GetVal("OpPage", "-1", out IsDefault);
        //操作类型 0添加 1修改 2删除 3查询
        string OpType = GetVal("OpType", "-1", out IsDefault);
        switch (OpFunction)
        {
            case "OpPolicy":
                {
                    //是否复制政策
                    string IsCopy = GetVal("Copy", "", out IsDefault);
                    //操作政策
                    if (AddAndUpdatePolicy(out state, out Msg, out result, OpPage, OpType))
                    {
                        state = "1";
                        Msg = "政策" + GetOpType(OpType) + "成功";
                    }
                    else
                    {
                        Msg = "政策" + GetOpType(OpType) + "失败,错误原因如下:<br />" + Msg;
                    }
                }
                break;
            case "GroupPolicy":
                {
                    //散冲团政策
                    if (GroupPolicy(out state, out Msg, out result, OpPage, OpType))
                    {
                        state = "1";
                        Msg = "政策" + GetOpType(OpType) + "成功";
                    }
                    else
                    {
                        Msg = "政策" + GetOpType(OpType) + "失败,错误原因如下:<br />" + Msg;
                    }
                }
                break;
            case "PTSpace":
                {
                    //获取普通仓位
                    result = GetPTSpace(out Msg);
                }
                break;
            case "TJSpace":
                {
                    //获取特价仓位
                    result = GetTJSpace(out Msg);
                }
                break;

            case "defaultPolicy":
                {
                    //获取默认政策
                    result = GetDefaultPolicyList();
                }
                break;
            case "lineOrder":
                {
                    //线下订单
                    if (CreateLineOrder(out state, out Msg, out result, OpPage, OpType))
                    {
                        state = "1";
                        Msg = "线下订单生成成功！<br />" + result;
                    }
                    else
                    {
                        Msg = "线下订单生成失败！错误原因如下:<br />" + Msg;
                    }
                }
                break;
            case "lineOrderProcess":
                {
                    bool Isdefault = false;
                    //线下订单处理
                    string Flag = GetVal("Flag", "0", out Isdefault);
                    string id = GetVal("Id", "", out Isdefault);
                    string StatusCode = GetVal("Status", "", out Isdefault);
                    if (Flag == "1")
                    {
                        //修改订单状态
                        if (UpdateOrderStatus(id, StatusCode))
                        {
                            state = "1";
                        }
                        else
                        {
                            Msg = "拒绝失败！错误原因如下:<br />" + Msg;
                        }
                    }
                    else if (Flag == "2")
                    {
                        //审核订单
                        if (UpdateOrder(out state, out Msg, out result, OpPage, OpType))
                        {
                            state = "1";
                            Msg = "订单审核成功！";
                        }
                        else
                        {
                            Msg = "审核订单失败！错误原因如下:<br />" + Msg;
                        }
                    }
                }
                break;
            case "CancelOrder":
                {
                    if (CancelOrder(out state, out Msg, out result, OpPage, OpType))
                    {
                        state = "1";
                        Msg = Msg.Trim(new char[] { ',' });
                    }
                    else
                    {
                        Msg = "订单取消失败！错误原因如下:<br />" + Msg;
                    }
                }
                break;
            case "Suppend":
                {
                    //航空公司政策挂起解挂
                    if (Suppend(out state, out Msg, out result, OpPage, OpType))
                    {
                        state = "1";
                    }
                }
                break;
            default:
                Msg = "未能进入操作功能项";
                break;
        }
        return string.Format("{0}##{1}##{2}", state, Msg, escape(result));
    }

    /// <summary>
    /// 航空公司挂起解挂
    /// </summary>
    /// <param name="errCode"></param>
    /// <param name="errMsg"></param>
    /// <param name="result"></param>
    /// <param name="PageName"></param>
    /// <param name="OpType"></param>
    /// <returns></returns>
    public bool Suppend(out string errCode, out string errMsg, out string result, string PageName, string OpType)
    {
        errCode = "";//用代码表示
        errMsg = "";//用错误描述表示
        result = "";//用于查询返回结果
        //操作成功 还是失败
        bool IsSuc = false;
        bool IsDefault = false;
        //1挂起 0解挂
        string SupType = GetVal("SupType", "1", out IsDefault);
        string IsWhere = GetVal("IsWhere", "", out IsDefault);
        try
        {
            //记录日志
            StringBuilder sbLogCon = new StringBuilder();
            string CpyNo = GetVal("CpyNo", "", out IsDefault);
            string AirB2b = GetVal("b2b", "", out IsDefault);
            string AirBSP = GetVal("bsp", "", out IsDefault);
            string LoginName = GetVal("LoginName", "", out IsDefault);
            string UserName = GetVal("UserName", "", out IsDefault);
            string RoleType = GetVal("RoleType", "", out IsDefault);
            //SQL列表
            List<string> sqlList = new List<string>();
            StringBuilder sbWhereLog = new StringBuilder();
            if (IsWhere == "0")//航空公司挂起解挂
            {
                // PolicyType:1=B2B，2=BSP，3=B2B/BSP
                string sqlB2B = string.Format(" update Tb_Ticket_Policy set IsPause={0} where PolicyType=1 and CpyNo='{1}' and CarryCode in({2}) and A1=0  ", SupType, CpyNo, AirB2b);

                string sqlBSP = string.Format(" update Tb_Ticket_Policy set IsPause={0} where PolicyType=2 and CpyNo='{1}' and CarryCode in({2}) and A1=0  ", SupType, CpyNo, AirBSP);
                if (!string.IsNullOrEmpty(AirBSP))
                {
                    sqlList.Add(sqlBSP);
                }
                if (!string.IsNullOrEmpty(AirB2b))
                {
                    sqlList.Add(sqlB2B);
                }
            }
            else if (IsWhere == "1")//根据条件挂起解挂
            {
                string CarrayCode = GetVal("CarrayCode", "", out IsDefault);
                string FromCityCode = GetVal("FromCityCode", "", out IsDefault);
                string MiddleCityCode = GetVal("MiddleCityCode", "", out IsDefault);
                string ToCityCode = GetVal("ToCityCode", "", out IsDefault);
                string PolicyType = GetVal("PolicyType", "", out IsDefault);
                string StartDate = GetVal("StartDate", "", out IsDefault);
                string EndDate = GetVal("EndDate", "", out IsDefault);

                sbWhereLog.AppendFormat("公司编号:{0}   ", CpyNo);
                sbWhereLog.AppendFormat("航空公司二字码:{0}   ", CarrayCode);
                sbWhereLog.AppendFormat("出发城市三字码:{0}   ", FromCityCode);
                sbWhereLog.AppendFormat("中转城市三字码:{0}   ", MiddleCityCode);
                sbWhereLog.AppendFormat("到达城市三字码:{0}   ", ToCityCode);
                sbWhereLog.AppendFormat("政策类型:{0}   ", (PolicyType == "1" ? "B2B" : (PolicyType == "2" ? "BSP" : "全部")));
                sbWhereLog.AppendFormat("乘机日期:{0}=>{1}", StartDate, EndDate);

                string sql = string.Format(" update Tb_Ticket_Policy set IsPause={0} where ", SupType);
                //条件
                StringBuilder sbWhere = new StringBuilder();
                sbWhere.AppendFormat(" CpyNo='{0}' and A1=0 ", CpyNo);
                if (PolicyType != "3")
                {
                    sbWhere.AppendFormat(" and PolicyType={0}", PolicyType);
                }
                if (CarrayCode.Trim() != "")
                {
                    sbWhere.AppendFormat(" and CarryCode='/{0}/'", CarrayCode);
                }
                if (FromCityCode.Trim() != "")
                {
                    sbWhere.AppendFormat(" and StartCityNameCode like '%/{0}/%'", FromCityCode);
                }
                if (MiddleCityCode.Trim() != "")
                {
                    sbWhere.AppendFormat(" and MiddleCityNameCode like  '%/{0}/%'", MiddleCityCode);
                }
                if (ToCityCode.Trim() != "")
                {
                    sbWhere.AppendFormat(" and TargetCityNameCode like  '%/{0}/%'", ToCityCode);
                }
                if (StartDate != "" && EndDate != "")
                {
                    sbWhere.Append(" and FlightStartDate  >='" + StartDate + " 00:00:00' and FlightEndDate <= '" + EndDate + " 23:59:59' ");
                }
                sql = string.Format(sql + " {0} ", sbWhere.ToString());
                //添加sql
                sqlList.Add(sql);
            }
            //执行操作
            if (this.baseDataManage.ExecuteSqlTran(sqlList, out errMsg))
            {
                if (IsWhere == "0")
                {
                    string b2bAir = string.Join("/", AirB2b.Replace("/", "").Split(new string[] { "'", "," }, StringSplitOptions.RemoveEmptyEntries));
                    string bspAir = string.Join("/", AirBSP.Replace("/", "").Split(new string[] { "'", "," }, StringSplitOptions.RemoveEmptyEntries));
                    sbLogCon.Append((SupType == "1" ? "挂起" : "解挂") + (string.IsNullOrEmpty(b2bAir) ? "" : "<br />B2B:" + b2bAir) + (string.IsNullOrEmpty(bspAir) ? "" : "   <br />BSP:" + bspAir));
                }
                else if (IsWhere == "1")
                {
                    sbLogCon.Append((SupType == "1" ? "挂起" : "解挂") + "政策筛选条件=>" + sbWhereLog.ToString());
                }
                //记录日志
                Log_Operation log_operation = new Log_Operation();
                log_operation.ModuleName = "政策挂起解挂";
                //1挂起 0解挂
                log_operation.OperateType = "政策" + (SupType == "1" ? "挂起" : "解挂");
                log_operation.CpyNo = CpyNo;
                log_operation.LoginName = LoginName;
                log_operation.UserName = UserName;
                log_operation.OrderId = PageName;//操作页面
                log_operation.CreateTime = System.DateTime.Now;
                log_operation.OptContent = sbLogCon.ToString();
                log_operation.ClientIP = this.ClientIP;
                //记录日志
                if (AddOperationLog(log_operation))
                {
                    IsSuc = true;
                    errMsg = (SupType == "1" ? "挂起成功" : "解挂成功");
                }
                else
                {
                    errMsg = (SupType == "1" ? "挂起失败" : "解挂失败");
                }
            }
            else
            {
                errMsg = (SupType == "1" ? "挂起失败" : "解挂失败");
            }
        }
        catch (Exception ex)
        {
            errMsg = (SupType == "1" ? "挂起失败" : "解挂失败") + "原因如下:" + ex.Message;
        }
        return IsSuc;
    }



    /// <summary>
    /// 获取普通仓位
    /// </summary>
    /// <returns></returns>
    public string GetPTSpace(out string Msg)
    {
        string reVal = "";
        Msg = "";
        try
        {
            bool IsDefault = false;
            //获取普通仓位
            string AirCode = GetVal("AirCode", "", out IsDefault);
            string sqlWhere = " ";
            if (!string.IsNullOrEmpty(AirCode))
            {
                sqlWhere = string.Format(" AirCode='{0}' order by rebate ", AirCode);
            }
            List<Bd_Air_BaseCabin> list = this.baseDataManage.CallMethod("Bd_Air_BaseCabin", "GetList", null, new object[] { sqlWhere }) as List<Bd_Air_BaseCabin>;
            if (list != null && list.Count > 0)
            {
                reVal = JsonHelper.ObjToJson<List<Bd_Air_BaseCabin>>(list);
            }
            else
            {
                Msg = "未找到该航空公司(" + AirCode + ")基本仓位!";
            }
        }
        catch (Exception ex)
        {
            Msg = ex.Message;
            PnrAnalysis.LogText.LogWrite(ex.Message, "CommonAjAx\\GetPTSpace");
        }
        return reVal;
    }

    /// <summary>
    /// 获取特价仓位
    /// </summary>
    /// <returns></returns>
    public string GetTJSpace(out string Msg)
    {
        string reVal = "";
        Msg = "";
        try
        {
            bool IsDefault = false;
            //获取特价仓位
            string AirCode = GetVal("AirCode", "", out IsDefault);
            string sqlWhere = string.Format(" CpyNo='{0}' ", mCompany.UninCode);
            if (!string.IsNullOrEmpty(AirCode))
            {
                sqlWhere = string.Format(" SpAirCode='{0}' and " + sqlWhere, AirCode);
            }
            List<Tb_SpecialCabin> list = this.baseDataManage.CallMethod("Tb_SpecialCabin", "GetList", null, new object[] { sqlWhere }) as List<Tb_SpecialCabin>;
            if (list != null && list.Count > 0)
            {
                reVal = JsonHelper.ObjToJson<List<Tb_SpecialCabin>>(list);
            }
            else
            {
                Msg = "该航空公司(" + AirCode + ")没有发布特价仓位,请发布特价舱位后操作！";
            }
        }
        catch (Exception ex)
        {
            Msg = ex.Message;
            PnrAnalysis.LogText.LogWrite(ex.Message, "CommonAjAx\\GetTJSpace");
        }
        return reVal;
    }
    /// <summary>
    /// 获取默认政策
    /// </summary>
    /// <param name="cpyNO"></param>
    /// <param name="AirCode"></param>
    /// <returns></returns>
    public string GetDefaultPolicyList()
    {
        bool IsDefault = false;
        string cpyNO = GetVal("cpyNO", "", out IsDefault);
        string AirCode = GetVal("CarryCode", "", out IsDefault);
        string sqlWhere = string.Format("  CpyNo='{0}' and CarryCode like '%{1}%' and A1 in(1,2) ", cpyNO, AirCode.Trim(new char[] { '/' }));
        List<Tb_Ticket_Policy> defaultList = this.baseDataManage.CallMethod("Tb_Ticket_Policy", "GetList", null, new object[] { sqlWhere }) as List<Tb_Ticket_Policy>;

        Tb_Ticket_Policy adult = defaultList.Find(delegate(Tb_Ticket_Policy tb_ticket_policy)
        {
            return (tb_ticket_policy.A1 == 1 ? true : false);
        });
        Tb_Ticket_Policy child = defaultList.Find(delegate(Tb_Ticket_Policy tb_ticket_policy)
        {
            return (tb_ticket_policy.A1 == 2 ? true : false);
        });
        string AdultResult = "", ChildResult = "";
        if (adult != null)
        {
            AdultResult = JsonHelper.ObjToJson<Tb_Ticket_Policy>(adult);
        }
        if (child != null)
        {
            ChildResult = JsonHelper.ObjToJson<Tb_Ticket_Policy>(child);
        }
        return AdultResult + "$@@@@@@@$" + ChildResult;
    }

    /// <summary>
    /// 获取请求值
    /// </summary>
    /// <param name="Name">请求key名称</param>
    /// <param name="DefaultVal">数据默认值</param>
    /// <param name="DefaultVal">是否取的默认值 true是 false否</param>
    /// <returns></returns>
    public string GetVal(string Name, string DefaultVal, out bool IsDefault)
    {
        IsDefault = false;
        if (Ajax_Request[Name] != null)
        {
            DefaultVal = HttpUtility.UrlDecode(Ajax_Request[Name].ToString(), Encoding.Default).Replace("'", "");
        }
        else
        {
            IsDefault = true;
        }
        return DefaultVal;
    }
    /// <summary>
    /// 操作类型字符串显示
    /// </summary>
    /// <returns></returns>
    public string GetOpType(string OpType)
    {
        string strOpType = "";
        if (OpType == "0")
        {
            strOpType = "添加";
        }
        else if (OpType == "1")
        {
            strOpType = "修改";
        }
        else if (OpType == "2")
        {
            strOpType = "删除";
        }
        else if (OpType == "3")
        {
            strOpType = "查询";
        }
        else if (OpType == "4")
        {
            strOpType = "复制";
        }
        else if (OpType == "5")
        {
            strOpType = "已审核";
        }
        else if (OpType == "6")
        {
            strOpType = "未审核";
        }
        return strOpType;
    }

    /// <summary>
    /// 添加和修改政策
    /// </summary>
    /// <param name="errCode">错误代码编号</param>
    /// <param name="errMsg">内部错误信息</param>
    /// <param name="errMsg">查询返回结果</param>
    /// <param name="errMsg">操作页面</param>
    /// <param name="OpType"> 0添加 1修改 2删除 3查询</param>
    /// <returns></returns>
    public bool AddAndUpdatePolicy(out string errCode, out string errMsg, out string result, string PageName, string OpType)
    {
        errCode = "";//用代码表示
        errMsg = "";//用错误描述表示
        result = "";//用于查询返回结果
        //操作成功 还是失败
        bool IsSuc = false;
        Tb_Ticket_Policy tb_ticket_policy = null;
        Tb_Ticket_Policy Temp_tb_ticket_policy = null;
        List<string> IdsList = new List<string>();
        bool IsDefault = false;
        //公司编号
        string CpyNo = GetVal("CpyNo", mCompany.UninCode, out IsDefault);
        if (CpyNo == "")
        {
            CpyNo = mCompany.UninCode;
            if (CpyNo == "")
            {
                errMsg = "公司编号为空,请重新登录！";
                return IsSuc;
            }
        }
        //创建人帐户
        string CreateLoginName = GetVal("LoginName", mUser.LoginName, out IsDefault);
        //供应商名字
        string CpyName = GetVal("CpyName", mUser.UserName, out IsDefault);
        try
        {
            //id
            string Id = GetVal("Id", "", out IsDefault);
            if (PageName == "AddDefaultPolicy.aspx")//默认政策
            {
                #region 默认政策处理
                string _tempCarryCode = GetVal("CarryCode", "", out IsDefault);
                string _tempA1 = GetVal("A1", "0", out IsDefault);
                if (_tempA1 != "0")
                {
                    StringBuilder sbDefaultSqlWhere = new StringBuilder();
                    sbDefaultSqlWhere.Append(" 1=1 ");
                    //ID不为空
                    if (!string.IsNullOrEmpty(Id))
                    {
                        sbDefaultSqlWhere.AppendFormat(" and id='{0}' ", Id);//修改
                        OpType = "1";
                    }
                    else
                    {
                        sbDefaultSqlWhere.AppendFormat(" and  CpyNo='{0}' and CarryCode like '%{1}%' and A1 in({2}) ", CpyNo, _tempCarryCode.Trim(new char[] { '/' }), _tempA1);
                        List<Tb_Ticket_Policy> defaultList = this.baseDataManage.CallMethod("Tb_Ticket_Policy", "GetList", null, new object[] { sbDefaultSqlWhere.ToString() }) as List<Tb_Ticket_Policy>;

                        Tb_Ticket_Policy DefaultPolicy = defaultList.Find(delegate(Tb_Ticket_Policy _T1)
                        {
                            return (_T1.A1.ToString() == _tempA1 ? true : false);
                        });
                        if (DefaultPolicy != null)//存在修改
                        {
                            Id = DefaultPolicy.id.ToString();
                            OpType = "1";
                        }
                        else//添加
                        {
                            OpType = "0";
                        }
                    }
                }
                #endregion
            }


            if (OpType == "0")
            {
                //添加
                tb_ticket_policy = new Tb_Ticket_Policy();
            }
            else if (OpType == "1" || OpType == "4")
            {
                //修改
                tb_ticket_policy = this.baseDataManage.CallMethod("Tb_Ticket_Policy", "GetById", null, new object[] { Id }) as Tb_Ticket_Policy;
                //拷贝一个新实例 用于比较数据记日志
                Temp_tb_ticket_policy = this.baseDataManage.CallMethod("Tb_Ticket_Policy", "CopyModel", null, new object[] { tb_ticket_policy }) as Tb_Ticket_Policy;
            }
            else if (OpType == "2")
            {
                //根据id集合删除
                IdsList.AddRange(Id.Replace("\'", "").Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries));
            }
            else
            {
                //查询
            }
            if (tb_ticket_policy != null)
            {
                #region  获取所有请求数据

                //政策种类 1.普通，2.特价
                string strPolicyKind = GetVal("PolicyKind", (OpType == "1") ? tb_ticket_policy.PolicyKind.ToString() : "0", out IsDefault);
                int PolicyKind = 1;
                int.TryParse(strPolicyKind, out PolicyKind);


                //票价生成方式 1.正常价格，2.动态特价，3.固定特价
                string strGenerationType = GetVal("GenerationType", OpType == "1" ? tb_ticket_policy.GenerationType.ToString() : "1", out IsDefault);
                int GenerationType = 1;
                int.TryParse(strGenerationType, out GenerationType);

                //发布类型 1.出港，2.入港,3.全国
                string strReleaseType = GetVal("ReleaseType", OpType == "1" ? tb_ticket_policy.ReleaseType.ToString() : "1", out IsDefault);
                int ReleaseType = 1;
                int.TryParse(strReleaseType, out ReleaseType);

                //承运人 航空公司编号 二字码 
                string CarryCode = GetVal("CarryCode", OpType == "1" ? tb_ticket_policy.CarryCode : "", out IsDefault);

                //行程类型 1.单程，2.往返/单程，3.往返，4.中转联程
                string strTravelType = GetVal("TravelType", OpType == "1" ? tb_ticket_policy.TravelType.ToString() : "1", out IsDefault);
                int TravelType = 1;
                int.TryParse(strTravelType, out TravelType);

                //政策类型 1=B2B，2=BSP，3=B2B/BSP
                string strPolicyType = GetVal("PolicyType", OpType == "1" ? tb_ticket_policy.PolicyType.ToString() : "1", out IsDefault);
                int PolicyType = 1;
                int.TryParse(strPolicyType, out PolicyType);

                //团队标志 0.普通，1.团队
                string strTeamFlag = GetVal("TeamFlag", OpType == "1" ? tb_ticket_policy.TeamFlag.ToString() : "0", out IsDefault);
                int TeamFlag = 0;
                int.TryParse(strTeamFlag, out TeamFlag);

                //出票Office号
                string Office = GetVal("Office", OpType == "1" ? tb_ticket_policy.Office : "", out IsDefault);

                //出发城市三字码（全国政策填：ALL）
                string StartCityNameCode = GetVal("FromCityCode", OpType == "1" ? tb_ticket_policy.StartCityNameCode : "", out IsDefault);

                //中转城市三字码
                string MiddleCityNameCode = GetVal("MiddleCityCode", OpType == "1" ? tb_ticket_policy.MiddleCityNameCode : "", out IsDefault);

                //到达城市三字码（全国政策填：ALL）
                string TargetCityNameCode = GetVal("ToCityCode", OpType == "1" ? tb_ticket_policy.TargetCityNameCode : "", out IsDefault);

                //城市同城机场共享政策1.是，2.否
                string strCityNameSame = GetVal("CityNameSame", "2", out IsDefault);
                int StartCityNameSame = (OpType == "1" && IsDefault) ? tb_ticket_policy.StartCityNameSame : 2;
                int TargetCityNameSame = (OpType == "1" && IsDefault) ? tb_ticket_policy.MiddleCityNameSame : 2;
                int MiddleCityNameSame = (OpType == "1" && IsDefault) ? tb_ticket_policy.TargetCityNameSame : 2;
                if (ReleaseType == 1)
                {
                    //出发城市同城机场共享政策 1.是，2.否
                    string strStartCityNameSame = strCityNameSame;
                    int.TryParse(strStartCityNameSame, out StartCityNameSame);
                }
                else if (ReleaseType == 2)
                {
                    //到达城市同城机场共享政策 1.是，2.否
                    string strTargetCityNameSame = strCityNameSame;
                    int.TryParse(strTargetCityNameSame, out TargetCityNameSame);
                }
                else
                {
                    //中转城市同城机场共享政策 1.是，2.否
                    string strMiddleCityNameSame = strCityNameSame;
                    int.TryParse(strMiddleCityNameSame, out MiddleCityNameSame);
                }

                //适用航班号类型 1.全部2.适用3.不适用
                string strApplianceFlightType = GetVal("FlightType", OpType == "1" ? tb_ticket_policy.ApplianceFlightType.ToString() : "1", out IsDefault);
                int ApplianceFlightType = 1;
                int.TryParse(strApplianceFlightType, out ApplianceFlightType);
                //航班值
                string strFlightValue = GetVal("FlightValue", "", out IsDefault);
                //适用航班
                string ApplianceFlight = "";
                //不适用航班
                string UnApplianceFlight = "";
                if (ApplianceFlightType == 1)
                {
                    ApplianceFlight = (OpType == "1" && IsDefault) ? tb_ticket_policy.ApplianceFlight : "";
                    UnApplianceFlight = (OpType == "1" && IsDefault) ? tb_ticket_policy.UnApplianceFlight : "";
                }
                else if (ApplianceFlightType == 2)
                {
                    ApplianceFlight = (OpType == "1" && IsDefault) ? tb_ticket_policy.ApplianceFlight : strFlightValue;
                    UnApplianceFlight = (OpType == "1" && IsDefault) ? tb_ticket_policy.UnApplianceFlight : "";
                }
                else if (ApplianceFlightType == 3)
                {
                    ApplianceFlight = (OpType == "1" && IsDefault) ? tb_ticket_policy.ApplianceFlight : "";
                    UnApplianceFlight = (OpType == "1" && IsDefault) ? tb_ticket_policy.UnApplianceFlight : strFlightValue;
                }
                //班期限制 周一到周日对应值1-7
                string ScheduleConstraints = GetVal("Schedule", OpType == "1" ? tb_ticket_policy.ScheduleConstraints : "", out IsDefault);
                //舱位
                string ShippingSpace = GetVal("ShippingSpace", OpType == "1" ? tb_ticket_policy.ShippingSpace : "", out IsDefault);

                //舱位价格（特价）
                string strSpacePrice = GetVal("SpacePrice", OpType == "1" ? tb_ticket_policy.SpacePrice.ToString() : "0", out IsDefault);
                decimal SpacePrice = 0m;
                decimal.TryParse(strSpacePrice, out SpacePrice);

                //舱位价格（参考特价）
                string strReferencePrice = GetVal("ReferencePrice", OpType == "1" ? tb_ticket_policy.ReferencePrice.ToString() : "0", out IsDefault);
                decimal ReferencePrice = 0m;
                decimal.TryParse(strReferencePrice, out ReferencePrice);

                //提前天数
                string strAdvanceDay = GetVal("AdvanceDay", OpType == "1" ? tb_ticket_policy.AdvanceDay.ToString() : "0", out IsDefault);
                int AdvanceDay = 0;
                int.TryParse(strAdvanceDay, out AdvanceDay);

                //下级分销返点
                string strDownPoint = GetVal("DownPoint", OpType == "1" ? tb_ticket_policy.DownPoint.ToString() : "0", out IsDefault);
                decimal DownPoint = 0m;
                decimal.TryParse(strDownPoint, out DownPoint);

                //下级分销后返
                string strLaterPoint = GetVal("LaterPoint", OpType == "1" ? tb_ticket_policy.LaterPoint.ToString() : "0", out IsDefault);
                decimal LaterPoint = 0m;
                decimal.TryParse(strLaterPoint, out LaterPoint);

                //共享政策返点
                string strSharePoint = GetVal("SharePoint", OpType == "1" ? tb_ticket_policy.SharePoint.ToString() : "0", out IsDefault);
                decimal SharePoint = 0m;
                decimal.TryParse(strSharePoint, out SharePoint);

                //航空公司返点
                string strAirReBate = GetVal("AirReBate", OpType == "1" ? tb_ticket_policy.AirReBate.ToString() : "0", out IsDefault);
                decimal AirReBate = 0m;
                decimal.TryParse(strAirReBate, out AirReBate);

                //下级分销现返金额
                string strDownReturnMoney = GetVal("DownReturnMoney", OpType == "1" ? tb_ticket_policy.DownReturnMoney.ToString() : "0", out IsDefault);
                decimal DownReturnMoney = 0m;
                decimal.TryParse(strDownReturnMoney, out DownReturnMoney);

                //下级分销后返现返金额
                string strLaterReturnMoney = GetVal("LaterReturnMoney", OpType == "1" ? tb_ticket_policy.LaterReturnMoney.ToString() : "0", out IsDefault);
                decimal LaterReturnMoney = 0m;
                decimal.TryParse(strLaterReturnMoney, out LaterReturnMoney);

                //下级分销后返现返金额
                string strShareReturnMoney = GetVal("ShareReturnMoney", OpType == "1" ? tb_ticket_policy.SharePointReturnMoney.ToString() : "0", out IsDefault);
                decimal ShareReturnMoney = 0m;
                decimal.TryParse(strShareReturnMoney, out ShareReturnMoney);

                //航空公司现返金额
                string strAirReturnMoney = GetVal("AirReturnMoney", OpType == "1" ? tb_ticket_policy.AirReBateReturnMoney.ToString() : "0", out IsDefault);
                decimal AirReBateReturnMoney = 0m;
                decimal.TryParse(strAirReturnMoney, out AirReBateReturnMoney);


                //乘机生效日期
                string strFlightStartDate = GetVal("FlightStartDate", OpType == "1" ? tb_ticket_policy.FlightStartDate.ToString("yyyy-MM-dd") : "1900-01-01", out IsDefault);
                DateTime FlightStartDate = DateTime.Parse(strFlightStartDate);
                //乘机失效日期
                string strFlightEndDate = GetVal("FlightEndDate", OpType == "1" ? tb_ticket_policy.FlightEndDate.ToString("yyyy-MM-dd") : "1900-01-01", out IsDefault) + " 23:59:59";
                DateTime FlightEndDate = DateTime.Parse(strFlightEndDate);
                //出票生效日期
                string strPrintStartDate = GetVal("PrintStartDate", OpType == "1" ? tb_ticket_policy.PrintStartDate.ToString("yyyy-MM-dd") : "1900-01-01", out IsDefault);
                DateTime PrintStartDate = DateTime.Parse(strPrintStartDate);
                //出票失效日期
                string strPrintEndDate = GetVal("PrintEndDate", OpType == "1" ? tb_ticket_policy.PrintEndDate.ToString("yyyy-MM-dd") : "1900-01-01", out IsDefault) + " 23:59:59";
                DateTime PrintEndDate = DateTime.Parse(strPrintEndDate);

                //审核人帐户
                string AuditLoginName = (OpType == "1") ? tb_ticket_policy.AuditLoginName : GetVal("", "", out IsDefault);
                //审核人姓名
                string AuditName = (OpType == "1") ? tb_ticket_policy.AuditName : GetVal("", "", out IsDefault);
                //审核时间
                string strAuditDate = (OpType == "1") ? tb_ticket_policy.AuditDate.ToString() : GetVal("", "1900-01-01", out IsDefault);
                DateTime AuditDate = DateTime.Parse(strAuditDate);
                //审核状态 1.已审，2.未审
                string strAuditType = GetVal("AuditType", OpType == "1" ? tb_ticket_policy.AuditType.ToString() : "2", out IsDefault);
                int AuditType = 2;
                int.TryParse(strAuditType, out AuditType);
                if (AuditType == 1)
                {
                    AuditDate = OpType == "1" ? tb_ticket_policy.AuditDate : System.DateTime.Now;
                    AuditLoginName = OpType == "1" ? tb_ticket_policy.CreateLoginName : CreateLoginName;
                    AuditName = OpType == "1" ? tb_ticket_policy.CpyName : CpyName;
                }
                //创建时间 添加时间
                string strCreateDate = GetVal("", "1900-01-01", out IsDefault);
                DateTime CreateDate = OpType == "1" ? tb_ticket_policy.CreateDate : DateTime.Parse(strCreateDate);
                //更新时间
                string strUpdateDate = GetVal("", "1900-01-01", out IsDefault);
                DateTime UpdateDate = OpType == "1" ? tb_ticket_policy.UpdateDate : DateTime.Parse(strUpdateDate);
                //更新人账户
                string UpdateLoginName = OpType == "1" ? tb_ticket_policy.UpdateLoginName : GetVal("", "", out IsDefault);
                //更新人姓名
                string UpdateName = OpType == "1" ? tb_ticket_policy.UpdateName : GetVal("", "", out IsDefault);
                if (OpType == "0")
                {
                    CreateDate = System.DateTime.Now;
                }
                else if (OpType == "1")
                {
                    UpdateDate = System.DateTime.Now;
                    UpdateLoginName = CreateLoginName;
                    UpdateName = CpyName;
                }
                //政策备注
                string Remark = GetVal("PolicyRemark", OpType == "1" ? tb_ticket_policy.Remark : "", out IsDefault);
                //这条政策是否适用于共享航班 1适用 0不适用
                string strIsApplyToShareFlight = GetVal("IsApplyToShareFlight", OpType == "1" ? tb_ticket_policy.IsApplyToShareFlight.ToString() : "0", out IsDefault);
                int IsApplyToShareFlight = 0;
                int.TryParse(strIsApplyToShareFlight, out IsApplyToShareFlight);
                //适用共享航空公司二字码 如:CA/CZ/ZH/HU
                string ShareAirCode = GetVal("ShareAirCode", OpType == "1" ? tb_ticket_policy.ShareAirCode : "", out IsDefault);
                //是否往返低开 0不低开 1低开
                string strIsLowerOpen = GetVal("IsLowerOpen", OpType == "1" ? tb_ticket_policy.IsLowerOpen.ToString() : "0", out IsDefault);
                int IsLowerOpen = 0;
                int.TryParse(strIsLowerOpen, out IsLowerOpen);
                //是否高返政策 1是 其他不是
                string strHighPolicyFlag = GetVal("HighPolicyFlag", OpType == "1" ? tb_ticket_policy.HighPolicyFlag.ToString() : "0", out IsDefault);
                int HighPolicyFlag = 0;
                int.TryParse(strHighPolicyFlag, out HighPolicyFlag);

                //自动出票方式 手动(0或者null空)， 半自动1， 自动2
                string strAutoPrintFlag = GetVal("AutoPrintFlag", OpType == "1" ? tb_ticket_policy.AutoPrintFlag.ToString() : "0", out IsDefault);
                int AutoPrintFlag = 0;
                int.TryParse(strAutoPrintFlag, out AutoPrintFlag);

                //专属扣点组ID
                string GroupId = GetVal("GroupId", OpType == "1" ? tb_ticket_policy.GroupId : "0", out IsDefault);
                //备用字段
                int A1 = int.Parse(GetVal("A1", OpType == "1" ? tb_ticket_policy.A1.ToString() : "0", out IsDefault));
                int A2 = int.Parse(GetVal("A2", OpType == "1" ? tb_ticket_policy.A2.ToString() : "0", out IsDefault));
                int A3 = int.Parse(GetVal("A3", OpType == "1" ? tb_ticket_policy.A3.ToString() : "0", out IsDefault));
                int A4 = int.Parse(GetVal("A4", OpType == "1" ? tb_ticket_policy.A4.ToString() : "0", out IsDefault));
                decimal A5 = decimal.Parse(GetVal("A5", OpType == "1" ? tb_ticket_policy.A5.ToString() : "0", out IsDefault));
                decimal A6 = decimal.Parse(GetVal("A6", OpType == "1" ? tb_ticket_policy.A6.ToString() : "0", out IsDefault));
                decimal A7 = decimal.Parse(GetVal("A7", OpType == "1" ? tb_ticket_policy.A7.ToString() : "0", out IsDefault));
                decimal A8 = decimal.Parse(GetVal("A8", OpType == "1" && IsDefault ? tb_ticket_policy.A8.ToString() : "0", out IsDefault));
                DateTime A9 = DateTime.Parse(GetVal("A9", OpType == "1" ? tb_ticket_policy.A9.ToString("yyyy-MM-dd HH:mm:ss") : "1900-01-01", out IsDefault));
                DateTime A10 = DateTime.Parse(GetVal("A10", OpType == "1" ? tb_ticket_policy.A10.ToString("yyyy-MM-dd HH:mm:ss") : "1900-01-01", out IsDefault));
                DateTime A11 = DateTime.Parse(GetVal("A11", OpType == "1" ? tb_ticket_policy.A11.ToString("yyyy-MM-dd HH:mm:ss") : "1900-01-01", out IsDefault));
                DateTime A12 = DateTime.Parse(GetVal("A12", OpType == "1" ? tb_ticket_policy.A12.ToString("yyyy-MM-dd HH:mm:ss") : "1900-01-01", out IsDefault));
                string A13 = GetVal("A13", OpType == "1" ? tb_ticket_policy.A13 : "", out IsDefault);
                string A14 = GetVal("A14", OpType == "1" ? tb_ticket_policy.A14 : "", out IsDefault);
                string A15 = GetVal("A15", OpType == "1" ? tb_ticket_policy.A15 : "", out IsDefault);
                string A16 = GetVal("A16", OpType == "1" ? tb_ticket_policy.A16 : "", out IsDefault);
                #endregion


                #region 赋值
                tb_ticket_policy.CpyNo = CpyNo;
                tb_ticket_policy.CpyName = CpyName;
                tb_ticket_policy.PolicyKind = PolicyKind;
                tb_ticket_policy.GenerationType = GenerationType;
                tb_ticket_policy.ReleaseType = ReleaseType;
                tb_ticket_policy.CarryCode = CommonManage.TrimSQL("/" + CarryCode.Trim(new char[] { '/' }) + "/");
                tb_ticket_policy.TravelType = TravelType;
                tb_ticket_policy.PolicyType = PolicyType;
                tb_ticket_policy.TeamFlag = TeamFlag;
                tb_ticket_policy.Office = CommonManage.TrimSQL(Office);
                tb_ticket_policy.StartCityNameCode = CommonManage.TrimSQL("/" + StartCityNameCode.Trim(new char[] { '/' }) + "/");
                tb_ticket_policy.StartCityNameSame = StartCityNameSame;
                tb_ticket_policy.MiddleCityNameCode = "/" + MiddleCityNameCode.Trim(new char[] { '/' }) + "/";
                tb_ticket_policy.MiddleCityNameSame = MiddleCityNameSame;
                tb_ticket_policy.TargetCityNameCode = "/" + TargetCityNameCode.Trim(new char[] { '/' }) + "/";
                tb_ticket_policy.TargetCityNameSame = TargetCityNameSame;
                tb_ticket_policy.ApplianceFlightType = ApplianceFlightType;
                tb_ticket_policy.ApplianceFlight = CommonManage.TrimSQL(ApplianceFlight);
                tb_ticket_policy.UnApplianceFlight = CommonManage.TrimSQL(UnApplianceFlight);
                tb_ticket_policy.ScheduleConstraints = "/" + ScheduleConstraints.Trim(new char[] { '/' }) + "/";
                tb_ticket_policy.ShippingSpace = "/" + ShippingSpace.Trim(new char[] { '/' }) + "/";
                tb_ticket_policy.SpacePrice = SpacePrice;
                tb_ticket_policy.ReferencePrice = ReferencePrice;
                tb_ticket_policy.AdvanceDay = AdvanceDay;
                tb_ticket_policy.DownPoint = DownPoint;
                tb_ticket_policy.LaterPoint = LaterPoint;
                tb_ticket_policy.SharePoint = SharePoint;
                tb_ticket_policy.AirReBate = AirReBate;
                tb_ticket_policy.DownReturnMoney = DownReturnMoney;
                tb_ticket_policy.LaterReturnMoney = LaterReturnMoney;
                tb_ticket_policy.SharePointReturnMoney = ShareReturnMoney;
                tb_ticket_policy.AirReBateReturnMoney = AirReBateReturnMoney;
                tb_ticket_policy.FlightStartDate = FlightStartDate;
                tb_ticket_policy.FlightEndDate = FlightEndDate;
                tb_ticket_policy.PrintStartDate = PrintStartDate;
                tb_ticket_policy.PrintEndDate = PrintEndDate;
                tb_ticket_policy.AuditDate = AuditDate;
                tb_ticket_policy.AuditType = AuditType;
                tb_ticket_policy.AuditLoginName = AuditLoginName;
                tb_ticket_policy.AuditName = AuditName;
                tb_ticket_policy.CreateLoginName = CreateLoginName;
                tb_ticket_policy.CreateName = CpyName;
                tb_ticket_policy.UpdateDate = UpdateDate;
                tb_ticket_policy.UpdateLoginName = UpdateLoginName;
                tb_ticket_policy.UpdateName = UpdateName;
                tb_ticket_policy.Remark = CommonManage.TrimSQL(Remark);
                //tb_ticket_policy.IsPause = IsPause;
                tb_ticket_policy.IsApplyToShareFlight = IsApplyToShareFlight;
                List<string> ShareAirCodelist = new List<string>();
                ShareAirCodelist.AddRange(ShareAirCode.Split(new string[] { "/" }, StringSplitOptions.RemoveEmptyEntries));
                tb_ticket_policy.ShareAirCode = CommonManage.TrimSQL("/" + string.Join("/", ShareAirCodelist.ToArray()) + "/");
                tb_ticket_policy.IsLowerOpen = IsLowerOpen;
                tb_ticket_policy.HighPolicyFlag = HighPolicyFlag;
                tb_ticket_policy.AutoPrintFlag = AutoPrintFlag;
                tb_ticket_policy.GroupId = GroupId;
                tb_ticket_policy.A1 = A1;
                tb_ticket_policy.A2 = A2;
                tb_ticket_policy.A3 = A3;
                tb_ticket_policy.A4 = A4;
                tb_ticket_policy.A5 = A5;
                tb_ticket_policy.A6 = A6;
                tb_ticket_policy.A7 = A7;
                tb_ticket_policy.A8 = A8;
                tb_ticket_policy.A9 = A9;
                tb_ticket_policy.A10 = A10;
                tb_ticket_policy.A11 = A11;
                tb_ticket_policy.A12 = A12;
                tb_ticket_policy.A13 = A13;
                tb_ticket_policy.A14 = A14;
                tb_ticket_policy.A15 = A15;
                tb_ticket_policy.A16 = A16;

                //默认政策处理
                if (A1 != 0)
                {
                    tb_ticket_policy.StartCityNameCode = "/ALL/";
                    tb_ticket_policy.MiddleCityNameCode = "/ALL/";
                    tb_ticket_policy.TargetCityNameCode = "/ALL/";
                    tb_ticket_policy.ShippingSpace = "/ALL/";
                }

                #endregion
            }

            //操作内容日志
            StringBuilder sbOpLog = new StringBuilder();
            sbOpLog.Append("请求页面:" + PageName + "|");
            //日志字段间分隔符
            string SplitChar = "###";
            if (OpType == "0")
            {
                //添加
                IsSuc = (bool)this.baseDataManage.CallMethod("Tb_Ticket_Policy", "Insert", null, new object[] { tb_ticket_policy });
                string InsertLog = this.baseDataManage.CallMethod("Tb_Ticket_Policy", "ModelToString", null, new object[] { tb_ticket_policy, SplitChar }).ToString();
                sbOpLog.AppendFormat("添加政策|{0}", InsertLog);
            }
            else if (OpType == "1" || OpType == "4")
            {
                if (OpType == "1")
                {
                    //修改根据单个id修改
                    IsSuc = (bool)this.baseDataManage.CallMethod("Tb_Ticket_Policy", "Update", null, new object[] { tb_ticket_policy });
                    //修改部分字符串显示
                    string UpdateLog = this.baseDataManage.CallMethod("Tb_Ticket_Policy", "ChangeDataToString", null, new object[] { tb_ticket_policy, Temp_tb_ticket_policy, SplitChar }).ToString();
                    sbOpLog.AppendFormat("修改政策|{0}", UpdateLog);
                }
                else if (OpType == "4")
                {
                    if (Temp_tb_ticket_policy != null)
                    {
                        //复制
                        IsSuc = (bool)this.baseDataManage.CallMethod("Tb_Ticket_Policy", "Insert", null, new object[] { Temp_tb_ticket_policy });
                        string InsertLog = this.baseDataManage.CallMethod("Tb_Ticket_Policy", "ModelToString", null, new object[] { Temp_tb_ticket_policy, SplitChar }).ToString();
                        sbOpLog.AppendFormat("添加政策(复制)|{0}", InsertLog);
                    }
                }
            }
            else if (OpType == "5")//审核
            {
                IdsList.AddRange(Id.Replace("\'", "").Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries));
                IsSuc = (bool)this.baseDataManage.CallMethod("Tb_Ticket_Policy", "UpdateByIds", null, new object[] { IdsList, " AuditType=1 " });
                sbOpLog.AppendFormat("政策审核|id:{0}{1}{2}", Id, SplitChar, "AuditType=1 ");
            }
            else if (OpType == "6")//未审核
            {
                IdsList.AddRange(Id.Replace("\'", "").Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries));
                IsSuc = (bool)this.baseDataManage.CallMethod("Tb_Ticket_Policy", "UpdateByIds", null, new object[] { IdsList, " AuditType=2 " });
                sbOpLog.AppendFormat("政策未审核|id:{0}{1}{2}", Id, SplitChar, "AuditType=2 ");
            }
            if (OpType == "2")
            {
                List<Tb_Ticket_Policy> delOutList = new List<Tb_Ticket_Policy>();
                IHashObject OutParams = new HashObject();
                OutParams.Add("2", "out");
                //删除  根据id集合删除
                IsSuc = (bool)this.baseDataManage.CallMethod("Tb_Ticket_Policy", "DeleteByIds", OutParams, new object[] { IdsList, delOutList });
                //日志
                delOutList = OutParams.GetValue<List<Tb_Ticket_Policy>>("2");
                int delCount = 0;
                foreach (Tb_Ticket_Policy item in delOutList)
                {
                    string DelLog = this.baseDataManage.CallMethod("Tb_Ticket_Policy", "ModelToString", null, new object[] { item, SplitChar }).ToString();
                    sbOpLog.Append("请求页面:" + PageName + "|");
                    sbOpLog.AppendFormat("删除政策|{0}", DelLog);
                    //记录除了查询之外的操作日志
                    sbOpLog = new StringBuilder();
                    Log_Operation log_operation = new Log_Operation();
                    log_operation.ModuleName = "本地政策";
                    log_operation.OperateType = "政策" + GetOpType(OpType);
                    log_operation.CpyNo = CpyNo;
                    log_operation.LoginName = CreateLoginName;
                    log_operation.UserName = CpyName;
                    log_operation.OrderId = "";
                    log_operation.CreateTime = System.DateTime.Now;
                    log_operation.OptContent = sbOpLog.ToString();
                    log_operation.ClientIP = this.ClientIP;
                    //记录日志
                    if (AddOperationLog(log_operation))
                    {
                        delCount++;
                    }
                }
                if (delCount == delOutList.Count)
                {
                    IsSuc = true;
                }
                else
                {
                    errMsg = "删除政策添加日志出错";
                    IsSuc = false;
                }
            }
            else
            {
                //记录除了查询之外的操作日志
                Log_Operation log_operation = new Log_Operation();
                log_operation.ModuleName = "本地政策";
                log_operation.OperateType = "政策" + GetOpType(OpType);
                log_operation.CpyNo = CpyNo;
                log_operation.LoginName = CreateLoginName;
                log_operation.UserName = CpyName;
                log_operation.OrderId = "";
                log_operation.CreateTime = System.DateTime.Now;
                log_operation.OptContent = sbOpLog.ToString();
                log_operation.ClientIP = this.ClientIP;
                //记录日志
                //记录日志
                if (AddOperationLog(log_operation))
                {
                    IsSuc = true;
                }
                else
                {
                    IsSuc = false;
                    errMsg = "政策添加日志出错";
                }
            }
        }
        catch (Exception ex)
        {
            Log_Error log = new Log_Error();
            log.CpyNo = CpyNo;//公司编号
            log.ErorrTime = System.DateTime.Now;
            log.Page = "CommonAjAx.ashx";
            log.LoginName = CreateLoginName;//当前登录用户账号
            log.ClientIP = this.ClientIP;
            log.Method = "AddAndUpdatePolicy()";
            log.DevName = "邓积远";
            log.ErrorContent = ex.Source.ToString() + ex.StackTrace.ToString() + ex.Message + ex.InnerException;
            //记录日志
            AddErrorLog(log);
            errMsg = "政策操作出错:" + ex.Message;
            IsSuc = false;
        }
        finally
        {

        }
        return IsSuc;
    }
    /// <summary>
    /// 获取所有城市信息
    /// </summary>
    /// <param name="IsDomestic"></param>
    /// <returns></returns>
    public List<Bd_Air_AirPort> GetCityList(string IsDomestic)
    {
        return this.baseDataManage.CallMethod("Bd_Air_AirPort", "GetList", null, new object[] { "IsDomestic=" + IsDomestic }) as List<Bd_Air_AirPort>;
    }
    private List<Bd_Air_AirPort> list = null;
    /// <summary>
    /// 获取城市信息
    /// </summary>
    /// <param name="city"></param>
    /// <returns></returns>
    public Bd_Air_AirPort GetCity(string city)
    {
        if (list == null)
        {
            list = GetCityList("1");
        }
        Bd_Air_AirPort reModel = list.Find(delegate(Bd_Air_AirPort bd_air_airport)
        {
            if (bd_air_airport.CityName.ToUpper() == city.Trim().ToUpper() || bd_air_airport.CityCodeWord == city.Trim().ToUpper())
            {
                return true;
            }
            else
            {
                return false;
            }
        });
        return reModel;
    }
    /// <summary>
    /// 散冲团政策
    /// </summary>
    /// <param name="errCode">错误代码编号</param>
    /// <param name="errMsg">内部错误信息</param>
    /// <param name="errMsg">查询返回结果</param>
    /// <param name="errMsg">操作页面</param>
    /// <param name="OpType"> 0添加 1修改 2删除 3查询</param>
    /// <returns></returns>
    public bool GroupPolicy(out string errCode, out string errMsg, out string result, string PageName, string OpType)
    {
        errCode = "";//用代码表示
        errMsg = "";//用错误描述表示
        result = "";//用于查询返回结果
        bool Isdefault = false;
        string CpyNo = GetVal("CpyNo", mCompany.UninCode, out Isdefault);
        string CpyName = GetVal("CpyName", mCompany.UninAllName, out Isdefault);
        string LoginName = GetVal("LoginName", mUser.LoginName, out Isdefault);
        string UserName = GetVal("UserName", mUser.UserName, out Isdefault);
        string Id = GetVal("Id", "0", out Isdefault);
        Tb_Ticket_UGroupPolicy tb_ticket_ugrouppolicy = null;
        Tb_Ticket_UGroupPolicy tb_ticket_ugrouppolicy_Copy = null;
        List<string> IdsList = new List<string>();
        //操作成功 还是失败
        bool IsSuc = false;
        try
        {
            if (OpType == "0")
            {
                //添加
                tb_ticket_ugrouppolicy = new Tb_Ticket_UGroupPolicy();
            }
            else if (OpType == "1" || OpType == "4")
            {
                //修改
                tb_ticket_ugrouppolicy = this.baseDataManage.CallMethod("Tb_Ticket_UGroupPolicy", "GetById", null, new object[] { Id }) as Tb_Ticket_UGroupPolicy;
                //拷贝一个新实例 用于比较数据记日志
                tb_ticket_ugrouppolicy_Copy = this.baseDataManage.CallMethod("Tb_Ticket_UGroupPolicy", "CopyModel", null, new object[] { tb_ticket_ugrouppolicy }) as Tb_Ticket_UGroupPolicy;
            }
            else if (OpType == "2")
            {
                //根据id集合删除
                IdsList.AddRange(Id.Replace("\'", "").Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries));
            }
            else
            {
                //查询
            }
            //操作内容日志
            StringBuilder sbOpLog = new StringBuilder();
            //日志字段间分隔符
            string SplitChar = "###";
            if (tb_ticket_ugrouppolicy != null)
            {
                string AirCode = GetVal("AirCode", OpType == "1" ? tb_ticket_ugrouppolicy.AirCode : "", out Isdefault);
                string FromCityCode = GetVal("FromCode", OpType == "1" ? tb_ticket_ugrouppolicy.FromCityCode : "", out Isdefault);
                string ToCityCode = GetVal("ToCode", OpType == "1" ? tb_ticket_ugrouppolicy.ToCityCode : "", out Isdefault);
                string FlightNo = GetVal("FlightNo", OpType == "1" ? tb_ticket_ugrouppolicy.FlightNo : "", out Isdefault);
                string Class = GetVal("Class", OpType == "1" ? tb_ticket_ugrouppolicy.Class : "", out Isdefault);
                string FlightTime = GetVal("FlightTime", OpType == "1" ? tb_ticket_ugrouppolicy.FlightTime : "00:00:00-00:00:00", out Isdefault);
                string PlaneType = GetVal("PlaneType", OpType == "1" ? tb_ticket_ugrouppolicy.PlaneType : "", out Isdefault);
                int PolicyType = 1;
                string _PolicyType = GetVal("PolicyType", OpType == "1" ? tb_ticket_ugrouppolicy.PolicyType.ToString() : "1", out Isdefault);
                int.TryParse(_PolicyType, out PolicyType);

                string FlightStartDate = GetVal("FlightStartDate", OpType == "1" ? tb_ticket_ugrouppolicy.FlightStartDate.ToString("yyyy-MM-dd HH:mm") : "1901-01-01 00:00", out Isdefault);
                string FlightEndDate = GetVal("FlightEndDate", OpType == "1" ? tb_ticket_ugrouppolicy.FlightEndDate.ToString("yyyy-MM-dd HH:mm") : "1901-01-01 00:00", out Isdefault);
                string PrintStartDate = GetVal("PrintStartDate", OpType == "1" ? tb_ticket_ugrouppolicy.PrintStartDate.ToString("yyyy-MM-dd") : "1901-01-01", out Isdefault);
                string PrintEndDate = GetVal("PrintEndDate", OpType == "1" ? tb_ticket_ugrouppolicy.PrintEndDate.ToString("yyyy-MM-dd") : "1901-01-01", out Isdefault);
                int AdvanceDays = 0;
                string _AdvanceDays = GetVal("AdvanceDays", OpType == "1" ? tb_ticket_ugrouppolicy.AdvanceDays.ToString() : "0", out Isdefault);
                int.TryParse(_AdvanceDays, out AdvanceDays);

                int SeatCount = 0;
                string _SeatCount = GetVal("SeatCount", OpType == "1" ? tb_ticket_ugrouppolicy.SeatCount.ToString() : "0", out Isdefault);
                int.TryParse(_SeatCount, out SeatCount);

                decimal _Prices = 0m;
                string Prices = GetVal("Prices", OpType == "1" ? tb_ticket_ugrouppolicy.Prices.ToString() : "0", out Isdefault);
                decimal.TryParse(Prices, out _Prices);

                decimal _Rebate = 0m;
                string Rebate = GetVal("Rebate", OpType == "1" ? tb_ticket_ugrouppolicy.Rebate.ToString() : "0", out Isdefault);
                decimal.TryParse(Rebate, out _Rebate);

                bool PriceType = false;
                string _PriceType = GetVal("PriceType", OpType == "1" ? (tb_ticket_ugrouppolicy.PriceType ? "true" : "false") : "false", out Isdefault);
                bool.TryParse(_PriceType.ToLower() == "true" || _PriceType == "1" ? "true" : "false", out PriceType);

                decimal _OilPrice = 0m;
                string OilPrice = GetVal("OilPrice", OpType == "1" ? tb_ticket_ugrouppolicy.OilPrice.ToString() : "0", out Isdefault);
                decimal.TryParse(OilPrice, out _OilPrice);

                decimal _BuildPrice = 0m;
                string BuildPrice = GetVal("BuildPrice", OpType == "1" ? tb_ticket_ugrouppolicy.BuildPrice.ToString() : "0", out Isdefault);
                decimal.TryParse(BuildPrice, out _BuildPrice);


                decimal _AirRebate = 0m;
                string AirRebate = GetVal("AirRebate", OpType == "1" ? tb_ticket_ugrouppolicy.AirRebate.ToString() : "0", out Isdefault);
                decimal.TryParse(AirRebate, out _AirRebate);

                decimal _DownRebate = 0m;
                string DownRebate = GetVal("DownRebate", OpType == "1" ? tb_ticket_ugrouppolicy.DownRebate.ToString() : "0", out Isdefault);
                decimal.TryParse(DownRebate, out _DownRebate);


                decimal _LaterRebate = 0m;
                string LaterRebate = GetVal("LaterRebate", OpType == "1" ? tb_ticket_ugrouppolicy.LaterRebate.ToString() : "0", out Isdefault);
                decimal.TryParse(LaterRebate, out _LaterRebate);


                decimal _ShareRebate = 0m;
                string ShareRebate = GetVal("ShareRebate", OpType == "1" ? tb_ticket_ugrouppolicy.ShareRebate.ToString() : "0", out Isdefault);
                decimal.TryParse(ShareRebate, out _ShareRebate);

                string OfficeCode = GetVal("OfficeCode", OpType == "1" ? tb_ticket_ugrouppolicy.OfficeCode.ToString() : "", out Isdefault);
                string Remarks = GetVal("Remarks", OpType == "1" ? tb_ticket_ugrouppolicy.Remarks.ToString() : "", out Isdefault);


                tb_ticket_ugrouppolicy.CpyNo = CpyNo;
                tb_ticket_ugrouppolicy.CpyName = CpyName;
                tb_ticket_ugrouppolicy.CpyType = mCompany != null ? mCompany.RoleType : 0;
                tb_ticket_ugrouppolicy.OperTime = System.DateTime.Now;
                tb_ticket_ugrouppolicy.OperLoginName = LoginName;
                tb_ticket_ugrouppolicy.OperUserName = UserName;
                tb_ticket_ugrouppolicy.AirCode = AirCode;
                Bd_Air_AirPort CityInfo = GetCity(FromCityCode);
                tb_ticket_ugrouppolicy.FromCityCode = FromCityCode;
                tb_ticket_ugrouppolicy.FromCityName = CityInfo != null ? CityInfo.CityName : "";
                CityInfo = GetCity(ToCityCode);
                tb_ticket_ugrouppolicy.ToCityCode = ToCityCode;
                tb_ticket_ugrouppolicy.ToCityName = CityInfo != null ? CityInfo.CityName : "";
                //航班号
                tb_ticket_ugrouppolicy.FlightNo = CommonManage.TrimSQL(FlightNo);
                //舱位
                tb_ticket_ugrouppolicy.Class = Class;
                //起抵时间
                tb_ticket_ugrouppolicy.FlightTime = FlightTime;
                //机型
                tb_ticket_ugrouppolicy.PlaneType = PlaneType;
                //政策类型
                tb_ticket_ugrouppolicy.PolicyType = PolicyType;
                //适用日期
                tb_ticket_ugrouppolicy.FlightStartDate = DateTime.Parse(FlightStartDate + ":00");
                tb_ticket_ugrouppolicy.FlightEndDate = DateTime.Parse(FlightEndDate + ":00");
                //出票日期
                tb_ticket_ugrouppolicy.PrintStartDate = DateTime.Parse(PrintStartDate + " 00:00:00");
                tb_ticket_ugrouppolicy.PrintEndDate = DateTime.Parse(PrintEndDate + " 23:59:59");
                tb_ticket_ugrouppolicy.AdvanceDays = AdvanceDays;
                tb_ticket_ugrouppolicy.SeatCount = SeatCount;
                tb_ticket_ugrouppolicy.Prices = _Prices;
                tb_ticket_ugrouppolicy.Rebate = _Rebate;
                tb_ticket_ugrouppolicy.PriceType = PriceType;
                tb_ticket_ugrouppolicy.OilPrice = _OilPrice;
                tb_ticket_ugrouppolicy.BuildPrice = _BuildPrice;
                tb_ticket_ugrouppolicy.AirRebate = _AirRebate;
                tb_ticket_ugrouppolicy.DownRebate = _DownRebate;
                tb_ticket_ugrouppolicy.LaterRebate = _LaterRebate;
                tb_ticket_ugrouppolicy.ShareRebate = _ShareRebate;
                tb_ticket_ugrouppolicy.OfficeCode = OfficeCode;
                tb_ticket_ugrouppolicy.Remarks = CommonManage.TrimSQL(Remarks);
            }
            if (OpType == "0")
            {
                //添加
                IsSuc = (bool)this.baseDataManage.CallMethod("Tb_Ticket_UGroupPolicy", "Insert", null, new object[] { tb_ticket_ugrouppolicy });
                string InsertLog = this.baseDataManage.CallMethod("Tb_Ticket_UGroupPolicy", "ModelToString", null, new object[] { tb_ticket_ugrouppolicy, SplitChar }).ToString();
                sbOpLog.AppendFormat("添加散冲团政策|{0}", InsertLog);
            }
            else if (OpType == "1" || OpType == "4")
            {
                if (OpType == "1")
                {
                    //修改根据单个id修改
                    IsSuc = (bool)this.baseDataManage.CallMethod("Tb_Ticket_UGroupPolicy", "Update", null, new object[] { tb_ticket_ugrouppolicy });
                    //修改部分字符串显示
                    string UpdateLog = this.baseDataManage.CallMethod("Tb_Ticket_UGroupPolicy", "ChangeDataToString", null, new object[] { tb_ticket_ugrouppolicy, tb_ticket_ugrouppolicy_Copy, SplitChar }).ToString();
                    sbOpLog.AppendFormat("修改散冲团政策|{0}", UpdateLog);
                }
                else if (OpType == "4")
                {
                    if (tb_ticket_ugrouppolicy_Copy != null)
                    {
                        //复制
                        IsSuc = (bool)this.baseDataManage.CallMethod("Tb_Ticket_UGroupPolicy", "Insert", null, new object[] { tb_ticket_ugrouppolicy_Copy });
                        string InsertLog = this.baseDataManage.CallMethod("Tb_Ticket_UGroupPolicy", "ModelToString", null, new object[] { tb_ticket_ugrouppolicy_Copy, SplitChar }).ToString();
                        sbOpLog.AppendFormat("添加散冲团政策(复制)|{0}", InsertLog);
                    }
                }
            }
            if (OpType == "2")
            {
                List<Tb_Ticket_UGroupPolicy> delOutList = new List<Tb_Ticket_UGroupPolicy>();
                IHashObject OutParams = new HashObject();
                OutParams.Add("2", "out");
                //删除  根据id集合删除
                IsSuc = (bool)this.baseDataManage.CallMethod("Tb_Ticket_UGroupPolicy", "DeleteByIds", OutParams, new object[] { IdsList, delOutList });
                //日志
                delOutList = OutParams.GetValue<List<Tb_Ticket_UGroupPolicy>>("2");
                int delCount = 0;
                foreach (Tb_Ticket_UGroupPolicy item in delOutList)
                {
                    string DelLog = this.baseDataManage.CallMethod("Tb_Ticket_UGroupPolicy", "ModelToString", null, new object[] { item, SplitChar }).ToString();
                    sbOpLog.AppendFormat("删除散冲团政策|{0}", DelLog);
                    //记录除了查询之外的操作日志
                    sbOpLog = new StringBuilder();
                    Log_Operation log_operation = new Log_Operation();
                    log_operation.ModuleName = "本地政策";
                    log_operation.OperateType = "政策" + GetOpType(OpType);
                    log_operation.CpyNo = CpyNo;
                    log_operation.LoginName = LoginName;
                    log_operation.UserName = CpyName;
                    log_operation.OrderId = "";
                    log_operation.CreateTime = System.DateTime.Now;
                    log_operation.OptContent = sbOpLog.ToString();
                    log_operation.ClientIP = this.ClientIP;
                    //记录日志
                    if (AddOperationLog(log_operation))
                    {
                        delCount++;
                    }
                }
                if (delCount == delOutList.Count)
                {
                    IsSuc = true;
                }
                else
                {
                    errMsg = "删除散冲团政策添加日志出错";
                    IsSuc = false;
                }
            }
            else
            {
                //记录除了查询之外的操作日志
                Log_Operation log_operation = new Log_Operation();
                log_operation.ModuleName = "本地政策";
                log_operation.OperateType = "政策" + GetOpType(OpType);
                log_operation.CpyNo = CpyNo;
                log_operation.LoginName = LoginName;
                log_operation.UserName = CpyName;
                log_operation.OrderId = "";
                log_operation.CreateTime = System.DateTime.Now;
                log_operation.OptContent = sbOpLog.ToString();
                log_operation.ClientIP = this.ClientIP;
                //记录日志
                //记录日志
                if (AddOperationLog(log_operation))
                {
                    IsSuc = true;
                }
                else
                {
                    IsSuc = false;
                    errMsg = "散冲团政策添加日志出错";
                }
            }
        }
        catch (Exception ex)
        {
            Log_Error log = new Log_Error();
            log.CpyNo = CpyNo;//公司编号
            log.ErorrTime = System.DateTime.Now;
            log.Page = "CommonAjAx.ashx";
            log.LoginName = LoginName;//当前登录用户账号
            log.ClientIP = this.ClientIP;
            log.Method = "GroupPolicy()";
            log.DevName = "邓积远";
            log.ErrorContent = ex.Source.ToString() + ex.StackTrace.ToString() + ex.Message + ex.InnerException;
            //记录日志
            AddErrorLog(log);
            errMsg = "散冲团政策操作出错:" + ex.Message;
            IsSuc = false;
        }
        return IsSuc;
    }
    #region 日志

    /// <summary>
    /// 记录操作日志
    /// </summary>
    /// <param name="Log"></param>
    /// <returns></returns>
    public bool AddOperationLog(Log_Operation Log)
    {
        bool IsSuc = false;
        try
        {
            IsSuc = (bool)this.baseDataManage.CallMethod("Log_Operation", "Insert", null, new object[] { Log });
        }
        catch (Exception ex)
        {
            Log_Error log = new Log_Error();
            log.CpyNo = Log.CpyNo;
            log.ErorrTime = System.DateTime.Now;
            log.Page = "CommonAjAx.ashx";
            log.LoginName = Log.LoginName;
            log.ClientIP = this.ClientIP;
            log.Method = "AddOperationLog()";
            log.DevName = "邓积远";
            log.ErrorContent = ex.Source.ToString() + ex.StackTrace.ToString() + ex.Message;
            //记录日志
            AddErrorLog(log);
            IsSuc = false;
        }
        return IsSuc;
    }
    /// <summary>
    /// 记录错误日志
    /// </summary>
    /// <param name="Error"></param>
    /// <returns></returns>
    public bool AddErrorLog(Log_Error Error)
    {
        bool IsSuc = false;
        try
        {
            IsSuc = (bool)this.baseDataManage.CallMethod("Log_Error", "Insert", null, new object[] { Error });
        }
        catch (Exception ex)
        {
            IsSuc = false;
            StringBuilder errLog = new StringBuilder();
            errLog.Append(System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "记录日志开始>===============================================================================================\r\n");
            errLog.AppendFormat(string.Format("【错误时间:{0}\t 公司编号:{1}页面:{2}\t账号:{3}\t错误方法:AddErrorLog\t客户登陆IP:{4}\t开发人员名称：{5}】\r\n内容描述：\r\n{6}\r\n\r\n",
            Error.ErorrTime, Error.CpyNo, Error.Page, Error.LoginName, Error.ClientIP, Error.DevName, "异常信息:" + Error.ErrorContent + " 记录数据库：" + ex.Source.ToString() + "|" + ex.StackTrace.ToString() + "|" + ex.Message
            ));
            errLog.Append("记录日志结束<===============================================================================================\r\n");
            //记录文本日志
            WriteTextLog("CommonAjAx", errLog.ToString()
            );
        }
        return IsSuc;
    }

    /// <summary>
    /// 记录文本日志
    /// </summary>
    /// <param name="DirName"></param>
    /// <param name="Content"></param>
    /// <returns></returns>
    public bool WriteTextLog(string DirName, string Content)
    {
        bool result = false;
        try
        {
            if (string.IsNullOrEmpty(Content))
            {
                Content = "";
            }
            //日志根目录
            string CurPath = System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "Log";
            if (!string.IsNullOrEmpty(DirName) && DirName.Trim(new char[] { '\\', '/', ' ' }) == "")
            {
                CurPath = CurPath + "\\" + DirName.Trim(new char[] { '\\', '/', ' ' }) + "\\";
            }
            //不存在该目录就创建一个
            if (!Directory.Exists(CurPath))
            {
                Directory.CreateDirectory(CurPath);
            }
            string FullName = CurPath + "\\" + System.DateTime.Now.ToString("yyyy_MM_dd_HH_mm") + ".txt";
            StreamWriter sw = File.AppendText(FullName);
            sw.Write(Content);
            sw.Close();
            result = true;
        }
        catch (Exception)
        {
            result = false;
        }
        return result;
    }
    #endregion



    #region 线下订单
    public bool CreateLineOrder(out string errCode, out string errMsg, out string result, string PageName, string OpType)
    {
        bool Isdefault = false;
        bool IsSuc = false;
        errCode = "-1";
        errMsg = "";
        result = "";
        string AirCode = GetVal("airCode", "", out Isdefault);
        string AirName = GetVal("airName", "", out Isdefault);
        string Remark = GetVal("Remark", "", out Isdefault);
        string JJFare = GetVal("JJFare", "", out Isdefault);
        string strPasStr = GetVal("PasStr", "", out Isdefault);//乘机人字符串
        string strSkyStr = GetVal("SkyStr", "", out Isdefault);//航段字符串
        string LinkMan = GetVal("linkMan", "", out Isdefault);
        string LinkManPhone = GetVal("linkPhone", "", out Isdefault);
        string CPCpyNo = GetVal("CPCpyNo", "", out Isdefault);//默认出票公司编号
        string CpyNo = GetVal("UninCode", "", out Isdefault);//申请订单的公司编号
        string strPnr = GetVal("Pnr", "", out Isdefault);//PNR或者PNR内容解析出来的PNR
        string strOrderfangShi = GetVal("OrderfangShi", "", out Isdefault);//录入方式 0手动 1pnr 2pnr内容
        //出票Office
        string PrintOffice = GetPrintOffice(CpyNo, AirCode);
        //订单提示信息
        StringBuilder sbOrderInfo = new StringBuilder();
        //线下订单文本日志
        StringBuilder sbLogText = new StringBuilder();
        try
        {
            //订单管理
            Tb_Ticket_OrderBLL OrderManage = new Tb_Ticket_OrderBLL();
            OrderInputParam OrderParam = new OrderInputParam();
            OrderParam.PnrInfo = new PnrAnalysis.Model.RePnrObj();
            sbLogText.Append("时间:" + System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " 线下订单航段信息:\r\n" + strSkyStr + "\r\n");
            sbLogText.Append("乘客信息:\r\n" + strPasStr + "\r\n");
            //构造实体
            List<Tb_Ticket_SkyWay> skyList = GetSkyWayList(strSkyStr);
            int AdultNum = 0, ChdNum = 0, INFNum = 0;
            List<List<Tb_Ticket_Passenger>> pasList = GetPasList(strPasStr, out AdultNum, out ChdNum, out INFNum);
            sbOrderInfo.AppendFormat("共有{0}位成人,{1}位儿童,{2}位婴儿<br />", AdultNum, ChdNum, INFNum);
            if (pasList.Count == 2 && pasList[0].Count > 0 && pasList[1].Count > 0)
            {
                //成人儿童两个订单
                //第一个订单
                OrderMustParamModel ParamModel0 = new OrderMustParamModel();
                Tb_Ticket_Order AdultOrder = GetOrder(CPCpyNo, LinkMan, LinkManPhone, Remark, pasList[0], skyList);

                AdultOrder.PrintOffice = PrintOffice;
                Log_Tb_AirOrder logOrder0 = new Log_Tb_AirOrder();
                logOrder0.OperTime = DateTime.Now;
                logOrder0.OperType = "创建订单";
                logOrder0.OperContent = mUser.LoginName + "于" + logOrder0.OperTime + "申请线下订单。";
                logOrder0.OperLoginName = mUser.LoginName;
                logOrder0.OperUserName = mUser.UserName;
                logOrder0.CpyNo = mCompany.UninCode;
                logOrder0.CpyName = mCompany.UninName;
                logOrder0.CpyType = mCompany.RoleType;
                //赋值
                ParamModel0.Order = AdultOrder;
                ParamModel0.PasList = pasList[0];
                ParamModel0.SkyList = skyList;
                ParamModel0.LogOrder = logOrder0;
                OrderParam.OrderParamModel.Add(ParamModel0);

                //第二个订单
                OrderMustParamModel ParamModel1 = new OrderMustParamModel();
                Tb_Ticket_Order ChildOrder = GetOrder(CPCpyNo, LinkMan, LinkManPhone, Remark, pasList[1], skyList);
                ChildOrder.PrintOffice = PrintOffice;
                Log_Tb_AirOrder logOrder1 = new Log_Tb_AirOrder();
                logOrder1.OperTime = DateTime.Now;
                logOrder1.OperType = "创建订单";
                logOrder1.OperContent = mUser.LoginName + "于" + logOrder0.OperTime + "申请线下儿童订单。";
                logOrder1.OperLoginName = mUser.LoginName;
                logOrder1.OperUserName = mUser.UserName;
                logOrder1.CpyNo = mCompany.UninCode;
                logOrder1.CpyName = mCompany.UninName;
                logOrder1.CpyType = mCompany.RoleType;
                //赋值
                ParamModel1.Order = ChildOrder;
                ParamModel1.PasList = pasList[1];
                ParamModel1.SkyList = skyList;
                ParamModel1.LogOrder = logOrder1;
                OrderParam.OrderParamModel.Add(ParamModel1);
            }
            else
            {
                OrderMustParamModel ParamModel = new OrderMustParamModel();
                bool IsCHD = false;
                //一个订单
                if (pasList[0].Count > 0)
                {
                    //成人订单               
                    Tb_Ticket_Order AdultOrder = GetOrder(CPCpyNo, LinkMan, LinkManPhone, Remark, pasList[0], skyList);
                    AdultOrder.PrintOffice = PrintOffice;
                    ParamModel.Order = AdultOrder;
                    ParamModel.PasList = pasList[0];

                    if (strOrderfangShi.Trim() != "0")
                    {
                        OrderParam.PnrInfo.AdultPnr = strPnr;
                    }
                }
                else
                {
                    if (pasList[1].Count > 0)
                    {
                        //儿童订单
                        IsCHD = true;
                        Tb_Ticket_Order ChildOrder = GetOrder(CPCpyNo, LinkMan, LinkManPhone, Remark, pasList[1], skyList);
                        ChildOrder.PrintOffice = PrintOffice;
                        ParamModel.Order = ChildOrder;
                        ParamModel.PasList = pasList[1];
                        if (strOrderfangShi.Trim() != "0")
                        {
                            OrderParam.PnrInfo.childPnr = strPnr;
                        }
                    }
                }
                Log_Tb_AirOrder logOrder = new Log_Tb_AirOrder();
                logOrder.OperTime = DateTime.Now;
                logOrder.OperType = "创建订单";
                logOrder.OperContent = mUser.LoginName + "于" + logOrder.OperTime + "申请线下" + (IsCHD ? "儿童" : "") + "订单。";
                logOrder.OperLoginName = mUser.LoginName;
                logOrder.OperUserName = mUser.UserName;
                logOrder.CpyNo = mCompany.UninCode;
                logOrder.CpyName = mCompany.UninName;
                logOrder.CpyType = mCompany.RoleType;
                ParamModel.SkyList = skyList;
                ParamModel.LogOrder = logOrder;
                OrderParam.OrderParamModel.Add(ParamModel);
            }
            string ErrMsg = "";
            //生成订单      
            IsSuc = OrderManage.CreateOrder(ref OrderParam, out ErrMsg);
            if (IsSuc)
            {
                foreach (OrderMustParamModel item in OrderParam.OrderParamModel)
                {
                    if (item.Order.IsChdFlag)
                    {
                        sbOrderInfo.Append("儿童订单号：" + item.Order.OrderId + "<br />");
                    }
                    else
                    {
                        sbOrderInfo.Append("成人订单号：" + item.Order.OrderId + "<br />");
                    }
                }
                errCode = "1";
                result = sbOrderInfo.ToString();
                sbLogText.Append("结果信息：" + result);
            }
            else
            {
                errMsg = ErrMsg;
                sbLogText.Append("结果信息：" + errMsg);
            }
        }
        catch (Exception ex)
        {
            errMsg = errMsg + ex.Message;
            PnrAnalysis.LogText.LogWrite(ex.Message, "CommonAjAx\\CreateLineOrder");
            sbLogText.Append("异常信息：" + errMsg);
        }
        finally
        {
            PnrAnalysis.LogText.LogWrite(sbLogText.ToString() + "\r\n\r\n", "CommonAjAx\\CreateLineOrderText");
        }
        return IsSuc;
    }
    /// <summary>
    /// 获取Y舱价格
    /// </summary>
    /// <param name="sqlWhere"></param>
    /// <param name="CarryCode"></param>
    /// <returns></returns>
    public Bd_Air_Fares GetYFares(string sqlWhere, string CarryCode)
    {
        List<Bd_Air_Fares> yList = this.baseDataManage.CallMethod("Bd_Air_Fares", "GetList", null, new object[] { sqlWhere }) as List<Bd_Air_Fares>;
        Bd_Air_Fares fare_CarryCode = null;
        if (yList != null && yList.Count > 0)
        {
            fare_CarryCode = yList.Find(delegate(Bd_Air_Fares _fare)
            {
                return _fare.CarryCode.ToUpper() == CarryCode.ToUpper();
            });
            if (fare_CarryCode == null)
            {
                fare_CarryCode = yList[0];
            }
        }
        return fare_CarryCode;
    }
    /// <summary>
    /// 构造航段信息
    /// </summary>
    /// <param name="strSkyWay"></param>
    /// <returns></returns>
    public List<Tb_Ticket_SkyWay> GetSkyWayList(string strSkyWay)
    {
        List<Tb_Ticket_SkyWay> listSky = new List<Tb_Ticket_SkyWay>();
        PbProject.Logic.Buy.AirQurey airqurey = new PbProject.Logic.Buy.AirQurey();
        string[] strArr = strSkyWay.Split(new string[] { "@" }, StringSplitOptions.RemoveEmptyEntries);
        DateTime fromdate = DateTime.Now;
        DateTime enddate = DateTime.Now;
        decimal _ABFee = 0m;
        decimal SkySeatPrice = -1;
        string sqlWhere = "";
        int i = 0;
        foreach (string strSky in strArr)
        {
            _ABFee = 0m;
            //航段序号(0)#####起飞日期(1)#####到达日期(2)#####起飞城市(3)#####起飞城市三字码(4)#####到达城市(5)#####到达城市三字码(6)#####承运人(7)#####承运人代码(8)#####航班号(9)#####舱位(10)#####机型(11)#####基建(12)
            string[] skyArr = strSky.Split(new string[] { "#####" }, StringSplitOptions.None);
            if (skyArr != null && skyArr.Length == 13)
            {
                Tb_Ticket_SkyWay sky = new Tb_Ticket_SkyWay();
                DateTime.TryParse(skyArr[1] + ":00", out fromdate);
                sky.FromDate = fromdate;
                DateTime.TryParse(skyArr[2] + ":00", out enddate);
                sky.ToDate = enddate;

                sky.FromCityName = skyArr[3];
                sky.FromCityCode = skyArr[4];
                sky.ToCityName = skyArr[5];
                sky.ToCityCode = skyArr[6];
                sky.CarryName = skyArr[7];
                sky.CarryCode = skyArr[8];
                sky.FlightCode = skyArr[9];
                sky.Space = skyArr[10];
                sky.Aircraft = skyArr[11];
                decimal.TryParse(string.IsNullOrEmpty(skyArr[12]) ? "0" : skyArr[12], out _ABFee);
                sqlWhere = string.Format("FromCityCode='{0}' and ToCityCode='{1}' and (CarryCode='' or CarryCode='{2}') and  EffectTime<='{3}' and InvalidTime>='{3}' ", sky.FromCityCode, sky.ToCityCode, sky.CarryCode, sky.FromDate.ToString("yyyy-MM-dd HH:mm:ss"));
                Bd_Air_Fares YFare = GetYFares(sqlWhere, sky.CarryCode);
                if (YFare != null)
                {
                    sky.FareFee = YFare.FareFee;
                    sky.Mileage = YFare.Mileage.ToString();
                    SkySeatPrice = airqurey.GetSkyInfo(sky).SpacePrice;
                    if (SkySeatPrice != -1 && SkySeatPrice != 0)
                    {
                        sky.Discount = PnrAnalysis.FormatPNR.GetZk(sky.FareFee.ToString(), SkySeatPrice.ToString()).ToString();
                        sky.SpacePrice = SkySeatPrice;
                    }
                }
                sky.ABFee = _ABFee;
                listSky.Add(sky);
                i++;
            }
        }
        return listSky;
    }
    /// <summary>
    /// 构造乘机人信息
    /// </summary>
    /// <param name="strPasStr"></param>
    /// <returns></returns>
    public List<List<Tb_Ticket_Passenger>> GetPasList(string strPasStr, out int AdultNum, out int ChdNum, out int INFNum)
    {
        AdultNum = 0;
        ChdNum = 0;
        INFNum = 0;
        List<List<Tb_Ticket_Passenger>> pasList = new List<List<Tb_Ticket_Passenger>>();
        List<Tb_Ticket_Passenger> ChildList = new List<Tb_Ticket_Passenger>();
        List<Tb_Ticket_Passenger> AduINFList = new List<Tb_Ticket_Passenger>();
        if (!string.IsNullOrEmpty(strPasStr))
        {
            string[] strArr = strPasStr.Split(new string[] { "@" }, StringSplitOptions.RemoveEmptyEntries);
            if (strArr.Length > 0)
            {
                foreach (string strPas in strArr)
                {
                    //乘客序号#####乘客姓名#####乘客类型#####证件号类型#####证件号码#####乘客手机#####是否常旅客#####常旅客备注
                    string[] pasArr = strPas.Split(new string[] { "#####" }, StringSplitOptions.None);
                    if (pasArr != null && pasArr.Length >= 8)
                    {
                        Tb_Ticket_Passenger tpasser = new Tb_Ticket_Passenger();
                        tpasser.PassengerName = pasArr[1];
                        tpasser.CType = pasArr[3];
                        tpasser.Cid = pasArr[4];
                        tpasser.PassengerType = int.Parse(pasArr[2]);
                        tpasser.A10 = pasArr[5];//乘客手机号码
                        tpasser.Remark = HttpUtility.UrlDecode(pasArr[7]);//乘客备注
                        if (tpasser.PassengerType == 2)
                        {
                            ChildList.Add(tpasser);
                            ChdNum++;
                        }
                        else
                        {
                            if (tpasser.PassengerType == 1)
                            {
                                AdultNum++;
                            }
                            else
                            {
                                INFNum++;
                            }
                            AduINFList.Add(tpasser);
                        }
                    }
                }
                //排序
                ChildList.Sort(delegate(Tb_Ticket_Passenger a, Tb_Ticket_Passenger b)
                {
                    return (a.PassengerType - b.PassengerType);
                });
                AduINFList.Sort(delegate(Tb_Ticket_Passenger a, Tb_Ticket_Passenger b)
                {
                    return (a.PassengerType - b.PassengerType);
                });
                pasList.Add(AduINFList);
                pasList.Add(ChildList);
            }
        }
        return pasList;
    }
    /// <summary>
    /// 获取订单实体
    /// </summary>
    /// <param name="LinkMan"></param>
    /// <param name="LinkManPhone"></param>
    /// <param name="PasList"></param>
    /// <param name="SkyWayList"></param>
    /// <returns></returns>
    private Tb_Ticket_Order GetOrder(string CPCpyNo, string LinkMan, string LinkManPhone, string Remark, List<Tb_Ticket_Passenger> PasList, List<Tb_Ticket_SkyWay> SkyWayList)
    {
        Tb_Ticket_Order to = new Tb_Ticket_Order();
        DataAction d = new DataAction();
        try
        {
            to.LinkMan = LinkMan == "" ? mCompany.ContactUser : LinkMan;
            to.LinkManPhone = LinkManPhone == "" ? mCompany.ContactTel : LinkManPhone;
            //线下订单
            to.OrderSourceType = 4;
            to.OrderStatusCode = 27;//线下订单申请,等待处理
            to.PolicySource = 1;//默认b2b


            to.CreateCpyName = mCompany != null ? mCompany.UninAllName : "";
            to.CreateCpyNo = mCompany != null ? mCompany.UninCode : "";
            to.CPCpyNo = CPCpyNo;//默认出票公司编号
            to.CreateLoginName = mUser.LoginName;
            to.CreateUserName = mUser.UserName;
            to.OwnerCpyNo = mCompany != null ? mCompany.UninCode : "";
            to.OwnerCpyName = mCompany != null ? mCompany.UninAllName : "";
            //Y舱价格
            decimal YFare = 0m;
            //是否允许换编码出票
            to.AllowChangePNRFlag = true;
            List<string> CarryCodeList = new List<string>();
            List<string> FlightCodeList = new List<string>();
            List<string> AirTimeList = new List<string>();
            List<string> TravelList = new List<string>();
            List<string> TravelCodeList = new List<string>();
            List<string> SpaceList = new List<string>();
            List<string> DiscountList = new List<string>();
            for (int i = 0; i < SkyWayList.Count; i++)
            {
                Tb_Ticket_SkyWay sky = SkyWayList[i];
                if (YFare == 0m)
                {
                    YFare = sky.FareFee;
                }
                CarryCodeList.Add(sky.CarryCode);
                FlightCodeList.Add(sky.FlightCode);
                AirTimeList.Add(sky.FromDate.ToString("yyyy-MM-dd HH:mm"));
                TravelCodeList.Add(sky.FromCityCode + "-" + sky.ToCityCode);
                TravelList.Add(sky.FromCityName + "-" + sky.ToCityName);
                SpaceList.Add(sky.Space);
                DiscountList.Add(sky.Discount);
            }
            //行程类型
            int _TravelType = 1;
            if (SkyWayList.Count == 1)
            {
                _TravelType = 1;//单程
            }
            else if (SkyWayList.Count == 2)
            {
                if (SkyWayList[0].ToCityCode.ToUpper() == SkyWayList[1].FromCityCode.ToUpper() && SkyWayList[0].FromCityCode.ToUpper() != SkyWayList[1].ToCityCode.ToUpper())
                {
                    _TravelType = 3;//联程
                }
                else
                {
                    _TravelType = 2;//往返
                }
            }
            else
            {
                _TravelType = 4;
            }
            to.TravelType = _TravelType;
            to.CarryCode = string.Join("/", CarryCodeList.ToArray());
            to.FlightCode = string.Join("/", FlightCodeList.ToArray());
            //to.AirTime = string.Join("/", AirTimeList.ToArray());
            to.AirTime = DateTime.Parse(AirTimeList[0]);
            to.Travel = string.Join("/", TravelList.ToArray());
            to.TravelCode = string.Join("/", TravelCodeList.ToArray());
            to.Space = string.Join("/", SpaceList.ToArray());
            to.Discount = string.Join("/", DiscountList.ToArray());
            //乘客人数
            to.PassengerNumber = PasList.Count;
            //是否有婴儿
            bool IsExistINF = false;
            //是否有儿童
            bool IsExistCHD = false;
            //乘客姓名 已"|"分割
            List<string> PasNameList = new List<string>();
            foreach (Tb_Ticket_Passenger item in PasList)
            {
                PasNameList.Add(item.PassengerName);
                if (item.PassengerType == 3)
                {
                    IsExistINF = true;
                }
                if (item.PassengerType == 2)
                {
                    IsExistCHD = true;
                }
            }
            to.IsChdFlag = IsExistCHD;
            to.HaveBabyFlag = IsExistINF;

            //婴儿价格           
            decimal _TempFare = (0.1m * YFare) / 10;
            to.BabyFee = d.FourToFiveNum(_TempFare, 0) * 10;

            to.PassengerName = string.Join("|", PasNameList.ToArray());
            //备注
            to.YDRemark = Remark;
        }
        catch (Exception ex)
        {
            PnrAnalysis.LogText.LogWrite(ex.Message, "CommonAjAx\\GetOrder");
        }
        return to;
    }
    /// <summary>
    /// 获取航空公司出票Office号 
    /// </summary>
    /// <param name="CarryCode"></param>
    /// <param name="defaultOffice"></param>
    /// <returns></returns>
    public string GetPrintOffice(string CpyNo, string CarryCode)
    {
        string PrintOffice = "";
        string sqlWhere = string.Format(" CpyNo='{0}' and AirCode='{1}' ", CpyNo, CarryCode);
        List<Tb_Ticket_PrintOffice> list = this.baseDataManage.CallMethod("Tb_Ticket_PrintOffice", "GetList", null, new object[] { sqlWhere }) as List<Tb_Ticket_PrintOffice>;
        if (list != null && list.Count > 0)
        {
            if (!string.IsNullOrEmpty(list[0].OfficeCode))
            {
                PrintOffice = list[0].OfficeCode;
            }
        }
        return PrintOffice;
    }
    /// <summary>
    /// 修改订单状态
    /// </summary>
    /// <param name="id"></param>
    /// <param name="OrderStatusCode"></param>
    /// <returns></returns>
    public bool UpdateOrderStatus(string id, string OrderStatusCode)
    {
        IHashObject hash = new HashObject();
        hash.Add("id", id);
        hash.Add("OrderStatusCode", OrderStatusCode);
        return (bool)this.baseDataManage.CallMethod("Tb_Ticket_Order", "Update", null, new object[] { hash });
    }
    /// <summary>
    /// 审核订单 修改订单
    /// </summary>
    /// <returns></returns>
    public bool UpdateOrder(out string errCode, out string ErrMsg, out string result, string PageName, string OpType)
    {
        bool IsSuc = false;
        bool IsDefault = false;
        errCode = "-1";
        ErrMsg = "";
        result = "";
        //订单管理
        Tb_Ticket_OrderBLL OrderBLL = new Tb_Ticket_OrderBLL();
        //订单id
        string id = GetVal("Id", "", out IsDefault);
        //订单号
        string OrderId = GetVal("OrderId", "", out IsDefault);
        //出票公司编号
        string strCPCpyNo = GetVal("CPCpyNo", "", out IsDefault);
        //登录账号
        string strLoginName = GetVal("LoginName", "", out IsDefault);
        //角色
        string strRoleType = GetVal("RoleType", "", out IsDefault);
        if (OrderId == "" || id == "" || strCPCpyNo == "" || strLoginName == "")
        {
            ErrMsg = "订单号不存在！";
            PnrAnalysis.LogText.LogWrite("strRoleType=" + strRoleType + "\r\nOrderId=" + OrderId + "\r\nid=" + id + "\r\nstrCPCpyNo=" + strCPCpyNo + "\r\nstrLoginName=" + strLoginName, "CommonAjAx\\UpdateOrder_Null");
            return IsSuc;
        }
        try
        {
            //加锁
            if (OrderBLL.LockOrder(true, id, strLoginName, strCPCpyNo, out ErrMsg))
            {
                string Flag = GetVal("Flag", "", out IsDefault);
                //是否手动录入 1是 0否
                string IsHand = GetVal("IsHand", "0", out IsDefault);
                string NotAdult = GetVal("NotAdult", "0", out IsDefault);
                string Pnr = GetVal("Pnr", "", out IsDefault);
                string Office = GetVal("Office", "", out IsDefault);

                string strPolicyPoint = GetVal("PolicyPoint", "0", out IsDefault);
                string strPolicyType = GetVal("PolicyType", "1", out IsDefault);
                string strSeatPrice = GetVal("SeatPrice", "0", out IsDefault);
                string strJJPrice = GetVal("JJPrice", "0", out IsDefault);
                string strRQPrice = GetVal("RQPrice", "0", out IsDefault);
                decimal PolicyPoint = 0m;
                int PolicyType = 1;

                //儿童或成人 舱位价格 机建价格 燃油价格
                decimal SeatPrice = 0m;
                decimal JJPrice = 0m;
                decimal RQPrice = 0m;

                decimal BodyFareINF = 0m;//婴儿舱位价格
                decimal BodyJJINF = 0m;//婴儿机建价格
                decimal BodyRQINF = 0m;//婴儿燃油价格


                decimal.TryParse(strPolicyPoint, out  PolicyPoint);
                decimal.TryParse(strSeatPrice, out  SeatPrice);
                decimal.TryParse(strJJPrice, out  JJPrice);
                decimal.TryParse(strRQPrice, out  RQPrice);
                int.TryParse(strPolicyType, out PolicyType);
                if (string.IsNullOrEmpty(Pnr))
                {
                    ErrMsg = "输入编码不能为空！";
                    return IsSuc;
                }
                if (string.IsNullOrEmpty(OrderId))
                {
                    ErrMsg = "该订单号不能为空！";
                    return IsSuc;
                }
                //管理员处理
                PbProject.Model.ConfigParam UseConfigParam = this.configparam;
                if (strRoleType == "1")
                {
                    List<PbProject.Model.Bd_Base_Parameters> GYParam = baseDataManage.CallMethod("Bd_Base_Parameters", "GetList", null, new Object[] { "CpyNo='" + strCPCpyNo + "'" }) as List<Bd_Base_Parameters>;
                    UseConfigParam = Bd_Base_ParametersBLL.GetConfigParam(GYParam);
                }
                if (Office == "")
                {
                    Office = UseConfigParam.Office.Split('^')[0];
                }

                PbProject.Logic.Pay.Bill bill = new PbProject.Logic.Pay.Bill();
                //扩展参数
                ParamEx pe = new ParamEx();
                pe.UsePIDChannel = this.KongZhiXiTong != null && this.KongZhiXiTong.Contains("|48|") ? 2 : 0;
                PbProject.Logic.PID.SendInsManage SendIns = new PbProject.Logic.PID.SendInsManage(strLoginName, strCPCpyNo, pe, UseConfigParam);
                OrderInputParam InputParam = new OrderInputParam();
                //成人订单数据显示            
                InputParam = OrderBLL.GetOrder(OrderId, InputParam, out ErrMsg);
                PnrAnalysis.PnrModel pnrModel = null;
                PnrAnalysis.PatModel patModel = null;
                PnrAnalysis.PatModel patINFModel = null;
                InputParam.PnrInfo = new PnrAnalysis.Model.RePnrObj();
                Tb_Ticket_Order Order = InputParam.OrderParamModel[0].Order;
                #region RT和PAT价格获取
                if (IsHand == "0")//1手动录入 0非手动录入
                {
                    //不是手动录入
                    if (!string.IsNullOrEmpty(Office))
                    {
                        pnrModel = SendIns.GetPnr(Pnr, Office, out ErrMsg);
                    }
                    else
                    {
                        pnrModel = SendIns.GetPnr(Pnr, out ErrMsg);
                    }
                    if (pnrModel != null && ErrMsg == "")
                    {
                        //儿童
                        if (Order.IsChdFlag)
                        {
                            //儿童编码
                            InputParam.PnrInfo.childPnr = Pnr;
                            //儿童大编码
                            InputParam.PnrInfo.childPnrToBigPNR = pnrModel._BigPnr;
                            //Office
                            InputParam.PnrInfo.Office = pnrModel._CurrUseOffice;
                            InputParam.PnrInfo.PnrList[1] = pnrModel;
                            InputParam.PnrInfo.PrintOffice = Order.PrintOffice;
                            InputParam.PnrInfo.childPnrRTContent = pnrModel._OldPnrContent;
                            InputParam.PnrInfo.HandleRTContent = pnrModel._OldPnrContent;
                            //Pat
                            patModel = SendIns.GetPat(Pnr, pnrModel._CurrUseOffice, 2, out ErrMsg);
                        }
                        else
                        {
                            //成人编码
                            InputParam.PnrInfo.AdultPnr = Pnr;
                            //成人大编码
                            InputParam.PnrInfo.AdultPnrToBigPNR = pnrModel._BigPnr;
                            //Office
                            InputParam.PnrInfo.Office = pnrModel._CurrUseOffice;
                            InputParam.PnrInfo.PnrList[0] = pnrModel;
                            InputParam.PnrInfo.PrintOffice = Order.PrintOffice;
                            InputParam.PnrInfo.AdultPnrRTContent = pnrModel._OldPnrContent;
                            InputParam.PnrInfo.HandleRTContent = pnrModel._OldPnrContent;

                            //婴儿
                            patINFModel = SendIns.GetPat(Pnr, pnrModel._CurrUseOffice, 3, out ErrMsg);
                            //成人
                            patModel = SendIns.GetPat(Pnr, pnrModel._CurrUseOffice, 1, out ErrMsg);
                        }

                        //婴儿
                        if (patINFModel != null)
                        {
                            InputParam.PnrInfo.PatList[2] = patINFModel.PatCon;
                            InputParam.PnrInfo.PatModelList[2] = patINFModel;
                            if (patINFModel.PatList.Count > 0)
                            {
                                //婴儿价格
                                decimal.TryParse(patINFModel.PatList[0].Fare, out BodyFareINF);
                                decimal.TryParse(patINFModel.PatList[0].TAX, out BodyJJINF);
                                decimal.TryParse(patINFModel.PatList[0].RQFare, out BodyRQINF);
                            }
                        }
                        else
                        {
                            if (NotAdult == "1")
                            {
                                ErrMsg = "未能PAT出婴儿价格！";
                                return IsSuc;
                            }
                        }
                        //不算成人
                        if (NotAdult == "0")
                        {
                            //成人或者儿童
                            if (patModel != null)
                            {
                                if (Order.IsChdFlag)
                                {
                                    //儿童PAT
                                    InputParam.PnrInfo.PatModelList[1] = patModel;
                                    InputParam.PnrInfo.PatList[1] = patModel.PatCon;
                                }
                                else
                                {
                                    //成人PAT                      
                                    InputParam.PnrInfo.PatModelList[0] = patModel;
                                    InputParam.PnrInfo.PatList[0] = patModel.PatCon;
                                }
                                //取价格 暂时取第一个低价格 以后可根据权限控制
                                decimal.TryParse(patModel.PatList[0].Fare, out SeatPrice);
                                decimal.TryParse(patModel.PatList[0].TAX, out JJPrice);
                                decimal.TryParse(patModel.PatList[0].RQFare, out RQPrice);
                            }
                            else
                            {
                                if (Order.IsChdFlag)
                                {
                                    ErrMsg = "未能PAT出儿童价格！";
                                }
                                else
                                {
                                    ErrMsg = "未能PAT出成人价格！";
                                }
                                return IsSuc;
                            }
                        }
                    }
                    else
                    {
                        return IsSuc;
                    }
                }
                else
                {
                    //不算成人
                    if (NotAdult == "1")
                    {
                        BodyFareINF = SeatPrice;
                        BodyJJINF = JJPrice;
                        BodyRQINF = RQPrice;
                    }
                }
                #endregion


                #region 更新订单 主要修改价格,订单状态,政策和添加订单账单明细
                Data d = new Data(this.mUser.CpyNo);//采购佣金进舍规则: 0.舍去佣金保留到元、1.舍去佣金保留到角、2.舍去佣金保留到分
                //订单实体需要更改数据的字段名
                List<string> UpdateOrderFileds = new List<string>();
                //订单修改字段
                UpdateOrderFileds.AddRange(new string[] { "A1", "PayMoney", "BabyPatContent", "PNR", "BigCode", "Office", "PrintOffice", "CPCpyNo", "OrderStatusCode", "DiscountDetail", "PolicyPoint", "ReturnPoint", "PolicyType", "PolicySource", "PMFee", "ABFee", "FuelFee", "BabyFee", "PolicyMoney" });
                //航段实体需要更改数据的字段名
                List<string> UpdateSkyWayFileds = new List<string>();
                //航段修改字段
                UpdateSkyWayFileds.AddRange(new string[] { "PNRContent", "NewPNRContent", "PAT", "Terminal", "SpacePrice", "ABFee", "FuelFee", "Discount" });
                //乘客实体需要更改数据的字段名
                List<string> UpdatePasFileds = new List<string>();
                //修改乘机人
                UpdatePasFileds.AddRange(new string[] { "PMFee", "ABFee", "FuelFee" });
                bool IsCHD = false;//是否儿童
                //修改实体相关的值后更新即可
                for (int i = 0; i < InputParam.OrderParamModel.Count; i++)
                {
                    OrderMustParamModel item = InputParam.OrderParamModel[i];

                    #region 设置需要更改数据的字段名集合
                    item.UpdateOrderFileds = UpdateOrderFileds;
                    item.UpdateSkyWayFileds = UpdateSkyWayFileds;
                    item.UpdatePassengerFileds = UpdatePasFileds;
                    #endregion

                    #region 实体处理
                    //订单中的总价
                    decimal TotalPMPrice = 0m, TotalABFare = 0, TotalRQFare = 0m;
                    decimal YongJin = 0m;
                    IsCHD = item.Order.IsChdFlag;
                    //不是手动录入编码类型要和订单类型一样
                    if (IsHand == "0")
                    {
                        if (!((pnrModel._PasType == "1" && !item.Order.IsChdFlag) || (pnrModel._PasType == "2" && item.Order.IsChdFlag)))
                        {
                            ErrMsg = string.Format("该{0}编码与订单类型不否，{0}为{1},该订单为{2}", Pnr, pnrModel._PasType == "1" ? "成人编码" : "儿童编码", item.Order.IsChdFlag ? "儿童订单" : "成人订单");
                            return IsSuc;
                        }
                    }
                    //订单处理 成人订单政策
                    if (!item.Order.IsChdFlag)
                    {
                        #region 成人或者婴儿实体价格赋值
                        //乘机人实体处理
                        for (int j = 0; j < item.PasList.Count; j++)
                        {
                            if (item.PasList[j].PassengerType == 1)
                            {
                                if (NotAdult == "0")
                                {
                                    //成人
                                    item.PasList[j].PMFee = SeatPrice;
                                    item.PasList[j].ABFee = JJPrice;
                                    item.PasList[j].FuelFee = RQPrice;
                                    YongJin = d.CreateCommissionCG(item.PasList[j].PMFee, PolicyPoint);
                                }
                            }
                            else
                            {
                                //婴儿
                                if (item.PasList[j].PassengerType == 3)
                                {
                                    if (NotAdult == "1")//不算成人 即输入为婴儿价格
                                    {
                                        item.PasList[j].PMFee = SeatPrice;
                                        item.PasList[j].ABFee = JJPrice;
                                        item.PasList[j].FuelFee = RQPrice;
                                    }
                                    else
                                    {
                                        if (BodyFareINF != 0)
                                        {
                                            item.PasList[j].PMFee = BodyFareINF;
                                            item.PasList[j].ABFee = BodyJJINF;
                                            item.PasList[j].FuelFee = BodyRQINF;
                                        }
                                        else
                                        {
                                            //手动的婴儿处理
                                            if (NotAdult == "0" && IsHand == "1")
                                            {
                                                item.PasList[j].PMFee = SeatPrice;
                                                item.PasList[j].ABFee = JJPrice;
                                                item.PasList[j].FuelFee = RQPrice;
                                            }
                                        }
                                    }
                                }
                            }
                            if (item.PasList[j].PassengerType == 1 || item.PasList[j].PassengerType == 3)
                            {
                                //订单价格
                                TotalPMPrice += item.PasList[j].PMFee;
                                TotalABFare += item.PasList[j].ABFee;
                                TotalRQFare += item.PasList[j].FuelFee;
                            }
                        }
                        //不是手动录入时
                        //if (IsHand == "0")
                        //{
                        //    //编码中的人数比申请的的
                        //    if (pnrModel != null && pnrModel._PassengerList.Count > item.PasList.Count)
                        //    {
                        //    }
                        //}


                        //成人订单(含有婴儿) 赋值
                        item.Order.PMFee = TotalPMPrice;
                        item.Order.ABFee = TotalABFare;
                        item.Order.FuelFee = TotalRQFare;
                        if (BodyFareINF != 0)
                        {
                            //婴儿票面价
                            item.Order.BabyFee = BodyFareINF;
                            if (IsHand == "0")
                            {
                                //婴儿PAT内容
                                item.Order.BabyPatContent = patINFModel.PatCon;
                            }
                        }
                        //政策
                        item.Order.PolicyPoint = PolicyPoint;
                        item.Order.ReturnPoint = PolicyPoint;
                        //原始政策返点
                        item.Order.OldPolicyPoint = PolicyPoint;

                        item.Order.DiscountDetail = "";
                        item.Order.PolicyType = PolicyType;
                        item.Order.PolicySource = PolicyType;
                        item.Order.CPCpyNo = strCPCpyNo;
                        //修改订单状态为 新订单等待支付
                        item.Order.OrderStatusCode = 1;
                        /*
                        decimal PayPrice = d.CreateOrderPayMoney(item.Order, item.PasList); //计算订单金额
                        item.Order.PayMoney = PayPrice;

                        decimal OrderPrice = d.CreateOrderOrderMoney(item.Order, item.PasList); //出票方收款金额
                        item.Order.OrderMoney = OrderPrice;
                        */
                        bill.CreateOrderAndTicketPayDetailNew(item.Order, item.PasList);

                        item.Order.PolicyMoney = YongJin;
                        item.Order.PNR = Pnr;
                        item.Order.A1 = 1;//已确认
                        if (pnrModel != null)
                        {
                            item.Order.Office = pnrModel._CurrUseOffice;
                            item.Order.BigCode = pnrModel._BigPnr;
                        }
                        #endregion

                    }
                    else
                    {
                        #region 儿童实体赋值
                        //儿童价格
                        //乘机人实体处理
                        for (int j = 0; j < item.PasList.Count; j++)
                        {
                            if (item.PasList[j].PassengerType == 2)
                            {
                                //儿童
                                item.PasList[j].PMFee = SeatPrice;
                                item.PasList[j].ABFee = JJPrice;
                                item.PasList[j].FuelFee = RQPrice;
                                //订单价格
                                TotalPMPrice += item.PasList[j].PMFee;
                                TotalABFare += item.PasList[j].ABFee;
                                TotalRQFare += item.PasList[j].FuelFee;
                                //佣金
                                YongJin = d.CreateCommissionCG(item.PasList[j].PMFee, PolicyPoint);
                            }
                        }

                        //儿童订单赋值                              
                        item.Order.PMFee = TotalPMPrice;
                        item.Order.ABFee = TotalABFare;
                        item.Order.FuelFee = TotalRQFare;
                        //政策
                        item.Order.PolicyPoint = PolicyPoint;
                        item.Order.ReturnPoint = PolicyPoint;
                        //原始政策返点
                        item.Order.OldPolicyPoint = PolicyPoint;

                        item.Order.DiscountDetail = "";
                        item.Order.PolicyType = PolicyType;
                        item.Order.PolicySource = PolicyType;
                        //修改订单状态为 新订单等待支付
                        item.Order.OrderStatusCode = 1;
                        /*
                        item.Order.PayMoney = d.CreateOrderPayMoney(item.Order, item.PasList); //计算订单金额
                        item.Order.OrderMoney = d.CreateOrderOrderMoney(item.Order, item.PasList); //出票方收款金额
                         * */
                        bill.CreateOrderAndTicketPayDetailNew(item.Order, item.PasList);

                        item.Order.PolicyMoney = YongJin;
                        item.Order.PNR = Pnr;
                        item.Order.A1 = 1;//已确认
                        if (pnrModel != null)
                        {
                            item.Order.Office = pnrModel._CurrUseOffice;
                            item.Order.BigCode = pnrModel._BigPnr;
                        }
                        #endregion
                    }

                    //航段实体处理
                    string Discount = "0";
                    for (int k = 0; k < item.SkyList.Count; k++)
                    {
                        if (pnrModel != null)
                        {
                            item.SkyList[k].PnrContent = pnrModel._OldPnrContent;
                            item.SkyList[k].NewPnrContent = pnrModel._OldPnrContent;
                        }
                        if (patModel != null)
                        {
                            item.SkyList[k].Pat = patModel.PatCon;
                        }
                        //只是单程才重新赋值
                        if (item.SkyList.Count == 1)
                        {
                            item.SkyList[k].SpacePrice = SeatPrice;
                            item.SkyList[k].ABFee = JJPrice;
                            item.SkyList[k].FuelFee = RQPrice;
                            item.SkyList[k].Discount = PnrAnalysis.FormatPNR.GetZk(item.SkyList[k].FareFee.ToString(), SeatPrice.ToString()).ToString();
                            Discount = item.SkyList[k].Discount;
                            if (Discount.Length > 10)
                            {
                                Discount = Discount.Substring(0, 10);
                            }
                            item.SkyList[k].Discount = Discount;
                        }
                        else
                        {
                            Discount = item.SkyList[k].Discount;
                            if (Discount.Length > 10)
                            {
                                Discount = Discount.Substring(0, 10);
                            }
                            item.SkyList[k].Discount = Discount;
                            break;
                        }
                    }
                    #endregion

                    #region 添加订单账单明细sql
                    //List<string> sqlList = bill.CreateOrderAndTicketPayDetail(item.Order, item.PasList);
                    List<string> sqlList = bill.CreateOrderAndTicketPayDetailNew(item.Order, item.PasList);
                    InputParam.ExecSQLList.AddRange(sqlList.ToArray());
                    #endregion

                    #region 日志
                    Log_Tb_AirOrder logOrder = new Log_Tb_AirOrder();
                    logOrder.id = Guid.NewGuid();
                    logOrder.OrderId = item.Order.OrderId;
                    logOrder.OperTime = DateTime.Now;
                    logOrder.OperType = "修改";
                    logOrder.OperContent = mUser.LoginName + "于" + logOrder.OperTime + "提交" + (IsCHD ? "儿童" : "") + "线下订单（" + item.Order.OrderId + "）,审核通过!";
                    logOrder.OperLoginName = mUser.LoginName;
                    logOrder.OperUserName = mUser.UserName;
                    logOrder.CpyNo = mCompany.UninCode;
                    logOrder.CpyName = mCompany.UninName;
                    logOrder.CpyType = mCompany.RoleType;
                    logOrder.WatchType = mCompany.RoleType;
                    InputParam.ExecSQLList.Add(PbProject.Dal.Mapping.MappingHelper<Log_Tb_AirOrder>.CreateInsertModelSql(logOrder));
                    #endregion
                }



                #endregion
                if (!string.IsNullOrEmpty(strCPCpyNo))
                {
                    //修改订单有关实体信息
                    IsSuc = OrderBLL.UpdateOrder(ref InputParam, out ErrMsg);
                }
                else
                {
                    ErrMsg = "审核公司编号不能为空！";
                }
            }
            else
            {
                IsSuc = false;
                ErrMsg = "该订单已被锁定！";
            }
        }
        catch (Exception ex)
        {
            ErrMsg = ex.Message + ErrMsg;
            PnrAnalysis.LogText.LogWrite(ErrMsg, "CommonAjAx\\UpdateOrder");
        }
        finally
        {
            //解锁
            string tempErrMsg = "";
            if (!OrderBLL.LockOrder(false, id, strLoginName, strCPCpyNo, out tempErrMsg))
            {
                ErrMsg += tempErrMsg;
            }
        }
        return IsSuc;
    }
    /// <summary>
    /// 取消订单
    /// </summary>
    /// <returns></returns>
    public bool CancelOrder(out string errCode, out string ErrMsg, out string result, string PageName, string OpType)
    {
        errCode = "-1";
        ErrMsg = "";
        result = "";
        bool IsSuc = false;
        bool IsDefault = false;
        string id = GetVal("oid", "", out IsDefault);
        string OrderId = GetVal("OrderId", "", out IsDefault);
        string PNR = GetVal("PNR", "", out IsDefault);
        string Office = GetVal("Office", "", out IsDefault);
        bool isCancelpnr = GetVal("isCancelpnr", "0", out IsDefault) == "1" ? true : false;
        string LoginName = GetVal("LoginName", "", out IsDefault);
        string CpyNo = GetVal("CpyNo", "", out IsDefault);
        try
        {
            //订单管理
            Tb_Ticket_OrderBLL OrderBLL = new Tb_Ticket_OrderBLL();
            //扩展参数
            ParamEx pe = new ParamEx();
            pe.UsePIDChannel = this.KongZhiXiTong != null && this.KongZhiXiTong.Contains("|48|") ? 2 : 0;
            PbProject.Logic.PID.SendInsManage SendIns = new PbProject.Logic.PID.SendInsManage(LoginName, CpyNo, pe, this.configparam);
            string msg = string.Format(" 取消订单  订单号:{0}", OrderId);
            /*
                if (isCancelpnr && !string.IsNullOrEmpty(PNR))
                {
                
                    //取消编码
                    if (SendIns.CancelPnr(PNR, Office))
                    {
                        msg += "取消编码:" + PNR + "成功,";
                        ErrMsg = PNR + "取消成功";
                    }
                    else
                    {
                        msg += "取消编码:" + PNR + "失败,";
                        ErrMsg = PNR + "取消失败";
                    }
                
                }
             */

            Tb_Ticket_Order Order = this.baseDataManage.CallMethod("Tb_Ticket_Order", "GetById", null, new object[] { id }) as Tb_Ticket_Order;
            if (Order != null)
            {
                IsSuc = OrderBLL.CancelOrder(Order, mUser, mCompany, msg);
                if (IsSuc)
                {
                    ErrMsg += " 订单取消成功";
                }
                else
                {
                    ErrMsg += " 订单取消失败";
                }
            }
            else
            {
                ErrMsg = "订单号(" + OrderId + ")未找到！";
            }
        }
        catch (Exception ex)
        {
            ErrMsg = ex.Message;
            PnrAnalysis.LogText.LogWrite(ErrMsg, "CommonAjAx\\CancelOrder");
        }
        return IsSuc;
    }
    #endregion

}
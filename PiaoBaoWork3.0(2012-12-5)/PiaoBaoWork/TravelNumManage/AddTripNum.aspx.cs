using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using PbProject.Logic;
using DataBase.Data;
using PbProject.Model;

public partial class TravelNumManage_AddTripNum : BasePage
{
    #region 属性
    /// <summary>
    /// 检查Session是否丢失
    /// </summary>
    //public bool SessionIsNull
    //{
    //    get
    //    {
    //        bool isSuc = false;
    //        if (Session[new SessionContent().USERLOGIN] == null)
    //        {
    //            isSuc = true;
    //        }
    //        return isSuc;
    //    }
    //}
    #endregion


    protected void Page_Load(object sender, EventArgs e)
    {
        //if (this.SessionIsNull)
        //{
        //    ScriptManager.RegisterStartupScript(this, GetType(), Guid.NewGuid().ToString(), "showdialog('页面已经失效,请重新登录！','0');", true);
        //    return;
        //}
        if (mCompany.RoleType > 3)
        {
            ScriptManager.RegisterStartupScript(this, GetType(), Guid.NewGuid().ToString(), "showdialog('您没有权限访问该页面！','0');", true);
            return;
        }
        try
        {
            if (GetVal("OP", "") == "trip")
            {
                //行程单操作
                TripOpertion();
            }
            else
            {
                if (!IsPostBack)
                {
                    //初始化数据
                    InitParam();
                    //绑定Office
                    BindOffice();
                }
            }
        }
        catch (Exception)
        {
            ScriptManager.RegisterStartupScript(this, GetType(), Guid.NewGuid().ToString(), "showdialog('页面加载异常！','0');", true);
        }
    }
    /// <summary>
    /// 传过来的参数
    /// </summary>
    public void InitParam()
    {
        //领用公司编号和全称
        if (Request["UseCpyNo"] != null)
        {
            Hid_UseCpyNo.Value = Request["UseCpyNo"].ToString();
        }
        if (Request["UseCpyName"] != null)
        {
            Hid_UseCpyName.Value = Request["UseCpyName"].ToString();
        }
        //行程单分配
        if (Request["AppId"] != null && Request["AppId"].ToString() != "")
        {
            string id = Request["AppId"].ToString();
            Hid_AppId.Value = id;
            string sqlWhere = string.Format("id='{0}'", id);
            List<Tb_TripNumApply> appList = this.baseDataManage.CallMethod("Tb_TripNumApply", "GetList", null, new object[] { sqlWhere }) as List<Tb_TripNumApply>;
            if (appList != null && appList.Count > 0)
            {
                Tb_TripNumApply Tb_TripNumApply = appList[0];
                ApplyConStr(Tb_TripNumApply);
                Hid_UseCpyNo.Value = Tb_TripNumApply.ApplyCpyNo;
                Hid_UseCpyName.Value = Tb_TripNumApply.ApplyCpyName;
                tr_adultRemark.Visible = true;
            }
        }
        spanImport.Visible = false;
        spanSendTrip.Visible = true;
        //行程单入库
        if (Request["Import"] != null)
        {
            spanImport.Visible = true;
            spanSendTrip.Visible = false;
            lblShow.Text = "行程单入库";
        }
        else
        {
            lblShow.Text = "行程单发放";
        }
        if (mCompany.RoleType == 1)
        {
            Hid_OwnerCpyNo.Value = Request["OwnerCpyNo"].ToString();
            Hid_OwnerCpyName.Value = Request["OwnerCpyName"].ToString();
            Hid_AddLoginName.Value = Request["AddLoginName"].ToString();
            Hid_AddUserName.Value = Request["AddUserName"].ToString();
        }
        else if (mCompany.RoleType == 2 || mCompany.RoleType == 3)
        {
            Hid_OwnerCpyNo.Value = mCompany.UninCode;
            Hid_OwnerCpyName.Value = mCompany.UninAllName;
            Hid_AddLoginName.Value = mUser.LoginName;
            Hid_AddUserName.Value = mUser.UserName;
        }
    }
    /// <summary>
    /// 构造行程单申请字符串
    /// </summary>
    /// <param name="ApplyModel"></param>
    /// <returns></returns>
    public void ApplyConStr(Tb_TripNumApply ApplyModel)
    {
        StringBuilder Sb = new StringBuilder();
        Sb.Append("<tr>");
        Sb.Append("<td align=\"right\" valign=\"top\">");
        Sb.Append("申请张数:");
        Sb.Append("</td>");
        Sb.Append("<td valign=\"top\">");
        Sb.Append(ApplyModel.ApplyCount);
        Sb.Append("</td>");
        Sb.Append("<td align=\"left\">申请时间:" + ApplyModel.ApplyDate.ToString("yyyy-MM-dd HH:mm:ss"));
        Sb.Append("</td>");
        Sb.Append("</tr>");
        Sb.Append("<tr>");
        Sb.Append("<td align=\"right\" valign=\"top\" >");
        Sb.Append("申请说明:");
        Sb.Append("</td>");
        Sb.Append("<td style=\"height:30px;\" valign=\"top\">");
        Sb.Append(ApplyModel.ApplyRemark);
        Sb.Append("</td>");

        Sb.Append("<td>");
        Sb.Append("</td>");
        Sb.Append("</tr>");
        ApplyCon.Text = Sb.ToString();
    }

    //绑定Office返回字符串
    public string BindOffice()
    {
        PbProject.Model.ConfigParam mconfig = null;
        StringBuilder sbstr = new StringBuilder();
        if (mCompany.RoleType == 1)
        {
            if (Hid_OwnerCpyNo.Value.Trim() != "")
            {
                List<Bd_Base_Parameters> baseDic = baseDataManage.CallMethod("Bd_Base_Parameters", "GetList", null, new Object[] { "CpyNo='" + Hid_OwnerCpyNo.Value.Trim() + "'" }) as List<Bd_Base_Parameters>;
                mconfig = PbProject.Logic.ControlBase.Bd_Base_ParametersBLL.GetConfigParam(baseDic);
            }
        }
        else
        {
            mconfig = this.configparam;
        }

        if (mconfig != null)
        {
            string strOffice = mconfig.Office;
            string strIataCode = mconfig.IataCode;
            string strTicketCompany = mconfig.TicketCompany;
            string[] arrOffice = strOffice.Split(new string[] { "^", "|", ",", " ", "@", "-" }, StringSplitOptions.None);
            string[] arrIataCode = strIataCode.Split(new string[] { "^", "|", ",", " ", "@", "-" }, StringSplitOptions.None);
            string[] arrTicketCompany = strTicketCompany.Split(new string[] { "^", "|", ",", " ", "@", "-" }, StringSplitOptions.None);
            if (arrOffice.Length != 0 && arrOffice.Length == arrIataCode.Length && arrIataCode.Length == arrTicketCompany.Length)
            {
                for (int i = 0; i < arrOffice.Length; i++)
                {
                    ListItem li = new ListItem(arrOffice[i], arrOffice[i].ToUpper().Trim() + "@@" + arrIataCode[i].Trim() + "@@" + arrTicketCompany[i].Trim());
                    if (i == 0)
                    {
                        li.Selected = true;
                        //航协号
                        txtIataCode.Text = arrIataCode[i].Trim();
                        //填开单位
                        txtTicketCompany.Text = arrTicketCompany[i].Trim();
                    }
                    ddlOffice.Items.Add(li);
                }
                ddlOffice.SelectedIndex = 0;
            }
        }

        return sbstr.ToString();
    }
    /// <summary>
    /// 获取请求值
    /// </summary>
    /// <param name="Name"></param>
    /// <param name="DefaultVal"></param>
    /// <returns></returns>
    public string GetVal(string Name, string DefaultVal)
    {
        if (Request[Name] != null)
        {
            DefaultVal = HttpUtility.UrlDecode(Request[Name].ToString(), Encoding.Default);
        }
        return DefaultVal;
    }
    /// <summary>
    /// 响应客户端结果数据
    /// </summary>
    /// <param name="result"></param>
    public void OutPut(string result)
    {
        Context.Response.ContentType = "text/plain";
        Context.Response.Clear();
        Context.Response.Write(result);
        Context.Response.Flush();
        Context.Response.End();
    }
    /// <summary>
    /// 行程单操作
    /// </summary>
    public void TripOpertion()
    {
        if (GetVal("OP", "") == "trip")
        {
            //0入库 1发放
            string OpType = GetVal("OpType", "0");
            string StartCode = GetVal("StartCode", "0");
            string StartNum = GetVal("StartNum", "0");
            string EndNum = GetVal("EndNum", "0");
            string Office = GetVal("Office", "");
            string IataCode = GetVal("IataCode", "");
            string TicketCompany = GetVal("TicketCompany", "");
            string OwnerCpyNo = GetVal("OwnerCpyNo", "");
            string OwnerCpyName = GetVal("OwnerCpyName", "");
            string UseCpyNo = GetVal("UseCpyNo", "");
            string UseCpyName = GetVal("UseCpyName", "");
            string AdultRemark = GetVal("AdultRemark", "");
            string AppId = GetVal("AppId", "");

            //操作员
            string AddLoginName = GetVal("AddLoginName", "");
            string AddUserName = GetVal("AddUserName", "");
            int _startcode = 0, _start = 0, _end = 0;
            bool a = int.TryParse(StartCode, out _startcode);
            bool b = int.TryParse(StartNum, out _start);
            bool c = int.TryParse(EndNum, out _end);
            string ErrMsg = "0@@失败";
            try
            {
                if (a && b && c)
                {
                    if (_start > _end)
                    {
                        ErrMsg = "0@@行程单开始号段必须小于结束号段！";
                    }
                    else
                    {
                        IHashObject InputParam = new HashObject();
                        InputParam.Add("OpType", OpType);
                        InputParam.Add("StartCode", _startcode);
                        InputParam.Add("startNum", _start);
                        InputParam.Add("endNum", _end);
                        InputParam.Add("Office", Office);
                        InputParam.Add("IataCode", IataCode);
                        InputParam.Add("TicketCompany", TicketCompany);
                        InputParam.Add("OwnerCpyNo", OwnerCpyNo);
                        InputParam.Add("OwnerCpyName", OwnerCpyName);
                        InputParam.Add("UseCpyNo", UseCpyNo);
                        InputParam.Add("UseCpyName", UseCpyName);
                        InputParam.Add("AddLoginName", AddLoginName);
                        InputParam.Add("AddUserName", AddUserName);
                        InputParam.Add("TripStatus", (OpType == "0" ? 1 : 2));//1已入库,未分配  2已分配,未使用
                        int Issuc = (int)this.baseDataManage.CallMethod("Tb_TripDistribution", "ExecProcedureUpdate", null, new object[] { "ImportTrip", InputParam });
                        if (Issuc > 0)
                        {
                            if (OpType == "0")
                            {
                                ErrMsg = "1@@行程单入库成功！";
                            }
                            else if (OpType == "1")
                            {
                                ErrMsg = "1@@行程发放成功！";
                                if (AppId != "")
                                {
                                    IHashObject param = new HashObject();
                                    param.Add("id", AppId);
                                    param.Add("AuditRemark", AdultRemark);
                                    param.Add("ApplyStatus", 4);//修改为行程单申请审核成功,未使用
                                    param.Add("AuditDate", System.DateTime.Now);
                                    param.Add("AuditAccount", AddLoginName);
                                    param.Add("AuditUserName", AddUserName);
                                    param.Add("AuditCpyNo", OwnerCpyNo);
                                    param.Add("AuditCpyName", OwnerCpyName);
                                    bool IsSuc = (bool)this.baseDataManage.CallMethod("Tb_TripNumApply", "Update", null, new object[] { param });
                                    if (!IsSuc)
                                    {
                                        ErrMsg = "1@@行程发放成功，审核失败！";
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (OpType == "0")
                            {
                                ErrMsg = "0@@行程单入库失败！";
                            }
                            else if (OpType == "1")
                            {
                                if (Issuc == -1)
                                {
                                    ErrMsg = "0@@行程发放失败！";
                                }
                                else
                                {
                                    ErrMsg = "0@@该号段还没有入库行程单号,请入库后再发放！";
                                }
                            }
                        }
                    }
                }
                else
                {
                    ErrMsg = "0@@行程单号段错误！";
                }
            }
            catch (Exception ex)
            {
                DataBase.LogCommon.Log.Error(ErrMsg, ex);
            }
            finally
            {
                OutPut(ErrMsg);
            }
        }
    }
}
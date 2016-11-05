using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using PbProject.Model;
using PbProject.WebCommon.Utility.Encoding;
using System.Collections;

public partial class DiscountSet_DiscountSet : BasePage
{
    public TextBox txtA = new TextBox();
    public TextBox txtB = new TextBox();
    public TextBox txtP = new TextBox();
    public TextBox txtM = new TextBox();
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            rblSelectType.Items.Add(new ListItem("扣点", "1"));
            rblSelectType.Items.Add(new ListItem("留点", "2"));
            if (mCompany.RoleType == 2 || mCompany.RoleType == 3)
            {
                rblSelectType.Items.Add(new ListItem("补点", "3"));
            }
            rblSelectType.Items[0].Selected = true;

            this.btnBack.PostBackUrl = string.Format("DiscountList.aspx?currentuserid={0}", Request["currentuserid"].ToString());
            BindCityList();
            //获取详情页面传值
            if (Request["gdid"] != null && Request["groupid"] != null)
            {
                ViewState["gdid"] = Request["gdid"];//详情id
                ViewState["groupid"] = Request["groupid"];//详情页面传来的组id
                GetDiscountDetailInfo(Request["groupid"].ToString(), Request["gdid"].ToString());
                Hid_IsEdit.Value = "1";//编辑
               
            }
            if (Request["addgroupid"] != null)
            {
                ViewState["addgroupid"] = Request["addgroupid"];
                GetDiscountDetailInfo(Request["addgroupid"].ToString(), "");
            }
            //获取组页面传值
            if (Request["gid"] != null)
            {
                ViewState["gid"] = Request["gid"];
                GetDiscountDetailInfo(Request["gid"].ToString(), "");
                showgroupdtl.Visible = false;
                hid_showgroupdtl.Value = "0";
            }
            //扣点组控件禁用
            if (Request["gdid"] != null || Request["addgroupid"] != null)
            {
                this.rblDefaultFlag.Enabled = false;
                this.rblUniteFlag.Enabled = false;
                this.txtUnitePoint.Enabled = false;
            }
           
        }
        if (rblSelectType.SelectedValue == "3")
        {
            this.ddlbasetype.Enabled = false;
        }
    }

    /// <summary>
    /// 获取要修改的组详情数据
    /// </summary>
    /// <param name="id"></param>
    public void GetDiscountDetailInfo(string gid, string gdid)
    {
        this.txtGroupName.Enabled = false;
        if (gid != "")
        {
            Tb_Ticket_StrategyGroup mgroup = (this.baseDataManage.CallMethod("Tb_Ticket_StrategyGroup", "GetList", null, new object[] { "id='" + gid + "'" }) as List<Tb_Ticket_StrategyGroup>)[0] as Tb_Ticket_StrategyGroup;
            this.txtGroupName.Text = mgroup.GroupName.ToString();
            this.rblDefaultFlag.SelectedIndex = mgroup.DefaultFlag == true ? 1 : 0;
            this.rblDefaultFlag.SelectedValue = mgroup.DefaultFlag.ToString();
            this.rblUniteFlag.SelectedIndex = mgroup.UniteFlag;
            this.rblUniteFlag.SelectedValue = mgroup.UniteFlag.ToString();
            this.txtUnitePoint.Text = mgroup.UnitePoint.ToString();
        }
        if (gdid != "")
        {
            Tb_Ticket_TakeOffDetail mgdetail = (this.baseDataManage.CallMethod("Tb_Ticket_TakeOffDetail", "GetList", null, new object[] { "id='" + gdid + "'" }) as List<Tb_Ticket_TakeOffDetail>)[0] as Tb_Ticket_TakeOffDetail;
            this.SelectAirCode1.Value = mgdetail.CarryCode;
            this.ddlbasetype.SelectedValue = mgdetail.BaseType.ToString();
            this.rblSelectType.SelectedValue = mgdetail.SelectType.ToString();
            List<string> listpolicys = new List<string>();
            string[] jkitems = mgdetail.PolicySource.Split(',');
            listpolicys.AddRange(jkitems);
            ListItemCollection listitems = cbljk.Items;
            for (int i = 0; i < listitems.Count; i++)
            {
                if (listpolicys.Contains(listitems[i].Value))
                {
                    listitems[i].Selected = true;
                }
            }
            this.txtStartDate.Value = mgdetail.TimeScope.Split('|')[0].ToString();
            this.txtEndDate.Value = mgdetail.TimeScope.Split('|')[1].ToString();
            this.txtFromCode.Value = mgdetail.FromCityCode.ToString().TrimStart('/');
            this.txtToCode.Value = mgdetail.ToCityCode.ToString().TrimStart('/');
            this.FromCityCode.Value = mgdetail.FromCityCode.ToString().TrimStart('/');
            this.ToCityCode.Value = mgdetail.ToCityCode.ToString().TrimStart('/');
            if (mgdetail.PointScope != "")
            {
                hidtxtCount.Value = mgdetail.PointScope.Split('|').Length.ToString();
                for (int i = 0; i < mgdetail.PointScope.Split('|').Length; i++)
                {
                    //不是最后一个
                    if (i < mgdetail.PointScope.Split('|').Length - 1)
                    {
                        if (i != 4)
                            ((System.Web.UI.HtmlControls.HtmlGenericControl)this.FindControl("sAdd" + i)).Style.Value = "display: none";
                        if (i != 0)
                            ((System.Web.UI.HtmlControls.HtmlGenericControl)this.FindControl("sDelete" + i)).Style.Value = "display: none";
                    }
                    //最后一个
                    else
                    {
                        if (i != 4)
                            ((System.Web.UI.HtmlControls.HtmlGenericControl)this.FindControl("sAdd" + i)).Style.Value = "display: block";
                        if (i != 0)
                            ((System.Web.UI.HtmlControls.HtmlGenericControl)this.FindControl("sDelete" + i)).Style.Value = "display: block";
                    }
                    ((System.Web.UI.HtmlControls.HtmlTableRow)this.FindControl("tr" + i)).Style.Value = "display: block";
                    txtA = ((TextBox)this.FindControl("txtA" + i));
                    txtB = ((TextBox)this.FindControl("txtB" + i));
                    txtP = ((TextBox)this.FindControl("txtpoint" + i));//扣点
                    txtM = ((TextBox)this.FindControl("txtMoney" + i));//现返
                    txtA.Text = mgdetail.PointScope.Split('|')[i].Split('^')[0].Split('-')[0].ToString();
                    txtB.Text = mgdetail.PointScope.Split('|')[i].Split('^')[0].Split('-')[1].ToString();
                    txtP.Text = mgdetail.PointScope.Split('|')[i].Split('^')[1].ToString();
                    txtM.Text = mgdetail.PointScope.Split('|')[i].Split('^')[2].ToString();
                }
            }
            this.showjk.Style.Value = mgdetail.BaseType == 2 ? "display: block" : "display: none";
        }

    }
    //绑定城市控件数据以及接口
    public void BindCityList()
    {
        List<Bd_Air_AirPort> cityList = GetCity("1");
        //出发城市
        From_RightBox.Items.Clear();

        //到达城市       
        To_RightBox.Items.Clear();
        //排序 
        SortedList sortLst = new SortedList();
        foreach (Bd_Air_AirPort city in cityList)
        {
            ListItem item = new ListItem();
            item.Text = city.CityCodeWord + "_" + city.CityName;
            item.Value = city.CityCodeWord;
            if (!sortLst.ContainsKey(city.CityCodeWord))
            {
                sortLst.Add(city.CityCodeWord, item);
            }
        }
        ListItem[] newItem = new ListItem[sortLst.Values.Count];
        sortLst.Values.CopyTo(newItem, 0);
        From_RightBox.Items.AddRange(newItem);
        To_RightBox.Items.AddRange(newItem);
        //List<Bd_Base_Dictionary> list = baseDataManage.CallMethod("Bd_Base_Dictionary", "GetList", null, new Object[] { "ParentID=24" }) as List<Bd_Base_Dictionary>;
        //this.ddljk.DataSource = list;
        //ddljk.DataTextField = "ChildName";
        //ddljk.DataValueField = "ChildID";
        //this.ddljk.DataBind();
    }
    /// <summary>
    /// 获取城市列表
    /// </summary>
    /// <param name="IsDomestic">是否国内 1.是，2.否</param>
    /// <returns></returns>
    public List<Bd_Air_AirPort> GetCity(string IsDomestic)
    {
        List<Bd_Air_AirPort> list = this.baseDataManage.CallMethod("Bd_Air_AirPort", "GetList", null, new object[] { "IsDomestic=" + IsDomestic }) as List<Bd_Air_AirPort>;
        if (list != null && list.Count > 0)
        {
            string cityData = JsonHelper.ObjToJson<List<Bd_Air_AirPort>>(list);
            Hid_InnerCityData.Value = escape(cityData);
        }
        return list;
    }

    /// <summary>
    /// 保存
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnAdd_Click(object sender, EventArgs e)
    {
        string msg = "";
        try
        {
            List<string> sqllist = new List<string>();
            string pointscope = "";
            string basetype = ddlbasetype.SelectedValue;
            string selecttype = rblSelectType.SelectedValue;
            string txtPoint = txtUnitePoint.Text.Trim().Length == 0 ? "0" : txtUnitePoint.Text.Trim();
            string strCarryCode = SelectAirCode1.Value == "" ? "ALL" : SelectAirCode1.Value;
            string strFromCityCode = FromCityCode.Value.Trim() == "" ? "ALL" :  FromCityCode.Value.Trim();
            if (strFromCityCode.Substring(strFromCityCode.Length - 1, 1) != "/")
            {
                strFromCityCode += "/";
            }
            string strToCityCode = ToCityCode.Value.Trim() == "" ? "ALL" : ToCityCode.Value.Trim();
            if (strToCityCode.Substring(strToCityCode.Length - 1, 1) != "/")
            {
                strToCityCode += "/";
            }
            DateTime dtStartDate = DateTime.Parse("1900-01-01 00:00:00");
            DateTime dtEndDate = DateTime.Parse("1900-01-01 00:00:00");
            if (ViewState["gid"] == null)//显示扣点详情时判断
            {
                if (txtStartDate.Value.Trim() == "")
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "", "showdialog('请选择有效开始时间');", true);
                    return;
                }
                if (txtEndDate.Value.Trim() == "")
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "", "showdialog('请选择有效结束时间');", true);
                    return;
                }

                try
                {
                    dtStartDate = DateTime.Parse(txtStartDate.Value);
                    dtEndDate = DateTime.Parse(txtEndDate.Value);
                }
                catch (Exception)
                {

                    ScriptManager.RegisterStartupScript(this, GetType(), "", "showdialog('有效时间格式错误');", true);
                    return;
                }
            }



            //接口
            string policySource = "";
            if (basetype == "2")
            {
                ListItemCollection itemck = cbljk.Items;
                for (int i = 0; i < itemck.Count; i++)
                {
                    if (itemck[i].Selected)
                    {
                        policySource += itemck[i].Value + ",";
                    }
                }
                policySource = policySource.TrimEnd(',');
                if (policySource.Length <= 0)
                {

                    ScriptManager.RegisterStartupScript(this, GetType(), "", "showdialog('请选择接口');", true);
                    return;
                }
            }
            else
            {
                policySource = "0";
            }
            //判断此航段是否重复
            string[] policySources = policySource.Split(',');
            string[] fromcitycodes = strFromCityCode.Split('/');
            string[] tocitycodes = strToCityCode.Split('/');
            List<Tb_Ticket_TakeOffDetail> listdetail = null;

            if (ViewState["addgroupid"] != null)//添加时候判断重复
            {

                string sqlwhere1 = " 1=1 "
                    + " and CpyNo='" + mCompany.UninCode + "'"
                    + " and GroupId='" + ViewState["addgroupid"] + "'";
                if (selecttype=="3")
                {
                    sqlwhere1 += " and SelectType=3";
                }
                else
                {
                    sqlwhere1 += " and SelectType<>3";
                }

                listdetail = this.baseDataManage.CallMethod("Tb_Ticket_TakeOffDetail", "GetList", null, new object[] { sqlwhere1 }) as List<Tb_Ticket_TakeOffDetail>;

            }
            if (ViewState["gdid"] != null && ViewState["groupid"] != null)//修改时判断重复
            {
                string sqlwhere2 = " 1=1 "
                    + " and CpyNo='" + mCompany.UninCode + "'"
                    + " and GroupId='" + ViewState["groupid"] + "'"
                    + " and id <> '" + ViewState["gdid"] + "'";
                if (selecttype == "3")
                {
                    sqlwhere2 += " and SelectType=3";
                }
                else
                {
                    sqlwhere2 += " and SelectType<>3";
                }
                listdetail = this.baseDataManage.CallMethod("Tb_Ticket_TakeOffDetail", "GetList", null, new object[] { sqlwhere2 }) as List<Tb_Ticket_TakeOffDetail>;
            }
            if (ViewState["addgroupid"] != null || (ViewState["gdid"] != null && ViewState["groupid"] != null))
            {
                for (int i = 0; i < listdetail.Count; i++)//循环已经存在的扣点明细表
                {
                    string CarryCodeError = "";
                    if (listdetail[i].CarryCode.Contains(strCarryCode)
                        || listdetail[i].CarryCode == "ALL"
                        || strCarryCode == "ALL"
                        )//判断承运人/调整类型
                    {
                        CarryCodeError = "承运人有重复.";
                        for (int j = 0; j < fromcitycodes.Length; j++)//循环出发城市
                        {
                            if (fromcitycodes[j].ToString() == "")
                            {
                                continue;
                            }
                            string fromcityError = "";

                            if (listdetail[i].FromCityCode.Contains(fromcitycodes[j].ToString())//判断出发城市
                                || listdetail[i].FromCityCode.Contains("ALL")
                                || fromcitycodes[j].ToString().Contains("ALL"))
                            {

                                fromcityError = "出发城市有重复.";
                                for (int k = 0; k < tocitycodes.Length; k++)//循环到达城市
                                {
                                    if (tocitycodes[k].ToString() == "")
                                    {
                                        continue;
                                    }
                                    string tocityError = "";
                                    if (listdetail[i].ToCityCode.Contains(tocitycodes[k].ToString())//判断到达城市
                                        || listdetail[i].ToCityCode.Contains("ALL")
                                        || tocitycodes[k].ToString().Contains("ALL"))
                                    {
                                        tocityError = "到达城市有重复.";
                                        string basetypeError = "";
                                        if (basetype == listdetail[i].BaseType.ToString())//判断扣点类型
                                        {
                                            string timeError = "";
                                            DateTime dtStartDateOld;
                                            DateTime dtEndDateOld;
                                            try
                                            {
                                                dtStartDateOld = DateTime.Parse(listdetail[i].TimeScope.Split('|')[0].ToString());
                                                dtEndDateOld = DateTime.Parse(listdetail[i].TimeScope.Split('|')[1].ToString());
                                            }
                                            catch (Exception)
                                            {
                                                //格式发生错误,直接给默认时间
                                                dtStartDateOld = DateTime.Parse("1990-01-01 00:00:01");
                                                dtEndDateOld = DateTime.Parse("1990-01-01 00:00:01");
                                            }

                                            if ((dtStartDate >= dtStartDateOld && dtStartDate <= dtEndDateOld) || (dtEndDate >= dtStartDateOld && dtEndDate <= dtEndDateOld))
                                            {
                                                timeError = "有效时间段内有重复.";
                                                if (basetype == "2")
                                                {
                                                    for (int l = 0; l < policySources.Length; l++)//循环接口
                                                    {
                                                        if (listdetail[i].PolicySource.Contains(policySources[l].ToString()))//判断接口
                                                        {
                                                            basetypeError = "接口来源有重复.";
                                                            string errorAll = CarryCodeError + fromcityError + tocityError + basetypeError + timeError;
                                                            ScriptManager.RegisterStartupScript(this, GetType(), "", "showdialog('此航段在本组已存在," + errorAll + "');", true);
                                                            return;
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    basetypeError = "扣点类型有重复.";
                                                    string errorAll = CarryCodeError + fromcityError + tocityError + basetypeError + timeError;
                                                    ScriptManager.RegisterStartupScript(this, GetType(), "", "showdialog('此航段在本组已存在," + errorAll + "');", true);
                                                    return;
                                                }

                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            //点数范围
            for (int i = 0; i < int.Parse(hidtxtCount.Value); i++)
            {
                txtA = ((TextBox)this.FindControl("txtA" + i));
                txtB = ((TextBox)this.FindControl("txtB" + i));
                txtP = ((TextBox)this.FindControl("txtpoint" + i));//扣点
                txtM = ((TextBox)this.FindControl("txtMoney" + i));//现返
                string txtpvalue = txtP.Text.Trim().Length == 0 ? "0" : txtP.Text.Trim();
                string txtmvalue = txtM.Text.Trim().Length == 0 ? "0" : txtM.Text.Trim();
                pointscope += txtA.Text.Trim() + "-" + txtB.Text.Trim() + "^" + txtpvalue + "^" + txtmvalue + "|";
            }
            pointscope = pointscope.TrimEnd('|');
            //日志
            Log_Operation logoper = new Log_Operation();
            logoper.ModuleName = "扣点组管理";
            logoper.LoginName = mUser.LoginName;
            logoper.UserName = mUser.UserName;
            logoper.CreateTime = Convert.ToDateTime(DateTime.Now);
            logoper.CpyNo = mCompany.UninCode;
            //修改
            if (ViewState["gid"] != null || (ViewState["gdid"] != null && ViewState["groupid"] != null))
            {
                string sql = "";
                if (ViewState["gid"] != null)
                {
                    sql = "update Tb_Ticket_StrategyGroup set OperTime='" + DateTime.Now + "',OperLoginName='" + mUser.LoginName + "',OperUserName='" + mUser.UserName + "',DefaultFlag='" + rblDefaultFlag.SelectedValue + "',UniteFlag=" + rblUniteFlag.SelectedValue + ",UnitePoint='" + txtPoint + "' where id='" + ViewState["gid"].ToString() + "'";
                    logoper.OperateType = "修改扣点组";
                    logoper.OptContent = "修改扣点组id:" + ViewState["gid"].ToString() + ",OperTime='" + DateTime.Now + "',OperLoginName='" + mUser.LoginName + "',OperUserName='" + mUser.UserName + "',DefaultFlag='" + rblDefaultFlag.SelectedValue + "',UniteFlag=" + rblUniteFlag.SelectedValue + ",UnitePoint='" + txtPoint + "'";
                }
                else
                {
                    sql = "update Tb_Ticket_TakeOffDetail set OperTime='" + DateTime.Now + "',OperLoginName='" + mUser.LoginName + "',OperUserName='" + mUser.UserName + "',BaseType=" + int.Parse(ddlbasetype.SelectedValue) + ",PolicySource='" + policySource + "',CarryCode='" + strCarryCode + "',FromCityCode='/" + strFromCityCode + "',ToCityCode='/" + strToCityCode + "',TimeScope='" + txtStartDate.Value + "|" + txtEndDate.Value + "',PointScope='" + pointscope + "',SelectType=" + int.Parse(rblSelectType.SelectedValue) + " where id='" + ViewState["gdid"].ToString() + "'";
                    logoper.OperateType = "修改扣点组明细";
                    logoper.OptContent = "修改扣点组明细OperTime='" + DateTime.Now + "',OperLoginName='" + mUser.LoginName + "',OperUserName='" + mUser.UserName + "',BaseType=" + int.Parse(ddlbasetype.SelectedValue) + ",PolicySource='" + policySource + "',CarryCode='" + strCarryCode + "',FromCityCode='/" + strFromCityCode + "',ToCityCode='/" + strToCityCode + "',TimeScope='" + txtStartDate.Value + "|" + txtEndDate.Value + "',PointScope='" + pointscope + "',SelectType=" + int.Parse(rblSelectType.SelectedValue);
                }
                sqllist.Add(sql);
                string errormsg = "修改扣点组出错";
                if (baseDataManage.ExecuteSqlTran(sqllist, out errormsg))
                {
                    msg = "修改成功";
                    new PbProject.Logic.Log.Log_OperationBLL().InsertLog_Operation(logoper);//添加日志
                }
                else
                {
                    msg = "修改失败";
                }
            }
            else//添加
            {
                List<Tb_Ticket_StrategyGroup> list = null;
                Guid gid = Guid.NewGuid();
                //判断是否从详情页面连接过来做添加
                if (ViewState["addgroupid"] != null)
                {
                    gid = Guid.Parse(ViewState["addgroupid"].ToString());
                }
                else
                {
                    list = this.baseDataManage.CallMethod("Tb_Ticket_StrategyGroup", "GetList", null, new object[] { "GroupName='" + txtGroupName.Text.Trim() + "' and CpyNo='" + mCompany.UninCode + "'" }) as List<Tb_Ticket_StrategyGroup>;
                    string sql1 = "Insert into Tb_Ticket_StrategyGroup(id,CpyNo,CpyName,CpyType,OperTime,OperLoginName,OperUserName,GroupName,DefaultFlag,UniteFlag,UnitePoint) " +
                    "values('" + gid + "','" + mCompany.UninCode + "','" + mCompany.UninAllName + "'," + mCompany.RoleType + ",'" + DateTime.Now + "','" + mUser.LoginName + "','" + mUser.UserName + "','" + txtGroupName.Text.Trim() + "','" + rblDefaultFlag.SelectedValue + "'," + rblUniteFlag.SelectedValue + ",'" + txtPoint + "')";
                    sqllist.Add(sql1);

                    logoper.OperateType = "添加扣点组";
                    logoper.OptContent = "组id:" + gid + ",组名:" + txtGroupName.Text.Trim();
                }
                if (list != null && list.Count > 0)
                {
                    msg = "此用户组已存在";
                }
                else
                {
                    string sql2 = "insert into Tb_Ticket_TakeOffDetail(CpyNo,CpyName,CpyType,OperTime,OperLoginName,OperUserName,GroupId,BaseType,PolicySource,CarryCode,FromCityCode,ToCityCode,TimeScope,PointScope,SelectType) " +
                        "values('" + mCompany.UninCode + "','" + mCompany.UninAllName + "'," + mCompany.RoleType + ",'" + DateTime.Now + "','" + mUser.LoginName + "','" + mUser.UserName + "','" + gid + "'," + int.Parse(ddlbasetype.SelectedValue) + ",'" + policySource + "','" + strCarryCode + "','/" + strFromCityCode + "','/" + strToCityCode + "','" + txtStartDate.Value + "|" + txtEndDate.Value + "','" + pointscope + "'," + int.Parse(rblSelectType.SelectedValue) + ")";
                    sqllist.Add(sql2);
                    string errormsg = "添加扣点组出错";
                    logoper.OperateType = "添加扣点组明细";
                    logoper.OptContent = "出发城市:" + strFromCityCode + ",到达城市:" + strFromCityCode + ",承运人:" + strCarryCode + ",扣点范围:" + pointscope;
                    if (baseDataManage.ExecuteSqlTran(sqllist, out errormsg))
                    {
                        msg = "添加成功";
                        new PbProject.Logic.Log.Log_OperationBLL().InsertLog_Operation(logoper);//添加日志
                    }
                    else
                    {
                        msg = "添加失败";
                    }
                }
            }
        }
        catch (Exception)
        {
            msg = "操作出错";
        }
        ScriptManager.RegisterStartupScript(this, GetType(), "", "showdialog('" + msg + "');", true);
    }

}
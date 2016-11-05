using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using PbProject.Model;
using System.Xml.Linq;


/// <summary>
/// 在线还款
/// </summary>
public partial class Financial_OnlineRepayment : BasePage
{
    /// <summary>
    /// Page_Load
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            if (!IsPostBack)
            {
                BindPayType();
                CFTBankBind();
            }
        }
        catch (Exception ex)
        {

        }
    }
    /// <summary>
    /// 财付通信用卡大额支付银行绑定
    /// </summary>
    private void CFTBankBind()
    {
        string path = Server.MapPath("~/banklist.xml");
        XElement root = XElement.Load(path);
        var list = from x in root.Descendants("bank")
                   select new
                   {
                       ID = x.Attribute("num").Value,
                       Name = x.Attribute("name").Value,
                       Code = x.Attribute("code").Value
                   };
        r_bank_type40.DataSource = list;
        r_bank_type40.DataBind();

    }
    /// <summary>
    /// BindPayType
    /// </summary>
    private void BindPayType()
    {
        try
        {
            string gYcpyNo = mUser.CpyNo.Substring(0, 12);
            string wangYinZhangHao = PbProject.Model.definitionParam.paramsName.wangYinZhangHao;
            string wangYinLeiXing = PbProject.Model.definitionParam.paramsName.wangYinLeiXing;
            string sqlWhere = " CpyNo='" + gYcpyNo + "' and (SetName='" + wangYinZhangHao + "' or SetName='" + wangYinLeiXing + "')";
            List<Bd_Base_Parameters> bParametersList = new PbProject.Logic.ControlBase.Bd_Base_ParametersBLL().GetParametersListByWhere(sqlWhere);

           #region 测试数据

            /*List<Bd_Base_Parameters> bParametersList = new List<Bd_Base_Parameters>();
            Bd_Base_Parameters ts = new Bd_Base_Parameters();
            ts.SetName = wangYinZhangHao;
            ts.SetValue = "jianghui520you@126.com^jianghui520you@126.com^0.001|jianghui520you@126.com^jianghui520you@126.com^0.001|jianghui520you@126.com^jianghui520you@126.com^0.001|jianghui520you@126.com^jianghui520you@126.com^0.001|";
            bParametersList.Add(ts);

            Bd_Base_Parameters ts1 = new Bd_Base_Parameters();
            ts1.SetName = wangYinLeiXing;
            ts1.SetValue = "6";
            bParametersList.Add(ts1);
             */ 
	        #endregion
          
            Bd_Base_Parameters zhangHao = null;
            Bd_Base_Parameters leiXing = null;

            if (bParametersList != null && bParametersList.Count > 1)
            {
                foreach (var item in bParametersList)
                {
                    if (item.SetName == wangYinZhangHao)
                    {
                        zhangHao = item;
                    }
                    else if (item.SetName == wangYinLeiXing)
                    {
                        leiXing = item;
                    }
                }
                string temp = "";
                if (zhangHao != null && !string.IsNullOrEmpty(zhangHao.SetValue) && zhangHao.SetValue.Contains("|"))
                {
                    string[] setValues = zhangHao.SetValue.Split('|');
                    string[] setValue0 = setValues[0].Split('^');
                    if (!string.IsNullOrEmpty(setValue0[1]))
                    {
                        //支付宝
                        rblPayType.Items.Add(new ListItem("支付宝", "1"));
                        temp += "5,";
                    }
                    string[] setValue1 = setValues[1].Split('^');
                    if (!string.IsNullOrEmpty(setValue1[1]))
                    {
                        //快钱
                        rblPayType.Items.Add(new ListItem("快钱", "2"));
                        temp += "6,";
                    }
                    string[] setValue2 = setValues[2].Split('^');
                    if (!string.IsNullOrEmpty(setValue2[1]))
                    {
                        //汇付
                        rblPayType.Items.Add(new ListItem("汇付", "3"));
                        temp += "7,";
                    }
                    string[] setValue3 = setValues[3].Split('^');
                    if (!string.IsNullOrEmpty(setValue3[1]))
                    {
                        //财付通
                        rblPayType.Items.Add(new ListItem("财付通", "4"));
                        temp += "8,";
                        rblPayType.Items.Add(new ListItem("信用卡大额支付", "40"));
                    }
                }

                //判断网银
                if (leiXing != null && !string.IsNullOrEmpty(leiXing.SetValue) && leiXing.SetValue != "0")
                {
                    if (temp.Contains(leiXing.SetValue))
                    {
                        rblPayType.Items.Insert(0, new ListItem("网银", leiXing.SetValue));

                        hidWangYingType.Value = leiXing.SetValue;
                    }
                }

                if (rblPayType.Items.Count > 0)
                {
                    rblPayType.Items[0].Selected = true;
                    hidPayWay.Value = rblPayType.Items[0].Value; //支付方式
                    spanid.Visible = true;
                }
                else
                {
                    spanid.Visible = false;

                    Page.ClientScript.RegisterStartupScript(this.GetType(), DateTime.Now.Ticks.ToString(), "alert('不能进入该页面!');history.go(-1);", true);
                }
            }
        }
        catch (Exception)
        {

        }
    }

    /// <summary>
    /// 去支付
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnPayOk_Click(object sender, EventArgs e)
    {
         // 代码
        ////跳转支付页面
        Page.ClientScript.RegisterStartupScript(this.GetType(), DateTime.Now.Ticks.ToString(), "window.open(" + hid_url.Value + ", '_blank');", true);
    }
}
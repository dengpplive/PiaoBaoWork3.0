using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DataBase.Data;
using PbProject.Model;

public partial class Policy_PolicyGuide : BasePage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            //List<string> source = new List<string> { "517^517", "51book^51book", "百拓^BaiTuo", "票盟^PiaoMeng", "八千翼^8000Yi", "今日^JinRi" };
            //foreach (string item in source)
            //{
            //    lboxPlat.Items.Add(new ListItem(item.Split('^')[0], item.Split('^')[1]));
            //}
            BindGuide();
        }
    }
    /// <summary>
    /// 加载
    /// </summary>
    protected void BindGuide()
    {
        try
        {
            Bd_Base_Parameters mparameter = (baseDataManage.CallMethod("Bd_Base_Parameters", "GetList", null, new Object[] { "CpyNo='" + mCompany.UninCode + "' and SetName='Policy_Order'" }) as List<Bd_Base_Parameters>)[0];
            ViewState["id"] = mparameter.id.ToString();
            string[] setvalue = mparameter.SetValue.Split('|');
            string text = "";
            for (int i = 0; i < setvalue.Length; i++)
            {
                switch (setvalue[i].ToString())
                {
                    case "517":
                        text = "517";
                        break;
                    case "51book":
                        text = "51book";
                        break;
                    case "BaiTuo":
                        text = "百拓";
                        break;
                    case "PiaoMeng":
                        text = "票盟";
                        break;
                    case "8000Yi":
                        text = "八千翼";
                        break;
                    case "JinRi":
                        text = "今日";
                        break;

                }
                lboxPlat.Items.Add(new ListItem(text, setvalue[i].ToString()));
            }
        }
        catch (Exception)
        {

            throw;
        }
    }
    /// <summary>
    /// 向上
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btUp_Click(object sender, EventArgs e)
    {
        if (lboxPlat.SelectedIndex > 0)
        {
            string text = lboxPlat.SelectedItem.Text;
            string value = lboxPlat.SelectedItem.Value;
            int index = lboxPlat.SelectedIndex;
            lboxPlat.Items.RemoveAt(index);
            lboxPlat.Items.Insert(index - 1, new ListItem(text, value));
        }
    }
    /// <summary>
    /// 向下
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btDown_Click(object sender, EventArgs e)
    {
        if (lboxPlat.SelectedIndex >= 0 && lboxPlat.SelectedIndex != lboxPlat.Items.Count - 1)
        {
            string text = lboxPlat.SelectedItem.Text;
            string value = lboxPlat.SelectedItem.Value;
            int index = lboxPlat.SelectedIndex;
            lboxPlat.Items.RemoveAt(index);
            lboxPlat.Items.Insert(index + 1, new ListItem(text, value));
        }
    }
    /// <summary>
    /// 保存
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btSave_Click(object sender, EventArgs e)
    {
        IHashObject parameter = new HashObject();
        string rs = "", msg = "";
        try
        {
            for (int i = 0; i < lboxPlat.Items.Count; i++)
            {
                rs += lboxPlat.Items[i].Value + "|";
            }
            rs = rs.TrimEnd('|');
            parameter.Add("id", ViewState["id"].ToString());
            parameter.Add("SetValue", rs);
            msg = (bool)baseDataManage.CallMethod("Bd_Base_Parameters", "Update", null, new Object[] { parameter }) == true ? "更新成功" : "更新失败";
        }
        catch (Exception)
        {
            msg = "操作异常";
        }
        ScriptManager.RegisterStartupScript(this, GetType(), "", "showdialog('" + msg + "');", true);

    }
}
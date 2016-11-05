using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using PbProject.Model;
using PbProject.Logic.ControlBase;

public partial class Buy_interFaceSelectTest : BasePage
{
    BaseDataManage Manage = new BaseDataManage();
    protected void Page_Load(object sender, EventArgs e)
    {

    }
    public List<Tb_Ticket_Policy> getInterFacePolicy(string OrderID, User_Employees mUser, bool IsOrGetPolicy)
    {
        List<Tb_Ticket_Policy> list = null;
        try
        {
            if (IsOrGetPolicy)
            {

            }
            else
            {
                string sqlwhere = " 1=1 "
                                + "and OrderId='" + OrderID + "'";
                List<Tb_Ticket_Order> objList = Manage.CallMethod("Tb_Ticket_Order", "GetList", null, new object[] { sqlwhere }) as List<Tb_Ticket_Order>;
                if (objList != null)
                {
                    if (objList.Count > 0)
                    {
                        PbProject.Logic.PTInterface.AllInterface allInterface = new PbProject.Logic.PTInterface.AllInterface(objList[0], mUser);
                        list = allInterface.GetPolicyAll();
                    }
                }
            }
        }
        catch (Exception)
        {
            list = null;
        }
        var q =
      from p in list
      orderby p.DownPoint * 10000 + p.DownReturnMoney descending
      select p;
        list = q.ToList<Tb_Ticket_Policy>();
        return list;
    }
    protected void Button1_Click(object sender, EventArgs e)
    {
        GridView1.DataSource = getInterFacePolicy(TextBox1.Text, mUser, false);
        GridView1.DataBind();
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace PiaoBao.API.Test
{
	public partial class testArch : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
            
		}

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            ServiceClient sc = new ServiceClient("chenchuan11", "123321");
            var url = txtUri.Text;
            var data = txtData.Text;
            var met = txtMethod.Text;
            txtResult.Text = sc.Request(url, met, data);
        }
	}
}
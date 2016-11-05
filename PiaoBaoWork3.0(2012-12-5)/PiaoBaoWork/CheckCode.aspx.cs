using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.IO;

/// <summary>
/// 生成验证控件页
/// </summary>
public partial class CheckCode : System.Web.UI.Page
{

    protected void Page_Load(object sender, EventArgs e)
    {
        this.CreateCheckCodeImage(GenerateCheckCode());
    }

    /// <summary>
    /// 产生验证码字符串
    /// </summary>
    /// <returns></returns>
    public string GenerateCheckCode()
    {
        //验证码的字符数组
        //char[] s = new char[]{'1','2','3','4','5','6','7','8','9','a'
        //    ,'b','c','d','e','f','g','h','i','j','k','m','n','p','q'
        //    ,'r','s','t','u','v','w','x','y','z','A','B','C','D','E','F','G'
        //    ,'H','J','K','L','M','N','P','Q','R','S','T','U','V','W'
        //    ,'X','Y','Z'};
        char[] s = new char[] { '1', '2', '3', '4', '5', '6', '7', '8', '9' };
        string num = "";
        //创建一组随机数
        Random r = new Random();
        //随机循环创建，并构成验证码 
        for (int i = 0; i < 4; i++)
        {
            num += s[r.Next(0, s.Length)].ToString() + " ";
        }
        Session[new PbProject.Logic.SessionContent().CHECKCODE] = num.Trim().Replace(" ", "");
        return num;
    }
    private void CreateCheckCodeImage(string checkCode)
    {
        if (checkCode == null || checkCode.Trim() == String.Empty)
            return;

        System.Drawing.Bitmap image = new System.Drawing.Bitmap((int)Math.Ceiling((checkCode.Length * 8.0)), 20);
        Graphics g = Graphics.FromImage(image);

        try
        {
            //生成随机生成器
            Random random = new Random();

            //清空图片背景色
            g.Clear(Color.White);

            //画图片的背景噪音线
            for (int i = 0; i < 25; i++)
            {
                int x1 = random.Next(image.Width);
                int x2 = random.Next(image.Width);
                int y1 = random.Next(image.Height);
                int y2 = random.Next(image.Height);
                g.DrawLine(new Pen(Color.Silver), x1, y1, x2, y2);
            }

            Font font = new System.Drawing.Font("Arial", 12, (System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic));
            System.Drawing.Drawing2D.LinearGradientBrush brush = new System.Drawing.Drawing2D.LinearGradientBrush(new Rectangle(0, 0, image.Width, image.Height), Color.Blue, Color.DarkRed, 1.2f, true);
            g.DrawString(checkCode, font, brush, 2, 2);

            //画图片的前景噪音点
            for (int i = 0; i < 100; i++)
            {
                int x = random.Next(image.Width);
                int y = random.Next(image.Height);
                image.SetPixel(x, y, Color.FromArgb(random.Next()));
            }
            //画图片的边框线
            g.DrawRectangle(new Pen(Color.Silver), 0, 0, image.Width - 1, image.Height - 1);
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            image.Save(ms, System.Drawing.Imaging.ImageFormat.Gif);
            Response.ClearContent();
            Response.ContentType = "image/Gif";
            Response.BinaryWrite(ms.ToArray());
        }
        finally
        {
            g.Dispose();
            image.Dispose();
        }
    }
}



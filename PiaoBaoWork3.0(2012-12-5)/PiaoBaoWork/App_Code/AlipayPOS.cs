using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using System.Configuration;

namespace Com.Alipay
{
    /// <summary>
    ///AlipayPOS 的摘要说明
    /// </summary>
    public class AlipayPOS
    {
        //支付宝的公钥
        private string _Public_key = ConfigurationManager.AppSettings["Public_key"].ToString(); //@"MIGfMA0GCSqGSIb3DQEBAQUAA4GNADCBiQKBgQDTGmkuheyC/DjpFadnQmFjN42VXIibP9lZI7IfWStKwPohbXPp/Nn0zKbaetWMzYJxRf3yW6RL8q9cJCNsL//xDqeTCR6p5JWDXH8ySH0NSE0fSVOVuaoILim6rNWAOZue4+XhnXcgYJbqsW1AMtPrswoGlC1iPBTs1MAhCXfR4wIDAQAB";
        //商户的私钥
        private string _Private_key = ConfigurationManager.AppSettings["Private_key"].ToString(); // @"MIICeAIBADANBgkqhkiG9w0BAQEFAASCAmIwggJeAgEAAoGBALQUsM3nPWuWH59ia0aTGo2eU7g1hkC7ZD9TenWbp0wUAoKWEnNY7vNIBddR5TBmYdS5U3umbZpRLs/lT5BgftoNbp/dOIYzgWXu2aBrG9UI3YG5Bko1Aaw963NteIT2wReRgvhVzkCRBtFEkx8NswxK7zcLK1Xa8UBzv85HW3QdAgMBAAECgYBFm0nMDPncwCZxASXeHbZBz1Uk+utt0gEpySaolwXPjlx6BXAUTefu+YPoeqtQTgK6qvft76Kl54NksIpUkDf3gojii0NUkUDURGhj/gmOKsWnrTQtHSFs1TAJmCS2s6L6W/36VoOJY2spBT8x7mY33IJstJ1AtD9HHFIDjZZAMQJBAOfDj5JtwvKnHdzM8kIw1NsIlAaHntNKiAXC8c+yLhnKmlzzsSnDBiTOnRY8KxZ9tcbALxkigEYLA2BpSd2WCxMCQQDG6YwGqPh8GVOJxr6BtklsKoUiToreRiOL0056qgOrL1glzUN9EsnQdqc4pTh3gpBVl9YIVAxrtVVArnBeB7oPAkEAywGMN8m28g8Z5DcMmJfnSnhoGJQgtZjaLpnEb5X2NZc2wOagLyFpt0HXbQuE/m1clNvwJUcILtIkwgVXsyVjSwJBAJZ9dbpINo81XhWb7uAWOOCEHuvAvqHCMPyF5xc478Og8zDOZmQHRfbY/lUF832/o1GOqZjtdeQ7cf+Yulz5vZMCQQC3QAZ2jpYLqxcj8AgFjMEzcIAdEPEccrzVVmkr7fVvLCy6hdM76K76TJ1zIphrl5d/CG5wrQo/Z9kOxFG16lbU";

        public AlipayPOS()
        {
            //
            //TODO: 在此处添加构造函数逻辑
            //
        }

        #region XML操作


        /// <summary>
        /// 从报文中截取指定名称的节点内容（包含节点本身） 从报文中截取指定名称的节点内容（包含节点本身 <transaction_id>MI0005</transaction_id>）
        /// </summary>
        /// <param name="strXML">报文文本</param>
        /// <param name="nodeName">节点名</param>
        /// <returns>节点内容</returns>
        public String getXMLByNodeName(String strXML, String nodeName)
        {
            String openNode = "<" + nodeName + ">";
            String closeNode = "</" + nodeName + ">";
            int startIdx = strXML.IndexOf(openNode);
            int endIdx = strXML.IndexOf(closeNode) + closeNode.Length - startIdx;
            if (startIdx == -1 || endIdx == -1)
            {
                return "找不到对应的节点名：" + nodeName;
            }
            else
            {
                return strXML.Substring(startIdx, endIdx);
            }
        }

        /// <summary>
        /// 移除报文中指定节点名的节点
        /// </summary>
        /// <param name="strXML">报文文本</param>
        /// <param name="nodeName">节点名</param>
        /// <returns>移除节点后的报文</returns>
        public String removeXMLNodeByName(String strXML, String nodeName)
        {
            String openNode = "<" + nodeName + ">";
            String closeNode = "</" + nodeName + ">";
            int startIdx = strXML.IndexOf(openNode);
            int endIdx = strXML.IndexOf(closeNode) + closeNode.Length;
            if (startIdx == -1 || endIdx == -1)
            {
                return "找不到对应的节点名：" + nodeName;
            }
            else
            {
                return strXML.Substring(0, startIdx) + strXML.Substring(endIdx);
            }
        }

        /// <summary>
        /// 从报文中截取指定名称的节点值（不包含节点本身）
        /// </summary>
        /// <param name="strXML">报文文本</param>
        /// <param name="nodeName">节点名</param>
        /// <returns>节点值</returns>
        public String getXMLNodeValue(String strXML, String nodeName)
        {
            String nodeXML = getXMLByNodeName(strXML, nodeName);
            int startIdx = nodeXML.IndexOf('>') + 1;
            int endIdx = nodeXML.LastIndexOf('<') - startIdx;
            if (startIdx == -1 || endIdx == -1)
            {
                return "找不到对应的节点名：" + nodeName;
            }
            else
            {
                return nodeXML.Substring(startIdx, endIdx);
            }
        }

        #endregion

        #region 签名操作

        /// <summary>
        /// 签名--动态读取XML，解析XML数据，返回签名
        /// </summary>
        /// <param name="strXML"></param>
        /// <returns></returns>
        public string RSASignFromXML(string strXML)
        {
            return RSAFromPkcs8.sign(new AlipayPOS().SignString(strXML), _Private_key);
        }

        /// <summary>
        /// 签名字符串组合
        /// </summary>
        /// <param name="strXML"></param>
        /// <returns></returns>
        private string SignString(string strXML)
        {
            string strSignString = "";
            strXML = strXML.Substring(strXML.IndexOf("?>") + 2);
            strXML = strXML.Replace("<Transaction>", "");
            strXML = strXML.Replace("</Transaction>", "");
            strXML = strXML.Replace("<Transaction_Header>", "");
            strXML = strXML.Replace("</Transaction_Header>", "");
            strXML = strXML.Replace("<Transaction_Body>", "");
            strXML = strXML.Replace("</Transaction_Body>", "");
            strSignString = removeXMLNodeByName(strXML, "MAC");
            return strSignString;
        }
        #endregion

        #region 加密

        /// <summary>
        /// 不对齐MD5加密
        /// </summary>
        /// <param name="password">要加密的密码</param>
        /// <returns>加密后的密码</returns>
        public string NotAlignmentMD5(string password)
        {
            string newPassword = password;
            //去除基数位0
            //for (int i = 0; i < password.Length; i++)
            //{
            //    //判断是否基数位
            //    if ((i + 1) % 2 == 0 || ((i + 1) % 2 != 0 && password.Substring(i, 1) != "0"))
            //    {
            //        newPassword = newPassword + password.Substring(i, 1);
            //    }
            //}

            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] t = md5.ComputeHash(Encoding.GetEncoding("utf-8").GetBytes(newPassword));
            StringBuilder sb = new StringBuilder(32);
            for (int i = 0; i < t.Length; i++)
            {
                sb.Append(t[i].ToString("x").PadLeft(2, '0'));
            }

            password = sb.ToString().ToUpper();
            newPassword = "";

            for (int i = 0; i < password.Length; i++)
            {
                //判断是否基数位
                if ((i + 1) % 2 == 0 || ((i + 1) % 2 != 0 && password.Substring(i, 1) != "0"))
                {
                    newPassword = newPassword + password.Substring(i, 1);
                }
            }
            return newPassword;
        }

        #endregion 
    }
}
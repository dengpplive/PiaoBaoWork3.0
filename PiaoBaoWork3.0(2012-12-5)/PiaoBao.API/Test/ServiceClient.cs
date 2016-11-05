using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

namespace PiaoBao.API.Test
{
    class ServiceClient
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public ServiceClient(string username, string password)
        {
            this.Username = username;
            this.Password = password;
        }
        public string Request(string url, string method, string data)
        {
            return Request(url, method, data, Encoding.UTF8);
        }

        public string Request(string url, string method, string data, Encoding encoding)
        {
            return doRequest(Username, Password, url, method, data, encoding);
        }

        private string doRequest(string username, string password, string url, string method, string data, Encoding encoding)
        {
            method = method.ToUpper();
            if (encoding == null)
                encoding = Encoding.UTF8;
            string doMethod;
            switch (method)
            {
                case "GET":
                    doMethod = "GET";
                    break;
                case "PUT":
                    doMethod = "POST";
                    break;
                case "DELETE":
                    doMethod = "GET";
                    break;
                case "POST":
                    doMethod = "POST";
                    break;
                default:
                    doMethod = "GET";
                    break;

            }
            var b = encoding.GetBytes(data);
            if (doMethod == "GET")
            {
                if (!string.IsNullOrEmpty(data))
                    url = url + "?" + data;
            }

            HttpWebRequest s = WebRequest.Create(url) as HttpWebRequest;
            //添加身份信息
            s.Headers.Add("Authentication", username + "," + password);
            s.Headers.Add("Method", method);

            s.Method = doMethod;
            if (doMethod != "GET")
            {
                s.ContentLength = b.Length;
                s.ContentType = "application/x-www-form-urlencoded";
                var req = s.GetRequestStream();
                req.Write(b, 0, b.Length);
                req.Close();
            }
            HttpWebResponse myResponse;
            try
            {
                myResponse = (HttpWebResponse)s.GetResponse();
            }
            catch (WebException ex)
            {
                myResponse = ex.Response as HttpWebResponse;
            }
            System.IO.StreamReader reader = new System.IO.StreamReader(myResponse.GetResponseStream(), encoding);
            string content = reader.ReadToEnd();
            return content;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PiaoBao.Arch.RestfulWebServices;

namespace PiaoBao.API.Services
{
    public class TestArch:BaseServices
    {
        [Authenticate]
        public override void Query(ResponseWriter writer, System.Collections.Specialized.NameValueCollection parames)
        {
            var user = AuthLogin.GetUserInfo(Username);

            writer.Write("GET提交 查询资源");
        }
        public override void Create(ResponseWriter writer, System.Collections.Specialized.NameValueCollection parames)
        {
            writer.Write("POST提交 添加资源");
        }
        public override void Delete(ResponseWriter writer, System.Collections.Specialized.NameValueCollection parames)
        {
            writer.Write("Delete提交 删除资源");
        }
        public override void Update(ResponseWriter writer, System.Collections.Specialized.NameValueCollection parames)
        {
            writer.Write("PUT提交 修改资源");
        }
    }
}
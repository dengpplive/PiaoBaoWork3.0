using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataBase.Data;
using DataBase;

namespace PbProject.Dal.Log
{
   public class Log_OperationDal
    {
       /// <summary>
       /// 添加操作日志
       /// </summary>
       /// <param name="parameter"></param>
       /// <returns></returns>
       public static int Insert(IHashObject parameter)
       {
           using (DbHelper db = new DbHelper(DbControl.GetDbConnStringBasic(DbHandleBehaviorDefine.WRITE, 0)))
           {
               int count = db.Insert("Log_Operation", parameter);
               if (count > 0)
               {
                   return count;
               }
               else
               {
                   return 0;
               }
           }
       }
    }
}

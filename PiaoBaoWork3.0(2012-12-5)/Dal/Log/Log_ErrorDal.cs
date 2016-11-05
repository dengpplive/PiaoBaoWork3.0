using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PbProject.Model;
using DataBase;
using PbProject.Dal.Mapping;
using System.Data;
using PbProject.Cache;
using DataBase.Data;

namespace PbProject.Dal.Log
{
    public class Log_ErrorDal
    {
        //public static int UpdateById(IHashObject parameter)
        //{
        //    using (DbHelper db = new DbHelper(DbControl.GetDbConnStringBasic(DbHandleBehaviorDefine.UPDATE, 0)))
        //    {
        //        int count = db.Update("Log_Error", "id", parameter);
        //        return count;
        //    }
        //}

        //public int DeleteById(long id)
        //{
        //    using (DbHelper db = new DbHelper(DbControl.GetDbConnStringBasic(DbHandleBehaviorDefine.DELETE, 0)))
        //    {
        //        int count = db.Delete("Log_Error", "id", id);
        //        return count;
        //    }
        //}
        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public static int Insert(IHashObject parameter)
        {
            using (DbHelper db = new DbHelper(DbControl.GetDbConnStringBasic(DbHandleBehaviorDefine.WRITE, 0)))
            {
               int count = db.Insert("Log_Error", parameter);
               if (count>0)
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

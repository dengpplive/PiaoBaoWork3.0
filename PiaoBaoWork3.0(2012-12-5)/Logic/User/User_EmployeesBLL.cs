using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PbProject.Model;
using System.Data;
using DataBase.Data;

namespace PbProject.Logic.User
{
    /// <summary>
    /// 用户信息
    /// </summary>
    public class User_EmployeesBLL
    {

        PbProject.Logic.ControlBase.BaseDataManage baseDataManage = new PbProject.Logic.ControlBase.BaseDataManage();

        /// <summary>
        /// 判断员工登录账号唯一
        /// </summary>
        /// <param name="LoginName"></param>
        /// <returns></returns>
        public List<User_Employees> GetListByLoginName(string LoginName)
        {
            // List<User_Employees> reList  =  new Dal.ControlBase.BaseData<User_Employees>().GetList();
            // List<User_Employees> ListBack = new List<User_Employees>();
            // foreach (User_Employees item in reList)
            //{
            //    if (item.LoginName == LoginName)
            //    {
            //        ListBack.Add(item);
            //    }
            //}
            // return ListBack;

            return baseDataManage.CallMethod("User_Employees", "GetList", null, new Object[] { "LoginName='" + LoginName + "'" }) as List<User_Employees>;
        }

        /// <summary>
        /// 添加员工
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public bool Insert(IHashObject parameter)
        {
            return new Dal.ControlBase.BaseData<User_Employees>().Insert(parameter);
        }


        /// <summary>
        /// 用户页面权限
        /// </summary>
        /// <param name="id">登录用户页面权限 ID</param>
        /// <returns></returns>
        public User_Employees GetById(string id)
        {
            return new Dal.ControlBase.BaseData<User_Employees>().GetById(id);
        }

        /// <summary>
        /// 用户信息修改
        /// </summary>
        /// <param name="parameter">修改信息</param>
        /// <returns></returns>
        public bool UpdateById(IHashObject parameter)
        {
            return new Dal.ControlBase.BaseData<User_Employees>().Update(parameter);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="strwhere"></param>
        /// <returns></returns>
        public List<User_Employees> GetBySQLList(string strwhere)
        {
            //return new Dal.ControlBase.BaseData<User_Employees>().GetBySQLList(strwhere);
            return baseDataManage.CallMethod("User_Employees", "GetList", null, new Object[] { strwhere }) as List<User_Employees>;
        }
    }
}
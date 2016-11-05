using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PbProject.Model;
using DataBase.Data;
using System.Data;

namespace PbProject.Logic.User
{
    /// <summary>
    /// 用户公司信息
    /// </summary>
    public class User_CompanyBLL
    {

        PbProject.Logic.ControlBase.BaseDataManage baseDataManage = new PbProject.Logic.ControlBase.BaseDataManage();

        ///// <summary>
        ///// 用户公司信息
        ///// </summary>
        ///// <param name="UninCode"></param>
        //public User_Company GetByUninCode(string UninCode)
        //{
        //    return new Dal.User.User_CompanyDal(true).GetByUninCode(UninCode);
        //}

        /// <summary>
        /// 添加/修改商家
        /// </summary>
        /// <param name="parameter">参数</param>
        /// <param name="parameter">执行类型</param>
        /// <returns></returns>
        public int uporinAccount(IHashObject parameter,int type)
        {
            return new PbProject.Logic.SQLEXBLL.SQLEXBLL_Base().uporinAccount(parameter, type);
        }

        /// <summary>
        /// 获取指定角色的公司
        /// </summary>
        /// <param name="RoleType"></param>
        /// <returns></returns>
        public List<User_Company> GetListByRoleType(int RoleType)
        {
            //List<User_Company> reList = new List<User_Company>();

            //List<User_Company> reLists = new Dal.ControlBase.BaseData<User_Company>().GetList();

            //foreach (User_Company item in reLists)
            //{
            //    if (item.RoleType == RoleType)
            //    {
            //        reList.Add(item);
            //    }
            //}


            List<User_Company> reList = baseDataManage.CallMethod("User_Company", "GetList", null, new Object[] { "RoleType=" + RoleType }) as List<User_Company>;
            return reList;
        }

        /// <summary>
        /// 获取公司信息
        /// </summary>
        /// <param name="sqlWhere">查询条件</param>
        /// <returns></returns>
        public List<User_Company> GetListBySqlWhere(string sqlWhere)
        {
            //List<User_Company> reList = new List<User_Company>();

            //List<User_Company> reLists = new Dal.ControlBase.BaseData<User_Company>().GetList();

            //foreach (User_Company item in reLists)
            //{
            //    if (item.RoleType == RoleType)
            //    {
            //        reList.Add(item);
            //    }
            //}


            List<User_Company> reList = baseDataManage.CallMethod("User_Company", "GetList", null, new Object[] { sqlWhere }) as List<User_Company>;
            return reList;
        }

        /// <summary>
        /// 获取用户信息
        /// </summary>
        /// <param name="sqlWhere"></param>
        /// <returns></returns>
        public DataTable GetUser_Company_Empolyees(string sqlWhere)
        {
           return new PbProject.Dal.SQLEXDAL.SQLEXDAL_Base().GetUser_Company_Empolyees(sqlWhere);
        }

        /// <summary>
        /// 判断公司是否存在
        /// </summary>
        /// <param name="RoleType"></param>
        /// <returns></returns>
        public List<User_Company> GetListByCpyName(string CpyName)
        {
            //List<User_Company> reLists = new Dal.ControlBase.BaseData<User_Company>().GetList();
            //List<User_Company> reList = new List<User_Company>();

            //foreach (User_Company item in reLists)
            //{
            //    if (item.UninAllName == CpyName)
            //    {
            //        reList.Add(item);
            //    }
            //}
            // return reList;

            return baseDataManage.CallMethod("User_Company", "GetList", null, new Object[] { "UninAllName='" + CpyName + "'" }) as List<User_Company>;
        }

       
        /// <summary>
        /// 根据公司id修改信息
        /// </summary>
        /// <param name="paramter"></param>
        /// <returns></returns>
        public bool UpdateById(IHashObject paramter)
        {
           return new Dal.ControlBase.BaseData<User_Company>().Update(paramter);
        }

        /// <summary>
        /// 根据公司编号获取公司信息
        /// </summary>
        /// <param name="CpyNo">公司编号</param>
        /// <returns></returns>
        public PbProject.Model.User_Company GetCompany(string CpyNo)
        {
            User_Company uCompany = null;

            try
            {
                string sqlWhere = " UninCode='" + CpyNo + "'";
                List<User_Company> uCompanyList = GetListBySqlWhere(sqlWhere);

                if (uCompanyList != null && uCompanyList.Count > 0)
                    uCompany = uCompanyList[0];
            }
            catch (Exception)
            {

            }
            return uCompany;
        }
    }
}

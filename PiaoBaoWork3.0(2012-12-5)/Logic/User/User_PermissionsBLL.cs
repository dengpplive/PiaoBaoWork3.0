using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PbProject.Model;
using PbProject.Logic.ControlBase;
using DataBase.Data;

namespace PbProject.Logic.User
{
    /// <summary>
    /// 用户页面权限
    /// </summary>
    public class User_PermissionsBLL
    {

        PbProject.Logic.ControlBase.BaseDataManage baseDataManage = new PbProject.Logic.ControlBase.BaseDataManage();

        /// <summary>
        /// 用户页面权限
        /// </summary>
        /// <param name="id">登录用户页面权限 ID</param>
        /// <returns></returns>
        public User_Permissions GetById(string id)
        {
            return new Dal.ControlBase.BaseData<User_Permissions>().GetById(id);
        }

        /// <summary>
        /// 更新用户角色缓存
        /// </summary>
        /// <param name="roleType">用户角色类型</param>
        /// <returns></returns>
        public bool Update_User_Permissions(int roleType)
        {
            bool reuslt = false;
            try
            {
                reuslt = new PbProject.Dal.SQLEXDAL.SQLEXDAL_Base().Update_User_Permissions(roleType) > -1 ? true : false;

                //if (reuslt == true)
                //{
                //    //清理页面权限缓存
                //    new BaseDataManage().CallMethod("User_Permissions", "RefreshCache", null, new object[] { });
                //}
            }
            catch (Exception)
            {

            }
            return reuslt;
        }

        /// <summary>
        /// 通过公司编号 获取所有页面权限
        /// </summary>
        /// <param name="cpyNo">公司编号</param>
        /// <returns></returns>
        public List<User_Permissions> GetListByCpyNo(string cpyNo)
        {
            List<User_Permissions> uPermissionslist = null;
            try
            {
                //uPermissionslist = new Dal.ControlBase.BaseData<User_Permissions>().GetList();
                //if (uPermissionslist != null && uPermissionslist.Count > 0)
                  //  uPermissionslist = uPermissionslist.Where(w => w.CpyNo == cpyNo).ToList<User_Permissions>();

                uPermissionslist = baseDataManage.CallMethod("User_Permissions", "GetList", null, new Object[] { "CpyNo='" + cpyNo + "'" }) as List<User_Permissions>;
            }
            catch (Exception)
            {

            }
            return uPermissionslist;
        }


        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="id">id</param>
        /// <returns></returns>
        public List<User_Permissions> GetListById(Guid id)
        {
            //List<User_Permissions> bPaseList = new List<User_Permissions>();
            //try
            //{
            //    bPaseList = new Dal.ControlBase.BaseData<User_Permissions>().GetList();
            //    bPaseList = bPaseList.Where(c => c.id == id).ToList<User_Permissions>();

            //    //IEnumerable<Bd_Base_Page> querys = null;
            //    //querys = from c in bPaseList where c.RoleType == roleType select c;
            //    //bPaseNewList = querys.ToList<Bd_Base_Page>();
            //}
            //catch (Exception)
            //{
            //}
            //return bPaseList;

            return baseDataManage.CallMethod("User_Permissions", "GetList", null, new Object[] { "id='" + id.ToString() + "'" }) as List<User_Permissions>;
        }
    }
}
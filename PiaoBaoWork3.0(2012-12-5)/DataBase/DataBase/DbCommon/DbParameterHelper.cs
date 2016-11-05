namespace DataBase.DbCommon
{
    using DataBase;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;
    using System.Data.SqlClient;
    using System.Runtime.InteropServices;

    internal static class DbParameterHelper
    {
        private static Hashtable dbCaches = new Hashtable();

        private static List<SqlParameter> CloneParamters(ICollection parameters)
        {
            List<SqlParameter> list = new List<SqlParameter>();
            foreach (SqlParameter parameter in parameters)
            {
                SqlParameter item = new SqlParameter(parameter.ParameterName, parameter.SqlDbType, parameter.Size) {
                    Direction = parameter.Direction,
                    Precision = parameter.Precision,
                    Scale = parameter.Scale
                };
                list.Add(item);
            }
            return list;
        }

        public static void DeriveParamters(DbHelper helper)
        {
            CommandType commandType = helper.Command.CommandType;
            if (commandType != CommandType.Text)
            {
                if (commandType != CommandType.StoredProcedure)
                {
                    if (commandType == CommandType.TableDirect)
                    {
                        throw new InvalidOperationException("当CommandType为TableDirect时不能有参数！");
                    }
                    return;
                }
            }
            else
            {
                DeriveSQLParameters();
                return;
            }
            DeriveSPParameters(helper.Command);
        }

        private static void DeriveSPParameters(DbCommand cmd)
        {
            bool flag;
            List<SqlParameter> parameters = GetSPParameters(GetSPCaches(cmd.Connection.ConnectionString), cmd, out flag);
            if (!flag)
            {
                cmd.Parameters.AddRange(CloneParamters(parameters).ToArray());
            }
        }

        private static void DeriveSQLParameters()
        {
            throw new InvalidOperationException("暂时不支持！");
        }

        private static Hashtable GetSPCaches(string dbKey)
        {
            Hashtable hashtable = (Hashtable) dbCaches[dbKey];
            if (hashtable == null)
            {
                lock (dbCaches.SyncRoot)
                {
                    hashtable = (Hashtable) dbCaches[dbKey];
                    if (hashtable == null)
                    {
                        hashtable = new Hashtable();
                        dbCaches[dbKey] = hashtable;
                        return hashtable;
                    }
                }
            }
            return hashtable;
        }

        private static List<SqlParameter> GetSPParameters(Hashtable spCaches, DbCommand cmd, out bool firstDerive)
        {
            string commandText = cmd.CommandText;
            List<SqlParameter> list = (List<SqlParameter>) spCaches[commandText];
            firstDerive = false;
            if (list == null)
            {
                lock (spCaches.SyncRoot)
                {
                    list = (List<SqlParameter>) spCaches[commandText];
                    if (list != null)
                    {
                        return list;
                    }
                    SqlCommandBuilder.DeriveParameters((SqlCommand) cmd);
                    if (cmd.Parameters[0].ParameterName == "@RETURN_VALUE")
                    {
                        cmd.Parameters.RemoveAt(0);
                    }
                    list = new List<SqlParameter>();
                    list = CloneParamters(cmd.Parameters);
                    spCaches[commandText] = list;
                    firstDerive = true;
                }
            }
            return list;
        }
    }
}


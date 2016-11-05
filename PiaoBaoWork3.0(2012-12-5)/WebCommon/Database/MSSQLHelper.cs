using System;
using System.Reflection;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace WebCommon.Database
{
    public class MSSQLHelper
    {
        private SqlConnection conn;
        private SqlCommand cmd;

        public MSSQLHelper(string connString)
        {
            conn = new System.Data.SqlClient.SqlConnection();
            conn.ConnectionString = connString;

            try
            {
                conn.Open();
            }
            catch
            {
                conn.Close();
                throw;
            }
        }

        public void Dispose()
        {
            if (conn.State != ConnectionState.Closed)
            {
                conn.Close();
            }

            conn.Dispose();
        }

        /// <summary>
        /// Command
        /// </summary>
        /// <param name="trans"></param>
        /// <param name="cmdType"></param>
        /// <param name="cmdText"></param>
        /// <param name="cmdParams"></param>
        private void PrepareCommand(SqlTransaction trans, CommandType cmdType, string cmdText, SqlParameter[] cmdParams)
        {
            cmd.CommandText = cmdText;

            if (trans != null)
            {
                cmd.Transaction = trans;
            }

            cmd.CommandType = cmdType;

            if (cmdParams != null)
            {
                foreach (SqlParameter param in cmdParams)
                {
                    cmd.Parameters.Add(param);
                }
            }
        }

        /// <summary>
        /// Return Int
        /// </summary>
        /// <param name="cmdType"></param>
        /// <param name="cmdText"></param>
        /// <param name="commandParameters"></param>
        /// <returns></returns>
        public int ExecuteNonQuery(CommandType cmdType, string cmdText, params SqlParameter[] commandParameters)
        {
            cmd = conn.CreateCommand();
            try
            {
                PrepareCommand(null, cmdType, cmdText, commandParameters);

                int retVal = cmd.ExecuteNonQuery();
                cmd.Parameters.Clear();
                cmd.Dispose();

                return retVal;
            }
            catch
            {
                Dispose();
                throw;
            }
        }

        /// <summary>
        /// Return SqlDataReader
        /// </summary>
        /// <param name="cmdType"></param>
        /// <param name="cmdText"></param>
        /// <param name="commandParameters"></param>
        /// <returns></returns>
        public SqlDataReader ExecuteReader(CommandType cmdType, string cmdText, params SqlParameter[] commandParameters)
        {
            cmd = conn.CreateCommand();

            // we use a try/catch here because if the method throws an exception we want to 
            // close the connection throw code, because no datareader will exist, hence the 
            // commandBehaviour.CloseConnection will not work

            try
            {
                PrepareCommand(null, cmdType, cmdText, commandParameters);
                SqlDataReader rdr = cmd.ExecuteReader();
                cmd.Parameters.Clear();
                cmd.Dispose();
                return rdr;
            }
            catch
            {
                Dispose();
                throw;
            }
        }

        /// <summary>
        /// Return DataTable
        /// </summary>
        /// <param name="cmdType"></param>
        /// <param name="cmdText"></param>
        /// <param name="commandParameters"></param>
        /// <returns></returns>
        public DataTable ExeSqlDataAdapter(CommandType cmdType, string cmdText, params SqlParameter[] commandParameters)
        {
            cmd = conn.CreateCommand();
            try
            {
                PrepareCommand(null, cmdType, cmdText, commandParameters);
                SqlDataAdapter objDataAdapter = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                objDataAdapter.Fill(dt);
                cmd.Parameters.Clear();
                cmd.Dispose();
                return dt;
            }
            catch
            {
                Dispose();
                throw;
            }
        }

        /// <summary>
        /// Return DataSet
        /// </summary>
        /// <param name="cmdType"></param>
        /// <param name="cmdText"></param>
        /// <param name="commandParameters"></param>
        /// <returns></returns>
        public DataSet ExeSqlDataSet(CommandType cmdType, string cmdText, params SqlParameter[] commandParameters)
        {
            cmd = conn.CreateCommand();
            try
            {
                PrepareCommand(null, cmdType, cmdText, commandParameters);
                SqlDataAdapter objDataAdapter = new SqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                objDataAdapter.Fill(ds);
                cmd.Parameters.Clear();
                cmd.Dispose();
                return ds;
            }
            catch
            {
                Dispose();
                throw;
            }
        }
    }
}

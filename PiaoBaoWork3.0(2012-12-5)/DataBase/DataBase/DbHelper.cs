namespace DataBase
{
    using DataBase.Common;
    using DataBase.Configuration;
    using DataBase.Data;
    using DataBase.DbCommon;
    using DataBase.DbInterface;
    using DataBase.Enums;
    using DataBase.LogCommon;
    using DataBase.Unique;
    using DataBase.Utils;
    using FirebirdSql.Data.FirebirdClient;
    using MySql.Data.MySqlClient;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;
    using System.Data.OleDb;
    using System.Data.SqlClient;
    using System.Diagnostics;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Web;

    public class DbHelper : IDisposable
    {
        [CompilerGenerated]
        private static IDbCloudProvider cloudProvider;
        private DbCommand command;
        private DbConnection connection;
        private string connectionString;
        private const string contextDbHelperKey = "__ContextDbHelper";
        private DataBase.Enums.DbType dbType;
        private static readonly int DefaultCommandTimeout = AppSettings.GetInt("DbHelper.CommandTimeout");
        private static readonly int DefaultConnectTimeout = AppSettings.GetInt("DbHelper.ConnectTimeout");
        private bool disposed;
        private bool fieldNameToLower;
        private bool hasError;
        private bool hasMysqlTimeoutError;
        private bool isDebuggingSql;
        private bool isLoggingSql;
        private static readonly bool IsLogSqlEnabled = AppSettings.GetBool("LogModule.LogSql");
        [CompilerGenerated]
        private bool isSelecting;
        private string lastNonTransactionSql;
        private SqlLogItem logItem;
        private static Hashtable mysqlConnectionInfos = Hashtable.Synchronized(new Hashtable());
        private SqlStatementExecutedEventHandler onStatementExecuted;
        private IHashObject outputParameters;
        private DateTime startTime;
        private Stopwatch stopwatch;
        private static DbHelper testDbHelper;

        static DbHelper()
        {
            string str = AppSettings.GetString("DbHelper.CloudProviderType");
            if (!string.IsNullOrEmpty(str))
            {
                Type type = Type.GetType(str, false);
                if (type == null)
                {
                    throw new InvalidOperationException("DbHelper.CloudProviderType 指定的类不存在：" + str);
                }
                CloudProvider = (IDbCloudProvider) Activator.CreateInstance(type);
            }
        }

        public DbHelper()
        {
            this.disposed = false;
        }

        public DbHelper(DbConnection connection) : this(connection, false)
        {
        }

        public DbHelper(string connectionString) : this(connectionString, false)
        {
        }

        public DbHelper(DbConnection connection, bool fieldNameToLower)
        {
            this.connection = connection;
            this.connectionString = connection.ConnectionString;
            this.dbType = GetDbType(this.connectionString);
            this.command = connection.CreateCommand();
            if (DefaultCommandTimeout > 0)
            {
                this.command.CommandTimeout = DefaultCommandTimeout;
            }
            this.fieldNameToLower = fieldNameToLower;
            bool isNoLoggingSql = false;
            InternalUtils.InitSqlDebug(ref this.isDebuggingSql, ref isNoLoggingSql);
            this.isLoggingSql = IsLogSqlEnabled && !isNoLoggingSql;
        }

        public DbHelper(string connectionString, bool fieldNameToLower) : this(CreateConnection(connectionString), fieldNameToLower)
        {
        }

        public DbParameter AddInputOutputParameter(string parameterName, object value)
        {
            return this.DoAddOutputParameter(parameterName, value, ParameterDirection.InputOutput);
        }

        public DbParameter AddOutputParameter(string parameterName, object value)
        {
            return this.DoAddOutputParameter(parameterName, value, ParameterDirection.Output);
        }

        public DbParameter AddParameter(string parameterName, object value)
        {
            DbParameter param = this.command.CreateParameter();
            param.ParameterName = parameterName;
            this.SetParameterValue(param, value);
            this.command.Parameters.Add(param);
            return param;
        }

        private void AddParameters(IHashObject paramValues)
        {
            if ((paramValues != null) && (paramValues.Count > 0))
            {
                foreach (KeyValuePair<string, object> pair in paramValues)
                {
                    this.AddParameter(pair.Key, pair.Value);
                }
            }
        }

        private void AddReturnParameter()
        {
            DbParameter parameter = this.command.CreateParameter();
            parameter.Direction = ParameterDirection.ReturnValue;
            this.command.Parameters.Add(parameter);
        }

        public int BatchExecute(string sql)
        {
            return this.BatchExecute(sql, null);
        }

        public int BatchExecute(string sql, SqlStatementExecutedEventHandler onStatementExecuted)
        {
            if (this.DbType != DataBase.Enums.DbType.MySql)
            {
                throw new InvalidOperationException();
            }
            MySqlScript script = new MySqlScript((MySqlConnection) this.connection, sql);
            this.onStatementExecuted = onStatementExecuted;
            script.StatementExecuted += new MySqlStatementExecutedEventHandler(this.DoStatementExecuted);
            return script.Execute();
        }

        public void BeginTransaction()
        {
            this.CheckTransaction();
            try
            {
                this.command.Transaction = this.connection.BeginTransaction();
            }
            catch (Exception exception)
            {
                throw new InvalidOperationException(("启动事务失败，上次执行的非事务SQL：" + this.lastNonTransactionSql) ?? "无", exception);
            }
        }

        private void BuildParameters(string[] fieldNames, object[] fieldValues)
        {
            if (fieldNames.Length != fieldValues.Length)
            {
                throw new InvalidOperationException("fieldNames 与 fieldValues 长度不同");
            }
            for (int i = 0; i < fieldNames.Length; i++)
            {
                this.AddParameter(fieldNames[i], fieldValues[i]);
            }
        }

        private void BuildParameters(string[] fieldNames, IDictionary<string, object> data)
        {
            foreach (string str in fieldNames)
            {
                object obj2 = data.ContainsKey(str) ? data[str] : null;
                this.AddParameter(str, obj2);
            }
        }

        private void CheckHasMysqlTimeoutError(Exception e)
        {
            if (e.InnerException != null)
            {
                e = e.InnerException;
            }
            if (e.Message.IndexOf("timeout", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                this.hasMysqlTimeoutError = true;
            }
        }

        public static string CheckParameter(string value)
        {
            value = value.Replace("’'", "＇'");
            value = value.Replace("‘'", "＇'");
            return value;
        }

        private static string CheckReplaceNonGbkChar(string value)
        {
            value = value.Replace("", "　");
            value = value.Replace("", "　");
            return value;
        }

        private static DataTable CheckSchemaTable(DbDataReader reader, bool throwIfNoDataTable)
        {
            DataTable schemaTable = reader.GetSchemaTable();
            if (schemaTable == null)
            {
                if (throwIfNoDataTable)
                {
                    reader.Close();
                    throw new InvalidOperationException("取不到查询结果的定义");
                }
                reader.Close();
            }
            return schemaTable;
        }

        private void CheckTransaction()
        {
            if (this.HasBegunTransaction)
            {
                throw new InvalidOperationException("已经存在活动的事务，不能开始事务！");
            }
            this.DoOpenConnection(DbHelperCommandKind.BeginTransaction, null);
        }

        public static DataTable CloneDataTable(DataTable src)
        {
            DataTable table = src.Clone();
            foreach (DataRow row in src.Rows)
            {
                DataRow row2 = table.NewRow();
                row2.ItemArray = row.ItemArray;
                table.Rows.Add(row2);
            }
            table.AcceptChanges();
            return table;
        }

        public void CommitTransaction()
        {
            if (this.command.Transaction == null)
            {
                throw new InvalidOperationException("没有活动的事务需要提交！");
            }
            this.command.Transaction.Commit();
            this.command.Transaction = null;
        }

        public static DbConnection CreateConnection(string connectionString)
        {
            DataBase.Enums.DbType dbType = GetDbType(connectionString);
            switch (dbType)
            {
                case DataBase.Enums.DbType.Sql2000:
                    if (((DefaultConnectTimeout > 0) && (connectionString != null)) && (connectionString.IndexOf("connect timeout", StringComparison.OrdinalIgnoreCase) < 0))
                    {
                        connectionString = connectionString.Trim();
                        if (connectionString[connectionString.Length - 1] != ';')
                        {
                            connectionString = connectionString + ';';
                        }
                        connectionString = connectionString + string.Format("{0}={1}", "connect timeout", DefaultConnectTimeout);
                    }
                    return new SqlConnection(connectionString);

                case DataBase.Enums.DbType.MySql:
                    return CreateMysqlConnection(connectionString);

                case DataBase.Enums.DbType.Oracle:
                case DataBase.Enums.DbType.Access:
                    return CreateOleDbConnection(connectionString);
            }
            if (dbType != DataBase.Enums.DbType.Firebird)
            {
                throw new InvalidOperationException();
            }
            return CreateFirebirdConnection(connectionString);
        }

        private static DbHelper CreateContextDbHelper()
        {
            IDbHelperCreator dbHelperCreator = Settings.DbHelperCreator;
            if (dbHelperCreator == null)
            {
                throw new InvalidOperationException("请先设置全局属性 Settings.DbHelperCreator");
            }
            DbHelper helper = dbHelperCreator.CreateDbHelper();
            if (helper == null)
            {
                throw new InvalidOperationException("IDbHelperCreator.CreateDbHelper 不能返回 null");
            }
            return helper;
        }

        private static DbConnection CreateFirebirdConnection(string connectionString)
        {
            return new FbConnection(connectionString);
        }

        private static DbConnection CreateMysqlConnection(string connectionString)
        {
            return new MySqlConnection(connectionString);
        }

        private static DbConnection CreateOleDbConnection(string connectionString)
        {
            return new OleDbConnection(connectionString);
        }

        public static IHashObject DataRowToObject(DataRow row)
        {
            return DataRowToObject(row.Table, row);
        }

        public static IHashObject DataRowToObject(DataTable table, DataRow row)
        {
            HashObject obj2 = new HashObject();
            object[] itemArray = row.ItemArray;
            for (int i = 0; i < table.Columns.Count; i++)
            {
                string columnName = table.Columns[i].ColumnName;
                obj2[columnName] = itemArray[i];
            }
            return obj2;
        }

        public static IHashObject DataTableFirstRowToObject(DataTable table)
        {
            if (table.Rows.Count <= 0)
            {
                return null;
            }
            return DataRowToObject(table, table.Rows[0]);
        }

        public static IHashObject DataTableToEmptyObject(DataTable table)
        {
            HashObject obj2 = new HashObject();
            for (int i = 0; i < table.Columns.Count; i++)
            {
                DataColumn column = table.Columns[i];
                object obj3 = null;
                if (column.DataType == typeof(string))
                {
                    obj3 = string.Empty;
                }
                else if (((column.DataType == typeof(decimal)) || (column.DataType == typeof(double))) || ((column.DataType == typeof(int)) || (column.DataType == typeof(short))))
                {
                    obj3 = 0M;
                }
                obj2[column.ColumnName] = obj3;
            }
            return obj2;
        }

        public static object DataTableToItemList(DataTable table, int first, int count)
        {
            int num;
            DataView defaultView = table.DefaultView;
            if (count < 0)
            {
                count = defaultView.Count;
            }
            _ScriptDbItemList list = new _ScriptDbItemList();
            int num2 = table.Columns.Count;
            list.fields = new string[num2];
            for (num = 0; num < num2; num++)
            {
                list.fields[num] = table.Columns[num].ColumnName;
            }
            if (first == 0)
            {
                list.fieldSizes = new int[num2];
                list.fieldTypes = new int[num2];
                for (num = 0; num < num2; num++)
                {
                    DataColumn column = table.Columns[num];
                    list.fieldSizes[num] = GetFieldSize(column.MaxLength);
                    DataType fieldType = GetFieldType(column.DataType);
                    list.fieldTypes[num] = (int) fieldType;
                }
            }
            int num3 = Math.Min(count, defaultView.Count - first);
            list.rows = new object[(num3 > 0) ? num3 : 0];
            for (num = 0; num < num3; num++)
            {
                list.rows[num] = defaultView[first + num].Row.ItemArray;
            }
            return list;
        }

        public static IHashObjectList DataTableToObjectList(DataTable table)
        {
            return DataTableToObjectList(table, 0, table.Rows.Count);
        }

        public static IHashObjectList DataTableToObjectList(DataTable table, int first, int count)
        {
            int capacity = Math.Min(count, table.Rows.Count - first);
            HashObjectList list = new HashObjectList(capacity);
            for (int i = 0; i < capacity; i++)
            {
                list.Add(DataRowToObject(table, table.Rows[first + i]));
            }
            return list;
        }

        private static string DbParameterToString(DbParameter param)
        {
            return DbParameterToString(param.Value);
        }

        private static string DbParameterToString(object value)
        {
            if ((value == null) || (value == DBNull.Value))
            {
                return "NULL";
            }
            Type type = value.GetType();
            if ((((type == typeof(ulong)) || (type == typeof(long))) || ((type == typeof(decimal)) || (type == typeof(double)))) || ((type == typeof(int)) || (type == typeof(short))))
            {
                return value.ToString();
            }
            if (type == typeof(DateTime))
            {
                DateTime time = (DateTime) value;
                return SqlHelper.GetQuotedStr(time.ToString("yyyy-MM-dd HH:mm:ss"));
            }
            if (type == typeof(string))
            {
                return SqlHelper.GetQuotedStr((string) value);
            }
            if (type.IsSubclassOf(typeof(Enum)))
            {
                int num = (int) value;
                return num.ToString();
            }
            return SqlHelper.GetQuotedStr(value.ToString());
        }

        public int Delete(string tableName, string keyField, object keyValue)
        {
            string sql = SqlHelper.BuildDeleteSql(tableName, keyField);
            this.AddParameter(keyField, keyValue);
            return this.ExecuteNonQuerySQL(sql);
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    this.connection.Close();
                }
                this.disposed = true;
            }
        }

        internal static void DisposeContextDbHelper()
        {
            HttpContext current = HttpContext.Current;
            if ((current != null) && current.Items.Contains("__ContextDbHelper"))
            {
                ((DbHelper) current.Items["__ContextDbHelper"]).Dispose();
                current.Items.Remove("__ContextDbHelper");
            }
        }

        private DbParameter DoAddOutputParameter(string parameterName, object value, ParameterDirection direction)
        {
            DbParameter parameter = this.AddParameter(parameterName, value);
            parameter.Direction = direction;
            if (value != null)
            {
                Type type = value.GetType();
                if (!IsPrimitiveType(type))
                {
                    throw new InvalidOperationException(string.Format("输出参数“{0}”不支持类型 {1}", parameterName, type.Name));
                }
                if (type == typeof(string))
                {
                    parameter.Size = 0xfa0;
                    return parameter;
                }
                if (type == typeof(decimal))
                {
                    ((IDbDataParameter) parameter).Scale = (byte) Settings.DecimalScale;
                }
            }
            return parameter;
        }

        private void DoAfterExecute()
        {
            if (this.logItem != null)
            {
                this.logItem.ExecuteElapsedMilliseconds = this.stopwatch.ElapsedMilliseconds;
            }
        }

        private void DoBeginExecute(string commandText, CommandType commandType)
        {
            this.hasMysqlTimeoutError = false;
            this.command.CommandText = commandText;
            this.command.CommandType = commandType;
            this.DoOpenConnection((commandType == CommandType.Text) ? DbHelperCommandKind.ExecuteSQL : DbHelperCommandKind.ExecuteProcedure, commandText);
            if (this.isLoggingSql || this.isDebuggingSql)
            {
                this.hasError = false;
                if (this.stopwatch == null)
                {
                    this.stopwatch = new Stopwatch();
                    this.stopwatch.Start();
                }
                else
                {
                    this.stopwatch.Reset();
                }
                this.startTime = DateTime.Now;
                this.logItem = new SqlLogItem();
                if (this.DbType == DataBase.Enums.DbType.MySql)
                {
                    this.LogMySqlConnection(this.startTime, commandText);
                }
            }
        }

        private void DoBeginExecuteProcedure(string procedureName, IHashObject paramValues)
        {
            this.DoBeginExecute(procedureName, CommandType.StoredProcedure);
            DbParameterHelper.DeriveParamters(this);
            bool flag = true;
            if (flag)
            {
                paramValues = KeysToLower(paramValues);
            }
            for (int i = 0; i < this.command.Parameters.Count; i++)
            {
                object obj2;
                DbParameter param = this.command.Parameters[i];
                string parameterName = param.ParameterName;
                if (parameterName[0] == '@')
                {
                    parameterName = parameterName.Substring(1);
                }
                if (flag)
                {
                    parameterName = parameterName.ToLower();
                }
                if (!paramValues.TryGetValue(parameterName, out obj2))
                {
                    if ((param.Direction != ParameterDirection.InputOutput) && (param.Direction != ParameterDirection.Output))
                    {
                        throw new InvalidOperationException(string.Format("没有传存储过程“{0}”参数“{1}”", procedureName, parameterName));
                    }
                }
                else
                {
                    this.SetParameterValue(param, obj2);
                }
            }
        }

        private void DoBeginExecuteProcedure(string procedureName, object[] paramValues)
        {
            this.DoBeginExecute(procedureName, CommandType.StoredProcedure);
            DbParameterHelper.DeriveParamters(this);
            if (paramValues.Length > this.command.Parameters.Count)
            {
                throw new InvalidOperationException("传入的参数个数超出实际需要的个数！");
            }
            for (int i = 0; i < paramValues.Length; i++)
            {
                this.SetParameterValue(this.command.Parameters[i], paramValues[i]);
            }
        }

        private static void DoDomainUnload(object sender, EventArgs e)
        {
            testDbHelper.Dispose();
            testDbHelper = null;
        }

        private void DoEndExecute()
        {
            if (this.hasMysqlTimeoutError)
            {
                this.DoFixMysqlTimeoutErrorOnEndExecute();
            }
            if ((!this.hasError && (this.isLoggingSql || this.isDebuggingSql)) && (this.stopwatch != null))
            {
                long elapsedMilliseconds = this.stopwatch.ElapsedMilliseconds;
                StringBuilder sb = new StringBuilder();
                this.FormatSql(sb);
                string sql = sb.ToString().Trim();
                bool flag = IsSelectSql(sql);
                if (flag)
                {
                    this.logItem.FetchElapsedMilliseconds = elapsedMilliseconds - this.logItem.ExecuteElapsedMilliseconds;
                }
                if (this.isLoggingSql)
                {
                    this.DoLog(sql, elapsedMilliseconds);
                }
                if (this.isDebuggingSql)
                {
                    if ((this.DbType == DataBase.Enums.DbType.MySql) && flag)
                    {
                        string str2 = "explain " + sql;
                        DbCommand command = this.connection.CreateCommand();
                        command.CommandType = CommandType.Text;
                        command.CommandText = str2;
                        try
                        {
                            DataTable table = ReaderToDataTable(command.ExecuteReader(), false);
                            sb.Append("<div style='padding-left: 20px; padding-bottom:10px;'><table border=0 cellspacing=1 cellpadding=1 bgcolor=#dddddd><tr>");
                            FormatExplainHeader(sb, "序号");
                            FormatExplainHeader(sb, "Select类型");
                            FormatExplainHeader(sb, "表名");
                            FormatExplainHeader(sb, "计划类型");
                            FormatExplainHeader(sb, "可能的索引");
                            FormatExplainHeader(sb, "使用的索引");
                            FormatExplainHeader(sb, "索引长");
                            FormatExplainHeader(sb, "引用");
                            FormatExplainHeader(sb, "读取行数");
                            FormatExplainHeader(sb, "其它信息");
                            sb.Append("</tr>");
                            foreach (DataRow row in table.Rows)
                            {
                                sb.Append("<tr>");
                                FormatExplainData(sb, row["id"], "right");
                                FormatExplainData(sb, row["select_type"], "center");
                                FormatExplainData(sb, row["table"], null);
                                FormatExplainData(sb, row["type"], "center");
                                FormatExplainData(sb, row["possible_keys"], null);
                                FormatExplainData(sb, row["key"], null);
                                FormatExplainData(sb, row["key_len"], "right");
                                FormatExplainData(sb, row["ref"], null);
                                FormatExplainData(sb, row["rows"], "right");
                                FormatExplainData(sb, row["Extra"], null);
                                sb.Append("</tr>");
                            }
                            sb.AppendLine("</table></div>");
                        }
                        catch (Exception exception)
                        {
                            sb.Append("<br /><font color=\"red\">取执行计划出错：").Append(exception.Message).Append("<br />").Append(str2).AppendLine("</font>");
                            DataBase.LogCommon.Log.Error(str2, exception);
                        }
                    }
                    SqlLog.Log(sb.ToString(), this.startTime, this.logItem.ExecuteElapsedMilliseconds, this.logItem.RowCount, this.logItem.FetchElapsedMilliseconds, elapsedMilliseconds);
                }
            }
            this.command.Parameters.Clear();
        }

        private int DoExecuteNoQueryProcedure(bool convertParameterName)
        {
            int num;
            try
            {
                int num2 = this.command.ExecuteNonQuery();
                this.GetContextOutputParameter(convertParameterName);
                this.DoLogRowCount(num2);
                num = num2;
            }
            catch (Exception exception)
            {
                this.DoLogErrorSQL(exception);
                throw;
            }
            finally
            {
                this.DoEndExecute();
            }
            return num;
        }

        private object DoExecuteOutLastParameterProcedure()
        {
            object obj2;
            try
            {
                this.command.ExecuteNonQuery();
                for (int i = this.command.Parameters.Count - 1; i > 0; i--)
                {
                    DbParameter parameter = this.command.Parameters[i];
                    if ((parameter.Direction == ParameterDirection.Output) || (parameter.Direction == ParameterDirection.InputOutput))
                    {
                        return parameter.Value;
                    }
                }
                obj2 = null;
            }
            catch (Exception exception)
            {
                this.DoLogErrorSQL(exception);
                throw;
            }
            finally
            {
                this.DoEndExecute();
            }
            return obj2;
        }

        private IHashObject DoExecuteOutputManyParameterProcedure(bool convertParameterName)
        {
            IHashObject obj2;
            try
            {
                this.command.ExecuteNonQuery();
                IHashObject outputParameterValues = new HashObject();
                this.GetOutputParameterValues(outputParameterValues, convertParameterName);
                obj2 = outputParameterValues;
            }
            catch (Exception exception)
            {
                this.DoLogErrorSQL(exception);
                throw;
            }
            finally
            {
                this.DoEndExecute();
            }
            return obj2;
        }

        private int DoExecuteOutputParameterProcedureAndReturnValue(IDictionary<string, object> outputParameterValues, bool convertParameterName)
        {
            int returnValue;
            try
            {
                this.AddReturnParameter();
                this.command.ExecuteNonQuery();
                this.GetOutputParameterValues(outputParameterValues, convertParameterName);
                returnValue = this.GetReturnValue();
            }
            catch (Exception exception)
            {
                this.DoLogErrorSQL(exception);
                throw;
            }
            finally
            {
                this.DoEndExecute();
            }
            return returnValue;
        }

        private DataTable DoExecuteProcedure(bool convertParameterName)
        {
            DataTable table;
            try
            {
                DbDataReader reader = this.command.ExecuteReader();
                this.DoAfterExecute();
                DataTable table2 = ReaderToDataTable(reader, this.fieldNameToLower);
                this.GetContextOutputParameter(convertParameterName);
                this.DoLogRowCount(table2.Rows.Count);
                table = table2;
            }
            catch (Exception exception)
            {
                this.DoLogErrorSQL(exception);
                throw;
            }
            finally
            {
                this.DoEndExecute();
            }
            return table;
        }

        private DataTable DoExecuteProcedureAndOutputParameter(IDictionary<string, object> outputParameterValues, bool convertParameterName)
        {
            DataTable table;
            try
            {
                DbDataReader reader = this.command.ExecuteReader();
                this.DoAfterExecute();
                DataTable table2 = ReaderToDataTable(reader, this.fieldNameToLower);
                this.GetOutputParameterValues(outputParameterValues, convertParameterName);
                this.DoLogRowCount(table2.Rows.Count);
                table = table2;
            }
            catch (Exception exception)
            {
                this.DoLogErrorSQL(exception);
                throw;
            }
            finally
            {
                this.DoEndExecute();
            }
            return table;
        }

        private DataTable DoExecuteProcedureAndReturnValue(out int returnValue)
        {
            DataTable table;
            try
            {
                this.AddReturnParameter();
                DbDataReader reader = this.command.ExecuteReader();
                this.DoAfterExecute();
                DataTable table2 = ReaderToDataTable(reader, this.fieldNameToLower, false);
                returnValue = this.GetReturnValue();
                this.DoLogRowCount((table2 != null) ? table2.Rows.Count : 0);
                table = table2;
            }
            catch (Exception exception)
            {
                this.DoLogErrorSQL(exception);
                throw;
            }
            finally
            {
                this.DoEndExecute();
            }
            return table;
        }

        private DataTable[] DoExecuteProcedureEx(bool convertParameterName)
        {
            DataTable[] tableArray;
            try
            {
                DbDataReader reader = this.command.ExecuteReader();
                this.DoAfterExecute();
                DataTable[] tableArray2 = ReaderToDataTables(reader, this.fieldNameToLower);
                this.GetContextOutputParameter(convertParameterName);
                this.DoLogRowCount(tableArray2[tableArray2.Length - 1].Rows.Count);
                tableArray = tableArray2;
            }
            catch (Exception exception)
            {
                this.DoLogErrorSQL(exception);
                throw;
            }
            finally
            {
                this.DoEndExecute();
            }
            return tableArray;
        }

        private int DoExecuteReturnValueProcedure()
        {
            int returnValue;
            try
            {
                this.AddReturnParameter();
                this.command.ExecuteNonQuery();
                returnValue = this.GetReturnValue();
            }
            catch (Exception exception)
            {
                this.DoLogErrorSQL(exception);
                throw;
            }
            finally
            {
                this.DoEndExecute();
            }
            return returnValue;
        }

        private object DoExecuteScalarProcedure()
        {
            object obj2;
            try
            {
                obj2 = this.command.ExecuteScalar();
            }
            catch (Exception exception)
            {
                this.DoLogErrorSQL(exception);
                throw;
            }
            finally
            {
                this.DoEndExecute();
            }
            return obj2;
        }

        private void DoFixMysqlTimeoutErrorOnEndExecute()
        {
            try
            {
                MySqlConnectionStringBuilder builder = (MySqlConnectionStringBuilder) this.connection.GetType().InvokeMember("Settings", BindingFlags.GetProperty | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance, null, this.connection, null);
                if ((builder != null) && !builder.Pooling)
                {
                    builder.Pooling = true;
                    DataBase.LogCommon.Log.Info("成功修正MySql超时处理导致的Bug");
                }
            }
            catch (Exception exception)
            {
                DataBase.LogCommon.Log.Fatal("修正MySql超时处理导致的Bug失败", exception);
            }
        }

        private void DoLog(string sql, long elapsedMilliseconds)
        {
            this.lastNonTransactionSql = (this.command.Transaction == null) ? sql : null;
            this.logItem.DateTime = this.startTime;
            this.logItem.ElapsedMilliseconds = elapsedMilliseconds;
            this.logItem.Sql = sql;
            HttpContext current = HttpContext.Current;
            if (current != null)
            {
                HttpRequest request = current.Request;
                this.logItem.Url = request.Path;
                this.logItem.Params = request.PathInfo + request.QueryString;
                this.logItem.IPAddress = request.UserHostAddress;
                LogQueue.LogSessionVar(this.logItem, current.Session);
            }
            LogQueue.Add(this.logItem);
        }

        private void DoLogErrorSQL(Exception e)
        {
            this.hasError = true;
            if (this.DbType == DataBase.Enums.DbType.MySql)
            {
                this.CheckHasMysqlTimeoutError(e);
            }
            long elapsedMilliseconds = (this.stopwatch != null) ? this.stopwatch.ElapsedMilliseconds : 0;
            StringBuilder sb = new StringBuilder();
            this.FormatError(sb, e);
            sb.AppendLine(this.command.CommandText);
            sb.AppendLine();
            foreach (DbParameter parameter in this.command.Parameters)
            {
                object obj2 = parameter.Value;
                sb.Append("  ").Append(parameter.ParameterName).Append('=').Append(DbParameterToString(obj2));
                if (parameter.Direction != ParameterDirection.Input)
                {
                    sb.Append(" [").Append(Enum.GetName(typeof(ParameterDirection), parameter.Direction)).Append(']');
                }
                if (obj2 != null)
                {
                    Type type = obj2.GetType();
                    if (type.IsSubclassOf(typeof(Enum)))
                    {
                        sb.Append(" (").Append(type.Name).Append('.').Append(obj2.ToString()).Append(")");
                    }
                }
                sb.AppendLine();
            }
            sb.AppendLine();
            this.FormatSql(sb);
            sb.AppendLine();
            DataBase.LogCommon.Log.Error(sb.ToString());
            if (this.isLoggingSql)
            {
                StringBuilder builder2 = new StringBuilder();
                this.FormatError(builder2, e);
                this.logItem.Error = builder2.ToString();
                StringBuilder builder3 = new StringBuilder();
                this.FormatSql(builder3);
                this.DoLog(builder3.ToString(), elapsedMilliseconds);
            }
            if (this.isDebuggingSql)
            {
                sb = new StringBuilder();
                sb.Append("<font color=\"red\">");
                this.FormatError(sb, e);
                sb.AppendLine("</font><br />");
                this.FormatSql(sb);
                SqlLog.Log(sb.ToString(), this.startTime, this.logItem.ExecuteElapsedMilliseconds, this.logItem.RowCount, this.logItem.FetchElapsedMilliseconds, elapsedMilliseconds);
            }
        }

        private void DoLogRowCount(int value)
        {
            if (this.logItem != null)
            {
                this.logItem.RowCount = value;
            }
        }

        private void DoOpenConnection(DbHelperCommandKind commandKind, string commandText)
        {
            if (this.connection.State != ConnectionState.Open)
            {
                this.connection.Close();
                try
                {
                    bool flag = false;
                    if (CloudProvider != null)
                    {
                        DbHelperState state = new DbHelperState(this.connection, this.connectionString, this.DbType, this.HasBegunTransaction, commandKind, commandText, this.IsSelecting);
                        flag = CloudProvider.OpenConnection(state);
                    }
                    if (!flag)
                    {
                        this.connection.Open();
                    }
                }
                catch (Exception exception)
                {
                    try
                    {
                        if ((this.DbType == DataBase.Enums.DbType.Sql2000) && exception.Message.Contains("error: 0 -"))
                        {
                            SqlConnection.ClearAllPools();
                            DataBase.LogCommon.Log.Info("SqlConnection.ClearAllPools 成功");
                        }
                    }
                    catch (Exception exception2)
                    {
                        DataBase.LogCommon.Log.Error("SqlConnection.ClearAllPools 失败", exception2);
                    }
                    throw new InvalidOperationException("数据库服务未开启或连接出错，请与系统管理员联系！", exception);
                }
            }
        }

        private IHashObjectList DoProcedureSelect(bool convertParameterName)
        {
            IHashObjectList list;
            try
            {
                DbDataReader reader = this.command.ExecuteReader();
                this.DoAfterExecute();
                IHashObjectList list2 = ReaderToObjectList(reader, this.fieldNameToLower);
                this.GetContextOutputParameter(convertParameterName);
                this.DoLogRowCount(list2.Count);
                list = list2;
            }
            catch (Exception exception)
            {
                this.DoLogErrorSQL(exception);
                throw;
            }
            finally
            {
                this.DoEndExecute();
            }
            return list;
        }

        private IHashObjectList DoProcedureSelectAndOutputParameter(out IHashObject outputParameterValues, bool convertParameterName)
        {
            IHashObjectList list;
            try
            {
                DbDataReader reader = this.command.ExecuteReader();
                this.DoAfterExecute();
                IHashObjectList list2 = ReaderToObjectList(reader, this.fieldNameToLower);
                outputParameterValues = new HashObject();
                this.GetOutputParameterValues(outputParameterValues, convertParameterName);
                this.DoLogRowCount(list2.Count);
                list = list2;
            }
            catch (Exception exception)
            {
                this.DoLogErrorSQL(exception);
                throw;
            }
            finally
            {
                this.DoEndExecute();
            }
            return list;
        }

        private IHashObjectList DoProcedureSelectAndReturnValue(out int returnValue)
        {
            IHashObjectList list;
            try
            {
                this.AddReturnParameter();
                DbDataReader reader = this.command.ExecuteReader();
                this.DoAfterExecute();
                IHashObjectList list2 = ReaderToObjectList(reader, this.fieldNameToLower, false);
                returnValue = this.GetReturnValue();
                this.DoLogRowCount((list2 != null) ? list2.Count : 0);
                list = list2;
            }
            catch (Exception exception)
            {
                this.DoLogErrorSQL(exception);
                throw;
            }
            finally
            {
                this.DoEndExecute();
            }
            return list;
        }

        private void DoStatementExecuted(object sender, MySqlScriptEventArgs args)
        {
            if (this.onStatementExecuted != null)
            {
                this.onStatementExecuted(sender, args.StatementText, args.Line, args.Position);
            }
        }

        public bool DropTable(string tableName)
        {
            if (this.DbType == DataBase.Enums.DbType.MySql)
            {
                return (this.ExecuteNonQuerySQL("drop table if exists " + tableName) == 0);
            }
            if (this.DbType == DataBase.Enums.DbType.Sql2000)
            {
                return (this.ExecuteNonQuerySQL(string.Format("if exists (select * from sysobjects where xtype='u' and name='{0}') drop table {0}", tableName)) == -1);
            }
            if (this.DbType != DataBase.Enums.DbType.Firebird)
            {
                throw new InvalidOperationException();
            }
            if (this.TableExists(tableName))
            {
                return (this.ExecuteNonQuerySQL("drop table " + tableName) == -1);
            }
            return true;
        }

        public DataRow ExecuteFirstRowSQL(string sql)
        {
            DataTable table = this.ExecuteSQL(sql);
            int count = table.Rows.Count;
            if (count >= 2)
            {
                throw new InvalidOperationException(string.Format("执行SQL“{0}”时期待返回一行，现在返回了 {1} 行", sql, count));
            }
            if (count <= 0)
            {
                return null;
            }
            return table.Rows[0];
        }

        public int ExecuteIntSQL(string sql)
        {
            object obj2 = this.ExecuteScalerSQL(sql);
            if (obj2 == DBNull.Value)
            {
                return 0;
            }
            return Convert.ToInt32(obj2);
        }

        internal _ScriptDbItemList ExecuteItemList(string sql)
        {
            _ScriptDbItemList list;
            try
            {
                _ScriptDbItemList list2 = ReaderToItemList(this.ExecuteReader(sql), this.fieldNameToLower);
                this.DoLogRowCount(list2.rows.Length);
                list = list2;
            }
            finally
            {
                this.DoEndExecute();
            }
            return list;
        }

        public int ExecuteNonQueryProcedure(string procedureName)
        {
            this.DoBeginExecute(procedureName, CommandType.StoredProcedure);
            return this.DoExecuteNoQueryProcedure(false);
        }

        public int ExecuteNonQueryProcedure(string procedureName, params object[] paramValues)
        {
            this.DoBeginExecuteProcedure(procedureName, paramValues);
            return this.DoExecuteNoQueryProcedure(true);
        }

        public int ExecuteNonQueryProcedure(string procedureName, IHashObject paramValues)
        {
            this.DoBeginExecuteProcedure(procedureName, paramValues);
            return this.DoExecuteNoQueryProcedure(true);
        }

        public int ExecuteNonQuerySQL(string sql)
        {
            int num;
            this.DoBeginExecute(sql, CommandType.Text);
            try
            {
                int num2 = this.command.ExecuteNonQuery();
                this.DoLogRowCount(num2);
                num = num2;
            }
            catch (Exception exception)
            {
                this.DoLogErrorSQL(exception);
                throw;
            }
            finally
            {
                this.DoEndExecute();
            }
            return num;
        }

        public object ExecuteOutputLastParameterProcedure(string procedureName)
        {
            this.DoBeginExecute(procedureName, CommandType.StoredProcedure);
            return this.DoExecuteOutLastParameterProcedure();
        }

        public object ExecuteOutputLastParameterProcedure(string procedureName, IHashObject paramValues)
        {
            this.DoBeginExecuteProcedure(procedureName, paramValues);
            return this.DoExecuteOutLastParameterProcedure();
        }

        public object ExecuteOutputLastParameterProcedure(string procedureName, params object[] paramValues)
        {
            this.DoBeginExecuteProcedure(procedureName, paramValues);
            return this.DoExecuteOutLastParameterProcedure();
        }

        public IHashObject ExecuteOutputParameterProcedure(string procedureName)
        {
            this.DoBeginExecute(procedureName, CommandType.StoredProcedure);
            return this.DoExecuteOutputManyParameterProcedure(false);
        }

        public IHashObject ExecuteOutputParameterProcedure(string procedureName, params object[] paramValues)
        {
            this.DoBeginExecuteProcedure(procedureName, paramValues);
            return this.DoExecuteOutputManyParameterProcedure(true);
        }

        public IHashObject ExecuteOutputParameterProcedure(string procedureName, IHashObject paramValues)
        {
            this.DoBeginExecuteProcedure(procedureName, paramValues);
            return this.DoExecuteOutputManyParameterProcedure(true);
        }

        public int ExecuteOutputParameterProcedureAndReturnValue(string procedureName, out IHashObject outputParameterValues)
        {
            this.DoBeginExecute(procedureName, CommandType.StoredProcedure);
            outputParameterValues = new HashObject();
            return this.DoExecuteOutputParameterProcedureAndReturnValue(outputParameterValues, false);
        }

        public int ExecuteOutputParameterProcedureAndReturnValue(string procedureName, out IHashObject outputParameterValues, params object[] paramValues)
        {
            this.DoBeginExecuteProcedure(procedureName, paramValues);
            outputParameterValues = new HashObject();
            return this.DoExecuteOutputParameterProcedureAndReturnValue(outputParameterValues, true);
        }

        public int ExecuteOutputParameterProcedureAndReturnValue(string procedureName, out IHashObject outputParameterValues, IHashObject paramValues)
        {
            this.DoBeginExecuteProcedure(procedureName, paramValues);
            outputParameterValues = new HashObject();
            return this.DoExecuteOutputParameterProcedureAndReturnValue(outputParameterValues, true);
        }

        public DataTable ExecuteProcedure(string procedureName)
        {
            this.DoBeginExecute(procedureName, CommandType.StoredProcedure);
            return this.DoExecuteProcedure(false);
        }

        public DataTable ExecuteProcedure(string procedureName, IHashObject paramValues)
        {
            this.DoBeginExecuteProcedure(procedureName, paramValues);
            return this.DoExecuteProcedure(true);
        }

        public DataTable ExecuteProcedure(string procedureName, params object[] paramValues)
        {
            this.DoBeginExecuteProcedure(procedureName, paramValues);
            return this.DoExecuteProcedure(true);
        }

        public DataTable ExecuteProcedureAndOutputParameter(string procedureName, out IHashObject outputParameterValues)
        {
            this.DoBeginExecute(procedureName, CommandType.StoredProcedure);
            outputParameterValues = new HashObject();
            return this.DoExecuteProcedureAndOutputParameter(outputParameterValues, false);
        }

        public DataTable ExecuteProcedureAndOutputParameter(string procedureName, out IHashObject outputParameterValues, IHashObject paramValues)
        {
            this.DoBeginExecuteProcedure(procedureName, paramValues);
            outputParameterValues = new HashObject();
            return this.DoExecuteProcedureAndOutputParameter(outputParameterValues, true);
        }

        public DataTable ExecuteProcedureAndOutputParameter(string procedureName, out IHashObject outputParameterValues, params object[] paramValues)
        {
            this.DoBeginExecuteProcedure(procedureName, paramValues);
            outputParameterValues = new HashObject();
            return this.DoExecuteProcedureAndOutputParameter(outputParameterValues, true);
        }

        public DataTable ExecuteProcedureAndReturnValue(string procedureName, out int returnValue)
        {
            this.DoBeginExecute(procedureName, CommandType.StoredProcedure);
            return this.DoExecuteProcedureAndReturnValue(out returnValue);
        }

        public DataTable ExecuteProcedureAndReturnValue(string procedureName, out int returnValue, params object[] paramValues)
        {
            this.DoBeginExecuteProcedure(procedureName, paramValues);
            return this.DoExecuteProcedureAndReturnValue(out returnValue);
        }

        public DataTable ExecuteProcedureAndReturnValue(string procedureName, out int returnValue, IHashObject paramValues)
        {
            this.DoBeginExecuteProcedure(procedureName, paramValues);
            return this.DoExecuteProcedureAndReturnValue(out returnValue);
        }

        public DataTable[] ExecuteProcedureEx(string procedureName)
        {
            this.DoBeginExecute(procedureName, CommandType.StoredProcedure);
            return this.DoExecuteProcedureEx(false);
        }

        public DataTable[] ExecuteProcedureEx(string procedureName, params object[] paramValues)
        {
            this.DoBeginExecuteProcedure(procedureName, paramValues);
            return this.DoExecuteProcedureEx(true);
        }

        public DataTable[] ExecuteProcedureEx(string procedureName, IHashObject paramValues)
        {
            this.DoBeginExecuteProcedure(procedureName, paramValues);
            return this.DoExecuteProcedureEx(true);
        }

        private DbDataReader ExecuteReader(string sql)
        {
            DbDataReader reader;
            this.DoBeginExecute(sql, CommandType.Text);
            try
            {
                DbDataReader reader2 = this.command.ExecuteReader();
                this.DoAfterExecute();
                reader = reader2;
            }
            catch (Exception exception)
            {
                this.DoLogErrorSQL(exception);
                throw;
            }
            return reader;
        }

        public int ExecuteReturnValueProcedure(string procedureName)
        {
            this.DoBeginExecute(procedureName, CommandType.StoredProcedure);
            return this.DoExecuteReturnValueProcedure();
        }

        public int ExecuteReturnValueProcedure(string procedureName, IHashObject paramValues)
        {
            this.DoBeginExecuteProcedure(procedureName, paramValues);
            return this.DoExecuteReturnValueProcedure();
        }

        public int ExecuteReturnValueProcedure(string procedureName, params object[] paramValues)
        {
            this.DoBeginExecuteProcedure(procedureName, paramValues);
            return this.DoExecuteReturnValueProcedure();
        }

        public object ExecuteScalerProcedure(string procedureName)
        {
            this.DoBeginExecute(procedureName, CommandType.StoredProcedure);
            return this.DoExecuteScalarProcedure();
        }

        public object ExecuteScalerProcedure(string procedureName, params object[] paramValues)
        {
            this.DoBeginExecuteProcedure(procedureName, paramValues);
            return this.DoExecuteScalarProcedure();
        }

        public object ExecuteScalerProcedure(string procedureName, IHashObject paramValues)
        {
            this.DoBeginExecuteProcedure(procedureName, paramValues);
            return this.DoExecuteScalarProcedure();
        }

        public object ExecuteScalerSQL(string sql)
        {
            Exception exception;
            object obj2;
            this.DoBeginExecute(sql, CommandType.Text);
            try
            {
                object obj3 = this.command.ExecuteScalar();
                if (obj3 != null)
                {
                    Type type = obj3.GetType();
                    if (((type == typeof(int)) || (type == typeof(long))) || ((type == typeof(uint)) || (type == typeof(short))))
                    {
                        try
                        {
                            this.DoLogRowCount(Convert.ToInt32(obj3));
                        }
                        catch (Exception exception2)
                        {
                            exception = exception2;
                            DataBase.LogCommon.Log.Error(type.Name + " " + obj3, exception);
                        }
                    }
                }
                obj2 = obj3;
            }
            catch (Exception exception3)
            {
                exception = exception3;
                this.DoLogErrorSQL(exception);
                throw;
            }
            finally
            {
                this.DoEndExecute();
            }
            return obj2;
        }

        public DataRow ExecuteSingleRowSQL(string sql)
        {
            DataTable table = this.ExecuteSQL(sql);
            int count = table.Rows.Count;
            if (count != 1)
            {
                throw new InvalidOperationException(string.Format("执行SQL“{0}”时期待返回一行，现在返回了 {1} 行", sql, count));
            }
            return table.Rows[0];
        }

        public DataTable ExecuteSQL(string sql)
        {
            DataTable table;
            this.DoBeginExecute(sql, CommandType.Text);
            try
            {
                DbDataReader reader = this.command.ExecuteReader();
                this.DoAfterExecute();
                DataTable table2 = ReaderToDataTable(reader, this.fieldNameToLower);
                this.DoLogRowCount(table2.Rows.Count);
                table = table2;
            }
            catch (Exception exception)
            {
                this.DoLogErrorSQL(exception);
                throw;
            }
            finally
            {
                this.DoEndExecute();
            }
            return table;
        }

        public DataTable ExecuteSQL(string sql, IHashObject paramValues)
        {
            this.AddParameters(paramValues);
            return this.ExecuteSQL(sql);
        }

        ~DbHelper()
        {
            this.Dispose(false);
        }

        private void FormatError(StringBuilder sb, Exception e)
        {
            sb.Append("执行");
            if (this.command.CommandType == CommandType.StoredProcedure)
            {
                sb.Append("存储过程");
            }
            else
            {
                sb.Append("SQL");
            }
            sb.Append("出错: ").AppendLine((e != null) ? e.Message : null);
        }

        private static void FormatExplainData(StringBuilder sb, object value, string align)
        {
            sb.Append("<td bgcolor=#ffffff").Append((align != null) ? (" align=" + align) : null).Append("><font color=#999999>").Append(value).Append("</font></td>");
        }

        private static void FormatExplainHeader(StringBuilder sb, string caption)
        {
            sb.Append("<td bgcolor=#f4f4f4><font color=#999999>").Append(caption).Append("</font></td>");
        }

        private void FormatSql(StringBuilder sb)
        {
            DbParameter current;
            string parameterName;
            string str2;
            IEnumerator enumerator;
            if (this.command.CommandType == CommandType.StoredProcedure)
            {
                sb.Append("exec ").Append(this.command.CommandText);
                bool flag = true;
                enumerator = this.command.Parameters.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    current = (DbParameter) enumerator.Current;
                    if (flag)
                    {
                        sb.Append(' ');
                        flag = false;
                    }
                    else
                    {
                        sb.Append(',');
                    }
                    parameterName = current.ParameterName;
                    str2 = DbParameterToString(current);
                    sb.Append((string) ((parameterName[0] == '@') ? null : "@")).Append(parameterName).Append('=').Append(str2);
                    if ((current.Direction == ParameterDirection.Output) || (current.Direction == ParameterDirection.InputOutput))
                    {
                        sb.Append(" output");
                    }
                }
                enumerator.Reset();
            }
            else
            {
                string commandText = this.command.CommandText;
                List<string> list = new List<string>();
                enumerator = this.command.Parameters.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    current = (DbParameter) enumerator.Current;
                    parameterName = current.ParameterName;
                    list.Add(parameterName);
                }
                enumerator.Reset();
                list.Sort();
                for (int i = list.Count - 1; i >= 0; i--)
                {
                    parameterName = list[i];
                    current = this.command.Parameters[parameterName];
                    str2 = DbParameterToString(current);
                    if (parameterName[0] != '@')
                    {
                        parameterName = '@' + parameterName;
                    }
                    commandText = StringUtils.Replace(commandText, parameterName, str2, StringComparison.OrdinalIgnoreCase);
                }
                sb.Append(commandText);
            }
        }

        private static string GetColumnName(DataRow row, bool fieldNameToLower, Dictionary<string, int> columnCountMap)
        {
            string key = row["ColumnName"] as string;
            if (fieldNameToLower)
            {
                key = key.ToLower();
            }
            if (columnCountMap.ContainsKey(key))
            {
                int num = columnCountMap[key] + 1;
                columnCountMap[key] = num;
                return (key + num.ToString());
            }
            columnCountMap.Add(key, 1);
            return key;
        }

        public static DbHelper GetContextDbHelper()
        {
            HttpContext current = HttpContext.Current;
            if (current == null)
            {
                if (testDbHelper == null)
                {
                    testDbHelper = CreateContextDbHelper();
                    string friendlyName = AppDomain.CurrentDomain.FriendlyName;
                    if ((friendlyName.IndexOf("nunit", StringComparison.OrdinalIgnoreCase) < 0) && (friendlyName.IndexOf("test", StringComparison.OrdinalIgnoreCase) < 0))
                    {
                        throw new InvalidOperationException("不支持的AppDomain：" + friendlyName);
                    }
                    AppDomain.CurrentDomain.DomainUnload += new EventHandler(DbHelper.DoDomainUnload);
                }
                return testDbHelper;
            }
            if (!current.Items.Contains("__ContextDbHelper"))
            {
                DbHelper helper = CreateContextDbHelper();
                current.Items["__ContextDbHelper"] = helper;
                return helper;
            }
            return (DbHelper) current.Items["__ContextDbHelper"];
        }

        private void GetContextOutputParameter(bool convertParameterName)
        {
            this.outputParameters = new HashObject();
            this.GetOutputParameterValues(this.outputParameters, convertParameterName);
        }

        public int GetCount(string tableName)
        {
            return this.ExecuteIntSQL("select count(*) from " + tableName);
        }

        private static DataBase.Enums.DbType GetDbType(string connectionString)
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("没有指定连接字符串");
            }
            if ((connectionString.IndexOf("OraOLEDB", StringComparison.OrdinalIgnoreCase) >= 0) || (connectionString.IndexOf("MSDAORA", StringComparison.OrdinalIgnoreCase) >= 0))
            {
                return DataBase.Enums.DbType.Oracle;
            }
            if (connectionString.IndexOf("Jet.OLEDB", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return DataBase.Enums.DbType.Access;
            }
            if (connectionString.IndexOf(".fdb", StringComparison.OrdinalIgnoreCase) > 0)
            {
                return DataBase.Enums.DbType.Firebird;
            }
            if (connectionString.Contains("Data Source="))
            {
                return DataBase.Enums.DbType.Sql2000;
            }
            if (connectionString.IndexOf("Server=", StringComparison.OrdinalIgnoreCase) < 0)
            {
                throw new InvalidOperationException("不支持的连接字符串：" + connectionString);
            }
            return DataBase.Enums.DbType.MySql;
        }

        private static int GetFieldSize(int maxLength)
        {
            if (maxLength > 0x2000)
            {
                return 0x1000;
            }
            return maxLength;
        }

        private static DataType GetFieldType(Type type)
        {
            if (type == typeof(string))
            {
                return DataType.String;
            }
            if ((((type == typeof(decimal)) || (type == typeof(int))) || ((type == typeof(long)) || (type == typeof(ulong)))) || ((type == typeof(double)) || (type == typeof(short))))
            {
                return DataType.Number;
            }
            if (type == typeof(DateTime))
            {
                return DataType.DateTime;
            }
            return DataType.Unknown;
        }

        private void GetOutputParameterValues(IDictionary<string, object> outputParameterValues, bool convertParameterName)
        {
            foreach (DbParameter parameter in this.command.Parameters)
            {
                if ((parameter.Direction == ParameterDirection.Output) || (parameter.Direction == ParameterDirection.InputOutput))
                {
                    string parameterName = parameter.ParameterName;
                    if (convertParameterName && (parameterName[0] == '@'))
                    {
                        parameterName = parameterName.Substring(1);
                    }
                    outputParameterValues.Add(parameterName, parameter.Value);
                }
            }
        }

        private int GetReturnValue()
        {
            return (int) this.command.Parameters[this.command.Parameters.Count - 1].Value;
        }

        public static int IndexOfRow(DataTable table, string keyField, IComparable keyValue)
        {
            if (string.IsNullOrEmpty(keyField))
            {
                throw new ArgumentNullException("keyField");
            }
            if (keyValue == null)
            {
                throw new ArgumentNullException("keyValue");
            }
            DataView defaultView = table.DefaultView;
            for (int i = 0; i < defaultView.Count; i++)
            {
                if (keyValue.Equals(defaultView[i].Row[keyField]))
                {
                    return i;
                }
            }
            return -1;
        }

        public int Insert(string tableName, IDictionary<string, object> data)
        {
            string[] array = new string[data.Count];
            data.Keys.CopyTo(array, 0);
            return this.Insert(tableName, array, data);
        }

        public int Insert(string tableName, string[] fieldNames, IDictionary<string, object> data)
        {
            string sql = SqlHelper.BuildInsertSql(tableName, fieldNames);
            this.BuildParameters(fieldNames, data);
            return this.ExecuteNonQuerySQL(sql);
        }

        public int Insert(string tableName, string[] fieldNames, params object[] fieldValues)
        {
            string sql = SqlHelper.BuildInsertSql(tableName, fieldNames);
            this.BuildParameters(fieldNames, fieldValues);
            return this.ExecuteNonQuerySQL(sql);
        }

        private static bool IsPrimitiveType(Type type)
        {
            if ((!type.IsPrimitive && !(type == typeof(string))) && !(type == typeof(DateTime)))
            {
                return (type == typeof(decimal));
            }
            return true;
        }

        private static bool IsSelectSql(string sql)
        {
            return sql.StartsWith("select ", StringComparison.OrdinalIgnoreCase);
        }

        private static IHashObject KeysToLower(IHashObject paramValues)
        {
            IHashObject obj2 = new HashObject();
            foreach (KeyValuePair<string, object> pair in paramValues)
            {
                obj2[pair.Key.ToLower()] = pair.Value;
            }
            return obj2;
        }

        private void LogMySqlConnection(DateTime accessTime, string commandText)
        {
            try
            {
                object obj2 = this.connection.GetType().InvokeMember("driver", BindingFlags.GetField | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance, null, this.connection, null);
                if (obj2 != null)
                {
                    MysqlConnectionInfo info = new MysqlConnectionInfo {
                        AccessTime = accessTime,
                        CommandText = commandText
                    };
                    mysqlConnectionInfos[obj2.GetHashCode()] = info;
                }
            }
            catch (Exception exception)
            {
                DataBase.LogCommon.Log.Fatal("LogMySqlConnection失败", exception);
            }
        }

        public static Guid NewGuidComb()
        {
            byte[] destinationArray = Guid.NewGuid().ToByteArray();
            DateTime time = new DateTime(0x76c, 1, 1);
            DateTime now = DateTime.Now;
            TimeSpan span = new TimeSpan(now.Ticks - time.Ticks);
            TimeSpan timeOfDay = now.TimeOfDay;
            byte[] bytes = BitConverter.GetBytes(span.Days);
            byte[] array = BitConverter.GetBytes((long) (timeOfDay.TotalMilliseconds / 3.333333));
            Array.Reverse(bytes);
            Array.Reverse(array);
            Array.Copy(bytes, bytes.Length - 2, destinationArray, destinationArray.Length - 6, 2);
            Array.Copy(array, array.Length - 4, destinationArray, destinationArray.Length - 4, 4);
            return new Guid(destinationArray);
        }

        public static ulong NewUlid()
        {
            return Ulid.NewUlid();
        }

        public static ulong ParseUlid(object obj)
        {
            ParseUlid(obj.ToString());
            if (obj == null)
            {
                return 0;
            }
            return ParseUlid(obj.ToString());
        }

        public static ulong ParseUlid(string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return 0;
            }
            return ulong.Parse(str);
        }

        public static Guid ParseUlidToGuid(string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return Guid.Empty;
            }
            return new Guid(str);
        }

        public IHashObjectList ProcedureSelect(string procedureName)
        {
            this.DoBeginExecute(procedureName, CommandType.StoredProcedure);
            return this.DoProcedureSelect(false);
        }

        public IHashObjectList ProcedureSelect(string procedureName, params object[] paramValues)
        {
            this.DoBeginExecuteProcedure(procedureName, paramValues);
            return this.DoProcedureSelect(true);
        }

        public IHashObjectList ProcedureSelect(string procedureName, IHashObject paramValues)
        {
            this.DoBeginExecuteProcedure(procedureName, paramValues);
            return this.DoProcedureSelect(true);
        }

        public IHashObjectList ProcedureSelectAndOutputParameter(string procedureName, out IHashObject outputParameterValues)
        {
            this.DoBeginExecute(procedureName, CommandType.StoredProcedure);
            return this.DoProcedureSelectAndOutputParameter(out outputParameterValues, false);
        }

        public IHashObjectList ProcedureSelectAndOutputParameter(string procedureName, out IHashObject outputParameterValues, params object[] paramValues)
        {
            this.DoBeginExecuteProcedure(procedureName, paramValues);
            return this.DoProcedureSelectAndOutputParameter(out outputParameterValues, true);
        }

        public IHashObjectList ProcedureSelectAndOutputParameter(string procedureName, out IHashObject outputParameterValues, IHashObject paramValues)
        {
            this.DoBeginExecuteProcedure(procedureName, paramValues);
            return this.DoProcedureSelectAndOutputParameter(out outputParameterValues, true);
        }

        public IHashObjectList ProcedureSelectAndReturnValue(string procedureName, out int returnValue)
        {
            this.DoBeginExecute(procedureName, CommandType.StoredProcedure);
            return this.DoProcedureSelectAndReturnValue(out returnValue);
        }

        public IHashObjectList ProcedureSelectAndReturnValue(string procedureName, out int returnValue, IHashObject paramValues)
        {
            this.DoBeginExecuteProcedure(procedureName, paramValues);
            return this.DoProcedureSelectAndReturnValue(out returnValue);
        }

        public IHashObjectList ProcedureSelectAndReturnValue(string procedureName, out int returnValue, params object[] paramValues)
        {
            this.DoBeginExecuteProcedure(procedureName, paramValues);
            return this.DoProcedureSelectAndReturnValue(out returnValue);
        }

        public static DataTable ReaderToDataTable(DbDataReader reader, bool fieldNameToLower)
        {
            return ReaderToDataTable(reader, fieldNameToLower, true, true);
        }

        private static DataTable ReaderToDataTable(DbDataReader reader, bool fieldNameToLower, bool throwIfNoDataTable)
        {
            return ReaderToDataTable(reader, fieldNameToLower, throwIfNoDataTable, true);
        }

        private static DataTable ReaderToDataTable(DbDataReader reader, bool fieldNameToLower, bool throwIfNoDataTable, bool closeReader)
        {
            DataTable table = CheckSchemaTable(reader, throwIfNoDataTable);
            if (table == null)
            {
                return null;
            }
            DataTable table2 = new DataTable();
            DataRowCollection rows = table.Rows;
            int count = rows.Count;
            Dictionary<string, int> columnCountMap = new Dictionary<string, int>(count);
            int num2 = -1;
            for (int i = 0; i < count; i++)
            {
                DataColumn column = new DataColumn();
                DataRow row = rows[i];
                string str = GetColumnName(row, fieldNameToLower, columnCountMap);
                column.ColumnName = str;
                column.DataType = (Type) row["DataType"];
                if (column.DataType == typeof(string))
                {
                    int num4 = (int) row["ColumnSize"];
                    if ((num4 == 10) && (str == "type"))
                    {
                        num4 = 20;
                    }
                    else if ((num4 == 14) && (str == "Slave_IO_State"))
                    {
                        num2 = 0x400;
                        num4 = num2;
                    }
                    else if (num2 > 0)
                    {
                        num4 = Math.Max(num4, num2);
                    }
                    if (num4 > 0)
                    {
                        column.MaxLength = num4;
                    }
                }
                table2.Columns.Add(column);
            }
            object[] values = new object[count];
            try
            {
                while (reader.Read())
                {
                    reader.GetValues(values);
                    table2.Rows.Add(values);
                }
            }
            finally
            {
                if (closeReader)
                {
                    reader.Close();
                }
            }
            return table2;
        }

        private static DataTable[] ReaderToDataTables(DbDataReader reader, bool fieldNameToLower)
        {
            IList<DataTable> list = new List<DataTable>();
            try
            {
                do
                {
                    DataTable item = ReaderToDataTable(reader, fieldNameToLower, true, false);
                    list.Add(item);
                }
                while (reader.NextResult());
            }
            finally
            {
                reader.Close();
            }
            DataTable[] array = new DataTable[list.Count];
            list.CopyTo(array, 0);
            return array;
        }

        private static _ScriptDbItemList ReaderToItemList(DbDataReader reader, bool fieldNameToLower)
        {
            DataRowCollection rows = reader.GetSchemaTable().Rows;
            int count = rows.Count;
            _ScriptDbItemList list = new _ScriptDbItemList {
                fields = new string[count],
                fieldSizes = new int[count]
            };
            Dictionary<string, int> columnCountMap = new Dictionary<string, int>(count);
            for (int i = 0; i < count; i++)
            {
                DataRow row = rows[i];
                list.fields[i] = GetColumnName(row, fieldNameToLower, columnCountMap);
                list.fieldSizes[i] = GetFieldSize((int) row["ColumnSize"]);
            }
            try
            {
                ArrayList list2 = new ArrayList();
                while (reader.Read())
                {
                    object[] values = new object[count];
                    reader.GetValues(values);
                    list2.Add(values);
                }
                list.rows = list2.ToArray();
            }
            finally
            {
                reader.Close();
            }
            return list;
        }

        private static IHashObjectList ReaderToObjectList(DbDataReader reader, bool fieldNameToLower)
        {
            return ReaderToObjectList(reader, fieldNameToLower, true);
        }

        private static IHashObjectList ReaderToObjectList(DbDataReader reader, bool fieldNameToLower, bool throwIfNoDataTable)
        {
            DataTable table = CheckSchemaTable(reader, throwIfNoDataTable);
            if (table == null)
            {
                return null;
            }
            DataRowCollection rows = table.Rows;
            int count = rows.Count;
            Dictionary<string, int> columnCountMap = new Dictionary<string, int>(count);
            List<string> list = new List<string>(count);
            int index = 0;
            while (index < count)
            {
                string item = GetColumnName(rows[index], fieldNameToLower, columnCountMap);
                list.Add(item);
                index++;
            }
            HashObjectList list2 = new HashObjectList();
            object[] values = new object[count];
            try
            {
                while (reader.Read())
                {
                    reader.GetValues(values);
                    HashObject obj2 = new HashObject();
                    for (index = 0; index < count; index++)
                    {
                        string str2 = list[index];
                        obj2[str2] = values[index];
                    }
                    list2.Add(obj2);
                }
            }
            finally
            {
                reader.Close();
            }
            return list2;
        }

        public void RollbackTransaction()
        {
            if (this.command.Transaction == null)
            {
                throw new InvalidOperationException("没有活动的事务需要回滚！");
            }
            this.command.Transaction.Rollback();
            this.command.Transaction = null;
        }

        public IHashObjectList Select(string sql)
        {
            IHashObjectList list;
            try
            {
                DbDataReader reader = this.ExecuteReader(sql);
                this.DoAfterExecute();
                IHashObjectList list2 = ReaderToObjectList(reader, this.fieldNameToLower);
                this.DoLogRowCount(list2.Count);
                list = list2;
            }
            finally
            {
                this.DoEndExecute();
            }
            return list;
        }

        public IHashObjectList Select(string sql, IHashObject paramValues)
        {
            this.AddParameters(paramValues);
            return this.Select(sql);
        }

        public IHashObject SelectFirstRow(string sql)
        {
            return DataTableFirstRowToObject(this.ExecuteSQL(sql));
        }

        public IHashObject SelectSingleRow(string sql)
        {
            DataTable table = this.ExecuteSQL(sql);
            int count = table.Rows.Count;
            if (count != 1)
            {
                throw new InvalidOperationException(string.Format("执行SQL“{0}”时期待返回一行，现在返回了 {1} 行", sql, count));
            }
            return DataTableFirstRowToObject(table);
        }

        private void SetParameterValue(DbParameter param, object value)
        {
            bool flag = false;
            if (value == null)
            {
                value = DBNull.Value;
            }
            else
            {
                Type type = value.GetType();
                if ((type == typeof(decimal)) || (type == typeof(int)))
                {
                    int maxIntLength = Settings.MaxIntLength;
                    if ((maxIntLength > 0) && (Math.Truncate(Math.Abs(Convert.ToDecimal(value))).ToString().Length > maxIntLength))
                    {
                        throw new Exception(string.Format("数字“{0}”太大，超出系统允许范围！", value));
                    }
                }
                else if (type == typeof(string))
                {
                    string str = (string) value;
                    if (this.DbType == DataBase.Enums.DbType.Sql2000)
                    {
                        flag = str.Length > 0xfa0;
                    }
                    else if (this.DbType == DataBase.Enums.DbType.MySql)
                    {
                        value = CheckReplaceNonGbkChar(str);
                    }
                }
                else if ((type == typeof(ulong)) && (this.DbType == DataBase.Enums.DbType.Sql2000))
                {
                    ulong num2 = (ulong) value;
                    value = (long) num2;
                }
            }
            param.Value = value;
            if (flag)
            {
                param.DbType = System.Data.DbType.AnsiString;
                param.Size = 0x7fffffff;
            }
        }

        public bool TableExists(string tableName)
        {
            if (this.DbType == DataBase.Enums.DbType.MySql)
            {
                this.AddParameter("database", this.Connection.Database);
                this.AddParameter("tableName", tableName);
                return (this.ExecuteScalerSQL("select TABLE_NAME from information_schema.tables where table_schema=@database and TABLE_NAME=@tableName") != null);
            }
            if (this.DbType == DataBase.Enums.DbType.Sql2000)
            {
                this.AddParameter("tableName", tableName);
                return (this.ExecuteScalerSQL("select name from sysobjects where xtype='u' and name=@tableName") != null);
            }
            if (this.DbType != DataBase.Enums.DbType.Firebird)
            {
                throw new InvalidOperationException();
            }
            this.AddParameter("tableName", tableName.ToUpper());
            int num = (int) this.ExecuteScalerSQL("SELECT COUNT(RDB$RELATION_NAME) FROM RDB$RELATIONS WHERE (RDB$RELATION_NAME=@tableName)\r\n AND RDB$VIEW_SOURCE IS NULL");
            return (num > 0);
        }

        public int Update(string tableName, string keyField, IDictionary<string, object> data)
        {
            string[] fieldNames = new string[data.Count];
            fieldNames[0] = keyField;
            int index = 1;
            foreach (string str in data.Keys)
            {
                if (str != keyField)
                {
                    fieldNames[index] = str;
                    index++;
                }
            }
            return this.Update(tableName, fieldNames, data);
        }

        public int Update(string tableName, string[] fieldNames, IDictionary<string, object> data)
        {
            string sql = SqlHelper.BuildUpdateSql(tableName, fieldNames);
            this.BuildParameters(fieldNames, data);
            return this.ExecuteNonQuerySQL(sql);
        }

        internal static IDbCloudProvider CloudProvider
        {
            [CompilerGenerated]
            get
            {
                return cloudProvider;
            }
            [CompilerGenerated]
            set
            {
                cloudProvider = value;
            }
        }

        public DbCommand Command
        {
            get
            {
                return this.command;
            }
        }

        public int CommandTimeout
        {
            get
            {
                return this.command.CommandTimeout;
            }
            set
            {
                this.command.CommandTimeout = value;
            }
        }

        public DbConnection Connection
        {
            get
            {
                return this.connection;
            }
        }

        public DataBase.Enums.DbType DbType
        {
            get
            {
                return this.dbType;
            }
        }

        public bool FieldNameToLower
        {
            get
            {
                return this.fieldNameToLower;
            }
            set
            {
                this.fieldNameToLower = value;
            }
        }

        public bool HasBegunTransaction
        {
            get
            {
                return (this.command.Transaction != null);
            }
        }

        public bool IsLoggingSql
        {
            get
            {
                return this.isLoggingSql;
            }
            set
            {
                this.isLoggingSql = value;
            }
        }

        public bool IsSelecting
        {
            [CompilerGenerated]
            get
            {
                return this.isSelecting;
            }
            [CompilerGenerated]
            set
            {
                this.isSelecting = value;
            }
        }

        public long LastInsertedId
        {
            get
            {
                if (this.command is MySqlCommand)
                {
                    return ((MySqlCommand) this.command).LastInsertedId;
                }
                if (!(this.command is SqlCommand))
                {
                    throw new InvalidOperationException("不支持LastInsertedId：" + this.command.GetType());
                }
                return Convert.ToInt64(this.ExecuteScalerSQL("select @@IDENTITY"));
            }
        }

        internal static Hashtable MysqlConnectionInfos
        {
            get
            {
                return mysqlConnectionInfos;
            }
        }

        public IHashObject OutputParameters
        {
            get
            {
                if (this.outputParameters == null)
                {
                    throw new InvalidOperationException("还没有调用过存储过程");
                }
                return this.outputParameters;
            }
        }

        public class _ScriptDbItemList
        {
            public int dataType = 1;
            public string[] fields;
            public int[] fieldSizes;
            public int[] fieldTypes;
            public object[] rows;
        }

        public delegate void SqlStatementExecutedEventHandler(object sender, string statementText, int line, int position);
    }
}


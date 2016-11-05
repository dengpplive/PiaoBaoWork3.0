namespace DataBase.DbCommon
{
    using System;
    using System.Text;

    public static class SqlHelper
    {
        private const string deleteTemplate = "delete from {0} where {1}=@{1}";
        private const string insertTemplate = "insert into {0} ({1}) values({2})";
        private const string selectTemplate = "select {1} from {0}";
        private const string updateTemplate = "update {0} set {1} where {2}=@{2}";

        public static string BuildDeleteSql(string tableName, string keyField)
        {
            return string.Format("delete from {0} where {1}=@{1}", tableName, keyField);
        }

        public static string BuildInsertSql(string tableName, params string[] fieldNames)
        {
            string str = StringListToCommaText(fieldNames);
            string str2 = StringListToCommaText(fieldNames, "@");
            return string.Format("insert into {0} ({1}) values({2})", tableName, str, str2);
        }

        public static string BuildSelectSql(string tableName, params string[] fieldNames)
        {
            string str = StringListToCommaText(fieldNames);
            return string.Format("select {1} from {0}", tableName, str);
        }

        public static string BuildUpdateSql(string tableName, params string[] fieldNames)
        {
            string keyField = fieldNames[0];
            string[] otherFieldNames = new string[fieldNames.Length - 1];
            for (int i = 1; i < fieldNames.Length; i++)
            {
                otherFieldNames[i - 1] = fieldNames[i];
            }
            return BuildUpdateSql(tableName, keyField, otherFieldNames);
        }

        public static string BuildUpdateSql(string tableName, string keyField, params string[] otherFieldNames)
        {
            StringBuilder builder = new StringBuilder();
            foreach (string str in otherFieldNames)
            {
                if (builder.Length != 0)
                {
                    builder.Append(',');
                }
                builder.Append(str);
                builder.Append("=@");
                builder.Append(str);
            }
            return string.Format("update {0} set {1} where {2}=@{2}", tableName, builder.ToString(), keyField);
        }

        public static string GetQuotedStr(string str)
        {
            string str2 = "'";
            if (str != null)
            {
                str2 = str2 + str.Replace("'", "''");
            }
            return (str2 + "'");
        }

        public static string StringListToCommaText(string[] items)
        {
            return StringListToCommaText(items, string.Empty, string.Empty);
        }

        public static string StringListToCommaText(string[] items, string prefix)
        {
            return StringListToCommaText(items, prefix, string.Empty);
        }

        public static string StringListToCommaText(string[] items, string prefix, string postfix)
        {
            StringBuilder builder = new StringBuilder();
            foreach (string str in items)
            {
                if (builder.Length != 0)
                {
                    builder.Append(',');
                }
                builder.Append(prefix);
                builder.Append(str);
                builder.Append(postfix);
            }
            return builder.ToString();
        }
    }
}


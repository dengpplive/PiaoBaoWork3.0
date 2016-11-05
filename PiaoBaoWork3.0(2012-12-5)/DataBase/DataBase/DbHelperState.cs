namespace DataBase
{
    using DataBase.Enums;
    using System;
    using System.Data.Common;

    internal sealed class DbHelperState
    {
        private DbHelperCommandKind commandKind;
        private string commandText;
        private DbConnection connection;
        private string connectionString;
        private DataBase.Enums.DbType dbType;
        private bool hasBegunTransaction;
        private bool isSelecting;

        internal DbHelperState(DbConnection connection, string connectionString, DataBase.Enums.DbType dbType, bool hasBegunTransaction, DbHelperCommandKind commandKind, string commandText, bool isSelecting)
        {
            this.connection = connection;
            this.connectionString = connectionString;
            this.dbType = dbType;
            this.hasBegunTransaction = hasBegunTransaction;
            this.commandKind = commandKind;
            this.commandText = commandText;
            this.isSelecting = isSelecting;
        }

        public DbHelperCommandKind CommandKind
        {
            get
            {
                return this.commandKind;
            }
        }

        public string CommandText
        {
            get
            {
                return this.commandText;
            }
        }

        public DbConnection Connection
        {
            get
            {
                return this.connection;
            }
        }

        public string ConnectionString
        {
            get
            {
                return this.connectionString;
            }
        }

        public DataBase.Enums.DbType DbType
        {
            get
            {
                return this.dbType;
            }
        }

        public bool HasBegunTransaction
        {
            get
            {
                return this.hasBegunTransaction;
            }
        }

        public bool IsSelecting
        {
            get
            {
                return this.isSelecting;
            }
        }
    }
}


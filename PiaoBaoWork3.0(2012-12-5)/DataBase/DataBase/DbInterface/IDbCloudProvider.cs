namespace DataBase.DbInterface
{
    using DataBase;
    using System;

    internal interface IDbCloudProvider
    {
        bool OpenConnection(DbHelperState state);
    }
}


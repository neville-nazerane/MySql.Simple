using System;
using System.Collections.Generic;
using System.Text;

namespace MySql.Simple
{
    public interface IDBDirect
    {
        string GetDbConnection();
    }

    /// <summary>
    /// Implements the GetAll function as well
    /// </summary>
    public interface IDBDirectAll<T> : IDBDirect
        where T : IDBDirectAll<T>
    { }

    public static class DBDirectExtensions
    {

        public static int Add(this IDBDirect context)
        {
            var con = new DBContent(context, context.GetDbConnection());
            using (Database db = context.GetDbConnection())
            {
                db.Insert(con.Table).Keys(con.Inserts)
                    .Values(con.Values).Execute();
                int pk = db.GetLatestID();
                con.PrimaryKey.SetRawValue(pk);
                return pk;
            }
        }

        public static bool Update(this IDBDirect context)
        {
            var con = new DBContent(context, context.GetDbConnection());
            using (Database db = context.GetDbConnection())
            {
                var upd = db.Update(con.Table);
                UpdateQueryer.Pairer pairer = null;
                var pk = con.PrimaryKey;
                foreach (var p in con.UpdatePairs)
                    pairer = upd.Pair(p.Key, p.Value);
                return pairer.Where("@0 = @1", pk.Name, pk.GetValue()).Execute() == 1;
            }
        }

        public static bool Get(this IDBDirect context)
        {
            var con = new DBContent(context, context.GetDbConnection());
            var pk = con.PrimaryKey;
            using (Database db = context.GetDbConnection())
            {
                using (var data = db.Select(con.Selects).From(con.Table)
                    .Where("@0 = @1", pk.Name, pk.GetValue()).QuerySingle())
                {
                    if (data)
                    {
                        foreach (var f in con.Fields.FindAll(p => p.Name != pk.Name))
                            f.SetValue(data);
                        return true;
                    }
                }
            }
            return false;
        }

        public static bool Delete(this IDBDirect context)
        {
            var con = new DBContent(context, context.GetDbConnection());
            var pk = con.PrimaryKey;
            using (Database db = context.GetDbConnection())
            {
                return db.Delete(con.Table).Where("@0 = @1", pk.Name, pk.GetValue()).Execute() == 1;
            }
        }

        public static List<T> GetAll<T>(this IDBDirect context)
            where T : IDBDirect, new()
        {
            var con = new DBContent(context, context.GetDbConnection());

            var list = new List<T>();
            using (Database db = context.GetDbConnection())
            {
                using (var r = db.Select(con.Selects).From(con.Table)
                                .Query())
                    while (r.Read())
                    {
                        T obj = new T();
                        list.Add(obj);
                        con.context = obj;
                        foreach (var f in con.Fields)
                            f.SetValue(r);
                    }
                con.context = context;
            }
            return list;
        }

        public static List<T> GetAll<T>(this IDBDirectAll<T> context)
            where T : IDBDirectAll<T>, new()
            => GetAll<T>((IDBDirect) context);

    }

}

using System;
using System.Collections.Generic;
using System.Text;

namespace MySql.Simple.Extensions
{
    public static class QueryExtensions
    {

        public static int Execute(this string connectionStr, string query, params object[] values)
        {
            using (Database db = connectionStr)
                return db.Execute(query, values);
        }

        public static QueryResult.DataItem QueryValue(this string connectionStr, string query, params object[] values)
        {
            using (Database db = connectionStr)
                return db.QueryValue(query, values);
        }

    }
}

using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Text;

namespace MySql.Simple
{
    public class QueryUtil
    {

        public static string Queryfy(string sql, params object[] values)
        {
            string opened = sql;
            for (int i = 0; i < values.Length; i++)
            {
                
                string val;
                string val2;
                if (values[i] == null) val = val2 = "null";
                else
                {

                    if (values[i].GetType() == typeof(bool))
                    {
                        val = Convert.ToInt16((bool)values[i]).ToString();
                        val2 = val;
                    }
                    else if (values[i].GetType() == typeof(DateTime))
                    {
                        val = ((DateTime)values[i]).ToString("yyyy-MM-dd HH:mm:ss");
                        val2 = $"'{val}'";
                    }
                    else
                    {
                        val = values[i].ToString();
                        val2 = "'" + val.Replace("'", "''") + "'";
                    }
                }
                sql = sql.Replace("$" + i, val2);
                sql = sql.Replace("@" + i, val);
            }
            return sql;
        }

    }
}

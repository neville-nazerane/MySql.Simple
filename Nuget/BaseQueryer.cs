using System;
using System.Collections.Generic;
using System.Text;
using static MySql.Simple.QueryResult;

namespace MySql.Simple
{
    public abstract partial class BaseQueryer
    {

        protected Func<string> Maker;

        //public string FullQuery
        //{
        //    get
        //    {
        //        return Maker();
        //    }
        //}

        protected Database db;

        public BaseQueryer(Database db)
        {
            this.db = db;
        }

        public abstract class Ender
        {

            private Func<string> Maker;
            private Database db;

            public string FullQuery { get
                {
                    return Maker();
                }
            }

            protected Ender(Func<string> Maker, Database db)
            {
                this.Maker = Maker;
                this.db = db;
            }

            public int Execute()
            {
                return db.Execute(Maker());
            }

            public DataItem QueryValue()
            {
                return db.QueryValue(Maker());
            }

            public QueryResult QuerySingle()
            {
                return db.QuerySingle(Maker());
            }

            public QueryResult Query()
            {
                return db.Query(Maker());
            }

        }

        protected class SimplePair : Paired
        {
            public string Key { get; private set; }
            public object Value { get; private set; }

            public SimplePair(string Key, object Value)
            {
                this.Key = Key;
                this.Value = Value;
            }

        }

        protected interface Paired
        {
            string Key { get;  }
            object Value { get;  }
        }

        protected interface Tabled
        {

            string Table { get; }

        }

        protected static string Queryfy(string sql, params object[] values)
        {
            return QueryUtil.Queryfy(sql, values);
        }
    }
}

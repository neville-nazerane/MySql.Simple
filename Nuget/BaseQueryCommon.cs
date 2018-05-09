using System;
using System.Collections.Generic;
using System.Text;

namespace MySql.Simple
{
    public abstract partial class BaseQueryer
    {

        public delegate void pairAdder(string Key, object Value);

        public delegate Pairer pairGen(string Key, object Value);
        

        public class Pairer : Ender, Paired 
        {

            public string Key { get; private set; }
            public object Value { get; private set; }

            private pairGen _Pair { get; set; }

            public Pairer Pair(string key, object Value)
            {
                return _Pair(key, Value);
            }

            public Pairer(Func<string> Maker, Database db, pairGen Pair, string Key, object Value)
                : base(Maker, db)
            {
                this.Key = Key;
                this.Value = Value;
                _Pair = Pair;
            }

        }

        public delegate T whereGen<T>(string query, params object[] selects) where T : Wherer;
        public delegate T whereContitionsGen<T>(BuiltCondition condition) where T : Wherer;
        public class Wherer : Ender
        {
            public string Value { get; set; }
            
            public Wherer(Func<string> Maker, Database db)
                : base(Maker, db)
            {
                Value = null;
            }

            public Wherer(Func<string> Maker, Database db, string query, params object[] selects)
                : base(Maker, db)
            {
                Value = QueryUtil.Queryfy(query, selects);
            }

            public Wherer(Func<string> Maker, Database db, BuiltCondition condition)
                :base(Maker, db)
            {
                Value = condition.FullQuery;
            }

        }

    }
}

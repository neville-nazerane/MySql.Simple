using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Reflection;

namespace MySql.Simple
{
    public class InsertQueryer : BaseQueryer
    {

       // insert("tbl").pair("",i)
       // insert("tbl").keys("").values("")
       // insert("tbl", object)

        private Tabled insert;
        private List<Paired> pairs;
        private Keyer keys;
        private Valuer values;
        private SelectQueryer.Selecter select;

        private Func<string> MakeEnd;

        public InsertQueryer(Database db) 
            : base(db)
        {

            pairs = new List<Paired>();
            Maker = Make;

        }

        private string Make()
        {
            return string.Format(
                "INSERT INTO {0} {1}",
                insert.Table,
                MakeEnd()
                );
        }

        private string MakePairingEnd()
        {
            return string.Format(
                "({0}) VALUES ({1})",
                string.Join(", ", pairs.Select(p => p.Key)),
                string.Join(", ", pairs.Select(p => Queryfy("$0", p.Value)))
             );
        }

        private string MakeKeyValueEnd()
        {
            return string.Format(
                "({0}) VALUES ({1})",
                keys.Keys,
                values.Value
                );
        }

        private string MakeSelectEnd(Func<string> end)
        {
            return string.Format(
                "({0}) {1}",
                keys.Keys,
                end()
                );
        }

        private Pairer PairInit(string Key, object Value)
        {
            MakeEnd = MakePairingEnd;
            
            return Pair(Key, Value);
        }
        
        private Pairer Pair(string Key, object Value)
        {
            Pairer pair = new Pairer(Maker, db, Pair, Key, Value);
            pairs.Add(pair);
            return pair;
        }

        public Inserter Insert(string Table)
        {
            insert = new Inserter(PairInit, Keys, Table);
            return (Inserter) insert;
        }

        public Inserter2 Insert(string Table, object Data)
        {
            MakeEnd = MakePairingEnd;
            insert = new Inserter2(Maker, db, (k, v) => pairs.Add(new SimplePair(k, v)), Table, Data);
            return (Inserter2)insert;
        }


        private Keyer Keys(params string[] Keys)
        {
            keys = new Keyer(Values, Select, SelectQuery, string.Join(", ", Keys));
            MakeEnd = MakeKeyValueEnd;
            return keys;
        }

        private Valuer Values(params object[] Values)
        {
            values = new Valuer(Maker, db, Queryfy(
                string.Join(", ", Values.Select((v, i) => "$" + i)), 
                Values));
            return values;
        }

        private SelectQueryer.Selecter Select(params string[] selects)
        {
            select = new SelectQueryer(db,
                old =>
                {
                    MakeEnd = () => MakeSelectEnd(old);
                    return Make;
                }
                ).Select(selects);
            return select;
        }

        SelectQueryer.Selecter SelectQuery(string sql, params object[] values)
        {
            select = new SelectQueryer(db,
                old =>
                {
                    MakeEnd = () => MakeSelectEnd(old);
                    return Make;
                }
                ).Select(QueryUtil.Queryfy(sql, values));
            return select;
        }

        public class Inserter : Tabled
        {

            public pairGen Pair { get; private set; }
            public keyGen Keys { get; private set; }

            public string Table { get; private set; }

            public Inserter(pairGen Pair, keyGen Keys, string Table)
            {
                this.Pair = Pair;
                this.Keys = Keys;
                this.Table = Table;
            }

        }

        public delegate SelectQueryer.Selecter selectGen(params string[] selects);
        public delegate SelectQueryer.Selecter selectGen2(string sql, params object[] values);
        public delegate Keyer keyGen(params string[] Keys);
        public class Keyer
        {

            public string Keys { get; private set; }

            public valueGen Values { get; private set; }

            public selectGen Select;

            public selectGen2 SelectQuery;

            public Keyer(valueGen Values, selectGen Select, selectGen2 SelectQuery, string Keys)
            {
                this.Values = Values;
                this.Select = Select;
                this.SelectQuery = SelectQuery;
                this.Keys = Keys;
            }

        }

        public delegate Valuer valueGen(params object[] Value);
        public class Valuer : Ender
        {


            public string Value { get; private set; }

            public Valuer(Func<string> Maker, Database db, string Value)
                : base(Maker, db)
            {
                this.Value = Value;
            }

        }

        
        public class Inserter2 : Ender, Tabled
        {

            public string Table { get; private set; }

            public Inserter2(Func<string> Maker, Database db, pairAdder Add, string Table, object obj)
                : base(Maker, db)
            {
                this.Table = Table;
                
                foreach (var prop in obj.GetType().GetProperties())
                {
                    Add(prop.Name, prop.GetValue(obj));
                }

            }

        }
        
        

    }
}

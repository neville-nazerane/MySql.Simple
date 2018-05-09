using System;
using System.Collections.Generic;
using System.Text;

using System.Linq;
using System.Reflection;

namespace MySql.Simple
{
    public class UpdateQueryer : BaseQueryer
    {

        // Update("tbl").pair().pair()
        // Update("tbl", object)

        private List<Paired> pairs;
        private Updatable update;
        private Wherer where;

        public UpdateQueryer(Database db) 
            : base(db)
        {
            Maker = Make;
            where = new Wherer(Maker, db);
            pairs = new List<Paired>();
        }

        private string Make()
        {
            return string.Format(
                    "UPDATE {0} SET {1} {2}",
                    update.Table,
                    string.Join(", ", pairs.Select(p => Queryfy("@0 = $1", p.Key, p.Value))),
                    where.Value == null ? "" : "WHERE " + where.Value
                );
        }

        public Updater Update(string Table)
        {
            update = new Updater(Table, Pair);

            return (Updater) update;
        }

        public Updater2 Update(string Table, object obj)
        {
            update = new Updater2(Maker, db, Where,  WhereCondition, (k, v) => pairs.Add(new SimplePair(k, v)), Table, obj);
            return (Updater2)update;
        }

        private Pairer Pair(string Key, object Value)
        {
            var pair = new Pairer(Maker, db, Where, WhereCondition, Pair, Key, Value);
            pairs.Add(pair);
            return pair;
        }

        private Wherer Where(string Query, object[] Selects)
        {
            where = new Wherer(Maker, db, Query, Selects);
            return where;
        }

        private Wherer WhereCondition(BuiltCondition condition)
        {
            where = new Wherer(Maker, db, condition);
            return where;
        }

        private interface Updatable
        {

            string Table { get; }

        }

        public class Updater : Updatable
        {

            public string Table { get; private set; }

            public pairGen Pair;

            public Updater(string Table, pairGen Pair)
                
            {
                this.Table = Table;
                this.Pair = Pair;
            }

        }

        public class Updater2 : Ender, Updatable
        {

            public whereGen<Wherer> Where { get; private set; }

            public whereContitionsGen<Wherer> WhereCondition { get; private set; }

            public string Table { get; private set; }

            public Updater2(Func<string> Maker, Database db, whereGen<Wherer> Where, whereContitionsGen<Wherer> WhereCondition , pairAdder Add, string Table, object pairs)
                : base(Maker, db)
            {
                this.Table = Table;
                this.Where = Where;
                this.WhereCondition = WhereCondition;
                foreach (var prop in pairs.GetType().GetProperties())
                {
                    Add(prop.Name, prop.GetValue(pairs));
                }

            }

        }

        public new delegate Pairer pairGen(string Key, object Value);
         
        public new class Pairer : BaseQueryer.Pairer
        {

            public whereGen<Wherer> Where { get; private set; }
            
            public whereContitionsGen<Wherer> WhereCondition { get; private set; }

            public Pairer(Func<string> Maker, Database db, whereGen<Wherer> Where, whereContitionsGen<Wherer> WhereCondition, BaseQueryer.pairGen Pair, string Key, object Value) 
                : base(Maker, db, Pair, Key, Value)
            {

                this.Where = Where;
                this.WhereCondition = WhereCondition;
            }

            public new Pairer Pair(string key, object Value)
            {
                return (Pairer) base.Pair(key, Value);
            }

        }

    }
}

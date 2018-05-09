using System;
using System.Collections.Generic;
using System.Text;

namespace MySql.Simple
{
    public class DeleteQueryer : BaseQueryer
    {


        Wherer where;
        Deleter delete;

        public DeleteQueryer(Database db)
            : base(db)
        {
            Maker = Make;
            where = new Wherer(Maker, db);
        }

        public Deleter Delete(string Table)
            => delete = new Deleter(Maker, db, Table, Where, WhereCondition);

        private Wherer Where(string Query, object[] Selects)
        => where = new Wherer(Maker, db, Query, Selects);

        private Wherer WhereCondition(BuiltCondition condition)
        => where = new Wherer(Maker, db, condition);

        string Make() => $"DELETE FROM {delete.Table}" + 
                    (where.Value == null ? "" : " WHERE " + where.Value);

        public class Deleter : Ender
        {
            public whereGen<Wherer> Where { get; private set; }

            public whereContitionsGen<Wherer> WhereCondition { get; private set; }
            
            public string Table { get; private set; }

            public Deleter(Func<string> Maker, Database db, string Table, whereGen<Wherer> Where, whereContitionsGen<Wherer> WhereCondition)
                                        : base(Maker, db)
            {
                this.Table = Table;
                this.Where = Where;
                this.WhereCondition = WhereCondition;
            }
            
        }

    }
}

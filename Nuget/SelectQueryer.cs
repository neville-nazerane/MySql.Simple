using System;
using System.Collections.Generic;
using System.Text;
using static MySql.Simple.QueryResult;
using System.Linq;


namespace MySql.Simple
{
    public class SelectQueryer : BaseQueryer
    {

        private Selecter _Select;
        private Fromer _From;
        private Wherer _Where;
        private Orderer _Order;
        private Limiter _Limit;
        private List<Joiner> _Joins;

        internal SelectQueryer(Database db)
            : base(db)
        {
            _Select = new Selecter(From);
            _Where = new Wherer(Maker, db);
            _Joins = new List<Joiner>();
            Maker = Make;
        }

        internal SelectQueryer(Database db, Func<Func<string>, Func<string>> ChangedMaker)
           : this(db)
        {
            Maker = ChangedMaker(Make);
        }
        
        internal string Make()
        {
            string str = string.Format("SELECT {0} FROM {1}{2}{3}",
                                _Select.Value, _From.Value, string.Join("", _Joins.Select(j => j.Value)),
                               _Where.Value == null ? "" : " WHERE " + _Where.Value);
            if (_Order != null)
            {
                str = string.Format("{0} ORDER BY {1}", str, _Order.Value);
            }
            return str;
        }
        
        public Selecter Select(params string[] selects)
        {
            _Select = new Selecter(From, selects);
            return _Select;
        }

        public Fromer From(params string[] selects)
        {
            _From = new Fromer(Maker, Join, LeftJoin, RightJoin, FullJoin,
                        Where, WhereCondition, Order, Limit, db, selects);
            return _From;
        }

        private Fromer Join(string table, string onQuery, params object[] objs)
        {
            _Joins.Add(new Joiner("", table, onQuery, objs));
            return _From;
        }

        private Fromer LeftJoin(string table, string onQuery, params object[] objs)
        {
            _Joins.Add(new Joiner(" Left", table, onQuery, objs));
            return _From;
        }

        private Fromer RightJoin(string table, string onQuery, params object[] objs)
        {
            _Joins.Add(new Joiner(" Right", table, onQuery, objs));
            return _From;
        }

        private Fromer FullJoin(string table, string onQuery, params object[] objs)
        {
            _Joins.Add(new Joiner(" Full", table, onQuery, objs));
            return _From;
        }

        private Wherer Where(string query, params object[] selects)
        {
            _Where = new Wherer(Maker, db, Order, Limit, query, selects);
            return _Where;
        }

        private Wherer WhereCondition(BuiltCondition condition)
        {
            _Where = new Wherer(Maker, db, Order, Limit, condition);
            return _Where;
        }

        private Orderer Order(string OrderBy)
        {
            _Order = new Orderer(Maker, db, OrderBy, Limit);
            return _Order;
        }

        private Limiter Limit(int limit)
        {
            _Limit = new Limiter(Maker, db, limit);
            return _Limit;
        }

        public delegate Fromer fromGen(params string[] selects);

        public class Selecter
        {
            public string Value { get; set; }

            public fromGen From { get; private set; }

            public Selecter(fromGen From)
            {
                this.From = From;
                Value = "*";
            }

            public Selecter(fromGen From, params string[] selects)
                : this(From)
            {
                Value = string.Join(", ", selects);
            }
        }

        public delegate Fromer joinGen(string table, string on, params object[] objs);
        public class Joiner
        {
            public string Value { get; set; }

            internal Joiner(string type, string table, string on, params object[] objs)
            {
                Value = string.Format("{0} JOIN {1} ON {2}", 
                                type, table, QueryUtil.Queryfy(on, objs));
            }

        }
        
        public new class Wherer : BaseQueryer.Wherer
        {

            public orderGen Order { get; set; }
            public limitGen Limit { get; set; }

            public Wherer(Func<string> Maker, Database db)
               : base(Maker, db)
            {

            }

            public Wherer(Func<string> Maker, Database db, orderGen Order, limitGen Limit, string query, params object[] selects)
                : base(Maker, db, query, selects)
            {
                this.Order = Order;
                this.Limit = Limit;
            }

            public Wherer(Func<string> Maker, Database db, orderGen Order, limitGen Limit, BuiltCondition condition)
                : base(Maker, db, condition)
            {
                this.Order = Order;
                this.Limit = Limit;
            }

        }

        public class Fromer : Ender
        {
            public string Value { get; set; }
            public whereGen<Wherer> Where { get; private set; }
            public whereContitionsGen<Wherer> WhereCondition { get; private set; }
            public orderGen Order { get; set; }
            public limitGen Limit { get; set; }

            public joinGen Join { get; private set; }
            public joinGen LeftJoin { get; private set; }
            public joinGen RightJoin { get; private set; }
            public joinGen FullJoin { get; private set; }

            public Fromer(Func<string> Maker, 
                joinGen Join, joinGen LeftJoin, joinGen RightJoin, joinGen FullJoin,
                whereGen<Wherer> Where, whereContitionsGen<Wherer> WhereConditional, 
                orderGen Order, limitGen Limit, Database db, params string[] selects)
                : base(Maker, db)
            {
                this.Join = Join;
                this.LeftJoin = LeftJoin;
                this.RightJoin = RightJoin;
                this.FullJoin = FullJoin;
                this.Where = Where;
                this.WhereCondition = WhereConditional;
                this.Order = Order;
                this.Limit = Limit;
                Value = string.Join(", ", selects);
            }

        }

        public delegate Orderer orderGen(string OrderBy);

        public class Orderer : Ender
        {
            public string Value { get; set; }

            public limitGen Limit  { get; set; }

            public Orderer(Func<string> Maker, Database db, string OrderBy, limitGen Limit)
                : base(Maker, db)
            {
                Value = OrderBy;
                this.Limit = Limit;
            }

        }

        public delegate Limiter limitGen(int limit);
        public class Limiter : Ender
        {

            public int Value { get; set; }

            public Limiter(Func<string> Maker, Database db, int limit) 
                : base(Maker, db)
            {
                this.Value = limit;
            }
        }

    }
}

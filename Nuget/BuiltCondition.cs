using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MySql.Simple
{

    public class BuiltCondition
    {

        public static BuiltCondition Default 
                            => new BuiltCondition("") { IsDefault = true };

        public string Query { private get; set; }

        public object Value { private get; set; }

        private bool IsDefault;

        bool IsNOT = false;

        private List<Append> Appends;

        public BuiltCondition() => Appends = new List<Append>();

        public BuiltCondition(string Query, object Value = null)
            : this()
        {
            this.Query = Query;
            this.Value = Value;
        }

        private BuiltCondition Add(BuiltCondition condition, bool IsAdd)
        {
            Appends.Add(new Append()
            {
                Condition = condition,
                IsAdd = IsAdd
            });
            return this;
        }

        public static BuiltCondition operator & (BuiltCondition c1, BuiltCondition c2) => c1.And(c2);

        public static BuiltCondition operator | (BuiltCondition c1, BuiltCondition c2) => c1.Or(c2);

        public static BuiltCondition operator !(BuiltCondition c)
        {
            c.IsNOT = !c.IsNOT;
            return c;
        }

        public BuiltCondition And(BuiltCondition condition)
        {
            return Add(condition, true);
        }

        public BuiltCondition Or(BuiltCondition condition)
        {
            return Add(condition, false);
        }

        private string Built
            => Value == null ? Query :  
                        (Query.IndexOfAny("='$@%".ToCharArray()) == -1?
                           QueryUtil.Queryfy("@0 = $1", Query, Value) : QueryUtil.Queryfy(Query, Value));

        // (IsNOT ? " NOT " : "")
        public virtual string FullQuery
            => (IsNOT ? " NOT " : "") + Built +
                string.Join("", Appends.Select((a, i) =>
                            (i == 0 && IsDefault ? "" : (a.IsAdd ? " AND " : " OR "))
                                   + string.Format(a.HasAppends ?
                                       (a.Condition.IsNOT && !(a.Condition.IsNOT = false) ? " NOT " : "") +
                                       "({0})" : "{0}", a.Condition.FullQuery)
                            ));

        private class Append
        {
            public BuiltCondition Condition { get; set; }
            public bool IsAdd { get; set; }
            public bool HasAppends => Condition.Appends.Count != 0;
        }

    }

}

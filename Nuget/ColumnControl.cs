using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace MySql.Simple
{
    public class ColumnControl
    {

        // might vary from PropertyName
        public string Key { get; set; }

        public object Value => GetValue();

        public void SetValue(QueryResult result) => AssignValue(result);

        internal string Selects
            => string.Concat(StackedControls.Select(c => c.Key));

        internal Action<QueryResult> AssignValue;
        internal Action<object> AssignRawValue;
        internal Func<object> GetValue;
        internal Func<BuiltCondition> GetCondition;

        internal Func<List<ColumnControl>> OtherControls;
        private List<ColumnControl> _stackedControls;
        internal List<ColumnControl> StackedControls
        {
            get => _stackedControls ?? new List<ColumnControl> { this };
            private set { _stackedControls = value; }
        }

        internal string PropertyName;
        internal bool IsComputed;

        internal ColumnControl()
        {

        }
        
        ColumnControl Other()
        {
            var others = OtherControls();
            return new ColumnControl
            {
                IsComputed = true,
                StackedControls = others,
                OtherControls = () => others
             };
            
        }


        public static ColumnControl operator !(ColumnControl control)
        {
            var other = control.Other();
            other.GetCondition = () => !control.GetCondition();
            return other;
        }

        public static ColumnControl operator -(ColumnControl c1, ColumnControl c2)
        {

            var stack = c1.IsComputed ? c1.StackedControls : c1.OtherControls();
            stack = stack.FindAll(c => c.Key != c2.Key);
            return new ColumnControl {
                IsComputed = true,
                StackedControls = stack,
            };
        }

        public static ColumnControl operator +(ColumnControl c1, ColumnControl c2)
        {
            var stack = c1.IsComputed ? c1.StackedControls :
                        new List<ColumnControl> { c1, c2 };
            return new ColumnControl {
                IsComputed = true,
                StackedControls = stack,
            };
        }

        public static BuiltCondition operator &(ColumnControl c1, ColumnControl c2)
            => c1.GetCondition() & c2.GetCondition();

        public static BuiltCondition operator |(ColumnControl c1, ColumnControl c2)
            => c1.GetCondition() | c2.GetCondition();

        public static implicit operator BuiltCondition(ColumnControl control)
            => control.GetCondition();
        
        public BuiltCondition this [object value]
            => Conditioner.Build($"{Key} = $0", value);


    }

   
}

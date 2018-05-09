using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

using System.Linq;

namespace MySql.Simple
{
    public class ColumnController<T>
    {

        List<ColumnControl> _Controls;
        internal List<ColumnControl> Controls
        {
            get
            {
                VerifyControls();
                return _Controls;
            }
        }
        UniversalConditioner<T> conditioner;

        internal T Content;

        public ColumnController(T Content)
        {
            AssignContent(Content);
        }

        internal ColumnController()
        {

        }

        internal void AssignContent(T Content)
        {
            this.Content = Content;
            conditioner = Conditioner.GetUniversalConditioner(Content);

        }

        Type _typ;
        internal Type typ {
            get => _typ ?? typeof(T);
            set => _typ = value;
        }

        object _ControlContent;
        internal object ControlContent
        {
            get => _ControlContent ?? Content;
            set => _ControlContent = value;
        }

        void VerifyControls()
        {
            if (_Controls == null)
            {
                _Controls = new List<ColumnControl>();
                //var typ = typeof(T);
                foreach (var p in typ.GetProperties(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance))
                    if (p.PropertyType == typeof(ColumnControl))
                        Controls.Add((ColumnControl)p.GetValue(ControlContent));
            }
        }

        public ColumnControl Build<K>(Expression<Func<T, K>> expression)
        {
            var member = expression.Body.GetMember();
            var typ = typeof(T);
            var prop = typ.GetProperty(member.Name);
            var col = new ColumnControl
            {
                PropertyName = member.Name,
                Key = member.Name,
                IsComputed = false,
                OtherControls = () => Controls.FindAll(c => c.Key != member.Name),
                GetValue = () => expression.Compile()(Content),
                AssignValue = r => prop.SetValue(Content,
                                                r.Get<K>(member.Name)),
                AssignRawValue = o => prop.SetValue(Content, (K)o),
                GetCondition = () => conditioner.Build(expression),
            };
            return col;
        }

        public ColumnControl Build<K>(Expression<Func<T, K>> expression, string Key)
        {
            var col = Build(expression);
            col.Key = Key;
            return col;
        }

        public BuiltCondition this[Expression<Func<T, int?>> expression] => Build(expression);
        public BuiltCondition this[Expression<Func<T, float?>> expression] => Build(expression);
        public BuiltCondition this[Expression<Func<T, double?>> expression] => Build(expression);
        public BuiltCondition this[Expression<Func<T, DateTime?>> expression] => Build(expression);
        public BuiltCondition this[Expression<Func<T, bool?>> expression] => Build(expression);
        public BuiltCondition this[Expression<Func<T, byte?>> expression] => Build(expression);
        public BuiltCondition this[Expression<Func<T, char?>> expression] => Build(expression);
        public BuiltCondition this[Expression<Func<T, decimal?>> expression] => Build(expression);
        public BuiltCondition this[Expression<Func<T, long?>> expression] => Build(expression);

        public BuiltCondition this[Expression<Func<T, string>> expression] => Build(expression);
        public BuiltCondition this[Expression<Func<T, int>> expression] => Build(expression);
        public BuiltCondition this[Expression<Func<T, float>> expression] => Build(expression);
        public BuiltCondition this[Expression<Func<T, double>> expression] => Build(expression);
        public BuiltCondition this[Expression<Func<T, DateTime>> expression] => Build(expression);
        public BuiltCondition this[Expression<Func<T, bool>> expression] => Build(expression);
        public BuiltCondition this[Expression<Func<T, byte>> expression] => Build(expression);
        public BuiltCondition this[Expression<Func<T, char>> expression] => Build(expression);
        public BuiltCondition this[Expression<Func<T, decimal>> expression] => Build(expression);
        public BuiltCondition this[Expression<Func<T, long>> expression] => Build(expression);



        public BuiltCondition this[Expression<Func<T, int?>> expression, string Key] => Build(expression, Key);
        public BuiltCondition this[Expression<Func<T, float?>> expression, string Key] => Build(expression, Key);
        public BuiltCondition this[Expression<Func<T, double?>> expression, string Key] => Build(expression, Key);
        public BuiltCondition this[Expression<Func<T, DateTime?>> expression, string Key] => Build(expression, Key);
        public BuiltCondition this[Expression<Func<T, bool?>> expression, string Key] => Build(expression, Key);
        public BuiltCondition this[Expression<Func<T, byte?>> expression, string Key] => Build(expression, Key);
        public BuiltCondition this[Expression<Func<T, char?>> expression, string Key] => Build(expression, Key);
        public BuiltCondition this[Expression<Func<T, decimal?>> expression, string Key] => Build(expression, Key);
        public BuiltCondition this[Expression<Func<T, long?>> expression, string Key] => Build(expression, Key);

        public BuiltCondition this[Expression<Func<T, string>> expression, string Key] => Build(expression, Key);
        public BuiltCondition this[Expression<Func<T, int>> expression, string Key] => Build(expression, Key);
        public BuiltCondition this[Expression<Func<T, float>> expression, string Key] => Build(expression, Key);
        public BuiltCondition this[Expression<Func<T, double>> expression, string Key] => Build(expression, Key);
        public BuiltCondition this[Expression<Func<T, DateTime>> expression, string Key] => Build(expression, Key);
        public BuiltCondition this[Expression<Func<T, bool>> expression, string Key] => Build(expression, Key);
        public BuiltCondition this[Expression<Func<T, byte>> expression, string Key] => Build(expression, Key);
        public BuiltCondition this[Expression<Func<T, char>> expression, string Key] => Build(expression, Key);
        public BuiltCondition this[Expression<Func<T, decimal>> expression, string Key] => Build(expression, Key);
        public BuiltCondition this[Expression<Func<T, long>> expression, string Key] => Build(expression, Key);


    }

}

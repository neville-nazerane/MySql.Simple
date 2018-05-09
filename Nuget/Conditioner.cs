using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Linq.Expressions;

namespace MySql.Simple
{

    public class Conditioner {

        public static BuiltCondition Default => BuiltCondition.Default;

        public static BuiltCondition Build(string Query, object Value = null)
        => new BuiltCondition
        {
            Query = Query,
            Value = Value
        };

        public BuiltCondition this[string Query, object Value = null] => Build(Query, Value);

        public static EquateConditioner<T> GetEquateConditioner<T>(T Content)
            => new EquateConditioner<T>(Content);

        public static LikeConditioner<T> GetLikeConditioner<T>(T Content)
            => new LikeConditioner<T>(Content);

        public static UniversalConditioner<T> GetUniversalConditioner<T>(T Content)
            => new UniversalConditioner<T>(Content);

    }

    public class EquateConditioner<T>
    {

        public T Content { get; set; }

        public EquateConditioner(T Content)
        {
            this.Content = Content;
        }

        public BuiltCondition Build<K>(Expression<Func<T, K>> expression)
        {
            var member = expression.Body.GetMember();
            return new BuiltCondition {
                Query = member.Name,
                Value = expression.Compile()(Content)
            };
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

    }

    public class LikeConditioner<T>
    {

        public T Content { get; set; }

        public LikeConditioner(T Content)
        {
            this.Content = Content;
        }

        public BuiltCondition Build<K>(Expression<Func<T, K>> expression, string format = "@0%")
        {
            var member = expression.Body.GetMember();
            return new BuiltCondition
            {
                Query = $"{member.Name} LIKE '{format}'",
                Value = expression.Compile()(Content)
                        .ToString().Replace("%", "\\%").Replace("_", "\\_")
            };
        }

        public BuiltCondition this[Expression<Func<T, int?>> expression, string format = "@0%"] => Build(expression, format);
        public BuiltCondition this[Expression<Func<T, float?>> expression, string format = "@0%"] => Build(expression, format);
        public BuiltCondition this[Expression<Func<T, double?>> expression, string format = "@0%"] => Build(expression, format);
        public BuiltCondition this[Expression<Func<T, DateTime?>> expression, string format = "@0%"] => Build(expression, format);
        public BuiltCondition this[Expression<Func<T, bool?>> expression, string format = "@0%"] => Build(expression, format);
        public BuiltCondition this[Expression<Func<T, byte?>> expression, string format = "@0%"] => Build(expression, format);
        public BuiltCondition this[Expression<Func<T, char?>> expression, string format = "@0%"] => Build(expression, format);
        public BuiltCondition this[Expression<Func<T, decimal?>> expression, string format = "@0%"] => Build(expression, format);
        public BuiltCondition this[Expression<Func<T, long?>> expression, string format = "@0%"] => Build(expression, format);

        public BuiltCondition this[Expression<Func<T, string>> expression, string format = "@0%"] => Build(expression, format);
        public BuiltCondition this[Expression<Func<T, int>> expression, string format = "@0%"] => Build(expression, format);
        public BuiltCondition this[Expression<Func<T, float>> expression, string format = "@0%"] => Build(expression, format);
        public BuiltCondition this[Expression<Func<T, double>> expression, string format = "@0%"] => Build(expression, format);
        public BuiltCondition this[Expression<Func<T, DateTime>> expression, string format = "@0%"] => Build(expression, format);
        public BuiltCondition this[Expression<Func<T, bool>> expression, string format = "@0%"] => Build(expression, format);
        public BuiltCondition this[Expression<Func<T, byte>> expression, string format = "@0%"] => Build(expression, format);
        public BuiltCondition this[Expression<Func<T, char>> expression, string format = "@0%"] => Build(expression, format);
        public BuiltCondition this[Expression<Func<T, decimal>> expression, string format = "@0%"] => Build(expression, format);
        public BuiltCondition this[Expression<Func<T, long>> expression, string format = "@0%"] => Build(expression, format);

    }

    public class UniversalConditioner<T>
    {
        
        EquateConditioner<T> _equateConditioner;
        LikeConditioner<T> _likeConditioner;

        public UniversalConditioner(T Content)
        {
            _equateConditioner = new EquateConditioner<T>(Content);
            _likeConditioner = new LikeConditioner<T>(Content);
        }

        public static BuiltCondition Build(string Query, object Value = null)
            => Conditioner.Build(Query, Value);

        public BuiltCondition Build<K>(Expression<Func<T, K>> expression)
            => _equateConditioner.Build(expression);

        public BuiltCondition Build<K>(Expression<Func<T, K>> expression, string format)
            => _likeConditioner.Build(expression, format);

        public BuiltCondition BuildLike<K>(Expression<Func<T, K>> expression, string likeFormat = "@0%")
            => _likeConditioner.Build(expression, likeFormat);

        public BuiltCondition this[string Query, object Value = null] => Build(Query, Value);

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


        public BuiltCondition this[Expression<Func<T, int?>> expression, string format = "@0%"] => Build(expression, format);
        public BuiltCondition this[Expression<Func<T, float?>> expression, string format = "@0%"] => Build(expression, format);
        public BuiltCondition this[Expression<Func<T, double?>> expression, string format = "@0%"] => Build(expression, format);
        public BuiltCondition this[Expression<Func<T, DateTime?>> expression, string format = "@0%"] => Build(expression, format);
        public BuiltCondition this[Expression<Func<T, bool?>> expression, string format = "@0%"] => Build(expression, format);
        public BuiltCondition this[Expression<Func<T, byte?>> expression, string format = "@0%"] => Build(expression, format);
        public BuiltCondition this[Expression<Func<T, char?>> expression, string format = "@0%"] => Build(expression, format);
        public BuiltCondition this[Expression<Func<T, decimal?>> expression, string format = "@0%"] => Build(expression, format);
        public BuiltCondition this[Expression<Func<T, long?>> expression, string format = "@0%"] => Build(expression, format);

        public BuiltCondition this[Expression<Func<T, string>> expression, string format = "@0%"] => Build(expression, format);
        public BuiltCondition this[Expression<Func<T, int>> expression, string format = "@0%"] => Build(expression, format);
        public BuiltCondition this[Expression<Func<T, float>> expression, string format = "@0%"] => Build(expression, format);
        public BuiltCondition this[Expression<Func<T, double>> expression, string format = "@0%"] => Build(expression, format);
        public BuiltCondition this[Expression<Func<T, DateTime>> expression, string format = "@0%"] => Build(expression, format);
        public BuiltCondition this[Expression<Func<T, bool>> expression, string format = "@0%"] => Build(expression, format);
        public BuiltCondition this[Expression<Func<T, byte>> expression, string format = "@0%"] => Build(expression, format);
        public BuiltCondition this[Expression<Func<T, char>> expression, string format = "@0%"] => Build(expression, format);
        public BuiltCondition this[Expression<Func<T, decimal>> expression, string format = "@0%"] => Build(expression, format);
        public BuiltCondition this[Expression<Func<T, long>> expression, string format = "@0%"] => Build(expression, format);

    }

}

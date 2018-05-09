using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace MySql.Simple
{
    static class UtilityExtensions
    {

        internal static MemberInfo GetMember(this Expression expression)
        {

            var member = expression as MemberExpression;
            if (member == null)
            {
                var unary = expression as UnaryExpression;
                if (unary == null)
                    throw new InvalidCastException("Data type in lamda is invalid");
                else
                {
                    return unary.Operand.GetMember();
                }
            }
            else return member.Member;
        }

    }
}

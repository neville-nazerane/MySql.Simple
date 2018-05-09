using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp
{
    static class Class1
    {


        public static string IfEmpty(this string str, string alternate)
        {
            return str?.Length == 0 ? str : alternate;
        }
    }
}

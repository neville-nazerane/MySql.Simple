using MySql.Simple;
using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp
{
    class Person : IDBDirect
    { 
        public int ID { get; set; }
        public string FullName { get; set; }
        public string Address { get; set; }
        public string Job { get; set; }
        public bool IsDead { get; set; }
        public string GetDbConnection() => Connection.Str;
    }
}

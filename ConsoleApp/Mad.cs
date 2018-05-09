using MySql.Simple;
using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp
{
    class Mad : IDBDirectAll<Mad>
    {

        public int ID { get; set; }

        public string Name { get; set; }

        public bool Gone { get; set; }

        public DateTime? Dated { get; set; }

        public string GetDbConnection() => Connection.Str;

    }
}

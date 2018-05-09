using System;
using System.Collections.Generic;
using System.Text;

namespace MySql.Simple
{
    public class OpenDatabase : IDisposable
    {

        public Database Database { get; private set; }

        Action OnDispose;

        internal OpenDatabase(Database Database, Action OnDispose)
        {
            this.Database = Database;
            this.OnDispose = OnDispose;
        }

        public void Dispose()
        {
            Database.Dispose();
            OnDispose();
        }
    }
}

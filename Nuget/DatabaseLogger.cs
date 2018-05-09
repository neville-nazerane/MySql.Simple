using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace MySql.Simple
{
    public enum LogLevel { Nothing, Error, Warning, Info };

    internal static class DatabaseLoggerExtension
    {
        internal static void Add(this DatabaseLogger logger, DatabaseLog log)
        {
            if (logger != null && logger.Level >= log.Level)
                logger.AddIn(log);
        }
    }

    public class DatabaseLogger : IEnumerable<DatabaseLog>
    {

        List<DatabaseLog> _Logs { get; set; }

        public LogLevel Level { get; set; }

        public DatabaseLogger()
        {
            _Logs = new List<DatabaseLog>();
            Level = LogLevel.Error;
        }

        internal void AddIn(DatabaseLog log) => Add(log);

        protected virtual void Add(DatabaseLog log)
            => _Logs.Add(log);

        public IEnumerator<DatabaseLog> GetEnumerator()
            => _Logs.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => _Logs.GetEnumerator();
    }

    public class DatabaseLog
    {

        public string Message { get; set; }

        public string Query { get; set; }

        public LogLevel Level { get; set; }

        public DateTime CreatedOn { get; private set; }

        internal DatabaseLog()
        {
            CreatedOn = DateTime.Now;   
        }

    }
}

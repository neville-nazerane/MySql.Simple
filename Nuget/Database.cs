using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Text;
using static MySql.Simple.QueryResult;

namespace MySql.Simple
{
    public partial class Database : IDisposable
    {

        private MySqlConnection _Connection;

        public MySqlConnection Connection
        {
            get { return _Connection; }
        }

        public Database(string ConnectionString)
        {
            _Connection = new MySqlConnection(ConnectionString);
            if (!string.IsNullOrEmpty(ConnectionString)) Connection.Open();
        }

        public static implicit operator Database(string ConnectionString)
        {
            return new Database(ConnectionString);
        }

        public static implicit operator MySqlConnection(Database db)
        {
            return db.Connection;
        }

        public int GetLatestID()
        {
            using (var id = QueryValue("SELECT LAST_INSERT_ID()"))
                return id;
        }

        public int GetUpdatedRowCount()
        {
            using (var c = QueryValue("SELECT ROW_COUNT()"))
                return c;
        }

        public bool HasUpdatedRows() => GetUpdatedRowCount() > 0;

        public SelectQueryer.Selecter Select(params string[] selects)
        {
            return new SelectQueryer(this).Select(selects);
        }

        public SelectQueryer.Fromer From(params string[] selects)
        {
            return new SelectQueryer(this).From(selects);
        }

        public InsertQueryer.Inserter Insert(string Table)
        {
            return new InsertQueryer(this).Insert(Table);
        }

        public InsertQueryer.Inserter2 Insert(string Table, object Data)
        {
            return new InsertQueryer(this).Insert(Table, Data);
        }

        public UpdateQueryer.Updater Update(string Table)
        {
            return new UpdateQueryer(this).Update(Table);
        }

        public UpdateQueryer.Updater2  Update(string Table, object Pairs)
        {
            return new UpdateQueryer(this).Update(Table, Pairs);
        }

        public DeleteQueryer.Deleter Delete(string Table)
            => new DeleteQueryer(this).Delete(Table);

        public int Execute(string sql, params object[] values)
        {
            sql =  QueryUtil.Queryfy(sql, values);
            var cmd = new MySqlCommand(sql, Connection);
            return cmd.ExecuteNonQuery();
        }

        public QueryResult Query(string sql, params object[] values)
        {
            sql = QueryUtil.Queryfy(sql, values);
            var cmd = new MySqlCommand(sql, Connection);
            return new QueryResult(cmd.ExecuteReader());
        }

        public QueryResult QuerySingle(string sql, params object[] values)
        {
            sql = QueryUtil.Queryfy(sql, values);
            var cmd = new MySqlCommand(sql, Connection);
            var qr = new QueryResult(cmd.ExecuteReader());
            qr.Read();
            return qr;
        }

        public DataItem QueryValue(string sql, params object[] values)
        {
            var result = QuerySingle(sql, values);
            return result[0];
        }

        public void Dispose()
        {
            Connection.Close();
            Connection.Dispose();
        }
    }
}

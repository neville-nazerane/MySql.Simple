using MySql.Data.MySqlClient;
using MySql.Data.Types;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Text;

using System.Reflection;


namespace MySql.Simple
{
    public class QueryResult : IDisposable
    {

        private MySqlDataReader _reader;
        public MySqlDataReader Reader => _reader;


        public QueryResult(MySqlDataReader reader)
        {
            _reader = reader;
        }

        public static implicit operator bool(QueryResult result) => result.Reader.HasRows;

        public static implicit operator MySqlDataReader(QueryResult result) => result.Reader;

        public bool Read() => Reader.Read();

        public T Get<T>(string key)
        {
            int ordinal = Reader.GetOrdinal(key);
            Type underlay = Nullable.GetUnderlyingType(typeof(T));
            if (Reader.IsDBNull(ordinal))
            {
                if (typeof(T).GetTypeInfo().IsValueType && underlay == null) // isn't a nullable field
                    throw new InvalidCastException();
                else return default(T);
            }

            if ((underlay ?? typeof(T)) == typeof(char))
                return (T)(object)(char)this[key];
            if ((underlay ?? typeof(T)) == typeof(bool))
                return (T)(object) Reader.GetBoolean(ordinal);
            return Reader.GetFieldValue<T>(ordinal);
        }

        public DataItem this[string key] => new DataItem(Reader, key);

        public DataItem this[int key] => new DataItem(Reader, key);


        public class DataItem : IDisposable
        {

            private MySqlDataReader reader;
            private dynamic key;
            
            public DataItem(MySqlDataReader reader, string key)
            {
                this.reader = reader;
                this.key = key;
            }

            public bool HasData() => reader.HasRows;

            public DataItem(MySqlDataReader reader, int key)
            {
                this.reader = reader;
                this.key = key;
            }

            public void Dispose()
            {
                reader.Dispose();
            }

            int Ordinal => key.GetType() == typeof(int) ? key : reader.GetOrdinal(key);

            static T? NullOrVal<T>(DataItem item, Func<MySqlDataReader, T> func)
                where T : struct
            {
                return item.reader.IsDBNull(item.Ordinal) ? null : (T?)func(item.reader);
            }

            public static implicit operator MySqlDataReader(DataItem item) => item.reader;

            public static implicit operator int(DataItem item) => item.reader.GetInt32(item.key);

            public static implicit operator int?(DataItem item) => NullOrVal<int>(item, r => r.GetInt32(item.key));

            public static implicit operator string(DataItem item) 
                => item.reader.IsDBNull(item.Ordinal) ? null : item.reader.GetString(item.key);

            public static implicit operator bool(DataItem item) => item.reader.GetBoolean(item.key);

            public static implicit operator bool? (DataItem item) => NullOrVal<bool>(item, r => r.GetBoolean(item.key));

            public static implicit operator byte(DataItem item) => item.reader.GetByte(item.key);

            public static implicit operator byte? (DataItem item) => NullOrVal<byte>(item, r => r.GetByte(item.key));

            public static implicit operator char(DataItem item) => item.reader.GetString(item.key).ToCharArray()[0];

            public static implicit operator char? (DataItem item) => NullOrVal<char>(item, r => r.GetChar(item.key));

            public static implicit operator DateTime(DataItem item) => item.reader.GetDateTime(item.key);

            public static implicit operator DateTime? (DataItem item) => NullOrVal<DateTime>(item, r => r.GetDateTime(item.key));

            public static implicit operator decimal(DataItem item) => item.reader.GetDecimal(item.key);

            public static implicit operator decimal? (DataItem item) => NullOrVal<decimal>(item, r => r.GetDecimal(item.key));

            public static implicit operator double(DataItem item) => item.reader.GetDouble(item.key);

            public static implicit operator double? (DataItem item) => NullOrVal<double>(item, r => r.GetDouble(item.key));

            public static implicit operator float(DataItem item)  => item.reader.GetFloat(item.key);

            public static implicit operator float? (DataItem item) => NullOrVal<float>(item, r => r.GetFloat(item.key));

            public static implicit operator short(DataItem item) => item.reader.GetInt16(item.key);

            public static implicit operator short? (DataItem item) => NullOrVal<short>(item, r => r.GetInt16(item.key));

            public static implicit operator long(DataItem item) => item.reader.GetInt64(item.key);

            public static implicit operator long? (DataItem item) => NullOrVal<long>(item, r => r.GetInt64(item.key));

            public static implicit operator MySqlDateTime(DataItem item)
            {
                return item.reader.GetMySqlDateTime(item.key);
            }

            public static implicit operator MySqlDecimal(DataItem item)
            {
                return item.reader.GetMySqlDecimal(item.key);
            }

            public static implicit operator MySqlGeometry(DataItem item)
            {
                return item.reader.GetMySqlGeometry(item.key);
            }

        }

        public void Dispose()
        {
            Reader.Dispose();
        }
    }
}

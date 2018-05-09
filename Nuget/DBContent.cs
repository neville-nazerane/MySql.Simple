using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace MySql.Simple
{
    class DBContent
    {

        public string Table { get; set; }

        public List<FieldContent> Fields { get; set; }

        string _PrimaryKey;
        public FieldContent PrimaryKey => Fields.Single(f => f.Name == _PrimaryKey);

        public string Selects => string.Join(", ", Fields.Select(f => f.Name));

        public string Inserts => string.Join(",",
                                 from f in Fields
                                 where f.Name != _PrimaryKey
                                 select f.Name);

        public object[] Values => (from f in Fields
                                   where f.Name != _PrimaryKey
                                   select f.GetValue()).ToArray();

        public IEnumerable<KeyValuePair<string, object>> UpdatePairs 
                                    => from f in Fields
                                        where f.Name != _PrimaryKey
                                        select new KeyValuePair<string, object>(f.Name, f.GetValue());

        internal object context;

        public DBContent(object obj, string ConnectionString)
        {
            context = obj;
            Type type = context.GetType();
            Table = type.Name;
            Fields = new List<FieldContent>();
            using (Database db = ConnectionString)
            using (var key = db.QuerySingle("SHOW INDEX FROM " + Table))
                _PrimaryKey = key["Column_name"];
            foreach (var prop in type.GetProperties())
                {
                    //var attrs = prop.CustomAttributes;

                    Fields.Add(new FieldContent
                    {
                        Name = prop.Name,
                        GetValue = () => prop.GetValue(context),
                        SetValue = r => prop.SetValue(context,
                                            typeof(QueryResult).GetMethod("Get")
                                            .MakeGenericMethod(prop.PropertyType)
                                            .Invoke(r, new object[] { prop.Name })),
                        SetRawValue = o => prop.SetValue(context, o)
                    });
                }

        }

    }

    class FieldContent
    {
        public string Name { get; set; }
        
        internal Func<object> GetValue;
        internal Action<QueryResult> SetValue;
        internal Action<object> SetRawValue;

    }

    

}

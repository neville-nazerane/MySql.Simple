using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace MySql.Simple
{

    public class DBInternalControl<TData, TControl> : DBControl<TData>
        where TData : new()
    {
        public DBInternalControl(string Table, string ConnectionString, TData Content, TControl Control) 
                                    : base(Table, ConnectionString, Content)
        {
            Controller.ControlContent = Control;
            Controller.typ = typeof(TControl);
        }
    }


    public class DBControl<T>
        where T : new()
    {
        public string Table { get; set; }

        public string ConnectionString { get; set; }

        ColumnControl _PrimaryKey;
        Database _UsingDatabase;

        public ColumnControl PrimaryKey
        {
            get => _PrimaryKey ?? FetchPrimaryKey();
            set { _PrimaryKey = value; }
        }

        public OpenDatabase KeepOpen() => new OpenDatabase(
                                            _UsingDatabase = ConnectionString,
                                            () => _UsingDatabase = null
                                         );

        ColumnControl FetchPrimaryKey()
        {
            using (Database db = ConnectionString)
            using (var key = db.QuerySingle("SHOW INDEX FROM " + Table))
                if (key)
                    return _PrimaryKey = Controller.Controls.Find(c => c.Key == key["Column_name"]);
            throw new MissingFieldException($"No primary key found in database table '{Table}'");
        }

        internal ColumnController<T> Controller { get; set; }

        public DBControl(string Table, string ConnectionString, T Content)
        {
            this.Table = Table;
            this.ConnectionString = ConnectionString;
            Controller = new ColumnController<T>(Content);
        }

        public int Insert(ColumnControl Columns = null, DatabaseLogger Logger = null)
        {
            if (_UsingDatabase != null) return Insert(_UsingDatabase, Columns, Logger);
            using (Database db = ConnectionString)
                return Insert(db, Columns, Logger);
        }

        public int Insert(Database db, ColumnControl Columns = null, DatabaseLogger Logger = null)
            => Columns == null ?
                    db.Insert(Table, Controller, Logger)
                    : db.Insert(Table, Columns, Logger);

        public int Update(ColumnControl Columns = null, BuiltCondition Condition = null, DatabaseLogger Logger = null)
        {
            if (_UsingDatabase != null) return Update(_UsingDatabase, Columns, Condition, Logger);
            using (Database db = ConnectionString)
                return Update(db, Columns, Condition, Logger);
        }

        public int Update(Database db, ColumnControl Columns = null, BuiltCondition Condition = null, DatabaseLogger Logger = null)
            => Columns == null ?
                    db.Update(Table, Controller, Condition, Logger) : db.Update(Table, Columns, Condition, Logger);

        public int Count(BuiltCondition Condition = null, DatabaseLogger Logger = null)
        {
            if (_UsingDatabase != null) return Count(_UsingDatabase, Condition, Logger);
            using (Database db = ConnectionString)
                return Count(db, Condition, Logger);
        }

        public int Count(Database db, BuiltCondition Condition = null, DatabaseLogger Logger = null)
            => db.Count(Table, Condition, Logger);

        public bool Exists(BuiltCondition Condition = null, DatabaseLogger Logger = null)
        {
            if (_UsingDatabase != null) return Exists(_UsingDatabase, Condition, Logger);
            using (Database db = ConnectionString)
                return Exists(db, Condition, Logger);
        }

        public bool Exists(Database db, BuiltCondition Condition = null, DatabaseLogger Logger = null)
            => db.Exists(Table, Condition, Logger);

        public bool Get(ColumnControl Columns = null, BuiltCondition Condition = null, DatabaseLogger Logger = null)
        {
            if (_UsingDatabase != null) return Get(_UsingDatabase, Columns, Condition, Logger);
            using (Database db = ConnectionString)
                return Get(db, Columns, Condition, Logger);
        }

        public bool Get(Database db, ColumnControl Columns = null, BuiltCondition Condition = null, DatabaseLogger Logger = null)
            => Columns == null ?
                    db.Get(Table, Controller, Condition, Logger) : db.Get(Table, Columns, Condition, Logger);

        public int Delete(BuiltCondition Condition = null, DatabaseLogger Logger = null)
        {
            if (_UsingDatabase != null) return Delete(_UsingDatabase, Condition, Logger);
            using (Database db = ConnectionString)
                return Delete(db, Condition, Logger);
        }

        public int Delete(Database db, BuiltCondition Condition = null, DatabaseLogger Logger = null)
            => db.Delete(Table, Condition, Logger);

        public List<T> GetAll(ColumnControl Columns = null, BuiltCondition Condition = null, DatabaseLogger Logger = null)
        {
            if (_UsingDatabase != null) return GetAll(_UsingDatabase, Columns, Condition, Logger);
            using (Database db = ConnectionString)
                return GetAll(db, Columns, Condition, Logger);
        }

        public List<T> GetAll(Database db, ColumnControl Columns = null, BuiltCondition Condition = null, DatabaseLogger Logger = null)
            => db.GetAll(Table, Controller, Columns, Condition, Logger);

        public ColumnControl Build<K>(Expression<Func<T, K>> expression)
            => Controller.Build(expression);

        public ColumnControl this[Expression<Func<T, int?>> expression] => Build(expression);
        public ColumnControl this[Expression<Func<T, float?>> expression] => Build(expression);
        public ColumnControl this[Expression<Func<T, double?>> expression] => Build(expression);
        public ColumnControl this[Expression<Func<T, DateTime?>> expression] => Build(expression);
        public ColumnControl this[Expression<Func<T, bool?>> expression] => Build(expression);
        public ColumnControl this[Expression<Func<T, byte?>> expression] => Build(expression);
        public ColumnControl this[Expression<Func<T, char?>> expression] => Build(expression);
        public ColumnControl this[Expression<Func<T, decimal?>> expression] => Build(expression);
        public ColumnControl this[Expression<Func<T, long?>> expression] => Build(expression);

        public ColumnControl this[Expression<Func<T, string>> expression] => Controller.Build(expression);
        public ColumnControl this[Expression<Func<T, int>> expression] => Controller.Build(expression);
        public ColumnControl this[Expression<Func<T, float>> expression] => Controller.Build(expression);
        public ColumnControl this[Expression<Func<T, double>> expression] => Controller.Build(expression);
        public ColumnControl this[Expression<Func<T, DateTime>> expression] => Controller.Build(expression);
        public ColumnControl this[Expression<Func<T, bool>> expression] => Controller.Build(expression);
        public ColumnControl this[Expression<Func<T, byte>> expression] => Controller.Build(expression);
        public ColumnControl this[Expression<Func<T, char>> expression] => Controller.Build(expression);
        public ColumnControl this[Expression<Func<T, decimal>> expression] => Controller.Build(expression);
        public ColumnControl this[Expression<Func<T, long>> expression] => Controller.Build(expression);


    }

}

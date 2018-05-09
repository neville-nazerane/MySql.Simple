using System;
using System.Collections.Generic;
using System.Text;

namespace MySql.Simple
{
    public static class UniversalGenerator
    {

        public static DBControl<T> DBControl<T>(string Table, string ConnectionString, T Content)
        where T : new()
            => new DBControl<T>(Table, ConnectionString, Content);

        public static DBControl<T> DBControl<T>(string ConnectionString, T Content)
        where T : new()
            => DBControl(typeof(T).Name, ConnectionString, Content);

        public static DBInternalControl<TData, TControl> DBInternalControl<TData, TControl>(string Table, string ConnectionString, TData Content, TControl Control)
        where TData : new()
            => new DBInternalControl<TData, TControl>(Table, ConnectionString, Content, Control);

        public static DBInternalControl<TData, TControl> DBInternalControl<TData, TControl>
                                        (string ConnectionString, TData Content, TControl Control)
        where TData : new()
            => new DBInternalControl<TData, TControl>(typeof(TData).Name, ConnectionString, Content, Control);

        public static ColumnController<T> ColumnController<T>(T Content)
            => new ColumnController<T>(Content);

        public static EquateConditioner<T> EquateConditioner<T>(T Content)
            => new EquateConditioner<T>(Content);

        public static LikeConditioner<T> LikeConditioner<T>(T Content)
            => new LikeConditioner<T>(Content);

        public static Conditioner Conditioner() => new Conditioner();

    }
}

using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Ender = MySql.Simple.BaseQueryer.Ender;

namespace MySql.Simple
{
    public partial class Database
    {

#region inserts

        public int Insert(string Table, ColumnControl Columns, DatabaseLogger Logger = null)
            => Insert(Table, Columns.StackedControls, Logger);

        public int Insert<T>(string Table, ColumnController<T> Columns, DatabaseLogger Logger = null)
            => Insert(Table, Columns.Controls, Logger);

        int Insert(string Table, List<ColumnControl> stack, DatabaseLogger logger)
        {
            var ins = Insert(Table);
            BaseQueryer.Pairer pairer = null;
            foreach (var col in stack)
                pairer = ins.Pair(col.Key, col.Value);
            try
            {
                int updated = pairer.Execute();
                bool noUpdate = updated == 0;
                logger.Add(new DatabaseLog
                {
                    Level = noUpdate ? LogLevel.Warning : LogLevel.Info,
                    Query = pairer.FullQuery,
                    Message = noUpdate ? "Nothing inserted" : $"Updated {updated} rows" 
                });
                if (noUpdate) return 0;
                return GetLatestID();
            }
            catch (Exception e)
            {
                logger.Add(new DatabaseLog { Level = LogLevel.Error, Query = pairer.FullQuery, Message = e.Message });
                throw;
            }
        }

#endregion

#region updates 

        public int Update(string Table, ColumnControl Columns, BuiltCondition Condition = null, DatabaseLogger Logger = null)
            => Update(Table, Columns.StackedControls, Condition, Logger);

        public int Update<T>(string Table, ColumnController<T> Columns, BuiltCondition Condition = null, DatabaseLogger Logger = null)
            => Update(Table, Columns.Controls, Condition, Logger);

        int Update(string Table, List<ColumnControl> stack, BuiltCondition condition, DatabaseLogger logger)
        {
            var upd = Update(Table);
            UpdateQueryer.Pairer pairer = null;
            foreach (var col in stack)
                pairer = upd.Pair(col.Key, col.Value);
            Ender query = pairer;
            if (condition != null)
                query = pairer.WhereCondition(condition);
            try
            {
                int updated = query.Execute();
                logger.Add(new DatabaseLog { Level = updated == 0? LogLevel.Warning : LogLevel.Info,
                                            Query = query.FullQuery,
                                            Message = updated == 0? "No rows were updated" : $"Updated {updated} rows" });
                return updated;
            }
            catch (Exception e)
            {
                logger.Add(new DatabaseLog { Level = LogLevel.Error, Query = query.FullQuery, Message = e.Message });
                throw;
            }
        }

        #endregion

        #region exists 

        public int Count(string Table, BuiltCondition Condition = null, DatabaseLogger Logger = null)
        {
            var frm = Select("COUNT(*)").From(Table);
            Ender ender = Condition == null ? (Ender) frm : frm.WhereCondition(Condition);
            try
            {
                using (var end = ender.QueryValue())
                {
                    Logger.Add(new DatabaseLog { Level = LogLevel.Info, Message = "checked exists", Query = ender.FullQuery });
                    return end;
                }
            }
            catch (Exception e)
            {
                Logger.Add(new DatabaseLog { Level = LogLevel.Error, Message = e.Message, Query = ender.FullQuery });
                throw;
            }
        }

        public bool Exists(string Table, BuiltCondition Condition = null, DatabaseLogger Logger = null)
            => Count(Table, Condition, Logger) > 0;

        #endregion

        #region gets

        public bool Get(string Table, ColumnControl Control, BuiltCondition Condition = null, DatabaseLogger Logger = null)
            => Get(Table, Control.StackedControls, Condition, Logger);

        public bool Get<T>(string Table, ColumnController<T> Control, BuiltCondition Condition = null, DatabaseLogger Logger = null)
            => Get(Table, Control.Controls, Condition, Logger);

        bool Get(string Table, List<ColumnControl> stack, BuiltCondition condition, DatabaseLogger logger)
        {
            var sel = Select(stack.Select(c => c.Key).ToArray()).From(Table);
            Ender get;
            if (condition == null) get = sel;
            else get = sel.WhereCondition(condition);
            try
            {
                using (var r = get.QuerySingle())
                {
                    if (r)
                    {
                        logger.Add(new DatabaseLog { Level = LogLevel.Info,  Query = get.FullQuery, Message = "Ran get query"});
                        foreach (var c in stack)
                            c.SetValue(r);
                        return true;
                    }
                    else
                        logger.Add(new DatabaseLog { Level = LogLevel.Warning, Query = get.FullQuery, Message = "No results" });
                    return false;
                }
                
            }
            catch (Exception e)
            {
                logger.Add(new DatabaseLog { Level = LogLevel.Error, Query = get.FullQuery, Message = e.Message });
                throw;
            }
        }

        #endregion

#region deletes
        

        public int Delete(string Table, BuiltCondition condition = null, DatabaseLogger logger = null)
        {
            var del = Delete(Table);
            Ender end;
            if (condition == null) end = del;
            else end = del.WhereCondition(condition);
            logger.Add(new DatabaseLog { Level = LogLevel.Info, Query = end.FullQuery, Message = "Running delete query" });
            try
            {
                int changed = end.Execute();
                bool noChange = changed == 0;
                logger.Add(new DatabaseLog { Level = noChange? LogLevel.Warning : LogLevel.Error,
                                             Query = end.FullQuery,
                                             Message = noChange ? "No rows deleted" : $"Deleted {changed} rows" });
                return changed;
            }
            catch (Exception e)
            {
                logger.Add(new DatabaseLog { Level = LogLevel.Error, Query = end.FullQuery, Message = e.Message });
                throw;
            }
        }

        #endregion

        #region getalls

        public List<T> GetAll<T>(string Table, ColumnController<T> Controller, ColumnControl Control = null, BuiltCondition Condition = null, DatabaseLogger Logger = null)
            where T : new()
        {
            var stack = Control?.StackedControls ?? Controller.Controls;
            return GetAll(Table, Controller, stack, Condition, Logger);
        }

        List<T> GetAll<T>(string Table, ColumnController<T> Control, List<ColumnControl> stack, BuiltCondition condition, DatabaseLogger logger)
            where T : new()
        {
            var sel = Select(stack.Select(c => c.Key).ToArray()).From(Table);
            Ender get;
            if (condition == null) get = sel;
            else get = sel.WhereCondition(condition);

            logger.Add(new DatabaseLog { Level = LogLevel.Info, Query = get.FullQuery, Message = "Running get query" });
            try
            {
                using (var r = get.Query())
                {
                    var vals = new List<T>();
                    if (r)
                    {
                        var original = Control.Content;
                        try
                        {
                            while (r.Read())
                            {
                                var val = new T();
                                Control.Content = val;
                                foreach (var c in stack) c.SetValue(r);
                                vals.Add(val);
                            }
                        }
                        catch (Exception e)
                        {
                            Control.Content = original;
                            throw e;
                        }
                        Control.Content = original;
                            
                    }
                    else
                        logger.Add(new DatabaseLog { Level = LogLevel.Warning, Query = get.FullQuery, Message = "No results" });
                    return vals;
                }

            }
            catch (Exception e)
            {
                logger.Add(new DatabaseLog { Level = LogLevel.Error, Query = get.FullQuery, Message = e.Message });
                throw e;
            }
        }

        #endregion

    }
}


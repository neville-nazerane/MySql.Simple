using System;
using System.Collections.Generic;
using System.Text;

namespace MySql.Simple
{

    /// <summary>
    /// Designed to add basic functions to table with 
    /// a primary key with auto increment
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IDBComplete<T> 
    where T : new()
    {

        DBControl<T> GetBControl();

    }

    public static class DBCompleteExtensions
    {

        public static int Add<T>(this IDBComplete<T> db, DatabaseLogger Logger = null)
        where T : new()
        {
            var control = db.GetBControl();
            if (control.PrimaryKey == null)
                throw new NullReferenceException("No primary key found");
            int ID = control.Insert(!control.PrimaryKey, Logger ); // assign id
            control.PrimaryKey.AssignRawValue(ID);
            return ID;
        }

        public static bool Update<T>(this IDBComplete<T> db, DatabaseLogger Logger = null)
        where T : new()
        {
            var control = db.GetBControl();
            return control.Update(!control.PrimaryKey, control.PrimaryKey, Logger) == 1;
        }

        public static bool Get<T>(this IDBComplete<T> db)
        where T : new()
        {
            var control = db.GetBControl();
            return control.Get(Condition: control.PrimaryKey);
        }

        public static bool Delete<T>(this IDBComplete<T> db, DatabaseLogger Logger = null)
        where T : new()
        {
            var control = db.GetBControl();
            return control.Delete(Condition: control.PrimaryKey, Logger: Logger) == 1;
        }

        public static List<T> GetAll<T>(this IDBComplete<T> db, DatabaseLogger Logger = null)
        where T : new()
        {
            var control = db.GetBControl();
            return control.GetAll(Logger: Logger);
        }

    }

}

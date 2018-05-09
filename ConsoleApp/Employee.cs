using MySql.Simple;
using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp
{

    interface IPersonal
    {
        int Add();
    }

    static class Extension
    {
        public static int Add(this Employee employee)
        {
            return 0;
        }
    }

    //class EmpDB : DBInternalControl<Employee, EmpDB>
    //{

        
    //    public EmpDB(Employee Content) : base("Employee", Connection.Str, Content)
    //    {
    //    }

    //    public ColumnControl ID => this[e => e.ID];
    //    public ColumnControl Age => this[e => e.Age];
    //    public ColumnControl Fname => this[e => e.Fname];
    //    public ColumnControl Lname => this[e => e.Lname];



    //}

    internal class EmpDB<T>
        where T : Employee, new()
    {
        DBInternalControl<T, EmpDB<T>> db;

        public ColumnControl ID => db[e => e.ID];
        public ColumnControl Fname => db[e => e.Fname];
        public ColumnControl Lname => db[e => e.Lname];

        internal EmpDB(T content)
        {
            db = UniversalGenerator.DBInternalControl(Connection.Str, content, this);
        }

        public List<T> ShowAll() => db.GetAll();

        public void Add() => db.Insert(!ID);


    }

    class Employee2 : Employee
    {

        EmpDB<Employee2> db;

        public Employee2()
        {
            db = new EmpDB<Employee2>(this);
        }

        public List<Employee2> ShowAll() => db.ShowAll();

        public void Add() => db.Add();

    }

    class Employee //: IDBComplete<Employee>
    {

        const string Table = "Employee";

        public int ID { get; set; }
        public int? Age { get; set; }
        public string Fname { get; set; }
        public string Lname { get; set; }
        public char? Initials { get; set; }
        public double Salary { get; set; }

        DBControl<Employee> db;

        ColumnControl _ID => db[e => e.ID];
        ColumnControl _Age => db[e => e.Age];
        ColumnControl _Fname => db[e => e.Fname];
        ColumnControl _Lname => db[e => e.Lname];
        ColumnControl _Initials => db[e => e.Initials];
        ColumnControl _Salary => db[e => e.Salary];

        public string Condition() {
            BuiltCondition cond = _Age | _Lname;
            cond = _ID & cond;
            return cond.FullQuery;
        }

        public Employee()
        {
            db = UniversalGenerator.DBControl(Connection.Str, this);
        }

        public DBControl<Employee> GetBControl() => db;

        public List<Employee> GetAll()
        {
            var logger = new DatabaseLogger { Level = LogLevel.Info };
            return db.GetAll(Logger: logger);

        }

        //public int Add()
        //    => db.Insert(!_ID);

        //public bool Update()
        //    => db.Update(!_ID, _ID) == 1;

        //public bool Get()
        //    => db.Get(Condition: _ID);

        //public int Delete()
        //    => db.Delete();

        //public List<Employee> GetAll()
        //    => db.GetAll();

    }
}

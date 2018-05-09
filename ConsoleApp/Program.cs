using MySql.Simple;
using MySql.Simple.Extensions;
using System;
using System.Collections.Generic;

namespace ConsoleApp
{
    class Program
    {

        static void Main(string[] args)
        {
            var logger = new DatabaseLogger { };
            
            //for (char ch = 'a'; ch <= 'z'; ch++)
            //    new Mad { Name = "Hello" + ch, Gone = ch%2 == 0 }.Add();

            //new Mad { Dated = DateTime.Now }.Add();
            //var mads = new Mad().GetAll();

            //var persons = new Person().GetAll();


            //string str = null;
            //str.IfEmpty("");

            Database db = "";
            BaseQueryer.Ender l;


            var emps = new Employee().GetAll();

            //var control = new EmpDB(new Employee());
            //var emps2 = control.GetAll();

            var control = new Employee2 { Fname = "Hell", Lname = "Bent" };
            //control.Add();
            var emps2 = control.ShowAll();

            Console.ReadLine();

            //var emp = new Employee { Fname = "Neville", Lname = "Nazerane_9", Age = 25, Initials = 'L', Salary = 4582.14 };

            //int eID = emp.Add();
            //emp.Lname = "Nazerane";
            //emp.Update();
            //var emp2 = new Employee { ID = eID };
            //emp2.Get();
            //emp2.Delete();
            //var all = emp2.GetAll();


            //var p1 = new Person { FullName = "Neville Nazerane", Address = "144 Sample St", Job = "Batman"};

            //p1.Add();
            //p1.Address = "Another address 2";
            //p1.FullName = "Neville 2 Fernando";
            //p1.Update();


            //new Person { FullName = "My name" }.Add(); // adds all except ID (primary key)
            //Person p = new Person { ID = 2 };
            //p.Get(); // gets by primary key
            //p.Address = "New address"; p.Update(); // updates by primary key
            //new Person { ID = 3 }.Delete(); // RIP




            //p1.Address = "Another address 2"; p1.Update();

            //p1.ID = 4; p1.Get();
            //p1.ID = 5; p1.Delete();

            //var p2 = new Person { ID = 20 };
            //p2.Get();
            ////p2.Delete();

            //var pAll = p1.GetAll();

            //var testEmployee = new BusEmployee { Fname = "Neville4", Lname = "Fernando", Age = 45, Initials = 'N', Salary = 8832.1 };

            Console.WriteLine(
                new Employee { ID = 9, Age = 23, Lname = "Nazerane" }.Condition()
                );


            Console.ReadLine();
            //Console.WriteLine(testEmployee.Add());


            //Console.WriteLine(new Employee().Delete());

            //l = db.Delete("MyName");
            //Console.WriteLine(l.FullQuery);

            //l = db.Delete("YourName").Where("ID = @0", 44);
            //Console.WriteLine(l.FullQuery);

            var aa = new Auth { id = 84, name = 544, title = "My %man_" };
            var c = Conditioner.GetUniversalConditioner(aa);
            
            //var ec = Conditioner.GetEquateConditioner(aa);
            //var lc = Conditioner.GetLikeConditioner(aa);
            var empty = Conditioner.Default;

            var cond = Conditioner.Default;

            var auths = new List<Auth> {
                new Auth{ id = 9, name = 54 },
                new Auth{ id = 8, name = 14 },
                new Auth{ id = 3, name = 65 }
            };

            auths.ForEach(a => cond = cond.Or(c["AuthID", a.id].And(c["AuthTypeID", a.name])));
            var condPerm = c["TargetID", 5].And(c["TargetTypeID", 8]);
            l = db.From("batman").WhereCondition(empty.And(cond).And(condPerm));

            Console.WriteLine(l.FullQuery);

            Console.WriteLine("Repeating:");
            empty = Conditioner.Default;

            var cond2 = Conditioner.Default;
            auths.ForEach(a => cond2 = cond2 | !(c["AuthID", a.id] & c["AuthTypeID", a.name]));
            var condPerm2 = c["TargetID", 5] & c["TargetTypeID", 8];
            var condEq = c[o => o.name];
            var condLk = c[o => o.title, "%@0%"];
            l = db.Delete("batman").WhereCondition(empty & cond2 & condPerm2 | (condEq & condLk));

            Console.WriteLine(l.FullQuery);
            Console.WriteLine("\n");

            l = db.Insert("tblUsers")
                    .Keys("username", "isActive")
                    .SelectQuery("username, id, @0", 993).From("tblUsers").Where("abc = $1 AND c LIKE %@0 AND IsActive = $2", 5, null, true);
            Console.WriteLine(l.FullQuery);

            var re = Console.ReadLine();
            
            Console.Read();

        }

        private class Auth
        {

            public int id { get; set; }
            public int name { get; set; }
            public string title { get; set; }

        }

        static void Backed()
        {


            Console.WriteLine("Hello World!");
            BaseQueryer.Ender l;
            Conditioner c = new Conditioner();

            var empty = Conditioner.Default;



            Database db = "";
            l = db.Select("username").From("Users");
            Console.WriteLine(l.FullQuery);
            l = db.Select("username").From("Users").Where("abc=$1 AND c LIKE %@0", 5, true);
            Console.WriteLine(l.FullQuery);
            l = db.From("USERS")
                .Where("abc=$1 AND c LIKE %@0 AND IsActive = $2", 5, "gg", true)
                .Order("batman");
            Console.WriteLine(l.FullQuery);
            l = db.Insert("tblUsers")
                    .Pair("username", "Neville")
                    .Pair("ref", false)
                    .Pair("cherar", 'y');
            Console.WriteLine(l.FullQuery);
            l = db.Insert("tblUsers")
                    .Keys("username", "isActive")
                    .Values(433555, true);
            Console.WriteLine(l.FullQuery);
            l = db.Insert("tblUsers")
                    .Keys("username", "isActive")
                    .Select("username, id").From("tblUsers").Where("abc=$1 AND c LIKE %@0 AND IsActive = $2", 5, null, true);
            Console.WriteLine(l.FullQuery);
            l = db.Insert("tblUsers", new
            {
                ID = 66,
                IsActive = false,
                username = "Hello world"
            });
            Console.WriteLine(l.FullQuery);


            l = db.Update("tblFriendRequests")
                        .Pair("IsActive", false)
                        .Where("ID = $0", 4);
            Console.WriteLine(l.FullQuery);


            l = db.Update("tblUsers")
                    .Pair("ID", 66)
                    .Pair("IsActive", false);
            Console.WriteLine(l.FullQuery);
            l = db.Update("tblUsers")
                    .Pair("ID", 66)
                    .Pair("IsActive", false)
                    .Where("abc=$1 AND c LIKE %@0 AND IsActive = $2", 5, "gg", true);
            Console.WriteLine(l.FullQuery);
            l = db.Update("tblUsers", new
            {
                ID = 66,
                IsActive = true,
                username = "Hello world"
            })
            .Where("abc=$1 AND c LIKE %@0 AND IsActive = $2", 5, "gg", true);
            Console.WriteLine(l.FullQuery);
            l = db.Update("tblUsers")
                    .Pair("ID", 66)
                    .Pair("IsActive", false);
            Console.WriteLine(l.FullQuery);
            var r = Console.ReadLine();


            //

            var a = c["name LIKE $0", "neville"];
            var b = c["id", 9];
            var d = c["username = $0", "nn2gcc"];
            var e = c["username = 'admin' @0", "batman"];


            //   string str = a.And(b.And(d)).FullQuery;
            //   string str = a.And(b).Queryfy;

            l = db.Select("fname").From("Users u").Join("comments c", "c.UserID = u.ID");
            Console.WriteLine(l.FullQuery);
            l = db.Select("fname").From("Users u").Join("comments c", "c.UserID = u.ID")
                        .WhereCondition(empty.And(a.Or(b)).Or(d.Or(e)));
            Console.WriteLine(l.FullQuery);
            l = db.Update("tblUsers")
                    .Pair("ID", 66)
                    .Pair("IsActive", false);
            Console.WriteLine(l.FullQuery);
            l = db.Select("username").From("Users");
            Console.WriteLine(l.FullQuery);

        }

    }
}
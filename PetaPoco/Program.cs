using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SQLite;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using FluentMigrator;
using FluentMigrator.Runner;
using FluentMigrator.Runner.Announcers;
using FluentMigrator.Runner.Initialization;
using PetaPoco.Pocos;

namespace PetaPoco
{
    class Program
    {
        const string CONNECTION_STRING = "Data Source=northwindEF.db;Version=3;";
        static void Main(string[] args)
        {

            testProduct();
            runMigrations();
            testFoos();
        }

        static void testProduct() 
        {
            try 
            {
                using( DbConnection connection = new SQLiteConnection(CONNECTION_STRING) )
                {
                    // Raw ADO query to make sure sqlite is working
                    connection.Open();        
                    DbCommand cmd = connection.CreateCommand();
                    cmd.CommandText = "select count(*) from products";
                    object count = cmd.ExecuteScalar();
                    Console.WriteLine("Products has {0} rows", count);

                    Console.WriteLine("\n-- Product test  ----------------------------------------------");
                    using (var petaPocoDB = new Database(connection)) {
    
                        // PetaPoco select * from example. No need to supply sql because of attributes on the object
                        List<Product> productList = petaPocoDB.Query<Product>("").ToList(); 
                        foreach(Product product in productList ) {
                            Console.WriteLine("Product {0}:{1}", product.ProductID, product.ProductName);
                        }

                        productList = petaPocoDB.Fetch<Product>("where UnitsInStock > 0");
                        foreach(Product product in productList ) {
                            Console.WriteLine("Product {0}:{1} units in stock={2}", product.ProductID, product.ProductName, product.UnitsInStock);
                        }

                        // Grabbing a single item by id
                        Console.WriteLine("\n------------------------------------------------");
                        int id = 29;
                        Product product29 = petaPocoDB.SingleOrDefault<Product>(id);
                        if(product29 != null ) 
                        {
                            Console.WriteLine("Found product id 29 {0}, units in stock={1} ", product29.ProductName, product29.UnitsInStock);
                           
                            // Update the record
                            product29.UnitsInStock += 2;
                            petaPocoDB.Save(product29);

                            // requery to make sure DB took the chagne
                            product29 = petaPocoDB.SingleOrDefault<Product>(product29.ProductID);
                            Console.WriteLine("units in stock updated to  " + product29.UnitsInStock);                           
                        }
                        else
                            Console.WriteLine("Product 29 not found");

                    }
                }
            }
            catch(Exception e) 
            {
                Console.WriteLine(e);
            }
        }

       public class MigrationOptions : IMigrationProcessorOptions
        {
            public bool PreviewOnly { get; set; }
            public string ProviderSwitches { get; set; }
            public int Timeout { get; set; }
        }
   
        static void runMigrations() 
        {
            Console.WriteLine("\n-- Running Foo migrations   ----------------------------------------------");
            using( DbConnection connection = new SQLiteConnection(CONNECTION_STRING) ) 
            {
                var announcer = new TextWriterAnnouncer(s => Console.WriteLine(s));
                var assembly = Assembly.GetExecutingAssembly();

                var migrationContext = new RunnerContext(announcer)
                {
                    Namespace = "PetaPoco.Migrations"
                };

                var options = new MigrationOptions { PreviewOnly=false, Timeout=60 };
                //var  = new FluentMigrator.Runner.Processors.SQLite.SQLiteProcess();
                var generator = new FluentMigrator.Runner.Generators.SQLite.SQLiteGenerator();
                var factory = new FluentMigrator.Runner.Processors.SQLite.SQLiteDbFactory();
                using (var processor = new FluentMigrator.Runner.Processors.SQLite.SQLiteProcessor(connection, generator, announcer, options, factory))
                { 
                    var runner = new MigrationRunner(assembly, migrationContext, processor);
                    runner.MigrateUp(true);
                }
            }
        }

        static void testFoos()
        {
            try 
            {
                using( DbConnection connection = new SQLiteConnection(CONNECTION_STRING) )
                {
                    // Raw ADO query to make sure sqlite is working
                    connection.Open();        

                    Console.WriteLine("\n-- Foo test ----------------------------------------------");
                    using (var petaPocoDB = new Database(connection)) {
    
                        // PetaPoco select * from example. No need to supply sql because of attributes on the object
                        Foo f = new Foo();
                        f.X = 29;
                        f.Y = 42;
                        f.Z = "xray";
                        petaPocoDB.Save(f);


                        List<Foo> fooList= petaPocoDB.Query<Foo>("").ToList(); 
                        foreach(Foo foo in fooList ) {
                            Console.WriteLine("Foo {0}:{1}", foo.FooID, foo.X);
                        }
                    }
                }
            }
            catch(Exception e) 
            {
                Console.WriteLine(e);
            }
        }

    }
}

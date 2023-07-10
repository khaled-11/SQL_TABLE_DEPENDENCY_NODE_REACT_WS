using System;
using System.Text;
using TableDependency.SqlClient.Base.EventArgs;
using TableDependency.SqlClient.Base;
using TableDependency.SqlClient;
using System.IO;
using System.Net;

namespace SqlNotifications
{
    public class Program
    {
        private static string _con = "data source=.; initial catalog=TableDependencyDB; integrated security=True";

        public static void Main()
        {
            // The mapper object is used to map model properties 
            // that do not have a corresponding table column name.
            // In case all properties of your model have same name 
            // of table columns, you can avoid to use the mapper.
            var mapper = new ModelToTableMapper<Customer>();
            mapper.AddMapping(c => c.Surname, "Surname");
            mapper.AddMapping(c => c.Name, "Name");

            // Here - as second parameter - we pass table name: 
            // this is necessary only if the model name is different from table name 
            // (in our case we have Customer vs Customers). 
            // If needed, you can also specifiy schema name.
            using (var dep = new SqlTableDependency<Customer>(_con, "Customers", mapper: mapper))
            {
                dep.OnChanged += Changed;
                dep.Start();
                Console.WriteLine("Press a key to exit");
                Console.ReadKey();
                dep.Stop();
            };
        }

        public static string CallRestMethod(string url, int id, string name, string surname)
        {
            HttpWebRequest webrequest = (HttpWebRequest)WebRequest.Create(url);
            webrequest.Method = "GET";
            webrequest.ContentType = "application/x-www-form-urlencoded";
            webrequest.Headers.Add("ID", id.ToString());
            webrequest.Headers.Add("NAME", name);
            webrequest.Headers.Add("SURNAME", surname);
            HttpWebResponse webresponse = (HttpWebResponse)webrequest.GetResponse();
            Encoding enc = System.Text.Encoding.GetEncoding("utf-8");
            StreamReader responseStream = new StreamReader(webresponse.GetResponseStream(), enc);
            string result = string.Empty;
            result = responseStream.ReadToEnd();
            webresponse.Close();
            return result;
        }

        public static void Changed(object sender, RecordChangedEventArgs<Customer> e)
        {
            var changedEntity = e.Entity;
            Console.WriteLine("DML operation: " + e.ChangeType);
            Console.WriteLine("ID: " + changedEntity.Id);
            Console.WriteLine("Name: " + changedEntity.Name);
            Console.WriteLine("Surname: " + changedEntity.Surname);
            string url = "http://localhost:3370/";
            string details = CallRestMethod(url, changedEntity.Id, changedEntity.Name, changedEntity.Surname);
            Console.WriteLine(details);
        }
    }
}
using System;
using System.Data.SqlClient;
using System.Configuration;
using System.Text;
using System.IO;
using System.Net;

namespace Financies
{
    class Program
    {
        public static string DownloadString(string address)
        {
            string text;
            using (var client = new WebClient())
            {
                text = client.DownloadString(address);
            }
            return text;
        }

        public static void WriteItToXml(string address)
        {
            var xml = DownloadString(@address);
            Encoding win1251 = Encoding.GetEncoding(1251);
            using (var writer = new StreamWriter("blah.xml", false, win1251))
            {
                writer.Write(xml);
            }
        }

        public static void UseFunction(string constring)
        {
            Console.WriteLine("Input date in format dd.mm.yyyy");
            String date = Console.ReadLine();
            Console.WriteLine("Enter the name of the currency (in Russian)"); // stupid github does not understand cyrillic
            String name = Console.ReadLine();
            string sqlExpression = "Select value from CurrencyRates where value=dbo.CurrencyByName('" + date + "','" + name + "')";
            using (SqlConnection connection = new SqlConnection(constring))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(sqlExpression, connection);
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    Console.WriteLine("Currency rate " + name + " on " + date + " is " + reader[0] + " roubles");
                }
            }
        }

        public static void UpdateTable(string constring)
        {
            Console.WriteLine("Input date in format dd.mm.yyyy");
            String date = Console.ReadLine();
            String checkingSqlExpr = "Select Date from CurrencyRates where Date='" + date+"'";
            using (SqlConnection connection = new SqlConnection(constring))
            {
                connection.Open();
                SqlCommand command = new SqlCommand(checkingSqlExpr, connection);
                object res = command.ExecuteScalar();
                if (res!=null)
                    Console.WriteLine("Table already has currency rates on this date");
                else
                {
                    WriteItToXml("http://www.cbr.ru/scripts/XML_daily.asp?date_req="+date);
                    string script = File.ReadAllText(@"DB\XmlToTable.sql");
                    var aStringBuilder = new StringBuilder(script);
                    aStringBuilder.Replace("05.05.2020", date);
                    script = aStringBuilder.ToString();
                    command = new SqlCommand(script, connection);
                    int number = command.ExecuteNonQuery();
                    Console.WriteLine("Objects added: {0}", number);
                }
            }
        }

        private static void Main(string[] args)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            Console.WriteLine("Enter 1 to update table or 2 to use function");
            String dec = Console.ReadLine();
            switch (dec)
            {
                case "1":
                    UpdateTable(connectionString);
                    break;
                case "2":
                    UseFunction(connectionString);
                    break;
                default:
                    Console.WriteLine("You entered wrong command");
                    break;
            }
        }
    }
}

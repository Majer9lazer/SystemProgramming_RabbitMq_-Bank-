using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Newtonsoft.Json.Bson;
using System.Windows;
namespace Producer
{
    class Program
    {
        public static string DocName = "LocalXmlDatabase";
        public static string DocExtension = ".xml";
        static void Main(string[] args)
        {

            Db a = new Db(DocName + DocExtension);

            User Egor = new User()
            {
                UserGuid = Guid.NewGuid(),
                Age = 18,
                UserMail = "sidorenkoegor1999@mail.ru",
                UserName = "Egor",
                UserNumber = "+77051642833"
            };
            //XDocu
            a.Add(Egor);
            //xdoc.Save(AppDomain.CurrentDomain.BaseDirectory + DocName + DocExtension);
            Console.WriteLine("Всё прошло успешно");

            Console.ReadLine();
        }

        struct Db
        {
            public Db(string dbFileName)
            {
                DbFile = dbFileName;
            }

            private string DbFile { get; set; }
            public void Add(User u)
            {
                XDocument xdoc = XDocument.Load(AppDomain.CurrentDomain.BaseDirectory + DocName + DocExtension);
                var element = xdoc.Element("Users");
                element?.Add(new XElement("User",
                    new XAttribute("Id", GetMaxUserId()+1),
                    new XElement("Name", u.UserName),
                    new XElement("Age", u.Age),
                    new XElement("UserMail", u.UserMail),
                    new XElement("UserNumber", u.UserNumber),
                    new XElement("UserGuid",u.UserGuid)));
                xdoc.Save(DbFile);
            }

            private int GetMaxUserId()
            {
                XDocument xdoc = XDocument.Load(DbFile);
                int a = Convert.ToInt32(xdoc.Elements().Elements().Max(w => Convert.ToInt32(w.Attribute("Id").Value)));
                //.Select(s=>new
                //{
                //    s.Name,s.Value
                //});
                return a;
            }
        }
    }

    struct User
    {
        public string UserName { get; set; }
        public int Age { get; set; }
        public Guid UserGuid { get; set; }
        public string UserNumber { get; set; }
        public string UserMail { get; set; }

    }
}

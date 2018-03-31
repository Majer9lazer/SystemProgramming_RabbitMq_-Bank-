using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Newtonsoft.Json.Bson;
using System.Windows;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Producer
{
    public enum UserStatus
    {
        Passed, Error, OnModeration
    }
    public class Program
    {
        public static string DocName = "LocalXmlDatabase";
        public static string DocExtension = ".xml";

        private static void FillInDb(ref Db db)
        {
            object converted;
            using (StreamReader str = new StreamReader("Users.json"))
            {
                converted = JsonConvert.DeserializeObject(str.ReadToEnd());
            }
            JArray arr = (JArray)converted;
            foreach (JToken t in arr)
            {
                Dictionary<IJEnumerable<JToken>, JToken> dd;
                dd = t.ToDictionary(w => w.Values());
                var ss = t.ToList();
                User u = new User();
                foreach (JToken token in ss)
                {
                    string key = token.Value<JProperty>().Name;
                    string value = token.Value<JProperty>().Value.ToString();
                    switch (key)
                    {
                        case "id": { u.UserId = int.Parse(value); } break;
                        case "UserName": { u.UserName = value; } break;
                        case "Age": { u.Age = int.Parse(value); } break;
                        case "UserMail": { u.UserMail = value; } break;
                        case "UserGuid": { u.UserGuid = (value); } break;
                        case "UserNumber": { u.UserNumber = value; } break;
                    }

                }


                db.Add(u, UserStatus.Passed.ToString());
            }
        }
        static void Main(string[] args)
        {

            Db a = new Db(DocName + DocExtension);
            Console.WriteLine("Всё прошло успешно");
            //FillInDb(ref a);
            Console.ReadLine();
        }

        public struct Db
        {
            public Db(string dbFileName)
            {
                DbFile = dbFileName;
            }

            private string DbFile { get; }
            public void Add(User u, string userStatus)
            {
                XDocument xdoc = XDocument.Load(AppDomain.CurrentDomain.BaseDirectory + DocName + DocExtension);
                var element = xdoc.Element("Users");
                if (u.UserId != null)
                    element?.Add(new XElement("User",
                        new XAttribute("Id", u.UserId),
                        new XElement("Name", u.UserName),
                        new XElement("Age", u.Age),
                        new XElement("UserMail", u.UserMail),
                        new XElement("UserNumber", u.UserNumber),
                        new XElement("UserGuid", u.UserGuid),
                        new XElement("Status", userStatus)));
                xdoc.Save(DbFile);
            }

            public User GetUserBymailOrGuid(string userData, bool searchForGuid)
            {
                XDocument xdoc = XDocument.Load(AppDomain.CurrentDomain.BaseDirectory + DocName + DocExtension);
                User u = new User();
                if (searchForGuid)
                {
                    XElement element = xdoc.Element("Users").Elements().Elements().FirstOrDefault(w => w.Name == "UserGuid" && w.Value == userData).Parent;

                    foreach (XElement xElement in element.Elements())
                    {
                        switch (xElement.Name.LocalName)
                        {
                            case "Name": { u.UserName = xElement.Value; break; }
                            case "Age": { u.Age = int.Parse(xElement.Value); break; }
                            case "UserMail": { u.UserMail = xElement.Value; break; }
                            case "UserNumber": { u.UserNumber = xElement.Value; break; }
                            case "UserGuid": { u.UserGuid = xElement.Value; break; }
                            case "Status": { u.Status = xElement.Value; break; }
                        }
                    }
                    u.UserId = int.Parse(element.Attribute("Id").Value);
                }
                else
                {

                    //  var sq = xdoc.Element("Users").Elements().Elements().Where(w=>w.Name=="UserMail"&&w.Value==userData);
                    XElement element = xdoc.Element("Users")?.Elements().Elements().FirstOrDefault(w => w.Name == "UserMail" && w.Value == userData)?.Parent;

                    foreach (XElement xElement in element.Elements())
                    {
                        switch (xElement.Name.LocalName)
                        {
                            case "Name": { u.UserName = xElement.Value; break; }
                            case "Age": { u.Age = int.Parse(xElement.Value); break; }
                            case "UserMail": { u.UserMail = xElement.Value; break; }
                            case "UserNumber": { u.UserNumber = xElement.Value; break; }
                            case "UserGuid": { u.UserGuid = xElement.Value; break; }
                            case "Status": { u.Status = xElement.Value; break; }
                        }
                    }
                    u.UserId = int.Parse(element.Attribute("Id").Value);
                }
                return u;
            }

            public void UpdateUserStatus(User u, string status)
            {

                XDocument xdoc = XDocument.Load(AppDomain.CurrentDomain.BaseDirectory + DocName + DocExtension);
                var element = xdoc.Element("Users").Elements().Elements().FirstOrDefault(f => f.Value == u.UserGuid)
                    .Parent;
                foreach (XElement xelement in element.Elements())
                {
                    if (xelement.Name.LocalName == "Status")
                    {
                        xelement.Value = status;
                    }
                }
                xdoc.Save(AppDomain.CurrentDomain.BaseDirectory+DocName+DocExtension);
            }
            public static int GetMaxId(string docPath)
            {
                XDocument xdoc = XDocument.Load(docPath);
                int? el = (xdoc.Element("Users")?.Elements().Select(s => int.Parse(s.Attribute("Id").Value)).Max());

                return (int)el;
            }
        }
    }

    public struct User
    {
        public int? UserId { get; set; }
        public string UserName { get; set; }
        public int Age { get; set; }
        public string UserGuid { get; set; }
        public string UserNumber { get; set; }
        public string UserMail { get; set; }
        public string Status { get; set; }

    }
}

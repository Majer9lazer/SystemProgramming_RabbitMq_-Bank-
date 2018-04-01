using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace UserInterface.WorkWithUsers
{
    public struct Db
    {
        public Db(string dbFileName)
        {
            DbFile = dbFileName;
        }

        private string DbFile { get; }
        public void Add(User u, string userStatus)
        {
            XDocument xdoc = XDocument.Load(DbFile);
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
            XDocument xdoc = XDocument.Load(DbFile);
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

                var sq = xdoc.Element("Users").Elements().Elements().Where(w => w.Name == "UserMail" && w.Value == userData);
                XElement element = xdoc.Element("Users")?.Elements().Elements().FirstOrDefault(w => w.Name == "UserMail" && w.Value == userData)?.Parent;

                if (element != null)
                {
                    foreach (XElement xElement in element.Elements())
                    {
                        switch (xElement.Name.LocalName)
                        {
                            case "Name":
                            {
                                u.UserName = xElement.Value;
                                break;
                            }
                            case "Age":
                            {
                                u.Age = int.Parse(xElement.Value);
                                break;
                            }
                            case "UserMail":
                            {
                                u.UserMail = xElement.Value;
                                break;
                            }
                            case "UserNumber":
                            {
                                u.UserNumber = xElement.Value;
                                break;
                            }
                            case "UserGuid":
                            {
                                u.UserGuid = xElement.Value;
                                break;
                            }
                            case "Status":
                            {
                                u.Status = xElement.Value;
                                break;
                            }
                        }
                    }

                    u.UserId = int.Parse(element.Attribute("Id").Value);
                }
            }
            return u;
        }

        public void UpdateUserStatus(User u, string status, bool redactAll)
        {
            XDocument xdoc = XDocument.Load(DbFile);
            var element = xdoc.Element("Users").Elements().Elements().FirstOrDefault(f => f.Value == u.UserGuid)
                .Parent;
            if (!redactAll)
            {

                foreach (XElement xelement in element.Elements())
                {
                    if (xelement.Name.LocalName == "Status")
                    {
                        xelement.Value = status;
                    }
                }

                xdoc.Save(DbFile);
            }
            else
            {
                foreach (XElement xElement in element.Elements())
                {
                    switch (xElement.Name.LocalName)
                    {
                        case "Name": { xElement.Value = u.UserName; break; }
                        case "Age": { xElement.Value = u.Age.ToString(); break; }
                        case "UserMail": { xElement.Value = u.UserMail; break; }
                        case "UserNumber": { xElement.Value = u.UserNumber; break; }
                        case "UserGuid": { xElement.Value = u.UserGuid; break; }
                        case "Status": { xElement.Value = u.Status; break; }
                    }
                }
                xdoc.Save(DbFile);
            }
        }
        public static int GetMaxId(string docPath)
        {
            XDocument xdoc = XDocument.Load(docPath);
            int? el = (xdoc.Element("Users")?.Elements().Select(s => int.Parse(s.Attribute("Id").Value)).Max());
            return (int)el;
        }
        public static void FillInDb(ref Db db)
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
    public enum UserStatus
    {
        Passed, Error, OnModeration
    }
}

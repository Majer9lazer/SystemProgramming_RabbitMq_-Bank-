using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using UserInterface.WorkWithUsers;


namespace UserInterface
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static readonly string PathToDataBase = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory).Parent?.Parent?.Parent?.FullName +
            @"\LocalXmlDatabase.xml";

        private Db _dataBase = new Db(PathToDataBase);

        public static readonly string PathToErrorLog = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory).Parent?.Parent?.Parent?.FullName +
            @"\ErrorLog.txt";

        private void SendMailToUser(string userMail, string messageGuid, ref User u)
        {

            try
            {
                //smtp сервер
                string smtpHost = "smtp.gmail.com";
                //smtp порт
                int smtpPort = 587;
                //логин
                string login = "csharp.sdp.162@gmail.com";
                //пароль
                string pass = "sdp123456789";


                //От кого письмо
                string from = "csharp.sdp.162@gmail.com";
                //Кому письмо
                string to = userMail;
                //Тема письма
                string subject = "Уникальный ключ";
                //Текст письма
                string body = $"Привет! ваш Guid - {messageGuid}";

                //Создаем сообщение
                MailMessage mess = new MailMessage(from, to, subject, body);

                //создаем подключение
                SmtpClient client = new SmtpClient(smtpHost, smtpPort);
                client.Credentials = new NetworkCredential(login, pass);
                client.EnableSsl = true;
                client.Send(new MailMessage(from, to, subject, body));
                u.Status = UserStatus.Passed.ToString();
            }
            catch (Exception ex)
            {
                RabbitMqMiddlewareBusService a = new RabbitMqMiddlewareBusService();
                a.PublishMessage((new ErrorMessage() { MessageBody = ex.Message, UserInfo = u }), "Error");
                //ErrorOrSuccessTextBlock.Text += ex;
            }
        }


        public MainWindow()
        {
            TextBoxMessages = new Dictionary<string, string>()
            {
                {"UserNameTextBox", "Your Name Here"},
                {"UserAgeTextBox", "Your Age Here"},
                {"UserMailTextBox", "Your Mail Here"},
                {"UserNumberTextBox", "Your Number Here"}
            };

            //Process.Start("Consumer.exe","Error");
            if (File.Exists(PathToErrorLog))
            {
                File.Delete(PathToErrorLog);
            }

            InitializeComponent();

        }


        public Dictionary<string, string> TextBoxMessages;

        private void UserNameTextBox_OnGotFocus(object sender, RoutedEventArgs e)
        {

            if (UserNameTextBox.Text == "Your Name Here")
            {
                UserNameTextBox.Text = null;
                UserNameTextBox.Background = new SolidColorBrush(Colors.White);
            }
            else
            {
                UserNameTextBox.Background = new SolidColorBrush(Colors.White);
            }

        }

        private void UserAgeTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (UserAgeTextBox.Text == "Your Age Here")
            {
                UserAgeTextBox.Text = null;
                UserAgeTextBox.Background = new SolidColorBrush(Colors.White);
            }
            else
            {
                UserAgeTextBox.Background = new SolidColorBrush(Colors.White);
            }
        }

        private void UserMailTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (UserMailTextBox.Text == "Your Mail Here")
            {
                UserMailTextBox.Text = null;
                UserMailTextBox.Background = new SolidColorBrush(Colors.White);
            }
            else
            {
                UserMailTextBox.Background = new SolidColorBrush(Colors.White);
            }
        }

        private void UserNumberTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (UserNumberTextBox.Text == "Your Number Here")
            {
                UserNumberTextBox.Text = null;
                UserNumberTextBox.Background = new SolidColorBrush(Colors.White);

            }
            else
            {
                UserNumberTextBox.Background = new SolidColorBrush(Colors.White);
            }
        }

        private RabbitMqMiddlewareBusService _busService = new RabbitMqMiddlewareBusService();

        private void RegistrateButton_OnClick(object sender, RoutedEventArgs e)
        {
            TextBox[] arrTextBoxs = { UserNameTextBox, UserAgeTextBox, UserMailTextBox, UserNumberTextBox };
            foreach (TextBox box in arrTextBoxs)
            {
                if (TextBoxMessages.First(f => f.Key == box.Name).Value == box.Text)
                {
                    box.Background = new SolidColorBrush(Colors.Red);
                }
            }

            if (arrTextBoxs.Any(b => b.Background.ToString() == new SolidColorBrush(Colors.Red).ToString()))
            {
                MessageBox.Show("Вы не можете зарегестрироваться так как неправильные поля отмечены красным цветом");
            }
            else
            {
                User u = new User();
                string newGuid = Guid.NewGuid().ToString();

                int defage;
                if (!UserMailTextBox.Text.Contains('@'))
                {
                    int.TryParse(UserAgeTextBox.Text, out defage);
                    u.Age = defage;
                    u.Status = UserStatus.Error.ToString();
                    u.UserId = Db.GetMaxId(PathToDataBase) + 1;
                    u.UserMail = UserMailTextBox.Text;
                    u.UserGuid = newGuid;
                    u.UserNumber = UserNumberTextBox.Text;
                    u.UserName = UserNameTextBox.Text;


                    _busService.PublishMessage(
                        (new ErrorMessage()
                        {
                            MessageBody = "Проблемы с тем что у вас тупо нет собачки) аахахаха, заведите собачку)",
                            UserInfo = u
                        }), "Error");
                    _dataBase.Add(u, u.Status);
                }
                else
                {
                    if (UserNumberTextBox.Text.Length != 12)
                    {
                        int.TryParse(UserAgeTextBox.Text, out defage);
                        u.Age = defage;
                        u.Status = UserStatus.Error.ToString();
                        u.UserId = Db.GetMaxId(PathToDataBase) + 1;
                        u.UserMail = UserMailTextBox.Text;
                        u.UserGuid = newGuid;
                        u.UserNumber = UserNumberTextBox.Text;
                        u.UserName = UserNameTextBox.Text;
                        _busService.PublishMessage(
                            (new ErrorMessage()
                            {
                                MessageBody = "Проблемы с тем что у вас неправильный номер)",
                                UserInfo = u
                            }), "Error");
                        _dataBase.Add(u, u.Status);
                    }
                    else
                    {
                        int.TryParse(UserAgeTextBox.Text, out defage);
                        u.Age = defage;
                        u.Status = UserStatus.OnModeration.ToString();
                        u.UserId = Db.GetMaxId(PathToDataBase);
                        u.UserMail = UserMailTextBox.Text;
                        u.UserGuid = newGuid;
                        u.UserNumber = UserNumberTextBox.Text;
                        u.UserName = UserNameTextBox.Text;

                        SendMailToUser(UserMailTextBox.Text, newGuid, ref u);
                        _dataBase.Add(u, u.Status);
                    }
                }

                MessageBox.Show("Добро мать его пожаловать! , ну почти)) ");
                foreach (KeyValuePair<string, string> mesage in TextBoxMessages)
                {
                    TextBox firstOrDefault = arrTextBoxs.FirstOrDefault(f => f.Name == mesage.Key);
                    if (firstOrDefault != null && mesage.Key == firstOrDefault.Name)
                    {
                        firstOrDefault.Text = mesage.Value;
                    }
                }
            }
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            UserInformationList.Items.Clear();
            if (UserLoginOrGuidTextBox.Text.Contains('@'))
            {

                User u = _dataBase.GetUserBymailOrGuid(UserLoginOrGuidTextBox.Text, false);
                _dataBase.UpdateUserStatus(u, UserStatus.Passed.ToString(),false);
                if (File.Exists(PathToErrorLog))
                {
                    using (StreamReader sr = new StreamReader(PathToErrorLog))
                    {
                        string text = sr.ReadLine();
                        var err = (ErrorMessage)JsonConvert.DeserializeObject(text);
                        if (err != null)
                        {
                            ProcessInformationList.Items.Add(err.MessageBody);
                        }
                    }
                }

                UserInformationList.Items.Add($"Name - {u.UserName}");
                UserInformationList.Items.Add($"Mail - {u.UserMail}");
                UserInformationList.Items.Add($"Guid - {u.UserGuid}");
                UserInformationList.Items.Add($"Age - {u.Age}");
                UserInformationList.Items.Add($"Number - {(u.UserNumber)}");
                UserInformationList.Items.Add($"Id - {u.UserId}");
                UserInformationList.Items.Add($"Login Status - {u.Status}");
                if (u.Status == UserStatus.Passed.ToString())
                {
                    MessageBox.Show("OMG You are valid person");
                }
            }
            else
            {

                string userLoginOrGuid = UserLoginOrGuidTextBox.Text;

                if (File.Exists(PathToErrorLog))
                {
                    string errmesaEmpty;
                    using (StreamReader sr = new StreamReader(PathToErrorLog))
                    {
                        errmesaEmpty = sr.ReadToEnd();
                    }

                    ErrorMessage error = JsonConvert.DeserializeObject<ErrorMessage>(errmesaEmpty);
                    ProcessInformationList.Items.Add(error.MessageBody);
                    File.Delete(PathToErrorLog);
                }



                if (userLoginOrGuid.Length == Guid.NewGuid().ToString().Length)
                {
                    User u = _dataBase.GetUserBymailOrGuid(userLoginOrGuid, true);
                    UserInformationList.Items.Add($"Name - {u.UserName}");
                    UserInformationList.Items.Add($"Mail - {u.UserMail}");
                    UserInformationList.Items.Add($"Guid - {u.UserGuid}");
                    UserInformationList.Items.Add($"Age - {u.Age}");
                    UserInformationList.Items.Add($"Number - {(u.UserNumber)}");
                    UserInformationList.Items.Add($"Id - {u.UserId}");
                    UserInformationList.Items.Add($"Login Status - {u.Status}");
                }
                else
                {
                    User u = _dataBase.GetUserBymailOrGuid(userLoginOrGuid, false);
                    if (u.Status == UserStatus.Error.ToString())
                    {
                        MessageBox.Show("You Have problems , check right column");
                    }

                    UserInformationList.Items.Add($"Name - {u.UserName}");
                    UserInformationList.Items.Add($"Mail - {u.UserMail}");
                    UserInformationList.Items.Add($"Guid - {u.UserGuid}");
                    UserInformationList.Items.Add($"Age - {u.Age}");
                    UserInformationList.Items.Add($"Number - {(u.UserNumber)}");
                    UserInformationList.Items.Add($"Id - {u.UserId}");
                    UserInformationList.Items.Add($"Login Status - {u.Status}");
                }
            }

            RedactWrapPanel.Visibility = Visibility.Visible;
        }

        private string[] GetDataFromUserInformationList(int index,ItemCollection lb)
        {
            string NameOfField = lb.GetItemAt(index).ToString();
            NameOfField = NameOfField.Substring(0, NameOfField.IndexOf('-') - 1);
            string value = lb.GetItemAt(index).ToString();
            value = value.Substring(value.IndexOf('-')+1, value.Length - value.IndexOf('-') - 1).Replace(" ","");
            return new[]{NameOfField,value};
        }
        private void RedactButtonClick(object sender, RoutedEventArgs e)
        {

            ItemCollection lb = UserInformationList.Items;
          
            
            switch (GetDataFromUserInformationList(UserInformationList.SelectedIndex,lb)[0])
            {
                case "Name":
                    {
                        User u = _dataBase.GetUserBymailOrGuid(GetDataFromUserInformationList(0, lb)[1], false);
                        u.Status = UserStatus.OnModeration.ToString();
                        u.UserName = UserMailRedactTextBox.Text;
                        _dataBase.UpdateUserStatus(u, u.Status, true);
                        break;
                    }
                case "Mail":
                    {
                        User u = _dataBase.GetUserBymailOrGuid(GetDataFromUserInformationList(1, lb)[1], false);
                        u.Status = UserStatus.OnModeration.ToString();
                        u.UserMail = UserMailRedactTextBox.Text;
                        _dataBase.UpdateUserStatus(u, u.Status, true);
                        break;
                    }
                case "Age":
                {
                    User u = _dataBase.GetUserBymailOrGuid(GetDataFromUserInformationList(3, lb)[1], false);
                    u.Status = UserStatus.OnModeration.ToString();
                    int defage = 18;
                    int.TryParse(UserMailRedactTextBox.Text, out defage);
                    u.Age = defage;
                    _dataBase.UpdateUserStatus(u, u.Status, true);
                        break;
                }
                case "Number":
                {
                    User u = _dataBase.GetUserBymailOrGuid(GetDataFromUserInformationList(4, lb)[1], false);
                    u.Status = UserStatus.OnModeration.ToString();
                    u.UserNumber = UserMailRedactTextBox.Text;
                    _dataBase.UpdateUserStatus(u, u.Status, true);
                        break;
                }
                case "Guid":{MessageBox.Show("You can't change your guid"); break;}
                case "Id":{ break; }
            }

            MessageBox.Show("Данные были успешно отредактированы!");
        }
    }
}





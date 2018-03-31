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
using System.Windows.Media;
using Newtonsoft.Json;
using Producer;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;


namespace UserInterface
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Program.Db _dataBase = new Program.Db(Program.DocName + Program.DocExtension);

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
                SmtpClient client = new SmtpClient(smtpHost,smtpPort);
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
                { "UserNameTextBox","Your Name Here"},
                { "UserAgeTextBox","Your Age Here"},
                {"UserMailTextBox","Your Mail Here"},
                {"UserNumberTextBox","Your Number Here"}
            };

            //Process.Start("Consumer.exe","Error");
            if (File.Exists(@"C:\Users\user\Source\Repos\SystemProgramming_RabbitMq_-Bank-\Consumer\bin\Debug\ErrorLog.txt"))
            {
                File.Delete(@"C:\Users\user\Source\Repos\SystemProgramming_RabbitMq_-Bank-\Consumer\bin\Debug\ErrorLog.txt");
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
                    u.UserId = Program.Db.GetMaxId(AppDomain.CurrentDomain.BaseDirectory + Program.DocName + Program.DocExtension) + 1;
                    u.UserMail = UserMailTextBox.Text;
                    u.UserGuid = newGuid;
                    u.UserNumber = UserNumberTextBox.Text;
                    u.UserName = UserNameTextBox.Text;


                    _busService.PublishMessage((new ErrorMessage() { MessageBody = "Проблемы с тем что у вас тупо нет собачки) аахахаха, заведите собачку)", UserInfo = u }), "Error");
                    _dataBase.Add(u,u.Status);
                }
                else
                {
                    if (UserNumberTextBox.Text.Length != 12)
                    {
                        int.TryParse(UserAgeTextBox.Text, out defage);
                        u.Age = defage;
                        u.Status = UserStatus.Error.ToString();
                        u.UserId = Program.Db.GetMaxId(AppDomain.CurrentDomain.BaseDirectory + Program.DocName + Program.DocExtension) + 1;
                        u.UserMail = UserMailTextBox.Text;
                        u.UserGuid = newGuid;
                        u.UserNumber = UserNumberTextBox.Text;
                        u.UserName = UserNameTextBox.Text;
                        _busService.PublishMessage((new ErrorMessage() { MessageBody = "Проблемы с тем что у вас неправильный номер)", UserInfo = u }), "Error");
                        _dataBase.Add(u, u.Status);
                    }
                    else
                    {
                        int.TryParse(UserAgeTextBox.Text, out defage);
                        u.Age = defage;
                        u.Status = UserStatus.OnModeration.ToString();
                        u.UserId = Program.Db.GetMaxId(AppDomain.CurrentDomain.BaseDirectory + Program.DocName +
                                                       Program.DocExtension);
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
            string path = @"C:\Users\user\Source\Repos\SystemProgramming_RabbitMq_-Bank-\Consumer\bin\Debug\ErrorLog.txt";
            UserInformationList.Items.Clear();
            if (UserLoginOrGuidTextBox.Text.Contains('@'))
            {
               
                User u = _dataBase.GetUserBymailOrGuid(UserLoginOrGuidTextBox.Text, false);
                _dataBase.UpdateUserStatus(u, UserStatus.Passed.ToString());
                ErrorMessage err = new ErrorMessage();
                if (File.Exists(path))
                {
                    using (StreamReader sr = new StreamReader(
                        @"C:\Users\user\Source\Repos\SystemProgramming_RabbitMq_-Bank-\Consumer\bin\Debug\ErrorLog.txt")
                    )
                    {
                        string text = sr.ReadLine();
                        err = (ErrorMessage) JsonConvert.DeserializeObject(text);
                        if (err != null)
                        {
                            ProcessInformationList.Items.Add(err.MessageBody);
                        }
                    }
                }

                UserInformationList.Items.Add($"Your Name - {u.UserName}");
                UserInformationList.Items.Add($"Your Mail - {u.UserMail}");
                UserInformationList.Items.Add($"Your Guid - {u.UserGuid}");
                UserInformationList.Items.Add($"Your Age - {u.Age}");
                UserInformationList.Items.Add($"Your Number - {(u.UserNumber)}");
                UserInformationList.Items.Add($"Your Id - {u.UserId}");
               
                UserInformationList.Items.Add($"Your Login Status - {u.Status}");
                if (u.Status == UserStatus.Passed.ToString())
                {
                    MessageBox.Show("OMG You are valid person");
                }
            }
            else
            {
                string message = "";
                string userLoginOrGuid = UserLoginOrGuidTextBox.Text;
                string errmesaEmpty = "";
               
                if (File.Exists(path))
                {
                    using (StreamReader sr = new StreamReader(path))
                    {
                        errmesaEmpty = sr.ReadToEnd();
                    }
                    ErrorMessage error = JsonConvert.DeserializeObject<ErrorMessage>(errmesaEmpty);
                    ProcessInformationList.Items.Add(error.MessageBody);
                    File.Delete(path);
                }

               
               
                if (userLoginOrGuid.Length == Guid.NewGuid().ToString().Length)
                {
                    User u = _dataBase.GetUserBymailOrGuid(userLoginOrGuid, true);
                    UserInformationList.Items.Add($"Your Name - {u.UserName}");
                    UserInformationList.Items.Add($"Your Mail - {u.UserMail}");
                    UserInformationList.Items.Add($"Your Guid - {u.UserGuid}");
                    UserInformationList.Items.Add($"Your Age - {u.Age}");
                    UserInformationList.Items.Add($"Your Number - {(u.UserNumber)}");
                    UserInformationList.Items.Add($"Your Id - {u.UserId}");
                    UserInformationList.Items.Add($"Your Login Status - {u.Status}");
                }
                else
                {
                    User u = _dataBase.GetUserBymailOrGuid(userLoginOrGuid, false);
                    if (u.Status == UserStatus.Error.ToString())
                    {
                        MessageBox.Show("You Have problems , check right column");
                    }

                    UserInformationList.Items.Add($"Your Name - {u.UserName}");
                    UserInformationList.Items.Add($"Your Mail - {u.UserMail}");
                    UserInformationList.Items.Add($"Your Guid - {u.UserGuid}");
                    UserInformationList.Items.Add($"Your Age - {u.Age}");
                    UserInformationList.Items.Add($"Your Number - {(u.UserNumber)}");
                    UserInformationList.Items.Add($"Your Id - {u.UserId}");
                    UserInformationList.Items.Add($"Your Login Status - {u.Status}");
                }
            }
        }
    }

}

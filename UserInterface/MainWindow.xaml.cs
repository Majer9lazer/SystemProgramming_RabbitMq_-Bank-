using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace UserInterface
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            TextBoxMessages = new Dictionary<string, string>()
            {
                { "UserNameTextBox","Your Name Here"},
                { "UserAgeTextBox","Your Age Here"},
                {"UserMailTextBox","Your Mail Here"},
                {"UserNumberTextBox","Your Number Here"}
            };
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

        private void RegistrateButton_OnClick(object sender, RoutedEventArgs e)
        {
            TextBox[] arrTextBoxs ={UserNameTextBox,UserAgeTextBox,UserMailTextBox,UserNumberTextBox };
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
                MessageBox.Show("Добро мать его пожаловать! , ну почти)) ");
                foreach (KeyValuePair<string,string> mesage in TextBoxMessages)
                {
                    TextBox firstOrDefault = arrTextBoxs.FirstOrDefault(f => f.Name == mesage.Key);
                    if (firstOrDefault != null && mesage.Key == firstOrDefault.Name)
                    {
                        firstOrDefault.Text = mesage.Value;
                    }
                }
            }




        }
    }
}

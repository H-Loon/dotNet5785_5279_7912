using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
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

namespace PL
{
    /// <summary>
    /// Interaction logic for LogIn.xaml
    /// </summary>
    public partial class LogIn : UserControl
    {
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
        public IdText IdText { get; set; } = new IdText();
        private MainWindow mainWindow;
        private int _id;
        public string Password
        {
            get { return (string)GetValue(PasswordProperty); }
            set { SetValue(PasswordProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Password.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PasswordProperty =
            DependencyProperty.Register("Password", typeof(string), typeof(LogIn), new PropertyMetadata(string.Empty));


        public LogIn(MainWindow mw)
        {
            mainWindow = mw;
            InitializeComponent();
        }

        private void btn_LogIn(object sender, RoutedEventArgs e)
        {
            try
            {
                _id = int.Parse(IdText.IdTextString);
                string name = s_bl.Volunteer.GetVolunteer(_id).Name;
                
                if (s_bl.Volunteer.Login(name, Password) == BO.BoRoleType.Admin)
                {

                    if(mainWindow.isAdminConnected == true)
                    {
                        MessageBox.Show("Admin is already connected");
                        return;
                    }
                    
                    var selectedTab = mainWindow.tabDynamic.SelectedItem as TabItem;
                    if (selectedTab != null)
                    {
                        selectedTab.Header = name;
                        selectedTab.Content = new Admin.AdminMainView(name);
                        s_bl.Volunteer.AddObserver(_id, HeaderUpdate);
                    }
                    mainWindow.isAdminConnected = true;
                }
                else if (s_bl.Volunteer.Login(name, Password) == BO.BoRoleType.Volunteer)
                {

                    if (mainWindow.IsVolunteerConnected(name))
                    {
                        MessageBox.Show($"{name} is already connected");
                        return;
                    }

                    var selectedTab = mainWindow.tabDynamic.SelectedItem as TabItem;
                    if (selectedTab != null)
                    {
                        selectedTab.Header = name;
                        selectedTab.Content = new Volunteer.VolunteerMainView(int.Parse(IdText.IdTextString));
                        s_bl.Volunteer.AddObserver(_id, HeaderUpdate);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        public void HeaderUpdate()
        {
            if (mainWindow.Dispatcher.CheckAccess())
            {
                var selectedTab = mainWindow.tabDynamic.SelectedItem as TabItem;
                if (selectedTab != null)
                {
                    selectedTab.Header = s_bl.Volunteer.GetVolunteer(_id).Name;
                }
            }
            else
            {
                mainWindow.Dispatcher.Invoke(() => HeaderUpdate());
            }
        }
    }
    
    public class IdText : INotifyPropertyChanged
    {
        private string _idText;

        public string IdTextString
        {
            get => _idText;
            set
            {
                string formattedValue = FormatId(value);
                if (_idText != formattedValue)
                {
                    _idText = formattedValue;
                    OnPropertyChanged();
                }
            }
        }

        public IdText()
        {
            _idText = ""; // Default value
        }

        // Format the input string and enforce rules
        private string FormatId(string input)
        {
            if (string.IsNullOrEmpty(input))
                return "";

            // Remove all non-numeric characters
            string cleanInput = Regex.Replace(input, "[^0-9]", "");
            return cleanInput;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        }
    }
}

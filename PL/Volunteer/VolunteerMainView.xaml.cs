using PL.Admin;
using PL.Admin.Call;
using PL.Admin.Menu;
using PL.Admin.Volunteer;
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
using System.Xml.Linq;

namespace PL.Volunteer
{
    /// <summary>
    /// Interaction logic for VolunteerMainView.xaml
    /// </summary>
    public partial class VolunteerMainView : UserControl
    {
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
        public string Initials { get; set; }
        public History.HistoryView HistoryView { get; set; } = new History.HistoryView();
        public OpenCalls.OpenCallsInListView OpenCallsInListView { get; set; }
        public Menu.MenuView MenuView { get; set; }
        
        public UserControl CurrentView
        {
            get { return (UserControl)GetValue(CurrentViewProperty); }
            set { SetValue(CurrentViewProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CurrentView.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CurrentViewProperty =
            DependencyProperty.Register("CurrentView", typeof(UserControl), typeof(VolunteerMainView), new PropertyMetadata(null));

        public VolunteerMainView(string name, int id)
        {
            Initials = new string(name.Where(char.IsUpper).Take(2).ToArray()); // get the first two capital letters from the name
            MenuView = new Menu.MenuView(id);
            OpenCallsInListView = new OpenCalls.OpenCallsInListView(MenuView, id);
            CurrentView = MenuView;
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            s_bl.Volunteer.AddObserver(VolunteerInListView.VolunteerListObserver);
            s_bl.Call.AddObserver(CallInListView.CallListObserver);
        }
        private void CurrentViewToOpenCallsInList(object sender, RoutedEventArgs e)
        {
            CurrentView = OpenCallsInListView;
        }

        private void CurrentViewToCallHistory(object sender, RoutedEventArgs e)
        {
            CurrentView = HistoryView;
        }

        private void CurrentViewToMenu(object sender, RoutedEventArgs e)
        {
            CurrentView = MenuView;
        }
    }
}

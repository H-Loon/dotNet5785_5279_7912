using BO;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
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

namespace PL.Volunteer
{
    /// <summary>
    /// Interaction logic for AdminMainView.xaml
    /// </summary>
    public partial class AdminMainView : UserControl
    {
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
        public string Initials { get; set; }
        public AdminView AdminV { get; set; } = new AdminView();
        public VolunteerInListVM VolunteerInListV { get; set; } = new VolunteerInListVM();
        public UserControl CurrentView
        {
            get { return (UserControl)GetValue(CurrentViewProperty); }
            set { SetValue(CurrentViewProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CurrentView.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CurrentViewProperty =
            DependencyProperty.Register("CurrentView", typeof(UserControl), typeof(AdminMainView), new PropertyMetadata(null));

        public AdminMainView(string name)
        {
            Initials = new string(name.Where(char.IsUpper).Take(2).ToArray()); // get the first two capital letters from the name
            CurrentView = AdminV;
            InitializeComponent();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
            => s_bl.Volunteer.AddObserver(VolunteerInListV.VolunteerListObserver);

        //private void Window_Closed(object sender, EventArgs e)
        //        => s_bl.Volunteer.RemoveObserver(VolunteerInListV.VolunteerListObserver);
        private void CurrentViewToVolunteerInList(object sender, RoutedEventArgs e)
        {
            
            CurrentView = VolunteerInListV;
        }

        private void CurrentViewToAdmin(object sender, RoutedEventArgs e)
        {
            CurrentView = AdminV;
        }
    }
    public class ConvertReverseTrueKey : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((bool)value) ? false : true;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

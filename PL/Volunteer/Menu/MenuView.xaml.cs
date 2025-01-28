using PL.Admin.Call;
using System;
using System.Collections.Generic;
using System.Globalization;
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

namespace PL.Volunteer.Menu
{
    /// <summary>
    /// Interaction logic for MenuView.xaml
    /// </summary>
    public partial class MenuView : UserControl
    {
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
        public string VolunteerIdLabel { get; set; } = "ID: ";
        public string CallIdLabel { get; set; } = "ID: ";
        public string AssignList { get; set; }

        public readonly VolunteerMainView volunteerMainView;
        public Visibility CallVisibility
        {
            get { return (Visibility)GetValue(CallVisibilityProperty); }
            set { SetValue(CallVisibilityProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CallVisibility.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CallVisibilityProperty =
            DependencyProperty.Register("CallVisibility", typeof(Visibility), typeof(MenuView), new PropertyMetadata(Visibility.Hidden));

        public BO.Volunteer Volunteer
        {
            get { return (BO.Volunteer)GetValue(VolunteerProperty); }
            set { SetValue(VolunteerProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Volunteer.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty VolunteerProperty =
            DependencyProperty.Register("Volunteer", typeof(BO.Volunteer), typeof(MenuView));

        public BO.Call Call
        {
            get { return (BO.Call)GetValue(CallProperty); }
            set { SetValue(CallProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Call.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CallProperty =
            DependencyProperty.Register("Call", typeof(BO.Call), typeof(MenuView), new PropertyMetadata(null));


        public MenuView(VolunteerMainView vMainView,BO.Volunteer v)
        {
            volunteerMainView = vMainView;
            Volunteer = v;
            VolunteerIdLabel += Volunteer.Id;
            if (Volunteer.CurrentCall is not null)
            {
                Call = s_bl.Call.GetCall(Volunteer.CurrentCall.CallId);
                CallIdLabel += Call.Id;
                AssignList = ListToStr();
                CallVisibility = Visibility.Visible;
            }
            InitializeComponent();
        }
        public string ListToStr()
        {
            string result = "";
            foreach (var item in Call.AssignInList)
            {
                result += $"{item}\n";
            }
            return result;
        }
        public void VolunteerObserver()
        {
            Volunteer = s_bl.Volunteer.GetVolunteer(Volunteer.Id);
            Volunteer.Password = "";
            if (Volunteer.CurrentCall is not null)
            {
                Call = s_bl.Call.GetCall(Volunteer.CurrentCall.CallId);
                CallIdLabel = "ID: " + Call.Id;
                AssignList = ListToStr();
                CallVisibility = Visibility.Visible;
            }
            else
            {
                CallVisibility = Visibility.Hidden;
            }
        }
        private void UpdateVolunteer(object sender, RoutedEventArgs e)
        {
            s_bl.Volunteer.UpdateVolunteer(Volunteer.Id, Volunteer);
            Volunteer = s_bl.Volunteer.GetVolunteer(Volunteer.Id);
            Volunteer.Password = "";
        }
        private void CancelCall(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show($"Are you sure you want to Cancel this call, ID: {Call.Id}?", "Cancel Call", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                s_bl.Call.CancelCall(Volunteer.Id, Volunteer.CurrentCall!.AssignmentId);
                Volunteer = s_bl.Volunteer.GetVolunteer(Volunteer.Id);
                Volunteer.Password = "";
                CallVisibility = Visibility.Hidden;
                volunteerMainView.IsNotCall = true;
            }
        }
        private void CompleteCall(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show($"Are you sure you want to complete this call, ID: {Call.Id}?", "Complete Call", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                s_bl.Call.CompleteCall(Volunteer.Id, Volunteer.CurrentCall!.AssignmentId);
                Volunteer = s_bl.Volunteer.GetVolunteer(Volunteer.Id);
                Volunteer.Password = "";
                CallVisibility = Visibility.Hidden;
                volunteerMainView.IsNotCall = true;
            }
        }
    }
    //public class BoolToVisibilityConverter : IValueConverter
    //{
    //    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    //    {
    //        if (value is bool boolValue && boolValue)
    //        {
    //            return Visibility.Hidden;
    //        }
    //        return Visibility.Visible;
    //    }
    //    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    //    {
    //        throw new NotImplementedException();
    //    }
    //}
}

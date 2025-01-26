using PL.Volunteer.OpenCalls;
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

namespace PL.Volunteer.Menu
{
    /// <summary>
    /// Interaction logic for MenuView.xaml
    /// </summary>
    public partial class MenuView : UserControl
    {
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();

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


        public MenuView(int id)
        {
            Volunteer = s_bl.Volunteer.GetVolunteer(id);
            if (Volunteer.CurrentCall is not null)
            {
                Call = s_bl.Call.GetCall(Volunteer.CurrentCall.CallId);
                CallVisibility = Visibility.Visible;
            }
            InitializeComponent();
        }
        private void UpdateVolunteer(object sender, RoutedEventArgs e)
        {
            s_bl.Volunteer.UpdateVolunteer(Volunteer.Id, Volunteer);
            Volunteer = s_bl.Volunteer.GetVolunteer(Volunteer.Id);
        }
        private void CancelCall(object sender, RoutedEventArgs e)
        {
            s_bl.Call.CancelCall(Volunteer.Id, Call.Id);
            Volunteer = s_bl.Volunteer.GetVolunteer(Volunteer.Id);
            CallVisibility = Visibility.Hidden;
        }
        private void CompleteCall(object sender, RoutedEventArgs e)
        {
            s_bl.Call.CompleteCall(Volunteer.Id, Call.Id);
            Volunteer = s_bl.Volunteer.GetVolunteer(Volunteer.Id);
            CallVisibility = Visibility.Hidden;
        }
    }
}

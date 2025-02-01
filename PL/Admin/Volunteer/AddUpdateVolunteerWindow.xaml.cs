using System;
using System.Collections;
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
using System.Windows.Shapes;
using System.Windows.Threading;

namespace PL.Admin.Volunteer
{
    /// <summary>
    /// Interaction logic for AddUpdateVolunteerWindow.xaml
    /// </summary>
    public partial class AddUpdateVolunteerWindow : Window
    {
        private static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
        public string ButtonText { get; set; }



        public BO.BoRoleType Role
        {
            get { return (BO.BoRoleType)GetValue(RoleProperty); }
            set { SetValue(RoleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Role.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RoleProperty =
            DependencyProperty.Register("Role", typeof(BO.BoRoleType), typeof(AddUpdateVolunteerWindow));



        public BO.BoDistanceType DistanceType
        {
            get { return (BO.BoDistanceType)GetValue(DistanceTypeProperty); }
            set { SetValue(DistanceTypeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DistanceType.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DistanceTypeProperty =
            DependencyProperty.Register("DistanceType", typeof(BO.BoDistanceType), typeof(AddUpdateVolunteerWindow));


        public BO.Volunteer Volunteer
        {
            get { return (BO.Volunteer)GetValue(VolunteerProperty); }
            set { SetValue(VolunteerProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Volunteer.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty VolunteerProperty =
            DependencyProperty.Register("VolunteerW", typeof(BO.Volunteer), typeof(AddUpdateVolunteerWindow), new PropertyMetadata(null));


        public AddUpdateVolunteerWindow(int id = 0)
        {
            ButtonText = id == 0 ? "Add" : "Update";
            if (id == 0) Volunteer = new BO.Volunteer()
            {
                Id = 0,
                Name = "",
                Phone = "",
                Address = "",
                Email = "",
                Role = BO.BoRoleType.Volunteer,
                IsActive = true,
                MaxDistance = 0,
                DistanceType = BO.BoDistanceType.Area,
            };
            else Volunteer = s_bl.Volunteer.GetVolunteer(id);
            Role = Volunteer.Role;
            DistanceType = Volunteer.DistanceType;

            s_bl.Volunteer.AddObserver(FetchVolunteerInfo);

            InitializeComponent();
        }

        void btnAddUpdate_Click(object sender, RoutedEventArgs e)
        {
            bool flag = false;

            try
            {
                if (ButtonText == "Add") s_bl.Volunteer.AddVolunteer(Volunteer);
                else s_bl.Volunteer.UpdateVolunteer(Volunteer.Id, Volunteer);
                flag = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Invalid data: {ex.Message}", "Error");
            }

            if (flag)
            {
                s_bl.Volunteer.RemoveObserver(FetchVolunteerInfo);
                Close();
            }
        }
        private volatile DispatcherOperation? _observerOperation = null; //stage 7
        public void FetchVolunteerInfo()
        {
            if (_observerOperation is null || _observerOperation.Status == DispatcherOperationStatus.Completed)
                _observerOperation = Dispatcher.BeginInvoke(() =>
                {
                    if (ButtonText == "Update")
                                {
                                    Volunteer = s_bl.Volunteer.GetVolunteer(Volunteer.Id);
                                    Role = Volunteer.Role;
                                    DistanceType = Volunteer.DistanceType;
                                }
                });
        }

    }
    internal class VolunteerRole : IEnumerable
    {
        static readonly IEnumerable<BO.BoRoleType> s_enums =
        (Enum.GetValues(typeof(BO.BoRoleType)) as IEnumerable<BO.BoRoleType>)!;

        public IEnumerator GetEnumerator() => s_enums.GetEnumerator();
    }
    internal class DistanceType : IEnumerable
    {
        static readonly IEnumerable<BO.BoDistanceType> s_enums =
        (Enum.GetValues(typeof(BO.BoDistanceType)) as IEnumerable<BO.BoDistanceType>)!;

        public IEnumerator GetEnumerator() => s_enums.GetEnumerator();
    }
    public class ConvertUpdateToTrueKey : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((string)value == "Add") ? false : true;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    public class ConvertUpdateToVisibleKey : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((string)value == "Add") ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

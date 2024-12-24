using BO;
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

namespace PL.Volunteer
{
    /// <summary>
    /// Interaction logic for VolunteerWindow.xaml
    /// </summary>
    public partial class VolunteerWindow : Window
    {
        private static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
        public string ButtonText { get; set; }

        public BO.BoRoleType Role { get; set; }
        public BO.BoDistanceType DistanceType { get; set; }

        public BO.Volunteer Volunteer
        {
            get { return (BO.Volunteer)GetValue(VolunteerProperty); }
            set { SetValue(VolunteerProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Volunteer.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty VolunteerProperty =
            DependencyProperty.Register("VolunteerW", typeof(BO.Volunteer), typeof(VolunteerWindow), new PropertyMetadata(null));


        public VolunteerWindow(int id = 0)
        {
            ButtonText = id == 0 ? "Add" : "Update";
            if (id == 0) Volunteer = new BO.Volunteer() 
            {
                Id = 0,
                Name = "",
                Phone = "",
                Address = "",
                Email = "",
                Role = BoRoleType.Volunteer,
                IsActive = true,
                MaxDistance = 0,
                DistanceType = BoDistanceType.Area,
            };
            else Volunteer = s_bl.Volunteer.GetVolunteer(id);
            Role = Volunteer.Role;
            DistanceType = Volunteer.DistanceType;
            InitializeComponent();
        }

        void btnAddUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (ButtonText == "Add") s_bl.Volunteer.AddVolunteer(Volunteer);
            else s_bl.Volunteer.UpdateVolunteer(Volunteer.Id, Volunteer);

            Close();
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

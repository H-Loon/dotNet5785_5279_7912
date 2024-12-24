using System.Windows;
using System.Windows.Controls;

namespace PL.Volunteer
{
    /// <summary>
    /// Interaction logic for VolunteerV.xaml
    /// </summary>
    public partial class VolunteerV : UserControl
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
            DependencyProperty.Register("VolunteerV", typeof(BO.Volunteer), typeof(VolunteerWindow), new PropertyMetadata(null));


        public VolunteerV(int id)
        {
            Volunteer = s_bl.Volunteer.GetVolunteer(id);
            Role = Volunteer.Role;
            DistanceType = Volunteer.DistanceType;
            InitializeComponent();
        }
    }
}

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

namespace PL.Admin.Call
{
    /// <summary>
    /// Logique d'interaction pour CallInfoView.xaml
    /// </summary>
    public partial class CallInfoView : UserControl
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
            DependencyProperty.Register("VolunteerV", typeof(BO.Volunteer), typeof(VolunteerInfoView), new PropertyMetadata(null));


        public CallInfoView(int id)
        {
            Volunteer = s_bl.Volunteer.GetVolunteer(id);
            Role = Volunteer.Role;
            DistanceType = Volunteer.DistanceType;
            InitializeComponent();
        }
    }
}

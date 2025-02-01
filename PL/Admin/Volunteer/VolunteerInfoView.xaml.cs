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
using System.Windows.Threading;

namespace PL.Admin.Volunteer
{
    /// <summary>
    /// Interaction logic for VolunteerInfoView.xaml
    /// </summary>
    public partial class VolunteerInfoView : UserControl
    {
        private static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
        public string ButtonText { get; set; }
        private int _id;
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


        public VolunteerInfoView(int id)
        {
            _id = id;
            FetchVolunteerInfo();
            s_bl.Volunteer.AddObserver(FetchVolunteerInfo);
            InitializeComponent();
        }

        private volatile DispatcherOperation? _observerOperation = null; //stage 7
        public void FetchVolunteerInfo()
        {
            if (_observerOperation is null || _observerOperation.Status == DispatcherOperationStatus.Completed)
                _observerOperation = Dispatcher.BeginInvoke(() =>
                {
                    Volunteer = s_bl.Volunteer.GetVolunteer(_id);
                                Role = Volunteer.Role;
                                DistanceType = Volunteer.DistanceType;
                });
        }
    }
}

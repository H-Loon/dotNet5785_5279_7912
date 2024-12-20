using System;
using System.Collections;
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
using System.Windows.Shapes;

namespace PL.Volunteer
{
    /// <summary>
    /// Interaction logic for VolunteerInListWindow.xaml
    /// </summary>
    public partial class VolunteerInListWindow : Window
    {
        private static readonly BlApi.IBl s_bl = BlApi.Factory.Get();

        public BO.VolunteerInList? SelectedVolunteer { get; set; }
        public BO.VolunteerInListField VolunteerInListField { get; set; } = BO.VolunteerInListField.Id;
        public IEnumerable<BO.VolunteerInList> VolunteerInList
        {
            get { return (IEnumerable<BO.VolunteerInList>)GetValue(VolunteerInListProperty); }
            set { SetValue(VolunteerInListProperty, value); }
        }

        // Using a DependencyProperty as the backing store for VolunteerInList.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty VolunteerInListProperty =
            DependencyProperty.Register("VolunteerInList", typeof(IEnumerable<BO.VolunteerInList>), typeof(VolunteerInListWindow));
       
        public VolunteerInListWindow()
        {
            InitializeComponent();
        }

        private void QueryVolunteerList( object sender, RoutedEventArgs e)
            => VolunteerInList = s_bl.Volunteer.GetVolunteerInList(null, VolunteerInListField);
        private void VolunteerListObserver()
            => VolunteerInList = s_bl.Volunteer.GetVolunteerInList(null, VolunteerInListField);
        private void Window_Loaded(object sender, RoutedEventArgs e)
            => s_bl.Volunteer.AddObserver(VolunteerListObserver);

        private void Window_Closed(object sender, EventArgs e)
            => s_bl.Volunteer.RemoveObserver(VolunteerListObserver);

        private void OpenVolunteerWindowAdd(object sender, RoutedEventArgs e)
        {
            new VolunteerWindow().Show();
        }
        private void lsvVolunteersList_MouseDoubleClick(object sender, RoutedEventArgs e)
        {
            new VolunteerWindow(SelectedVolunteer!.Id).Show();
        }
        void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.CommandParameter is BO.VolunteerInList volunteer)
                if (MessageBox.Show($"Are you sure you want to delete {volunteer.Name}?", "Delete Volunteer", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                    s_bl.Volunteer.DeleteVolunteer(volunteer.Id);
        }
    }
    internal class VolunteerField : IEnumerable
    {
        static readonly IEnumerable<BO.VolunteerInListField> s_enums =
        (Enum.GetValues(typeof(BO.VolunteerInListField)) as IEnumerable<BO.VolunteerInListField>)!;

        public IEnumerator GetEnumerator() => s_enums.GetEnumerator();
    }


}

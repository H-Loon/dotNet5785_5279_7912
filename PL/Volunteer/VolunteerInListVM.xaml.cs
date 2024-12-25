using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
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

namespace PL.Volunteer
{
    /// <summary>
    /// Interaction logic for VolunteerInListVM.xaml
    /// </summary>
    public partial class VolunteerInListVM : UserControl
    {
        private static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
        private int Id = -1;

        public VolunteerV VolunteerView
        {
            get { return (VolunteerV)GetValue(VolunteerViewProperty); }
            set { SetValue(VolunteerViewProperty, value); }
        }

        // Using a DependencyProperty as the backing store for VolunteerView.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty VolunteerViewProperty =
            DependencyProperty.Register("VolunteerView", typeof(VolunteerV), typeof(VolunteerInListVM), new PropertyMetadata(null));

        private BO.VolunteerInList? _selectedVolunteer;
        public BO.VolunteerInList? SelectedVolunteer
        {
            get => _selectedVolunteer;
            set
            {
                _selectedVolunteer = value;
                if (value != null)
                    VolunteerView = new VolunteerV(value.Id);
            }
        }

        public BO.VolunteerInListField VolunteerInListField { get; set; } = BO.VolunteerInListField.Id;
        public IEnumerable<BO.VolunteerInList> VolunteerInList
        {
            get { return (IEnumerable<BO.VolunteerInList>)GetValue(VolunteerInListProperty); }
            set { SetValue(VolunteerInListProperty, value); }
        }

        // Using a DependencyProperty as the backing store for VolunteerInList.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty VolunteerInListProperty =
            DependencyProperty.Register("VolunteerInList", typeof(IEnumerable<BO.VolunteerInList>), typeof(VolunteerInListVM));
        public VolunteerInListVM()
        {
            InitializeComponent();
        }
        private void QueryVolunteerList(object sender, RoutedEventArgs e)
            => VolunteerInList = s_bl.Volunteer.GetVolunteerInList(null, VolunteerInListField);
        public void VolunteerListObserver()
        {
            VolunteerInList = s_bl.Volunteer.GetVolunteerInList(null, VolunteerInListField);
        }

        private void VolunteerVAdd(object sender, RoutedEventArgs e)
        {
            new VolunteerWindow().Show();
        }
        private void lsvVolunteersList_MouseDoubleClick(object sender, RoutedEventArgs e)
        {
            // Check if the window is already open
            foreach (Window window in Application.Current.Windows)
            {
                if (window is VolunteerWindow)
                {
                    if (Id != SelectedVolunteer!.Id)
                    {
                        window.Close();
                        Id = SelectedVolunteer!.Id;
                        new VolunteerWindow(Id).Show();
                        return;
                    }
                    // Bring the existing window to the front
                    window.Activate();
                    return; // Exit the method
                }
            }
            Id = SelectedVolunteer!.Id;
            new VolunteerWindow(SelectedVolunteer!.Id).Show();
        }
        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.CommandParameter is BO.VolunteerInList volunteer)
                if (MessageBox.Show($"Are you sure you want to delete {volunteer.Name}?", "Delete Volunteer", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                    s_bl.Volunteer.DeleteVolunteer(volunteer.Id);
        }

    }
    public class IsDeletable : IValueConverter
    {
        private static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return s_bl.Volunteer.IsDeletable((int)value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    internal class VolunteerField : IEnumerable
    {
        static readonly IEnumerable<BO.VolunteerInListField> s_enums =
        (Enum.GetValues(typeof(BO.VolunteerInListField)) as IEnumerable<BO.VolunteerInListField>)!;

        public IEnumerator GetEnumerator() => s_enums.GetEnumerator();
    }
}

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
using System.Windows.Threading;

namespace PL.Admin.Volunteer
{
    public enum ActiveField { All, Active, Inactive }
    /// <summary>
    /// Interaction logic for VolunteerInListView.xaml
    /// </summary>
    public partial class VolunteerInListView : UserControl
    {
        private static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
        private int Id = -1;

        public VolunteerInfoView VolunteerInfo
        {
            get { return (VolunteerInfoView)GetValue(VolunteerViewProperty); }
            set { SetValue(VolunteerViewProperty, value); }
        }

        // Using a DependencyProperty as the backing store for VolunteerInfo.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty VolunteerViewProperty =
            DependencyProperty.Register("VolunteerInfo", typeof(VolunteerInfoView), typeof(VolunteerInListView), new PropertyMetadata(null));

        private BO.VolunteerInList? _selectedVolunteer;
        public BO.VolunteerInList? SelectedVolunteer
        {
            get => _selectedVolunteer;
            set
            {
                _selectedVolunteer = value;
                if (value != null)
                    VolunteerInfo = new VolunteerInfoView(value.Id);
            }
        }

        public BO.VolunteerInListField VolunteerInListField { get; set; } = BO.VolunteerInListField.Id;
        public ActiveField VActiveField { get; set; } = ActiveField.All;
        public IEnumerable<BO.VolunteerInList> VolunteerInList
        {
            get { return (IEnumerable<BO.VolunteerInList>)GetValue(VolunteerInListProperty); }
            set { SetValue(VolunteerInListProperty, value); }
        }

        // Using a DependencyProperty as the backing store for VolunteerInList.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty VolunteerInListProperty =
            DependencyProperty.Register("VolunteerInList", typeof(IEnumerable<BO.VolunteerInList>), typeof(VolunteerInListView));
        public VolunteerInListView()
        {
            InitializeComponent();
        }
        private void QueryVolunteerList(object sender, RoutedEventArgs e)
        {
            switch (VActiveField)
            {
                case ActiveField.All:
                    VolunteerInList = s_bl.Volunteer.GetVolunteerInList(null, VolunteerInListField);
                    break;
                case ActiveField.Active:
                    VolunteerInList = s_bl.Volunteer.GetVolunteerInList(true, VolunteerInListField);
                    break;
                case ActiveField.Inactive:
                    VolunteerInList = s_bl.Volunteer.GetVolunteerInList(false, VolunteerInListField);
                    break;
            }
        }

        private volatile DispatcherOperation? _observerOperation = null; //stage 7
        public void VolunteerListObserver()
        {
            if (_observerOperation is null || _observerOperation.Status == DispatcherOperationStatus.Completed)
                _observerOperation = Dispatcher.BeginInvoke(() =>
                {
                    switch (VActiveField)
                                {
                                    case ActiveField.All:
                                        VolunteerInList = s_bl.Volunteer.GetVolunteerInList(null, VolunteerInListField);
                                        break;
                                    case ActiveField.Active:
                                        VolunteerInList = s_bl.Volunteer.GetVolunteerInList(true, VolunteerInListField);
                                        break;
                                    case ActiveField.Inactive:
                                        VolunteerInList = s_bl.Volunteer.GetVolunteerInList(false, VolunteerInListField);
                                        break;
                                }
                });
        }

        private void VolunteerVAdd(object sender, RoutedEventArgs e)
        {
            new AddUpdateVolunteerWindow().Show();
        }
        private void lsvVolunteersList_MouseDoubleClick(object sender, RoutedEventArgs e)
        {
            // Check if the window is already open
            foreach (Window window in Application.Current.Windows)
            {
                if (window is VolunteerInfoView)
                {
                    if (Id != SelectedVolunteer!.Id)
                    {
                        window.Close();
                        Id = SelectedVolunteer!.Id;
                        new AddUpdateVolunteerWindow(Id).Show();
                        return;
                    }
                    // Bring the existing window to the front
                    window.Activate();
                    return; // Exit the method
                }
            }
            Id = SelectedVolunteer!.Id;
            new AddUpdateVolunteerWindow(SelectedVolunteer!.Id).Show();
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
    internal class VolunteerActiveField : IEnumerable
    {
        static readonly IEnumerable<ActiveField> s_enums =
        (Enum.GetValues(typeof(ActiveField)) as IEnumerable<ActiveField>)!;

        public IEnumerator GetEnumerator() => s_enums.GetEnumerator();
    }
}

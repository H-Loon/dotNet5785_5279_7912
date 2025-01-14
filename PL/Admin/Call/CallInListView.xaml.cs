using PL.Admin.Volunteer;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PL.Admin.Call
{
    /// <summary>
    /// Logique d'interaction pour CallInListView.xaml
    /// </summary>
    public partial class CallInListView : UserControl
    {
        /// <summary>
        /// Interaction logic for VolunteerInListView.xaml
        /// </summary>
    
        private static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
        private int Id = -1;

        public CallInfoView CallInfo
        {
            get { return (CallInfoView)GetValue(CallInfoViewProperty); }
            set { SetValue(CallInfoViewProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CallInfoView.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CallInfoViewProperty =
            DependencyProperty.Register("CallInfoV", typeof(CallInfoView), typeof(CallInListView), new PropertyMetadata(null));

        private BO.CallInList? _selectedCall;
        public BO.CallInList? SelectedCall
        {
            get => _selectedCall;
            set
            {
                _selectedCall = value;
                if (value != null)
                    CallInfo = new CallInfoView(value.CallId);
            }
        }

        public BO.CallInListField CallInListFieldFiltred { get; set; } = BO.CallInListField.None;
        public BO.CallInListField CallInListFieldSorted { get; set; } = BO.CallInListField.CallId;
        public IEnumerable<BO.CallInList> CallInList
        {
            get { return (IEnumerable<BO.CallInList>)GetValue(CallInlistProperty); }
            set { SetValue(CallInlistProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CallInList.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CallInlistProperty =
            DependencyProperty.Register("CallInList", typeof(IEnumerable<BO.CallInList>), typeof(CallInListView));
        public CallInListView()
        {
            InitializeComponent();
        }
        private void QueryCallList(object sender, RoutedEventArgs e)
        {
            if(sender is TextBox tb)
                CallInList = s_bl.Call.GetCallsInList(CallInListFieldFiltred,tb.Text,CallInListFieldSorted);
            if (sender is DatePicker dp)
                CallInList = s_bl.Call.GetCallsInList(CallInListFieldFiltred, DateTime.Parse(dp.Text), CallInListFieldSorted);
            if (sender is ComboBox cb)
                CallInList = s_bl.Call.GetCallsInList(CallInListFieldFiltred, cb.SelectedItem, CallInListFieldSorted);
        }
        public void VolunteerListObserver()
        {
            switch (VActiveField)
            {
                case ActiveField.All:
                    CallInList = s_bl.Volunteer.GetVolunteerInList(null, CallInListField);
                    break;
                case ActiveField.Active:
                    CallInList = s_bl.Volunteer.GetVolunteerInList(true, CallInListField);
                    break;
                case ActiveField.Inactive:
                    CallInList = s_bl.Volunteer.GetVolunteerInList(false, CallInListField);
                    break;
            }
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
                    if (Id != SelectedCall!.Id)
                    {
                        window.Close();
                        Id = SelectedCall!.Id;
                        new AddUpdateVolunteerWindow(Id).Show();
                        return;
                    }
                    // Bring the existing window to the front
                    window.Activate();
                    return; // Exit the method
                }
            }
            Id = SelectedCall!.Id;
            new AddUpdateVolunteerWindow(SelectedCall!.Id).Show();
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

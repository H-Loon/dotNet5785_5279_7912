using PL.Admin.Call;
using PL.Admin.Menu;
using PL.Volunteer.Menu;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
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

namespace PL.Volunteer.OpenCalls
{
    /// <summary>
    /// Interaction logic for OpenCallsInListView.xaml
    /// </summary>
    public partial class OpenCallsInListView : UserControl
    {
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
        private Menu.MenuView _menuView;
        public bool IsCallSelected = false;
        public object? FilterValue = null;
        private bool flag1 = false, flag2 = true;

        //public IEnumerable EnumSource { get; private set; }

        public IEnumerable EnumSource
        {
            get { return (IEnumerable)GetValue(EnumSourceProperty); }
            set { SetValue(EnumSourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for EnumSource.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty EnumSourceProperty =
            DependencyProperty.Register("EnumSource", typeof(IEnumerable), typeof(CallInListView));

        public void SetEnumSource(Type enumType)
        {
            if (enumType.FullName is "BO.BoCallType")
                FilterValue = BO.BoCallType.None;
            else if (enumType.FullName is "BO.BoCallStatus" && flag1)
                FilterValue = null;
            EnumSource = new EnumItemSource(enumType);
        }
        public int Days { get; set; }

        private DateTime _selectedTime = default;
        public DateTime SelectedTime
        {
            get => _selectedTime;
            set
            {
                _selectedTime = value;
                FilterValue = _selectedDate.Add(new TimeSpan(SelectedTime.Hour, SelectedTime.Minute, 0));
            }
        }
        private DateTime _selectedDate = s_bl.Admin.GetConfigClock();
        public DateTime SelectedDate
        {
            get => _selectedDate;
            set
            {
                _selectedDate = value;
                FilterValue = _selectedDate.Add(new TimeSpan(SelectedTime.Hour, SelectedTime.Minute, 0));
            }
        }

        public Visibility DaysTimeSpanVisibility
        {
            get { return (Visibility)GetValue(DaysTimeSpanVisibilityProperty); }
            set { SetValue(DaysTimeSpanVisibilityProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DaysTimeSpanVisibility.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DaysTimeSpanVisibilityProperty =
            DependencyProperty.Register("DaysTimeSpanVisibility", typeof(Visibility), typeof(CallInListView), new PropertyMetadata(Visibility.Hidden));


        public Visibility TimeSpanVisibility
        {
            get { return (Visibility)GetValue(TimeSpanVisibilityProperty); }
            set { SetValue(TimeSpanVisibilityProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TimeSpanVisibility.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TimeSpanVisibilityProperty =
            DependencyProperty.Register("TimeSpanVisibility", typeof(Visibility), typeof(CallInListView), new PropertyMetadata(Visibility.Hidden));


        public Visibility NumericUpDownVisibility
        {
            get { return (Visibility)GetValue(NumericUpDownVisibilityProperty); }
            set { SetValue(NumericUpDownVisibilityProperty, value); }
        }

        // Using a DependencyProperty as the backing store for NumericUpDownVisibility.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty NumericUpDownVisibilityProperty =
            DependencyProperty.Register("NumericUpDownVisibility", typeof(Visibility), typeof(CallInListView), new PropertyMetadata(Visibility.Hidden));


        public Visibility DateTimeVisibility
        {
            get { return (Visibility)GetValue(DateTimeVisibilityProperty); }
            set { SetValue(DateTimeVisibilityProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DateTimeVisibility.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DateTimeVisibilityProperty =
            DependencyProperty.Register("DateTimeVisibility", typeof(Visibility), typeof(CallInListView), new PropertyMetadata(Visibility.Hidden));


        public Visibility ComboBoxVisibility
        {
            get { return (Visibility)GetValue(ComboBoxVisibilityProperty); }
            set { SetValue(ComboBoxVisibilityProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ComboBoxVisibility.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ComboBoxVisibilityProperty =
            DependencyProperty.Register("ComboBoxVisibility", typeof(Visibility), typeof(CallInListView), new PropertyMetadata(Visibility.Hidden));



        public Visibility TextBoxVisibility
        {
            get { return (Visibility)GetValue(TextBoxVisibilityProperty); }
            set { SetValue(TextBoxVisibilityProperty, value); }
        }

        // Using a DependencyProperty as the backing store for  TextBoxVisibility.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TextBoxVisibilityProperty =
            DependencyProperty.Register("TextBoxVisibility", typeof(Visibility), typeof(CallInListView), new PropertyMetadata(Visibility.Hidden));

        private BO.OpenCallInList? _selectedCall = null; 
        public BO.OpenCallInList? SelectedCall
        {
            get => _selectedCall;
            set
            {
                _selectedCall = value;
                if (_selectedCall != null)
                {
                    IsCallSelected = true;
                }
            } 
                
        }
        public OpenCallsInListView(Menu.MenuView menuView, int id)
        {
            _menuView = menuView;
            InitializeComponent();
        }
        public BO.CallInListField CallInListFldFiltred
        {
            get { return (BO.CallInListField)GetValue(CallInListFldFiltredProperty); }
            set { SetValue(CallInListFldFiltredProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CallInListFldFiltred.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CallInListFldFiltredProperty =
            DependencyProperty.Register("CallInListFldFiltred", typeof(BO.CallInListField), typeof(CallInListView), new PropertyMetadata(BO.CallInListField.None));

        public BO.CallInListField CallInListFieldSorted { get; set; } = BO.CallInListField.CallId;
        public IEnumerable<BO.CallInList> CallInList
        {
            get { return (IEnumerable<BO.CallInList>)GetValue(CallInListProperty); }
            set { SetValue(CallInListProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CallInList.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CallInListProperty =
            DependencyProperty.Register("CallInList", typeof(IEnumerable<BO.CallInList>), typeof(CallInListView));

        private void FilterVisibilitySwitch()
        {
            if (flag1)
                flag2 = true;
            ComboBoxVisibility = Visibility.Hidden;
            TextBoxVisibility = Visibility.Hidden;
            DateTimeVisibility = Visibility.Hidden;
            NumericUpDownVisibility = Visibility.Hidden;
            TimeSpanVisibility = Visibility.Hidden;
            DaysTimeSpanVisibility = Visibility.Hidden;

            switch (CallInListFldFiltred)
            {
                case BO.CallInListField.AssignmentId:
                    TextBoxVisibility = Visibility.Visible;
                    break;
                case BO.CallInListField.CallId:
                    TextBoxVisibility = Visibility.Visible;
                    break;
                case BO.CallInListField.CallType:
                    SetEnumSource(typeof(BO.BoCallType));
                    ComboBoxVisibility = Visibility.Visible;
                    break;
                case BO.CallInListField.StartTime:
                    DateTimeVisibility = Visibility.Visible;
                    TimeSpanVisibility = Visibility.Visible;
                    break;
                case BO.CallInListField.TimeLeft:
                    TimeSpanVisibility = Visibility.Visible;
                    DaysTimeSpanVisibility = Visibility.Visible;
                    break;
                case BO.CallInListField.TimeOpen:
                    TimeSpanVisibility = Visibility.Visible;
                    DaysTimeSpanVisibility = Visibility.Visible;
                    break;
                case BO.CallInListField.LastVolunteerName:
                    TextBoxVisibility = Visibility.Visible;
                    break;
                case BO.CallInListField.CallStatus:
                    SetEnumSource(typeof(BO.BoCallStatus));
                    ComboBoxVisibility = Visibility.Visible;
                    break;
                case BO.CallInListField.AssignCount:
                    NumericUpDownVisibility = Visibility.Visible;
                    break;
                default:
                    break;
            }
        }
        private void SortCallList(object sender, RoutedEventArgs e)
        {
            if (flag2)
            {
                CallInList = s_bl.Call.GetCallsInList(null, null, CallInListFieldSorted);
                flag2 = false;
            }
            else
                CallListObserver();
        }
        private void QueryCallList(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox tb)
            {
                CallInList = s_bl.Call.GetCallsInList(CallInListFldFiltred, tb.Text, CallInListFieldSorted);
                FilterValue = tb.Text;
            }
            else if (sender is ComboBox cb)
            {
                CallInList = s_bl.Call.GetCallsInList(CallInListFldFiltred, cb.SelectedItem, CallInListFieldSorted);
                FilterValue = cb.SelectedItem;
            }
            else if (sender is MaterialDesignThemes.Wpf.NumericUpDown n1 && CallInListFldFiltred == BO.CallInListField.AssignCount)
            {
                CallInList = s_bl.Call.GetCallsInList(CallInListFldFiltred, n1.Value, CallInListFieldSorted);
                FilterValue = n1.Value;
            }
            else if (CallInListFldFiltred == BO.CallInListField.TimeLeft || CallInListFldFiltred == BO.CallInListField.TimeOpen)
            {
                TimeSpan time = new TimeSpan(Days, SelectedTime.Hour, SelectedTime.Minute, 0);
                CallInList = s_bl.Call.GetCallsInList(CallInListFldFiltred, time, CallInListFieldSorted);
                FilterValue = time;
            }
            else if (flag2)
            {
                CallListObserver();
            }
        }

        public void CallListObserver()
        {
            if (FilterValue != null && FilterValue.GetType() != GetFilterType(CallInListFldFiltred))
            {
                FilterValue = null;
            }
            CallInList = s_bl.Call.GetCallsInList(CallInListFldFiltred, FilterValue, CallInListFieldSorted);
            FilterVisibilitySwitch();
        }

        private Type GetFilterType(BO.CallInListField field)
        {
            return field switch
            {
                BO.CallInListField.AssignmentId => typeof(string),
                BO.CallInListField.CallId => typeof(string),
                BO.CallInListField.CallType => typeof(BO.BoCallType),
                BO.CallInListField.StartTime => typeof(DateTime),
                BO.CallInListField.TimeLeft => typeof(TimeSpan),
                BO.CallInListField.LastVolunteerName => typeof(string),
                BO.CallInListField.TimeOpen => typeof(TimeSpan),
                BO.CallInListField.CallStatus => typeof(BO.BoCallStatus),
                BO.CallInListField.AssignCount => typeof(int),
                _ => typeof(object),
            };
        }
        private void AcceptCall(object sender, RoutedEventArgs e)
        {
            _menuView.CallVisibility = Visibility.Visible;
            _menuView.Call = s_bl.Call.GetCall(_selectedCall!.Id);
        }
    }
}

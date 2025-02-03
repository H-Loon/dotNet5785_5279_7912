using DO;
using PL.Admin.Call;
using PL.Volunteer.History;
using PL.Volunteer.OpenCalls;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
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

namespace PL.Admin.Call
{
    /// <summary>
    /// Logique d'interaction pour AddUpdateCallWindow.xaml
    /// </summary>
    public partial class AddUpdateCallWindow : Window
    {
        private static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
        //private bool flag = true;
        public string ButtonText { get; set; }

        public bool ToggleMTbool { get; set; } = false;
        public string IdLabel { get; set; } = "ID: ";

        private BO.BoCallType _callType;
        public BO.BoCallType CallType
        {
            get => _callType;
            set
            {
                //if (flag)
                //{
                //    _callType = value;
                //    flag = false;
                //}
                _callType = value;
                Call.CallType = value;
            }
        }
        public BO.BoCallStatus Status { get; set; }

        private DateTime _selectedTime = default;
        public DateTime SelectedTime
        {
            get => _selectedTime;
            set
            {
                //if (flag)
                //{
                //    _selectedTime = value;
                //    flag = false;
                //}
                _selectedTime = value;
                Call.MaxTime = new DateTime(_selectedDate.Year,_selectedDate.Month,_selectedDate.Day,_selectedTime.Hour,_selectedTime.Minute,0);
            }
        }
        private DateTime _selectedDate = s_bl.Admin.GetConfigClock();
        public DateTime SelectedDate
        {
            get => _selectedDate;
            set
            {
                //if (flag)
                //{
                //    _selectedTime = value;
                //    flag = false;
                //}
                _selectedDate = value;
                Call.MaxTime = new DateTime(_selectedDate.Year, _selectedDate.Month, _selectedDate.Day, _selectedTime.Hour, _selectedTime.Minute, 0);
            }
        }

        public BO.Call Call
        {
            get { return (BO.Call)GetValue(CallProperty); }
            set { SetValue(CallProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Volunteer.  This enables styling, binding, etc...
        public static readonly DependencyProperty CallProperty =
            DependencyProperty.Register("Call", typeof(BO.Call), typeof(AddUpdateCallWindow), new PropertyMetadata(null));



        public string AssignList
        {
            get { return (string)GetValue(AssignListProperty); }
            set { SetValue(AssignListProperty, value); }
        }

        // Using a DependencyProperty as the backing store for AssignList.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AssignListProperty =
            DependencyProperty.Register("AssignList", typeof(string), typeof(AddUpdateCallWindow), new PropertyMetadata(string.Empty));


        public AddUpdateCallWindow(int id = 0)
        {
            IdLabel += id.ToString();
            ButtonText = id == 0 ? "Add" : "Update";
            if (id == 0) Call = new BO.Call()
            {
                Id = 0,
                Description = "",
                Address = "",
                CallType = BO.BoCallType.Other

            };
            else
            { 
                Call = s_bl.Call.GetCall(id);
                if (Call.AssignInList != null)
                    AssignList = ListToStr();
                else
                    AssignList = "";
            }
            if (Call.MaxTime != null)
                ToggleMTbool = true;
            CallType = Call.CallType;
            SelectedDate = Call.MaxTime ?? s_bl.Admin.GetConfigClock();
            SelectedTime = Call.MaxTime ?? SelectedDate;
            
            InitializeComponent();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            s_bl.Call.AddObserver(FetchCallInfo);
        }
        private void Window_Closed(object sender, EventArgs e)
        {
            s_bl.Call.RemoveObserver(FetchCallInfo);
        }

        private string ListToStr()
        {
            string result = "";
            foreach (var item in Call.AssignInList)
            {
                result += $"{item}\n";
            }
            return result;
        }

        void btnAddUpdate_Click(object sender, RoutedEventArgs e)
        {
            bool flag = false;

            try
            {
                if (!ToggleMTbool)
                {
                    Call.MaxTime = null;
                }
                if (ButtonText == "Add") s_bl.Call.AddCall(Call);
                else s_bl.Call.UpdateCall(Call);
                flag = true;
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }

            if (flag)
            {   
                Close();
            }
        }
        private volatile DispatcherOperation? _observerOperation = null; //stage 7
        public void FetchCallInfo()
        {
            if (_observerOperation is null || _observerOperation.Status == DispatcherOperationStatus.Completed)
                _observerOperation = Dispatcher.BeginInvoke(() =>
                {
                    if (Call.Id == 0)
                        return;

                    Call = s_bl.Call.GetCall(Call.Id);
                    if (Call.AssignInList != null)
                        AssignList = ListToStr();
                });
        }

        
    }
    internal class CallType : IEnumerable
    {
        static readonly IEnumerable<BO.BoCallType> s_enums =
        (Enum.GetValues(typeof(BO.BoCallType)) as IEnumerable<BO.BoCallType>)!;

        public IEnumerator GetEnumerator() => s_enums.GetEnumerator();
    }
    internal class Status : IEnumerable
    {
        static readonly IEnumerable<BO.BoCallStatus> s_enums =
        (Enum.GetValues(typeof(BO.BoCallStatus)) as IEnumerable<BO.BoCallStatus>)!;

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

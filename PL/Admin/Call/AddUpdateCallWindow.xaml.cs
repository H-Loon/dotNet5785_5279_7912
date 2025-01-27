using DO;
using PL.Admin.Call;
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

        public string AssignList { get; set; }

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
            SelectedTime = Call.MaxTime ?? s_bl.Admin.GetConfigClock();
            InitializeComponent();
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
                Close();
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

using PL.Admin.Call;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;

namespace PL.Admin.Menu
{
    /// <summary>
    /// Interaction logic for MenuView.xaml
    /// </summary>
    public partial class MenuView : UserControl
    {
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();

        public RiskRangeEditor RiskRangeEdt { get; set; } = new RiskRangeEditor();

        public DateTime ConfigTime
        {
            get { return (DateTime)GetValue(ConfigTimeNowProperty); }
            set { SetValue(ConfigTimeNowProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DateTimeNow.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ConfigTimeNowProperty =
            DependencyProperty.Register("ConfigTime", typeof(DateTime), typeof(MenuView));

        public TimeSpan RiskRange
        {
            get { return (TimeSpan)GetValue(RiskRangeViewProperty); }
            set { SetValue(RiskRangeViewProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RiskRangeEditor.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RiskRangeViewProperty =
            DependencyProperty.Register("RiskRange", typeof(TimeSpan), typeof(MenuView));

        // Array of call statuses
        public string[] CallStatuses { get; set; } = { "Open", "Open in danger", "In treatment", "In treatment danger", "Closed", "Over dated" };

        private readonly AdminMainView _adminMainView;
        private readonly Call.CallInListView _callInListView;
        public MenuView(AdminMainView adminMainView, Call.CallInListView callInListView)
        {
            _adminMainView = adminMainView;
            _callInListView = callInListView;
            UpdateCallStatuses();
            InitializeComponent();
        }

        // Apply Button Click - Update the RiskRangeEditor property
        private void ApplyButton_Click(object sender, RoutedEventArgs e)
        {
            s_bl.Admin.UpdateRiskRange(TimeSpan.Parse(RiskRangeEdt.RiskRange));
            RiskRangeEdt.RiskRange = "0.00:00:00";
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            RiskRangeEdt.RiskRange = "0.00:00:00";
        }

        private void ClockObserver()
        {
            ConfigTime = s_bl.Admin.GetConfigClock();
        }

        private void RiskRangeObserver()
        {
            RiskRange = s_bl.Admin.GetRiskRange();
        }

        //private void Window_Closed(object sender, EventArgs e)
        //{
        //    s_bl.Admin.RemoveClockObserver(ClockObserver);
        //    s_bl.Admin.RemoveConfigObserver(RiskRangeObserver);
        //}
        private void Window_Loaded(object sender, EventArgs e)
        {
            ConfigTime = s_bl.Admin.GetConfigClock();
            RiskRange = s_bl.Admin.GetRiskRange();

            s_bl.Admin.AddClockObserver(ClockObserver);
            s_bl.Admin.AddConfigObserver(RiskRangeObserver);
            s_bl.Call.AddObserver(UpdateCallStatuses);
        }

        private void PlusOneSec(object sender, RoutedEventArgs e)
        {
            s_bl.Admin.ForwardClock(BO.TimeUnit.Seconds);
        }

        private void PlusOneMin(object sender, RoutedEventArgs e)
        {
            s_bl.Admin.ForwardClock(BO.TimeUnit.Minutes);
        }

        private void PlusOneHour(object sender, RoutedEventArgs e)
        {
            s_bl.Admin.ForwardClock(BO.TimeUnit.Hours);
        }

        private void PlusOneDay(object sender, RoutedEventArgs e)
        {
            s_bl.Admin.ForwardClock(BO.TimeUnit.Days);
        }

        private void PlusOneMonth(object sender, RoutedEventArgs e)
        {
            s_bl.Admin.ForwardClock(BO.TimeUnit.Months);
        }

        private void PlusOneYear(object sender, RoutedEventArgs e)
        {
            s_bl.Admin.ForwardClock(BO.TimeUnit.Years);
        }

        private void ResetDB(object sender, RoutedEventArgs e)
        {
            s_bl.Admin.ResetDB();
        }

        private void InitDB(object sender, RoutedEventArgs e)
        {
            s_bl.Admin.InitDB();
        }

        // Function to retrieve the number of calls of each status and add it to the corresponding string
        private void UpdateCallStatuses()
        {
            int[] callQuantities = s_bl.Call.GetCallsQuantities();
            for (int i = 0; i < CallStatuses.Length; i++)
            {
                CallStatuses[i] = $"{CallStatuses[i]}\n{callQuantities[i]}";
            }
        }

        private void Button_Status_Click(object sender, RoutedEventArgs e)
        {
            switch
                ((string)((Button)sender).CommandParameter)
            {
                case "0":
                    _callInListView.FilterValue = BO.BoCallStatus.Open;
                    break;
                case "1":
                    _callInListView.FilterValue = BO.BoCallStatus.OpenAndDanger;
                    break;
                case "2":
                    _callInListView.FilterValue = BO.BoCallStatus.InTreatment;
                    break;
                case "3":
                    _callInListView.FilterValue = BO.BoCallStatus.InTreatmentAndDanger;
                    break;
                case "4":
                    _callInListView.FilterValue = BO.BoCallStatus.Closed;
                    break;
                case "5":
                    _callInListView.FilterValue = BO.BoCallStatus.OverDated;
                    break;
            }
            _callInListView.StatusFilter();
            _adminMainView.CurrentView = _callInListView;
        }
    }
    public class RiskRangeEditor : INotifyPropertyChanged
    {
        private string _riskRange;

        public string RiskRange
        {
            get => _riskRange;
            set
            {
                string formattedValue = FormatRiskRange(value);
                if (_riskRange != formattedValue)
                {
                    _riskRange = formattedValue;
                    OnPropertyChanged();
                }
            }
        }

        public RiskRangeEditor()
        {
            _riskRange = "0.00:00:00"; // Default value
        }

        // Format the input string and enforce rules
        private string FormatRiskRange(string input)
        {
            if (string.IsNullOrEmpty(input))
                return "0.00:00:00";

            // Remove all non-numeric characters
            string cleanInput = Regex.Replace(input, "[^0-9]", "");

            // Ensure at least 7 digits for the format (pad with zeros if needed)
            cleanInput = cleanInput.PadLeft(7, '0');

            // Split the digits into days, hours, minutes, and seconds
            int days = int.Parse(cleanInput.Substring(0, 1));
            int hours = int.Parse(cleanInput.Substring(1, 2));
            int minutes = int.Parse(cleanInput.Substring(3, 2));
            int seconds = int.Parse(cleanInput.Substring(5, 2));

            // Apply constraints
            days = Math.Min(days, 30);
            hours = Math.Min(hours, 23);
            minutes = Math.Min(minutes, 59);
            seconds = Math.Min(seconds, 59);

            // Return formatted string
            return $"{days}.{hours:00}:{minutes:00}:{seconds:00}";
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

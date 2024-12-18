using System.CodeDom;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PL;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
    public RiskRangeViewModel riskRangeViewModel { get; set; } = new RiskRangeViewModel();
    public DateTime ConfigTime
    {
        get { return (DateTime)GetValue(ConfigTimeNowProperty); }
        set { SetValue(ConfigTimeNowProperty, value); }
    }

    // Using a DependencyProperty as the backing store for DateTimeNow.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty ConfigTimeNowProperty =
        DependencyProperty.Register("ConfigTime", typeof(DateTime), typeof(MainWindow));

    public TimeSpan RiskRangeView
    {
        get { return (TimeSpan)GetValue(RiskRangeViewProperty); }
        set { SetValue(RiskRangeViewProperty, value); }
    }

    // Using a DependencyProperty as the backing store for RiskRange.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty RiskRangeViewProperty =
        DependencyProperty.Register("RiskRangeView", typeof(TimeSpan), typeof(MainWindow));

    public MainWindow()
    {    
        InitializeComponent();
    }
    // Apply Button Click - Update the RiskRange property
    private void ApplyButton_Click(object sender, RoutedEventArgs e)
    {
        s_bl.Admin.UpdateRiskRange(TimeSpan.Parse(riskRangeViewModel.RiskRange));
    }

    private void ClockObserver()
    {
        ConfigTime = s_bl.Admin.GetConfigClock();
    }
    private void RiskRangeObserver()
    {
        RiskRangeView = s_bl.Admin.GetRiskRange();
    }
    private void WindowClosed(object sender, EventArgs e)
    {
        s_bl.Admin.RemoveClockObserver(ClockObserver);
        s_bl.Admin.RemoveConfigObserver(RiskRangeObserver);
    }
    private void WindowLoaded(object sender, EventArgs e)
    {
        ConfigTime = s_bl.Admin.GetConfigClock();
        RiskRangeView = s_bl.Admin.GetRiskRange();

        s_bl.Admin.AddClockObserver(ClockObserver);
        s_bl.Admin.AddConfigObserver(RiskRangeObserver);
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
    private void OpenVolunteerInListWindow(object sender, RoutedEventArgs e)
    {
        // Check if the window is already open
        foreach (Window window in Application.Current.Windows)
        {
            if (window is Volunteer.VolunteerInListWindow)
            {
                // Bring the existing window to the front
                window.Activate();
                return; // Exit the method
            }
        }
        new Volunteer.VolunteerInListWindow().Show();
    }
}
public class BoolToOpacityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return (value is bool boolValue && boolValue) ? 0 : 1;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

public class RiskRangeViewModel : INotifyPropertyChanged
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

    public RiskRangeViewModel()
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
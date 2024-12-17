using System.CodeDom;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Text;
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
    public DateTime ConfigTime
    {
        get { return (DateTime)GetValue(ConfigTimeNowProperty); }
        set { SetValue(ConfigTimeNowProperty, value); }
    }

    // Using a DependencyProperty as the backing store for DateTimeNow.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty ConfigTimeNowProperty =
        DependencyProperty.Register("ConfigTime", typeof(DateTime), typeof(MainWindow));

    public TimeSpan RiskRange
    {
        get { return (TimeSpan)GetValue(RiskRangeProperty); }
        set { SetValue(RiskRangeProperty, value); }
    }

    // Using a DependencyProperty as the backing store for RiskRange.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty RiskRangeProperty =
        DependencyProperty.Register("RiskRange", typeof(TimeSpan), typeof(MainWindow));

    public MainWindow()
    {    
        InitializeComponent();
    }

    private void ClockObserver()
    {
        ConfigTime = s_bl.Admin.GetConfigClock();
    }
    private void RiskRangeObserver()
    {
        RiskRange = s_bl.Admin.GetRiskRange();
    }
    private void WindowClosed(object sender, EventArgs e)
    {
        s_bl.Admin.RemoveClockObserver(ClockObserver);
        s_bl.Admin.RemoveConfigObserver(RiskRangeObserver);
    }
    private void WindowLoaded(object sender, EventArgs e)
    {
        ConfigTime = s_bl.Admin.GetConfigClock();
        RiskRange = s_bl.Admin.GetRiskRange();

        s_bl.Admin.AddClockObserver(ClockObserver);
        s_bl.Admin.AddConfigObserver(RiskRangeObserver);
    }
    private void PlusOneSec(object sender, RoutedEventArgs e)
    {
        s_bl.Admin.ForwardClock(1 , BO.TimeUnit.Seconds);
    }
    private void PlusOneMin(object sender, RoutedEventArgs e)
    {
        s_bl.Admin.ForwardClock(1, BO.TimeUnit.Minutes);
    }
    private void PlusOneHour(object sender, RoutedEventArgs e)
    {
        s_bl.Admin.ForwardClock(1, BO.TimeUnit.Hours);
    }
    private void PlusOneDay(object sender, RoutedEventArgs e)
    {
        s_bl.Admin.ForwardClock(1, BO.TimeUnit.Days);
    }
    private void PlusOneMonth(object sender, RoutedEventArgs e)
    {
        s_bl.Admin.ForwardClock(1, BO.TimeUnit.Months);
    }
    private void PlusOneYear(object sender, RoutedEventArgs e)
    {
        s_bl.Admin.ForwardClock(1, BO.TimeUnit.Years);
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
public static class WatermarkService
{
    // Define the Watermark attached property
    public static readonly DependencyProperty WatermarkProperty =
        DependencyProperty.RegisterAttached(
            "Watermark",
            typeof(string),
            typeof(WatermarkService),
            new PropertyMetadata(string.Empty, OnWatermarkChanged));

    // Getter and setter for the attached property
    public static void SetWatermark(UIElement element, string value)
    {
        element.SetValue(WatermarkProperty, value);
    }

    public static string GetWatermark(UIElement element)
    {
        return (string)element.GetValue(WatermarkProperty);
    }

    private static void OnWatermarkChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is TextBox textBox)
        {
            // Hook up focus events
            textBox.GotFocus += RemoveWatermark;
            textBox.LostFocus += ApplyWatermark;

            // Apply watermark initially
            ApplyWatermark(textBox, null);
        }
    }

    private static void RemoveWatermark(object sender, RoutedEventArgs e)
    {
        if (sender is TextBox textBox && textBox.Text == GetWatermark(textBox))
        {
            textBox.Text = "";
        }
    }

    private static void ApplyWatermark(object sender, RoutedEventArgs e)
    {
        if (sender is TextBox textBox && string.IsNullOrWhiteSpace(textBox.Text))
        {
            textBox.Text = GetWatermark(textBox);
        }
    }
}
public class BoolToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return (value is bool boolValue && boolValue) ? Visibility.Collapsed : Visibility.Visible;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
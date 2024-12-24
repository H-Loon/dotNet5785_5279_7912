using System.Windows;

namespace PL;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    static readonly BlApi.IBl s_bl = BlApi.Factory.Get();

    public Volunteer.AdminView AdminV { get; set; } = new Volunteer.AdminView();
    public Volunteer.VolunteerInListVM VolunteerInListV { get; set; } = new Volunteer.VolunteerInListVM();



    public object CurrentView
    {
        get { return (object)GetValue(CurrentViewProperty); }
        set { SetValue(CurrentViewProperty, value); }
    }

    // Using a DependencyProperty as the backing store for CurrentView.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty CurrentViewProperty =
        DependencyProperty.Register("CurrentView", typeof(object), typeof(MainWindow), new PropertyMetadata(null));


    public MainWindow()
    {
        CurrentView = AdminV;
        InitializeComponent();
    }

    private void Window_Loaded(object sender, RoutedEventArgs e)
            => s_bl.Volunteer.AddObserver(VolunteerInListV.VolunteerListObserver);

    private void Window_Closed(object sender, EventArgs e)
            => s_bl.Volunteer.RemoveObserver(VolunteerInListV.VolunteerListObserver);
    private void CurrentViewToVolunteerInList(object sender, RoutedEventArgs e)
    {
        CurrentView = VolunteerInListV;
    }

    private void CurrentViewToAdmin(object sender, RoutedEventArgs e)
    {
        CurrentView = AdminV;
    }

}
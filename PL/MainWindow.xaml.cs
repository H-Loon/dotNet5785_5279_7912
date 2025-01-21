using PL.Admin.Volunteer;
using System.CodeDom;
using System.Windows;
using System.Windows.Controls;

namespace PL;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
    int count = 0;
    private List<TabItem> _tabItems;
    private TabItem _tabAdd;
    public bool isAdminConnected = false;
    public MainWindow()
    {
        InitializeComponent();
        // initialize tabItem array
        _tabItems = new List<TabItem>();

        // add a tabItem with + in header 
        _tabAdd = new TabItem();
        _tabAdd.Header = "+";

        _tabItems.Add(_tabAdd);

        // add first tab
        this.AddTabItem();

        // bind tab control
        tabDynamic.DataContext = _tabItems;

        tabDynamic.SelectedIndex = 0;
    }
    private void tabDynamic_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        TabItem tab = tabDynamic.SelectedItem as TabItem;
        
        if (tab != null && tab.Header != null)
        {
            if (tab.Header.Equals(_tabAdd.Header))
            {
                // clear tab control binding
                tabDynamic.DataContext = null;

                // add new tab
                TabItem newTab = this.AddTabItem();

                // bind tab control
                tabDynamic.DataContext = _tabItems;

                
                // select newly added tab item
                tabDynamic.SelectedItem = newTab;
            }
            else
            {
                
                // your code here...
            }
        }
    }
    private TabItem AddTabItem()
    {
        // create new tab item
        TabItem tab = new TabItem();
        tab.Header = "Log In";
        tab.Name = $"tab{count}";
        tab.HeaderTemplate = tabDynamic.FindResource("TabHeader") as DataTemplate;

        // add controls to tab item, this case I added just a text box
        UserControl view = new LogIn();
        view.Name = "view";
        tab.Content = view;

        // insert tab item right before the last (+) tab item
        _tabItems.Insert(_tabItems.Count - 1, tab);
        count++;
        return tab;
    }
    private void btnDelete_Click(object sender, RoutedEventArgs e)
    {
        string tabName = (sender as Button).CommandParameter.ToString();

        var item = tabDynamic.Items.Cast<TabItem>().Where
                   (i => i.Name.Equals(tabName)).SingleOrDefault();

        TabItem tab = item;

        if (tab != null)
        {
            if (_tabItems.Count < 3 && MessageBox.Show(string.Format
                                            ("Are you sure you want to remove the tab '{0}'? \nIt will close the app.", tab.Header.ToString()),
                                            "Remove Tab", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                                            {
                                                Close();
                                            }

            // get selected tab
            TabItem selectedTab = tabDynamic.SelectedItem as TabItem;

            // clear tab control binding
            tabDynamic.DataContext = null;


            _tabItems.Remove(tab);
            if (tab.Content.GetType() == typeof(Admin.AdminMainView))
                isAdminConnected = false;

            // bind tab control
            tabDynamic.DataContext = _tabItems;


            // select previously selected tab. if that is removed then select first tab
            if (selectedTab == null || selectedTab.Equals(tab))
            {
                selectedTab = _tabItems[0];
            }
            tabDynamic.SelectedItem = selectedTab;
        }
    }

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        VolunteerInListView volunteerInListView = new VolunteerInListView();
        Admin.Call.CallInListView callInListView = new Admin.Call.CallInListView();

        s_bl.Volunteer.AddObserver(volunteerInListView.VolunteerListObserver);
        s_bl.Call.AddObserver(callInListView.CallListObserver);
    }
}
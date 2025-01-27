using PL.Admin.Call;
using PL.Admin.Menu;
using PL.Admin.Volunteer;
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
        private int _id;
        public bool IsCallSelected = false; 

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
            _id = id;
            _menuView = menuView;
            InitializeComponent();
        }
        

        public BO.OpenCallInListField OpenCallInListFieldSorted { get; set; } = BO.OpenCallInListField.Id;
        public BO.BoCallType CallTypeFiltered { get; set; }
        public IEnumerable<BO.OpenCallInList> OpenCallInList
        {
            get { return (IEnumerable<BO.OpenCallInList>)GetValue(CallInListProperty); }
            set { SetValue(CallInListProperty, value); }
        }

        // Using a DependencyProperty as the backing store for OpenCallInList.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CallInListProperty =
            DependencyProperty.Register("OpenCallInList", typeof(IEnumerable<BO.OpenCallInList>), typeof(OpenCallsInListView));

        public void OpenCallListObserver()
        {    
            OpenCallInList = s_bl.Call.GetOpenCallForVolunteer(_id, CallTypeFiltered, OpenCallInListFieldSorted);
        }

        private void AcceptCall(object sender, RoutedEventArgs e)
        {
            _menuView.CallVisibility = Visibility.Visible;
            _menuView.Call = s_bl.Call.GetCall(_selectedCall!.Id);
        }
    }

    internal class OpenCallInListField : IEnumerable
    {
        static readonly IEnumerable<BO.OpenCallInListField> s_enums =
        (Enum.GetValues(typeof(BO.OpenCallInListField)) as IEnumerable<BO.OpenCallInListField>)!;

        public IEnumerator GetEnumerator() => s_enums.GetEnumerator();
    }

    internal class CallType : IEnumerable
    {
        static readonly IEnumerable<BO.BoCallType> s_enums =
        (Enum.GetValues(typeof(BO.BoCallType)) as IEnumerable<BO.BoCallType>)!;
        public IEnumerator GetEnumerator() => s_enums.GetEnumerator();
    }
}

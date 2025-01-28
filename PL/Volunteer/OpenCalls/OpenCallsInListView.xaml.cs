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
        private int _Id;


        public bool IsCallSelected
        {
            get { return (bool)GetValue(IsCallSelectedProperty); }
            set { SetValue(IsCallSelectedProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsCallSelected.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsCallSelectedProperty =
            DependencyProperty.Register("IsCallSelected", typeof(bool), typeof(OpenCallsInListView), new PropertyMetadata(false));



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
                else
                {
                    IsCallSelected = false;
                }
            } 
                
        }
        

        public BO.OpenCallInListField OpenCallInListFieldSorted { get; set; } = BO.OpenCallInListField.Id;
        public BO.BoCallType CallTypeFiltered { get; set; } = BO.BoCallType.None;
        public OpenCallsInListView(Menu.MenuView menuView, int id)
        {
            _Id = id;
            _menuView = menuView;
            OpenCallListObserver();
            InitializeComponent();
        }
        public IEnumerable<BO.OpenCallInList> OpenCallsInList
        {
            get { return (IEnumerable<BO.OpenCallInList>)GetValue(CallInListProperty); }
            set { SetValue(CallInListProperty, value); }
        }

        // Using a DependencyProperty as the backing store for OpenCallsInList.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CallInListProperty =
            DependencyProperty.Register("OpenCallsInList", typeof(IEnumerable<BO.OpenCallInList>), typeof(OpenCallsInListView));

        public void Sort_Filtre_List(object sender, RoutedEventArgs e)
        {
            OpenCallsInList = s_bl.Call.GetOpenCallForVolunteer(_Id, CallTypeFiltered == BO.BoCallType.None ? null : CallTypeFiltered, OpenCallInListFieldSorted);
        }
        public void OpenCallListObserver()
        {    
            OpenCallsInList = s_bl.Call.GetOpenCallForVolunteer(_Id, CallTypeFiltered == BO.BoCallType.None ? null : CallTypeFiltered, OpenCallInListFieldSorted);
        }

        private void AcceptCall(object sender, RoutedEventArgs e)
        {
            int callId = _selectedCall!.Id;
            if (MessageBox.Show($"Are you sure you want to accept this call, ID: {callId}?", "Accept Call", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                s_bl.Call.AssignCall(_Id, callId);
                _menuView.Call = s_bl.Call.GetCall(callId);
                _menuView.volunteerMainView.IsNotCall = false;
                _menuView.CallIdLabel = "ID: " + callId;
                _menuView.AssignList = _menuView.ListToStr();
                _menuView.volunteerMainView.CurrentView = _menuView;
                _menuView.CallVisibility = Visibility.Visible;
            }
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

using BO;
using PL.Admin.Volunteer;
using System;
using System.Collections;
using System.Collections.Generic;
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

namespace PL.Volunteer.History
{
    /// <summary>
    /// Interaction logic for HistoryView.xaml
    /// </summary>
    public partial class HistoryView : UserControl
    {
        private static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
        private int _Id;
        public BoCallType TypeCall { get; set; } = BoCallType.None;
        public BO.ClosedCallInListField TypeClosed { get; set; } = BO.ClosedCallInListField.Id;
        public HistoryView(int id)
        {
            _Id = id;
            InitializeComponent();
        }
       
        public IEnumerable<BO.ClosedCallInList> CallClosedInList
        {
            get { return (IEnumerable<BO.ClosedCallInList>)GetValue(CallClosedInListProperty); }
            set { SetValue(CallClosedInListProperty, value); }
        }
        public static readonly DependencyProperty CallClosedInListProperty =
            DependencyProperty.Register("CallClosedInList", typeof(IEnumerable<BO.ClosedCallInList>), typeof(HistoryView));

        //private void QueryCallList(object sender, SelectionChangedEventArgs e)
        //{
        //    int volunteerId = Id; // Assuming Id is the ID of the connected volunteer
        //    switch (TypeCall)
        //    {
        //        case BoCallType.HomeBotIssue:
        //            CallClosedInList = s_bl.Call.GetClosedCallByVolunteer(volunteerId, BoCallType.HomeBotIssue, null);
        //            break;
        //        case BoCallType.TeleporterBlockedOnMachonLev:
        //            CallClosedInList = s_bl.Call.GetClosedCallByVolunteer(volunteerId, BoCallType.TeleporterBlockedOnMachonLev, null);
        //            break;
        //        case BoCallType.MyDishwasherIsInDepression:
        //            CallClosedInList = s_bl.Call.GetClosedCallByVolunteer(volunteerId, BoCallType.MyDishwasherIsInDepression, null);
        //            break;
        //        case BoCallType.MyTimeTravelMachineIsLazy:
        //            CallClosedInList = s_bl.Call.GetClosedCallByVolunteer(volunteerId, BoCallType.MyTimeTravelMachineIsLazy, null);
        //            break;
        //        case BoCallType.Other:
        //            CallClosedInList = s_bl.Call.GetClosedCallByVolunteer(volunteerId, BoCallType.Other, null);
        //            break;
        //        case BoCallType.None:
        //            CallClosedInList = s_bl.Call.GetClosedCallByVolunteer(volunteerId, BoCallType.None, null);
        //            break;
        //    }
        //}
        //private void ApplyOrderBy(object sender, SelectionChangedEventArgs e)
        //{
        //    int volunteerId = Id;
        //    // Trie la liste des appels fermés en fonction du champ sélectionné
        //    CallClosedInList = s_bl.Call.GetClosedCallByVolunteer( volunteerId,null, TypeClosed );
        //}

        private void QueryAndOrderCallList(object sender, RoutedEventArgs e)
        {
            CallClosedInList = s_bl.Call.GetClosedCallByVolunteer(
                _Id,
                TypeCall == BoCallType.None ? null : (BO.BoCallType?)TypeCall, 
                TypeClosed 
            );
        }
        public void ClosedListObserver()
        {
            CallClosedInList = s_bl.Call.GetClosedCallByVolunteer(
                _Id,
                TypeCall == BoCallType.None ? null : (BO.BoCallType?)TypeCall,
                TypeClosed
            );
        }

    }
    internal class ClosedCallInListField : IEnumerable
    {
        static readonly IEnumerable<BO.ClosedCallInListField> s_enums =
        (Enum.GetValues(typeof(BO.ClosedCallInListField)) as IEnumerable<BO.ClosedCallInListField>)!;

        public IEnumerator GetEnumerator() => s_enums.GetEnumerator();
    }
    internal class CallType : IEnumerable
    {
        static readonly IEnumerable<BO.BoCallType> s_enums =
        (Enum.GetValues(typeof(BO.BoCallType)) as IEnumerable<BO.BoCallType>)!;

        public IEnumerator GetEnumerator() => s_enums.GetEnumerator();
    }

    
}

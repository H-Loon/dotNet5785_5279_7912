using PL.Admin.Call;
using System;
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
using System.Windows.Threading;

namespace PL.Admin.Call
{
    /// <summary>
    /// Logique d'interaction pour CallInfoView.xaml
    /// </summary>
    public partial class CallInfoView : UserControl
    {
        private static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
        public string IdLabel { get; set; } = "ID: ";
        public string AssignList { get; set; }
        private int _id;
        public BO.Call Call
        {
            get { return (BO.Call)GetValue(CallProperty); }
            set { SetValue(CallProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Call.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CallProperty =
            DependencyProperty.Register("CallC", typeof(BO.Call), typeof(CallInfoView), new PropertyMetadata(null));

        private string ListToStr()
        {
            string result = "";
            foreach (var item in Call.AssignInList)
            {
                result += $"{item}\n";
            }
            return result;
        }

        public CallInfoView(int id)
        {
            _id = id;
            IdLabel += id;
            FetchCallInfo();
            s_bl.Call.AddObserver(FetchCallInfo);
            InitializeComponent();
        }
        private volatile DispatcherOperation? _observerOperation = null; //stage 7
        public void FetchCallInfo()
        {
            if (_observerOperation is null || _observerOperation.Status == DispatcherOperationStatus.Completed)
                _observerOperation = Dispatcher.BeginInvoke(() =>
                {
                    Call = s_bl.Call.GetCall(_id);
                    if (Call.AssignInList != null)
                        AssignList = ListToStr();
                    else
                        AssignList = "";
                });
        }
    }
}

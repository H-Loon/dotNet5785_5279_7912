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

namespace PL.Admin.Call
{
    /// <summary>
    /// Logique d'interaction pour CallInfoView.xaml
    /// </summary>
    public partial class CallInfoView : UserControl
    {
        private static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
        public string ButtonText { get; set; }
        public string IdLabel { get; set; } = "ID: ";
        public BO.BoRoleType Status { get; set; }
        public BO.BoDistanceType CallType { get; set; }
        public string AssignList { get; set; }

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
            Call = s_bl.Call.GetCall(id);
            Status = (BO.BoRoleType)Call.Status;
            CallType = (BO.BoDistanceType)Call.CallType;
            IdLabel += id;
            if (Call.AssignInList != null)
                AssignList = ListToStr();
            else
                AssignList = "";

            InitializeComponent();
        }
    }
}

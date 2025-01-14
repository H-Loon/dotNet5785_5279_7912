using DO;
using PL.Admin.Volunteer;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
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
using System.Windows.Shapes;

namespace PL.Admin.Call
{
    /// <summary>
    /// Logique d'interaction pour AddUpdateCallWindow.xaml
    /// </summary>
    public partial class AddUpdateCallWindow : Window
    {
        private static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
        public string ButtonText { get; set; }
        public string IdLabel { get; set; } = "ID: ";
        public BO.BoCallType CallType { get; set; }

        public BO.Call Call
        {
            get { return (BO.Call)GetValue(CallProperty); }
            set { SetValue(CallProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Volunteer.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CallProperty =
            DependencyProperty.Register("CallW", typeof(BO.Volunteer), typeof(AddUpdateVolunteerWindow), new PropertyMetadata(null));


        public AddUpdateCallWindow(int id = 0)
        {
            IdLabel += id.ToString();
            ButtonText = id == 0 ? "Add" : "Update";
            if (id == 0) Call = new BO.Call()
            {
                Id = 0,
                Description = "",
                Address = "",
                CallType = BO.BoCallType.Other    
            };
            else Call = s_bl.Call.GetCall(id);
            InitializeComponent();
        }

        void btnAddUpdate_Click(object sender, RoutedEventArgs e)
        {
            bool flag = true;

            try
            {
                if (ButtonText == "Add") s_bl.Call.AddCall(Call);
                else s_bl.Call.UpdateCall(Call);
                flag = false;
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }

            if (flag == false)
                Close();
        }

    }
    internal class CallType : IEnumerable
    {
        static readonly IEnumerable<BO.BoCallType> s_enums =
        (Enum.GetValues(typeof(BO.BoCallType)) as IEnumerable<BO.BoCallType>)!;

        public IEnumerator GetEnumerator() => s_enums.GetEnumerator();
    }
    internal class Status : IEnumerable
    {
        static readonly IEnumerable<BO.BoCallStatus> s_enums =
        (Enum.GetValues(typeof(BO.BoCallStatus)) as IEnumerable<BO.BoCallStatus>)!;

        public IEnumerator GetEnumerator() => s_enums.GetEnumerator();
    }
    public class ConvertUpdateToTrueKey : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((string)value == "Add") ? false : true;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    public class ConvertUpdateToVisibleKey : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((string)value == "Add") ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

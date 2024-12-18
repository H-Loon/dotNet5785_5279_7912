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
using System.Windows.Shapes;

namespace PL.Volunteer
{
    /// <summary>
    /// Interaction logic for VolunteerInListWindow.xaml
    /// </summary>
    public partial class VolunteerInListWindow : Window
    {
        private static readonly BlApi.IBl s_bl = BlApi.Factory.Get();


        public IEnumerable<BO.VolunteerInList> VolunteerInList
        {
            get { return (IEnumerable<BO.VolunteerInList>)GetValue(VolunteerInListProperty); }
            set { SetValue(VolunteerInListProperty, value); }
        }

        // Using a DependencyProperty as the backing store for VolunteerInList.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty VolunteerInListProperty =
            DependencyProperty.Register("VolunteerInList", typeof(IEnumerable<BO.VolunteerInList>), typeof(VolunteerInListWindow));


        public BO.VolunteerInListField Field { get; set; } = BO.VolunteerInListField.Id;

        private void ComboBox_SelectionChanged(object sender, RoutedEventArgs e)
        {
            VolunteerInList = s_bl.Volunteer.GetVolunteerInList(null, Field);
        }

        public VolunteerInListWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }
    }
    internal class VolunteerField : IEnumerable
    {
        static readonly IEnumerable<BO.VolunteerInListField> s_enums =
        (Enum.GetValues(typeof(BO.VolunteerInListField)) as IEnumerable<BO.VolunteerInListField>)!;

        public IEnumerator GetEnumerator() => s_enums.GetEnumerator();
    }

}

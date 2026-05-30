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

namespace QLKS.Views
{
    /// <summary>
    /// Interaction logic for PrintMainView.xaml
    /// </summary>
    public partial class PrintMainView : UserControl
    {
        public PrintMainView()
        {
            InitializeComponent();
        }
        private void PrintGuest_Click(object sender, RoutedEventArgs e)
        {
            if (GuestReport != null)
            {
                GuestReport.ExecutePrint();
            }
        }

        private void PrintEmployee_Click(object sender, RoutedEventArgs e)
        {
            if (EmployeeReport != null)
            {
                EmployeeReport.ExecutePrint();
            }
        }
    }
}

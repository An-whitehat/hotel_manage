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
    /// Interaction logic for PrintGuestListView.xaml
    /// </summary>
    public partial class PrintGuestListView : UserControl
    {
        public PrintGuestListView()
        {
            InitializeComponent();
            this.DataContext = new QLKS.ViewModels.PrintGuestViewModel();
        }
        public void ExecutePrint()
        {
            PrintDialog printDialog = new PrintDialog();

            if (printDialog.ShowDialog() == true)
            {
                printDialog.PrintVisual(PrintArea, "DanhSachKhachLuuTru");
            }
        }
    }
}

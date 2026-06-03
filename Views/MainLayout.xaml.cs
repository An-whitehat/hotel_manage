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
using System.Windows.Shapes;

namespace QLKS.Views
{
    public partial class MainLayout : Window
    {
        public MainLayout()
        {
            InitializeComponent();
        }
        private void btn_OpenReportKhachHang_Click(object sender, RoutedEventArgs e)
        {
            ReportKhachHangWindow rptWindow = new ReportKhachHangWindow();
            rptWindow.Owner = Window.GetWindow(this);
            rptWindow.ShowDialog();
        }

        private void btn_OpenReportPhong_Click(object sender, RoutedEventArgs e)
        {
            Views.ReportPhongWindow rp = new Views.ReportPhongWindow();
            rp.Owner = Window.GetWindow(this);
            rp.ShowDialog();
        }
    }
}

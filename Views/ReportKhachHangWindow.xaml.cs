using CrystalDecisions.ReportAppServer;
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
    /// <summary>
    /// Interaction logic for ReportKhachHangWindow.xaml
    /// </summary>
    public partial class ReportKhachHangWindow : Window
    {
        public ReportKhachHangWindow()
        {
            InitializeComponent();
        }
        private void btn_showreport_Click(object sender, RoutedEventArgs e)
        {
            // Lấy cái report m vừa thiết kế ra
            Report.DanhSachKhachHang rpt = new Report.DanhSachKhachHang();

            // Dòng tự động đăng nhập (m nhớ thay "sa", "123", tên Server và CSDL của máy m vào nha)
            rpt.SetDatabaseLogon("sa", "123", "TÊN_SERVER", "TÊN_CSDL");

            // Nhét đĩa vào Tivi để chiếu lên
            report1.ViewerCore.ReportSource = rpt;
        }
    }
}

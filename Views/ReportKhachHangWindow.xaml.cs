using CrystalDecisions.CrystalReports.Engine;
using QLKS.Report;
using System;
using System.Configuration;
using System.Windows;

namespace QLKS.Views
{
    /// <summary>
    /// Crystal Reports viewer cho Danh sách Khách hàng
    /// </summary>
    public partial class ReportKhachHangWindow : Window
    {
        public ReportKhachHangWindow()
        {
            InitializeComponent();
        }

        private void btn_showreport_Click(object sender, RoutedEventArgs e)
        {
            ReportDocument rpt = new Report.DanhSachKhachHang();

            // Dòng đăng nhập của m
            rpt.SetDatabaseLogon("sa", "123", "TÊN_SERVER", "TÊN_CSDL");

            // NÉM THÊM DÒNG NÀY VÀO ĐỂ ÉP NÓ XÓA CACHE CŨ, KÉO DATA MỚI TỪ SQL
            rpt.Refresh();

            report1.ViewerCore.ReportSource = rpt;
        }
    }
}

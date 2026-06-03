using CrystalDecisions.CrystalReports.Engine;
using QLKS.Report;
using System;
using System.Configuration;
using System.Windows;

namespace QLKS.Views
{
    public partial class ReportKhachHangWindow : Window
    {
        public ReportKhachHangWindow()
        {
            InitializeComponent();
        }

        private void btn_showreport_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var connStr = ConfigurationManager.ConnectionStrings["HotelManagementDB"]?.ConnectionString
                              ?? "Data Source=.;Initial Catalog=HotelManagement;Integrated Security=True";

                var builder  = new System.Data.SqlClient.SqlConnectionStringBuilder(connStr);
                string server   = builder.DataSource;
                string database = builder.InitialCatalog;

                ReportDocument rpt = new DanhSachKhachHang();
                rpt.SetDatabaseLogon("", "", server, database);
                rpt.Refresh();

                report1.ViewerCore.ReportSource = rpt;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Không thể tải báo cáo:\n" + ex.Message,
                    "Lỗi Crystal Reports", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}

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
            try
            {
                // Đọc server và database từ App.config — không hardcode
                var connStr = ConfigurationManager.ConnectionStrings["HotelManagementDB"]?.ConnectionString
                              ?? "Data Source=.;Initial Catalog=HotelManagement;Integrated Security=True";

                var builder  = new System.Data.SqlClient.SqlConnectionStringBuilder(connStr);
                string server   = builder.DataSource;
                string database = builder.InitialCatalog;

                var rpt = new DanhSachKhachHang();

                // Integrated Security → truyền empty string cho user/pass
                // Nếu máy dùng SQL Auth thì đổi lại: rpt.SetDatabaseLogon("sa", "matkhau", server, database)
                rpt.SetDatabaseLogon("", "", server, database);

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

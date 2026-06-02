using QLKS.Models;
using QLKS.Report;
using System;
using System.Configuration;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace QLKS.Views
{
    /// <summary>
    /// Tab 1: WPF print preview (giữ nguyên code team)
    /// Tab 2: Crystal Reports viewer (HoaDonThanhToan.rpt)
    /// </summary>
    public partial class InvoiceReportWindow : Window
    {
        private readonly int _maHoaDon;

        public InvoiceReportWindow(int maHoaDon)
        {
            InitializeComponent();
            _maHoaDon = maHoaDon;

            // ── Tab 1: WPF preview của team — giữ nguyên 100% ────────────────
            BuildWpfPreview(maHoaDon);
        }

        // ════════════════════════════════════════════════════════════════════
        //  TAB 1 — WPF preview (code team, không thay đổi logic)
        // ════════════════════════════════════════════════════════════════════
        private void BuildWpfPreview(int maHoaDon)
        {
            using (var db = new HotelManagementEntities())
            {
                var data = (from hd in db.HoaDons
                            join dp in db.DatPhongs on hd.MaDatPhong equals dp.MaDatPhong into dpGroup
                            from dp in dpGroup.DefaultIfEmpty()
                            join kh in db.KhachHangs on dp.MaKH equals kh.MaKH into khGroup
                            from kh in khGroup.DefaultIfEmpty()
                            join nv in db.NhanViens on hd.MaNV equals nv.MaNV into nvGroup
                            from nv in nvGroup.DefaultIfEmpty()
                            join p in db.Phongs on dp.MaPhong equals p.MaPhong into pGroup
                            from p in pGroup.DefaultIfEmpty()
                            where hd.MaHoaDon == maHoaDon
                            select new
                            {
                                hd.MaHoaDon,
                                hd.NgayLap,
                                HoTenKhachHang = kh != null ? kh.HoTen : "N/A",
                                TenNhanVien    = nv != null ? nv.HoTen : "N/A",
                                SoPhong        = p  != null ? p.SoPhong : "N/A",
                                hd.TongTien,
                                hd.LoaiHoaDon,
                                hd.GhiChu
                            }).FirstOrDefault();

                var chiTiet = (from ct in db.ChiTietHoaDons
                               join dv in db.DichVus on ct.MaDichVu equals dv.MaDichVu
                               where ct.MaHoaDon == maHoaDon
                               select new
                               {
                                   dv.TenDichVu,
                                   ct.SoLuong,
                                   ct.DonGia,
                                   ct.ThanhTien
                               }).ToList();

                if (data == null)
                {
                    MessageBox.Show("Không tìm thấy hóa đơn!", "Lỗi",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    Close();
                    return;
                }

                // Build text preview — giữ nguyên format của team
                var preview = new TextBlock
                {
                    FontFamily = new FontFamily("Consolas"),
                    FontSize   = 14,
                    Padding    = new Thickness(40),
                    TextWrapping = TextWrapping.Wrap,
                    Background = Brushes.White
                };

                var sb = new System.Text.StringBuilder();
                sb.AppendLine("═══════════════════════════════════════════════════════");
                sb.AppendLine("           HAPPY HOTEL");
                sb.AppendLine("              HÓA ĐƠN THANH TOÁN");
                sb.AppendLine("═══════════════════════════════════════════════════════");
                sb.AppendLine($"Mã hóa đơn:      {data.MaHoaDon}");
                sb.AppendLine($"Ngày lập:        {data.NgayLap:dd/MM/yyyy HH:mm}");
                sb.AppendLine($"Loại HĐ:         {data.LoaiHoaDon}");
                sb.AppendLine($"Khách hàng:      {data.HoTenKhachHang}");
                sb.AppendLine($"Nhân viên:       {data.TenNhanVien}");
                sb.AppendLine($"Phòng:           {data.SoPhong}");
                sb.AppendLine("───────────────────────────────────────────────────────");
                sb.AppendLine("CHI TIẾT DỊCH VỤ:");
                sb.AppendLine();

                foreach (var item in chiTiet)
                {
                    sb.AppendLine($"  • {item.TenDichVu}");
                    sb.AppendLine($"    SL: {item.SoLuong}  |  Đơn giá: {item.DonGia:#,##0} VNĐ  |  Thành tiền: {item.ThanhTien:#,##0} VNĐ");
                    sb.AppendLine();
                }

                sb.AppendLine("───────────────────────────────────────────────────────");
                sb.AppendLine($"TỔNG TIỀN:       {data.TongTien:#,##0} VNĐ");
                sb.AppendLine("═══════════════════════════════════════════════════════");
                if (!string.IsNullOrEmpty(data.GhiChu))
                {
                    sb.AppendLine($"Ghi chú: {data.GhiChu}");
                    sb.AppendLine();
                }
                sb.AppendLine("          Cảm ơn quý khách! Hẹn gặp lại!");

                preview.Text = sb.ToString();

                var scrollViewer = new ScrollViewer
                {
                    Content = preview,
                    VerticalScrollBarVisibility   = ScrollBarVisibility.Auto,
                    HorizontalScrollBarVisibility = ScrollBarVisibility.Auto
                };

                var printButton = new Button
                {
                    Content             = "🖨 In hóa đơn",
                    FontSize            = 16,
                    Padding             = new Thickness(20, 10, 20, 10),
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Margin              = new Thickness(10),
                    Background          = new SolidColorBrush(Color.FromRgb(46, 204, 113)),
                    Foreground          = Brushes.White
                };
                printButton.Click += (s, e) => PrintDocument(preview);

                var dockPanel = new DockPanel();
                DockPanel.SetDock(printButton, Dock.Bottom);
                dockPanel.Children.Add(printButton);
                dockPanel.Children.Add(scrollViewer);

                // Đặt vào ContentPresenter của Tab 1
                WpfPreviewHost.Content = dockPanel;
            }
        }

        // ════════════════════════════════════════════════════════════════════
        //  TAB 2 — Crystal Reports
        // ════════════════════════════════════════════════════════════════════
        private void BtnLoadCrystal_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                TxtCrystalStatus.Text = "Đang tải...";

                // Đọc connection string từ App.config (không hardcode)
                var connStr = ConfigurationManager.ConnectionStrings["HotelManagementDB"]?.ConnectionString
                              ?? "Data Source=.;Initial Catalog=HotelManagement;Integrated Security=True";

                // Parse server và database từ connection string
                var builder = new System.Data.SqlClient.SqlConnectionStringBuilder(connStr);
                string server   = builder.DataSource;
                string database = builder.InitialCatalog;

                var rpt = new HoaDonThanhToan();

                // Dùng Integrated Security → không cần user/pass
                // Nếu SQL Auth thì thay bằng: rpt.SetDatabaseLogon("user", "pass", server, database)
                rpt.SetDatabaseLogon("", "", server, database);

                // Lọc đúng hóa đơn theo MaHoaDon
                rpt.RecordSelectionFormula =
                    $"{{HoaDon.MaHoaDon}} = {_maHoaDon}";

                CrystalViewer.ViewerCore.ReportSource = rpt;
                TxtCrystalStatus.Text = $"Đã tải hóa đơn #{_maHoaDon}";
            }
            catch (Exception ex)
            {
                TxtCrystalStatus.Text = "Lỗi: " + ex.Message;
                MessageBox.Show("Không thể tải Crystal Report:\n" + ex.Message,
                    "Lỗi Crystal Reports", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // ── In WPF preview ────────────────────────────────────────────────
        private void PrintDocument(FrameworkElement element)
        {
            try
            {
                var printDialog = new PrintDialog();
                if (printDialog.ShowDialog() == true)
                {
                    printDialog.PrintVisual(element, "Hóa đơn thanh toán");
                    MessageBox.Show("In thành công!", "Thông báo",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi in: {ex.Message}", "Lỗi",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}

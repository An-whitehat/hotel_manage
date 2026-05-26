using QLKS.Commands;
using QLKS.Services;
using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;

namespace QLKS.ViewModels
{
    // DTO cho từng dòng báo cáo
    public class BaoCaoLoaiPhongDto
    {
        public string TenLoaiPhong { get; set; }
        public int SoPhong { get; set; }
        public int SoLuotDat { get; set; }
        public int TongNgayThue { get; set; }
        public decimal DoanhThu { get; set; }
        public decimal TyLe { get; set; } // % trên tổng doanh thu
    }

    public class BaoCaoViewModel : BaseViewModel
    {
        // ── Danh sách kết quả ────────────────────────────────────────────────
        public ObservableCollection<BaoCaoLoaiPhongDto> DanhSachBaoCao { get; }
            = new ObservableCollection<BaoCaoLoaiPhongDto>();

        // ── Bộ lọc thời gian ─────────────────────────────────────────────────
        private DateTime _tuNgay = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
        public DateTime TuNgay
        {
            get => _tuNgay;
            set { SetProperty(ref _tuNgay, value); }
        }

        private DateTime _denNgay = DateTime.Today;
        public DateTime DenNgay
        {
            get => _denNgay;
            set { SetProperty(ref _denNgay, value); }
        }

        // ── Tổng hợp hiển thị ở footer ───────────────────────────────────────
        private decimal _tongDoanhThu;
        public decimal TongDoanhThu
        {
            get => _tongDoanhThu;
            set { SetProperty(ref _tongDoanhThu, value); }
        }

        private int _tongLuotDat;
        public int TongLuotDat
        {
            get => _tongLuotDat;
            set { SetProperty(ref _tongLuotDat, value); }
        }

        private string _trangThaiBaoCao = "Chọn khoảng thời gian và nhấn Xem báo cáo";
        public string TrangThaiBaoCao
        {
            get => _trangThaiBaoCao;
            set { SetProperty(ref _trangThaiBaoCao, value); }
        }

        // ── Commands ──────────────────────────────────────────────────────────
        public ICommand XemBaoCaoCommand { get; }
        public ICommand XuatExcelCommand { get; }
        public ICommand InBaoCaoCommand { get; }
        public ICommand ThangNayCommand { get; }
        public ICommand QuyNayCommand { get; }
        public ICommand NamNayCommand { get; }

        public BaoCaoViewModel()
        {
            XemBaoCaoCommand = new RelayCommand(_ => LoadBaoCao());
            XuatExcelCommand = new RelayCommand(_ => XuatExcel(), _ => DanhSachBaoCao.Count > 0);
            InBaoCaoCommand = new RelayCommand(_ => InBaoCao(), _ => DanhSachBaoCao.Count > 0);
            ThangNayCommand = new RelayCommand(_ => SetThangNay());
            QuyNayCommand = new RelayCommand(_ => SetQuyNay());
            NamNayCommand = new RelayCommand(_ => SetNamNay());

            // Load ngay khi mở
            LoadBaoCao();
        }

        // ── Phím tắt lọc nhanh ───────────────────────────────────────────────
        private void SetThangNay()
        {
            TuNgay = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            DenNgay = DateTime.Today;
            LoadBaoCao();
        }

        private void SetQuyNay()
        {
            int quy = (DateTime.Today.Month - 1) / 3;
            TuNgay = new DateTime(DateTime.Today.Year, quy * 3 + 1, 1);
            DenNgay = DateTime.Today;
            LoadBaoCao();
        }

        private void SetNamNay()
        {
            TuNgay = new DateTime(DateTime.Today.Year, 1, 1);
            DenNgay = DateTime.Today;
            LoadBaoCao();
        }

        // ── Logic chính ──────────────────────────────────────────────────────
        private void LoadBaoCao()
        {
            if (TuNgay > DenNgay)
            {
                MessageBox.Show("Từ ngày không được lớn hơn Đến ngày.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            DanhSachBaoCao.Clear();
            TrangThaiBaoCao = "Đang tải...";

            // Query tổng hợp doanh thu theo loại phòng
            // Chỉ tính hóa đơn LoaiHoaDon = 'ThanhToan' và đã thanh toán (TrangThai = 1)
            string sql = @"
                SELECT
                    lp.TenLoaiPhong,
                    COUNT(DISTINCT p.MaPhong)       AS SoPhong,
                    COUNT(DISTINCT dp.MaDatPhong)   AS SoLuotDat,
                    ISNULL(SUM(DATEDIFF(DAY, dp.NgayCheckIn, dp.NgayCheckOut)), 0) AS TongNgayThue,
                    ISNULL(SUM(hd.TongTien), 0)     AS DoanhThu
                FROM LoaiPhong lp
                LEFT JOIN Phong p
                    ON p.MaLoaiPhong = lp.MaLoaiPhong
                LEFT JOIN DatPhong dp
                    ON dp.MaPhong = p.MaPhong
                    AND dp.TrangThai = N'Đã check-out'
                    AND dp.NgayCheckOut >= @TuNgay
                    AND dp.NgayCheckOut <= @DenNgay
                LEFT JOIN HoaDon hd
                    ON hd.MaDatPhong = dp.MaDatPhong
                    AND hd.LoaiHoaDon = N'ThanhToan'
                    AND hd.TrangThai  = 1
                GROUP BY lp.MaLoaiPhong, lp.TenLoaiPhong
                ORDER BY DoanhThu DESC";

            DataTable dt = DbService.GetDataTable(sql,
                new SqlParameter("@TuNgay", TuNgay.Date),
                new SqlParameter("@DenNgay", DenNgay.Date.AddDays(1).AddSeconds(-1)));

            decimal tongDoanhThu = 0;
            foreach (DataRow r in dt.Rows)
                tongDoanhThu += Convert.ToDecimal(r["DoanhThu"]);

            foreach (DataRow r in dt.Rows)
            {
                decimal dt2 = Convert.ToDecimal(r["DoanhThu"]);
                DanhSachBaoCao.Add(new BaoCaoLoaiPhongDto
                {
                    TenLoaiPhong = r["TenLoaiPhong"].ToString(),
                    SoPhong = Convert.ToInt32(r["SoPhong"]),
                    SoLuotDat = Convert.ToInt32(r["SoLuotDat"]),
                    TongNgayThue = Convert.ToInt32(r["TongNgayThue"]),
                    DoanhThu = dt2,
                    TyLe = tongDoanhThu > 0 ? Math.Round(dt2 / tongDoanhThu * 100, 1) : 0
                });
            }

            TongDoanhThu = tongDoanhThu;
            TongLuotDat = DanhSachBaoCao.Sum(x => x.SoLuotDat);
            TrangThaiBaoCao = $"Báo cáo từ {TuNgay:dd/MM/yyyy} đến {DenNgay:dd/MM/yyyy} — {DanhSachBaoCao.Count} loại phòng";
        }

        // ── Xuất Excel ───────────────────────────────────────────────────────
        private void XuatExcel()
        {
            // Gọi ExcelExporter nếu team đã có, hoặc dùng CSV đơn giản
            try
            {
                var saveDialog = new Microsoft.Win32.SaveFileDialog
                {
                    Filter = "Excel Files|*.xlsx",
                    FileName = $"BaoCao_DoanhThu_{TuNgay:yyyyMMdd}_{DenNgay:yyyyMMdd}.xlsx"
                };
                if (saveDialog.ShowDialog() != true) return;

                // Dùng ExcelExporter của team (Helpers/ExcelExporter.cs)
                // ExcelExporter.Export(DanhSachBaoCao.ToList(), saveDialog.FileName);

                // Nếu chưa có ExcelExporter, xuất CSV tạm:
                var sb = new System.Text.StringBuilder();
                sb.AppendLine("Loại phòng,Số phòng,Số lượt đặt,Tổng ngày thuê,Doanh thu,Tỷ lệ (%)");
                foreach (var row in DanhSachBaoCao)
                    sb.AppendLine($"{row.TenLoaiPhong},{row.SoPhong},{row.SoLuotDat},{row.TongNgayThue},{row.DoanhThu},{row.TyLe}");
                sb.AppendLine($"TỔNG,,{TongLuotDat},,{TongDoanhThu},100");

                System.IO.File.WriteAllText(
                    System.IO.Path.ChangeExtension(saveDialog.FileName, ".csv"),
                    sb.ToString(),
                    System.Text.Encoding.UTF8);

                MessageBox.Show("Đã xuất file thành công!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi xuất file: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // ── In báo cáo ───────────────────────────────────────────────────────
        private void InBaoCao()
        {
            var doc = new FlowDocument
            {
                PagePadding = new Thickness(45),
                FontFamily = new System.Windows.Media.FontFamily("Arial"),
                FontSize = 12
            };

            // Tiêu đề
            doc.Blocks.Add(new Paragraph(new Run("BÁO CÁO DOANH THU THEO LOẠI PHÒNG"))
            {
                FontSize = 18,
                FontWeight = FontWeights.Bold,
                TextAlignment = TextAlignment.Center
            });
            doc.Blocks.Add(new Paragraph(new Run($"Từ ngày: {TuNgay:dd/MM/yyyy}   —   Đến ngày: {DenNgay:dd/MM/yyyy}"))
            {
                TextAlignment = TextAlignment.Center
            });
            doc.Blocks.Add(new Paragraph(new Run($"Ngày in: {DateTime.Now:dd/MM/yyyy HH:mm}")));

            // Bảng
            var table = new Table();
            foreach (var w in new[] { 2.5, 1, 1, 1.5, 2, 1 })
                table.Columns.Add(new TableColumn { Width = new GridLength(w, GridUnitType.Star) });

            var rg = new TableRowGroup();
            table.RowGroups.Add(rg);

            // Header
            var header = new TableRow { Background = System.Windows.Media.Brushes.LightGray };
            rg.Rows.Add(header);
            foreach (var h in new[] { "Loại phòng", "Số phòng", "Lượt đặt", "Tổng ngày thuê", "Doanh thu (VNĐ)", "Tỷ lệ %" })
                header.Cells.Add(new TableCell(new Paragraph(new Run(h))) { FontWeight = FontWeights.Bold });

            // Data rows
            foreach (var row in DanhSachBaoCao)
            {
                var r = new TableRow();
                rg.Rows.Add(r);
                r.Cells.Add(new TableCell(new Paragraph(new Run(row.TenLoaiPhong))));
                r.Cells.Add(new TableCell(new Paragraph(new Run(row.SoPhong.ToString()))));
                r.Cells.Add(new TableCell(new Paragraph(new Run(row.SoLuotDat.ToString()))));
                r.Cells.Add(new TableCell(new Paragraph(new Run(row.TongNgayThue.ToString()))));
                r.Cells.Add(new TableCell(new Paragraph(new Run(row.DoanhThu.ToString("N0")))));
                r.Cells.Add(new TableCell(new Paragraph(new Run(row.TyLe + "%"))));
            }

            // Footer tổng
            var footer = new TableRow { Background = System.Windows.Media.Brushes.LightYellow };
            rg.Rows.Add(footer);
            footer.Cells.Add(new TableCell(new Paragraph(new Run("TỔNG CỘNG"))) { FontWeight = FontWeights.Bold });
            footer.Cells.Add(new TableCell(new Paragraph(new Run(""))));
            footer.Cells.Add(new TableCell(new Paragraph(new Run(TongLuotDat.ToString()))) { FontWeight = FontWeights.Bold });
            footer.Cells.Add(new TableCell(new Paragraph(new Run(""))));
            footer.Cells.Add(new TableCell(new Paragraph(new Run(TongDoanhThu.ToString("N0")))) { FontWeight = FontWeights.Bold });
            footer.Cells.Add(new TableCell(new Paragraph(new Run("100%"))) { FontWeight = FontWeights.Bold });

            doc.Blocks.Add(table);

            var pd = new PrintDialog();
            if (pd.ShowDialog() == true)
                pd.PrintDocument(((IDocumentPaginatorSource)doc).DocumentPaginator, "Báo cáo doanh thu theo loại phòng");
        }
    }
}
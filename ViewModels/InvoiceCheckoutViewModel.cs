using QLKS.Commands;
using QLKS.Models;
using QLKS.Services;
using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;

namespace QLKS.ViewModels
{
    public class InvoiceCheckoutViewModel : BaseViewModel
    {
        public ObservableCollection<DatPhongDto> ActiveBookings { get; } = new ObservableCollection<DatPhongDto>();
        public ObservableCollection<HoaDonDto> HoaDons { get; } = new ObservableCollection<HoaDonDto>();

        private DatPhongDto _selectedBooking;
        public DatPhongDto SelectedBooking { get => _selectedBooking; set { SetProperty(ref _selectedBooking, value); CalculateTotal(); } }
        private HoaDonDto _selectedHoaDon;
        public HoaDonDto SelectedHoaDon { get => _selectedHoaDon; set => SetProperty(ref _selectedHoaDon, value); }

        private decimal _tienDichVu;
        public decimal TienDichVu { get => _tienDichVu; set { SetProperty(ref _tienDichVu, value); CalculateTotal(); } }
        private decimal _phuThu;
        public decimal PhuThu { get => _phuThu; set { SetProperty(ref _phuThu, value); CalculateTotal(); } }
        private decimal _tongTien;
        public decimal TongTien { get => _tongTien; set => SetProperty(ref _tongTien, value); }
        public string GhiChu { get; set; }

        public ICommand LoadCommand { get; }
        public ICommand CreateCheckoutInvoiceCommand { get; }
        public ICommand MarkPaidCommand { get; }
        public ICommand PrintInvoiceCommand { get; }
        public ICommand CalculateCommand { get; }

        public InvoiceCheckoutViewModel()
        {
            LoadCommand = new RelayCommand(_ => LoadAll());
            CreateCheckoutInvoiceCommand = new RelayCommand(_ => CreateCheckoutInvoice());
            MarkPaidCommand = new RelayCommand(_ => MarkPaid());
            PrintInvoiceCommand = new RelayCommand(_ => PrintInvoice());
            CalculateCommand = new RelayCommand(_ => CalculateTotal());
            LoadAll();
        }

        private void LoadAll(){ LoadActiveBookings(); LoadInvoices(); }

        private void LoadActiveBookings()
        {
            ActiveBookings.Clear();
            var dt = DbService.GetDataTable(@"SELECT dp.MaDatPhong, dp.MaKH, kh.HoTen TenKhachHang, dp.MaPhong, p.SoPhong, dp.MaNV, nv.HoTen TenNhanVien, dp.NgayDat, dp.NgayCheckIn, dp.NgayCheckOut, dp.SoNguoi, dp.TrangThai, dp.TongTien, dp.GhiChu, lp.GiaPhong
FROM DatPhong dp JOIN KhachHang kh ON dp.MaKH=kh.MaKH JOIN Phong p ON dp.MaPhong=p.MaPhong JOIN LoaiPhong lp ON p.MaLoaiPhong=lp.MaLoaiPhong JOIN NhanVien nv ON dp.MaNV=nv.MaNV
WHERE dp.TrangThai=N'Đã check-in'
ORDER BY dp.MaDatPhong DESC");
            foreach (DataRow r in dt.Rows) ActiveBookings.Add(new DatPhongDto{ MaDatPhong=Convert.ToInt32(r["MaDatPhong"]), MaKH=Convert.ToInt32(r["MaKH"]), TenKhachHang=r["TenKhachHang"].ToString(), MaPhong=Convert.ToInt32(r["MaPhong"]), SoPhong=r["SoPhong"].ToString(), MaNV=Convert.ToInt32(r["MaNV"]), TenNhanVien=r["TenNhanVien"].ToString(), NgayDat=Convert.ToDateTime(r["NgayDat"]), NgayCheckIn=Convert.ToDateTime(r["NgayCheckIn"]), NgayCheckOut=Convert.ToDateTime(r["NgayCheckOut"]), SoNguoi=Convert.ToInt32(r["SoNguoi"]), TrangThai=r["TrangThai"].ToString(), TongTien=r["TongTien"]==DBNull.Value?(decimal?)null:Convert.ToDecimal(r["TongTien"]), GhiChu=r["GhiChu"].ToString(), GiaPhong=Convert.ToDecimal(r["GiaPhong"]) });
        }

        private void LoadInvoices()
        {
            HoaDons.Clear();
            var dt = DbService.GetDataTable(@"SELECT hd.MaHoaDon, hd.MaDatPhong, kh.HoTen TenKhachHang, p.SoPhong, nv.HoTen TenNhanVien, hd.NgayLap, hd.TongTien, hd.TrangThai, hd.LoaiHoaDon, hd.GhiChu
FROM HoaDon hd LEFT JOIN DatPhong dp ON hd.MaDatPhong=dp.MaDatPhong LEFT JOIN KhachHang kh ON dp.MaKH=kh.MaKH LEFT JOIN Phong p ON dp.MaPhong=p.MaPhong JOIN NhanVien nv ON hd.MaNV=nv.MaNV
WHERE hd.LoaiHoaDon=N'ThanhToan'
ORDER BY hd.MaHoaDon DESC");
            foreach (DataRow r in dt.Rows) HoaDons.Add(new HoaDonDto{ MaHoaDon=Convert.ToInt32(r["MaHoaDon"]), MaDatPhong=r["MaDatPhong"]==DBNull.Value?(int?)null:Convert.ToInt32(r["MaDatPhong"]), TenKhachHang=r["TenKhachHang"].ToString(), SoPhong=r["SoPhong"].ToString(), TenNhanVien=r["TenNhanVien"].ToString(), NgayLap=Convert.ToDateTime(r["NgayLap"]), TongTien=Convert.ToDecimal(r["TongTien"]), TrangThai=Convert.ToBoolean(r["TrangThai"]), LoaiHoaDon=r["LoaiHoaDon"].ToString(), GhiChu=r["GhiChu"].ToString() });
        }

        private int CountNights(DatPhongDto b)
        {
            if (b == null) return 0;
            int nights = (b.NgayCheckOut.Date - b.NgayCheckIn.Date).Days;
            return nights <= 0 ? 1 : nights;
        }

        private void CalculateTotal()
        {
            if (SelectedBooking == null) { TongTien = 0; return; }
            TongTien = CountNights(SelectedBooking) * SelectedBooking.GiaPhong + TienDichVu + PhuThu;
        }

        private void CreateCheckoutInvoice()
        {
            if (!SessionService.HasRole("Admin", "LeTan", "QuanLy")) { MessageBox.Show("Bạn không có quyền lập hóa đơn."); return; }
            if (SelectedBooking == null) { MessageBox.Show("Chọn phiếu đang check-in."); return; }
            CalculateTotal();
            using (var conn = DbService.GetConnection())
            using (var cmd = conn.CreateCommand())
            {
                conn.Open();
                var tran = conn.BeginTransaction();
                cmd.Transaction = tran;
                try
                {
                    cmd.CommandText = @"INSERT INTO HoaDon(MaDatPhong,MaNV,TongTien,TrangThai,LoaiHoaDon,GhiChu) VALUES(@MaDatPhong,@MaNV,@TongTien,1,N'ThanhToan',@GhiChu); SELECT SCOPE_IDENTITY();";
                    cmd.Parameters.AddWithValue("@MaDatPhong", SelectedBooking.MaDatPhong);
                    cmd.Parameters.AddWithValue("@MaNV", SessionService.CurrentUser.MaNV);
                    cmd.Parameters.AddWithValue("@TongTien", TongTien);
                    cmd.Parameters.AddWithValue("@GhiChu", (object)GhiChu ?? DBNull.Value);
                    int maHoaDon = Convert.ToInt32(cmd.ExecuteScalar());
                    cmd.Parameters.Clear();

                    cmd.CommandText = "INSERT INTO ChiTietHoaDon(MaHoaDon,MaDichVu,SoLuong,DonGia,ThanhTien,GhiChu) VALUES(@MaHoaDon,NULL,@SoDem,@GiaPhong,@TienPhong,N'Tiền phòng')";
                    cmd.Parameters.AddWithValue("@MaHoaDon", maHoaDon);
                    cmd.Parameters.AddWithValue("@SoDem", CountNights(SelectedBooking));
                    cmd.Parameters.AddWithValue("@GiaPhong", SelectedBooking.GiaPhong);
                    cmd.Parameters.AddWithValue("@TienPhong", CountNights(SelectedBooking) * SelectedBooking.GiaPhong);
                    cmd.ExecuteNonQuery();
                    cmd.Parameters.Clear();

                    if (TienDichVu > 0 || PhuThu > 0)
                    {
                        cmd.CommandText = "INSERT INTO ChiTietHoaDon(MaHoaDon,MaDichVu,SoLuong,DonGia,ThanhTien,GhiChu) VALUES(@MaHoaDon,NULL,1,@DonGia,@ThanhTien,N'Dịch vụ/phụ thu')";
                        cmd.Parameters.AddWithValue("@MaHoaDon", maHoaDon);
                        cmd.Parameters.AddWithValue("@DonGia", TienDichVu + PhuThu);
                        cmd.Parameters.AddWithValue("@ThanhTien", TienDichVu + PhuThu);
                        cmd.ExecuteNonQuery();
                        cmd.Parameters.Clear();
                    }

                    cmd.CommandText = "UPDATE DatPhong SET TrangThai=N'Đã check-out', TongTien=@TongTien WHERE MaDatPhong=@MaDatPhong; UPDATE Phong SET TrangThai=N'Trống' WHERE MaPhong=@MaPhong;";
                    cmd.Parameters.AddWithValue("@TongTien", TongTien);
                    cmd.Parameters.AddWithValue("@MaDatPhong", SelectedBooking.MaDatPhong);
                    cmd.Parameters.AddWithValue("@MaPhong", SelectedBooking.MaPhong);
                    cmd.ExecuteNonQuery();
                    tran.Commit();
                    MessageBox.Show("Đã tạo hóa đơn và check-out thành công.");
                    TienDichVu = 0; PhuThu = 0; GhiChu = null; OnPropertyChanged(nameof(GhiChu)); LoadAll();
                }
                catch (Exception ex)
                {
                    tran.Rollback();
                    MessageBox.Show("Lỗi tạo hóa đơn: " + ex.Message);
                }
            }
        }

        private void MarkPaid()
        {
            if (SelectedHoaDon == null) { MessageBox.Show("Chọn hóa đơn cần cập nhật."); return; }
            DbService.ExecuteNonQuery("UPDATE HoaDon SET TrangThai=1 WHERE MaHoaDon=@MaHoaDon", new SqlParameter("@MaHoaDon", SelectedHoaDon.MaHoaDon));
            LoadInvoices(); MessageBox.Show("Đã cập nhật đã thanh toán.");
        }

        private void PrintInvoice()
        {
            if (SelectedHoaDon == null) { MessageBox.Show("Chọn hóa đơn cần in."); return; }
            var doc = new FlowDocument { PagePadding = new Thickness(45), FontFamily = new System.Windows.Media.FontFamily("Arial"), FontSize = 13 };
            doc.Blocks.Add(new Paragraph(new Run("HÓA ĐƠN THANH TOÁN KHÁCH SẠN")) { FontSize = 20, FontWeight = FontWeights.Bold, TextAlignment = TextAlignment.Center });
            doc.Blocks.Add(new Paragraph(new Run($"Mã hóa đơn: {SelectedHoaDon.MaHoaDon}")));
            doc.Blocks.Add(new Paragraph(new Run($"Khách hàng: {SelectedHoaDon.TenKhachHang}")));
            doc.Blocks.Add(new Paragraph(new Run($"Phòng: {SelectedHoaDon.SoPhong}")));
            doc.Blocks.Add(new Paragraph(new Run($"Nhân viên lập: {SelectedHoaDon.TenNhanVien}")));
            doc.Blocks.Add(new Paragraph(new Run($"Ngày lập: {SelectedHoaDon.NgayLap:dd/MM/yyyy HH:mm}")));
            doc.Blocks.Add(new Paragraph(new Run($"Tổng tiền: {SelectedHoaDon.TongTien:N0} VNĐ")) { FontSize = 16, FontWeight = FontWeights.Bold });
            doc.Blocks.Add(new Paragraph(new Run($"Trạng thái: {SelectedHoaDon.TrangThaiText}")));
            new PrintDialog().PrintDocument(((IDocumentPaginatorSource)doc).DocumentPaginator, "In hóa đơn thanh toán");
        }
    }
}

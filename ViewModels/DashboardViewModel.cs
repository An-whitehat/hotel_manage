using QLKS.Models;
using QLKS.Services;
using System;
using System.Data.SqlClient;
using System.Linq;

namespace QLKS.ViewModels
{
    public class DashboardViewModel : BaseViewModel
    {
        // ─── Stat cards ──────────────────────────────────────────────────────
        private int _phongTrong;
        public int PhongTrong
        {
            get => _phongTrong;
            set { _phongTrong = value; OnPropertyChanged(); }
        }

        private int _phongDangThue;
        public int PhongDangThue
        {
            get => _phongDangThue;
            set { _phongDangThue = value; OnPropertyChanged(); }
        }

        private int _khachHomNay;
        public int KhachHomNay
        {
            get => _khachHomNay;
            set { _khachHomNay = value; OnPropertyChanged(); }
        }

        private decimal _doanhThuHomNay;
        public decimal DoanhThuHomNay
        {
            get => _doanhThuHomNay;
            set { _doanhThuHomNay = value; OnPropertyChanged(); }
        }

        private int _datPhongChoXuLy;
        public int DatPhongChoXuLy
        {
            get => _datPhongChoXuLy;
            set { _datPhongChoXuLy = value; OnPropertyChanged(); }
        }

        private int _tongNhanVien;
        public int TongNhanVien
        {
            get => _tongNhanVien;
            set { _tongNhanVien = value; OnPropertyChanged(); }
        }

        // ─── Info ─────────────────────────────────────────────────────────────
        public string NgayHienTai => DateTime.Now.ToString("dddd, dd/MM/yyyy");
        public string TenNguoiDung => SessionService.CurrentUser?.HoTen ?? "Admin";
        public string VaiTro => SessionService.CurrentUser?.VaiTro ?? "Quản trị viên";

        public DashboardViewModel()
        {
            LoadStats();
        }

        private void LoadStats()
        {
            try
            {
                using (var db = new HotelManagementEntities())
                {
                    PhongTrong = db.Phongs.Count(p => p.TrangThai == "Trống");
                    PhongDangThue = db.Phongs.Count(p => p.TrangThai == "Đang thuê");
                    TongNhanVien = db.NhanViens.Count(n => n.TrangThai == true);
                    DatPhongChoXuLy = db.DatPhongs.Count(d => d.TrangThai == "Đang chờ");
                    KhachHomNay = db.DatPhongs.Count(d =>
                        d.TrangThai == "Đã check-in");

                    var today = DateTime.Today;
                    DoanhThuHomNay = db.HoaDons
                        .Where(h => h.TrangThai == true &&
                                    h.NgayLap.HasValue &&
                                    h.NgayLap.Value >= today)
                        .Sum(h => (decimal?)h.TongTien) ?? 0;
                }
            }
            catch
            {
                // Nếu chưa có DB, giữ giá trị 0
            }
        }
    }
}

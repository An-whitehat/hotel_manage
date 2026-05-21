using System;

namespace QLKS.Models
{
    public class NhanVienDto
    {
        public int MaNV { get; set; }
        public string HoTen { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public string VaiTro { get; set; }
        public string SDT { get; set; }
        public DateTime? NgayVaoLam { get; set; }
        public bool TrangThai { get; set; }
        public string TrangThaiText => TrangThai ? "Đang làm" : "Nghỉ";
    }

    public class CaLamViecDto
    {
        public int MaCa { get; set; }
        public int MaNV { get; set; }
        public string HoTen { get; set; }
        public DateTime NgayLam { get; set; }
        public string Ca { get; set; }
        public string GhiChu { get; set; }
    }

    public class KhachHangDto
    {
        public int MaKH { get; set; }
        public string HoTen { get; set; }
        public string CCCD { get; set; }
        public string SDT { get; set; }
        public string Display => $"{MaKH} - {HoTen} - {SDT}";
    }

    public class PhongDto
    {
        public int MaPhong { get; set; }
        public string SoPhong { get; set; }
        public int MaLoaiPhong { get; set; }
        public string TenLoaiPhong { get; set; }
        public decimal GiaPhong { get; set; }
        public string TrangThai { get; set; }
        public string Display => $"{SoPhong} - {TenLoaiPhong} - {GiaPhong:N0}đ - {TrangThai}";
    }

    public class DatPhongDto
    {
        public int MaDatPhong { get; set; }
        public int MaKH { get; set; }
        public string TenKhachHang { get; set; }
        public int MaPhong { get; set; }
        public string SoPhong { get; set; }
        public int MaNV { get; set; }
        public string TenNhanVien { get; set; }
        public DateTime NgayDat { get; set; }
        public DateTime NgayCheckIn { get; set; }
        public DateTime NgayCheckOut { get; set; }
        public int SoNguoi { get; set; }
        public string TrangThai { get; set; }
        public decimal? TongTien { get; set; }
        public string GhiChu { get; set; }
        public decimal GiaPhong { get; set; }
        public string Display => $"#{MaDatPhong} - {TenKhachHang} - Phòng {SoPhong} - {TrangThai}";
    }

    public class HoaDonDto
    {
        public int MaHoaDon { get; set; }
        public int? MaDatPhong { get; set; }
        public string TenKhachHang { get; set; }
        public string SoPhong { get; set; }
        public string TenNhanVien { get; set; }
        public DateTime NgayLap { get; set; }
        public decimal TongTien { get; set; }
        public bool TrangThai { get; set; }
        public string LoaiHoaDon { get; set; }
        public string GhiChu { get; set; }
        public string TrangThaiText => TrangThai ? "Đã thanh toán" : "Chưa thanh toán";
    }

    public class CurrentUser
    {
        public int MaNV { get; set; }
        public string HoTen { get; set; }
        public string Username { get; set; }
        public string VaiTro { get; set; }
        public bool IsAdmin => VaiTro == "Admin";
        public bool IsManager => VaiTro == "QuanLy";
        public bool IsReceptionist => VaiTro == "LeTan";
    }
}

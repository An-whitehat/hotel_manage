-- =============================================
-- DATABASE: HotelManagement
-- Mô tả: Phần mềm quản lý khách sạn - Nhóm 10
-- Ngày tạo: 12/05/2026
-- =============================================

USE master;
GO

-- Tạo database (nếu chưa có)
IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'HotelManagement')
BEGIN
    CREATE DATABASE HotelManagement;
END
GO

USE HotelManagement;
GO

-- =============================================
-- 1. BẢNG DANH MỤC
-- =============================================

-- Loại phòng
CREATE TABLE LoaiPhong (
    MaLoaiPhong INT PRIMARY KEY IDENTITY(1,1),
    TenLoaiPhong NVARCHAR(100) NOT NULL,
    GiaPhong DECIMAL(18,2) NOT NULL,
    SoGiuong INT NOT NULL DEFAULT 2,
    MoTa NVARCHAR(500),
    NgayTao DATETIME DEFAULT GETDATE()
);
GO

-- Phòng
CREATE TABLE Phong (
    MaPhong INT PRIMARY KEY IDENTITY(1,1),
    SoPhong NVARCHAR(10) NOT NULL UNIQUE,
    MaLoaiPhong INT NOT NULL FOREIGN KEY REFERENCES LoaiPhong(MaLoaiPhong),
    TrangThai NVARCHAR(20) DEFAULT N'Trống', -- Trống, Đang thuê, Đang dọn, Bảo trì
    GhiChu NVARCHAR(200),
    NgayTao DATETIME DEFAULT GETDATE()
);
GO

-- Dịch vụ
CREATE TABLE DichVu (
    MaDichVu INT PRIMARY KEY IDENTITY(1,1),
    TenDichVu NVARCHAR(100) NOT NULL,
    GiaDichVu DECIMAL(18,2) NOT NULL,
    MoTa NVARCHAR(200),
    NgayTao DATETIME DEFAULT GETDATE()
);
GO

-- =============================================
-- 2. BẢNG NGƯỜI DÙNG & KHÁCH HÀNG
-- =============================================

-- Khách hàng
CREATE TABLE KhachHang (
    MaKH INT PRIMARY KEY IDENTITY(1,1),
    HoTen NVARCHAR(100) NOT NULL,
    CCCD NVARCHAR(20) UNIQUE,
    SDT NVARCHAR(15),
    DiaChi NVARCHAR(200),
    Email NVARCHAR(100),
    NgayTao DATETIME DEFAULT GETDATE()
);
GO

-- Nhân viên (dùng để đăng nhập + phân quyền)
CREATE TABLE NhanVien (
    MaNV INT PRIMARY KEY IDENTITY(1,1),
    HoTen NVARCHAR(100) NOT NULL,
    Username NVARCHAR(50) UNIQUE NOT NULL,
    PasswordHash NVARCHAR(255) NOT NULL,
    VaiTro NVARCHAR(30) NOT NULL,           -- Admin, LeTan, QuanLy
    SDT NVARCHAR(15),
    NgayVaoLam DATE,
    TrangThai BIT DEFAULT 1,                -- 1: Đang làm, 0: Nghỉ
    NgayTao DATETIME DEFAULT GETDATE()
);
GO

-- =============================================
-- 3. BẢNG GIAO DỊCH CHÍNH
-- =============================================

-- Phiếu đặt phòng / Booking
CREATE TABLE DatPhong (
    MaDatPhong INT PRIMARY KEY IDENTITY(1,1),
    MaKH INT NOT NULL FOREIGN KEY REFERENCES KhachHang(MaKH),
    MaPhong INT NOT NULL FOREIGN KEY REFERENCES Phong(MaPhong),
    MaNV INT NOT NULL FOREIGN KEY REFERENCES NhanVien(MaNV),   -- Người đặt
    NgayDat DATETIME DEFAULT GETDATE(),
    NgayCheckIn DATE NOT NULL,
    NgayCheckOut DATE NOT NULL,
    SoNguoi INT NOT NULL DEFAULT 2,
    TrangThai NVARCHAR(30) DEFAULT N'Đang chờ', -- Đang chờ, Đã check-in, Đã check-out, Hủy
    TongTien DECIMAL(18,2) NULL,
    GhiChu NVARCHAR(200),
    NgayTao DATETIME DEFAULT GETDATE()
);
GO

-- Hóa đơn
CREATE TABLE HoaDon (
    MaHoaDon INT PRIMARY KEY IDENTITY(1,1),
    MaDatPhong INT NULL FOREIGN KEY REFERENCES DatPhong(MaDatPhong),
    MaNV INT NOT NULL FOREIGN KEY REFERENCES NhanVien(MaNV),   -- Người lập hóa đơn
    NgayLap DATETIME DEFAULT GETDATE(),
    TongTien DECIMAL(18,2) NOT NULL,
    TrangThai BIT DEFAULT 0,                    -- 0: Chưa thanh toán, 1: Đã thanh toán
    LoaiHoaDon NVARCHAR(20) NOT NULL,           -- 'ThanhToan' hoặc 'Nhap'
    GhiChu NVARCHAR(200)
);
GO

-- Chi tiết hóa đơn (dùng cho dịch vụ + các khoản khác)
CREATE TABLE ChiTietHoaDon (
    MaCTHD INT PRIMARY KEY IDENTITY(1,1),
    MaHoaDon INT NOT NULL FOREIGN KEY REFERENCES HoaDon(MaHoaDon),
    MaDichVu INT NULL FOREIGN KEY REFERENCES DichVu(MaDichVu),
    SoLuong INT NOT NULL DEFAULT 1,
    DonGia DECIMAL(18,2) NOT NULL,
    ThanhTien DECIMAL(18,2) NOT NULL,
    GhiChu NVARCHAR(100)
);
GO

-- Ca làm việc (cho chức năng in danh sách nhân viên + ca)
CREATE TABLE CaLamViec (
    MaCa INT PRIMARY KEY IDENTITY(1,1),
    MaNV INT NOT NULL FOREIGN KEY REFERENCES NhanVien(MaNV),
    NgayLam DATE NOT NULL,
    Ca NVARCHAR(20) NOT NULL,                   -- Sáng, Chiều, Tối
    GhiChu NVARCHAR(100),
    NgayTao DATETIME DEFAULT GETDATE()
);
GO

-- =============================================
-- TẠO INDEX ĐỂ TĂNG TỐC ĐỘ TÌM KIẾM
-- =============================================
CREATE INDEX IX_Phong_TrangThai ON Phong(TrangThai);
CREATE INDEX IX_DatPhong_TrangThai ON DatPhong(TrangThai);
CREATE INDEX IX_HoaDon_NgayLap ON HoaDon(NgayLap);
GO

PRINT N'===== DATABASE HotelManagement ĐÃ TẠO THÀNH CÔNG =====';
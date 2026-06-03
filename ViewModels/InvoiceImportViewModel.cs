using QLKS.Commands;
using QLKS.Models;
using QLKS.Services;
using QLKS.Views;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace QLKS.ViewModels
{
    public class InvoiceImportViewModel : BaseViewModel
    {
        private HotelManagementEntities _db = new HotelManagementEntities();

        // ── Danh sách phiếu đang check-in ────────────────────────────────
        private ObservableCollection<DatPhongDto> _listDatPhongCheckIn;
        public ObservableCollection<DatPhongDto> ListDatPhongCheckIn
        {
            get => _listDatPhongCheckIn;
            set => SetProperty(ref _listDatPhongCheckIn, value);
        }

        private DatPhongDto _selectedDatPhong;
        public DatPhongDto SelectedDatPhong
        {
            get => _selectedDatPhong;
            set
            {
                _selectedDatPhong = value;
                OnPropertyChanged();
                if (_currentInvoice != null && value != null)
                    _currentInvoice.MaDatPhong = value.MaDatPhong;
            }
        }

        // ── Hóa đơn hiện tại ─────────────────────────────────────────────
        private HoaDon _currentInvoice;
        public HoaDon CurrentInvoice
        {
            get => _currentInvoice;
            set { _currentInvoice = value; OnPropertyChanged(); }
        }

        // ── Danh sách dịch vụ ────────────────────────────────────────────
        private ObservableCollection<DichVu> _listDichVu;
        public ObservableCollection<DichVu> ListDichVu
        {
            get => _listDichVu;
            set => SetProperty(ref _listDichVu, value);
        }

        // ── Chi tiết hóa đơn ─────────────────────────────────────────────
        private ObservableCollection<ChiTietHoaDon> _currentDetails;
        public ObservableCollection<ChiTietHoaDon> CurrentDetails
        {
            get => _currentDetails;
            set => SetProperty(ref _currentDetails, value);
        }

        private DichVu _selectedDichVu;
        public DichVu SelectedDichVu
        {
            get => _selectedDichVu;
            set { _selectedDichVu = value; OnPropertyChanged(); }
        }

        private int _quantityToImport = 1;
        public int QuantityToImport
        {
            get => _quantityToImport;
            set { _quantityToImport = value; OnPropertyChanged(); }
        }

        private string _ghiChu;
        public string GhiChu
        {
            get => _ghiChu;
            set
            {
                _ghiChu = value;
                if (CurrentInvoice != null) CurrentInvoice.GhiChu = value;
                OnPropertyChanged();
            }
        }

        // ── Commands ─────────────────────────────────────────────────────
        public ICommand CreateNewInvoiceCmd { get; set; }
        public ICommand AddItemToInvoiceCmd { get; set; }
        public ICommand SaveInvoiceCmd      { get; set; }
        public ICommand PrintInvoiceCmd     { get; set; }

        // ── Constructor ───────────────────────────────────────────────────
        public InvoiceImportViewModel()
        {
            LoadDichVu();
            LoadDatPhongCheckIn();
            CurrentDetails = new ObservableCollection<ChiTietHoaDon>();
            ResetInvoice();

            CreateNewInvoiceCmd = new RelayCommand(_ => ResetInvoice());

            AddItemToInvoiceCmd = new RelayCommand(_ =>
            {
                if (SelectedDichVu == null) return;

                decimal donGia    = SelectedDichVu.GiaDichVu;
                decimal thanhTien = donGia * QuantityToImport;

                var existing = CurrentDetails
                    .FirstOrDefault(d => d.MaDichVu == SelectedDichVu.MaDichVu);

                if (existing != null)
                {
                    existing.SoLuong  += QuantityToImport;
                    existing.ThanhTien = existing.SoLuong * existing.DonGia;
                }
                else
                {
                    CurrentDetails.Add(new ChiTietHoaDon
                    {
                        MaDichVu  = SelectedDichVu.MaDichVu,
                        DichVu    = SelectedDichVu,
                        SoLuong   = QuantityToImport,
                        DonGia    = donGia,
                        ThanhTien = thanhTien
                    });
                }
                UpdateInvoiceTotal();

            }, _ => SelectedDichVu != null && QuantityToImport > 0);

            SaveInvoiceCmd = new RelayCommand(_ =>
            {
                if (SelectedDatPhong == null)
                {
                    MessageBox.Show("Vui lòng chọn phiếu đặt phòng đang check-in!",
                        "Thiếu thông tin", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                if (CurrentDetails.Count == 0)
                {
                    MessageBox.Show("Vui lòng thêm ít nhất 1 dịch vụ!",
                        "Thiếu thông tin", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                try
                {
                    int maNV = SessionService.CurrentUser?.MaNV
                               ?? _db.NhanViens.FirstOrDefault()?.MaNV ?? 1;

                    CurrentInvoice.MaNV       = maNV;
                    CurrentInvoice.MaDatPhong = SelectedDatPhong.MaDatPhong;
                    CurrentInvoice.LoaiHoaDon = "Nhap";

                    _db.HoaDons.Add(CurrentInvoice);
                    _db.SaveChanges();

                    foreach (var detail in CurrentDetails)
                    {
                        detail.MaHoaDon = CurrentInvoice.MaHoaDon;
                        _db.ChiTietHoaDons.Add(detail);
                    }
                    _db.SaveChanges();

                    MessageBox.Show(
                        $"Đã lưu dịch vụ cho khách {SelectedDatPhong.TenKhachHang} - Phòng {SelectedDatPhong.SoPhong}",
                        "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);

                    ResetInvoice();
                    LoadDatPhongCheckIn();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi hệ thống: " + ex.Message,
                        "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }

            }, _ => CurrentDetails.Count > 0);

            PrintInvoiceCmd = new RelayCommand(_ =>
            {
                if (CurrentInvoice.MaHoaDon <= 0)
                {
                    MessageBox.Show("Vui lòng lưu hóa đơn trước!");
                    return;
                }
                new InvoiceReportWindow(CurrentInvoice.MaHoaDon).ShowDialog();
            });
        }

        // ── Load phiếu đang check-in ──────────────────────────────────────
        private void LoadDatPhongCheckIn()
        {
            var dt = DbService.GetDataTable(@"
                SELECT dp.MaDatPhong, kh.HoTen TenKhachHang,
                       p.SoPhong, dp.NgayCheckIn, dp.NgayCheckOut
                FROM DatPhong dp
                JOIN KhachHang kh ON dp.MaKH  = kh.MaKH
                JOIN Phong     p  ON dp.MaPhong = p.MaPhong
                WHERE dp.TrangThai = N'Đã check-in'
                ORDER BY dp.NgayCheckIn DESC");

            ListDatPhongCheckIn = new ObservableCollection<DatPhongDto>();
            foreach (System.Data.DataRow r in dt.Rows)
            {
                ListDatPhongCheckIn.Add(new DatPhongDto
                {
                    MaDatPhong   = Convert.ToInt32(r["MaDatPhong"]),
                    TenKhachHang = r["TenKhachHang"].ToString(),
                    SoPhong      = r["SoPhong"].ToString(),
                    NgayCheckIn  = Convert.ToDateTime(r["NgayCheckIn"]),
                    NgayCheckOut = Convert.ToDateTime(r["NgayCheckOut"]),
                });
            }
        }

        private void LoadDichVu()
        {
            using (var freshDb = new HotelManagementEntities())
                ListDichVu = new ObservableCollection<DichVu>(freshDb.DichVus.ToList());
        }

        private void ResetInvoice()
        {
            CurrentInvoice = new HoaDon
            {
                NgayLap    = DateTime.Now,
                TongTien   = 0,
                TrangThai  = true,
                LoaiHoaDon = "Nhap",
                GhiChu     = ""
            };
            CurrentDetails?.Clear();
            SelectedDatPhong = null;
            GhiChu = "";
        }

        private void UpdateInvoiceTotal()
        {
            CurrentInvoice.TongTien = CurrentDetails.Sum(d => d.ThanhTien);
            OnPropertyChanged(nameof(CurrentInvoice));
        }
    }
}

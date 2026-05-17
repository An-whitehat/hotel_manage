using QLKS.Commands;
using QLKS.Models; // Sửa từ HotelManagement.Models thành QLKS.Models
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel; // Đã thêm để fix lỗi ObservableCollection
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows; // Đã thêm để sử dụng được MessageBox
using System.Windows.Input;

namespace QLKS.ViewModels // Sửa từ HotelManagement thành QLKS
{
    // Đã thêm kế thừa BaseViewModel để gọi được hàm OnPropertyChanged()
    public class InvoiceImportViewModel : BaseViewModel
    {
        private HotelManagementEntities _db = new HotelManagementEntities();

        // Hóa đơn nhập hiện tại đang lập
        private HoaDon _currentInvoice;
        public HoaDon CurrentInvoice
        {
            get => _currentInvoice;
            set { _currentInvoice = value; OnPropertyChanged(); }
        }

        // Danh sách dịch vụ/hàng hóa có sẵn để chọn nhập
        public ObservableCollection<DichVu> ListDichVu { get; set; }

        // Chi tiết các mặt hàng trong hóa đơn nhập hiện tại
        public ObservableCollection<ChiTietHoaDon> CurrentDetails { get; set; }

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

        public ICommand CreateNewInvoiceCmd { get; set; }
        public ICommand AddItemToInvoiceCmd { get; set; }
        public ICommand SaveInvoiceCmd { get; set; }

        public InvoiceImportViewModel()
        {
            ListDichVu = new ObservableCollection<DichVu>(_db.DichVus.ToList());
            CurrentDetails = new ObservableCollection<ChiTietHoaDon>();
            ResetInvoice();

            // Đã đồng bộ loại bỏ <object> của RelayCommand giống m và Bảo
            CreateNewInvoiceCmd = new RelayCommand(p => {
                ResetInvoice();
            });

            // Thêm một mặt hàng vào danh sách tạm thời
            AddItemToInvoiceCmd = new RelayCommand(p => {
                decimal itemPrice = SelectedDichVu.GiaDichVu != null ? Convert.ToDecimal(SelectedDichVu.GiaDichVu) : 0m; decimal totalItem = itemPrice * QuantityToImport;

                var existingItem = CurrentDetails.FirstOrDefault(d => d.MaDichVu == SelectedDichVu.MaDichVu);
                if (existingItem != null)
                {
                    existingItem.SoLuong += QuantityToImport;
                    existingItem.ThanhTien = existingItem.SoLuong * existingItem.DonGia;
                }
                else
                {
                    CurrentDetails.Add(new ChiTietHoaDon
                    {
                        MaDichVu = SelectedDichVu.MaDichVu,
                        DichVu = SelectedDichVu,
                        SoLuong = QuantityToImport,
                        DonGia = itemPrice,
                        ThanhTien = totalItem
                    });
                }
                UpdateInvoiceTotal();
            }, p => SelectedDichVu != null && QuantityToImport > 0);

            // Lưu toàn bộ hóa đơn xuống Database
            SaveInvoiceCmd = new RelayCommand(p => {
                try
                {
                    // LƯU Ý: Gán MaNV tĩnh ở đây. Thực tế phải lấy từ thông tin đăng nhập của Phúc (Ngày 1)
                    CurrentInvoice.MaNV = _db.NhanViens.FirstOrDefault()?.MaNV ?? 1;

                    _db.HoaDons.Add(CurrentInvoice);
                    _db.SaveChanges(); // Tạo ID tự tăng cho HoaDon trước

                    foreach (var detail in CurrentDetails)
                    {
                        detail.MaHoaDon = CurrentInvoice.MaHoaDon;
                        _db.ChiTietHoaDons.Add(detail);
                    }
                    _db.SaveChanges();

                    MessageBox.Show("Lưu hóa đơn nhập kho thành công!");
                    ResetInvoice();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi hệ thống: " + ex.Message);
                }
            }, p => CurrentDetails.Count > 0);
        }

        private void ResetInvoice()
        {
            CurrentInvoice = new HoaDon
            {
                NgayLap = DateTime.Now,
                TongTien = 0,
                TrangThai = true, // Đã thanh toán tiền nhập kho
                LoaiHoaDon = "Nhap",
                GhiChu = ""
            };
            CurrentDetails.Clear();
        }

        private void UpdateInvoiceTotal()
        {
            CurrentInvoice.TongTien = CurrentDetails.Sum(d => d.ThanhTien);
            OnPropertyChanged(nameof(CurrentInvoice));
        }
    }
}
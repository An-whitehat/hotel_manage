using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using QLKS.Commands;
using QLKS.Models;

namespace QLKS.ViewModels
{
    public class InvoiceImportViewModel : BaseViewModel
    {
        private HotelManagementEntities _db = new HotelManagementEntities();

        private HoaDon _currentInvoice;
        public HoaDon CurrentInvoice
        {
            get => _currentInvoice;
            set { _currentInvoice = value; OnPropertyChanged(); }
        }

        public ObservableCollection<DichVu> ListDichVu { get; set; }
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

            CreateNewInvoiceCmd = new RelayCommand(p => {
                ResetInvoice();
            });

            AddItemToInvoiceCmd = new RelayCommand(p => {
                decimal itemPrice = SelectedDichVu.GiaDichVu;
                decimal totalItem = itemPrice * QuantityToImport;

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

            SaveInvoiceCmd = new RelayCommand(p => {
                try
                {
                    CurrentInvoice.MaNV = _db.NhanViens.FirstOrDefault()?.MaNV ?? 1;
                    _db.HoaDons.Add(CurrentInvoice);
                    _db.SaveChanges();

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
                TrangThai = true,
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
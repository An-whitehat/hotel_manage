using QLKS.Commands;
using QLKS.Models;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace QLKS.ViewModels
{
    public class CustomerViewModel : BaseViewModel
    {
        private HotelManagementEntities db = new HotelManagementEntities();

        private ObservableCollection<KhachHang> _danhSachKhachHang;
        public ObservableCollection<KhachHang> DanhSachKhachHang
        {
            get => _danhSachKhachHang;
            set { _danhSachKhachHang = value; OnPropertyChanged(); }
        }

        private KhachHang _selectedKhachHang;
        public KhachHang SelectedKhachHang
        {
            get => _selectedKhachHang;
            set
            {
                _selectedKhachHang = value;

                OnPropertyChanged(nameof(SelectedKhachHang));

                System.Windows.Input.CommandManager.InvalidateRequerySuggested();
            }
        }
        public ICommand AddCommand { get; set; }
        public ICommand UpdateCommand { get; set; }
        public ICommand DeleteCommand { get; set; }
        public ICommand ClearCommand { get; set; }

        public CustomerViewModel()
        {
            LoadData();
            ClearForm();

            AddCommand = new RelayCommand(p => ExecuteAdd());
            UpdateCommand = new RelayCommand(p => ExecuteUpdate(), p => SelectedKhachHang != null && SelectedKhachHang.MaKH != 0);
            DeleteCommand = new RelayCommand(p => ExecuteDelete(), p => SelectedKhachHang != null && SelectedKhachHang.MaKH != 0);
            ClearCommand = new RelayCommand(p => ClearForm());
        }

        private void LoadData()
        {
            // Tạo context mới mỗi lần load để tránh EF cache dữ liệu cũ
            using (var freshDb = new HotelManagementEntities())
            {
                var list = freshDb.KhachHangs.ToList();
                DanhSachKhachHang = new ObservableCollection<KhachHang>(list);
            }
        }

        private void ExecuteAdd()
        {
            if (!ValidateForm()) return;

            try
            {
                if (db.KhachHangs.Any(k => k.CCCD == SelectedKhachHang.CCCD))
                {
                    MessageBox.Show("Số CCCD này đã tồn tại trong hệ thống!", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                SelectedKhachHang.NgayTao = DateTime.Now;
                db.KhachHangs.Add(SelectedKhachHang);
                db.SaveChanges();

                MessageBox.Show("Thêm khách hàng thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                LoadData();
                ClearForm();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi hệ thống khi thêm dữ liệu: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ExecuteUpdate()
        {
            if (!ValidateForm()) return;

            try
            {
                var khInDb = db.KhachHangs.Find(SelectedKhachHang.MaKH);
                if (khInDb != null)
                {
                    khInDb.HoTen = SelectedKhachHang.HoTen;
                    khInDb.CCCD = SelectedKhachHang.CCCD;
                    khInDb.SDT = SelectedKhachHang.SDT;
                    khInDb.DiaChi = SelectedKhachHang.DiaChi;
                    khInDb.Email = SelectedKhachHang.Email;

                    db.SaveChanges();
                    MessageBox.Show("Cập nhật thông tin thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                    LoadData();
                    ClearForm();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi hệ thống khi cập nhật: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ExecuteDelete()
        {
            var result = MessageBox.Show("Bạn có chắc chắn muốn xóa khách hàng này không?", "Xác nhận xóa", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    var khInDb = db.KhachHangs.Find(SelectedKhachHang.MaKH);
                    if (khInDb != null)
                    {
                        db.KhachHangs.Remove(khInDb);
                        db.SaveChanges();
                        MessageBox.Show("Xóa khách hàng thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                        LoadData();
                        ClearForm();
                    }
                }
                catch (Exception)
                {
                    MessageBox.Show("Không thể xóa khách hàng này do đang có lịch sử đặt phòng liên kết!", "Lỗi liên kết dữ liệu", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void ClearForm()
        {
            SelectedKhachHang = new KhachHang();
        }

        private bool ValidateForm()
        {
            if (string.IsNullOrWhiteSpace(SelectedKhachHang.HoTen))
            {
                MessageBox.Show("Vui lòng điền Họ và Tên khách hàng!", "Dữ liệu không hợp lệ", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            if (string.IsNullOrWhiteSpace(SelectedKhachHang.CCCD) || SelectedKhachHang.CCCD.Length < 9 || SelectedKhachHang.CCCD.Length > 12)
            {
                MessageBox.Show("Số CCCD phải hợp lệ (từ 9 đến 12 ký tự số)!", "Dữ liệu không hợp lệ", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            if (string.IsNullOrWhiteSpace(SelectedKhachHang.SDT) || SelectedKhachHang.SDT.Length < 10 || SelectedKhachHang.SDT.Length > 11)
            {
                MessageBox.Show("Số điện thoại không đúng định dạng (10-11 số)!", "Dữ liệu không hợp lệ", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            return true;
        }
    }
}
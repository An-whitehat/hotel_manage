using QLKS.Commands;
using QLKS.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace QLKS.ViewModels
{
    public class RoomViewModel : BaseViewModel
    {
        public ObservableCollection<Phong> ListPhong { get; set; }
        public ObservableCollection<LoaiPhong> ListLoaiPhong { get; set; } // Dùng đổ vào ComboBox khi tạo phòng

        private Phong _selectedPhong;
        public Phong SelectedPhong
        {
            get => _selectedPhong;
            set
            {
                _selectedPhong = value;
                OnPropertyChanged();
                if (_selectedPhong != null)
                {
                    SoPhong = _selectedPhong.SoPhong;
                    SelectedLoaiPhong = ListLoaiPhong.FirstOrDefault(x => x.MaLoaiPhong == _selectedPhong.MaLoaiPhong);
                    TrangThai = _selectedPhong.TrangThai;
                    GhiChu = _selectedPhong.GhiChu;
                }
            }
        }

        private LoaiPhong _selectedLoaiPhong;
        public LoaiPhong SelectedLoaiPhong { get => _selectedLoaiPhong; set { _selectedLoaiPhong = value; OnPropertyChanged(); } }

        private string _soPhong;
        public string SoPhong { get => _soPhong; set { _soPhong = value; OnPropertyChanged(); } }

        private string _trangThai = "Trống";
        public string TrangThai { get => _trangThai; set { _trangThai = value; OnPropertyChanged(); } }

        private string _ghiChu;
        public string GhiChu { get => _ghiChu; set { _ghiChu = value; OnPropertyChanged(); } }

        public ObservableCollection<string> ListTrangThai { get; set; } = new ObservableCollection<string> { "Trống", "Đang thuê", "Đang dọn", "Bảo trì" };

        public ICommand AddRoomCmd { get; set; }
        public ICommand EditRoomCmd { get; set; }
        public ICommand DeleteRoomCmd { get; set; }

        public RoomViewModel()
        {
            LoadData();
            // Đã chuyển về RelayCommand chuẩn không dùng Generic như cấu hình dự án của m
            AddRoomCmd = new RelayCommand((p) => AddRoom());
            EditRoomCmd = new RelayCommand((p) => EditRoom(), (p) => SelectedPhong != null);
            DeleteRoomCmd = new RelayCommand((p) => DeleteRoom(), (p) => SelectedPhong != null);
        }

        private void LoadData()
        {
            using (var db = new HotelManagementEntities())
            {
                // Sử dụng Include để tải kèm thông tin Loại Phòng (Eager Loading)
                ListPhong = new ObservableCollection<Phong>(db.Phongs.Include("LoaiPhong").ToList());
                ListLoaiPhong = new ObservableCollection<LoaiPhong>(db.LoaiPhongs.ToList());
            }
        }

        private void AddRoom()
        {
            if (string.IsNullOrEmpty(SoPhong) || SelectedLoaiPhong == null) return;

            using (var db = new HotelManagementEntities())
            {
                if (db.Phongs.Any(x => x.SoPhong == SoPhong))
                {
                    MessageBox.Show("Số phòng này đã tồn tại!", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var p = new Phong { SoPhong = SoPhong, MaLoaiPhong = SelectedLoaiPhong.MaLoaiPhong, TrangThai = TrangThai, GhiChu = GhiChu };
                db.Phongs.Add(p);
                db.SaveChanges();
                LoadData();
                ClearForm();
            }
        }

        private void EditRoom()
        {
            using (var db = new HotelManagementEntities())
            {
                var p = db.Phongs.Find(SelectedPhong.MaPhong);
                if (p != null)
                {
                    p.SoPhong = SoPhong;
                    p.MaLoaiPhong = SelectedLoaiPhong.MaLoaiPhong;
                    p.TrangThai = TrangThai;
                    p.GhiChu = GhiChu;
                    db.SaveChanges();
                    LoadData();
                }
            }
        }

        private void DeleteRoom()
        {
            using (var db = new HotelManagementEntities())
            {
                var p = db.Phongs.Find(SelectedPhong.MaPhong);
                if (p != null)
                {
                    if (p.TrangThai == "Đang thuê")
                    {
                        MessageBox.Show("Không thể xóa phòng đang có khách ở!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                    db.Phongs.Remove(p);
                    db.SaveChanges();
                    LoadData();
                    ClearForm();
                }
            }
        }

        private void ClearForm() { SoPhong = ""; TrangThai = "Trống"; GhiChu = ""; SelectedLoaiPhong = null; SelectedPhong = null; }
    }
}
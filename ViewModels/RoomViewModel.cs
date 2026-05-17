using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
<<<<<<< HEAD
using System.Windows;
using System.Windows.Input;
using QLKS.Models;
using QLKS.Commands;
=======
using System.Windows.Input;
using QLKS.Models;
>>>>>>> 83c1e4f5a5338a02b6ac30b4f13eee94d7f15a74

namespace QLKS.ViewModels
{
    public class RoomViewModel : BaseViewModel
    {
<<<<<<< HEAD
        public ObservableCollection<Phong> ListPhong { get; set; }
        public ObservableCollection<LoaiPhong> ListLoaiPhong { get; set; } // Dùng đổ vào ComboBox khi tạo phòng
=======
        private HotelManagementEntities _db = new HotelManagementEntities();

        public ObservableCollection<Phong> ListPhong { get; set; }
        public ObservableCollection<LoaiPhong> ListLoaiPhongCombo { get; set; } // Dùng cho ComboBox chọn loại phòng
        public ObservableCollection<string> ListTrangThai { get; set; } // Trống, Đang thuê, Đang dọn, Bảo trì
>>>>>>> 83c1e4f5a5338a02b6ac30b4f13eee94d7f15a74

        private Phong _selectedPhong;
        public Phong SelectedPhong
        {
            get => _selectedPhong;
<<<<<<< HEAD
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
            AddRoomCmd = new RelayCommand<object>((p) => AddRoom());
            EditRoomCmd = new RelayCommand<object>((p) => EditRoom(), (p) => SelectedPhong != null);
            DeleteRoomCmd = new RelayCommand<object>((p) => DeleteRoom(), (p) => SelectedPhong != null);
        }

        private void LoadData()
        {
            using (var db = new HotelDbContext())
            {
                // Sử dụng Include để tải kèm thông tin Loại Phòng (Eager Loading)
                ListPhong = new ObservableCollection<Phong>(db.Phongs.Include("LoaiPhong").ToList());
                ListLoaiPhong = new ObservableCollection<LoaiPhong>(db.LoaiPhongs.ToList());
            }
        }

        private void AddRoom()
        {
            if (string.IsNullOrEmpty(SoPhong) || SelectedLoaiPhong == null) return;

            using (var db = new HotelDbContext())
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
            using (var db = new HotelDbContext())
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
            using (var db = new HotelDbContext())
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
}
=======
            set { _selectedPhong = value; OnPropertyChanged(); }
        }

        public ICommand AddPhongCmd { get; set; }
        public ICommand EditPhongCmd { get; set; }
        public ICommand DeletePhongCmd { get; set; }

        public RoomViewModel()
        {
            ListPhong = new ObservableCollection<Phong>(_db.Phongs.ToList());
            ListLoaiPhongCombo = new ObservableCollection<LoaiPhong>(_db.LoaiPhongs.ToList());
            ListTrangThai = new ObservableCollection<string> { "Trống", "Đang thuê", "Đang dọn", "Bảo trì" };

            AddPhongCmd = new RelayCommand<object>(p => true, p => {
                if (!ListLoaiPhongCombo.Any()) return;

                var pNew = new Phong
                {
                    SoPhong = "Pxxx",
                    MaLoaiPhong = ListLoaiPhongCombo.First().MaLoaiPhong,
                    TrangThai = "Trống",
                    NgayTao = System.DateTime.Now
                };
                _db.Phongs.Add(pNew);
                _db.SaveChanges();
                ListPhong.Add(pNew);
            });

            EditPhongCmd = new RelayCommand<object>(p => SelectedPhong != null, p => {
                _db.SaveChanges();
                MessageBox.Show("Cập nhật thông tin phòng thành công!");
            });

            DeletePhongCmd = new RelayCommand<object>(p => SelectedPhong != null, p => {
                if (MessageBox.Show("Xóa phòng này?", "Cảnh báo", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    _db.Phongs.Remove(SelectedPhong);
                    _db.SaveChanges();
                    ListPhong.Remove(SelectedPhong);
                }
            });
        }
    }
}
>>>>>>> 83c1e4f5a5338a02b6ac30b4f13eee94d7f15a74

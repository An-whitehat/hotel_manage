using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using QLKS.Models;

namespace QLKS.ViewModels
{
    public class RoomViewModel : BaseViewModel
    {
        private HotelManagementEntities _db = new HotelManagementEntities();

        public ObservableCollection<Phong> ListPhong { get; set; }
        public ObservableCollection<LoaiPhong> ListLoaiPhongCombo { get; set; } // Dùng cho ComboBox chọn loại phòng
        public ObservableCollection<string> ListTrangThai { get; set; } // Trống, Đang thuê, Đang dọn, Bảo trì

        private Phong _selectedPhong;
        public Phong SelectedPhong
        {
            get => _selectedPhong;
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

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows;
using System.Windows.Input;
using QLKS.Models;
using QLKS.Commands;
//using System.Net.NetworkInformation;

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
            set
            {
                _selectedPhong = value;
                OnPropertyChanged();
                if (_selectedPhong != null)
                {
                    SoPhong = _selectedPhong.SoPhong;
                    SelectedLoaiPhong = ListLoaiPhongCombo.FirstOrDefault(x => x.MaLoaiPhong == _selectedPhong.MaLoaiPhong);
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

        public ICommand AddPhongCmd { get; set; }
        public ICommand EditPhongCmd { get; set; }
        public ICommand DeletePhongCmd { get; set; }

        public RoomViewModel()
        {
            ListPhong = new ObservableCollection<Phong>(_db.Phongs.ToList());
            ListLoaiPhongCombo = new ObservableCollection<LoaiPhong>(_db.LoaiPhongs.ToList());
            ListTrangThai = new ObservableCollection<string> { "Trống", "Đang thuê", "Đang dọn", "Bảo trì" };
            SelectedLoaiPhong = ListLoaiPhongCombo.FirstOrDefault();

            AddPhongCmd = new RelayCommand(p => {
                var soPhong = SoPhong?.Trim();
                if (string.IsNullOrWhiteSpace(soPhong))
                {
                    MessageBox.Show("Vui lòng nhập số phòng.");
                    return;
                }

                if (SelectedLoaiPhong == null)
                {
                    MessageBox.Show("Vui lòng chọn loại phòng.");
                    return;
                }

                if (_db.Phongs.Any(x => x.SoPhong == soPhong))
                {
                    MessageBox.Show("Số phòng đã tồn tại. Vui lòng nhập số phòng khác.");
                    return;
                }

                var pNew = new Phong
                {
                    SoPhong = soPhong,
                    MaLoaiPhong = SelectedLoaiPhong.MaLoaiPhong,
                    TrangThai = string.IsNullOrWhiteSpace(TrangThai) ? "Trống" : TrangThai,
                    GhiChu = GhiChu,
                    NgayTao = DateTime.Now
                };

                _db.Phongs.Add(pNew);
                _db.SaveChanges();
                ListPhong.Add(pNew);
                SelectedPhong = pNew;
                MessageBox.Show("Thêm phòng thành công!");
            });

            EditPhongCmd = new RelayCommand(p => {
                if (SelectedPhong == null) return;
                var soPhong = SoPhong?.Trim();
                if (string.IsNullOrWhiteSpace(soPhong))
                {
                    MessageBox.Show("Vui lòng nhập số phòng.");
                    return;
                }

                if (SelectedLoaiPhong == null)
                {
                    MessageBox.Show("Vui lòng chọn loại phòng.");
                    return;
                }

                if (_db.Phongs.Any(x => x.SoPhong == soPhong && x.MaPhong != SelectedPhong.MaPhong))
                {
                    MessageBox.Show("Số phòng đã tồn tại. Vui lòng nhập số phòng khác.");
                    return;
                }

                // Gán giá trị từ form vào entity đang được EF track
                SelectedPhong.SoPhong = soPhong;
                SelectedPhong.MaLoaiPhong = SelectedLoaiPhong.MaLoaiPhong;
                SelectedPhong.TrangThai = TrangThai;
                SelectedPhong.GhiChu = GhiChu;

                _db.SaveChanges();
                MessageBox.Show("Cập nhật thông tin phòng thành công!");
            }, p => SelectedPhong != null);

            DeletePhongCmd = new RelayCommand(p => {
                if (MessageBox.Show("Xóa phòng này?", "Cảnh báo", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    var item = SelectedPhong;

                    _db.Phongs.Remove(item);
                    _db.SaveChanges();

                    ListPhong.Remove(item);

                    //_db.Phongs.Remove(SelectedPhong);
                    //_db.SaveChanges();
                    //ListPhong.Remove(SelectedPhong);
                }
            }, p => SelectedPhong != null);
        }
    }
}

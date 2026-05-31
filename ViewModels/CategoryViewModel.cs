using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using QLKS.Models;
using QLKS.Commands;


namespace QLKS.ViewModels
{
    public class CategoryViewModel : BaseViewModel
    {
        private HotelManagementEntities _db = new HotelManagementEntities();

        // Danh sách hiển thị lên UI
        public ObservableCollection<LoaiPhong> ListLoaiPhong { get; set; }
        public ObservableCollection<DichVu> ListDichVu { get; set; }

        // Đối tượng ràng buộc khi Chọn dòng trên DataGrid hoặc điền Form
        private LoaiPhong _selectedLoaiPhong;
        public LoaiPhong SelectedLoaiPhong
        {
            get => _selectedLoaiPhong;
            set
            {
                _selectedLoaiPhong = value;
                OnPropertyChanged();
                if (_selectedLoaiPhong != null)
                {
                    TenLoaiPhong = _selectedLoaiPhong.TenLoaiPhong;
                    GiaPhong = _selectedLoaiPhong.GiaPhong;
                    SoGiuong = _selectedLoaiPhong.SoGiuong;
                    MoTaLoaiPhong = _selectedLoaiPhong.MoTa;
                }
            }
        }

        private DichVu _selectedDichVu;
        public DichVu SelectedDichVu
        {
            get => _selectedDichVu;
            set
            {
                _selectedDichVu = value;
                OnPropertyChanged();
                if (_selectedDichVu != null)
                {
                    TenDichVu = _selectedDichVu.TenDichVu;
                    GiaDichVu = _selectedDichVu.GiaDichVu;
                    MoTaDichVu = _selectedDichVu.MoTa;
                }
            }
        }

        // Các thuộc tính Binding Form Nhập Liệu - Loại Phòng
        private string _tenLoaiPhong;
        public string TenLoaiPhong { get => _tenLoaiPhong; set { _tenLoaiPhong = value; OnPropertyChanged(); } }

        private decimal _giaPhong;
        public decimal GiaPhong { get => _giaPhong; set { _giaPhong = value; OnPropertyChanged(); } }

        private int _soGiuong;
        public int SoGiuong { get => _soGiuong; set { _soGiuong = value; OnPropertyChanged(); } }

        private string _moTaLoaiPhong;
        public string MoTaLoaiPhong { get => _moTaLoaiPhong; set { _moTaLoaiPhong = value; OnPropertyChanged(); } }

        // Các thuộc tính Binding Form Nhập Liệu - Dịch Vụ
        private string _tenDichVu;
        public string TenDichVu { get => _tenDichVu; set { _tenDichVu = value; OnPropertyChanged(); } }

        private decimal _giaDichVu;
        public decimal GiaDichVu { get => _giaDichVu; set { _giaDichVu = value; OnPropertyChanged(); } }

        private string _moTaDichVu;
        public string MoTaDichVu { get => _moTaDichVu; set { _moTaDichVu = value; OnPropertyChanged(); } }

        // Commands cho Loại Phòng
        public ICommand AddLoaiPhongCmd { get; set; }
        public ICommand EditLoaiPhongCmd { get; set; }
        public ICommand DeleteLoaiPhongCmd { get; set; }

        // Commands cho Dịch Vụ
        public ICommand AddDichVuCmd { get; set; }
        public ICommand EditDichVuCmd { get; set; }
        public ICommand DeleteDichVuCmd { get; set; }

        public CategoryViewModel()
        {
            LoadData();

            // Khởi tạo các Command CRUD Loại Phòng
            AddLoaiPhongCmd = new RelayCommand(p => {
                //var lp = new LoaiPhong { TenLoaiPhong = "Loại phòng mới", GiaPhong = 0, SoGiuong = 2, NgayTao = System.DateTime.Now };
                var lp = new LoaiPhong
                {
                    TenLoaiPhong = TenLoaiPhong,
                    GiaPhong = GiaPhong,
                    SoGiuong = SoGiuong,
                    MoTa = MoTaLoaiPhong,
                    NgayTao = DateTime.Now
                };
                _db.LoaiPhongs.Add(lp);
                _db.SaveChanges();
                ListLoaiPhong.Add(lp);
            });

            EditLoaiPhongCmd = new RelayCommand(p => {
                if (SelectedLoaiPhong == null) return;

                SelectedLoaiPhong.TenLoaiPhong = TenLoaiPhong;
                SelectedLoaiPhong.GiaPhong = GiaPhong;
                SelectedLoaiPhong.SoGiuong = SoGiuong;
                SelectedLoaiPhong.MoTa = MoTaLoaiPhong;

                _db.SaveChanges();
                MessageBox.Show("Cập nhật loại phòng thành công!");
            }, p => SelectedLoaiPhong != null);

            EditDichVuCmd = new RelayCommand(p => {
                if (SelectedDichVu == null) return;

                SelectedDichVu.TenDichVu = TenDichVu;
                SelectedDichVu.GiaDichVu = GiaDichVu;
                SelectedDichVu.MoTa = MoTaDichVu;

                _db.SaveChanges();
                MessageBox.Show("Cập nhật dịch vụ thành công!");
            }, p => SelectedDichVu != null);

            DeleteLoaiPhongCmd = new RelayCommand(p => {
                if (MessageBox.Show("Bạn có chắc chắn muốn xóa?", "Xác nhận", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    if (_db.Phongs.Any(x => x.MaLoaiPhong == SelectedLoaiPhong.MaLoaiPhong))
                    {
                        MessageBox.Show("Loại phòng đang được sử dụng!");
                        return;
                    }
                    var item = SelectedLoaiPhong;

                    _db.LoaiPhongs.Remove(item);
                    _db.SaveChanges();

                    ListLoaiPhong.Remove(item);

                    //_db.LoaiPhongs.Remove(SelectedLoaiPhong);
                    //_db.SaveChanges();
                    //ListLoaiPhong.Remove(SelectedLoaiPhong);
                }
            }, p => SelectedLoaiPhong != null);

            AddDichVuCmd = new RelayCommand(p => {
                //var dv = new DichVu { TenDichVu = "Dịch vụ mới", GiaDichVu = 0, NgayTao = System.DateTime.Now };
                var dv = new DichVu
                {
                    TenDichVu = TenDichVu,
                    GiaDichVu = GiaDichVu,
                    MoTa = MoTaDichVu,
                    NgayTao = DateTime.Now
                };
                _db.DichVus.Add(dv);
                _db.SaveChanges();
                ListDichVu.Add(dv);
            });

            DeleteDichVuCmd = new RelayCommand(p => {
                if (MessageBox.Show("Bạn có chắc muốn xóa dịch vụ này?", "Xác nhận", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    var item = SelectedDichVu;

                    _db.DichVus.Remove(item);
                    _db.SaveChanges();

                    ListLoaiPhong.Remove(item);

                    //_db.DichVus.Remove(SelectedDichVu);
                    //_db.SaveChanges();
                    //ListDichVu.Remove(SelectedDichVu);
                }
            }, p => SelectedDichVu != null);
        }

        private void LoadData()
        {
            ListLoaiPhong = new ObservableCollection<LoaiPhong>(_db.LoaiPhongs.ToList());
            ListDichVu = new ObservableCollection<DichVu>(_db.DichVus.ToList());
        }
    }
}
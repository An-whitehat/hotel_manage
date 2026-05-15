using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using QLKS.Models;

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
            set { _selectedLoaiPhong = value; OnPropertyChanged(); }
        }

        private DichVu _selectedDichVu;
        public DichVu SelectedDichVu
        {
            get => _selectedDichVu;
            set { _selectedDichVu = value; OnPropertyChanged(); }
        }

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
            AddLoaiPhongCmd = new RelayCommand<object>(p => true, p => {
                var lp = new LoaiPhong { TenLoaiPhong = "Loại phòng mới", GiaPhong = 0, SoGiuong = 2, NgayTao = System.DateTime.Now };
                _db.LoaiPhongs.Add(lp);
                _db.SaveChanges();
                ListLoaiPhong.Add(lp);
            });

            EditLoaiPhongCmd = new RelayCommand<object>(p => SelectedLoaiPhong != null, p => {
                _db.SaveChanges();
                MessageBox.Show("Cập nhật loại phòng thành công!");
            });

            DeleteLoaiPhongCmd = new RelayCommand<object>(p => SelectedLoaiPhong != null, p => {
                if (MessageBox.Show("Bạn có chắc chắn muốn xóa?", "Xác nhận", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    _db.LoaiPhongs.Remove(SelectedLoaiPhong);
                    _db.SaveChanges();
                    ListLoaiPhong.Remove(SelectedLoaiPhong);
                }
            });

            // Khởi tạo các Command CRUD Dịch Vụ
            AddDichVuCmd = new RelayCommand<object>(p => true, p => {
                var dv = new DichVu { TenDichVu = "Dịch vụ mới", GiaDichVu = 0, NgayTao = System.DateTime.Now };
                _db.DichVus.Add(dv);
                _db.SaveChanges();
                ListDichVu.Add(dv);
            });

            EditDichVuCmd = new RelayCommand<object>(p => SelectedDichVu != null, p => {
                _db.SaveChanges();
                MessageBox.Show("Cập nhật dịch vụ thành công!");
            });

            DeleteDichVuCmd = new RelayCommand<object>(p => SelectedDichVu != null, p => {
                if (MessageBox.Show("Bạn có chắc muốn xóa dịch vụ này?", "Xác nhận", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    _db.DichVus.Remove(SelectedDichVu);
                    _db.SaveChanges();
                    ListDichVu.Remove(SelectedDichVu);
                }
            });
        }

        private void LoadData()
        {
            ListLoaiPhong = new ObservableCollection<LoaiPhong>(_db.LoaiPhongs.ToList());
            ListDichVu = new ObservableCollection<DichVu>(_db.DichVus.ToList());
        }
    }
}

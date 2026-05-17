using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using QLKS.Models;
using QLKS.Commands;

namespace QLKS.ViewModels
{
    public class CategoryViewModel : BaseViewModel
    {
        public ObservableCollection<LoaiPhong> ListLoaiPhong { get; set; }
        public ObservableCollection<DichVu> ListDichVu { get; set; }

        // Đối tượng đang được chọn trên DataGrid
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

        // Commands hành động
        public ICommand AddLoaiPhongCmd { get; set; }
        public ICommand EditLoaiPhongCmd { get; set; }
        public ICommand DeleteLoaiPhongCmd { get; set; }

        public ICommand AddDichVuCmd { get; set; }
        public ICommand EditDichVuCmd { get; set; }
        public ICommand DeleteDichVuCmd { get; set; }

        public CategoryViewModel()
        {
            LoadData();

            // Khởi tạo Commands cho Loại Phòng
            AddLoaiPhongCmd = new RelayCommand<object>((p) => AddLoaiPhong());
            EditLoaiPhongCmd = new RelayCommand<object>((p) => EditLoaiPhong(), (p) => SelectedLoaiPhong != null);
            DeleteLoaiPhongCmd = new RelayCommand<object>((p) => DeleteLoaiPhong(), (p) => SelectedLoaiPhong != null);

            // Khởi tạo Commands cho Dịch Vụ
            AddDichVuCmd = new RelayCommand<object>((p) => AddDichVu());
            EditDichVuCmd = new RelayCommand<object>((p) => EditDichVu(), (p) => SelectedDichVu != null);
            DeleteDichVuCmd = new RelayCommand<object>((p) => DeleteDichVu(), (p) => SelectedDichVu != null);
        }

        private void LoadData()
        {
            using (var db = new HotelDbContext())
            {
                ListLoaiPhong = new ObservableCollection<LoaiPhong>(db.LoaiPhongs.ToList());
                ListDichVu = new ObservableCollection<DichVu>(db.DichVus.ToList());
            }
        }

        // ---- NGHIỆP VỤ LOẠI PHÒNG ----
        private void AddLoaiPhong()
        {
            if (string.IsNullOrEmpty(TenLoaiPhong)) return;
            using (var db = new HotelDbContext())
            {
                var lp = new LoaiPhong { TenLoaiPhong = TenLoaiPhong, GiaPhong = GiaPhong, SoGiuong = SoGiuong, MoTa = MoTaLoaiPhong };
                db.LoaiPhongs.Add(lp);
                db.SaveChanges();
                ListLoaiPhong.Add(lp);
                ClearLoaiPhongForm();
            }
        }

        private void EditLoaiPhong()
        {
            using (var db = new HotelDbContext())
            {
                var lp = db.LoaiPhongs.Find(SelectedLoaiPhong.MaLoaiPhong);
                if (lp != null)
                {
                    lp.TenLoaiPhong = TenLoaiPhong;
                    lp.GiaPhong = GiaPhong;
                    lp.SoGiuong = SoGiuong;
                    lp.MoTa = MoTaLoaiPhong;
                    db.SaveChanges();
                    LoadData();
                }
            }
        }

        private void DeleteLoaiPhong()
        {
            var res = MessageBox.Show("Bạn có chắc chắn muốn xóa loại phòng này không?", "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (res == MessageBoxResult.Yes)
            {
                using (var db = new HotelDbContext())
                {
                    var lp = db.LoaiPhongs.Find(SelectedLoaiPhong.MaLoaiPhong);
                    if (lp != null)
                    {
                        // Kiểm tra xem có phòng nào đang thuộc loại phòng này không để tránh lỗi khóa ngoại
                        if (db.Phongs.Any(x => x.MaLoaiPhong == lp.MaLoaiPhong))
                        {
                            MessageBox.Show("Không thể xóa! Loại phòng này đang được gán cho các phòng thực tế.", "Lỗi dữ liệu", MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }
                        db.LoaiPhongs.Remove(lp);
                        db.SaveChanges();
                        ListLoaiPhong.Remove(SelectedLoaiPhong);
                        ClearLoaiPhongForm();
                    }
                }
            }
        }

        // ---- NGHIỆP VỤ DỊCH VỤ ----
        private void AddDichVu()
        {
            if (string.IsNullOrEmpty(TenDichVu)) return;
            using (var db = new HotelDbContext())
            {
                var dv = new DichVu { TenDichVu = TenDichVu, GiaDichVu = GiaDichVu, MoTa = MoTaDichVu };
                db.DichVus.Add(dv);
                db.SaveChanges();
                ListDichVu.Add(dv);
                ClearDichVuForm();
            }
        }

        private void EditDichVu()
        {
            using (var db = new HotelDbContext())
            {
                var dv = db.DichVus.Find(SelectedDichVu.MaDichVu);
                if (dv != null)
                {
                    dv.TenDichVu = TenDichVu;
                    dv.GiaDichVu = GiaDichVu;
                    dv.MoTa = MoTaDichVu;
                    db.SaveChanges();
                    LoadData();
                }
            }
        }

        private void DeleteDichVu()
        {
            using (var db = new HotelDbContext())
            {
                var dv = db.DichVus.Find(SelectedDichVu.MaDichVu);
                if (dv != null)
                {
                    db.DichVus.Remove(dv);
                    db.SaveChanges();
                    ListDichVu.Remove(SelectedDichVu);
                    ClearDichVuForm();
                }
            }
        }

        private void ClearLoaiPhongForm() { TenLoaiPhong = ""; GiaPhong = 0; SoGiuong = 2; MoTaLoaiPhong = ""; SelectedLoaiPhong = null; }
        private void ClearDichVuForm() { TenDichVu = ""; GiaDichVu = 0; MoTaDichVu = ""; SelectedDichVu = null; }
    }
}

using QLKS.Commands;
using QLKS.Models;
using System.Collections.ObjectModel;
using System.Linq;
using System.Collections.Generic;

namespace QLKS.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        // ─── Current page being displayed ───────────────────────────────────
        private BaseViewModel _currentViewModel;
        public BaseViewModel CurrentViewModel
        {
            get => _currentViewModel;
            set { _currentViewModel = value; OnPropertyChanged(); }
        }

        // ─── Active nav item (để highlight button đang chọn) ────────────────
        private string _currentPage;
        public string CurrentPage
        {
            get => _currentPage;
            set { _currentPage = value; OnPropertyChanged(); }
        }

        // ─── Title hiển thị trên TopBar ─────────────────────────────────────
        public string PageTitle
        {
            get
            {
                switch (CurrentPage)
                {
                    case "Dashboard": return "Dashboard";
                    case "FloorPlan": return "Sơ đồ phòng";
                    case "Phong": return "Quản lý phòng";
                    case "KhachHang": return "Khách hàng";
                    case "DatPhong": return "Đặt phòng";
                    case "HoaDon": return "Hóa đơn";
                    case "ThongKe": return "Thống kê";
                    case "BaoCao": return "Báo cáo";
                    case "NhanVien": return "Nhân viên";
                    case "CauHinh": return "Cấu hình";
                    case "Search": return "Tìm kiếm";
                    default: return "";
                }
            }
        }

        // ─── Thông tin user đang đăng nhập ──────────────────────────────────
        private string _currentUser;
        public string CurrentUser
        {
            get => _currentUser;
            set { _currentUser = value; OnPropertyChanged(); }
        }

        private bool _isAdmin;
        public bool IsAdmin
        {
            get => _isAdmin;
            set { _isAdmin = value; OnPropertyChanged(); }
        }

        // ─── Navigation Commands ─────────────────────────────────────────────
        public RelayCommand NavigateDashboardCommand { get; }
        public RelayCommand NavigateFloorPlanCommand { get; }
        public RelayCommand NavigatePhongCommand { get; }
        public RelayCommand NavigateKhachHangCommand { get; }
        public RelayCommand NavigateDatPhongCommand { get; }
        public RelayCommand NavigateHoaDonCommand { get; }
        public RelayCommand NavigateThongKeCommand { get; }
        public RelayCommand NavigateBaoCaoCommand { get; }
        public RelayCommand NavigateNhanVienCommand { get; }
        public RelayCommand NavigateCauHinhCommand { get; }
        public RelayCommand NavigateSearchCommand { get; }
        public RelayCommand DangXuatCommand { get; }
        // Không tham số
        public MainViewModel() : this("Admin", true) { }

        // ─── Constructor ─────────────────────────────────────────────────────
        public MainViewModel(string username = "Admin", bool isAdmin = true)
        {
            CurrentUser = username;
            IsAdmin = isAdmin;

            // Khởi động mở trang Dashboard
            //NavigateTo("Dashboard", new DashboardViewModel());

            //// Gán commands
            //NavigateDashboardCommand = new RelayCommand(_ => NavigateTo("Dashboard", new DashboardViewModel()));
            //NavigateFloorPlanCommand = new RelayCommand(_ => NavigateTo("FloorPlan", new FloorPlanViewModel()));
            //NavigatePhongCommand = new RelayCommand(_ => NavigateTo("Phong", new PhongViewModel()));
            //NavigateKhachHangCommand = new RelayCommand(_ => NavigateTo("KhachHang", new KhachHangViewModel()));
            //NavigateDatPhongCommand = new RelayCommand(_ => NavigateTo("DatPhong", new DatPhongViewModel()));
            //NavigateHoaDonCommand = new RelayCommand(_ => NavigateTo("HoaDon", new HoaDonViewModel()));
            //NavigateThongKeCommand = new RelayCommand(_ => NavigateTo("ThongKe", new ThongKeViewModel()));
            //NavigateSearchCommand = new RelayCommand(_ => NavigateTo("Search", new SearchViewModel()));

            //// Phân quyền: chỉ Admin mới vào được Báo cáo & Nhân viên
            //NavigateBaoCaoCommand = new RelayCommand(
            //    _ => NavigateTo("BaoCao", new BaoCaoViewModel()),
            //    _ => IsAdmin);

            //NavigateNhanVienCommand = new RelayCommand(
            //    _ => NavigateTo("NhanVien", new NhanVienViewModel()),
            //    _ => IsAdmin);

            //NavigateCauHinhCommand = new RelayCommand(_ => NavigateTo("CauHinh", new CauHinhViewModel()));
            //DangXuatCommand = new RelayCommand(_ => DangXuat());
        }

        // ─── Helpers ─────────────────────────────────────────────────────────

        /// <summary>
        /// Đổi trang: cập nhật CurrentPage + CurrentViewModel + PageTitle cùng lúc.
        /// </summary>
        private void NavigateTo(string page, BaseViewModel vm)
        {
            CurrentPage = page;
            CurrentViewModel = vm;
            OnPropertyChanged(nameof(PageTitle)); // cập nhật TopBar title
        }

        //private void DangXuat()
        //{
        //    // Mở lại LoginWindow
        //    var loginWindow = new QLKS.Views.LoginWindow();
        //    loginWindow.Show();

        //    // Đóng MainLayout hiện tại
        //    foreach (System.Windows.Window w in System.Windows.Application.Current.Windows)
        //    {
        //        if (w is QLKS.Views.MainLayout)
        //        {
        //            w.Close();
        //            break;
        //        }
        //    }
        //}
    }
}
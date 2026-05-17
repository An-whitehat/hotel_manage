using QLKS.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QLKS.ViewModels
{
    public class MainLayoutViewModel
    {
        public class MainViewModel : BaseViewModel
        {
            // ─── Current page being displayed ───────────────────────────────────
            private BaseViewModel _currentViewModel;
            public BaseViewModel CurrentViewModel
            {
                get => _currentViewModel;
                set
                {
                    _currentViewModel = value;
                    OnPropertyChanged();
                }
            }

            // ─── Active nav item (để highlight button đang chọn) ────────────────
            private string _currentPage;
            public string CurrentPage
            {
                get => _currentPage;
                set
                {
                    _currentPage = value;
                    OnPropertyChanged();
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
            public RelayCommand NavigatePhongCommand { get; }
            public RelayCommand NavigateKhachHangCommand { get; }
            public RelayCommand NavigateDatPhongCommand { get; }
            public RelayCommand NavigateHoaDonCommand { get; }
            public RelayCommand NavigateThongKeCommand { get; }
            public RelayCommand NavigateBaoCaoCommand { get; }
            public RelayCommand NavigateNhanVienCommand { get; }
            public RelayCommand NavigateCauHinhCommand { get; }
            public RelayCommand DangXuatCommand { get; }

            // ─── Constructor ─────────────────────────────────────────────────────
            //public MainViewModel(string username = "Admin", bool isAdmin = true)
            //{
            //    CurrentUser = username;
            //    IsAdmin = isAdmin;

            //    // Khởi động mở trang Dashboard
            //    NavigateTo("Dashboard", new DashboardViewModel());

            //    // Gán commands
            //    NavigateDashboardCommand = new RelayCommand(_ => NavigateTo("Dashboard", new DashboardViewModel()));
            //    NavigatePhongCommand = new RelayCommand(_ => NavigateTo("Phong", new PhongViewModel()));
            //    NavigateKhachHangCommand = new RelayCommand(_ => NavigateTo("KhachHang", new KhachHangViewModel()));
            //    NavigateDatPhongCommand = new RelayCommand(_ => NavigateTo("DatPhong", new DatPhongViewModel()));
            //    NavigateHoaDonCommand = new RelayCommand(_ => NavigateTo("HoaDon", new HoaDonViewModel()));
            //    NavigateThongKeCommand = new RelayCommand(_ => NavigateTo("ThongKe", new ThongKeViewModel()));

            //    // Phân quyền: chỉ Admin mới vào được Báo cáo & Nhân viên
            //    NavigateBaoCaoCommand = new RelayCommand(
            //        _ => NavigateTo("BaoCao", new BaoCaoViewModel()),
            //        _ => IsAdmin);

            //    NavigateNhanVienCommand = new RelayCommand(
            //        _ => NavigateTo("NhanVien", new NhanVienViewModel()),
            //        _ => IsAdmin);

            //    NavigateCauHinhCommand = new RelayCommand(_ => NavigateTo("CauHinh", new CauHinhViewModel()));

            //    DangXuatCommand = new RelayCommand(_ => DangXuat());
            //}

            // ─── Helpers ─────────────────────────────────────────────────────────

            /// <summary>
            /// Đổi trang: cập nhật CurrentPage (để sidebar highlight)
            /// và CurrentViewModel (để ContentControl render đúng View).
            /// </summary>
            //private void NavigateTo(string page, BaseViewModel vm)
            //{
            //    CurrentPage = page;
            //    CurrentViewModel = vm;
            //}

            //private void DangXuat()
            //{
            //    // Mở lại LoginWindow, đóng MainWindow
            //    var loginWindow = new QLKS.Views.LoginWindow();
            //    loginWindow.Show();

            //    // Đóng MainWindow hiện tại
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
}

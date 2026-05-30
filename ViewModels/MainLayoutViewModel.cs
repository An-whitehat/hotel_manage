using QLKS.Commands;
using QLKS.Services;
using System.Windows;

namespace QLKS.ViewModels
{
    public class MainLayoutViewModel : BaseViewModel
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
        private string _currentPageTitle = "Trang chủ";
        public string CurrentPageTitle
        {
            get => _currentPageTitle;
            set { _currentPageTitle = value; OnPropertyChanged(); }
        }

        // ─── Thông tin user đang đăng nhập ──────────────────────────────────
        public string CurrentUser => SessionService.CurrentUser?.HoTen ?? "Admin";
        public string VaiTro => SessionService.CurrentUser?.VaiTro ?? "";
        public bool IsAdmin => SessionService.HasRole("Admin", "QuanLy");

        // ─── Navigation Command (dùng CommandParameter từ XAML) ─────────────
        public RelayCommand NavigateCommand { get; }
        public RelayCommand DangXuatCommand { get; }

        // ─── Constructor ─────────────────────────────────────────────────────
        public MainLayoutViewModel()
        {
            NavigateCommand = new RelayCommand(param => HandleNavigate(param?.ToString()));
            DangXuatCommand = new RelayCommand(_ => DangXuat());

            // Mở trang mặc định
            HandleNavigate("Staff");
        }

        // ─── Router điều hướng ───────────────────────────────────────────────
        private void HandleNavigate(string page)
        {
            switch (page)
            {
                case "Staff":
                    NavigateTo(page, "Nhân viên", new StaffViewModel());
                    break;
                case "Booking":
                    NavigateTo(page, "Đặt phòng", new BookingViewModel());
                    break;
                case "FloorPlan":
                    NavigateTo(page, "Sơ đồ phòng", new FloorPlanViewModel());
                    break;
                case "Search":
                    NavigateTo(page, "Tìm kiếm", new SearchViewModel());
                    break;
                case "Invoice":
                    NavigateTo(page, "Hóa đơn", new InvoiceCheckoutViewModel());
                    break;
                case "BaoCao":
                case "Report":
                    if (!IsAdmin)
                    {
                        MessageBox.Show("Chỉ Admin hoặc Quản lý mới xem được báo cáo.", "Phân quyền", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }
                    NavigateTo(page, "Báo cáo doanh thu", new BaoCaoViewModel());
                    break;
                // Các view chưa có ViewModel → bỏ qua hoặc thông báo
                case "Dashboard":
                    NavigateTo(page, "Trang chủ", null);
                    break;
                case "Room":
                    NavigateTo(page, "Quản lý phòng", new RoomViewModel());
                    break;
                case "Revenue":
                    if (!IsAdmin)
                    {
                        MessageBox.Show("Chỉ Admin hoặc Quản lý mới xem được.", "Phân quyền", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }
                    NavigateTo(page, "Thống kê doanh thu", new BaoCaoViewModel());
                    break;
                default:
                    break;
            }
        }

        // ─── Helper ──────────────────────────────────────────────────────────
        private void NavigateTo(string page, string title, BaseViewModel vm)
        {
            CurrentPage = page;
            CurrentPageTitle = title;
            CurrentViewModel = vm;
        }

        private void DangXuat()
        {
            SessionService.Logout();

            var loginWindow = new MainWindow(); // hoặc tên Window khởi động app
            loginWindow.Show();

            foreach (System.Windows.Window w in System.Windows.Application.Current.Windows)
            {
                if (w is QLKS.Views.MainLayout)
                {
                    w.Close();
                    break;
                }
            }
        }
    }
}
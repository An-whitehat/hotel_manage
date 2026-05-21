using QLKS.Commands;
using QLKS.Services;
using System;
using System.Windows;
using System.Windows.Input;

namespace QLKS.ViewModels
{
    public class ShellViewModel : BaseViewModel
    {
        private BaseViewModel _currentViewModel;
        public BaseViewModel CurrentViewModel { get => _currentViewModel; set => SetProperty(ref _currentViewModel, value); }

        private string _title = "Đăng nhập";
        public string Title { get => _title; set => SetProperty(ref _title, value); }

        private string _currentUserText = "Chưa đăng nhập";
        public string CurrentUserText { get => _currentUserText; set => SetProperty(ref _currentUserText, value); }

        public ICommand ShowStaffCommand { get; }
        public ICommand ShowBookingCommand { get; }
        public ICommand ShowInvoiceCommand { get; }
        public ICommand LogoutCommand { get; }

        public ShellViewModel()
        {
            ShowStaffCommand = new RelayCommand(_ => ShowStaff());
            ShowBookingCommand = new RelayCommand(_ => ShowBooking());
            ShowInvoiceCommand = new RelayCommand(_ => ShowInvoice());
            LogoutCommand = new RelayCommand(_ => Logout());
            ShowLogin();
        }

        private void ShowLogin()
        {
            CurrentViewModel = new LoginViewModel(() =>
            {
                CurrentUserText = $"{SessionService.CurrentUser.HoTen} ({SessionService.CurrentUser.VaiTro})";
                ShowStaff();
            });
            Title = "Đăng nhập hệ thống";
        }

        private void ShowStaff()
        {
            if (!RequireLogin()) return;
            CurrentViewModel = new StaffViewModel();
            Title = "Quản lý nhân viên + ca làm việc";
        }

        private void ShowBooking()
        {
            if (!RequireLogin()) return;
            CurrentViewModel = new BookingViewModel();
            Title = "Đặt phòng + Check-in";
        }

        private void ShowInvoice()
        {
            if (!RequireLogin()) return;
            CurrentViewModel = new InvoiceCheckoutViewModel();
            Title = "Hóa đơn thanh toán + Check-out";
        }

        private bool RequireLogin()
        {
            if (SessionService.IsLoggedIn) return true;
            MessageBox.Show("Vui lòng đăng nhập trước.");
            ShowLogin();
            return false;
        }

        private void Logout()
        {
            SessionService.Logout();
            CurrentUserText = "Chưa đăng nhập";
            ShowLogin();
        }
    }
}

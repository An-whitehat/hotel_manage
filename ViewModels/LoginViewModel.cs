using QLKS.Commands;
using QLKS.Services;
using System;
using System.Windows;
using System.Windows.Input;

namespace QLKS.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        private readonly Action _loginSuccess;
        private string _username;
        public string Username { get => _username; set => SetProperty(ref _username, value); }

        private string _password;
        public string Password { get => _password; set => SetProperty(ref _password, value); }

        public ICommand LoginCommand { get; }
        public ICommand DemoAdminCommand { get; }
        public ICommand DemoReceptionistCommand { get; }
        public ICommand DemoManagerCommand { get; }

        public LoginViewModel(Action loginSuccess)
        {
            _loginSuccess = loginSuccess;
            LoginCommand = new RelayCommand(_ => Login());
            DemoAdminCommand = new RelayCommand(_ => { Username = "admin"; Password = "123456"; });
            DemoReceptionistCommand = new RelayCommand(_ => { Username = "letan01"; Password = "123456"; });
            DemoManagerCommand = new RelayCommand(_ => { Username = "quanly01"; Password = "123456"; });
        }

        private void Login()
        {
            if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password))
            {
                MessageBox.Show("Vui lòng nhập username và password.");
                return;
            }
            if (SessionService.Login(Username.Trim(), Password.Trim(), out var error))
            {
                _loginSuccess?.Invoke();
            }
            else MessageBox.Show(error);
        }
    }
}

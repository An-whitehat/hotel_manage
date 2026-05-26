using System.Windows;

namespace QLKS.Views
{
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
            DataContext = new QLKS.ViewModels.LoginViewModel(() =>
            {
                // Mở MainLayout
                var main = new MainLayout();
                main.Show();

                // Đóng LoginWindow
                this.Close();
            });
        }
    }
}
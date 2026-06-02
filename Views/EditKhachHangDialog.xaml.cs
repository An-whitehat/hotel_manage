using QLKS.Models;
using System.Windows;

namespace QLKS.Views
{
    public partial class EditKhachHangDialog : Window
    {
        // Các property để SearchViewModel đọc sau khi dialog đóng
        public string HoTen  => TxtHoTen.Text.Trim();
        public string CCCD   => TxtCCCD.Text.Trim();
        public string SDT    => TxtSDT.Text.Trim();
        public string DiaChi => TxtDiaChi.Text.Trim();
        public string Email  => TxtEmail.Text.Trim();

        public EditKhachHangDialog(KhachHang kh)
        {
            InitializeComponent();

            // Điền sẵn thông tin hiện tại
            TxtHoTen.Text  = kh.HoTen  ?? string.Empty;
            TxtCCCD.Text   = kh.CCCD   ?? string.Empty;
            TxtSDT.Text    = kh.SDT    ?? string.Empty;
            TxtDiaChi.Text = kh.DiaChi ?? string.Empty;
            TxtEmail.Text  = kh.Email  ?? string.Empty;
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            // Validate cơ bản trước khi đóng
            if (string.IsNullOrWhiteSpace(HoTen))
            {
                MessageBox.Show("Vui lòng điền Họ và Tên!", "Dữ liệu không hợp lệ",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                TxtHoTen.Focus();
                return;
            }
            if (string.IsNullOrWhiteSpace(CCCD) || CCCD.Length < 9 || CCCD.Length > 12)
            {
                MessageBox.Show("Số CCCD phải hợp lệ (9–12 ký tự)!", "Dữ liệu không hợp lệ",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                TxtCCCD.Focus();
                return;
            }
            if (string.IsNullOrWhiteSpace(SDT) || SDT.Length < 10 || SDT.Length > 11)
            {
                MessageBox.Show("Số điện thoại không đúng định dạng (10–11 số)!", "Dữ liệu không hợp lệ",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                TxtSDT.Focus();
                return;
            }

            DialogResult = true;
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}

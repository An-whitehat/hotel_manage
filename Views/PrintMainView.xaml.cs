using QLKS.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace QLKS.Views
{
    /// <summary>
    /// Interaction logic for PrintMainView.xaml
    /// Code-behind chỉ làm 1 việc: wire PrintAction delegates vào ViewModel
    /// sau khi InitializeComponent hoàn tất (lúc này GuestReport và EmployeeReport đã tồn tại).
    /// </summary>
    public partial class PrintMainView : UserControl
    {
        public PrintMainView()
        {
            InitializeComponent();

            // Sau khi DataTemplate tạo ViewModel qua MainLayout, DataContext đã được set.
            // Dùng Loaded để chắc chắn ViewModel đã sẵn sàng.
            this.Loaded += (_, __) => WireCommands();
        }

        private void WireCommands()
        {
            if (DataContext is PrintMainViewModel vm)
            {
                vm.PrintGuestAction    = () => GuestReport?.ExecutePrint();
                vm.PrintEmployeeAction = () => EmployeeReport?.ExecutePrint();

                // Refresh CanExecute sau khi delegate được gán
                System.Windows.Input.CommandManager.InvalidateRequerySuggested();
            }
        }

        // Giữ lại 2 handler cũ để tương thích nếu có chỗ nào còn gọi
        private void PrintGuest_Click(object sender, RoutedEventArgs e)
            => GuestReport?.ExecutePrint();

        private void PrintEmployee_Click(object sender, RoutedEventArgs e)
            => EmployeeReport?.ExecutePrint();
    }
}

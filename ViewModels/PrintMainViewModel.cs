using QLKS.Commands;
using System.Windows;
using System.Windows.Input;

namespace QLKS.ViewModels
{
    public class PrintMainViewModel : BaseViewModel
    {
        // Commands để XAML bind vào nút In (thay vì dùng code-behind Click event)
        public ICommand PrintGuestCommand    { get; }
        public ICommand PrintEmployeeCommand { get; }

        // ViewModel giữ reference tới các sub-view thông qua delegate
        // (View code-behind sẽ set delegate này khi InitializeComponent xong)
        public System.Action PrintGuestAction    { get; set; }
        public System.Action PrintEmployeeAction { get; set; }

        public PrintMainViewModel()
        {
            PrintGuestCommand    = new RelayCommand(_ => PrintGuestAction?.Invoke(),
                                                   _ => PrintGuestAction != null);
            PrintEmployeeCommand = new RelayCommand(_ => PrintEmployeeAction?.Invoke(),
                                                   _ => PrintEmployeeAction != null);
        }
    }
}

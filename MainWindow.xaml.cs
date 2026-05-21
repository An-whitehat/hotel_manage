using System.Windows;
using QLKS.ViewModels;

namespace QLKS
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            ShowLogin();
        }

        private void ShowLogin()
        {
            MainContent.Content = new LoginView
            {
                DataContext = new LoginViewModel(() =>
                {
                    MainContent.Content = new StaffView
                    {
                        DataContext = new StaffViewModel()
                    };
                })
            };
        }

        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            ShowLogin();
        }

        private void BtnStaff_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new StaffView
            {
                DataContext = new StaffViewModel()
            };
        }

        private void BtnBooking_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new BookingView
            {
                DataContext = new BookingViewModel()
            };
        }

        private void BtnInvoice_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new InvoiceCheckoutView
            {
                DataContext = new InvoiceCheckoutViewModel()
            };
        }
    }
}
using System.Windows.Controls;
using QLKS.ViewModels;

namespace QLKS.Views
{
    public partial class KhachHangView : UserControl
    {
        public KhachHangView()
        {
            InitializeComponent();

            // Ép giao diện phải kết nối bộ não của nó với file ViewModel
            this.DataContext = new KhachHangViewModel();
        }
    }
}
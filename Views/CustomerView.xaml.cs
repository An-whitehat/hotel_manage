using System.Windows.Controls;
using QLKS.ViewModels;

namespace QLKS.Views
{
    public partial class CustomerView : UserControl
    {
        public CustomerView()
        {
            InitializeComponent();

            this.DataContext = new QLKS.ViewModels.CustomerViewModel();
        }
    }
}
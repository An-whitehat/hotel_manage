using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.ReportAppServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace QLKS.Views
{
    /// <summary>
    /// Interaction logic for ReportPhongWindow.xaml
    /// </summary>
    public partial class ReportPhongWindow : Window
    {
        public ReportPhongWindow()
        {
            InitializeComponent();
        }
        private void btn_showreport_Click(object sender, RoutedEventArgs e)
        {
            ReportDocument rpt = new Report.DanhSachPhong_TheoLoai();

            rpt.SetDatabaseLogon("", "", "LAPTOP-RNTPF90S", "HotelManagement");

            rpt.Refresh();

            report1.ViewerCore.ReportSource = rpt;
        }
    }
}

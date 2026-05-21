using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using QLKS.Commands;
using QLKS.Models;
using QLKS.Helpers;
namespace QLKS.ViewModels
{
    public class ExportExcelViewModel : BaseViewModel
    {
        private HotelManagementEntities _db = new HotelManagementEntities();
        public ICommand ExportRevenueReportCmd { get; set; }

        public ExportExcelViewModel()
        {
            ExportRevenueReportCmd = new RelayCommand(p => {
                DataTable dt = new DataTable();
                dt.Columns.Add("Mã Hóa Đơn");
                dt.Columns.Add("Ngày Lập");
                dt.Columns.Add("Tổng Tiền (VND)");
                dt.Columns.Add("Loại Hóa Đơn");
                dt.Columns.Add("Ghi Chú");

                var listInvoices = _db.HoaDons.ToList();
                foreach (var item in listInvoices)
                {
                    dt.Rows.Add(item.MaHoaDon, item.NgayLap, item.TongTien, item.LoaiHoaDon, item.GhiChu);
                }
                ExcelExporter.ExportToExcel(dt, "Báo cáo tổng kết hóa đơn khách sạn", "DanhSachHoaDon");
            });
        }
    }
}
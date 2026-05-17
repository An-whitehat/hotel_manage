using QLKS.Models;
using System;
using System.Buffers.Text;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace QLKS.ViewModels
{
    public class ExportExcelViewModel : BaseViewModel
    {
        private HotelManagementEntities _db = new HotelManagementEntities();

        public ICommand ExportRevenueReportCmd { get; set; }

        public ExportExcelViewModel()
        {
            // Command liên kết với nút bấm Xuất Báo Cáo trên giao diện
            ExportRevenueReportCmd = new RelayCommand<object>(p => true, p => {

                // Chuyển đổi Linq Entity sang DataTable để đổ vào Helper Excel
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

                // Gọi helper xuất file
                ExcelExporter.ExportToExcel(dt, "Báo cáo tổng kết hóa đơn khách sạn", "DanhSachHoaDon");
            });
        }
    }
}

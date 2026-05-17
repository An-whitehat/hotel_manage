using QLKS.Commands;
using QLKS.Models;
using System;
using System.Buffers.Text;
using System.Collections.Generic;
using System.Data; // ĐÃ THÊM: Để nhận diện cấu trúc DataTable
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using QLKS.Helpers; // Nếu file ExcelExporter nằm trong thư mục Helpers
namespace QLKS.ViewModels
{
    public class ExportExcelViewModel : BaseViewModel
    {
        private HotelManagementEntities _db = new HotelManagementEntities();

        public ICommand ExportRevenueReportCmd { get; set; }

        public ExportExcelViewModel()
        {
            // ĐÃ SỬA: Loại bỏ <object> của RelayCommand để đồng bộ với project
            ExportRevenueReportCmd = new RelayCommand(p => {

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
            }, p => true);
        }
    }
}
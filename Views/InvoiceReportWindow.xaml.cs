using QLKS.Models;
using QLKS.Report;
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
    /// Interaction logic for InvoiceReportWindow.xaml
    /// </summary>
    public partial class InvoiceReportWindow : Window
    {
        public InvoiceReportWindow(int maHoaDon)
        {
            InitializeComponent();

            using (var db = new HotelManagementEntities())
            {
                var data = (from hd in db.HoaDons
                            join dp in db.DatPhongs
                                on hd.MaDatPhong equals dp.MaDatPhong

                            join kh in db.KhachHangs
                                on dp.MaKH equals kh.MaKH

                            join nv in db.NhanViens
                                on hd.MaNV equals nv.MaNV

                            join p in db.Phongs
                                on dp.MaPhong equals p.MaPhong

                            join ct in db.ChiTietHoaDons
                                on hd.MaHoaDon equals ct.MaHoaDon

                            join dv in db.DichVus
                                on ct.MaDichVu equals dv.MaDichVu

                            where hd.MaHoaDon == maHoaDon

                            select new
                            {
                                hd.MaHoaDon,
                                hd.NgayLap,
                                kh.HoTen,
                                TenNhanVien = nv.HoTen,
                                p.SoPhong,
                                dv.TenDichVu,
                                ct.SoLuong,
                                ct.DonGia,
                                ct.ThanhTien,
                                hd.TongTien
                            }).ToList();

                HoaDonThanhToan rpt =
                    new HoaDonThanhToan();

                rpt.SetDataSource(data);

                crystalReportViewer.ReportSource = rpt;
            }
        }
    }
}

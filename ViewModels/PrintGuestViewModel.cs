using QLKS.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QLKS.ViewModels
{
    public class PrintGuestViewModel : BaseViewModel
    {
        private ObservableCollection<KhachHang> _listKhachLuuTru;
        public ObservableCollection<KhachHang> ListKhachLuuTru
        {
            get => _listKhachLuuTru;
            set { _listKhachLuuTru = value; OnPropertyChanged(); }
        }

        public PrintGuestViewModel()
        {
            LoadData();
        }

        private void LoadData()
        {
            var listDatPhong = DataProvider.Ins.DB.DatPhongs
                                .Where(dp => dp.TrangThai == "Đã check-in")
                                .ToList();

            var data = listDatPhong.Select(dp => dp.KhachHang).Distinct().ToList();

            ListKhachLuuTru = new ObservableCollection<KhachHang>(data);
        }
    }
}

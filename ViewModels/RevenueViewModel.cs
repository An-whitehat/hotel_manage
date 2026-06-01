using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using QLKS.Models;
using QLKS.Commands; 
namespace QLKS.ViewModels
{
    public class RevenueDisplayItem
    {
        public string ThoiGian { get; set; } 
        public int SoLuongHoaDon { get; set; }
        public decimal ThanhTien { get; set; }
    }

    public class RevenueViewModel : BaseViewModel 
    {
        private DateTime _startDate = DateTime.Now.AddDays(-7);
        private DateTime _endDate = DateTime.Now;
        private decimal _totalRevenue;
        private ObservableCollection<RevenueDisplayItem> _revenueList;

        public DateTime StartDate
        {
            get => _startDate;
            set { _startDate = value; OnPropertyChanged(nameof(StartDate)); }
        }

        public DateTime EndDate
        {
            get => _endDate;
            set { _endDate = value; OnPropertyChanged(nameof(EndDate)); }
        }

        public decimal TotalRevenue
        {
            get => _totalRevenue;
            set { _totalRevenue = value; OnPropertyChanged(nameof(TotalRevenue)); }
        }

        public ObservableCollection<RevenueDisplayItem> RevenueList
        {
            get => _revenueList;
            set { _revenueList = value; OnPropertyChanged(nameof(RevenueList)); }
        }

        public ICommand ThongKeCommand { get; set; }

        public RevenueViewModel()
        {
            RevenueList = new ObservableCollection<RevenueDisplayItem>();
            ThongKeCommand = new RelayCommand((p) => ExecuteThongKe(), (p) => true);            
            ExecuteThongKe();
        }

     private void ExecuteThongKe()
{
    var sDate = StartDate.Date;
    var eDate = EndDate.Date.AddDays(1).AddTicks(-1);

 
    var invoices = DataProvider.Ins.DB.HoaDons
        .Where(h => h.NgayLap >= sDate && h.NgayLap <= eDate)
        .ToList();


    TotalRevenue = invoices.Sum(h => h.TongTien);

    
    var groupedData = invoices
        .Where(h => h.NgayLap.HasValue) 
        .GroupBy(h => h.NgayLap.Value.ToString("dd/MM/yyyy"))
        .Select(g => new RevenueDisplayItem
        {
            ThoiGian = g.Key,
            SoLuongHoaDon = g.Count(),
            ThanhTien = g.Sum(h => h.TongTien)
        })
        .OrderBy(x => x.ThoiGian);

    RevenueList.Clear();
    foreach (var item in groupedData)
    {
        RevenueList.Add(item);
    }
}
    }
}
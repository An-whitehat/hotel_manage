using QLKS.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QLKS.ViewModels
{
    public class PrintEmployeeViewModel : BaseViewModel
    {
        private ObservableCollection<CaLamViec> _listCaLam;
        public ObservableCollection<CaLamViec> ListCaLam
        {
            get => _listCaLam;
            set { _listCaLam = value; OnPropertyChanged(); }
        }

        public PrintEmployeeViewModel()
        {
            LoadData();
        }

        private void LoadData()
        {
            var data = DataProvider.Ins.DB.CaLamViecs
                        .OrderBy(c => c.NgayLam)
                        .ThenBy(c => c.Ca)
                        .ToList();

            ListCaLam = new ObservableCollection<CaLamViec>(data);
        }
    }
}

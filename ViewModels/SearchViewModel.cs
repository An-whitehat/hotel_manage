using QLKS.Commands;
using QLKS.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace QLKS.ViewModels
{
    public class SearchViewModel:BaseViewModel
    {
        private string _searchText;
        public string SearchText
        {
            get => _searchText;
            set
            {
                _searchText = value;
                OnPropertyChanged();
                ExecuteSearch(); // Gọi hàm tìm kiếm mỗi khi người dùng gõ chữ
            }
        }

        private ObservableCollection<KhachHang> _listKhachHang;
        public ObservableCollection<KhachHang> ListKhachHang
        {
            get => _listKhachHang;
            set { _listKhachHang = value; OnPropertyChanged(); }
        }
        public ICommand ResetCommand => new RelayCommand(_ => { SearchText = string.Empty; });
        // Đây chính là đoạn code bạn vừa hỏi, nhưng được đặt vào hàm thực thi
        void ExecuteSearch()
        {
            // 1. Luôn bắt đầu từ Queryable để tối ưu SQL
            var query = DataProvider.Ins.DB.KhachHangs.AsQueryable();

            // 2. Kiểm tra nếu có nhập từ khóa thì mới lọc
            if (!string.IsNullOrEmpty(SearchText))
            {
                query = query.Where(x => x.HoTen.Contains(SearchText) || x.SDT.Contains(SearchText));
            }

            // 3. Đẩy kết quả ra View
            ListKhachHang = new ObservableCollection<KhachHang>(query.ToList());
        }
    }
}

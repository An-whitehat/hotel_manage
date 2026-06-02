using QLKS.Commands;
using QLKS.Models;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace QLKS.ViewModels
{
    public class SearchViewModel : BaseViewModel
    {
        private readonly HotelManagementEntities _db = new HotelManagementEntities();

        // ── Search text ───────────────────────────────────────────────────────
        private string _searchText;
        public string SearchText
        {
            get => _searchText;
            set
            {
                _searchText = value;
                OnPropertyChanged();
                ExecuteSearch(); // real-time search mỗi khi gõ
            }
        }

        // ── Danh sách kết quả ─────────────────────────────────────────────────
        private ObservableCollection<KhachHang> _listKhachHang;
        public ObservableCollection<KhachHang> ListKhachHang
        {
            get => _listKhachHang;
            set { _listKhachHang = value; OnPropertyChanged(); }
        }

        // ── Dòng đang chọn trong DataGrid ────────────────────────────────────
        private KhachHang _selectedKhachHang;
        public KhachHang SelectedKhachHang
        {
            get => _selectedKhachHang;
            set
            {
                _selectedKhachHang = value;
                OnPropertyChanged();
                CommandManager.InvalidateRequerySuggested();
            }
        }

        // ── Commands ──────────────────────────────────────────────────────────
        public ICommand ResetCommand   { get; }
        public ICommand EditCommand    { get; }
        public ICommand DeleteCommand  { get; }

        // ── Constructor ───────────────────────────────────────────────────────
        public SearchViewModel()
        {
            ResetCommand  = new RelayCommand(_ => SearchText = string.Empty);
            EditCommand   = new RelayCommand(
                _ => ExecuteEdit(),
                _ => SelectedKhachHang != null && SelectedKhachHang.MaKH != 0);
            DeleteCommand = new RelayCommand(
                _ => ExecuteDelete(),
                _ => SelectedKhachHang != null && SelectedKhachHang.MaKH != 0);

            ExecuteSearch();
        }

        // ── Search ────────────────────────────────────────────────────────────
        private void ExecuteSearch()
        {
            var query = DataProvider.Ins.DB.KhachHangs.AsQueryable();

            if (!string.IsNullOrEmpty(SearchText))
            {
                query = query.Where(x =>
                    x.HoTen.Contains(SearchText) ||
                    x.SDT.Contains(SearchText)   ||
                    x.CCCD.Contains(SearchText)  ||
                    x.DiaChi.Contains(SearchText));
            }

            ListKhachHang = new ObservableCollection<KhachHang>(query.ToList());
        }

        // ── Edit: mở dialog inline — load thông tin vào form popup ──────────
        private void ExecuteEdit()
        {
            if (SelectedKhachHang == null) return;

            var dialog = new Views.EditKhachHangDialog(SelectedKhachHang);
            if (dialog.ShowDialog() == true)
            {
                try
                {
                    var kh = _db.KhachHangs.Find(SelectedKhachHang.MaKH);
                    if (kh == null) return;

                    kh.HoTen = dialog.HoTen;
                    kh.CCCD  = dialog.CCCD;
                    kh.SDT   = dialog.SDT;
                    kh.DiaChi = dialog.DiaChi;
                    kh.Email = dialog.Email;

                    _db.SaveChanges();
                    MessageBox.Show("Cập nhật thành công!", "Thông báo",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                    ExecuteSearch();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi khi cập nhật: " + ex.Message, "Lỗi",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        // ── Delete ────────────────────────────────────────────────────────────
        private void ExecuteDelete()
        {
            if (SelectedKhachHang == null) return;

            var confirm = MessageBox.Show(
                $"Xóa khách hàng \"{SelectedKhachHang.HoTen}\"?",
                "Xác nhận xóa", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (confirm != MessageBoxResult.Yes) return;

            try
            {
                var kh = _db.KhachHangs.Find(SelectedKhachHang.MaKH);
                if (kh == null) return;

                _db.KhachHangs.Remove(kh);
                _db.SaveChanges();
                MessageBox.Show("Đã xóa khách hàng.", "Thông báo",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                SelectedKhachHang = null;
                ExecuteSearch();
            }
            catch
            {
                MessageBox.Show(
                    "Không thể xóa do khách hàng đang có lịch sử đặt phòng liên kết!",
                    "Lỗi liên kết dữ liệu", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}

using QLKS.Commands;
using QLKS.Models;
using QLKS.Services;
using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Input;

namespace QLKS.ViewModels
{
    public class BookingViewModel : BaseViewModel
    {
        public ObservableCollection<DatPhongDto> DatPhongs { get; } = new ObservableCollection<DatPhongDto>();
        public ObservableCollection<KhachHangDto> KhachHangs { get; } = new ObservableCollection<KhachHangDto>();
        public ObservableCollection<PhongDto> PhongsTrong { get; } = new ObservableCollection<PhongDto>();

        private DatPhongDto _selectedDatPhong;
        public DatPhongDto SelectedDatPhong { get => _selectedDatPhong; set { SetProperty(ref _selectedDatPhong, value); FillForm(value); } }
        private KhachHangDto _selectedKhachHang;
        public KhachHangDto SelectedKhachHang { get => _selectedKhachHang; set => SetProperty(ref _selectedKhachHang, value); }
        private PhongDto _selectedPhong;
        public PhongDto SelectedPhong { get => _selectedPhong; set => SetProperty(ref _selectedPhong, value); }

        public int MaDatPhong { get; set; }
        public DateTime NgayCheckIn { get; set; } = DateTime.Today;
        public DateTime NgayCheckOut { get; set; } = DateTime.Today.AddDays(1);
        public int SoNguoi { get; set; } = 2;
        public string GhiChu { get; set; }
        public string Keyword { get; set; }

        public ICommand LoadCommand { get; }
        public ICommand AddBookingCommand { get; }
        public ICommand UpdateBookingCommand { get; }
        public ICommand CancelBookingCommand { get; }
        public ICommand CheckInCommand { get; }
        public ICommand ClearCommand { get; }
        public ICommand SearchCommand { get; }

        public BookingViewModel()
        {
            LoadCommand = new RelayCommand(_ => LoadAll());
            AddBookingCommand = new RelayCommand(_ => AddBooking());
            UpdateBookingCommand = new RelayCommand(_ => UpdateBooking());
            CancelBookingCommand = new RelayCommand(_ => CancelBooking());
            CheckInCommand = new RelayCommand(_ => CheckIn());
            ClearCommand = new RelayCommand(_ => ClearForm());
            SearchCommand = new RelayCommand(_ => LoadBookings());
            LoadAll();
        }

        private bool CanBooking()
        {
            if (SessionService.HasRole("Admin", "LeTan", "QuanLy")) return true;
            MessageBox.Show("Bạn không có quyền đặt phòng.");
            return false;
        }

        private void NotifyForm(){ OnPropertyChanged(nameof(MaDatPhong)); OnPropertyChanged(nameof(NgayCheckIn)); OnPropertyChanged(nameof(NgayCheckOut)); OnPropertyChanged(nameof(SoNguoi)); OnPropertyChanged(nameof(GhiChu)); OnPropertyChanged(nameof(Keyword)); }
        private void FillForm(DatPhongDto d)
        {
            if (d == null) return;
            MaDatPhong = d.MaDatPhong; NgayCheckIn = d.NgayCheckIn; NgayCheckOut = d.NgayCheckOut; SoNguoi = d.SoNguoi; GhiChu = d.GhiChu;
            NotifyForm();
        }

        private void LoadAll(){ LoadCustomers(); LoadRooms(); LoadBookings(); }

        private void LoadCustomers()
        {
            KhachHangs.Clear();
            var dt = DbService.GetDataTable("SELECT MaKH,HoTen,CCCD,SDT FROM KhachHang ORDER BY MaKH DESC");
            foreach (DataRow r in dt.Rows) KhachHangs.Add(new KhachHangDto{ MaKH=Convert.ToInt32(r["MaKH"]), HoTen=r["HoTen"].ToString(), CCCD=r["CCCD"].ToString(), SDT=r["SDT"].ToString() });
        }

        private void LoadRooms()
        {
            PhongsTrong.Clear();
            var dt = DbService.GetDataTable(@"SELECT p.MaPhong,p.SoPhong,p.MaLoaiPhong,lp.TenLoaiPhong,lp.GiaPhong,p.TrangThai FROM Phong p JOIN LoaiPhong lp ON p.MaLoaiPhong=lp.MaLoaiPhong WHERE p.TrangThai=N'Trống' ORDER BY p.SoPhong");
            foreach (DataRow r in dt.Rows) PhongsTrong.Add(new PhongDto{ MaPhong=Convert.ToInt32(r["MaPhong"]), SoPhong=r["SoPhong"].ToString(), MaLoaiPhong=Convert.ToInt32(r["MaLoaiPhong"]), TenLoaiPhong=r["TenLoaiPhong"].ToString(), GiaPhong=Convert.ToDecimal(r["GiaPhong"]), TrangThai=r["TrangThai"].ToString() });
        }

        private void LoadBookings()
        {
            DatPhongs.Clear();
            var dt = DbService.GetDataTable(@"SELECT dp.MaDatPhong, dp.MaKH, kh.HoTen TenKhachHang, dp.MaPhong, p.SoPhong, dp.MaNV, nv.HoTen TenNhanVien, dp.NgayDat, dp.NgayCheckIn, dp.NgayCheckOut, dp.SoNguoi, dp.TrangThai, dp.TongTien, dp.GhiChu, lp.GiaPhong
FROM DatPhong dp JOIN KhachHang kh ON dp.MaKH=kh.MaKH JOIN Phong p ON dp.MaPhong=p.MaPhong JOIN LoaiPhong lp ON p.MaLoaiPhong=lp.MaLoaiPhong JOIN NhanVien nv ON dp.MaNV=nv.MaNV
WHERE (@kw='' OR kh.HoTen LIKE @like OR p.SoPhong LIKE @like OR dp.TrangThai LIKE @like)
ORDER BY dp.MaDatPhong DESC", new SqlParameter("@kw", Keyword ?? ""), new SqlParameter("@like", "%" + (Keyword ?? "") + "%"));
            foreach (DataRow r in dt.Rows) DatPhongs.Add(new DatPhongDto{ MaDatPhong=Convert.ToInt32(r["MaDatPhong"]), MaKH=Convert.ToInt32(r["MaKH"]), TenKhachHang=r["TenKhachHang"].ToString(), MaPhong=Convert.ToInt32(r["MaPhong"]), SoPhong=r["SoPhong"].ToString(), MaNV=Convert.ToInt32(r["MaNV"]), TenNhanVien=r["TenNhanVien"].ToString(), NgayDat=Convert.ToDateTime(r["NgayDat"]), NgayCheckIn=Convert.ToDateTime(r["NgayCheckIn"]), NgayCheckOut=Convert.ToDateTime(r["NgayCheckOut"]), SoNguoi=Convert.ToInt32(r["SoNguoi"]), TrangThai=r["TrangThai"].ToString(), TongTien=r["TongTien"]==DBNull.Value?(decimal?)null:Convert.ToDecimal(r["TongTien"]), GhiChu=r["GhiChu"].ToString(), GiaPhong=Convert.ToDecimal(r["GiaPhong"]) });
        }

        private void ClearForm(){ MaDatPhong=0; SelectedDatPhong=null; SelectedKhachHang=null; SelectedPhong=null; NgayCheckIn=DateTime.Today; NgayCheckOut=DateTime.Today.AddDays(1); SoNguoi=2; GhiChu=null; NotifyForm(); }

        private void AddBooking()
        {
            if (!CanBooking()) return;
            if (SelectedKhachHang == null || SelectedPhong == null) { MessageBox.Show("Chọn khách hàng và phòng trống."); return; }
            if (NgayCheckOut <= NgayCheckIn) { MessageBox.Show("Ngày check-out phải sau ngày check-in."); return; }
            DbService.ExecuteNonQuery(@"INSERT INTO DatPhong(MaKH,MaPhong,MaNV,NgayCheckIn,NgayCheckOut,SoNguoi,TrangThai,GhiChu) VALUES(@MaKH,@MaPhong,@MaNV,@NgayCheckIn,@NgayCheckOut,@SoNguoi,N'Đang chờ',@GhiChu)",
                new SqlParameter("@MaKH", SelectedKhachHang.MaKH), new SqlParameter("@MaPhong", SelectedPhong.MaPhong), new SqlParameter("@MaNV", SessionService.CurrentUser.MaNV), new SqlParameter("@NgayCheckIn", NgayCheckIn.Date), new SqlParameter("@NgayCheckOut", NgayCheckOut.Date), new SqlParameter("@SoNguoi", SoNguoi), new SqlParameter("@GhiChu", (object)GhiChu ?? DBNull.Value));
            LoadBookings(); MessageBox.Show("Đã tạo phiếu đặt phòng.");
        }

        private void UpdateBooking()
        {
            if (!CanBooking()) return;
            if (MaDatPhong <= 0) { MessageBox.Show("Chọn phiếu đặt phòng cần sửa."); return; }
            DbService.ExecuteNonQuery("UPDATE DatPhong SET NgayCheckIn=@NgayCheckIn,NgayCheckOut=@NgayCheckOut,SoNguoi=@SoNguoi,GhiChu=@GhiChu WHERE MaDatPhong=@MaDatPhong AND TrangThai=N'Đang chờ'",
                new SqlParameter("@MaDatPhong", MaDatPhong), new SqlParameter("@NgayCheckIn", NgayCheckIn.Date), new SqlParameter("@NgayCheckOut", NgayCheckOut.Date), new SqlParameter("@SoNguoi", SoNguoi), new SqlParameter("@GhiChu", (object)GhiChu ?? DBNull.Value));
            LoadBookings(); MessageBox.Show("Đã cập nhật phiếu đặt phòng nếu còn trạng thái Đang chờ.");
        }

        private void CancelBooking()
        {
            if (!CanBooking()) return;
            if (SelectedDatPhong == null) { MessageBox.Show("Chọn phiếu cần hủy."); return; }
            DbService.ExecuteNonQuery("UPDATE DatPhong SET TrangThai=N'Hủy' WHERE MaDatPhong=@MaDatPhong AND TrangThai<>N'Đã check-out'", new SqlParameter("@MaDatPhong", SelectedDatPhong.MaDatPhong));
            LoadBookings(); LoadRooms(); MessageBox.Show("Đã hủy phiếu đặt phòng.");
        }

        private void CheckIn()
        {
            if (!CanBooking()) return;
            if (SelectedDatPhong == null) { MessageBox.Show("Chọn phiếu cần check-in."); return; }
            if (SelectedDatPhong.TrangThai != "Đang chờ") { MessageBox.Show("Chỉ check-in phiếu đang chờ."); return; }
            DbService.ExecuteNonQuery("UPDATE DatPhong SET TrangThai=N'Đã check-in' WHERE MaDatPhong=@MaDatPhong; UPDATE Phong SET TrangThai=N'Đang thuê' WHERE MaPhong=@MaPhong;", new SqlParameter("@MaDatPhong", SelectedDatPhong.MaDatPhong), new SqlParameter("@MaPhong", SelectedDatPhong.MaPhong));
            LoadAll(); MessageBox.Show("Check-in thành công.");
        }
    }
}

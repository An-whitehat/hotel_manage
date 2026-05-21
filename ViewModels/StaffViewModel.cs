using QLKS.Commands;
using QLKS.Models;
using QLKS.Services;
using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;

namespace QLKS.ViewModels
{
    public class StaffViewModel : BaseViewModel
    {
        public ObservableCollection<NhanVienDto> NhanViens { get; } = new ObservableCollection<NhanVienDto>();
        public ObservableCollection<CaLamViecDto> CaLamViecs { get; } = new ObservableCollection<CaLamViecDto>();
        public ObservableCollection<string> VaiTroList { get; } = new ObservableCollection<string>{ "Admin", "LeTan", "QuanLy" };
        public ObservableCollection<string> CaList { get; } = new ObservableCollection<string>{ "Sáng", "Chiều", "Tối" };

        private NhanVienDto _selectedNhanVien;
        public NhanVienDto SelectedNhanVien { get => _selectedNhanVien; set { SetProperty(ref _selectedNhanVien, value); FillStaffForm(value); } }
        private CaLamViecDto _selectedCa;
        public CaLamViecDto SelectedCa { get => _selectedCa; set { SetProperty(ref _selectedCa, value); FillShiftForm(value); } }

        private string _keyword;
        public string Keyword { get => _keyword; set => SetProperty(ref _keyword, value); }
        public int MaNV { get; set; }
        public string HoTen { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public string VaiTro { get; set; } = "LeTan";
        public string SDT { get; set; }
        public DateTime? NgayVaoLam { get; set; } = DateTime.Today;
        public bool TrangThai { get; set; } = true;

        public int MaCa { get; set; }
        public int CaMaNV { get; set; }
        public DateTime NgayLam { get; set; } = DateTime.Today;
        public string Ca { get; set; } = "Sáng";
        public string GhiChuCa { get; set; }

        public ICommand LoadCommand { get; }
        public ICommand SearchCommand { get; }
        public ICommand ClearStaffCommand { get; }
        public ICommand AddStaffCommand { get; }
        public ICommand UpdateStaffCommand { get; }
        public ICommand DeleteStaffCommand { get; }
        public ICommand ClearShiftCommand { get; }
        public ICommand AddShiftCommand { get; }
        public ICommand UpdateShiftCommand { get; }
        public ICommand DeleteShiftCommand { get; }
        public ICommand PrintStaffShiftCommand { get; }

        public StaffViewModel()
        {
            LoadCommand = new RelayCommand(_ => LoadAll());
            SearchCommand = new RelayCommand(_ => LoadStaff());
            ClearStaffCommand = new RelayCommand(_ => ClearStaff());
            AddStaffCommand = new RelayCommand(_ => AddStaff());
            UpdateStaffCommand = new RelayCommand(_ => UpdateStaff());
            DeleteStaffCommand = new RelayCommand(_ => DeleteStaff());
            ClearShiftCommand = new RelayCommand(_ => ClearShift());
            AddShiftCommand = new RelayCommand(_ => AddShift());
            UpdateShiftCommand = new RelayCommand(_ => UpdateShift());
            DeleteShiftCommand = new RelayCommand(_ => DeleteShift());
            PrintStaffShiftCommand = new RelayCommand(_ => PrintStaffShift());
            LoadAll();
        }

        private bool CanManageStaff()
        {
            if (SessionService.HasRole("Admin", "QuanLy")) return true;
            MessageBox.Show("Chỉ Admin hoặc Quản lý được quản lý nhân viên/ca làm.");
            return false;
        }

        private void NotifyAllForm()
        {
            OnPropertyChanged(nameof(MaNV)); OnPropertyChanged(nameof(HoTen)); OnPropertyChanged(nameof(Username)); OnPropertyChanged(nameof(PasswordHash));
            OnPropertyChanged(nameof(VaiTro)); OnPropertyChanged(nameof(SDT)); OnPropertyChanged(nameof(NgayVaoLam)); OnPropertyChanged(nameof(TrangThai));
            OnPropertyChanged(nameof(MaCa)); OnPropertyChanged(nameof(CaMaNV)); OnPropertyChanged(nameof(NgayLam)); OnPropertyChanged(nameof(Ca)); OnPropertyChanged(nameof(GhiChuCa));
        }

        private void FillStaffForm(NhanVienDto nv)
        {
            if (nv == null) return;
            MaNV = nv.MaNV; HoTen = nv.HoTen; Username = nv.Username; PasswordHash = nv.PasswordHash;
            VaiTro = nv.VaiTro; SDT = nv.SDT; NgayVaoLam = nv.NgayVaoLam; TrangThai = nv.TrangThai; CaMaNV = nv.MaNV;
            NotifyAllForm();
        }

        private void FillShiftForm(CaLamViecDto ca)
        {
            if (ca == null) return;
            MaCa = ca.MaCa; CaMaNV = ca.MaNV; NgayLam = ca.NgayLam; Ca = ca.Ca; GhiChuCa = ca.GhiChu;
            NotifyAllForm();
        }

        private void LoadAll(){ LoadStaff(); LoadShift(); }

        private void LoadStaff()
        {
            NhanViens.Clear();
            string sql = @"SELECT MaNV, HoTen, Username, PasswordHash, VaiTro, SDT, NgayVaoLam, TrangThai FROM NhanVien
WHERE (@kw = '' OR HoTen LIKE @like OR Username LIKE @like OR SDT LIKE @like)
ORDER BY MaNV DESC";
            DataTable dt = DbService.GetDataTable(sql,
                new SqlParameter("@kw", Keyword ?? ""),
                new SqlParameter("@like", "%" + (Keyword ?? "") + "%"));
            foreach (DataRow r in dt.Rows)
            {
                NhanViens.Add(new NhanVienDto
                {
                    MaNV = Convert.ToInt32(r["MaNV"]), HoTen = r["HoTen"].ToString(), Username = r["Username"].ToString(),
                    PasswordHash = r["PasswordHash"].ToString(), VaiTro = r["VaiTro"].ToString(), SDT = r["SDT"].ToString(),
                    NgayVaoLam = r["NgayVaoLam"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(r["NgayVaoLam"]),
                    TrangThai = Convert.ToBoolean(r["TrangThai"])
                });
            }
        }

        private void LoadShift()
        {
            CaLamViecs.Clear();
            string sql = @"SELECT c.MaCa, c.MaNV, nv.HoTen, c.NgayLam, c.Ca, c.GhiChu
FROM CaLamViec c JOIN NhanVien nv ON c.MaNV = nv.MaNV
ORDER BY c.NgayLam DESC, c.MaCa DESC";
            DataTable dt = DbService.GetDataTable(sql);
            foreach (DataRow r in dt.Rows)
                CaLamViecs.Add(new CaLamViecDto{ MaCa = Convert.ToInt32(r["MaCa"]), MaNV = Convert.ToInt32(r["MaNV"]), HoTen = r["HoTen"].ToString(), NgayLam = Convert.ToDateTime(r["NgayLam"]), Ca = r["Ca"].ToString(), GhiChu = r["GhiChu"].ToString() });
        }

        private void ClearStaff(){ MaNV = 0; HoTen = Username = PasswordHash = SDT = null; VaiTro = "LeTan"; NgayVaoLam = DateTime.Today; TrangThai = true; SelectedNhanVien = null; NotifyAllForm(); }
        private void ClearShift(){ MaCa = 0; CaMaNV = SelectedNhanVien?.MaNV ?? 0; NgayLam = DateTime.Today; Ca = "Sáng"; GhiChuCa = null; SelectedCa = null; NotifyAllForm(); }

        private void AddStaff()
        {
            if (!CanManageStaff()) return;
            if (string.IsNullOrWhiteSpace(HoTen) || string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(PasswordHash)) { MessageBox.Show("Nhập đủ họ tên, username, password."); return; }
            DbService.ExecuteNonQuery(@"INSERT INTO NhanVien(HoTen,Username,PasswordHash,VaiTro,SDT,NgayVaoLam,TrangThai) VALUES(@HoTen,@Username,@PasswordHash,@VaiTro,@SDT,@NgayVaoLam,@TrangThai)",
                new SqlParameter("@HoTen", HoTen.Trim()), new SqlParameter("@Username", Username.Trim()), new SqlParameter("@PasswordHash", PasswordHash.Trim()),
                new SqlParameter("@VaiTro", VaiTro ?? "LeTan"), new SqlParameter("@SDT", (object)SDT ?? DBNull.Value), new SqlParameter("@NgayVaoLam", (object)NgayVaoLam ?? DBNull.Value), new SqlParameter("@TrangThai", TrangThai));
            LoadStaff(); ClearStaff(); MessageBox.Show("Đã thêm nhân viên.");
        }

        private void UpdateStaff()
        {
            if (!CanManageStaff()) return;
            if (MaNV <= 0) { MessageBox.Show("Chọn nhân viên cần sửa."); return; }
            DbService.ExecuteNonQuery(@"UPDATE NhanVien SET HoTen=@HoTen,Username=@Username,PasswordHash=@PasswordHash,VaiTro=@VaiTro,SDT=@SDT,NgayVaoLam=@NgayVaoLam,TrangThai=@TrangThai WHERE MaNV=@MaNV",
                new SqlParameter("@MaNV", MaNV), new SqlParameter("@HoTen", HoTen ?? ""), new SqlParameter("@Username", Username ?? ""), new SqlParameter("@PasswordHash", PasswordHash ?? ""),
                new SqlParameter("@VaiTro", VaiTro ?? "LeTan"), new SqlParameter("@SDT", (object)SDT ?? DBNull.Value), new SqlParameter("@NgayVaoLam", (object)NgayVaoLam ?? DBNull.Value), new SqlParameter("@TrangThai", TrangThai));
            LoadStaff(); MessageBox.Show("Đã cập nhật nhân viên.");
        }

        private void DeleteStaff()
        {
            if (!CanManageStaff()) return;
            if (MaNV <= 0) { MessageBox.Show("Chọn nhân viên cần xóa."); return; }
            if (MessageBox.Show("Xóa nhân viên này?", "Xác nhận", MessageBoxButton.YesNo) != MessageBoxResult.Yes) return;
            DbService.ExecuteNonQuery("DELETE FROM NhanVien WHERE MaNV=@MaNV", new SqlParameter("@MaNV", MaNV));
            LoadStaff(); ClearStaff(); MessageBox.Show("Đã xóa nhân viên.");
        }

        private void AddShift()
        {
            if (!CanManageStaff()) return;
            if (CaMaNV <= 0) { MessageBox.Show("Chọn hoặc nhập mã nhân viên cho ca làm."); return; }
            DbService.ExecuteNonQuery("INSERT INTO CaLamViec(MaNV,NgayLam,Ca,GhiChu) VALUES(@MaNV,@NgayLam,@Ca,@GhiChu)",
                new SqlParameter("@MaNV", CaMaNV), new SqlParameter("@NgayLam", NgayLam), new SqlParameter("@Ca", Ca ?? "Sáng"), new SqlParameter("@GhiChu", (object)GhiChuCa ?? DBNull.Value));
            LoadShift(); ClearShift(); MessageBox.Show("Đã thêm ca làm.");
        }

        private void UpdateShift()
        {
            if (!CanManageStaff()) return;
            if (MaCa <= 0) { MessageBox.Show("Chọn ca cần sửa."); return; }
            DbService.ExecuteNonQuery("UPDATE CaLamViec SET MaNV=@MaNV,NgayLam=@NgayLam,Ca=@Ca,GhiChu=@GhiChu WHERE MaCa=@MaCa",
                new SqlParameter("@MaCa", MaCa), new SqlParameter("@MaNV", CaMaNV), new SqlParameter("@NgayLam", NgayLam), new SqlParameter("@Ca", Ca ?? "Sáng"), new SqlParameter("@GhiChu", (object)GhiChuCa ?? DBNull.Value));
            LoadShift(); MessageBox.Show("Đã cập nhật ca làm.");
        }

        private void DeleteShift()
        {
            if (!CanManageStaff()) return;
            if (MaCa <= 0) { MessageBox.Show("Chọn ca cần xóa."); return; }
            DbService.ExecuteNonQuery("DELETE FROM CaLamViec WHERE MaCa=@MaCa", new SqlParameter("@MaCa", MaCa));
            LoadShift(); ClearShift(); MessageBox.Show("Đã xóa ca làm.");
        }

        private void PrintStaffShift()
        {
            var doc = new FlowDocument { PagePadding = new Thickness(45), FontFamily = new System.Windows.Media.FontFamily("Arial"), FontSize = 12 };
            doc.Blocks.Add(new Paragraph(new Run("DANH SÁCH NHÂN VIÊN VÀ CA LÀM")) { FontSize = 20, FontWeight = FontWeights.Bold, TextAlignment = TextAlignment.Center });
            doc.Blocks.Add(new Paragraph(new Run("Ngày in: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm"))));
            var table = new Table();
            for (int i = 0; i < 5; i++) table.Columns.Add(new TableColumn());
            table.RowGroups.Add(new TableRowGroup());
            var header = new TableRow(); table.RowGroups[0].Rows.Add(header);
            foreach (var h in new[] { "Mã ca", "Ngày", "Ca", "Mã NV", "Họ tên" }) header.Cells.Add(new TableCell(new Paragraph(new Run(h))) { FontWeight = FontWeights.Bold });
            foreach (var c in CaLamViecs)
            {
                var row = new TableRow(); table.RowGroups[0].Rows.Add(row);
                row.Cells.Add(new TableCell(new Paragraph(new Run(c.MaCa.ToString()))));
                row.Cells.Add(new TableCell(new Paragraph(new Run(c.NgayLam.ToString("dd/MM/yyyy")))));
                row.Cells.Add(new TableCell(new Paragraph(new Run(c.Ca))));
                row.Cells.Add(new TableCell(new Paragraph(new Run(c.MaNV.ToString()))));
                row.Cells.Add(new TableCell(new Paragraph(new Run(c.HoTen))));
            }
            doc.Blocks.Add(table);
            new PrintDialog().PrintDocument(((IDocumentPaginatorSource)doc).DocumentPaginator, "In danh sách nhân viên và ca làm");
        }
    }
}

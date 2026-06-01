using QLKS.Commands;
using QLKS.Models;
using System.Collections.ObjectModel;
using System.Linq;
using System.Collections.Generic;

namespace QLKS.ViewModels
{
    // ── Model hiển thị cho mỗi phòng trên Floor Plan ──────────────────────────
    public class PhongDisplayModel : BaseViewModel
    {
        public int MaPhong { get; set; }
        public string SoPhong { get; set; }
        public string TrangThai { get; set; }
        public string TenLoaiPhong { get; set; }
        public string GhiChu { get; set; }

        // Màu nền button theo trạng thái
        
        public string MauNen
        {
            get
            {
                switch (TrangThai)
                {
                    case "Trống": return "#2ECC71";  // xanh lá
                    case "Đang thuê":
                    case "Đang sử dụng": return "#E74C3C";  // đỏ
                    case "Đang dọn": return "#F39C12";  // cam
                    case "Bảo trì": return "#95A5A6";  // xám
                    default: return "#BDC3C7";
                }
            }
        }

        // Màu chữ trên button
        public string MauChu => "#FFFFFF";
    }

    // ── Model nhóm phòng theo tầng ────────────────────────────────────────────
    public class TangModel
    {
        public string TenTang { get; set; }
        public ObservableCollection<PhongDisplayModel> DanhSachPhong { get; set; }
    }

    // ── ViewModel chính ───────────────────────────────────────────────────────
    public class FloorPlanViewModel : BaseViewModel
    {
        // Danh sách tầng (mỗi tầng chứa list phòng)
        private ObservableCollection<TangModel> _danhSachTang;
        public ObservableCollection<TangModel> DanhSachTang
        {
            get => _danhSachTang;
            set { _danhSachTang = value; OnPropertyChanged(); }
        }

        // Phòng đang được chọn (hiện popup/detail)
        private PhongDisplayModel _phongDangChon;
        public PhongDisplayModel PhongDangChon
        {
            get => _phongDangChon;
            set { _phongDangChon = value; OnPropertyChanged(); OnPropertyChanged(nameof(CoPhongDangChon)); }
        }

        public bool CoPhongDangChon => PhongDangChon != null;

        // Legend thống kê nhanh
        private int _soPhongTrong;
        public int SoPhongTrong
        {
            get => _soPhongTrong;
            set { _soPhongTrong = value; OnPropertyChanged(); }
        }

        private int _soPhongDangDung;
        public int SoPhongDangDung
        {
            get => _soPhongDangDung;
            set { _soPhongDangDung = value; OnPropertyChanged(); }
        }

        private int _soPhongBaoTri;
        public int SoPhongBaoTri
        {
            get => _soPhongBaoTri;
            set { _soPhongBaoTri = value; OnPropertyChanged(); }
        }

        // Commands
        public RelayCommand ChonPhongCommand { get; }
        public RelayCommand DongChiTietCommand { get; }
        public RelayCommand LamMoiCommand { get; }

        // ── Constructor ───────────────────────────────────────────────────────
        public FloorPlanViewModel()
        {
            ChonPhongCommand = new RelayCommand(p => ChonPhong(p as PhongDisplayModel));
            DongChiTietCommand = new RelayCommand(_ => PhongDangChon = null);
            LamMoiCommand = new RelayCommand(_ => LoadDuLieu());

            LoadDuLieu();
        }

        // ── Load dữ liệu từ DB ────────────────────────────────────────────────
        private void LoadDuLieu()
        {
            var danhSachPhong = DataProvider.Ins.DB.Phongs
                .Include("LoaiPhong")   // EF load navigation property
                .ToList();

            // Map sang DisplayModel
            var displayList = danhSachPhong.Select(p => new PhongDisplayModel
            {
                MaPhong = p.MaPhong,
                SoPhong = p.SoPhong,
                TrangThai = p.TrangThai ?? "Trống",
                TenLoaiPhong = p.LoaiPhong?.TenLoaiPhong ?? "Chưa phân loại",
                GhiChu = p.GhiChu ?? ""
            }).ToList();

            // Group theo tầng: lấy ký tự đầu của SoPhong
            // VD: "101" → tầng "1", "201" → tầng "2"
            var grouped = displayList
                .GroupBy(p => LayTang(p.SoPhong))
                .OrderBy(g => g.Key)
                .Select(g => new TangModel
                {
                    TenTang = $"Tầng {g.Key}",
                    DanhSachPhong = new ObservableCollection<PhongDisplayModel>(g.ToList())
                });

            DanhSachTang = new ObservableCollection<TangModel>(grouped);

            // Cập nhật thống kê
            SoPhongTrong = displayList.Count(p => p.TrangThai == "Trống");
            SoPhongDangDung = displayList.Count(p => p.TrangThai == "Đang thuê" || p.TrangThai == "Đang sử dụng");
            SoPhongBaoTri = displayList.Count(p => p.TrangThai == "Bảo trì" || p.TrangThai == "Đang dọn");
        }

        // ── Helpers ───────────────────────────────────────────────────────────

        /// <summary>
        /// Lấy số tầng từ SoPhong.
        /// "101" → "1" | "B01" → "B" | "1A" → "1"
        /// </summary>
        private string LayTang(string soPhong)
        {
            if (string.IsNullOrEmpty(soPhong)) return "?";
            // Lấy ký tự đầu tiên làm số tầng
            return soPhong.Substring(0, 1).ToUpper();
        }

        private void ChonPhong(PhongDisplayModel phong)
        {
            if (phong == null) return;
            PhongDangChon = phong;
        }
    }
}

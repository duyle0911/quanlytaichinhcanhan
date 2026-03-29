using WpfApp3.Models;

namespace WpfApp3.Services
{
    public interface IGiaoDichService
    {
        Task<List<GiaoDich>> LayDanhSachAsync(int nguoiDungId, LoaiGiaoDich? loai = null);

        Task<GiaoDich?> LayTheoIdAsync(int giaoDichId, int nguoiDungId);

        Task<KetQua> TaoAsync(string moTa, decimal soTien, DateTime ngay, LoaiGiaoDich loai, int danhMucId, int nguoiDungId);

        Task<KetQua> CapNhatAsync(int giaoDichId, decimal soTien, string moTa, int danhMucId, DateTime ngay, int nguoiDungId);

        Task<KetQua> XoaAsync(int giaoDichId, int nguoiDungId);

        Task<KiemTraChiTieu> KiemTraChiAsync(decimal soTien, int danhMucId, int nguoiDungId, DateTime ngay);

        Task<KiemTraChiTieu> KiemTraCapNhatChiAsync(int giaoDichId, decimal soTienMoi, int danhMucIdMoi, int nguoiDungId, DateTime ngayMoi);
    }

    public class KetQua
    {
        public bool ThanhCong { get; set; }
        public string ThongBao { get; set; } = "";
    }

    public class KiemTraChiTieu
    {
        public bool HopLe { get; set; }
        public string ThongBao { get; set; } = "";
    }
}
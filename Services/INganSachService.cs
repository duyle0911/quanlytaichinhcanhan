using WpfApp3.Models;

namespace WpfApp3.Services
{
    public interface INganSachService
    {
        Task<List<NganSach>> LayDanhSachAsync(int nguoiDungId);

        Task<NganSach?> LayTheoIdAsync(int nganSachId, int nguoiDungId);

        Task<NganSach?> LayTheoDanhMucVaNgayAsync(int danhMucId, int nguoiDungId, DateTime ngay);

        Task<bool> TaoAsync(NganSach nganSach);

        Task<bool> CapNhatAsync(int nganSachId, decimal soTien, DateTime batDau, DateTime ketThuc, bool trangThai, int nguoiDungId);

        Task<bool> XoaAsync(int nganSachId, int nguoiDungId);

        Task<bool> CongChiAsync(int danhMucId, int nguoiDungId, decimal soTien, DateTime? ngay = null);

        Task<CanhBaoNganSach> KiemTraCanhBaoAsync(int nguoiDungId);

        Task<decimal> TinhDaChiAsync(int nganSachId);
    }

    public class CanhBaoNganSach
    {
        public List<string> VuotMuc { get; set; } = new();
        public List<string> CanhBao { get; set; } = new();

        public bool CoCanhBao => VuotMuc.Count > 0 || CanhBao.Count > 0;
    }
}
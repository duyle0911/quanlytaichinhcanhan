using WpfApp3.Models;

namespace WpfApp3.Services
{
    public interface IDuLieuService
    {
        Task<bool> CoDuLieuAsync(int nguoiDungId);

        Task<List<ChiTieuDanhMuc>> LayChiTieuTheoDanhMucAsync(int nguoiDungId, DateTime tuNgay, DateTime denNgay);

        Task<List<GiaoDich>> LayGanDayAsync(int nguoiDungId, int soLuong = 10);

        Task<List<GiaoDich>> LayTheoKyAsync(int nguoiDungId, DateTime tuNgay, DateTime denNgay);

        Task<TongQuanTaiChinh> LayTongQuanAsync(int nguoiDungId);

        Task<decimal> TongThuAsync(int nguoiDungId);

        Task<decimal> TongChiAsync(int nguoiDungId);

        Task<decimal> TongThuTheoKyAsync(int nguoiDungId, DateTime tuNgay, DateTime denNgay);

        Task<decimal> TongChiTheoKyAsync(int nguoiDungId, DateTime tuNgay, DateTime denNgay);

        Task<decimal> ThuThangAsync(int nguoiDungId, int? thang = null, int? nam = null);

        Task<decimal> ChiThangAsync(int nguoiDungId, int? thang = null, int? nam = null);

        Task<decimal> TrungBinhThuAsync(int nguoiDungId);

        Task<decimal> TrungBinhChiAsync(int nguoiDungId);

        Task<bool> ResetAsync(int nguoiDungId);
    }
}
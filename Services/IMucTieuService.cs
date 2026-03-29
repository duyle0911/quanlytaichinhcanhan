using WpfApp3.Models;

namespace WpfApp3.Services
{
    public interface IMucTieuService
    {
        Task<List<MucTieu>> LayDanhSachAsync(int nguoiDungId);

        Task<MucTieu?> LayTheoIdAsync(int mucTieuId, int nguoiDungId);

        Task<bool> TaoAsync(string ten, decimal mucTieuTien, DateTime ngayMucTieu, int nguoiDungId, string? moTa = null);

        Task<bool> CapNhatAsync(int mucTieuId, string ten, decimal mucTieuTien, DateTime ngayMucTieu, string? moTa, int nguoiDungId);

        Task<bool> XoaAsync(int mucTieuId, int nguoiDungId);

        Task<(bool ThanhCong, string ThongBao)> NapTienAsync(int mucTieuId, decimal soTien, int nguoiDungId);

        Task<string> LayTrangThaiAsync(MucTieu mucTieu);
    }
}
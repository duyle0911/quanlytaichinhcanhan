using WpfApp3.Models;

namespace WpfApp3.Services
{
    public interface IDanhMucService
    {
        Task<List<DanhMuc>> LayDanhSachAsync(int nguoiDungId, LoaiGiaoDich? loai = null);

        Task<bool> TaoAsync(string ten, LoaiGiaoDich loai, int nguoiDungId, string? mau = null, string? bieuTuong = null);

        Task<bool> CapNhatAsync(int id, string ten, LoaiGiaoDich loai, string? mau, string? bieuTuong, int nguoiDungId);

        Task<bool> XoaAsync(int id, int nguoiDungId);
    }
}
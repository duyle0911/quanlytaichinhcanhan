using WpfApp3.Models;

namespace WpfApp3.Services
{
    public interface IXacThucService
    {
        Task<NguoiDung?> DangNhap(string tenDangNhap, string matKhau);

        Task<bool> DangKy(string tenDangNhap, string email, string matKhau, string? hoTen = null);

        Task<bool> CapNhat(int nguoiDungId, string? hoTen, string? email, string? avatar);
    }
}
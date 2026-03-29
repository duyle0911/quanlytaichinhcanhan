using WpfApp3.Models;

namespace WpfApp3.Services
{
    public interface IPhienNguoiDung
    {
        NguoiDung? NguoiDungHienTai { get; }

        int? NguoiDungId { get; }

        bool DaDangNhap { get; }

        void ThietLap(NguoiDung nguoiDung);

        void Xoa();
    }
}
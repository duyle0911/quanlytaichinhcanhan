using WpfApp3.Models;

namespace WpfApp3.Services
{
    public sealed class PhienNguoiDung : IPhienNguoiDung
    {
        private NguoiDung? _nguoiDung;

        public NguoiDung? NguoiDungHienTai => _nguoiDung;

        public int? NguoiDungId => _nguoiDung?.Id;

        public bool DaDangNhap => _nguoiDung != null;

        public void ThietLap(NguoiDung nguoiDung)
        {
            _nguoiDung = nguoiDung;
        }

        public void Xoa()
        {
            _nguoiDung = null;
        }
    }
}
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WpfApp3.Models
{
    [Table("NguoiDung")]
    public class NguoiDung
    {
        [Key]
        public int Id { get; private set; }

        [Required, MaxLength(50)]
        public string TenDangNhap { get; private set; }

        [Required, MaxLength(100)]
        [EmailAddress]
        public string Email { get; private set; }

        [Required, MaxLength(255)]
        public string MatKhau { get; private set; }

        [MaxLength(100)]
        public string? HoTen { get; private set; }

        [MaxLength(500)]
        public string? AnhDaiDien { get; private set; }

        public DateTime NgayTao { get; private set; }

        public DateTime? LanDangNhapCuoi { get; private set; }

        public bool TrangThai { get; private set; }
        public ICollection<GiaoDich> GiaoDiches { get; private set; } = new List<GiaoDich>();
        public ICollection<NganSach> NganSaches { get; private set; } = new List<NganSach>();
        public ICollection<MucTieu> MucTieus { get; private set; } = new List<MucTieu>();
        public ICollection<DanhMuc> DanhMucs { get; private set; } = new List<DanhMuc>();
        public NguoiDung(string tenDangNhap, string email, string matKhau)
        {
            TenDangNhap = tenDangNhap;
            Email = email;
            MatKhau = matKhau;
            NgayTao = DateTime.Now;
            TrangThai = true;
        }
        public void CapNhatThongTin(string? hoTen, string? anhDaiDien)
        {
            HoTen = hoTen;
            AnhDaiDien = anhDaiDien;
        }

        public void CapNhatDangNhap()
        {
            LanDangNhapCuoi = DateTime.Now;
        }

        public void KhoaTaiKhoan()
        {
            TrangThai = false;
        }
    }
}
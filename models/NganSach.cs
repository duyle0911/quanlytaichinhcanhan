using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WpfApp3.Models
{
    [Table("NganSach")]
    public sealed class NganSach
    {
        private NganSach() { }

        [Key]
        public int Id { get; private set; }

        [Required, MaxLength(100)]
        public string Ten { get; private set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal SoTien { get; private set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal DaChi { get; private set; }

        [Required]
        public DateTime BatDau { get; private set; }

        [Required]
        public DateTime KetThuc { get; private set; }

        public int DanhMucId { get; private set; }
        public DanhMuc DanhMuc { get; private set; }

        public int NguoiDungId { get; private set; }
        public NguoiDung NguoiDung { get; private set; }

        public bool TrangThai { get; private set; }

        [MaxLength(500)]
        public string? GhiChu { get; private set; }

        public DateTime TaoLuc { get; private set; }
        public DateTime CapNhatLuc { get; private set; }

        public NganSach(string ten, decimal soTien, DateTime batDau, DateTime ketThuc, int nguoiDungId, int danhMucId, string? ghiChu = null)
        {
            Ten = ten.Trim();
            SoTien = soTien > 0 ? soTien : throw new ArgumentException();
            DaChi = 0;
            BatDau = batDau;
            KetThuc = ketThuc > batDau ? ketThuc : throw new ArgumentException();
            NguoiDungId = nguoiDungId;
            DanhMucId = danhMucId;
            GhiChu = ghiChu;
            TrangThai = true;
            TaoLuc = DateTime.UtcNow;
            CapNhatLuc = TaoLuc;
        }

        public void CapNhat(string ten, decimal soTien, DateTime batDau, DateTime ketThuc, string? ghiChu)
        {
            Ten = ten.Trim();
            SoTien = soTien > 0 ? soTien : throw new ArgumentException();
            BatDau = batDau;
            KetThuc = ketThuc > batDau ? ketThuc : throw new ArgumentException();
            GhiChu = ghiChu;
            CapNhatLuc = DateTime.UtcNow;
        }

        public void CongChi(decimal soTien)
        {
            if (soTien <= 0) throw new ArgumentException();
            DaChi += soTien;
            CapNhatLuc = DateTime.UtcNow;
        }

        public void TruChi(decimal soTien)
        {
            if (soTien <= 0) throw new ArgumentException();
            DaChi = DaChi - soTien < 0 ? 0 : DaChi - soTien;
            CapNhatLuc = DateTime.UtcNow;
        }

        public void Khoa()
        {
            TrangThai = false;
            CapNhatLuc = DateTime.UtcNow;
        }

        public void Mo()
        {
            TrangThai = true;
            CapNhatLuc = DateTime.UtcNow;
        }
    }
}
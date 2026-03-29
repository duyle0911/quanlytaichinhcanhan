using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WpfApp3.Models
{
    [Table("MucTieu")]
    public sealed class MucTieu
    {
        private MucTieu() { }

        [Key]
        public int Id { get; private set; }

        [Required, MaxLength(200)]
        public string Ten { get; private set; }

        [MaxLength(500)]
        public string? MoTa { get; private set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal MucTieuTien { get; private set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal HienTai { get; private set; }

        [Required]
        public DateTime Han { get; private set; }

        public DateTime? HoanThanhLuc { get; private set; }

        [MaxLength(7)]
        public string Mau { get; private set; }

        public int NguoiDungId { get; private set; }
        public NguoiDung NguoiDung { get; private set; }

        public DateTime TaoLuc { get; private set; }
        public DateTime CapNhatLuc { get; private set; }

        [NotMapped]
        public bool DaHoanThanh => HienTai >= MucTieuTien;

        public MucTieu(string ten, decimal mucTieuTien, DateTime han, int nguoiDungId, string? moTa = null, string? mau = null)
        {
            Ten = ten.Trim();
            MucTieuTien = mucTieuTien > 0 ? mucTieuTien : throw new ArgumentException();
            HienTai = 0;
            Han = han;
            NguoiDungId = nguoiDungId;
            MoTa = moTa;
            Mau = string.IsNullOrWhiteSpace(mau) ? "#2196F3" : mau;
            TaoLuc = DateTime.UtcNow;
            CapNhatLuc = TaoLuc;
        }

        public void CapNhat(string ten, decimal mucTieuTien, DateTime han, string? moTa, string? mau)
        {
            Ten = ten.Trim();
            MucTieuTien = mucTieuTien > 0 ? mucTieuTien : throw new ArgumentException();
            Han = han;
            MoTa = moTa;
            Mau = string.IsNullOrWhiteSpace(mau) ? Mau : mau;
            CapNhatLuc = DateTime.UtcNow;
        }

        public void NapTien(decimal soTien)
        {
            if (soTien <= 0) throw new ArgumentException();
            HienTai += soTien;
            if (HienTai >= MucTieuTien && HoanThanhLuc == null)
                HoanThanhLuc = DateTime.UtcNow;
            CapNhatLuc = DateTime.UtcNow;
        }

        public void TruTien(decimal soTien)
        {
            if (soTien <= 0) throw new ArgumentException();
            HienTai = HienTai - soTien < 0 ? 0 : HienTai - soTien;
            if (HienTai < MucTieuTien) HoanThanhLuc = null;
            CapNhatLuc = DateTime.UtcNow;
        }
    }
}
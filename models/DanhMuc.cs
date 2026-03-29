using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WpfApp3.Models
{
    [Table("DanhMuc")]
    public sealed class DanhMuc
    {
        private DanhMuc() { }

        [Key]
        public int Id { get; private set; }

        [Required, MaxLength(100)]
        public string Ten { get; private set; }

        [MaxLength(7)]
        public string Mau { get; private set; }

        [MaxLength(50)]
        public string BieuTuong { get; private set; }

        [Required]
        public LoaiGiaoDich Loai { get; private set; }

        public int NguoiDungId { get; private set; }
        public NguoiDung NguoiDung { get; private set; }

        public DateTime TaoLuc { get; private set; }
        public DateTime CapNhatLuc { get; private set; }

        public ICollection<GiaoDich> GiaoDiches { get; private set; } = new List<GiaoDich>();

        public DanhMuc(string ten, LoaiGiaoDich loai, int nguoiDungId, string? mau = null, string? bieuTuong = null)
        {
            Ten = ten.Trim();
            Loai = loai;
            NguoiDungId = nguoiDungId;
            Mau = string.IsNullOrWhiteSpace(mau) ? "#2196F3" : mau;
            BieuTuong = string.IsNullOrWhiteSpace(bieuTuong) ? "Category" : bieuTuong;
            TaoLuc = DateTime.UtcNow;
            CapNhatLuc = TaoLuc;
        }

        public void CapNhat(string ten, string? mau, string? bieuTuong, LoaiGiaoDich loai)
        {
            Ten = ten.Trim();
            Mau = string.IsNullOrWhiteSpace(mau) ? Mau : mau;
            BieuTuong = string.IsNullOrWhiteSpace(bieuTuong) ? BieuTuong : bieuTuong;
            Loai = loai;
            CapNhatLuc = DateTime.UtcNow;
        }
    }
}
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WpfApp3.Models
{
    [Table("GiaoDich")]
    public sealed class GiaoDich
    {
        private GiaoDich() { }

        [Key]
        public int Id { get; private set; }

        [Required, MaxLength(200)]
        public string NoiDung { get; private set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal SoTien { get; private set; }

        [Required]
        public DateTime Ngay { get; private set; }

        [Required]
        public LoaiGiaoDich Loai { get; private set; }

        public int DanhMucId { get; private set; }
        public DanhMuc DanhMuc { get; private set; }

        public int? NganSachId { get; private set; }
        public NganSach? NganSach { get; private set; }

        public int NguoiDungId { get; private set; }
        public NguoiDung NguoiDung { get; private set; }

        public DateTime TaoLuc { get; private set; }
        public DateTime CapNhatLuc { get; private set; }

        public bool DaHoanTien { get; private set; }
        public bool LaPhanBo { get; private set; }

        public GiaoDich(string noiDung, decimal soTien, DateTime ngay, LoaiGiaoDich loai, int nguoiDungId, int danhMucId)
        {
            NoiDung = noiDung.Trim();
            SoTien = soTien > 0 ? soTien : throw new ArgumentException();
            Ngay = ngay;
            Loai = loai;
            NguoiDungId = nguoiDungId;
            DanhMucId = danhMucId;
            TaoLuc = DateTime.UtcNow;
            CapNhatLuc = TaoLuc;
            DaHoanTien = false;
            LaPhanBo = false;
        }

        public void CapNhat(string noiDung, decimal soTien, DateTime ngay, LoaiGiaoDich loai, int danhMucId)
        {
            NoiDung = noiDung.Trim();
            SoTien = soTien > 0 ? soTien : throw new ArgumentException();
            Ngay = ngay;
            Loai = loai;
            DanhMucId = danhMucId;
            CapNhatLuc = DateTime.UtcNow;
        }

        public void GanNganSach(int nganSachId)
        {
            NganSachId = nganSachId;
            CapNhatLuc = DateTime.UtcNow;
        }

        public void HoanTien()
        {
            DaHoanTien = true;
            CapNhatLuc = DateTime.UtcNow;
        }

        public void PhanBo()
        {
            LaPhanBo = true;
            CapNhatLuc = DateTime.UtcNow;
        }
    }

    public enum LoaiGiaoDich
    {
        Thu = 1,
        Chi = 2
    }
}
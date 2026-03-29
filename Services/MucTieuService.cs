using Microsoft.EntityFrameworkCore;
using WpfApp3.Data;
using WpfApp3.Models;

namespace WpfApp3.Services
{
    public sealed class MucTieuService
    {
        private readonly IDbContextFactory<ChiTieuDbContext> _factory;

        public MucTieuService(IDbContextFactory<ChiTieuDbContext> factory)
        {
            _factory = factory;
        }

        public async Task<List<MucTieu>> LayDanhSach(int nguoiDungId)
        {
            await using var db = await _factory.CreateDbContextAsync();

            return await db.MucTieu
                .Where(x => x.NguoiDungId == nguoiDungId)
                .OrderByDescending(x => x.NgayMucTieu)
                .ToListAsync();
        }

        public async Task<MucTieu?> LayTheoId(int id, int nguoiDungId)
        {
            await using var db = await _factory.CreateDbContextAsync();

            return await db.MucTieu
                .FirstOrDefaultAsync(x => x.Id == id && x.NguoiDungId == nguoiDungId);
        }

        public async Task<bool> Tao(string ten, decimal mucTieuTien, DateTime ngay, int nguoiDungId, string? moTa = null)
        {
            await using var db = await _factory.CreateDbContextAsync();

            var mt = new MucTieu(ten, mucTieuTien, ngay, nguoiDungId, moTa);

            db.MucTieu.Add(mt);
            await db.SaveChangesAsync();

            return true;
        }

        public async Task<bool> CapNhat(int id, string ten, decimal mucTieuTien, DateTime ngay, string? moTa, int nguoiDungId)
        {
            await using var db = await _factory.CreateDbContextAsync();

            var mt = await db.MucTieu
                .FirstOrDefaultAsync(x => x.Id == id && x.NguoiDungId == nguoiDungId);

            if (mt == null) return false;

            mt.CapNhat(ten, mucTieuTien, ngay, moTa);

            await db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> Xoa(int id, int nguoiDungId)
        {
            await using var db = await _factory.CreateDbContextAsync();

            var mt = await db.MucTieu
                .FirstOrDefaultAsync(x => x.Id == id && x.NguoiDungId == nguoiDungId);

            if (mt == null) return false;

            db.MucTieu.Remove(mt);
            await db.SaveChangesAsync();

            return true;
        }

        public async Task<(bool ThanhCong, string ThongBao)> NapTien(int mucTieuId, decimal soTien, int nguoiDungId)
        {
            await using var db = await _factory.CreateDbContextAsync();

            var mt = await db.MucTieu
                .FirstOrDefaultAsync(x => x.Id == mucTieuId && x.NguoiDungId == nguoiDungId);

            if (mt == null)
                return (false, "Không tìm thấy mục tiêu");

            var soDu = await db.GiaoDich
                .Where(x => x.NguoiDungId == nguoiDungId
                    && x.Loai == LoaiGiaoDich.Thu
                    && !x.LaPhanBo
                    && !x.DaHoanTien)
                .SumAsync(x => x.SoTien)
                -
                await db.GiaoDich
                .Where(x => x.NguoiDungId == nguoiDungId
                    && x.Loai == LoaiGiaoDich.Chi
                    && !x.LaPhanBo)
                .SumAsync(x => x.SoTien);

            if (soTien > soDu)
                return (false, $"Không đủ tiền. Còn thiếu {(soTien - soDu):N0} ₫");

            var dm = await db.DanhMuc
                .FirstOrDefaultAsync(x => x.Ten == "Mục tiêu" && x.NguoiDungId == nguoiDungId);

            if (dm == null)
            {
                dm = new DanhMuc("Mục tiêu", LoaiGiaoDich.Chi, nguoiDungId);
                db.DanhMuc.Add(dm);
                await db.SaveChangesAsync();
            }

            var gd = new GiaoDich(
                $"Nạp vào mục tiêu: {mt.Ten}",
                soTien,
                DateTime.Now,
                LoaiGiaoDich.Chi,
                dm.Id,
                nguoiDungId
            );

            db.GiaoDich.Add(gd);

            mt.NapTien(soTien);

            await db.SaveChangesAsync();

            return mt.DaHoanThanh
                ? (true, $"Đã hoàn thành mục tiêu '{mt.Ten}'")
                : (true, $"Đã nạp {soTien:N0} ₫");
        }

        public string TrangThai(MucTieu mt)
        {
            if (mt.DaHoanThanh) return "Hoàn thành";

            var conLai = (mt.NgayMucTieu - DateTime.Now).Days;

            if (conLai < 0) return "Quá hạn";
            if (conLai <= 7) return "Sắp hết hạn";

            return "Đang thực hiện";
        }
    }
}
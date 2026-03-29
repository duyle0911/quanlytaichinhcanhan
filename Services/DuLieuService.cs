using Microsoft.EntityFrameworkCore;
using WpfApp3.Data;
using WpfApp3.Models;

namespace WpfApp3.Services
{
    public sealed class DuLieuService
    {
        private readonly IDbContextFactory<ChiTieuDbContext> _factory;

        public DuLieuService(IDbContextFactory<ChiTieuDbContext> factory)
        {
            _factory = factory;
        }

        public async Task<bool> CoDuLieu(int nguoiDungId)
        {
            await using var db = await _factory.CreateDbContextAsync();

            return await db.GiaoDich.AnyAsync(x => x.NguoiDungId == nguoiDungId)
                || await db.NganSach.AnyAsync(x => x.NguoiDungId == nguoiDungId)
                || await db.MucTieu.AnyAsync(x => x.NguoiDungId == nguoiDungId);
        }

        public async Task<List<ChiTieuDanhMuc>> LayChiTieuTheoDanhMuc(int nguoiDungId, DateTime tuNgay, DateTime denNgay)
        {
            await using var db = await _factory.CreateDbContextAsync();

            return await db.GiaoDich
                .Where(x => x.NguoiDungId == nguoiDungId
                    && x.Loai == LoaiGiaoDich.Chi
                    && x.Ngay >= tuNgay
                    && x.Ngay <= denNgay
                    && !x.LaPhanBo)
                .GroupBy(x => new { x.DanhMucId, x.DanhMuc.Ten, x.DanhMuc.Mau })
                .Select(g => new ChiTieuDanhMuc
                {
                    TenDanhMuc = g.Key.Ten,
                    Mau = g.Key.Mau,
                    SoTien = g.Sum(x => x.SoTien)
                })
                .OrderByDescending(x => x.SoTien)
                .ToListAsync();
        }

        public async Task<List<GiaoDich>> LayGanDay(int nguoiDungId, int soLuong = 10)
        {
            await using var db = await _factory.CreateDbContextAsync();

            return await db.GiaoDich
                .Include(x => x.DanhMuc)
                .Where(x => x.NguoiDungId == nguoiDungId)
                .OrderByDescending(x => x.TaoLuc)
                .Take(soLuong)
                .ToListAsync();
        }

        public async Task<List<GiaoDich>> LayTheoKy(int nguoiDungId, DateTime tuNgay, DateTime denNgay)
        {
            await using var db = await _factory.CreateDbContextAsync();

            return await db.GiaoDich
                .Include(x => x.DanhMuc)
                .Where(x => x.NguoiDungId == nguoiDungId
                    && x.Ngay >= tuNgay
                    && x.Ngay <= denNgay
                    && !x.LaPhanBo
                    && !x.DaHoanTien)
                .OrderByDescending(x => x.TaoLuc)
                .ToListAsync();
        }

        public async Task<TongQuanTaiChinh> TongQuan(int nguoiDungId)
        {
            var hienTai = DateTime.Now;
            var thang = hienTai.Month;
            var nam = hienTai.Year;

            var thu = await TongThuThang(nguoiDungId, thang, nam);
            var chi = await TongChiThang(nguoiDungId, thang, nam);

            var tongThu = await TongThu(nguoiDungId);
            var tongChi = await TongChi(nguoiDungId);

            var soDu = tongThu - tongChi;
            var tyLe = thu > 0 ? (thu - chi) / thu * 100 : 0;

            return new TongQuanTaiChinh
            {
                TongThu = tongThu,
                TongChi = tongChi,
                SoDu = soDu,
                ThuThang = thu,
                ChiThang = chi,
                TyLeTietKiem = tyLe
            };
        }

        public async Task<decimal> TongThu(int nguoiDungId)
        {
            await using var db = await _factory.CreateDbContextAsync();

            return await db.GiaoDich
                .Where(x => x.NguoiDungId == nguoiDungId
                    && x.Loai == LoaiGiaoDich.Thu
                    && !x.LaPhanBo
                    && !x.DaHoanTien)
                .SumAsync(x => x.SoTien);
        }

        public async Task<decimal> TongChi(int nguoiDungId)
        {
            await using var db = await _factory.CreateDbContextAsync();

            return await db.GiaoDich
                .Where(x => x.NguoiDungId == nguoiDungId
                    && x.Loai == LoaiGiaoDich.Chi
                    && !x.LaPhanBo)
                .SumAsync(x => x.SoTien);
        }

        public async Task<decimal> TongThuThang(int nguoiDungId, int thang, int nam)
        {
            await using var db = await _factory.CreateDbContextAsync();

            return await db.GiaoDich
                .Where(x => x.NguoiDungId == nguoiDungId
                    && x.Loai == LoaiGiaoDich.Thu
                    && x.Ngay.Month == thang
                    && x.Ngay.Year == nam
                    && !x.LaPhanBo
                    && !x.DaHoanTien)
                .SumAsync(x => x.SoTien);
        }

        public async Task<decimal> TongChiThang(int nguoiDungId, int thang, int nam)
        {
            await using var db = await _factory.CreateDbContextAsync();

            return await db.GiaoDich
                .Where(x => x.NguoiDungId == nguoiDungId
                    && x.Loai == LoaiGiaoDich.Chi
                    && x.Ngay.Month == thang
                    && x.Ngay.Year == nam
                    && !x.LaPhanBo)
                .SumAsync(x => x.SoTien);
        }

        public async Task<bool> Reset(int nguoiDungId)
        {
            await using var db = await _factory.CreateDbContextAsync();

            var gd = db.GiaoDich.Where(x => x.NguoiDungId == nguoiDungId);
            var ns = db.NganSach.Where(x => x.NguoiDungId == nguoiDungId);
            var mt = db.MucTieu.Where(x => x.NguoiDungId == nguoiDungId);
            var dm = db.DanhMuc.Where(x => x.NguoiDungId == nguoiDungId);

            db.GiaoDich.RemoveRange(gd);
            db.NganSach.RemoveRange(ns);
            db.MucTieu.RemoveRange(mt);
            db.DanhMuc.RemoveRange(dm);

            await db.SaveChangesAsync();
            return true;
        }
    }

    public class ChiTieuDanhMuc
    {
        public string TenDanhMuc { get; set; } = "";
        public decimal SoTien { get; set; }
        public string Mau { get; set; } = "#2196F3";
    }

    public class TongQuanTaiChinh
    {
        public decimal TongThu { get; set; }
        public decimal TongChi { get; set; }
        public decimal SoDu { get; set; }
        public decimal ThuThang { get; set; }
        public decimal ChiThang { get; set; }
        public decimal TyLeTietKiem { get; set; }
    }
}
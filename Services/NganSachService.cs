using Microsoft.EntityFrameworkCore;
using WpfApp3.Data;
using WpfApp3.Models;

namespace WpfApp3.Services
{
    public sealed class NganSachService
    {
        private readonly IDbContextFactory<ChiTieuDbContext> _factory;

        public NganSachService(IDbContextFactory<ChiTieuDbContext> factory)
        {
            _factory = factory;
        }

        public async Task<List<NganSach>> LayDanhSach(int nguoiDungId)
        {
            await using var db = await _factory.CreateDbContextAsync();

            return await db.NganSach
                .Include(x => x.DanhMuc)
                .Where(x => x.NguoiDungId == nguoiDungId)
                .OrderBy(x => x.DanhMuc.Ten)
                .ToListAsync();
        }

        public async Task<NganSach?> LayTheoId(int id, int nguoiDungId)
        {
            await using var db = await _factory.CreateDbContextAsync();

            return await db.NganSach
                .Include(x => x.DanhMuc)
                .FirstOrDefaultAsync(x => x.Id == id && x.NguoiDungId == nguoiDungId);
        }

        public async Task<bool> Tao(NganSach ns)
        {
            await using var db = await _factory.CreateDbContextAsync();

            var tonTai = await db.NganSach.AnyAsync(x =>
                x.DanhMucId == ns.DanhMucId &&
                x.NguoiDungId == ns.NguoiDungId &&
                x.BatDau <= ns.KetThuc &&
                x.KetThuc >= ns.BatDau);

            if (tonTai) return false;

            db.NganSach.Add(ns);
            await db.SaveChangesAsync();

            return true;
        }

        public async Task<bool> CapNhat(int id, decimal soTienMoi, DateTime batDau, DateTime ketThuc, bool trangThai)
        {
            await using var db = await _factory.CreateDbContextAsync();

            var ns = await db.NganSach.FirstOrDefaultAsync(x => x.Id == id);
            if (ns == null) return false;

            var trung = await db.NganSach.AnyAsync(x =>
                x.Id != id &&
                x.DanhMucId == ns.DanhMucId &&
                x.NguoiDungId == ns.NguoiDungId &&
                x.BatDau <= ketThuc &&
                x.KetThuc >= batDau);

            if (trung) return false;

            var daChi = await TinhDaChi(id);
            if (soTienMoi < daChi) return false;

            ns.CapNhat(ns.Ten, soTienMoi, batDau, ketThuc, ns.GhiChu);

            if (trangThai) ns.Mo();
            else ns.Khoa();

            await db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> Xoa(int id)
        {
            await using var db = await _factory.CreateDbContextAsync();

            var ns = await db.NganSach.FirstOrDefaultAsync(x => x.Id == id);
            if (ns == null) return false;

            db.NganSach.Remove(ns);
            await db.SaveChangesAsync();

            return true;
        }

        public async Task<bool> CongChi(int danhMucId, int nguoiDungId, decimal soTien, DateTime ngay)
        {
            await using var db = await _factory.CreateDbContextAsync();

            var ns = await db.NganSach.FirstOrDefaultAsync(x =>
                x.DanhMucId == danhMucId &&
                x.NguoiDungId == nguoiDungId &&
                x.TrangThai &&
                ngay >= x.BatDau &&
                ngay <= x.KetThuc);

            if (ns == null) return false;

            ns.CongChi(soTien);
            await db.SaveChangesAsync();

            return true;
        }

        public async Task<List<string>> CanhBao(int nguoiDungId)
        {
            await using var db = await _factory.CreateDbContextAsync();

            var ds = await db.NganSach
                .Include(x => x.DanhMuc)
                .Where(x => x.NguoiDungId == nguoiDungId && x.TrangThai)
                .ToListAsync();

            var kq = new List<string>();

            foreach (var ns in ds)
            {
                var tiLe = ns.SoTien == 0 ? 0 : (ns.DaChi / ns.SoTien) * 100;

                if (tiLe >= 100)
                    kq.Add($"{ns.DanhMuc.Ten} vượt {tiLe:F1}%");
                else if (tiLe >= 80)
                    kq.Add($"{ns.DanhMuc.Ten} đạt {tiLe:F1}%");
            }

            return kq;
        }

        public async Task<decimal> TinhDaChi(int nganSachId)
        {
            await using var db = await _factory.CreateDbContextAsync();

            return await db.GiaoDich
                .Where(x => x.NganSachId == nganSachId &&
                            x.Loai == LoaiGiaoDich.Chi &&
                            !x.LaPhanBo)
                .SumAsync(x => x.SoTien);
        }
    }
}
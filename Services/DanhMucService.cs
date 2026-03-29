using Microsoft.EntityFrameworkCore;
using WpfApp3.Data;
using WpfApp3.Models;

namespace WpfApp3.Services
{
    public sealed class DanhMucService
    {
        private readonly IDbContextFactory<ChiTieuDbContext> _factory;

        public DanhMucService(IDbContextFactory<ChiTieuDbContext> factory)
        {
            _factory = factory;
        }

        public async Task<List<DanhMuc>> LayDanhSach(int nguoiDungId, LoaiGiaoDich? loai = null)
        {
            await using var db = await _factory.CreateDbContextAsync();

            var query = db.DanhMuc
                .Where(x => x.NguoiDungId == nguoiDungId);

            if (loai.HasValue)
                query = query.Where(x => x.Loai == loai.Value);

            return await query
                .OrderBy(x => x.Ten)
                .ToListAsync();
        }

        public async Task<bool> Tao(string ten, LoaiGiaoDich loai, int nguoiDungId, string? mau = null, string? bieuTuong = null)
        {
            await using var db = await _factory.CreateDbContextAsync();

            var tonTai = await db.DanhMuc.AnyAsync(x =>
                x.Ten == ten &&
                x.Loai == loai &&
                x.NguoiDungId == nguoiDungId);

            if (tonTai) return false;

            var dm = new DanhMuc(ten, loai, nguoiDungId, mau, bieuTuong);

            db.DanhMuc.Add(dm);
            await db.SaveChangesAsync();

            return true;
        }

        public async Task<bool> CapNhat(int id, string ten, LoaiGiaoDich loai, string? mau, string? bieuTuong, int nguoiDungId)
        {
            await using var db = await _factory.CreateDbContextAsync();

            var dm = await db.DanhMuc
                .FirstOrDefaultAsync(x => x.Id == id && x.NguoiDungId == nguoiDungId);

            if (dm == null) return false;

            var trung = await db.DanhMuc.AnyAsync(x =>
                x.Id != id &&
                x.Ten == ten &&
                x.Loai == loai &&
                x.NguoiDungId == nguoiDungId);

            if (trung) return false;

            dm.CapNhat(ten, mau, bieuTuong, loai);

            await db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> Xoa(int id, int nguoiDungId)
        {
            await using var db = await _factory.CreateDbContextAsync();

            var dm = await db.DanhMuc
                .FirstOrDefaultAsync(x => x.Id == id && x.NguoiDungId == nguoiDungId);

            if (dm == null) return false;

            db.DanhMuc.Remove(dm);
            await db.SaveChangesAsync();

            return true;
        }
    }
}
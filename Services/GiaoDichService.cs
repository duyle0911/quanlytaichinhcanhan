using Microsoft.EntityFrameworkCore;
using WpfApp3.Data;
using WpfApp3.Models;

namespace WpfApp3.Services
{
    public sealed class GiaoDichService : IGiaoDichService
    {
        private readonly IDbContextFactory<ChiTieuDbContext> _factory;
        private readonly INganSachService _nganSach;

        public GiaoDichService(IDbContextFactory<ChiTieuDbContext> factory, INganSachService nganSach)
        {
            _factory = factory;
            _nganSach = nganSach;
        }

        public async Task<List<GiaoDich>> LayDanhSachAsync(int nguoiDungId, LoaiGiaoDich? loai = null)
        {
            await using var db = await _factory.CreateDbContextAsync();

            var query = db.GiaoDich
                .Include(x => x.DanhMuc)
                .Where(x => x.NguoiDungId == nguoiDungId && !x.LaPhanBo);

            if (loai.HasValue)
                query = query.Where(x => x.Loai == loai.Value);

            return await query
                .OrderByDescending(x => x.Ngay)
                .ToListAsync();
        }

        public async Task<GiaoDich?> LayTheoIdAsync(int id, int nguoiDungId)
        {
            await using var db = await _factory.CreateDbContextAsync();

            return await db.GiaoDich
                .Include(x => x.DanhMuc)
                .FirstOrDefaultAsync(x => x.Id == id && x.NguoiDungId == nguoiDungId);
        }

        public async Task<KetQua> TaoAsync(string moTa, decimal soTien, DateTime ngay, LoaiGiaoDich loai, int danhMucId, int nguoiDungId)
        {
            if (soTien <= 0)
                return ThatBai("Số tiền phải lớn hơn 0");

            if (string.IsNullOrWhiteSpace(moTa))
                return ThatBai("Thiếu mô tả");

            if (loai == LoaiGiaoDich.Chi)
            {
                var kt = await KiemTraChiAsync(soTien, danhMucId, nguoiDungId, ngay);
                if (!kt.HopLe) return ThatBai(kt.ThongBao);
            }

            await using var db = await _factory.CreateDbContextAsync();

            var gd = new GiaoDich(moTa, soTien, ngay, loai, danhMucId, nguoiDungId);

            if (loai == LoaiGiaoDich.Chi)
            {
                var ns = await _nganSach.LayTheoDanhMucVaNgayAsync(danhMucId, nguoiDungId, ngay);

                if (ns != null)
                {
                    gd.GanNganSach(ns.Id);
                    await _nganSach.CongChiAsync(danhMucId, nguoiDungId, soTien, ngay);
                }
            }

            db.GiaoDich.Add(gd);
            await db.SaveChangesAsync();

            return ThanhCong("Đã thêm giao dịch");
        }

        public async Task<KetQua> CapNhatAsync(int id, decimal soTien, string moTa, int danhMucId, DateTime ngay, int nguoiDungId)
        {
            await using var db = await _factory.CreateDbContextAsync();

            var gd = await db.GiaoDich.FirstOrDefaultAsync(x => x.Id == id && x.NguoiDungId == nguoiDungId);
            if (gd == null) return ThatBai("Không tìm thấy");

            if (soTien <= 0)
                return ThatBai("Số tiền phải lớn hơn 0");

            if (string.IsNullOrWhiteSpace(moTa))
                return ThatBai("Thiếu mô tả");

            var soTienCu = gd.SoTien;
            var danhMucCu = gd.DanhMucId;
            var ngayCu = gd.Ngay;

            if (gd.LaChi)
            {
                var kt = await KiemTraCapNhatChiAsync(id, soTien, danhMucId, nguoiDungId, ngay);
                if (!kt.HopLe) return ThatBai(kt.ThongBao);

                await _nganSach.CongChiAsync(danhMucCu, nguoiDungId, -soTienCu, ngayCu);
            }

            gd.CapNhat(moTa, soTien, ngay, danhMucId);

            if (gd.LaChi)
            {
                var ns = await _nganSach.LayTheoDanhMucVaNgayAsync(danhMucId, nguoiDungId, ngay);

                if (ns != null)
                {
                    gd.GanNganSach(ns.Id);
                    await _nganSach.CongChiAsync(danhMucId, nguoiDungId, soTien, ngay);
                }
                else
                {
                    gd.BoNganSach();
                }
            }

            await db.SaveChangesAsync();
            return ThanhCong("Đã cập nhật");
        }

        public async Task<KetQua> XoaAsync(int id, int nguoiDungId)
        {
            await using var db = await _factory.CreateDbContextAsync();

            var gd = await db.GiaoDich.FirstOrDefaultAsync(x => x.Id == id && x.NguoiDungId == nguoiDungId);
            if (gd == null) return ThatBai("Không tìm thấy");

            if (gd.LaChi && gd.CoNganSach)
            {
                await _nganSach.CongChiAsync(gd.DanhMucId, nguoiDungId, -gd.SoTien, gd.Ngay);
            }

            db.GiaoDich.Remove(gd);
            await db.SaveChangesAsync();

            return ThanhCong("Đã xóa");
        }

        public async Task<KiemTraChiTieu> KiemTraChiAsync(decimal soTien, int danhMucId, int nguoiDungId, DateTime ngay)
        {
            var ns = await _nganSach.LayTheoDanhMucVaNgayAsync(danhMucId, nguoiDungId, ngay);

            if (ns != null)
            {
                var con = ns.SoTien - ns.DaChi;
                if (soTien > con)
                    return Loi($"Hết ngân sách, còn {con:N0} ₫");
            }

            return HopLe();
        }

        public async Task<KiemTraChiTieu> KiemTraCapNhatChiAsync(int id, decimal soTienMoi, int danhMucId, int nguoiDungId, DateTime ngay)
        {
            var ns = await _nganSach.LayTheoDanhMucVaNgayAsync(danhMucId, nguoiDungId, ngay);

            if (ns != null)
            {
                var con = ns.SoTien - ns.DaChi;
                if (soTienMoi > con)
                    return Loi($"Hết ngân sách, còn {con:N0} ₫");
            }

            return HopLe();
        }

        static KetQua ThanhCong(string tb) => new() { ThanhCong = true, ThongBao = tb };
        static KetQua ThatBai(string tb) => new() { ThanhCong = false, ThongBao = tb };

        static KiemTraChiTieu HopLe() => new() { HopLe = true };
        static KiemTraChiTieu Loi(string tb) => new() { HopLe = false, ThongBao = tb };
    }
}
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using WpfApp3.Data;
using WpfApp3.Models;

namespace WpfApp3.Services
{
    public sealed class XacThucService
    {
        private readonly IDbContextFactory<ChiTieuDbContext> _factory;

        public XacThucService(IDbContextFactory<ChiTieuDbContext> factory)
        {
            _factory = factory;
        }

        public async Task<NguoiDung?> DangNhap(string tenDangNhap, string matKhau)
        {
            await using var db = await _factory.CreateDbContextAsync();

            var user = await db.NguoiDung
                .FirstOrDefaultAsync(x => x.TenDangNhap == tenDangNhap && x.TrangThai);

            if (user == null) return null;
            if (MaHoa(matKhau) != user.MatKhau) return null;

            user.CapNhatDangNhap();
            await db.SaveChangesAsync();

            return user;
        }

        public async Task<bool> DangKy(string tenDangNhap, string email, string matKhau, string? hoTen = null)
        {
            if (string.IsNullOrWhiteSpace(tenDangNhap) || tenDangNhap.Contains(' '))
                return false;

            await using var db = await _factory.CreateDbContextAsync();

            var tonTai = await db.NguoiDung
                .AnyAsync(x => x.TenDangNhap == tenDangNhap || x.Email == email);

            if (tonTai) return false;

            var user = new NguoiDung(tenDangNhap, email, MaHoa(matKhau));
            user.CapNhatThongTin(hoTen, null);

            db.NguoiDung.Add(user);
            await db.SaveChangesAsync();

            await TaoDanhMucMacDinh(user.Id);

            return true;
        }

        public async Task<bool> CapNhat(int id, string? hoTen, string? email, string? avatar)
        {
            await using var db = await _factory.CreateDbContextAsync();

            var user = await db.NguoiDung.FindAsync(id);
            if (user == null) return false;

            if (!string.IsNullOrWhiteSpace(email))
            {
                var trung = await db.NguoiDung
                    .AnyAsync(x => x.Email == email && x.Id != id);

                if (trung) return false;

                typeof(NguoiDung).GetProperty("Email")?.SetValue(user, email);
            }

            user.CapNhatThongTin(hoTen, avatar);

            await db.SaveChangesAsync();
            return true;
        }

        static string MaHoa(string input)
        {
            using var sha = SHA256.Create();
            var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(input));
            return Convert.ToBase64String(bytes);
        }

        async Task TaoDanhMucMacDinh(int nguoiDungId)
        {
            await using var db = await _factory.CreateDbContextAsync();

            var ds = new[]
            {
                new DanhMuc("Ăn uống", LoaiGiaoDich.Chi, nguoiDungId),
                new DanhMuc("Mua sắm", LoaiGiaoDich.Chi, nguoiDungId),
                new DanhMuc("Lương", LoaiGiaoDich.Thu, nguoiDungId),
                new DanhMuc("Thưởng", LoaiGiaoDich.Thu, nguoiDungId)
            };

            await db.DanhMuc.AddRangeAsync(ds);
            await db.SaveChangesAsync();
        }
    }
}
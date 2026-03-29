using Microsoft.EntityFrameworkCore;
using WpfApp3.Models;

namespace WpfApp3.Data
{
    public sealed class ChiTieuDbContext : DbContext
    {
        public ChiTieuDbContext(DbContextOptions<ChiTieuDbContext> options) : base(options) { }

        public DbSet<NguoiDung> NguoiDung => Set<NguoiDung>();
        public DbSet<GiaoDich> GiaoDich => Set<GiaoDich>();
        public DbSet<DanhMuc> DanhMuc => Set<DanhMuc>();
        public DbSet<NganSach> NganSach => Set<NganSach>();
        public DbSet<MucTieu> MucTieu => Set<MucTieu>();

        protected override void OnModelCreating(ModelBuilder mb)
        {
            var nguoiDung = mb.Entity<NguoiDung>();
            var giaoDich = mb.Entity<GiaoDich>();
            var danhMuc = mb.Entity<DanhMuc>();
            var nganSach = mb.Entity<NganSach>();
            var mucTieu = mb.Entity<MucTieu>();

            nguoiDung.HasIndex(x => x.TenDangNhap).IsUnique();
            nguoiDung.HasIndex(x => x.Email).IsUnique();

            giaoDich.Property(x => x.SoTien).HasPrecision(18, 2);

            giaoDich.HasOne(x => x.DanhMuc)
                    .WithMany(x => x.GiaoDiches)
                    .HasForeignKey(x => x.DanhMucId)
                    .OnDelete(DeleteBehavior.Cascade);

            giaoDich.HasOne(x => x.NganSach)
                    .WithMany()
                    .HasForeignKey(x => x.NganSachId)
                    .OnDelete(DeleteBehavior.NoAction);

            giaoDich.HasOne(x => x.NguoiDung)
                    .WithMany(x => x.GiaoDiches)
                    .HasForeignKey(x => x.NguoiDungId)
                    .OnDelete(DeleteBehavior.NoAction);

            danhMuc.HasOne(x => x.NguoiDung)
                   .WithMany(x => x.DanhMucs)
                   .HasForeignKey(x => x.NguoiDungId)
                   .OnDelete(DeleteBehavior.Cascade);

            nganSach.Property(x => x.SoTien).HasPrecision(18, 2);
            nganSach.Property(x => x.DaChi).HasPrecision(18, 2);

            nganSach.HasOne(x => x.DanhMuc)
                    .WithMany()
                    .HasForeignKey(x => x.DanhMucId)
                    .OnDelete(DeleteBehavior.Cascade);

            nganSach.HasOne(x => x.NguoiDung)
                    .WithMany(x => x.NganSaches)
                    .HasForeignKey(x => x.NguoiDungId)
                    .OnDelete(DeleteBehavior.NoAction);

            mucTieu.Property(x => x.MucTieuTien).HasPrecision(18, 2);
            mucTieu.Property(x => x.HienTai).HasPrecision(18, 2);

            mucTieu.HasOne(x => x.NguoiDung)
                   .WithMany(x => x.MucTieus)
                   .HasForeignKey(x => x.NguoiDungId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
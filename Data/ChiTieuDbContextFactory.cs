using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace WpfApp3.Data
{
    public sealed class ChiTieuDbContextFactory 
        : IDesignTimeDbContextFactory<NguCanhDuLieuChiTieu>
    {
        public NguCanhDuLieuChiTieu CreateDbContext(string[] args)
        {
            var cauHinh = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false)
                .Build();

            var chuoiKetNoi = cauHinh.GetConnectionString("Connection");

            var tuyChon = new DbContextOptionsBuilder<NguCanhDuLieuChiTieu>()
                .UseSqlServer(chuoiKetNoi)
                .Options;

            return new NguCanhDuLieuChiTieu(tuyChon);
        }
    }
}
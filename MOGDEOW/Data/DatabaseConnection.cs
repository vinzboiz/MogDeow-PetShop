using Microsoft.EntityFrameworkCore;
using MOGDEOW.Models;

namespace MOGDEOW.Data
{
    public static class DatabaseConnection
    {
        private static readonly string ConnectionString =
            "Server=ACER\\SQLEXPRESS;Database=MogdeowDB;Trusted_Connection=True;TrustServerCertificate=True;";

        // Hàm tạo DbContextOptions để kết nối Database
        public static DbContextOptions<MogdeowContext> GetDbContextOptions()
        {
            return new DbContextOptionsBuilder<MogdeowContext>()
                .UseSqlServer(ConnectionString)
                .Options;
        }

        // Hàm tạo DbContext tự động
        public static MogdeowContext CreateDbContext()
        {
            return new MogdeowContext(GetDbContextOptions());
        }
    }
}

using Microsoft.EntityFrameworkCore;
using MOGDEOW;
using MOGDEOW.Models;

namespace MOGDEOW.Data
{
    public class MogdeowContext : DbContext
    {
        public MogdeowContext(DbContextOptions<MogdeowContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<ProductType> ProductTypes { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Image> Images { get; set; }
        public DbSet<CartDetail> CartDetails { get; set; }
        public DbSet<Rating> Ratings { get; set; }
        public DbSet<Bill> Bills { get; set; }
        public DbSet<BillDetail> BillDetails { get; set; }
        public DbSet<Favorite> Favorites { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Favorite>()
            .HasKey(f => new { f.ProductID, f.UserID }); // Khóa chính kép

            modelBuilder.Entity<Favorite>()
                .HasOne(f => f.Product)
                .WithMany(p => p.Favorites)
                .HasForeignKey(f => f.ProductID);

            modelBuilder.Entity<Favorite>()
                .HasOne(f => f.User)
                .WithMany(u => u.Favorites)
                .HasForeignKey(f => f.UserID);

            // Thiết lập quan hệ giữa các bảng
            modelBuilder.Entity<Product>()
                .HasOne(p => p.ProductType)
                .WithMany(pt => pt.Products)
                .HasForeignKey(p => p.ProductTypeID)
                .OnDelete(DeleteBehavior.Restrict); // Tránh mất ProductTypeID khi xóa

            modelBuilder.Entity<Product>()
                .HasMany(p => p.Images)
                .WithOne(i => i.Product)
                .HasForeignKey(i => i.ProductID);

            modelBuilder.Entity<User>()
                .HasMany(u => u.Bills)
                .WithOne(b => b.User)
                .HasForeignKey(b => b.UserID);

            modelBuilder.Entity<CartDetail>()
                .HasOne(cd => cd.User)
                .WithMany()
                .HasForeignKey(cd => cd.UserID);

            modelBuilder.Entity<CartDetail>()
                .HasOne(cd => cd.Product)
                .WithMany(p => p.CartDetails)
                .HasForeignKey(cd => cd.ProductID);

            modelBuilder.Entity<Rating>()
                .HasOne(r => r.User)
                .WithMany()
                .HasForeignKey(r => r.UserID);

            modelBuilder.Entity<Rating>()
                .HasOne(r => r.Product)
                .WithMany(p => p.Ratings)
                .HasForeignKey(r => r.ProductID);

            modelBuilder.Entity<BillDetail>()
                .HasOne(bd => bd.Bill)
                .WithMany(b => b.BillDetails)
                .HasForeignKey(bd => bd.BillID)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<BillDetail>()
                .HasOne(bd => bd.Product)
                .WithMany(p => p.BillDetails)
                .HasForeignKey(bd => bd.ProductID);
            modelBuilder.Entity<Product>()
                .Property(p => p.ProductImage)
                .HasColumnType("NVARCHAR(MAX)"); // Ép kiểu dữ liệu cho EF Core
            modelBuilder.Entity<Bill>()
                .Property(b => b.Total)
                .HasColumnType("decimal(18,2)");
        }
    }
}

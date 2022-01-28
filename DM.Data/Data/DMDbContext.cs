using DM.Core.Entities;
using DM.Core.Entities.Auth;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace NM.Data.Data
{
    public class DMDbContext : IdentityDbContext<DMUser>
    {
        public DMDbContext(DbContextOptions<DMDbContext> options)
            : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<ShelfProduct>()
              .HasMany(x => x.NewProductHistories)
              .WithOne(x => x.NewShelfProduct);

            builder.Entity<ShelfProduct>()
             .HasMany(x => x.OldProductHistories)
             .WithOne(x => x.OldShelfProduct);
        }
        public DbSet<Exhibition> Exhibitions { get; set; }
        public DbSet<Shelf> Shelfs { get; set; }
        public DbSet<ShelfProduct> ShelfProducts { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductHistory> ProductHistories { get; set; }
    }
}

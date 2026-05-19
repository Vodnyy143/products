using Microsoft.EntityFrameworkCore;

namespace products.Models;

public class AppDbContext: DbContext
{
    public DbSet<Category> Categories { get; set; }
    public DbSet<Manufacturer> Manufacturers { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderProduct> OrderProducts { get; set; }
    public DbSet<OrderStatus> OrderStatuses { get; set; }
    public DbSet<PickupPoint> PickupPoints { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<Supplier> Suppliers { get; set; }
    public DbSet<Unit> Units { get; set; }
    public DbSet<User> Users { get; set; }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql("Host=localhost;Port=5433;Database=products;Username=postgres;Password=asdfjkl");
    }
}
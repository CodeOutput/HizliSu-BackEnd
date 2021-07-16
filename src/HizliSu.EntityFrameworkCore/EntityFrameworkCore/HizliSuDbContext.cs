using Microsoft.EntityFrameworkCore;
using Abp.Zero.EntityFrameworkCore;
using HizliSu.Address;
using HizliSu.Authorization.Roles;
using HizliSu.Authorization.Users;
using HizliSu.Catalog;
using HizliSu.General;
using HizliSu.MultiTenancy;
using HizliSu.Orders;
using HizliSu.Tax;

namespace HizliSu.EntityFrameworkCore
{
    public class HizliSuDbContext : AbpZeroDbContext<Tenant, Role, User, HizliSuDbContext>
    {
        /* Define a DbSet for each entity of the application */
        public DbSet<Manufacturer> Manufacturers { get; set; }
        public DbSet<File> Files { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<TaxRate> TaxRates { get; set; }
        public DbSet<Facility> Facilities { get; set; }
        public DbSet<FacilityAttribute> FacilityAttributes { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<District> Districts { get; set; }
        public DbSet<Neighborhood> Neighborhoods { get; set; }
        public DbSet<UserAddress> UserAddresses { get; set; }
        public DbSet<OrderStatus> OrderStatuses { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }

        public HizliSuDbContext(DbContextOptions<HizliSuDbContext> options)
            : base(options)
        {
        }
    }
}

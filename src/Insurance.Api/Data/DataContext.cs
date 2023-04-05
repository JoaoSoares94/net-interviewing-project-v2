using Insurance.Api.Model;
using Insurance.Api.Repositories.InsuranceRepository;
using Insurance.Api.Repositories.OrderRepo;
using Insurance.Api.Repositories.SurchargeRateRepo;
using Microsoft.EntityFrameworkCore;

namespace Insurance.Api.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {

        }
        public DbSet<InsuredProduct> InsuredProducts { get; set; }

        public DbSet<SurchargeRate> SurchargeRates { get; set; }

        public DbSet<Order> Orders { get; set; }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseInMemoryDatabase(databaseName: "testDb");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new InsuredProductTypeConfiguration());
            modelBuilder.ApplyConfiguration(new SurchargeRateTypeConfiguration());
            modelBuilder.ApplyConfiguration(new OrderTypeConfiguration());

        }
    }
}

using Insurance.Api.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Insurance.Api.Repositories.InsuranceRepository
{
    public class InsuredProductTypeConfiguration : IEntityTypeConfiguration<InsuredProduct>
    {
        public void Configure(EntityTypeBuilder<InsuredProduct> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.ProductId);
            builder.Property(x => x.Name);
            builder.Property(x => x.ProductTypeId);
            builder.Property(x => x.ProductTypeName);
            builder.Property(x => x.ProductTypeHasInsurance);
            builder.Property(x => x.SalesPrice);
            builder.Property(x => x.InsuranceValue);
        }
    }
}

using Insurance.Api.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Insurance.Api.Repositories.SurchargeRateRepo
{
    public class SurchargeRateTypeConfiguration : IEntityTypeConfiguration<SurchargeRate>
    {
        public void Configure(EntityTypeBuilder<SurchargeRate> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.ProductTypeId);
            builder.Property(x => x.ProductTypeName);
            builder.Property(x => x.Rate);

        }
    }
}
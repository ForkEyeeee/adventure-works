using System.Reflection.Metadata;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AdventureWorks.Server.Core;

namespace AdventureWorks.Server.Data;

public class PersonEntityTypeConfiguration : IEntityTypeConfiguration<BusinessEntity>
{
    public void Configure(EntityTypeBuilder<BusinessEntity> builder)
    {
        builder.ToTable("BusinessEntity");

        builder.HasKey(b => b.BusinessEntityID);

        builder.Property(b => b.BusinessEntityID)
            .ValueGeneratedOnAdd();

        builder.Property(b => b.rowguid)
            .ValueGeneratedOnAdd();

        builder.Property(b => b.ModifiedDate)
            .IsRequired()
            .HasDefaultValue(new DateTime(2000, 1, 1));
    }
}

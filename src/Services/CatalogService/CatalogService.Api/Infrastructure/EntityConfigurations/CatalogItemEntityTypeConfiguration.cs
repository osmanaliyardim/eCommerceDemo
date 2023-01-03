using CatalogService.Api.Core.Domain;
using CatalogService.Api.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CatalogService.Api.Infrastructure.EntityConfigurations;

public class CatalogItemEntityTypeConfiguration : IEntityTypeConfiguration<CatalogItem>
{
    public void Configure(EntityTypeBuilder<CatalogItem> builder)
    {
        builder.ToTable("Catalog", CatalogContext.DEFAULT_SCHEMA);

        builder.Property(ci => ci.Id)
            .UseHiLo("catalog_hilo")
            .IsRequired();

        builder.Property(cn => cn.Name)
            .IsRequired(true)
            .HasMaxLength(50);

        builder.Property(cp => cp.Price)
            .IsRequired(true);

        builder.Property(cpfn => cpfn.PictureFileName)
            .IsRequired(false);

        builder.Ignore(ci => ci.PictureUri);

        builder.HasOne(cb => cb.CatalogBrand)
            .WithMany()
            .HasForeignKey(cb => cb.CatalogBrandId);

        builder.HasOne(cb => cb.CatalogType)
            .WithMany()
            .HasForeignKey(cb => cb.CatalogTypeId);
    }
}
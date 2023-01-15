using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrderService.Domain.AggregateModels.BuyerAggregate;
using OrderService.Infrastructure.Context;

namespace OrderService.Infrastructure.EntityConfigurations;

public class CardTypeEntityConfiguration : IEntityTypeConfiguration<CardType>
{
    public void Configure(EntityTypeBuilder<CardType> cardTypeConfiguration)
    {
        cardTypeConfiguration.ToTable("cardtypes", OrderDbContext.DEFAULT_SCHEMA);

        cardTypeConfiguration.HasKey(ct => ct.Id);
        cardTypeConfiguration.Property(i => i.Id).HasColumnName("id").ValueGeneratedOnAdd();

        cardTypeConfiguration.Property(ct => ct.Id)
            .HasDefaultValue(1)
            .ValueGeneratedNever()
            .IsRequired();

        cardTypeConfiguration.Property(ct => ct.Name)
            .HasMaxLength(200)
            .IsRequired();
    }
}

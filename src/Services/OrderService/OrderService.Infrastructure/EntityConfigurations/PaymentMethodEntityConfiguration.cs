using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrderService.Domain.AggregateModels.BuyerAggregate;
using OrderService.Infrastructure.Context;

namespace OrderService.Infrastructure.EntityConfigurations;

public class PaymentMethodEntityConfiguration : IEntityTypeConfiguration<PaymentMethod>
{
    public void Configure(EntityTypeBuilder<PaymentMethod> paymentMethodConfiguration)
    {
        paymentMethodConfiguration.ToTable("paymentmethods", OrderDbContext.DEFAULT_SCHEMA);

        paymentMethodConfiguration.Ignore(b => b.DomainEvents);

        paymentMethodConfiguration.HasKey(o => o.Id);
        paymentMethodConfiguration.Property(i => i.Id).HasColumnName("id").ValueGeneratedOnAdd();

        paymentMethodConfiguration.Property<int>("BuyerId")
            .IsRequired();

        paymentMethodConfiguration.Property(i => i.CardHolderName)
            .UsePropertyAccessMode(PropertyAccessMode.Field)
            .HasColumnName("CardHolderName")
            .HasMaxLength(200)
            .IsRequired();

        paymentMethodConfiguration.Property(i => i.Alias)
            .UsePropertyAccessMode(PropertyAccessMode.Field)
            .HasColumnName("Alias")
            .HasMaxLength(200)
            .IsRequired();

        paymentMethodConfiguration.Property(i => i.CardNumber)
            .UsePropertyAccessMode(PropertyAccessMode.Field)
            .HasColumnName("CardNumber")
            .HasMaxLength(25)
            .IsRequired();

        paymentMethodConfiguration.Property(i => i.Expiration)
            .UsePropertyAccessMode(PropertyAccessMode.Field)
            .HasColumnName("Expiration")
            .HasMaxLength(25)
            .IsRequired();

        paymentMethodConfiguration.Property(i => i.CardTypeId)
            .UsePropertyAccessMode(PropertyAccessMode.Field)
            .HasColumnName("CardTypeId")
            .IsRequired();

        paymentMethodConfiguration.HasOne(p => p.CardType)
            .WithMany()
            .HasForeignKey(i => i.CardTypeId);
    }
}
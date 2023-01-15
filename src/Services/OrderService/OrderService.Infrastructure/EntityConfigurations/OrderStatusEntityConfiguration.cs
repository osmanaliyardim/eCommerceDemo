using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrderService.Domain.AggregateModels.OrderAggregate;
using OrderService.Infrastructure.Context;

namespace OrderService.Infrastructure.EntityConfigurations;

public class OrderStatusEntityConfiguration : IEntityTypeConfiguration<OrderStatus>
{
    public void Configure(EntityTypeBuilder<OrderStatus> orderStatusConfiguration)
    {
        orderStatusConfiguration.ToTable("orderstatus", OrderDbContext.DEFAULT_SCHEMA);

        orderStatusConfiguration.HasKey(o => o.Id);
        orderStatusConfiguration.Property(i => i.Id).ValueGeneratedOnAdd();

        orderStatusConfiguration.Property(o => o.Id)
            .HasDefaultValue(1)
            .ValueGeneratedNever()
            .IsRequired();

        orderStatusConfiguration.Property(o => o.Name)
            .HasMaxLength(200)
            .IsRequired();
    }
}
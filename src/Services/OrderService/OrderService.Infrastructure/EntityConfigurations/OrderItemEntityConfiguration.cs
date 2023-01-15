using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrderService.Domain.AggregateModels.OrderAggregate;
using OrderService.Infrastructure.Context;

namespace OrderService.Infrastructure.EntityConfigurations;

public class OrderItemEntityConfiguration : IEntityTypeConfiguration<OrderItem>
{
    public void Configure(EntityTypeBuilder<OrderItem> orderItemsConfiguration)
    {
        orderItemsConfiguration.ToTable("orderitems", OrderDbContext.DEFAULT_SCHEMA);

        orderItemsConfiguration.HasKey(o => o.Id);

        orderItemsConfiguration.Ignore(b => b.DomainEvents);

        orderItemsConfiguration.Property(o => o.Id).ValueGeneratedOnAdd();

        orderItemsConfiguration.Property<int>("OrderId").IsRequired();
    }
}
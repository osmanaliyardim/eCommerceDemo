using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;

namespace OrderService.Infrastructure.Context;

public class OrderDbContextSeed
{
    public async Task SeedAsync(OrderDbContext dbContext, ILogger<OrderDbContext> logger)
    {
        var policy = CreatePolicy(logger, nameof(OrderDbContextSeed)); // Polly : ToDo

        await policy.ExecuteAsync(async () =>
        {
        var useCustomizationData = false;
        var contentRootPath = "Seeding/Setup";

            using (dbContext)
            {
                dbContext.Database.Migrate();

                if (!dbContext.CardTypes.Any())
                {
                    dbContext.CardTypes.AddRange(useCustomizationData
                        ? GetCardTypesFromFile(contentRootPath, logger) // Not implemented : ToDo
                        : GetPredefinedCardTypes()); // Not implemented : ToDo

                    await dbContext.SaveChangesAsync();
                }

                if (!dbContext.OrderStatus.Any())
                {
                    dbContext.CardTypes.AddRange(useCustomizationData
                        ? GetOrderStatusFromFile(contentRootPath, logger) // Not implemented : ToDo
                        : GetPredefinedOrderStatus()); // Not implemented : ToDo
                }

                await dbContext.SaveChangesAsync();
            }
        });
    }
}
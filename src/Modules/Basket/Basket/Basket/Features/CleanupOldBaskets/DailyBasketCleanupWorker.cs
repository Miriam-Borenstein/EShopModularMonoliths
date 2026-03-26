using Basket.Basket.Features.DeleteBasket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Basket.Basket.Features.CleanupOldBaskets;


public class DailyBasketCleanupWorker(
    IServiceProvider serviceProvider,
    ILogger<DailyBasketCleanupWorker> logger,
    IConfiguration configuration)
    : BackgroundService
{

    private readonly NCrontab.CrontabSchedule _schedule =
        NCrontab.CrontabSchedule.Parse(configuration.GetValue<string>("BasketSettings:CleanupSchedule") ?? "0 3 * * *");

    private readonly SemaphoreSlim _semaphore = new(
        configuration.GetValue<int>("BasketSettings:MaxParallelism", 5));

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            
            await WaitForNextOccurrenceAsync(stoppingToken);

            await RunCleanupAsync(stoppingToken);
        }
    }

    
    private async Task WaitForNextOccurrenceAsync(CancellationToken stoppingToken)
    {
        var now = DateTime.UtcNow;
        var delay = _schedule.GetNextOccurrence(now) - now;

        if (delay > TimeSpan.Zero)
        {
            logger.LogInformation("Next BasketCleanup execution scheduled in {DelayHours} hours and {DelayMinutes} minutes", (int)delay.TotalHours, delay.Minutes);
            await Task.Delay(delay, stoppingToken);
        }
    }
    private async Task RunCleanupAsync(CancellationToken stoppingToken)
    {
        var expiredUserNames = await GetExpiredBasketUserNamesAsync(stoppingToken);

        if (expiredUserNames.Count == 0)
        {
            logger.LogInformation("No expired baskets found.");
            return;
        }

        await ProcessCleanupInParallelAsync(expiredUserNames, stoppingToken);
    }

    private async Task<List<string>> GetExpiredBasketUserNamesAsync(CancellationToken stoppingToken)
    {
        using var scope = serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<BasketDbContext>();

        int expirationDays = configuration.GetValue<int>("BasketSettings:ExpirationDays", 30);
        var threshold = DateTime.UtcNow.AddDays(-expirationDays);

        var expiredBasketsUserNames =  await dbContext.ShoppingCarts
            .AsNoTracking()
            .Where(c => c.LastModified < threshold)
            .Select(c => c.UserName)
            .ToListAsync(stoppingToken);

        return expiredBasketsUserNames;
    }

    private async Task ProcessCleanupInParallelAsync(List<string> expiredUserNames, CancellationToken stoppingToken)
    {

        logger.LogInformation("Starting parallel cleanup for {Count} baskets", expiredUserNames.Count);

        var tasks = expiredUserNames.Select(async userName =>
        {
            await _semaphore.WaitAsync(stoppingToken);
            try
            {
                await DeleteSingleBasketAsync(userName, stoppingToken);
            }
            finally
            {
                _semaphore.Release();
            }
        });

        await Task.WhenAll(tasks);
    }

    private async Task DeleteSingleBasketAsync(string userName, CancellationToken stoppingToken)
    {
        try
        {
            using var scope = serviceProvider.CreateScope();
            var mediator = scope.ServiceProvider.GetRequiredService<ISender>();
            var result = await mediator.Send(new DeleteBasketCommand(userName), stoppingToken);
            if (!result.IsSuccess)
            {
                logger.LogWarning("Cleanup Failure: Could not delete basket for user {User}", userName);
            }
            else {
                logger.LogInformation("Successfully deleted basket for user {User}", userName);
            }
        }
        catch (Exception ex) {
            logger.LogError(ex, "Critical error during individual basket deletion for {User}", userName);
        }
        

    }
}

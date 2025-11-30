using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using project1.Infrastructure.Data;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace project1.Application.Services
{
    public class RefreshTokenCleanupService : BackgroundService
    {
        private readonly IServiceProvider _provider;
        private readonly ILogger<RefreshTokenCleanupService> _logger;
        private readonly TimeSpan _interval;
        private readonly int _deleteAfterDays;

        public RefreshTokenCleanupService(IServiceProvider provider, ILogger<RefreshTokenCleanupService> logger, Microsoft.Extensions.Configuration.IConfiguration config)
        {
            _provider = provider;
            _logger = logger;
            _interval = TimeSpan.FromMinutes(config.GetValue<int>("RefreshTokenCleanup:CleanupIntervalMinutes"));
            _deleteAfterDays = config.GetValue<int>("RefreshTokenCleanup:DeleteExpiredAfterDays");
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _provider.CreateScope();
                    var db = scope.ServiceProvider.GetRequiredService<SchoolDbContext>();
                    var cutoff = DateTime.UtcNow.AddDays(-_deleteAfterDays);
                    var toDelete = await db.RefreshTokens.Where(rt => rt.Expires < DateTime.UtcNow && rt.Created < cutoff).ToListAsync(stoppingToken);
                    if (toDelete.Any())
                    {
                        db.RefreshTokens.RemoveRange(toDelete);
                        await db.SaveChangesAsync(stoppingToken);
                        _logger.LogInformation("Deleted {Count} expired refresh tokens", toDelete.Count);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error cleaning refresh tokens");
                }

                await Task.Delay(_interval, stoppingToken);
            }
        }
    }
}

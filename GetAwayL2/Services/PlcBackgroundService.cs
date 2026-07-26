using Microsoft.EntityFrameworkCore.Internal;

namespace GetAwayL2.Services
{
    public class PlcBackgroundService : BackgroundService
    {
        private readonly PLKConnectionSendService plkService;
        private readonly IServiceScopeFactory scopeFactory;
        private readonly ILogger<PlcBackgroundService> logger;

        public PlcBackgroundService(PLKConnectionSendService plkService, IServiceScopeFactory scopeFactory, ILogger<PlcBackgroundService> logger)
        {
            this.plkService = plkService;
            this.scopeFactory = scopeFactory;
            this.logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            string plcHost = "127.0.0.1";
            int plcPort = 5000;

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using (var scope = scopeFactory.CreateScope())
                    {
                        var db = scope.ServiceProvider.GetRequiredService<LogDBContext>();

                        await plkService.Run(plcHost, plcPort);
                    }

                    if (!stoppingToken.IsCancellationRequested)
                    {
                        await Task.Delay(5000, stoppingToken);
                    }
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    await Task.Delay(5000, stoppingToken);
                }
            }
        }


    }
}

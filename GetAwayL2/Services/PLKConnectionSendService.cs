using GetAwayL2.Utilities;
using Polly;
using System.Net.Sockets;

namespace GetAwayL2.Services
{
    public class PLKConnectionSendService
    {

        private readonly IServiceScopeFactory scopeFactory;

        public PLKConnectionSendService(IServiceScopeFactory scopeFactory)
        {
            this.scopeFactory = scopeFactory;
        }

        public async Task<bool> IsPortOpenAsync(string host, int port, int timeoutMs = 1000)
        {
            // Проверка доступности порта на хосте
            try
            {
                using var client = new TcpClient();
                var connectTask = client.ConnectAsync(host, port);
                var finished = await Task.WhenAny(connectTask, Task.Delay(timeoutMs));
                return finished == connectTask && client.Connected;
            }
            catch
            {
                return false;
            }
        }
        public async Task Run(string host, int port)
        {
            using (var scope = scopeFactory.CreateScope())
            {
                var logger = scope.ServiceProvider.GetRequiredService<ILoggerService>();
                var plkService = new InteractionPLKService(logger);

                bool isPortOpen = await IsPortOpenAsync(host, port);
                if (isPortOpen)
                {
                    string message = await plkService.FormingMsg4PLK();
                    var retryPolicy = Policy.Handle<SocketException>().Or<TimeoutException>()
                        .WaitAndRetryAsync(
                            retryCount: 3, 
                            sleepDurationProvider: attempt => ExponentialProvider.Calculate(attempt),
                            onRetry: (exception, timeSpan, retryCount, context) =>{
                                    logger.LogDB(
                                     name: "PLK",
                                     camMsgRequest: message,
                                     message: null,
                                     error: $"Ошибка отправки: {exception.Message} (попытка {retryCount})",
                                     isValid: false);
                            }
                        );

                    await retryPolicy.ExecuteAsync(async () =>
                    {
                        await plkService.SendStringAsync(host, port, message);
                    });

                    await plkService.LogDB(message, null);
                }
                else
                {
                    string message = await plkService.FormingMsg4PLK();
                    await plkService.LogDB(message, $"Port {port} on host {host} is not open");
                }
            }


        }
    }
}

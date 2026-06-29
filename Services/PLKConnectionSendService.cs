using System.Net.Sockets;

namespace GetAwayL2.Services
{
    public class PLKConnectionSendService
    {

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
        public async Task Run(string host, int port, LogDBContext dbContext)
        {
            var plkService = new InteractionPLKService(dbContext);
            bool isPortOpen = await IsPortOpenAsync(host, port);
            if (isPortOpen) {
                string message = await plkService.FormingMsg4PLK();
                await plkService.SendStringAsync(host, port, message);
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

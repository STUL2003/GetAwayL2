using System.Net;
using System.Net.Sockets;
using System.Text;
namespace GetAwayL2.Services
{

    public class TCPListennerServicecs : BackgroundService
    {
        const int port = 22822;
        private readonly IServiceScopeFactory scopeFactory;

        public TCPListennerServicecs(IServiceScopeFactory scopeFactory)
        {
            this.scopeFactory = scopeFactory;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            
            var listener = new TcpListener(IPAddress.Any, port);
            listener.Start();

            while (true)
            {
                var client = await listener.AcceptTcpClientAsync();
                _ = LestenerMsgFromCamAsync(client, stoppingToken);
            }
        }
        public async Task LestenerMsgFromCamAsync(TcpClient client, CancellationToken stoppingToken)
        {
            var buffer = new byte[1024];
            var sb = new StringBuilder();
            using (client)
            using (var stream = client.GetStream())
            {
                while(true) {
                    int bytesRead;
                    try
                    {
                        bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                    }
                    catch (IOException)
                    {
                        Console.WriteLine("Клиент отключился (соединение разорвано)");
                        break;
                    }

                    if (bytesRead == 0)
                    {
                        Console.WriteLine("Клиент закрыл соединение");
                        break;
                    }

                    sb.Append(Encoding.UTF8.GetString(buffer, 0, bytesRead));
                    var actualsb = sb.ToString();
                    using (var scope = scopeFactory.CreateScope())
                    {
                        var parser = scope.ServiceProvider.GetRequiredService<ParserCameraMsgService>();
                        parser.SetRequest(actualsb);
                        await parser.ParseCamMsgAsync();
                        await parser.LogDB();
                    }

                }
            
            }
        }
    }
}

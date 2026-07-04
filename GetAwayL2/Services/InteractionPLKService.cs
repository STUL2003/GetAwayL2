using GetAwayL2.Models;
using Microsoft.EntityFrameworkCore;
using System.Net.Sockets;
using System.Text;


namespace GetAwayL2.Services
{
    public class InteractionPLKService : IInteractionPLK
    {
        private string msg;
        private const string mainCod = "01";
        private const string markStart = "90";
        private const string markEnd = "91";
        private readonly ILoggerService logger;
        private bool isValid;
        public InteractionPLKService(ILoggerService logger)
        {
            this.logger = logger;
        }
        public async Task GetMsg4PLK()
        {
            // Получение сообщения из канала
            var ch = ChannelsByName.GetOrCreate<string>("ChannelMsg");

            if (await ch.Reader.WaitToReadAsync())
            {
                if (ch.Reader.TryRead(out var message))
                {
                    msg = message;
                }
            }

        }
        public async Task<string> FormingMsg4PLK()
        {
            // Формирование сообщения для ПЛК
            await GetMsg4PLK();
            string msgCode = msg switch
            {
                "READ" => "1010",
                "NOREAD" => "1020",
                _ => "1030"
            };
            isValid = msgCode == "1030" ? false : true;
            string fullMsg = $"#{markStart}{mainCod}{msgCode}#{markEnd}";
            return fullMsg;

        }


        async public Task SendStringAsync(string host, int port, string message)
        {
            // Отправка сообщения на TCP-server
                using var client = new TcpClient();
                await client.ConnectAsync(host, port);

                NetworkStream stream = client.GetStream();
                byte[] data = Encoding.UTF8.GetBytes(message + "\n");
                await stream.WriteAsync(data, 0, data.Length);
                await stream.FlushAsync();
        }

        public async Task LogDB(string fullMsg, string? error)
        {
            // Логирование в базу данных
            await logger.LogDB(isValid, null, "PLK", fullMsg, error);
        }

    }
}


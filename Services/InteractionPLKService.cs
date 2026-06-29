using GetAwayL2.Models;
using Microsoft.EntityFrameworkCore;
using System.Net.Sockets;
using System.Text;


namespace GetAwayL2.Services
{
    public class InteractionPLKService : IInteractionPLK
    {
        private string msg;
        const string mainCod = "01";
        const string markStart = "90";
        const string markEnd = "91";

        private readonly LogDBContext dbContext;
        private bool isValid;
        public InteractionPLKService(LogDBContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public async Task GetMsg4PLK()
        {
            // Получение сообщения из канала
            var ch = ChannelsByName.GetOrCreate<string>("ChannelMsg");
            //await foreach (var x in ch.Reader.ReadAllAsync())
            //{
            //    msg = x;
            //}
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
            await LoggerService.LogDB(dbContext, isValid, null, "PLK", fullMsg, error);
        }

    }
}


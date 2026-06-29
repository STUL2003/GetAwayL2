using GetAwayL2;

namespace GetAwayL2.Services
{
    public interface IInteractionPLK // интерфейс для реализаци класса по взаимодействию с ПЛК
    {

        public Task GetMsg4PLK(); // получение сообщения от ПЛК
        public Task<string> FormingMsg4PLK(); // формирование сообщения 
        public Task SendStringAsync(string host, int port, string message); // отправка сообщения 
        public Task LogDB(string fullMsg, string? error); //логирование
    }
}
//var ch = ChannelsByName.GetOrCreate<string>("myChannel");
//await foreach (var x in ch.Reader.ReadAllAsync(ct))
//{
//    Console.WriteLine(x);
//}

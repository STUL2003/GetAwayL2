using GetAwayL2;

namespace GetAwayL2.Services
{
    public interface IInteractionPLK
    {
        public Task GetMsg4PLK();
        public Task<string> FormingMsg4PLK();
        public Task SendStringAsync(string host, int port, string message);
        public Task LogDB(string fullMsg, string? error);
    }
}
//var ch = ChannelsByName.GetOrCreate<string>("myChannel");
//await foreach (var x in ch.Reader.ReadAllAsync(ct))
//{
//    Console.WriteLine(x);
//}

using System.Net.Sockets;
using System.Text;

class CameraEmulator
{
    static async Task Main(string[] args)
    {
        var random = new Random();
        while (true)
        {
            using var client = new TcpClient();
            await client.ConnectAsync("127.0.0.1", 22822);
            using var stream = client.GetStream();
            string message = random.Next(2) == 0 ? "READ" : "NOREAD";
            byte[] data = Encoding.UTF8.GetBytes(message + "\n");
            await stream.WriteAsync(data, 0, data.Length);
            await stream.FlushAsync();
            Console.WriteLine($"Отправлено: {message}");
            await Task.Delay(random.Next(2000, 5000));
        }
    }
}
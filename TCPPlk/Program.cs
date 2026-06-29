using System.Net;
using System.Net.Sockets;
using System.Text;

class TcpTestServer
{
    static async Task Main(string[] args)
    {
        int port = 5000;
        var listener = new TcpListener(IPAddress.Any, port);
        listener.Start();
        Console.WriteLine($"TCP-сервер запущен на порту {port}");

        while (true)
        {
            var client = await listener.AcceptTcpClientAsync();
            _ = HandleClientAsync(client);
        }
    }

    static async Task HandleClientAsync(TcpClient client)
    {
        Console.WriteLine($"Клиент подключён: {client.Client.RemoteEndPoint}");
        using (client)
        using (var stream = client.GetStream())
        {
            byte[] buffer = new byte[1024];
            while (true)
            {
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

                string received = Encoding.UTF8.GetString(buffer, 0, bytesRead).TrimEnd('\n', '\r');
                Console.WriteLine($"Получено: {received}");

                string response = $"OK: {received}\n";
                byte[] responseBytes = Encoding.UTF8.GetBytes(response);
                await stream.WriteAsync(responseBytes, 0, responseBytes.Length);
                await stream.FlushAsync();
            }
        }
        Console.WriteLine("Клиент отключён");
    }
}
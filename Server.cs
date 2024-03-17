using System.Net.Sockets;
using System.Net;
using System.Text;


class Server
{
    public static List<TcpClient> Clients = new List<TcpClient>();


    public static async Task ProcessClient(TcpClient client)
    {
        var stream = client.GetStream();
        List<byte> bytes = new List<byte>();

        while (true)
        {
            int bytes_read = 0;

            while ((bytes_read = stream.ReadByte()) != '\0')
            {
                bytes.Add((byte)bytes_read);
            }

            string str_user = Encoding.UTF8.GetString(bytes.ToArray());
            User user = Newtonsoft.Json.JsonConvert.DeserializeObject<User>(str_user);

            bytes.Clear();
        }
    }


    public static async Task Main(string[] args)
    {
        TcpListener tcp_listener = new TcpListener(IPAddress.Parse("127.0.0.1"), 9010);
        tcp_listener.Start();

        Console.WriteLine("Server started...\n");


        while (true)
        {
            TcpClient tcp_client = await tcp_listener.AcceptTcpClientAsync();

            await Console.Out.WriteLineAsync("Client started...\n");

            Clients.Add(tcp_client);

            _ = Task.Run(async () => await ProcessClient(Clients[Clients.Count - 1]));
        }
    }
}
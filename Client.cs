using System.Net.Sockets;
using System.Net;
using System.Text;

class Client
{
    public static async Task Autentification(TcpClient tcp_client)
    {
        await Console.Out.WriteAsync("you want to:\n1) register\n2) log in\nyour choice: ");

        string str_choice = await Console.In.ReadLineAsync();
        char choice = str_choice[0];


        User user = new User();

        await Console.Out.WriteAsync("\nLogin: ");
        user.Login = await Console.In.ReadLineAsync();

        await Console.Out.WriteAsync("Password: ");
        user.Password = await Console.In.ReadLineAsync();

        user.ID = Guid.NewGuid().ToString();

        user.IsNewUser = (choice == '1' ? true : false);


        string str_user = Newtonsoft.Json.JsonConvert.SerializeObject(user);
        str_user += '\0';

        Console.WriteLine(str_user);

        var stream = tcp_client.GetStream();

        byte[] bytes = Encoding.UTF8.GetBytes(str_user.ToArray());

        await stream.WriteAsync(bytes);
    }

    public static async Task Main(string[] args)
    {
        TcpClient tcp_client = new TcpClient();

        await tcp_client.ConnectAsync(IPAddress.Parse("127.0.0.1"), 9010);
        await Console.Out.WriteLineAsync("Connected...\n");


        await Autentification(tcp_client);
    }
}
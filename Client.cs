using System.Net.Sockets;
using System.Net;
using System.Text;


enum ErrorMessage
{
    UserWithTheSameLoginIsAlreadyExist,
    WrongLoginOrPassword
}

class User
{
    public string Login { get; set; }

    public string Password { get; set; }

    public string ID { get; set; }

    public bool IsNewUser { get; set; }
}

class Message
{
    public bool IsAllOK { get; set; }

    public ErrorMessage error { get; set; }
}


class Interaction
{
    public static async Task WriteToStream(TcpClient client, string message)
    {
        var stream = client.GetStream();
        message += '\0';

        byte[] bytes = Encoding.UTF8.GetBytes(message.ToArray());

        await stream.WriteAsync(bytes);
    }


    public static string ReadfromStream(TcpClient client)
    {
        var stream = client.GetStream();
        List<byte> bytes = new List<byte>();


        int bytes_read = 0;

        while ((bytes_read = stream.ReadByte()) != '\0')
        {
            bytes.Add((byte)bytes_read);
        }

        string str = Encoding.UTF8.GetString(bytes.ToArray());

        return str;
    }
}


class Client
{
    public static async Task Autentification(TcpClient client)
    {
        await Console.Out.WriteAsync("\nyou want to:\n1) register\n2) log in\nyour choice: ");

        string str_choice = await Console.In.ReadLineAsync();
        char choice = str_choice[0];


        User user = new User();

        await Console.Out.WriteAsync("\nLogin: ");
        user.Login = await Console.In.ReadLineAsync();

        await Console.Out.WriteAsync("Password: ");
        user.Password = await Console.In.ReadLineAsync();

        user.IsNewUser = (choice == '1' ? true : false);


        string str_user = Newtonsoft.Json.JsonConvert.SerializeObject(user);

        
        await Interaction.WriteToStream(client, str_user);

        string message = Interaction.ReadfromStream(client);

        Message answer = Newtonsoft.Json.JsonConvert.DeserializeObject<Message>(message);

        if (answer.IsAllOK)
        {
            Console.WriteLine("\n\nyou are in!\n");
        }
        else
        {
            Console.WriteLine("\n\nError: " + answer.error.ToString());

            Autentification(client);
        }
    }

    public static async Task Main(string[] args)
    {
        TcpClient tcp_client = new TcpClient();

        await tcp_client.ConnectAsync(IPAddress.Parse("127.0.0.1"), 12345);
        await Console.Out.WriteLineAsync("Connected...\n");


        await Autentification(tcp_client);
    }
}

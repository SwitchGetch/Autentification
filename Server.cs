using System.Net.Sockets;
using System.Net;
using System.Text;


enum ErrorMessage
{
    None,
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


class Server
{
    public static List<TcpClient> Clients = new List<TcpClient>();
    public static List<User> Users = new List<User>();

    public static bool CheckUser(User user)
    {
        if (
            user.IsNewUser && Users.Find(x => x.Login == user.Login) == null ||
            !user.IsNewUser && Users.Find(x => x.Login == user.Login && x.Password == user.Password) != null
            )
        {
            return true;
        }

        return false;
    }

    public static async Task ProcessClient(TcpClient client)
    {
        while (true)
        {
            User user = Newtonsoft.Json.JsonConvert.DeserializeObject<User>(Interaction.ReadfromStream(client));
            


            Message answer = new Message();

            answer.IsAllOK = CheckUser(user);
            
            if (answer.IsAllOK)
            {
                Users.Add(user);
                answer.error = ErrorMessage.None;
            }
            else
            {
                answer.error = (user.IsNewUser ? ErrorMessage.UserWithTheSameLoginIsAlreadyExist : ErrorMessage.WrongLoginOrPassword);
            }


            string str_answer = Newtonsoft.Json.JsonConvert.SerializeObject(answer);

            
            await Interaction.WriteToStream(client, str_answer);
        }
    }


    public static async Task Main(string[] args)
    {
        TcpListener tcp_listener = new TcpListener(IPAddress.Parse("127.0.0.1"), 12345);
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

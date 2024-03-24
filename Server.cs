using System.Net.Sockets;
using System.Net;
using System.Text;

using Classes;


class Server
{
    public static List<TcpClient> Clients = new List<TcpClient>();
    public static List<User> Users = new List<User>();

    public static void CheckUser(User user, ref Message answer)
    {
        if (user.IsNewUser)
        {
            if (Users.Find(x => x.Login == user.Login) != null)
            {
                answer.IsAllOK = false;
                answer.error = ErrorMessage.UserWithTheSameLoginIsAlreadyExist;
            }
            else
            {
                user.IsNewUser = false;
                Users.Add(user);

                Json.UploadToFile(Users);
            }
        }
        else
        {
            if (Users.Find(x => x.Login == user.Login && x.Password == user.Password) == null)
            {
                answer.IsAllOK = false;
                answer.error = ErrorMessage.WrongLoginOrPassword;
            }
        }
    }

    public static async Task ProcessClient(TcpClient client)
    {
        while (true)
        {
            User user = Newtonsoft.Json.JsonConvert.DeserializeObject<User>(Interaction.ReadfromStream(client));

            Message answer = new Message() { IsAllOK = true, error = ErrorMessage.None };

            CheckUser(user, ref answer);

            await Interaction.WriteToStream(client, Newtonsoft.Json.JsonConvert.SerializeObject(answer));
        }
    }


    public static async Task Main(string[] args)
    {
        TcpListener tcp_listener = new TcpListener(IPAddress.Parse("127.0.0.1"), 12345);
        tcp_listener.Start();

        Console.WriteLine("Server started...\n");


        Json.DownloadFromFile(ref Users);

        while (true)
        {
            TcpClient tcp_client = await tcp_listener.AcceptTcpClientAsync();

            await Console.Out.WriteLineAsync("Client started...\n");

            Clients.Add(tcp_client);

            _ = Task.Run(async () => await ProcessClient(Clients[Clients.Count - 1]));
        }
    }
}

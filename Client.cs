using System.Net.Sockets;
using System.Net;
using System.Text;

using Classes;


class Client
{
    public static bool GetAnswer(TcpClient client)
    {
        string server_answer = Interaction.ReadfromStream(client);

        Message answer = Newtonsoft.Json.JsonConvert.DeserializeObject<Message>(server_answer);

        if (answer.IsAllOK)
        {
            Console.WriteLine("\n\nyou are in!\n");

            return true;
        }
        else
        {
            Console.WriteLine("\nError: " + answer.error.ToString());

            return false;
        }
    }

    public static async Task Autentification(TcpClient client)
    {
        await Console.Out.WriteAsync("\nyou want to:\n1) register\n2) log in\nyour choice: ");

        string str_choice = Console.ReadLine();
        char choice = str_choice[0];


        User user = new User();

        await Console.Out.WriteAsync("\nLogin: ");
        user.Login = Console.ReadLine();

        await Console.Out.WriteAsync("Password: ");
        user.Password = Console.ReadLine();

        user.IsNewUser = choice == '1';


        string str_user = Newtonsoft.Json.JsonConvert.SerializeObject(user);


        await Interaction.WriteToStream(client, str_user);
    }

    public static async Task Main(string[] args)
    {
        TcpClient tcp_client = new TcpClient();

        await tcp_client.ConnectAsync(IPAddress.Parse("127.0.0.1"), 12345);
        await Console.Out.WriteLineAsync("Connected...\n");


        while (true)
        {
            await Autentification(tcp_client);

            if (GetAnswer(tcp_client))
            {
                break;
            }
        }

    }
}

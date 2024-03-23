using System.Net.Sockets;
using System.Text;

namespace Classes
{
    public enum ErrorMessage
    {
        None,
        UserWithTheSameLoginIsAlreadyExist,
        WrongLoginOrPassword
    }

    public class User
    {
        public string Login { get; set; }

        public string Password { get; set; }

        public string ID { get; set; }

        public bool IsNewUser { get; set; }
    }

    public class Message
    {
        public bool IsAllOK { get; set; }

        public ErrorMessage error { get; set; }
    }


    public class Interaction
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
}

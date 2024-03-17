public enum ErrorMessage
{
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
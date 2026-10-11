
namespace e_commerce_web_api.Exceptions;

public class InvalidCredentialsException : Exception
{
    public InvalidCredentialsException()
        : base("Invalid email or password.")
    {
    }
}

public class EmailAlreadyExistsException : Exception
{
    public EmailAlreadyExistsException()
        : base("This email already exists.")
    {
    }
}

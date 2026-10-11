
namespace e_commerce_web_api.Exceptions;

public class EmailAlreadyExistsException : Exception
{
    public EmailAlreadyExistsException()
        : base("This email already exists.")
    {
    }
}



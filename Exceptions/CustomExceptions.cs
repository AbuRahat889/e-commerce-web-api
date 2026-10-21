
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


public class EmailNotVerifiedException : Exception
{
    public EmailNotVerifiedException()
        : base("Your email is not verified. Please verify your email first.")
    {
    }
}

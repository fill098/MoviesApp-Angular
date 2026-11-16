namespace MovieApp.Common.Extensinons;

public class ForbiddenException : AppException
{
    public ForbiddenException(string message) : base(message)
    {
    }
}

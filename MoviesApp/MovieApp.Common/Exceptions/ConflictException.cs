namespace MovieApp.Common.Extensinons;

public class ConflictException : AppException
{
    public ConflictException(string message) : base(message)
    {
    }
}

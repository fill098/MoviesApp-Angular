namespace MovieApp.Common.Extensinons;

public abstract class AppException : Exception
{
    protected AppException(string message) : base(message)
    {
    }
}

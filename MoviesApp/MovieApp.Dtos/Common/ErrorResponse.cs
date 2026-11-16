namespace MovieApp.Dtos.Common;

public class ErrorResponse
{
    public int StatusCode { get; set; }
    public string Message { get; set; } = string.Empty;

    /// <summary>Details, e.g. one line per validation error. Empty when there are none.</summary>
    public List<string> Errors { get; set; } = [];

    /// <summary>Identifies the request in the server logs; handy when reporting a bug.</summary>
    public string? TraceId { get; set; }
}

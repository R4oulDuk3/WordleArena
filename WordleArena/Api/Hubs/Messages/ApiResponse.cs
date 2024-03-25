using TypeGen.Core.TypeAnnotations;

namespace WordleArena.Api.Hubs.Messages;

[ExportTsInterface(OutputDir = "messages")]
public class ApiResponse<T>(ApiResponseStatusCode statusCode, T data, string message = "")
{
    public ApiResponseStatusCode StatusCode { get; set; } = statusCode;
    public T? Data { get; set; } = data;
    public string Message { get; set; } = message;
}
namespace CleanArch.Domain.Abstractions;

public class Response<T>
{
    public T? Data { get; set; }
    public bool IsSuccess { get; set; }
    public string Message { get; set; } = string.Empty;

    public static Response<T> Success(T data, string message = "Operación exitosa") =>
        new() { Data = data, IsSuccess = true, Message = message };

    public static Response<T> Failure(string message) =>
        new() { IsSuccess = false, Message = message };
}

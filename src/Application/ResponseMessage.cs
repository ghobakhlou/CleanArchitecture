using CSharpFunctionalExtensions;

namespace Application.Common.Exceptions;

public class ResponseMessage
{
    public string Message { get; set; }
    public bool IsSuccessful => string.IsNullOrWhiteSpace(Message);

    public static ResponseMessage Success()
    {
        return new ResponseMessage();
    }

    public static ResponseMessage SuccessIf(bool isSuccess, string message)
    {
        return isSuccess ? Success() : new ResponseMessage() { Message = message };
    }

    public static ResponseMessage ToResponse<T>(Result<T> result)
    {
        return result.IsSuccess ? Success() : new ResponseMessage() { Message = result.Error };
    }

    public static ResponseMessage ToResponse(Result result)
    {
        return result.IsSuccess ? Success() : new ResponseMessage() { Message = result.Error };
    }


}
public class ResponseMessage<T> : ResponseMessage
{
    public T Data { get; set; }

    public static ResponseMessage<TData> ToResponse<KResult, TData>(Result<KResult> result, TData data)
    {
        return result.IsSuccess ? Success(data) : new ResponseMessage<TData>() { Message = result.Error };
    }

    public static ResponseMessage<TData> Success<TData>(TData data)
    {
        return new ResponseMessage<TData>() { Data = data };
    }
}

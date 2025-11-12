namespace lab_5.BusinessLogic;

public readonly struct FileAccessResult(bool success, string message)
{
    public readonly string Message = message;
    public readonly bool IsSuccess = success;
    public readonly static FileAccessResult EmptySuccess = new FileAccessResult(true, "");
}
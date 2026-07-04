namespace GetAwayL2.Services
{
    public interface ILoggerService
    {
        Task LogDB(bool isValid, string camMsgRequest, string name, string? message, string? error);
    }
}

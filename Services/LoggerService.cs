using GetAwayL2.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Channels;
namespace GetAwayL2.Services
{
    public static class LoggerService
    {
        public static async Task LogDB(LogDBContext dbContext, bool isValid, CamMsgRequest? camMsgRequest, string name, string? message, string? error)
        {
            // Логирование в базу данных
            await using var transaction = await dbContext.Database.BeginTransactionAsync(System.Data.IsolationLevel.Serializable);
            try
            {
                var status = isValid ? "Valid" : "Not Valid";
                var rawData = camMsgRequest?.msg ?? message;
                var e = new EquipLogs() { timeSend = DateTime.UtcNow, Name = name, rawData = rawData, status = status, errorMessage = error };
                dbContext.EquipLogs.Add(e);
                await dbContext.SaveChangesAsync();
                await transaction.CommitAsync();

            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new Exception("Error logging to database", ex);
            }
        }
        

    }
}

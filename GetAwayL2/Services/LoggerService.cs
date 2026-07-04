using GetAwayL2.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Channels;
namespace GetAwayL2.Services
{
    public class LoggerService : ILoggerService
    {
        private readonly LogDBContext dbContext;
        public LoggerService(LogDBContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task LogDB( bool isValid, string camMsgRequest, string name, string? message, string? error)
        {
            // Логирование в базу данных
            await using var transaction = await dbContext.Database.BeginTransactionAsync(System.Data.IsolationLevel.Serializable);
            try
            {
                var status = isValid ? "Valid" : "Not Valid";
                var rawData = camMsgRequest ?? message;
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

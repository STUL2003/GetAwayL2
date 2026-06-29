using Microsoft.EntityFrameworkCore;
namespace GetAwayL2
{
    public class LogDBContext : DbContext
    {
        public DbSet<GetAwayL2.Models.EquipLogs> EquipLogs { get; set; }
        public LogDBContext(DbContextOptions<LogDBContext> options) : base(options)
        {
        }
    }
}

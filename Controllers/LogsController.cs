using GetAwayL2.Models;
using GetAwayL2.Services;
using Microsoft.AspNetCore.Mvc;


using Microsoft.EntityFrameworkCore;
using System.Text.Json;



namespace GetAwayL2.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LogsController : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetLogs([FromServices] LogDBContext dbContext)
        {
            try
            {
                // имя и макс время
                var topNames = await dbContext.EquipLogs.GroupBy(x => x.Name).Select(g => new { Name = g.Key, MaxTime = g.Max(x => x.timeSend) })
                    .OrderByDescending(x => x.MaxTime)
                    .Select(x => x.Name)
                    .ToListAsync();

                // все логи, у которых Name входит в полученный список
                var logs = await dbContext.EquipLogs.AsNoTracking().Where(x => topNames.Contains(x.Name)).ToListAsync();

                // Группировка на приложении, а не на sql запросе
                var result = topNames.Select(name => new
                    {
                        CategoryId = name,
                        Items = logs
                            .Where(x => x.Name == name)
                            .Select(x => new
                            {
                                x.Id,
                                x.Name,
                                x.rawData,
                                x.timeSend,
                                x.status
                            })
                            .ToList()
                    })
                    .ToList();

                return Ok(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ОШИБКА: {ex.Message}");
                return StatusCode(500, ex.Message);
            }
        }

    }
}

using GetAwayL2.Models;
using GetAwayL2.Services;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class CameraController : ControllerBase
{
    private readonly ParserCameraMsgService parserService;

    public CameraController(ParserCameraMsgService parserService)
    {
        this.parserService = parserService;
    }

    [HttpPost("cam_msg")]
    public async Task<IActionResult> PostCamMsg([FromBody] CamMsgRequest camMsgRequest)
    {
        Console.WriteLine($"В контроллере: {camMsgRequest.msg}");
        try
        {
            parserService.SetRequest(camMsgRequest);
            var result = await parserService.ParseCamMsgAsync();
            return Ok(result);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"ОШИБКА: {ex.Message}");
            Console.WriteLine($"СТЕК: {ex.StackTrace}");
            return StatusCode(500, ex.Message);
        }
    }

}
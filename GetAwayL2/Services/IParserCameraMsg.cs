using GetAwayL2.Models;
namespace GetAwayL2.Services
{
    public interface IParserCameraMsg // интерфейс для реализации класса по парсингу сообщений от камеры
    {
        
        public Task<string> ParseCamMsgAsync();
        protected Task LogDB();
    }
}

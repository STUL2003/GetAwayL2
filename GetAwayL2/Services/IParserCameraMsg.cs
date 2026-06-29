using GetAwayL2.Models;
namespace GetAwayL2.Services
{
    public interface IParserCameraMsg
    {
        public Task<string> ParseCamMsgAsync();
        protected Task LogDB();
    }
}

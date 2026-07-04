using GetAwayL2.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Channels;
namespace GetAwayL2.Services
{
    public class ParserCameraMsgService : IParserCameraMsg
    {
        private string camMsgRequest;
        private bool isValid;
        private readonly ILoggerService logger;

        public ParserCameraMsgService(ILoggerService logger)
        {

            this.isValid = false;
            this.logger = logger;
        }
        public async Task<string> ParseCamMsgAsync()
        {
            // Парсер сообщений от камеры

            try
            {
                isValid = camMsgRequest == null? false : true;
                isValid = camMsgRequest == "READ" || camMsgRequest == "NOREAD" ? true : false;
                await LogDB();
                // Закидываем в канал для дальнейшей обработки
                var ch = ChannelsByName.GetOrCreate<string>("ChannelMsg");
                await ch.Writer.WriteAsync(camMsgRequest);

                return camMsgRequest;


            }
            catch (Exception ex)
            {
                throw new Exception("Error parsing camera message");
            }
        }
        public void SetRequest(string request)
        {
            this.camMsgRequest = request;
        }
        public async Task LogDB()
        {
            await logger.LogDB(isValid, camMsgRequest, "Camera", null, null);
        }
    }
}


using GetAwayL2.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Channels;
namespace GetAwayL2.Services
{
    public class ParserCameraMsgService : IParserCameraMsg
    {
        private readonly LogDBContext dbContext;
        private CamMsgRequest camMsgRequest;
        private bool isValid;
        public ParserCameraMsgService(LogDBContext dbContext, CamMsgRequest camMsgRequest)
        {
            this.dbContext = dbContext;
            this.camMsgRequest = camMsgRequest;
            this.isValid = false;
        }
        public async Task<string> ParseCamMsgAsync()
        {
            // Парсер сообщений от камеры

            try
            {
                var msg = camMsgRequest.msg;
                isValid = msg == null? false : true;
                isValid = msg == "READ" || msg == "NOREAD" ? true : false;
                await LogDB();
                // Закидываем в канал для дальнейшей обработки
                var ch = ChannelsByName.GetOrCreate<string>("ChannelMsg");
                await ch.Writer.WriteAsync(msg);

                return msg;


            }
            catch (Exception ex)
            {
                throw new Exception("Error parsing camera message");
            }
        }
        public void SetRequest(CamMsgRequest request)
        {
            this.camMsgRequest = request;
        }
        public async Task LogDB()
        {
            await LoggerService.LogDB(dbContext, isValid, camMsgRequest, "Camera", null, null);
        }
    }
}


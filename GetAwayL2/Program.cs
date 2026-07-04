using Microsoft.EntityFrameworkCore;
using GetAwayL2;
using GetAwayL2.Models;
using GetAwayL2.Services;
public class Programm
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);


        
        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        builder.Services.AddOpenApi();
        builder.Services.AddDbContext<LogDBContext>(o => o.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
        //builder.Services.AddSingleton<ParserCameraMsgService>();
        //builder.Services.AddTransient<CamMsgRequest>();
        builder.Services.AddHostedService<TCPListennerServicecs>();
        builder.Services.AddScoped<ParserCameraMsgService>();
        builder.Services.AddSingleton<PLKConnectionSendService>();
        builder.Services.AddScoped<ILoggerService, LoggerService>();
        builder.Services.AddControllers();




        var app = builder.Build();

        var plkService = app.Services.GetRequiredService<PLKConnectionSendService>();
        var dbContextFactory = app.Services.GetRequiredService<IServiceScopeFactory>();

        // Configure the HTTP request pipeline
        _ = Task.Run(async () =>
        {
            string plcHost = "127.0.0.1";
            int plcPort = 5000;

            while (true)
            {
                using (var scope = dbContextFactory.CreateScope())
                {
                    var db = scope.ServiceProvider.GetRequiredService<LogDBContext>();
                    await plkService.Run(plcHost, plcPort);
                }
                // если соединение разорвалось – подождём и попробуем снова
                await Task.Delay(5000);
            }
        });

        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
        }

        
        //app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapControllers();

        app.Run();

    }
}



using DataTranferObjects.Common.Request;
using FPTTrackingSystem.Extensions;
using FPTTrackingSystem.Services.Common.MQ;
using FPTTrackingSystem.Services.Common.Schedules;
using Microsoft.Extensions.FileProviders;
using Quartz;
using Repositories.Common.Interfaces;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseWebRoot("wwwroot");
builder.Services.AddControllers();
builder.Services.AddRepositories();
builder.Services.AddServices();
builder.Services.AddMemoryCache();
builder.Services.AddHttpClient();
builder.Services.AddDatabase(builder.Configuration);
builder.Services.AddSwaggerDocumentation();
builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddHttpContextAccessor();
builder.Services.AddMappings();
builder.Services.Configure<MailSettings>(builder.Configuration.GetSection("MailSettings"));
builder.Services.Configure<RabbitMQSettings>(builder.Configuration.GetSection("RabbitMQ"));
builder.Services.AddSingleton<RabbitMQProducer>();
builder.Services.AddHostedService<RabbitMQConsumer>();
builder.Services.AddHostedService<AIConsumer>();
builder.Services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);
builder.Services.AddQuartz(q =>
{
    var jobKey = new JobKey("DailyMailJob");

    q.AddJob<SendMailJob>(opts => opts.WithIdentity(jobKey));

    q.AddTrigger(opts => opts
    .ForJob(jobKey)
    .WithIdentity("DailyMailJob-trigger")
    .WithCronSchedule("0 15 23 * * ?", x => x
        .InTimeZone(TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time")))
);
});
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFE", policy =>
    {
        policy.WithOrigins("http://160.30.21.113:8082"
            ,"https://localhost:5000",
            "http://10.0.0.2:8082",
            "http://10.0.0.3:8082",
            "http://10.0.0.4:8082",
            "http://10.0.0.5:8082",
            "http://10.0.0.6:8082",
            "http://35.240.135.75:8082",
            "http://192.168.110.70:9999")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});
builder.WebHost.ConfigureKestrel(options =>
{
    options.Configure(builder.Configuration.GetSection("Kestrel"));
});

var app = builder.Build();
var env = app.Environment;

using (var scope = app.Services.CreateScope())
{
    var cache = scope.ServiceProvider.GetRequiredService<IMailSettingCache>();
    var aiCache = scope.ServiceProvider.GetRequiredService<IAISettingsCache>();
    await cache.ReloadAsync();
    await aiCache.ReloadAsync();
}
app.UseCors("AllowFE");
app.UseFileFallback(env.WebRootPath);
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(Path.Combine(env.WebRootPath, "uploads")),
    RequestPath = "/uploads",
    OnPrepareResponse = ctx =>
    {
        ctx.Context.Response.Headers.Append("Access-Control-Allow-Origin", "*");
    }
});


// xoa sau khi deploy
app.UseSwagger();
app.UseSwaggerUI();

app.UseRouting();
app.UseGlobalErrorHandler();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers().RequireAuthorization();
app.Run();

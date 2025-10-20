using DataTranferObjects.Common.Request;
using FPTTrackingSystem.Extensions;
using FPTTrackingSystem.Middlewares;
using FPTTrackingSystem.Services.Common.MQ;
using FPTTrackingSystem.Services.Common.Schedules;
using Quartz;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseWebRoot("wwwroot");
builder.Services.AddControllers();
builder.Services.AddRepositories();
builder.Services.AddServices();
builder.Services.AddDatabase(builder.Configuration);
builder.Services.AddSwaggerDocumentation();
builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddHttpContextAccessor();
builder.Services.AddMappings();
builder.Services.Configure<MailSettings>(builder.Configuration.GetSection("MailSettings"));
builder.Services.Configure<RabbitMQSettings>(builder.Configuration.GetSection("RabbitMQ"));
builder.Services.AddSingleton<RabbitMQProducer>();
builder.Services.AddHostedService<RabbitMQConsumer>();
builder.Services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);
builder.Services.AddQuartz(q =>
{
    var jobKey = new JobKey("DailyMailJob");

    q.AddJob<SendMailJob>(opts => opts.WithIdentity(jobKey));

    q.AddTrigger(opts => opts
    .ForJob(jobKey)
    .WithIdentity("DailyMailJob-trigger")
    .WithCronSchedule("0 30 22 * * ?", x => x
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
            "http://10.0.0.6:8082")
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
app.UseCors("AllowFE");
// cho phep tat ca ip tai ve
app.UseStaticFiles(new StaticFileOptions
{
    OnPrepareResponse = ctx =>
    {
        ctx.Context.Response.Headers.Append("Access-Control-Allow-Origin", "*");
    }
});
// xoa sau khi deploy
app.UseSwagger();
app.UseSwaggerUI();
//cors
app.UseRouting();
app.UseGlobalErrorHandler();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers().RequireAuthorization();
app.Run();

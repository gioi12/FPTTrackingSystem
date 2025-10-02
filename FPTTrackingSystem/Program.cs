using FPTTrackingSystem.Extensions;
using FPTTrackingSystem.Middlewares;
using FPTTrackingSystem.Services.Group;
using Repositories.Authentication;
using Repositories.Group;
using Repositories.GroupRepository;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddRepositories();
builder.Services.AddServices();
builder.Services.AddDatabase(builder.Configuration);
builder.Services.AddSwaggerDocumentation();
builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddScoped<IGroupRepository, GroupRepository>();
builder.Services.AddScoped<IGroupService, GroupService>();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFE", policy =>
    {
        policy.WithOrigins("http://192.168.116.1:8082", "https://localhost:5000", "http://192.168.131.7:8082", "http://192.168.1.163:8082", "http://192.168.1.167:8082", "http://160.30.21.113:8082") 
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

app.UseGlobalErrorHandler();
// xoa sau
app.UseSwagger();
app.UseSwaggerUI();
//cors
app.UseCors("AllowFE");
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers().RequireAuthorization();
app.Run();

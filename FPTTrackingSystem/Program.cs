using FPTTrackingSystem.Extensions;
using FPTTrackingSystem.Middlewares;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddRepositories();
builder.Services.AddServices();
builder.Services.AddDatabase(builder.Configuration);
builder.Services.AddSwaggerDocumentation();
builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddHttpContextAccessor();
builder.Services.AddMappings();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFE", policy =>
    {
        policy.WithOrigins("http://192.168.116.1:8082"
            , "https://localhost:5000"
            , "http://192.168.131.7:8082"
            , "http://192.168.1.163:8082"
            , "http://192.168.1.167:8082"
            , "http://160.30.21.113:8082"
            , "http://192.168.2.3:8082"
            , "http://172.17.6.230:8082"
            , "http://10.0.62.121:8082"
            , "http://192.168.2.4:8082"
            , "http://192.168.1.92:8082"
            , "http://192.168.131.8:8082") 
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


// xoa sau
app.UseSwagger();
app.UseSwaggerUI();
//cors
app.UseCors("AllowFE");
app.UseRouting();
app.UseGlobalErrorHandler();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers().RequireAuthorization();
app.Run();

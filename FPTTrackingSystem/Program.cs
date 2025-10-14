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
        policy.WithOrigins("http://160.30.21.113:8082"
            , "https://localhost:5000",
            "10.0.0.2",
            "10.0.0.3",
            "10.0.0.4",
            "10.0.0.5",
            "10.0.0.6") 
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

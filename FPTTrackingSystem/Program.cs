using FPTTrackingSystem.Extensions;
using FPTTrackingSystem.Middlewares;
using Repositories.Authentication;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddRepositories();
builder.Services.AddServices();
builder.Services.AddDatabase(builder.Configuration);
builder.Services.AddSwaggerDocumentation();
builder.Services.AddJwtAuthentication(builder.Configuration);

var app = builder.Build();

app.UseGlobalErrorHandler();
// xoa sau
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers().RequireAuthorization();
app.Run();

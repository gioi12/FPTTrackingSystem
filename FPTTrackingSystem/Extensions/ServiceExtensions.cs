using Entities.Models;
using FPTTrackingSystem.Mappers;
using FPTTrackingSystem.Middlewares;
using FPTTrackingSystem.Services.Admin;
using FPTTrackingSystem.Services.Authentication;
using FPTTrackingSystem.Services.Common.Gemini;
using FPTTrackingSystem.Services.Common.Implements;
using FPTTrackingSystem.Services.Common.Interfaces;
using FPTTrackingSystem.Services.Login;
using FPTTrackingSystem.Services.Staff.Implementations;
using FPTTrackingSystem.Services.Staff.Implements;
using FPTTrackingSystem.Services.Staff.Interfaces;
using FPTTrackingSystem.Services.Student.Implements;
using FPTTrackingSystem.Services.Student.Interfaces;
using FPTTrackingSystem.Services.Token;
using FPTTrackingSystem.Utilities;
using Mapster;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Repositories.Authentication;
using Repositories.Common.Implements;
using Repositories.Common.Interfaces;
using Repositories.Staff.Implements;
using Repositories.Staff.Interfaces;
using Repositories.Student.Implements;
using Repositories.Student.Interfaces;
using System.Text;
namespace FPTTrackingSystem.Extensions
{
    public static class ServiceExtensions
    {
        // add repo , service 
        public  static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            services.AddScoped<IGroupRepository, GroupRepository>();
            services.AddScoped<IAccountRepository, AccountRepository>();
            services.AddScoped<IMilestoneRepository,MilestoneRepository>();
            services.AddScoped<ISemesterRepository, SemesterRepository>();
            services.AddScoped<IMajorRepository, MajorRepository>();
            services.AddScoped<ILogRepository, LogRepository>();
            services.AddScoped<IDeliverableRepository, DeliverableRepository>();
            services.AddScoped<ITaskRepository, TaskRepository>();
            services.AddScoped<ICommentRepository, CommentRepository>();
            services.AddScoped<IAttachmentRepository, AttachmentRepository>();
            services.AddScoped<IEvaluationRepository, EvaluationRepository>();
            services.AddScoped<IMeetingRepository, MeetingRepository>();
            services.AddScoped<ICampusRepository, CampusRepository>();
            services.AddSingleton<IMailSettingCache, MailSettingCache>();
            services.AddScoped<IMailRepository,MailRepository>();
            services.AddScoped<IAISettingsRepository, AISettingsRepository>();
            services.AddSingleton<IAISettingsCache, AISettingsCache>();
            return services;
        }
        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            services.AddScoped<IGroupService, GroupService>();
            services.AddScoped<IJwtService, JwtService>();
            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<IMilestoneService, MilestoneService>();
            services.AddScoped<ISemesterService, SemesterService>();
            services.AddScoped<ILogService, LogService>();
            services.AddScoped<IMajorService, MajorService>();
            services.AddScoped<IDeliverableSevice, DeliverableService>();
            services.AddScoped<ITaskService, TaskService>();
            services.AddScoped<ICommentService, CommentService>();
            services.AddScoped<IUploadService, UploadService>();
            services.AddScoped<AuthUtils>();
            services.AddScoped<IMailService, MailService>();
            services.AddScoped<IEvaluationService, EvaluationService>();
            services.AddScoped<IMeetingService, MeetingService>();
            services.AddScoped<ICampusService, CampusService>();
            services.AddScoped<IStorageService, StorageService>();
            services.AddScoped<IAIService, AIService>();
            services.AddScoped<IGeminiService, GeminiService>();

            return services;
        }
        public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration config)
        {
            var connectionString = config.GetConnectionString("DefaultConnection");
            services.AddDbContext<FpttrackingSystemContext>(options =>
                options.UseSqlServer(connectionString));
            return services;
        }

        public static IServiceCollection AddSwaggerDocumentation(this IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Your API",
                    Version = "v1",
                    Description = "API documentation for YourApp"
                });

                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Nhập JWT token với Bearer {token}",
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });
            });
            return services;
        }

        public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration config)
        {
            var jwtSettings = config.GetSection("JwtSettings");
            var secretKey = jwtSettings["SecretKey"];

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        context.Token = context.Request.Cookies["token"];
                        return System.Threading.Tasks.Task.CompletedTask;
                    }
                };
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings["Issuer"],
                    ValidAudience = jwtSettings["Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
                };
            });
            // phan quyen
            services.AddAuthorization();
            return services;
        }

        public static IServiceCollection AddMappings(this IServiceCollection services)
        {
            TypeAdapterConfig.GlobalSettings.Scan(typeof(ServiceExtensions).Assembly);
            MilestoneMapper.ToMilestoneResponse();
            DeliverableMapper.ToDeliverableResponse();
            MeetingMinuteMapper.ToMeetingMinuteResponse();
            AttachmentMapper.ToAttachmentResponse();
            return services;
        }

        public static IApplicationBuilder UseGlobalErrorHandler(this IApplicationBuilder services)
        {
            return services.UseMiddleware<GlobalErrorMiddleware>();
        }
        public static IApplicationBuilder UseFileFallback(this IApplicationBuilder services, string webRoot)
        {
            var uploadsRoot = Path.Combine(webRoot, "uploads");
            services.UseMiddleware<ZipFallbackMiddleware>(uploadsRoot);
            return services;
        }
    }
}

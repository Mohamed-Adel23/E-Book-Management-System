using Asp.Versioning;
using EBMS.Data;
using EBMS.Data.DataAccess;
using EBMS.Data.Services.Auth;
using EBMS.Data.Services.Cache;
using EBMS.Data.Services.Email;
using EBMS.Data.Services.File;
using EBMS.Infrastructure;
using EBMS.Infrastructure.Helpers;
using EBMS.Infrastructure.Helpers.Constants;
using EBMS.Infrastructure.IServices.IAuth;
using EBMS.Infrastructure.IServices.ICache;
using EBMS.Infrastructure.IServices.IEmail;
using EBMS.Infrastructure.IServices.IFile;
using EBMS.Infrastructure.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

namespace EBMS.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            // ----------------------------
            // Add BookDbContext Service To The Pipeline
            builder.Services.AddDbContext<BookDbContext>(
                op => op.UseSqlServer(
                    builder.Configuration.GetConnectionString("DefaultConStr"),
                    con => con.MigrationsAssembly(typeof(BookDbContext).Assembly.FullName)
                )
            );
            // Add The Identity
            builder.Services.AddIdentity<BookUser, IdentityRole>().AddEntityFrameworkStores<BookDbContext>();
            // Mapping JWT values into a class
            builder.Services.Configure<JWT>(builder.Configuration.GetSection("JWT"));
            // Add New Service for Authentication
            builder.Services.AddScoped<IBookAuthService, BookAuthService>();
            // JWT Configuration Settings
            builder.Services.AddAuthentication(op =>
            {
                op.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                op.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(o =>
            {
                o.RequireHttpsMetadata = false;
                o.SaveToken = true;
                o.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidIssuer = builder.Configuration["JWT:Issuer"],
                    ValidAudience = builder.Configuration["JWT:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Key"])),
                    ClockSkew = TimeSpan.Zero
                };
            });
            // Register Unit Of Work
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            // Register CacheService
            builder.Services.AddScoped<ICacheService, CacheService>();
            // Register EmailService
            builder.Services.AddScoped<IEmailService, EmailService>();
            // Register FileService
            builder.Services.AddScoped<IFileService, FileService>();
            // Add Memory Cache
            builder.Services.AddDistributedMemoryCache();
            // Add Session To Pipeline
            builder.Services.AddSession(op =>
            {
                op.IdleTimeout = TimeSpan.FromMinutes(20);
            });

            // Implementing RateLimiting (Fixed Window)
            builder.Services.AddRateLimiter(option =>
            {
                option.AddFixedWindowLimiter(RateLimiterConstants.FixedWindow, opt =>
                {
                    opt.Window = TimeSpan.FromSeconds(10);
                    opt.PermitLimit = 5;
                    opt.QueueLimit = 3;
                    opt.QueueProcessingOrder = System.Threading.RateLimiting.QueueProcessingOrder.OldestFirst;
                }).RejectionStatusCode = 429;
            });

            // Implementing RateLimiting (Sliding Window)
            builder.Services.AddRateLimiter(option =>
            {
                option.AddSlidingWindowLimiter(RateLimiterConstants.SlidingWindow, opt =>
                {
                    opt.Window = TimeSpan.FromSeconds(10);
                    opt.PermitLimit = 4;
                    opt.SegmentsPerWindow = 3;
                    opt.QueueLimit = 3;
                    opt.QueueProcessingOrder = System.Threading.RateLimiting.QueueProcessingOrder.OldestFirst;
                }).RejectionStatusCode = 429;
            });

            // Implementing RateLimiting (Concurrency)
            builder.Services.AddRateLimiter(option =>
            {
                option.AddConcurrencyLimiter(RateLimiterConstants.Concurrency, opt =>
                {
                    opt.PermitLimit = 5;
                    opt.QueueLimit = 3;
                    opt.QueueProcessingOrder = System.Threading.RateLimiting.QueueProcessingOrder.OldestFirst;
                }).RejectionStatusCode = 429;
            });

            // Implementing RateLimiting (Token Bucket)
            builder.Services.AddRateLimiter(option =>
            {
                option.AddTokenBucketLimiter(RateLimiterConstants.TokenBucket, opt =>
                {
                    opt.TokenLimit = 10; // max number of tokens the bucket can have
                    // requests that don't have tokens are queued and if the queue is full the incoming requests will be rejected
                    opt.QueueLimit = 5;
                    opt.QueueProcessingOrder = System.Threading.RateLimiting.QueueProcessingOrder.OldestFirst;
                    opt.ReplenishmentPeriod = TimeSpan.FromSeconds(10); // tokens are added to the bucket every 10s
                    opt.TokensPerPeriod = 10; // 10T/10s
                    opt.AutoReplenishment = true;
                }).RejectionStatusCode = 429;
            });

            // API Versioning
            builder.Services.AddApiVersioning(options =>
            {
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.DefaultApiVersion = new Asp.Versioning.ApiVersion(1, 0);
                options.ReportApiVersions = true;
                options.ApiVersionReader = ApiVersionReader.Combine(
                        //new QueryStringApiVersionReader("api-version"),
                        new HeaderApiVersionReader("x-version")
                        //new MediaTypeApiVersionReader("ver")
                    );
            }).AddApiExplorer(options =>
            {
                options.GroupNameFormat = "'v'VVV";
                options.SubstituteApiVersionInUrl = true;
            });


            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(
                options =>
                {
                    options.SwaggerDoc("v1", new OpenApiInfo
                    {
                        Version = "v1",
                        Title = "EBMS API",
                        Description = "A comprehensive E-Book System API which provides many features",
                        TermsOfService = new Uri("https://example.com/terms"),
                        Contact = new OpenApiContact
                        {
                            Name = "BnAdel",
                            Url = new Uri("https://example.com/contact")
                        },
                        License = new OpenApiLicense
                        {
                            Name = "MIT",
                            Url = new Uri("https://example.com/license")
                        }
                    });
                }
            );

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseRateLimiter();

            app.UseStaticFiles();

            app.UseHttpsRedirection();

            app.UseSession();

            // User authenticated, then authorized
            app.UseAuthentication();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}

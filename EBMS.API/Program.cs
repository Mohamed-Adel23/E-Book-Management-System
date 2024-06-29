using EBMS.Data;
using EBMS.Data.DataAccess;
using EBMS.Data.Services.Auth;
using EBMS.Infrastructure;
using EBMS.Infrastructure.Helpers;
using EBMS.Infrastructure.IServices.IAuth;
using EBMS.Infrastructure.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
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

            app.UseHttpsRedirection();

            // User authenticated, then authorized
            app.UseAuthentication();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}

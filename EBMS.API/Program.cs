
using EBMS.Data.DataAccess;
using EBMS.Infrastructure.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

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

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}

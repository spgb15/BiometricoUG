
using BiometricoZTK.Application.Interfaces;
using BiometricoZTK.Application.Services;
using BiometricoZTK.Domain.Interfaces;
using BiometricoZTK.Infrastructure.Persistence;
using BiometricoZTK.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace BiometricoZTK
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(connectionString));

            // Add services to the container.
            builder.Services.AddScoped<IAttendanceRepository, AttendanceRepository>();
            builder.Services.AddScoped<IZKProcessingService, ZTKProcessingService>();
            builder.Services.AddScoped<IZKCommandService, ZKCommandService>();

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

            //app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}

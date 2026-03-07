
using BiometricoZTK.Application.Interfaces;
using BiometricoZTK.Application.Interfaces.Auth;
using BiometricoZTK.Application.Services;
using BiometricoZTK.Application.Services.Auth;
using BiometricoZTK.Domain.Entities.Auth;
using BiometricoZTK.Domain.Interfaces;
using BiometricoZTK.Infrastructure.Persistence;
using BiometricoZTK.Infrastructure.Repositories;
using BiometricoZTK.Infrastructure.Repositories.Auth;
using BiometricoZTK.Infrastructure.Seeding;
using BiometricoZTK.Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace BiometricoZTK
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            var jwtSettings = builder.Configuration.GetSection("jwt");
            var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]);

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = jwtSettings["Issuer"],
                        ValidAudience = jwtSettings["Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(key),
                        ClockSkew = TimeSpan.Zero
                    };
                });

            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(connectionString));

            // Add services to the container.
            builder.Services.AddScoped<IAttendanceRepository, AttendanceRepository>();
            builder.Services.AddScoped<IZKProcessingService, ZTKProcessingService>();
            builder.Services.AddScoped<IZKCommandService, ZKCommandService>();
            builder.Services.AddScoped<IJwtSerivce, JwtGenerator>();

            // Auth
            builder.Services.AddScoped<ILoginRepository, LoginRepository>();
            builder.Services.AddScoped<IPasswordHasher<Usuario>, PasswordHasher<Usuario>>();
            builder.Services.AddScoped<IAuthService, AuthService>();

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

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            // Ejecutar seeds al iniciar (primero roles, luego usuario por defecto)
            using (var scope = app.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher<Usuario>>();
                RolSeeder.SeedAdminRolAsync(context).GetAwaiter().GetResult();
                UsuarioSeeder.SeedDefaultUserAsync(context, passwordHasher).GetAwaiter().GetResult();
            }

            app.Run();
        }
    }
}

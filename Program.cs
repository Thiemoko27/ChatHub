using System.Text;
using ChatHub.Models;
using ChatHub.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ChatHub.Services;

namespace ChatHub
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            //Recupération des chaines de connection
            var userConnection = builder.Configuration.GetConnectionString("UserConnection")
                ?? throw new ArgumentNullException("UserConnection", "UserConnection is not configured");
            var messageConnection = builder.Configuration.GetConnectionString("MessageConnection")
                ?? throw new ArgumentNullException("MessageConnection", "MessageConnection is not configured");

            builder.Services.AddDbContext<UserDataBaseContext>(options => 
                            options.UseSqlServer(userConnection));

            builder.Services.AddDbContext<MessageDataBaseContext>(options =>
                            options.UseSqlServer(messageConnection));

            // Add services to the container.
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = "yourIssuer",
                    ValidAudience = "yourAudience",
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("yourSecret"))
                };
            });

            //Configuration de SignalR
            builder.Services.AddSignalR();

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddScoped<AuthService>();

            builder.Services.AddCors(options => {
                options.AddPolicy("AllowSpecificOrigin",
                builder => builder.WithOrigins("http://localhost:5175")
                                  .AllowAnyHeader()
                                  .AllowAnyMethod()
                                  .AllowCredentials());
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.UseCors("AllowSpecificOrigin");

            app.MapHub<Chat>("/chathub");

            app.MapControllers();

            app.Run();
        }
    }
}

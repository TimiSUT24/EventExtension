
using EventClassLibrary.Models;
using EventExtension.Data;
using EventExtension.Repositories;
using EventExtension.Repositories.Interfaces;
using EventExtension.Services;
using EventExtension.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using System;
using System.Threading.Tasks;

namespace EventExtension
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
     
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddOpenApi();

            //Services
            builder.Services.AddScoped<IEventService, EventService>();

            //Repositories
            builder.Services.AddScoped<IGenericRepository<EventItem>, EventRepository>();
            builder.Services.AddScoped<IEventRepository, EventRepository>();

            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi

            builder.Services.AddDbContext<EventDBContext>(options =>
                options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

            //CORS
            builder.Services.AddCors(options =>
            {
                options.AddDefaultPolicy(policy =>
                {
                    policy.WithOrigins("http://localhost:5173")
                    .AllowAnyHeader()
                    .AllowAnyMethod();                   

                });
            });
    
            var app = builder.Build();

            app.UseCors();
            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
                app.MapScalarApiReference();
            }

            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.MapControllers();

            using (var scope = app.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<EventDBContext>();
                await SeedEvents.SeedEvent(dbContext); 
            }

            await app.RunAsync();
        }
    }
}

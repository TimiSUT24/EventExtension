
using EventClassLibrary.Models;
using EventExtension.Data;
using EventExtension.Repositories;
using EventExtension.Repositories.Interfaces;
using EventExtension.Services;
using EventExtension.Services.EventExtension.Services;
using EventExtension.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
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
            builder.Services.AddScoped<JWT_Service>();

            //Repositories
            builder.Services.AddScoped<IGenericRepository<EventItem>, EventRepository>();
            builder.Services.AddScoped<IEventRepository, EventRepository>();

            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi

        
            var connectionString_supabase = Environment.GetEnvironmentVariable("Connectionstrings__Supabase");

            builder.Services.AddDbContext<EventDBContext>(options =>
                options.UseNpgsql(connectionString_supabase));

            //Identity 
            builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<EventDBContext>()
                .AddDefaultTokenProviders();

            //Authentication
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                .AddJwtBearer(option =>
                {
                    option.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = builder.Configuration["Jwt:Issuer"],
                        ValidAudience = builder.Configuration["Jwt:Audience"],
                        IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
                    };
                });

            builder.Services.AddAuthorization();

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

            var port = Environment.GetEnvironmentVariable("PORT") ?? "5000";
            app.Urls.Add($"http://*:{port}");

            app.UseCors();
            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
                app.MapScalarApiReference();
            }

            //app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();
            app.Use(async (context, next) =>
            {
                if (context.Request.Path.StartsWithSegments("/Event/UploadEvents"))
                {
                    var apiKey = context.Request.Headers["X-API-KEY"].FirstOrDefault();
                    if (apiKey != builder.Configuration["UploadEventsKey"])
                    {
                        context.Response.StatusCode = 401;
                        await context.Response.WriteAsync("Unauthorized");
                        return;
                    }
                }
                await next();
            });

            using (var scope = app.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<EventDBContext>();
                db.Database.Migrate();
            }

                await app.RunAsync();
        }
    }
}

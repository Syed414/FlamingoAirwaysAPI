
using Microsoft.EntityFrameworkCore;
using FlamingoAirwaysAPI.Models.Interfaces;
using FlamingoAirwaysAPI.Models.Repositories;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;


namespace FlamingoAirwaysAPI.Models

{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            //registers Dbcontext and tells the DI Container how to congigure it..
            //lambda expression used to config Dbcontext to use sql server 
            builder.Services.AddDbContext<FlamingoAirwaysDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("AirwaysCnString")));

            builder.Services.AddScoped<IUserRepository, UserRepository>(); //new instance is created for each http request
            builder.Services.AddScoped<IFlightRepository, FlightRepository>();//every time it needs an IFlightrepo during an http request create a new instance of flightRepository
            builder.Services.AddScoped<IBookingRepository, BookingRepository>();
            builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();
            builder.Services.AddScoped<ITicketRepository, TicketRepository>();



            builder.Services.AddControllers(); //add controller services to the DI

            
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("V1", new OpenApiInfo //Registers a swagger document
                {
                    Version = "V1",
                    Title = "WebAPI",
                    Description = "Product WebAPI"
                });
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Scheme = "Bearer",
                    BearerFormat = "Jwt",
                    In = ParameterLocation.Header, //token will be passed in http header
                    Name = "Authorization",
                    Description = "Bearer Authentication with Jwt Token",
                    Type = SecuritySchemeType.Http
                });
                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Id = "Bearer",
                                Type = ReferenceType.SecurityScheme
                            }
                        },
                        new List<string>()
                     }
                });
            });
            builder.Services.AddAuthentication(opt => {
                opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = ConfigurationManager.AppSetting["Jwt:ValidIssuer"],
                        ValidAudience = ConfigurationManager.AppSetting["Jwt:ValidAudience"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(ConfigurationManager.AppSetting["Jwt:Secret"]))
                    };
                });
            builder.Services.AddAuthorization(
                options =>
                {
                    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
                    options.AddPolicy("UserOnly", policy => policy.RequireRole("User"));
                });


            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(options => {
                    options.SwaggerEndpoint("/swagger/V1/swagger.json", "Product WebAPI");
                });
            }

            app.UseHttpsRedirection(); //Redirects HTTP requests to HTTPS, ensuring secure communication.

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers(); //Connects the http req to the corresponding controller

            app.Run();
        }
    }
}
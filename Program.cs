using minimalAPI_webbutveckling_labb2.Data;
using minimalAPI_webbutveckling_labb2.Models;
using Microsoft.EntityFrameworkCore;

namespace minimalAPI_webbutveckling_labb2
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddDbContext<DataContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
            builder.Services.AddAuthorization();
           
            builder.Services.AddScoped<DbContext, DataContext>();
             


            // Add services to the container.
            builder.Services.AddAuthorization();

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowSpecificOrigin",
                    builder => builder.WithOrigins("http://localhost:3000") 
                                      .AllowAnyMethod()
                                      .AllowAnyHeader());
            });

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddDbContext<DataContext>();

            var app = builder.Build();
            using(var scope= app.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<DbContext>();
                dbContext.Database.Migrate();
                //SeedDatabase(dbContext);
            } 

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment() || !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER")))
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            // Conditionally disable HTTPS redirection if running inside Docker
            if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER")))
            {
                app.UseHttpsRedirection();
            }


            app.UseHttpsRedirection();
            app.UseAuthorization();
            



            app.MapGet("/car", async (DataContext context) =>
            {
                return await context.Cars.ToListAsync();
            });



            app.MapGet("/car/{id}", async (DataContext context, int id) =>

                await context.Cars.FindAsync(id) is Car car ?
                Results.Ok(car) :
                Results.NotFound("Does not exist")
            );





            app.MapPost("/car", async (DataContext context, Car car) =>
            {
                context.Cars.Add(car);
                await context.SaveChangesAsync();
                return Results.Ok(await context.Cars.ToListAsync());

            });

            app.MapPut("/car/{id}", async (DataContext context, Car updateCar, int id) =>
            {
                var car = await context.Cars.FindAsync(id);

                if (car == null)
                {
                    return Results.NotFound("Sorry, this car doesnt exists");


                }

                car.Make = updateCar.Make;
                car.Model = updateCar.Model;

                await context.SaveChangesAsync();

                return Results.Ok(await context.Cars.ToListAsync());
            });

            app.MapDelete("/car/{id}", async (DataContext context, int id) =>
            {
                var car = context.Cars.FindAsync(id);

                if (car == null)

                    return Results.NotFound("Sorry, this car doesnt exists");




                context.Cars.Remove(await car);
                await context.SaveChangesAsync();

                return Results.Ok(await context.Cars.ToListAsync());
            });

            

                app.Urls.Add("http://*:80");

            app.Run();
        }

        private static void SeedDatabase(DataContext dbContext)
        {
            // Seed the database with initial data
           
                dbContext.Cars.AddRange(new List<Car>
                {
                    new Car { Make = "Audi", Model = "RS7"},
                    new Car { Make = "BMW", Model = "M5"},
                    new Car { Make = "Volvo", Model = "XC90"}
                });
                dbContext.SaveChanges();
            

        }


    }
}










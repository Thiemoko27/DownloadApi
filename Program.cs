
namespace DownloadApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddCors(options => {
                options.AddPolicy("AllowSpecificOrigin", 
                        builder => builder.WithOrigins("http://localhost:5173")
                                           .AllowAnyHeader()
                                           .AllowAnyMethod());
            });

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

            var env = app.Environment;

            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");

            var webRootPath = env.WebRootPath ?? path;
            var filePath = Path.Combine(webRootPath, "files");

            if(!Directory.Exists(filePath)) {
                Directory.CreateDirectory(filePath);
            }

            app.UseCors("AllowSpecificOrigin");

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}

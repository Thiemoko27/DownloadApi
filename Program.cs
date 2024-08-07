using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.FileProviders;

namespace DownloadApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.WebHost.ConfigureKestrel(serverOptions => {
                serverOptions.Limits.MaxRequestBodySize = 100_000_000; // 100 Mo
            });

            // Add services to the container.

            builder.Services.AddCors(options => {
                options.AddPolicy("AllowSpecificOrigin", 
                        builder => builder.WithOrigins("http://localhost:5173")
                                           .AllowAnyHeader()
                                           .AllowAnyMethod());
            });

            builder.Services.Configure<FormOptions>(options => {
                options.MultipartBodyLengthLimit = 100_000_000; // 150 Mo
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

            app.UseStaticFiles(new StaticFileOptions {
                FileProvider = new PhysicalFileProvider(
                    Path.Combine(builder.Environment.ContentRootPath, "wwwroot", "files")),
                RequestPath = "/files"
            });

            app.UseCors("AllowSpecificOrigin");

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DownloadApi.Controllers;

[Route("api/[controller]")]
[ApiController]

public class UploadController : ControllerBase
{
    public readonly string _targetFilePath;

    public UploadController(IWebHostEnvironment env) {
        _targetFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/files");
    }

    [HttpPost("upload")]
    public async Task<IActionResult> Upload(IFormFile file)
    {
        if(file == null || file.Length == 0)
            return BadRequest("no file droped");
        else
            Console.WriteLine("file droped");

        var AllowedExtentions = new[] {".pptx", ".pdf", ".jpg", ".docx"};
        var extention = Path.GetExtension(file.FileName).ToLowerInvariant();

        if(!AllowedExtentions.Contains(extention)) 
            return BadRequest("Unsuported extention");
        

        var path = Path.Combine(_targetFilePath, file.FileName);

        if(!Directory.Exists(_targetFilePath)) {
            Directory.CreateDirectory(_targetFilePath);
        }

        using(var stream = new FileStream(path, FileMode.Create)) {
            await file.CopyToAsync(stream);
        }

        return Ok(new {FilePath = path});
    }

    [HttpGet("files")]
    public IActionResult GetFiles() {
        var path = _targetFilePath;
        var files = Directory.GetFiles(path).Select(f => Path.GetFileName(f)).ToList();

        return Ok(files);
    }
} 
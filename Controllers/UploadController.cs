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
    public readonly string _targetFilePathPdf;
    public readonly string _targetFilePathDocx;

    public UploadController(IWebHostEnvironment env) {
        _targetFilePathDocx = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/files/Docx");
        _targetFilePathPdf = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/files/Pdf");
    }

    [HttpPost("upload")]
    public async Task<IActionResult> Upload(IFormFile file)
    {
        if(file == null || file.Length == 0)
            return BadRequest("no file droped");
        else
            Console.WriteLine("file droped");

        var AllowedExtentions = new[] {".pdf", ".docx"};
        var extention = Path.GetExtension(file.FileName).ToLowerInvariant();

        if(!AllowedExtentions.Contains(extention)) 
            return BadRequest("Unsuported extention");
        

        var path = extention switch {
            ".docx" => Path.Combine(_targetFilePathDocx, file.FileName),
            ".pdf" => Path.Combine(_targetFilePathPdf, file.FileName),
            _ => null
        };

        if(path == null) 
            return BadRequest("invalid file path");

        var directoryPath = Path.GetDirectoryName(path);

        if(directoryPath == null) 
            return StatusCode(StatusCodes.Status500InternalServerError, "Invalid directory path");

        if(!Directory.Exists(path)) {
           Directory.CreateDirectory(directoryPath);
        }

        using(var stream = new FileStream(path, FileMode.Create)) {
            await file.CopyToAsync(stream);
        }

        return Ok(new {FilePath = path});
    }

    [HttpGet("files")]
    public IActionResult GetFiles() {
        var docxFiles = Directory.GetFiles(_targetFilePathDocx).Select(f => Path.GetFileName(f));
        var pdfFiles = Directory.GetFiles(_targetFilePathPdf).Select(f => Path.GetFileName(f));

        var files = docxFiles.Concat(pdfFiles).ToList();

        return Ok(files);
    }
} 
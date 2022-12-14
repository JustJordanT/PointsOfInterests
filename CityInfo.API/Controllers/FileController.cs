using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;

namespace CityInfo.API.Controllers;

[Route("api/logs")]
[ApiController]
public class LogsController : ControllerBase
{
    private readonly FileExtensionContentTypeProvider _fileExtensionContentTypeProvider;

    public LogsController(FileExtensionContentTypeProvider fileExtensionContentTypeProvider)
    {
        _fileExtensionContentTypeProvider 
            = fileExtensionContentTypeProvider 
              ?? throw new System.ArgumentNullException(nameof(fileExtensionContentTypeProvider));
    }
    
    [HttpGet("{filedate}")]
    public async Task<IActionResult> Get(int filedate)
    {
        var pathToFile = $"./logs/cityinfo{filedate}.txt";

        if (!System.IO.File.Exists(pathToFile))
        {
            // return NotFound();
            return NotFound("No date found, date must be entered as yyyymmdd");
        }

        if (!_fileExtensionContentTypeProvider.TryGetContentType(pathToFile, out var contentType))
        {
            contentType = "application/octet-stream";
        }
        
        var bytes = System.IO.File.ReadAllBytes(pathToFile);
        return File(bytes, contentType, Path.GetFileName(pathToFile));
    }
}
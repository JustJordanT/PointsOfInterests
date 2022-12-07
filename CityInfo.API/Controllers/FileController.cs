using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;

namespace CityInfo.API.Controllers;

[Route("api/files")]
[ApiController]
public class FileController : ControllerBase
{
    private readonly FileExtensionContentTypeProvider _fileExtensionContentTypeProvider;

    public FileController(FileExtensionContentTypeProvider fileExtensionContentTypeProvider)
    {
        _fileExtensionContentTypeProvider 
            = fileExtensionContentTypeProvider 
              ?? throw new System.ArgumentNullException(nameof(fileExtensionContentTypeProvider));
    }
    
    [HttpGet("{fileId}")]
    public async Task<IActionResult> Get(int fileId)
    {
        var pathToFile = "Monolith to Microservices Evolutionary Patterns to Transform Your Monolith by Sam Newman .pdf";

        if (!System.IO.File.Exists(pathToFile))
        {
            return NotFound();
        }

        if (!_fileExtensionContentTypeProvider.TryGetContentType(pathToFile, out var contentType))
        {
            contentType = "application/octet-stream";
        }
        
        var bytes = System.IO.File.ReadAllBytes(pathToFile);
        return File(bytes, contentType, Path.GetFileName(pathToFile));
    }
}
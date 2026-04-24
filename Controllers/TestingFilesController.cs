using ApiExamenViernesANF.Helpers;
using ApiExamenViernesANF.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ApiExamenViernesANF.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestingFilesController : ControllerBase
    {
        private FileStorageHelper _storage;

        public TestingFilesController(FileStorageHelper storage)
        {
            _storage = storage;
        }

        [HttpPost]
        public async Task<IActionResult> Upload([FromBody] FileModel request)
        {
            Models.FileInfo result = await _storage.SaveFileAsync(
                request.FileName,
                request.FileContent
            );
            return Ok(result);
        }

        [HttpGet("images")]
        public IActionResult GetAll()
        {
            List<Models.FileInfo> images = _storage.GetAllFiles();
            return Ok(images);
        }
    }
}

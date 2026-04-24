using ApiExamenViernesANF.Models;

namespace ApiExamenViernesANF.Helpers
{
    public class FileStorageHelper
    {
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly string _carpeta = "Images";

        public FileStorageHelper(
            IWebHostEnvironment hostEnvironment,
            IHttpContextAccessor httpContextAccessor
        )
        {
            _hostEnvironment = hostEnvironment;
            _httpContextAccessor = httpContextAccessor;
        }

        private string ObtenerExtension(string nombreOriginal, string base64)
        {
            string extension = Path.GetExtension(nombreOriginal);
            if (!string.IsNullOrEmpty(extension))
                return extension;

            if (base64.StartsWith("/9j/"))
                return ".jpg";
            if (base64.StartsWith("iVBOR"))
                return ".png";
            if (base64.StartsWith("R0lGO"))
                return ".gif";
            if (base64.StartsWith("UklGR"))
                return ".webp";
            if (base64.StartsWith("Qk0"))
                return ".bmp";
            if (base64.StartsWith("PHN2Zy"))
                return ".svg";

            return ".jpg";
        }

        public async Task<Models.FileInfo> SaveFileAsync(string nombreOriginal, string base64)
        {
            if (string.IsNullOrEmpty(base64))
                return null;

            if (base64.Contains(","))
                base64 = base64.Split(',')[1];

            string extension = ObtenerExtension(nombreOriginal, base64);
            string nombreSinExtension = Path.GetFileNameWithoutExtension(nombreOriginal);
            string nombreArchivo = Guid.NewGuid().ToString() + "_" + nombreSinExtension + extension;

            byte[] bytes = Convert.FromBase64String(base64);
            string path = Path.Combine(_hostEnvironment.ContentRootPath, _carpeta, nombreArchivo);

            Directory.CreateDirectory(Path.GetDirectoryName(path));

            await System.IO.File.WriteAllBytesAsync(path, bytes);

            HttpRequest request = _httpContextAccessor.HttpContext!.Request;
            string baseUrl = $"{request.Scheme}://{request.Host}";

            return new Models.FileInfo
            {
                FileName = nombreArchivo,
                UrlPath = $"{baseUrl}/{_carpeta}/{Uri.EscapeDataString(nombreArchivo)}",
            };
        }

        public List<Models.FileInfo> GetAllFiles()
        {
            string uploadsFolder = Path.Combine(_hostEnvironment.ContentRootPath, _carpeta);

            Directory.CreateDirectory(uploadsFolder);

            HttpRequest request = _httpContextAccessor.HttpContext!.Request;
            string baseUrl = $"{request.Scheme}://{request.Host}";

            return Directory
                .GetFiles(uploadsFolder)
                .Select(f => new Models.FileInfo
                {
                    FileName = Path.GetFileName(f),
                    UrlPath = $"{baseUrl}/{_carpeta}/{Uri.EscapeDataString(Path.GetFileName(f))}",
                })
                .ToList();
        }
    }
}

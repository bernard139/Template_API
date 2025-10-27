using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using SixLabors.ImageSharp;
using System.Text;
using Template.Application.Contracts.Misc;
using Template.Application.Models.Enums;

namespace Template.Misc
{
    public class FileStorageService : IFileStorageService
    {
        private readonly IHostingEnvironment _webHostEnvironment;
        private readonly ILogger<FileStorageService> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;

        // Maximum allowed dimensions
        private const int MaxWidth = 4000;
        private const int MaxHeight = 2500;

        public FileStorageService(IHostingEnvironment webHostEnvironment,
                                 ILogger<FileStorageService> logger,
                                 IHttpContextAccessor httpContextAccessor)
        {
            _webHostEnvironment = webHostEnvironment;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<string> SaveImageAsync(IFormFile imageFile, UploadType uploadType)
        {
            if (imageFile == null || imageFile.Length == 0)
                throw new ArgumentException("Image file is required");

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".webp" };
            var fileExtension = Path.GetExtension(imageFile.FileName).ToLowerInvariant();

            if (!allowedExtensions.Contains(fileExtension))
                throw new ArgumentException($"Invalid image format. Allowed: {string.Join(", ", allowedExtensions)}");

            if (imageFile.Length > 5 * 1024 * 1024)
                throw new ArgumentException("Image size exceeds 5MB limit");

            await ValidateImageDimensionsAsync(imageFile);

            // Use WebRootPath (wwwroot) for better static file serving
            string uploadsRoot = _webHostEnvironment.WebRootPath;

            // Fallback to ContentRootPath if WebRootPath is null
            if (string.IsNullOrEmpty(uploadsRoot))
            {
                uploadsRoot = _webHostEnvironment.ContentRootPath;
            }

            // Determine folder based on upload type
            string folderName = uploadType switch
            {
                UploadType.User => "User",
                UploadType.Others => "Others",
                _ => "Uploads"
            };

            string uploadsFolder = "Uploads";
            string imageDirectory = Path.Combine(uploadsRoot, uploadsFolder, folderName);
            var uniqueFileName = $"{Guid.NewGuid()}{fileExtension}";
            string filePath = Path.Combine(imageDirectory, uniqueFileName);

            // Ensure the directory exists
            if (!Directory.Exists(imageDirectory))
            {
                Directory.CreateDirectory(imageDirectory);
            }

            // Save the file
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await imageFile.CopyToAsync(stream);
            }

            // Get base URL and return full URL path
            var baseUrl = GetBaseUrl();
            var relativePath = Path.Combine(uploadsFolder, folderName, uniqueFileName).Replace("\\", "/");

            return $"{baseUrl}/{relativePath}";
        }

        public async Task<string> SaveDocumentAsync(IFormFile documentFile)
        {
            if (documentFile == null || documentFile.Length == 0)
                throw new ArgumentException("Document file is required");

            var allowedExtensions = new[] { ".pdf", ".doc", ".docx", ".txt", ".xls", ".xlsx", ".ppt", ".pptx" };
            var fileExtension = Path.GetExtension(documentFile.FileName).ToLowerInvariant();

            if (!allowedExtensions.Contains(fileExtension))
                throw new ArgumentException($"Invalid document format. Allowed: {string.Join(", ", allowedExtensions)}");

            if (documentFile.Length > 10 * 1024 * 1024) // 10MB limit for documents
                throw new ArgumentException("Document size exceeds 10MB limit");

            // Use WebRootPath (wwwroot) for better static file serving
            string uploadsRoot = _webHostEnvironment.WebRootPath;

            // Fallback to ContentRootPath if WebRootPath is null
            if (string.IsNullOrEmpty(uploadsRoot))
            {
                uploadsRoot = _webHostEnvironment.ContentRootPath;
            }

            // Create Documents folder
            string uploadsFolder = "Uploads";
            string documentsFolder = "Documents";
            string documentDirectory = Path.Combine(uploadsRoot, uploadsFolder, documentsFolder);

            // Maintain original filename with GUID prefix to avoid conflicts
            var originalFileName = Path.GetFileNameWithoutExtension(documentFile.FileName);
            var uniqueFileName = $"{Guid.NewGuid()}_{originalFileName}{fileExtension}";

            string filePath = Path.Combine(documentDirectory, uniqueFileName);

            // Ensure the directory exists
            if (!Directory.Exists(documentDirectory))
            {
                Directory.CreateDirectory(documentDirectory);
            }

            // Save the file
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await documentFile.CopyToAsync(stream);
            }

            // Get base URL and return full URL path
            var baseUrl = GetBaseUrl();
            var relativePath = Path.Combine(uploadsFolder, documentsFolder, uniqueFileName).Replace("\\", "/");

            return $"{baseUrl}/{relativePath}";
        }

        private async Task ValidateImageDimensionsAsync(IFormFile imageFile)
        {
            await using var memoryStream = new MemoryStream();
            await imageFile.CopyToAsync(memoryStream);
            memoryStream.Position = 0;

            using (var image = await Image.LoadAsync(memoryStream))
            {
                if (image.Width > MaxWidth || image.Height > MaxHeight)
                {
                    throw new ArgumentException($"Image dimensions exceed maximum allowed size of {MaxWidth}x{MaxHeight}px. Current size: {image.Width}x{image.Height}px");
                }
            }
        }

        private string GetBaseUrl()
        {
            var request = _httpContextAccessor.HttpContext?.Request;
            if (request == null)
            {
                // Fallback if HTTP context is not available (e.g., during testing)
                return "https://localhost:5001"; // Adjust this to your actual base URL
            }

            var baseUrl = new StringBuilder()
                .Append(request.Scheme)
                .Append("://")
                .Append(request.Host)
                .Append(request.PathBase)
                .ToString();

            return baseUrl.TrimEnd('/');
        }
    }
}

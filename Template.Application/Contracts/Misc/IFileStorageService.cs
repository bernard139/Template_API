using Microsoft.AspNetCore.Http;
using Template.Application.Models.Enums;

namespace Template.Application.Contracts.Misc
{
    public interface IFileStorageService
    {
        Task<string> SaveImageAsync(IFormFile imageFile, UploadType uploadType);
        Task<string> SaveDocumentAsync(IFormFile documentFile);
    }
}

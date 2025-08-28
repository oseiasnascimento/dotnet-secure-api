using Microsoft.AspNetCore.Http;
using Guardian.Application.Shared.Dtos;
using Guardian.Application.Wrappers;

namespace Guardian.Application.Shared.Contracts
{
    public interface IUploadFileService
    {
        public Task<Response<string>> UploadFileAsync(UploadFileRequest request, IFormFile file, string type);
        public Task<Response<string>> RemoveFileAsync(string filePath);
    }
}

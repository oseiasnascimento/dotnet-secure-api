using Microsoft.AspNetCore.Http;

namespace Guardian.Application.Shared.Dtos
{
    public class UploadFileRequest
    {
        public string Name { get; set; }
        public string Extension { get; set; }
        public string Path { get; set; }
    }
}

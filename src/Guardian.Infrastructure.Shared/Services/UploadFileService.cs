using Microsoft.AspNetCore.Http;
using Guardian.Application.Shared.Contracts;
using Guardian.Application.Shared.Dtos;
using Guardian.Application.Wrappers;

namespace Guardian.Infrastructure.Shared.Services
{
    public class UploadFileService : IUploadFileService
    {
        public async Task<Response<string>> UploadFileAsync(UploadFileRequest request, IFormFile file, string type)
        {
            if (file == null)
            {
                return Response<string>.Failure(
                    errors: ["É necessario anexar o banner da postagem."]
                );
            }

            if (FileIsValid(file) == false)
            {
                return Response<string>.Failure(
                    errors: ["Arquivo deve possuir no máximo 8mb e precisa ter o formato: .png / .jpg / .pdf"]
                );
            }

            string fileName = request.Name;

            string directoryPath = Path.Combine(Directory.GetCurrentDirectory(), "staticfiles", request.Path, type.ToLower());

            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            string filePath = Path.Combine(directoryPath, fileName);
            var imagePath = (request.Path + type + "/" + fileName).ToLower();
            using FileStream stream = new(filePath, FileMode.Create);
            await file.CopyToAsync(stream);

            return Response<string>.Success(
                message: "Arquivo salvo com sucesso.",
                data: imagePath
            );
        }

        private static bool FileIsValid(IFormFile file)
        {
            List<string> validExtensions = [".png", ".jpg", ".pdf"];
            string extension = Path.GetExtension(file.FileName);

            if (!validExtensions.Contains(extension))
            {
                return false;
            }

            long fileSize = file.Length;

            long maxSize = 8 * 1024 * 1024;

            if (fileSize > maxSize)
            {
                return false;
            }

            return true;
        }

        public async Task<Response<string>> RemoveFileAsync(string filePath)
        {
            try
            {
                string fullPath = Path.Combine(Directory.GetCurrentDirectory(), "staticfiles/", filePath);

                bool fileExists = File.Exists(fullPath);

                if (!fileExists)
                {
                    return Response<string>.Failure(
                        errors: ["Arquivo não encontrado. Verifique e tente novamente."]
                    );
                }

                await Task.Run(() => File.Delete(fullPath));

                return Response<string>.Success(
                    "Arquivo deletado com sucesso.",
                    string.Empty
                );
            }
            catch (Exception ex)
            {
                return Response<string>.Failure(
                    errors: [
                        "Erro interno ao processar solicitação.",
                        ex.Message
                    ]
                );
            }
        }
    }
}
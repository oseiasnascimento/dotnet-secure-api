using Guardian.Application.Shared.Dtos;

namespace Guardian.Application.Shared.Contracts
{
    public interface ILoggerService
    {
        /// <summary>
        /// Gera e salva um novo log do tipo Information no Banco de Dados.
        /// </summary>
        /// <param name="request">Request necessária para salvar um novo log.</param>
        public void LogInfo(CreateLogRequest request);

        /// <summary>
        /// Gera e salva um novo log do tipo Warning no Banco de Dados.
        /// </summary>
        /// <param name="request">Request necessária para salvar um novo log.</param>
        public void LogWarning(CreateLogRequest request);
    }
}
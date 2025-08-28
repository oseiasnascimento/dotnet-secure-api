using Guardian.Application.Shared.Dtos;

namespace Guardian.Application.Shared.Contracts
{
    public interface IEmailService
    {
        /// <summary>
        /// Utiliza o serviço de SMTP para enviar um e-mail para o e-mail passado na requisição.
        /// </summary>
        /// <param name="request">Requisição necessária para enviar um e-mail corretamente.</param>
        /// <returns></returns>
        public Task SendEmailAsync(SendMailRequest request);
    }
}
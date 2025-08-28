using FluentValidation;
using Guardian.Application.Validations;
using Guardian.Domain.User.Entities;

namespace Guardian.Application.Accounts.Dtos.Inputs
{
    public class UpdateAccountRequest
    {
        public string FullName { get; set; }
        public string CPF { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public List<string> Roles { get; set; }

        public static ApplicationUser Map(ApplicationUser user, UpdateAccountRequest request)
        {
            user.UserName = request.CPF;
            user.NormalizedUserName = request.CPF;
            user.FullName = request.FullName;
            user.CPF = request.CPF;
            user.Email = request.Email;
            user.NormalizedEmail = request.Email;
            user.PhoneNumber = request.PhoneNumber;

            return user;
        }
    }

    public class UpdateAccountRequestValidation : AbstractValidator<UpdateAccountRequest>
    {
        public UpdateAccountRequestValidation()
        {
            RuleFor(x => x.FullName)
                .NotEmpty().WithMessage("Nome completo é obrigatório")
                .Length(3, 100).WithMessage("Nome completo deve conter entre 3 e 100 caracteres");

            RuleFor(x => x.CPF)
                .NotEmpty().WithMessage("CPF é obrigatório")
                .Length(11).WithMessage("CPF deve conter apenas 11 caracteres")
                .Must(CPFValidator.Validate).WithMessage("Formato de CPF inválido");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email é obrigatório")
                .Length(3, 100).WithMessage("Email deve conter entre 3 e 100 caracteres")
                .EmailAddress().WithMessage("Formato incorreto de e-mail");

            RuleFor(x => x.PhoneNumber)
                .NotEmpty().WithMessage("Número de Telefone é obrigatório")
                .Length(11).WithMessage("Número de Telefone deve conter 11 caracteres");
        }
    }
}
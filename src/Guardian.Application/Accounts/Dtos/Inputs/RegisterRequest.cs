using System.Text.Json.Serialization;
using FluentValidation;
using Guardian.Application.Validations;

namespace Guardian.Application.Accounts.Dtos.Inputs
{
    public class RegisterRequest
    {
        [JsonIgnore]
        public int? UserId { get; set; }
        public string FullName { get; set; }
        public string CPF { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime BirthDate { get; set; }
        public List<string> Roles { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
    }

    public class RegisterRequestValidation : AbstractValidator<RegisterRequest>
    {
        public RegisterRequestValidation()
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

            RuleFor(x => x.BirthDate)
                .NotEmpty().WithMessage("Data de nascimento é obrigatório")
                .LessThan(DateTime.Today);

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Senha é obrigatório")
                .Length(8, 128).WithMessage("Senha deve conter entre 8 e 128 caracteres");

            RuleFor(x => x.ConfirmPassword)
                .NotEmpty().WithMessage("Confirmar Senha é obrigatório")
                .NotEqual(x => nameof(x.Password)).WithMessage("Suas senhas não conferem.");
        }
    }
}
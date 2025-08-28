using FluentValidation;

namespace Guardian.Application.Accounts.Dtos.Inputs
{
    public class ChangePasswordRequest
    {
        public string CurrentPassword { get; set; }

        public string NewPassword { get; set; }

        public string ConfirmNewPassword { get; set; }
    }
    public class ChangePasswordRequestValidation : AbstractValidator<ChangePasswordRequest>
    {
        public ChangePasswordRequestValidation()
        {
            RuleFor(x => x.CurrentPassword)
                .NotEmpty().WithMessage("A Senha atual é obrigatório.")
                .Length(8, 100).WithMessage("Senha deve conter entre 8 e 100 caracteres.");

            RuleFor(x => x.NewPassword)
                .NotEmpty().WithMessage("A Senha nova é obrigatório.")
                .Length(8, 100).WithMessage("Senha deve conter entre 8 e 100 caracteres.");

            RuleFor(x => x.ConfirmNewPassword)
                    .NotNull().WithMessage("A Nova Senha e a confirmação não coincidem.")
                    .Length(8, 100).WithMessage("Senha deve conter entre 8 e 100 caracteres.");
        }
    }
}
using Entities.DTOs.Auth;
using FluentValidation;

namespace Services.FluentValidations.User
{
    public class CreateUserRequestValidator : AbstractValidator<CreateUserRequest>
    {
        public CreateUserRequestValidator()
        {
            // UserName doğrulaması
            RuleFor(user => user.UserName)
                .NotEmpty().WithMessage("Kullanıcı adı boş olamaz.")
                .MinimumLength(3).WithMessage("Kullanıcı adı en az 3 karakter uzunluğunda olmalıdır.")
                .MaximumLength(50).WithMessage("Kullanıcı adı en fazla 50 karakter uzunluğunda olabilir.");

            // Password doğrulaması
            RuleFor(user => user.Password)
                .NotEmpty().WithMessage("Şifre boş olamaz.")
                .MinimumLength(6).WithMessage("Şifre en az 6 karakter uzunluğunda olmalıdır.");

            // Role doğrulaması
            RuleFor(user => user.Role)
                .IsInEnum().WithMessage("Geçersiz bir kullanıcı rolü seçildi.");
        }

    }
}

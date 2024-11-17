using Entities.DTOs.Appointment;
using FluentValidation;

namespace Services.FluentValidations
{
    public class UpdateAppointmentStatusResponseValidator : AbstractValidator<UpdateAppointmentStatusResponse>
    {
        public UpdateAppointmentStatusResponseValidator()
        {
            RuleFor(response => response.Id)
                .NotNull().WithMessage("Id boş olamaz.")
                .NotEmpty().WithMessage("Id geçerli bir değer olmalıdır.");

            RuleFor(response => response.Status)
                .NotEmpty().WithMessage("Durum bilgisi (Status) boş olamaz.");
        }

    }
}
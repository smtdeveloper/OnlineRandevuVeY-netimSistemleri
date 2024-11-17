using Entities.DTOs.Appointment;
using FluentValidation;

namespace Services.FluentValidations.Appointment;

public class CreateAppointmentRequestValidator : AbstractValidator<CreateAppointmentRequest>
{
    public CreateAppointmentRequestValidator()
    {
        RuleFor(_appointment => _appointment.AppointmentDate)
            .NotNull().WithMessage("Tarih boş olamaz.")
            .Must(BeAValidDate).WithMessage("Geçerli bir tarih olmalıdır.")
            .GreaterThan(DateTime.Now).WithMessage("Geçmiş bir tarih olamaz.");
    }

    private bool BeAValidDate(DateTime date)
    {
        return date != default;
    }
}
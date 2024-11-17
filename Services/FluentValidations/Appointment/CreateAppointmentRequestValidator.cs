using Entities.DTOs.Appointment;
using FluentValidation;

namespace Services.FluentValidations.Appointment;

public class CreateAppointmentRequestValidator : AbstractValidator<CreateAppointmentRequest>
{
    public CreateAppointmentRequestValidator()
    {
        // AppointmentDate doğrulaması
        RuleFor(_appointment => _appointment.AppointmentDate)
            .NotNull().WithMessage("Tarih boş olamaz.")
            .Must(BeAValidDate).WithMessage("Geçerli bir tarih olmalıdır.")
            .GreaterThan(DateTime.Now).WithMessage("Geçmiş bir tarih olamaz.");
    }

    // Geçerli bir tarih kontrolü
    private bool BeAValidDate(DateTime date)
    {
        return date != default;
    }
}

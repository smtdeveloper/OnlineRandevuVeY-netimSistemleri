using Entities.DTOs.Appointment;
using FluentValidation;

namespace Services.FluentValidations.Appointment
{
    public class UpdateAppointmentRequestValidator : AbstractValidator<UpdateAppointmentRequest>
    {
        public UpdateAppointmentRequestValidator()
        {
            RuleFor(_appointment => _appointment.Id)
                .NotNull().WithMessage("Id boş olamaz.")
                .NotEmpty().WithMessage("Id geçerli bir değer olmalıdır.");


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
}
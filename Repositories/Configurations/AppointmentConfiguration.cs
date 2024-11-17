using Entities.Enums;
using Entities.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.ValueGeneration;

public class AppointmentConfiguration : IEntityTypeConfiguration<Appointment>
{
    public void Configure(EntityTypeBuilder<Appointment> builder)
    {
        builder.ToTable("Appointments").HasKey(_appointment => _appointment.Id);        
        builder.Property(_appointment => _appointment.Id).ValueGeneratedOnAdd().HasValueGenerator<SequentialGuidValueGenerator>();
        builder.Property(_appointment => _appointment.ServiceId).IsRequired();
        builder.Property(_appointment => _appointment.UserId).IsRequired();
        builder.Property(_appointment => _appointment.AppointmentDate).IsRequired();
        builder.Property(_appointment => _appointment.Status).IsRequired().HasDefaultValue(AppointmentStatus.Unknown); 
        builder.Property(_appointment => _appointment.CreatedDate).IsRequired();
        builder.Property(_appointment => _appointment.UpdatedDate).IsRequired(false);
        builder.Property(_appointment => _appointment.DeletedDate).IsRequired(false);
        builder.Property(_appointment => _appointment.IsDelete).HasDefaultValue(false);

        builder.HasOne(_appointment => _appointment.User)
               .WithMany(_appointment => _appointment.Appointments)
               .HasForeignKey(_appointment => _appointment.UserId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(_appointment => _appointment.Service)
               .WithMany(_appointment => _appointment.Appointments)
               .HasForeignKey(_appointment => _appointment.ServiceId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}
using Entities.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.ValueGeneration;

public class ServiceConfiguration : IEntityTypeConfiguration<Service>
{
    public void Configure(EntityTypeBuilder<Service> builder)
    {
        builder.ToTable("Services").HasKey(_service => _service.Id);  
        builder.Property(_service => _service.Id).ValueGeneratedOnAdd().HasValueGenerator<SequentialGuidValueGenerator>();
        builder.Property(_service => _service.Name).IsRequired();
        builder.Property(_service => _service.CreatedDate).IsRequired();
        builder.Property(_service => _service.UpdatedDate).IsRequired(false);
        builder.Property(_service => _service.DeletedDate).IsRequired(false);
        builder.Property(_service => _service.IsDelete).HasDefaultValue(false);
    }
}
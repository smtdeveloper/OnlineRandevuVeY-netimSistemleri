using Entities.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.ValueGeneration;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users").HasKey(_user => _user.Id);
        builder.Property(_user => _user.Id).ValueGeneratedOnAdd().HasValueGenerator<SequentialGuidValueGenerator>();
        builder.Property(_user => _user.UserName).IsRequired().HasMaxLength(150);
        builder.Property(_user => _user.PasswordHash).IsRequired();
        builder.Property(_user => _user.CreatedDate).IsRequired();
        builder.Property(_user => _user.UpdatedDate).IsRequired(false);
        builder.Property(_user => _user.DeletedDate).IsRequired(false);
        builder.Property(_user => _user.IsDelete).HasDefaultValue(false);
    }
}
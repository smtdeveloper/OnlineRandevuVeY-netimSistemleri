using Entities.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.ValueGeneration;

namespace Repositories.Configurations;

public class UserRoleConfiguration : IEntityTypeConfiguration<UserRole>
{
    public void Configure(EntityTypeBuilder<UserRole> builder)
    {
        builder.ToTable("UserRoles").HasKey(_userRole => _userRole.Id);
        builder.Property(_userRole => _userRole.Id).ValueGeneratedOnAdd().HasValueGenerator<SequentialGuidValueGenerator>();
        builder.Property(_userRole => _userRole.CreatedDate).IsRequired();
        builder.Property(_userRole => _userRole.UpdatedDate).IsRequired(false);
        builder.Property(_userRole => _userRole.DeletedDate).IsRequired(false);
        builder.Property(_userRole => _userRole.IsDelete).HasDefaultValue(false);

        builder.HasOne(_userRole => _userRole.User)
              .WithMany(_user => _user.Roles)
              .HasForeignKey(_userRole => _userRole.UserId)
              .OnDelete(DeleteBehavior.Cascade);
    }
}
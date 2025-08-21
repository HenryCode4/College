using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace College.Data.Config
{
    public class StudentConfig : IEntityTypeConfiguration<Student>
    {
        public void Configure(EntityTypeBuilder<Student> builder)
        {
            builder.ToTable("Students");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id).UseIdentityColumn();

            builder.Property(n => n.StudentName).IsRequired().HasMaxLength(250);
            builder.Property(n => n.Address).IsRequired(false).HasMaxLength(500);
            builder.Property(n => n.Email).IsRequired().HasMaxLength(250);

            builder.HasData(new List<Student>()
            {
                new Student
                {
                    Id = 1,
                    StudentName = "Henry",
                    Address = "Nigeria",
                    Email = "henry@gmail.com",
                    DOB = new DateTime(2005, 10, 12)
                },
                new Student
                {
                    Id = 2,
                    StudentName = "Tony",
                    Address = "Nigeria",
                    Email = "tony@gmail.com",
                    DOB = new DateTime(2006, 10, 12)
                }
            });
        }
    }
}

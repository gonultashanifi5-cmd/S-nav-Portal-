using Microsoft.EntityFrameworkCore;

namespace OnlineSinavPortali.Models;

public sealed class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Course> Courses => Set<Course>();
    public DbSet<Exam> Exams => Set<Exam>();
    public DbSet<Question> Questions => Set<Question>();
    public DbSet<Option> Options => Set<Option>();
    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Course>(course =>
        {
            course.Property(x => x.Code).HasMaxLength(20);
            course.Property(x => x.Name).HasMaxLength(120);
            course.HasIndex(x => x.Code).IsUnique();
            course.HasMany(x => x.Exams).WithOne(x => x.Course).OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Exam>(exam =>
        {
            exam.Property(x => x.Title).HasMaxLength(120);
            exam.ToTable(table => table.HasCheckConstraint("CK_Exams_Duration", "[DurationMinutes] BETWEEN 1 AND 300"));
        });

        modelBuilder.Entity<Question>(question =>
        {
            question.Property(x => x.Text).HasMaxLength(1000);
            question.ToTable(table =>
            {
                table.HasCheckConstraint("CK_Questions_Points", "[Points] BETWEEN 1 AND 100");
                table.HasCheckConstraint("CK_Questions_CorrectOption", "[CorrectOptionPosition] BETWEEN 1 AND 4");
            });
        });

        modelBuilder.Entity<Option>(option =>
        {
            option.Property(x => x.Text).HasMaxLength(300);
            option.HasIndex(x => new { x.QuestionId, x.Position }).IsUnique();
            option.ToTable(table => table.HasCheckConstraint("CK_Options_Position", "[Position] BETWEEN 1 AND 4"));
        });

        modelBuilder.Entity<User>(user =>
        {
            user.Property(x => x.UserName).HasMaxLength(60);
            user.Property(x => x.PasswordHash).HasMaxLength(255);
            user.Property(x => x.Role).HasMaxLength(30);
            user.HasIndex(x => x.UserName).IsUnique();
        });
    }
}

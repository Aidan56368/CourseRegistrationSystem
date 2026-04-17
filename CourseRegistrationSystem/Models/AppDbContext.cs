using Microsoft.EntityFrameworkCore;
using CourseRegistrationSystem.Models;

namespace CourseRegistrationSystem.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Course> Courses { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<Registration> Registrations { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Course>().HasData(
                new Course { Id = 1, Code = "CS101", Name = "Intro to Computer Science", Credits = 3, Capacity = 30, Instructor = "Dr. Smith", Semester = "Fall 2026", Status = "Open" },
                new Course { Id = 2, Code = "MATH201", Name = "Calculus II", Credits = 4, Capacity = 25, Instructor = "Dr. Lee", Semester = "Fall 2026", Status = "Open" },
                new Course { Id = 3, Code = "ENG110", Name = "Technical Writing", Credits = 2, Capacity = 20, Instructor = "Prof. Davis", Semester = "Fall 2026", Status = "Closed" }
            );

            modelBuilder.Entity<Student>().HasData(
                new Student { Id = 1, StudentId = "S001", Name = "Alice Johnson", Email = "alice@school.edu", Major = "Computer Science", Year = "Sophomore", GPA = 3.8 },
                new Student { Id = 2, StudentId = "S002", Name = "Bob Martinez", Email = "bob@school.edu", Major = "Mathematics", Year = "Junior", GPA = 3.5 },
                new Student { Id = 3, StudentId = "S003", Name = "Carol White", Email = "carol@school.edu", Major = "Engineering", Year = "Freshman", GPA = 3.9 }
            );
        }
    }
}

using System.ComponentModel.DataAnnotations;

namespace CourseRegistrationSystem.Models
{
    public class Course
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Course code is required")]
        [RegularExpression(@"^[A-Za-z]{2,6}\d{3}$",
            ErrorMessage = "Format: 2-6 letters + 3 digits (e.g. CS101)")]
        [Display(Name = "Course Code")]
        public string Code { get; set; } = "";

        [Required(ErrorMessage = "Course name is required")]
        [MinLength(3, ErrorMessage = "Name must be at least 3 characters")]
        [Display(Name = "Course Name")]
        public string Name { get; set; } = "";

        [Required(ErrorMessage = "Credits are required")]
        [Range(1, 6, ErrorMessage = "Credits must be between 1 and 6")]
        public int Credits { get; set; }

        [Required(ErrorMessage = "Capacity is required")]
        [Range(1, 500, ErrorMessage = "Capacity must be at least 1")]
        public int Capacity { get; set; }

        [Required(ErrorMessage = "Instructor name is required")]
        public string Instructor { get; set; } = "";

        [Required(ErrorMessage = "Semester is required")]
        public string Semester { get; set; } = "Fall 2026";

        [Required]
        public string Status { get; set; } = "Open";

        public ICollection<Registration> Registrations { get; set; } = new List<Registration>();
    }
}

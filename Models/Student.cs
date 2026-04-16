using System.ComponentModel.DataAnnotations;

namespace CourseRegistrationSystem.Models
{
    public class Student
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Student ID is required")]
        [RegularExpression(@"^S\d{3,}$",
            ErrorMessage = "Format: S followed by 3+ digits (e.g. S001)")]
        [Display(Name = "Student ID")]
        public string StudentId { get; set; } = "";

        [Required(ErrorMessage = "Full name is required")]
        [MinLength(2, ErrorMessage = "Name must be at least 2 characters")]
        [Display(Name = "Full Name")]
        public string Name { get; set; } = "";

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Enter a valid email address")]
        public string Email { get; set; } = "";

        [Required(ErrorMessage = "Major is required")]
        public string Major { get; set; } = "";

        [Required(ErrorMessage = "Year is required")]
        public string Year { get; set; } = "Freshman";

        [Range(0.0, 4.0, ErrorMessage = "GPA must be between 0.0 and 4.0")]
        [DisplayFormat(DataFormatString = "{0:F1}")]
        public double? GPA { get; set; }

        public ICollection<Registration> Registrations { get; set; } = new List<Registration>();
    }
}

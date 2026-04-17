using System.ComponentModel.DataAnnotations;

namespace CourseRegistrationSystem.Models
{
    public class Registration
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Please select a student")]
        [Display(Name = "Student")]
        public int StudentId { get; set; }

        [Required(ErrorMessage = "Please select a course")]
        [Display(Name = "Course")]
        public int CourseId { get; set; }

        [Display(Name = "Registration Date")]
        public DateTime RegistrationDate { get; set; } = DateTime.Today;

        public string Grade { get; set; } = "";

        public Student? Student { get; set; }
        public Course? Course { get; set; }
    }
}

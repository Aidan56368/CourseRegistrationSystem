using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CourseRegistrationSystem.Data;
using CourseRegistrationSystem.Models;

namespace CourseRegistrationSystem.Controllers
{
    public class RegistrationsController : Controller
    {
        private readonly AppDbContext _context;

        public RegistrationsController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var registrations = await _context.Registrations
                .Include(r => r.Student)
                .Include(r => r.Course)
                .ToListAsync();
            return View(registrations);
        }

        public async Task<IActionResult> Create()
        {
            ViewBag.Students = new SelectList(await _context.Students.ToListAsync(), "Id", "Name");
            ViewBag.Courses = new SelectList(
                await _context.Courses.Where(c => c.Status == "Open").ToListAsync(),
                "Id", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Registration registration)
        {
            bool alreadyEnrolled = await _context.Registrations
                .AnyAsync(r => r.StudentId == registration.StudentId
                            && r.CourseId == registration.CourseId);

            if (alreadyEnrolled)
            {
                ModelState.AddModelError("", "This student is already enrolled in that course.");
            }

            var course = await _context.Courses
                .Include(c => c.Registrations)
                .FirstOrDefaultAsync(c => c.Id == registration.CourseId);

            if (course != null && course.Registrations.Count >= course.Capacity)
            {
                ModelState.AddModelError("CourseId", "This course has reached its maximum capacity.");
            }

            if (course != null && course.Status == "Closed")
            {
                ModelState.AddModelError("CourseId", "This course is closed for registration.");
            }

            if (ModelState.IsValid)
            {
                registration.RegistrationDate = DateTime.Today;
                _context.Add(registration);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Student registered successfully!";
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Students = new SelectList(await _context.Students.ToListAsync(), "Id", "Name");
            ViewBag.Courses = new SelectList(
                await _context.Courses.Where(c => c.Status == "Open").ToListAsync(),
                "Id", "Name");
            return View(registration);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var registration = await _context.Registrations
                .Include(r => r.Student)
                .Include(r => r.Course)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (registration == null)
                return NotFound();

            return View(registration);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var registration = await _context.Registrations.FindAsync(id);
            if (registration == null)
                return NotFound();

            _context.Registrations.Remove(registration);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Registration dropped successfully!";
            return RedirectToAction(nameof(Index));
        }
    }
}

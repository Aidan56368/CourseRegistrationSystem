using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CourseRegistrationSystem.Controllers;
using CourseRegistrationSystem.Data;
using CourseRegistrationSystem.Models;
using Xunit;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

namespace CourseRegistrationSystem.Tests
{
    public class FakeTempDataProvider : ITempDataProvider
    {
        private Dictionary<string, object> _data = new Dictionary<string, object>();

        public IDictionary<string, object> LoadTempData(HttpContext context)
        {
            return _data;
        }

        public void SaveTempData(HttpContext context, IDictionary<string, object> values)
        {
            _data = new Dictionary<string, object>(values);
        }
    }

    public class CoursesControllerTests
    {
        private AppDbContext GetInMemoryContext(string dbName)
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;
            return new AppDbContext(options);
        }

        private void SetTempData(Controller controller)
        {
            var httpContext = new DefaultHttpContext();
            var tempData = new TempDataDictionary(httpContext, new FakeTempDataProvider());
            controller.TempData = tempData;
        }

        [Fact]
        public async Task Create_InvalidCourseCode_ReturnsViewWithError()
        {
            var context = GetInMemoryContext("Test_InvalidCode");
            var controller = new CoursesController(context);
            SetTempData(controller);

            var badCourse = new Course
            {
                Code = "BADCODE!!",
                Name = "Test Course",
                Credits = 3,
                Capacity = 30,
                Instructor = "Dr. Test",
                Semester = "Fall 2026",
                Status = "Open"
            };

            controller.ModelState.AddModelError("Code", "Format: 2-6 letters + 3 digits (e.g. CS101)");

            var result = await controller.Create(badCourse);

            Assert.IsType<ViewResult>(result);
            Assert.False(controller.ModelState.IsValid);
        }

        [Fact]
        public async Task Create_DuplicateCourseCode_ReturnsViewWithError()
        {
            var context = GetInMemoryContext("Test_DuplicateCode");

            context.Courses.Add(new Course
            {
                Id = 1,
                Code = "CS101",
                Name = "Existing Course",
                Credits = 3,
                Capacity = 30,
                Instructor = "Dr. Smith",
                Semester = "Fall 2026",
                Status = "Open"
            });
            await context.SaveChangesAsync();

            var controller = new CoursesController(context);
            SetTempData(controller);

            var duplicateCourse = new Course
            {
                Code = "CS101",
                Name = "Another Course",
                Credits = 3,
                Capacity = 25,
                Instructor = "Dr. Jones",
                Semester = "Fall 2026",
                Status = "Open"
            };

            var result = await controller.Create(duplicateCourse);

            Assert.IsType<ViewResult>(result);
            Assert.True(controller.ModelState.ContainsKey("Code"));
        }

        [Fact]
        public async Task Delete_CourseWithRegistrations_RedirectsWithError()
        {
            var context = GetInMemoryContext("Test_DeleteWithRegs");

            context.Courses.Add(new Course { Id = 1, Code = "CS101", Name = "Test", Credits = 3, Capacity = 30, Instructor = "Dr. X", Semester = "Fall 2026", Status = "Open" });
            context.Students.Add(new Student { Id = 1, StudentId = "S001", Name = "Alice", Email = "a@a.com", Major = "CS", Year = "Freshman" });
            context.Registrations.Add(new Registration { Id = 1, StudentId = 1, CourseId = 1, RegistrationDate = DateTime.Today });
            await context.SaveChangesAsync();

            var controller = new CoursesController(context);
            SetTempData(controller);

            var result = await controller.DeleteConfirmed(1);

            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirect.ActionName);

            Assert.Equal(1, context.Courses.Count());
        }

        [Fact]
        public async Task Create_ValidCourse_SavesAndRedirects()
        {
            var context = GetInMemoryContext("Test_ValidCourse");
            var controller = new CoursesController(context);
            SetTempData(controller);

            var newCourse = new Course
            {
                Code = "CS202",
                Name = "Data Structures",
                Credits = 3,
                Capacity = 25,
                Instructor = "Dr. Brown",
                Semester = "Fall 2026",
                Status = "Open"
            };

            var result = await controller.Create(newCourse);

            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirect.ActionName);

            Assert.Equal(1, context.Courses.Count());
        }
    }

    public class StudentsControllerTests
    {
        private AppDbContext GetInMemoryContext(string dbName)
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;
            return new AppDbContext(options);
        }

        private void SetTempData(Controller controller)
        {
            var httpContext = new DefaultHttpContext();
            var tempData = new TempDataDictionary(httpContext, new FakeTempDataProvider());
            controller.TempData = tempData;
        }

        [Fact]
        public async Task Create_InvalidEmail_ReturnsViewWithError()
        {
            var context = GetInMemoryContext("Test_InvalidEmail");
            var controller = new StudentsController(context);
            SetTempData(controller);

            var badStudent = new Student
            {
                StudentId = "S010",
                Name = "Test Student",
                Email = "not-a-valid-email",
                Major = "CS",
                Year = "Freshman"
            };

            controller.ModelState.AddModelError("Email", "Enter a valid email address");

            var result = await controller.Create(badStudent);

            Assert.IsType<ViewResult>(result);
            Assert.False(controller.ModelState.IsValid);
        }

        [Fact]
        public async Task Delete_StudentWithRegistrations_RedirectsWithError()
        {
            var context = GetInMemoryContext("Test_DeleteStudentWithRegs");

            context.Courses.Add(new Course { Id = 1, Code = "CS101", Name = "Test", Credits = 3, Capacity = 30, Instructor = "Dr. X", Semester = "Fall 2026", Status = "Open" });
            context.Students.Add(new Student { Id = 1, StudentId = "S001", Name = "Alice", Email = "a@a.com", Major = "CS", Year = "Freshman" });
            context.Registrations.Add(new Registration { Id = 1, StudentId = 1, CourseId = 1, RegistrationDate = DateTime.Today });
            await context.SaveChangesAsync();

            var controller = new StudentsController(context);
            SetTempData(controller);

            var result = await controller.DeleteConfirmed(1);

            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirect.ActionName);

            Assert.Equal(1, context.Students.Count());
        }

        [Fact]
        public async Task Create_GPAAboveMax_ReturnsViewWithError()
        {
            var context = GetInMemoryContext("Test_HighGPA");
            var controller = new StudentsController(context);
            SetTempData(controller);

            var badStudent = new Student
            {
                StudentId = "S020",
                Name = "Over Achiever",
                Email = "over@school.edu",
                Major = "Math",
                Year = "Senior",
                GPA = 5.0
            };

            controller.ModelState.AddModelError("GPA", "GPA must be between 0.0 and 4.0");

            var result = await controller.Create(badStudent);

            Assert.IsType<ViewResult>(result);
            Assert.False(controller.ModelState.IsValid);
        }
    }

    public class RegistrationsControllerTests
    {
        private AppDbContext GetInMemoryContext(string dbName)
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;
            return new AppDbContext(options);
        }

        private void SetTempData(Controller controller)
        {
            var httpContext = new DefaultHttpContext();
            var tempData = new TempDataDictionary(httpContext, new FakeTempDataProvider());
            controller.TempData = tempData;
        }

        [Fact]
        public async Task Create_DuplicateRegistration_ReturnsViewWithError()
        {
            var context = GetInMemoryContext("Test_DuplicateReg");

            context.Courses.Add(new Course { Id = 1, Code = "CS101", Name = "CS", Credits = 3, Capacity = 30, Instructor = "Dr. X", Semester = "Fall 2026", Status = "Open" });
            context.Students.Add(new Student { Id = 1, StudentId = "S001", Name = "Alice", Email = "a@a.com", Major = "CS", Year = "Freshman" });
            context.Registrations.Add(new Registration { Id = 1, StudentId = 1, CourseId = 1, RegistrationDate = DateTime.Today });
            await context.SaveChangesAsync();

            var controller = new RegistrationsController(context);
            SetTempData(controller);

            var duplicate = new Registration { StudentId = 1, CourseId = 1 };
            var result = await controller.Create(duplicate);

            Assert.IsType<ViewResult>(result);
            Assert.False(controller.ModelState.IsValid);
        }

        [Fact]
        public async Task Create_CourseAtCapacity_ReturnsViewWithError()
        {
            var context = GetInMemoryContext("Test_CapacityFull");

            context.Courses.Add(new Course { Id = 1, Code = "CS101", Name = "CS", Credits = 3, Capacity = 1, Instructor = "Dr. X", Semester = "Fall 2026", Status = "Open" });
            context.Students.Add(new Student { Id = 1, StudentId = "S001", Name = "Alice", Email = "a@a.com", Major = "CS", Year = "Freshman" });
            context.Students.Add(new Student { Id = 2, StudentId = "S002", Name = "Bob", Email = "b@b.com", Major = "CS", Year = "Junior" });
            context.Registrations.Add(new Registration { Id = 1, StudentId = 1, CourseId = 1, RegistrationDate = DateTime.Today });
            await context.SaveChangesAsync();

            var controller = new RegistrationsController(context);
            SetTempData(controller);

            var reg = new Registration { StudentId = 2, CourseId = 1 };
            var result = await controller.Create(reg);

            Assert.IsType<ViewResult>(result);
            Assert.True(controller.ModelState.ContainsKey("CourseId"));
        }
    }
}
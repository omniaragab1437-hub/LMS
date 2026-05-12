using Microsoft.AspNetCore.Mvc;
using LMSProject.ViewModel;
using System.Collections.Generic;

namespace LMSProject.Areas.Instructor.Controllers
{
    public class AssignmentsController : Controller
    {
        public IActionResult Index(
                string searchTerm,
                string selectedCourse,
                string selectedTerm,
                string selectedStatus)
        {
            var assignments = new List<Assignment>
            {
                new Assignment
                {
                    Id = 1,
                    Name = "Math Homework 1",
                    Course = "Mathematics",
                    Term = "Fall 2023",
                    StartDate = "2023-09-01",
                    EndDate = "2023-09-15",
                    Deadline = "2023-09-15",
                    Status = "Ended",
                    Submitted = 25,
                    Total = 30
                },

                new Assignment
                {
                    Id = 2,
                    Name = "Science Project",
                    Course = "Science",
                    Term = "Spring 2024",
                    StartDate = "2024-02-01",
                    EndDate = "2024-02-28",
                    Deadline = "2024-02-28",
                    Status = "Active",
                    Submitted = 18,
                    Total = 30
                }
            };

            if (!string.IsNullOrEmpty(searchTerm))
            {
                assignments = assignments
                    .Where(a => a.Name.ToLower()
                    .Contains(searchTerm.ToLower()))
                    .ToList();
            }

            if (!string.IsNullOrEmpty(selectedCourse))
            {
                assignments = assignments
                    .Where(a => a.Course == selectedCourse)
                    .ToList();
            }

            if (!string.IsNullOrEmpty(selectedTerm))
            {
                assignments = assignments
                    .Where(a => a.Term == selectedTerm)
                    .ToList();
            }

            if (!string.IsNullOrEmpty(selectedStatus))
            {
                assignments = assignments
                    .Where(a => a.Status == selectedStatus)
                    .ToList();
            }

            var vm = new AssignmentsViewModel
            {
                Assignments = assignments,

                Courses = new List<string>
                {
                    "Mathematics",
                    "Science",
                    "History",
                    "English"
                },

                Terms = new List<string>
                {
                    "Fall 2023",
                    "Spring 2024"
                },

                Statuses = new List<string>
                {
                    "Active",
                    "Ended"
                },

                SearchTerm = searchTerm,
                SelectedCourse = selectedCourse,
                SelectedTerm = selectedTerm,
                SelectedStatus = selectedStatus
            };

            return View(vm);
        }
    }
}

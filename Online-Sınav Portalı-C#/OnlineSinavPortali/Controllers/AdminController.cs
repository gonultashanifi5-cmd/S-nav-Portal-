using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineSinavPortali.Models;
using OnlineSinavPortali.Repositories;
using OnlineSinavPortali.ViewModels;

namespace OnlineSinavPortali.Controllers;

[Authorize(Roles = Models.User.AdminRole)]
public sealed class AdminController(
    GenericRepository<Course> courses,
    GenericRepository<Exam> exams,
    QuestionRepository questions) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var courseCount = await courses.Query().CountAsync();
        var examCount = await exams.Query().CountAsync();
        var questionCount = await questions.Query().CountAsync();
        var recent = await exams.Query().OrderByDescending(exam => exam.Id).Take(5)
            .Select(exam => new ExamRow(
                exam.Id, exam.Title, exam.Course.Name, exam.DurationMinutes, exam.Questions.Count))
            .ToListAsync();

        return View(new DashboardViewModel(courseCount, examCount, questionCount, recent));
    }
}

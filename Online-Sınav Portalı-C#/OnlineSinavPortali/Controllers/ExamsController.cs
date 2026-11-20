using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using OnlineSinavPortali.Models;
using OnlineSinavPortali.Repositories;
using OnlineSinavPortali.ViewModels;

namespace OnlineSinavPortali.Controllers;

[Authorize(Roles = Models.User.AdminRole)]
public sealed class ExamsController(
    GenericRepository<Exam> exams,
    GenericRepository<Course> courses,
    QuestionRepository questions) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Index() =>
        View(await ExamRows(exams.Query().OrderByDescending(exam => exam.Id)).ToListAsync());

    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        var exam = await ExamRows(exams.Query().Where(exam => exam.Id == id)).SingleOrDefaultAsync();
        if (exam is null)
        {
            return NotFound();
        }

        var rows = await questions.ListForExamAsync(id);
        return View(new ExamDetailsViewModel(exam, new QuestionFormViewModel(), rows.Select(QuestionRowFrom).ToList()));
    }

    [HttpGet]
    public async Task<IActionResult> Form(int id = 0)
    {
        var exam = id == 0 ? new Exam { DurationMinutes = 40 } : await exams.GetByIdAsync(id);
        if (exam is null)
        {
            return NotFound();
        }

        return View(await WithCoursesAsync(new ExamFormViewModel
        {
            Id = exam.Id,
            Title = exam.Title,
            CourseId = exam.CourseId,
            DurationMinutes = exam.DurationMinutes
        }));
    }

    [HttpPost]
    public async Task<IActionResult> Save(ExamFormViewModel model)
    {
        if (ModelState.IsValid && !await courses.Query().AnyAsync(course => course.Id == model.CourseId))
        {
            ModelState.AddModelError(nameof(model.CourseId), "Listeden geçerli bir ders seçin.");
        }

        if (!ModelState.IsValid)
        {
            return View(nameof(Form), await WithCoursesAsync(model));
        }

        var exam = model.Id == 0 ? new Exam() : await exams.GetByIdAsync(model.Id);
        if (exam is null)
        {
            return NotFound();
        }

        exam.Title = model.Title.Trim();
        exam.CourseId = model.CourseId;
        exam.DurationMinutes = model.DurationMinutes;
        try
        {
            if (model.Id == 0)
            {
                await exams.AddAsync(exam);
            }
            else
            {
                await exams.SaveAsync();
            }
        }
        catch (DbUpdateException exception) when (exception.InnerException is SqlException { Number: 547 })
        {
            ModelState.AddModelError(nameof(model.CourseId), "Seçilen ders silindi. Başka bir ders seçin.");
            return View(nameof(Form), await WithCoursesAsync(model));
        }

        TempData["Success"] = "Sınav kaydedildi.";
        return RedirectToAction(nameof(Details), new { id = exam.Id });
    }

    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        var exam = await exams.GetByIdAsync(id);
        if (exam is null)
        {
            return NotFound();
        }

        await exams.DeleteAsync(exam);
        TempData["Success"] = "Sınav ve bağlı soruları silindi.";
        return RedirectToAction(nameof(Index));
    }

    private static IQueryable<ExamRow> ExamRows(IQueryable<Exam> query) => query
        .Select(exam => new ExamRow(exam.Id, exam.Title, exam.Course.Name, exam.DurationMinutes, exam.Questions.Count));

    private async Task<ExamFormViewModel> WithCoursesAsync(ExamFormViewModel model)
    {
        model.Courses = await courses.Query().OrderBy(course => course.Code)
            .Select(course => new SelectListItem(course.Code + " · " + course.Name, course.Id.ToString()))
            .ToListAsync();
        return model;
    }

    internal static QuestionRow QuestionRowFrom(Question question) => new(
        question.Id, question.Text, question.Points, question.CorrectOptionPosition,
        question.Options.OrderBy(option => option.Position).Select(option => option.Text).ToList());
}

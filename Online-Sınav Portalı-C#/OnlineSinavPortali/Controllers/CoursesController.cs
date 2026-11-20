using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using OnlineSinavPortali.Models;
using OnlineSinavPortali.Repositories;
using OnlineSinavPortali.ViewModels;

namespace OnlineSinavPortali.Controllers;

[Authorize(Roles = Models.User.AdminRole)]
public sealed class CoursesController(GenericRepository<Course> courses, GenericRepository<Exam> exams) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var items = await courses.Query().OrderBy(course => course.Code)
            .Select(course => new CourseRow(course.Id, course.Code, course.Name, course.Exams.Count))
            .ToListAsync();

        return View(items);
    }

    [HttpGet]
    public async Task<IActionResult> Form(int id = 0)
    {
        if (id == 0)
        {
            return View(new CourseFormViewModel());
        }

        var course = await courses.GetByIdAsync(id);
        return course is null ? NotFound() : View(new CourseFormViewModel
        {
            Id = course.Id,
            Code = course.Code,
            Name = course.Name
        });
    }

    [HttpPost]
    public async Task<IActionResult> Save(CourseFormViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(nameof(Form), model);
        }

        var course = model.Id == 0 ? new Course() : await courses.GetByIdAsync(model.Id);
        if (course is null)
        {
            return NotFound();
        }

        course.Code = model.Code.Trim().ToUpperInvariant();
        course.Name = model.Name.Trim();
        if (await courses.Query().AnyAsync(item => item.Code == course.Code && item.Id != course.Id))
        {
            ModelState.AddModelError(nameof(model.Code), "Bu ders kodu zaten kullanılıyor.");
            return View(nameof(Form), model);
        }

        try
        {
            if (model.Id == 0)
            {
                await courses.AddAsync(course);
            }
            else
            {
                await courses.SaveAsync();
            }
        }
        catch (DbUpdateException exception) when (exception.InnerException is SqlException { Number: 2601 or 2627 })
        {
            ModelState.AddModelError(nameof(model.Code), "Bu ders kodu zaten kullanılıyor.");
            return View(nameof(Form), model);
        }

        TempData["Success"] = "Ders kaydedildi.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        var course = await courses.GetByIdAsync(id);
        if (course is null)
        {
            return NotFound();
        }

        if (await exams.Query().AnyAsync(exam => exam.CourseId == id))
        {
            TempData["Error"] = "Dersi silmek için önce bu derse bağlı sınavları silin veya başka bir derse taşıyın.";
            return RedirectToAction(nameof(Index));
        }

        try
        {
            await courses.DeleteAsync(course);
            TempData["Success"] = "Ders silindi.";
        }
        catch (DbUpdateException exception) when (exception.InnerException is SqlException { Number: 547 })
        {
            TempData["Error"] = "Derse bir sınav bağlı olduğu için ders silinemedi.";
        }

        return RedirectToAction(nameof(Index));
    }
}

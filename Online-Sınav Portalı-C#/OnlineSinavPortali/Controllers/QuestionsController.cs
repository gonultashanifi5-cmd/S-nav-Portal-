using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using OnlineSinavPortali.Models;
using OnlineSinavPortali.Repositories;
using OnlineSinavPortali.ViewModels;

namespace OnlineSinavPortali.Controllers;

[ApiController]
[Authorize(Roles = Models.User.AdminRole)]
[Route("admin/exams/{examId:int}/questions")]
public sealed class QuestionsController(GenericRepository<Exam> exams, QuestionRepository questions) : Controller
{
    [HttpGet]
    public async Task<IActionResult> List(int examId)
    {
        if (!await exams.Query().AnyAsync(exam => exam.Id == examId))
        {
            return NotFound();
        }

        var rows = await questions.ListForExamAsync(examId);
        return PartialView("_QuestionList", rows.Select(ExamsController.QuestionRowFrom).ToList());
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Get(int examId, int id)
    {
        var question = await questions.GetForExamAsync(examId, id);
        return question is null ? NotFound() : Ok(ExamsController.QuestionRowFrom(question));
    }

    [HttpPost]
    public async Task<IActionResult> Save(int examId, [FromForm] QuestionFormViewModel model)
    {
        if (!await exams.Query().AnyAsync(exam => exam.Id == examId))
        {
            return NotFound();
        }

        var question = model.Id == 0
            ? new Question { ExamId = examId }
            : await questions.GetForExamAsync(examId, model.Id);
        if (question is null)
        {
            return NotFound();
        }

        question.Text = model.Text.Trim();
        question.Points = model.Points;
        question.CorrectOptionPosition = model.CorrectOptionPosition;
        if (model.Id == 0)
        {
            question.Options = model.Options.Select((option, index) => new Option
            {
                Position = index + 1,
                Text = option.Text.Trim()
            }).ToList();

            try
            {
                await questions.AddAsync(question);
            }
            catch (DbUpdateException exception) when (exception.InnerException is SqlException { Number: 547 })
            {
                return NotFound();
            }
        }
        else
        {
            foreach (var option in question.Options)
            {
                option.Text = model.Options[option.Position - 1].Text.Trim();
            }

            try
            {
                await questions.SaveAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                return NotFound();
            }
        }

        return Ok();
    }

    [HttpPost("{id:int}/delete")]
    public async Task<IActionResult> Delete(int examId, int id)
    {
        var question = await questions.GetForExamAsync(examId, id);
        if (question is null)
        {
            return NotFound();
        }

        try
        {
            await questions.DeleteAsync(question);
            return Ok();
        }
        catch (DbUpdateConcurrencyException)
        {
            return NotFound();
        }
    }
}

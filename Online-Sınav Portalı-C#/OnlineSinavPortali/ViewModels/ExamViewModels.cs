using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace OnlineSinavPortali.ViewModels;

public sealed class ExamFormViewModel
{
    public int Id { get; set; }

    [Display(Name = "Ders")]
    [Range(1, int.MaxValue, ErrorMessage = "Bir ders seçin.")]
    public int CourseId { get; set; }

    [Display(Name = "Sınav adı")]
    [Required(ErrorMessage = "Sınav adını girin.")]
    [StringLength(120, ErrorMessage = "Sınav adı en fazla 120 karakter olabilir.")]
    public string Title { get; set; } = "";

    [Display(Name = "Süre (dakika)")]
    [Range(1, 300, ErrorMessage = "Süre 1 ile 300 dakika arasında olmalı.")]
    public int DurationMinutes { get; set; } = 40;

    [BindNever]
    public List<SelectListItem> Courses { get; set; } = [];
}

public sealed record ExamRow(int Id, string Title, string CourseName, int DurationMinutes, int QuestionCount);

public sealed record ExamDetailsViewModel(ExamRow Exam, QuestionFormViewModel Question, IReadOnlyList<QuestionRow> Questions);

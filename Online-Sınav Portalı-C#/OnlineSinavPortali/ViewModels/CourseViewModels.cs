using System.ComponentModel.DataAnnotations;

namespace OnlineSinavPortali.ViewModels;

public sealed class CourseFormViewModel
{
    public int Id { get; set; }

    [Display(Name = "Ders kodu")]
    [Required(ErrorMessage = "Ders kodunu girin.")]
    [StringLength(20, ErrorMessage = "Ders kodu en fazla 20 karakter olabilir.")]
    public string Code { get; set; } = "";

    [Display(Name = "Ders adı")]
    [Required(ErrorMessage = "Ders adını girin.")]
    [StringLength(120, ErrorMessage = "Ders adı en fazla 120 karakter olabilir.")]
    public string Name { get; set; } = "";
}

public sealed record CourseRow(int Id, string Code, string Name, int ExamCount);

using System.ComponentModel.DataAnnotations;

namespace OnlineSinavPortali.ViewModels;

public sealed class QuestionFormViewModel
{
    public int Id { get; set; }

    [Display(Name = "Soru metni")]
    [Required(ErrorMessage = "Soru metnini girin.")]
    [StringLength(1000, ErrorMessage = "Soru metni en fazla 1000 karakter olabilir.")]
    public string Text { get; set; } = "";

    [Display(Name = "Puan")]
    [Range(1, 100, ErrorMessage = "Puan 1 ile 100 arasında olmalı.")]
    public int Points { get; set; } = 10;

    [Display(Name = "Doğru cevap")]
    [Range(1, 4, ErrorMessage = "Doğru cevabı seçin.")]
    public int CorrectOptionPosition { get; set; }

    [MinLength(4, ErrorMessage = "Dört seçenek girin.")]
    [MaxLength(4, ErrorMessage = "Dört seçenek girin.")]
    public List<OptionInput> Options { get; set; } = [new(), new(), new(), new()];
}

public sealed class OptionInput
{
    [Required(ErrorMessage = "Seçenek metnini girin.")]
    [StringLength(300, ErrorMessage = "Seçenek en fazla 300 karakter olabilir.")]
    public string Text { get; set; } = "";
}

public sealed record QuestionRow(int Id, string Text, int Points, int CorrectOptionPosition, IReadOnlyList<string> Options);

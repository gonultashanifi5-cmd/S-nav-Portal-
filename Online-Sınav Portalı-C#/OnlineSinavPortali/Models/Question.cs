namespace OnlineSinavPortali.Models;

public sealed class Question : BaseEntity
{
    public int ExamId { get; set; }
    public Exam Exam { get; set; } = null!;
    public string Text { get; set; } = "";
    public int Points { get; set; }
    public int CorrectOptionPosition { get; set; }
    public List<Option> Options { get; set; } = [];
}

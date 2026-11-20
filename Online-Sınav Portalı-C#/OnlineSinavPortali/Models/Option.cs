namespace OnlineSinavPortali.Models;

public sealed class Option : BaseEntity
{
    public int QuestionId { get; set; }
    public Question Question { get; set; } = null!;
    public int Position { get; set; }
    public string Text { get; set; } = "";
}

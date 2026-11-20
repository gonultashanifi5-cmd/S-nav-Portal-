namespace OnlineSinavPortali.Models;

public sealed class Exam : BaseEntity
{
    public int CourseId { get; set; }
    public Course Course { get; set; } = null!;
    public string Title { get; set; } = "";
    public int DurationMinutes { get; set; }
    public List<Question> Questions { get; set; } = [];
}

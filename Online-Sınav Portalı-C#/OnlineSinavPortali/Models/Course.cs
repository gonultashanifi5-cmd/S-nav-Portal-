namespace OnlineSinavPortali.Models;

public sealed class Course : BaseEntity
{
    public string Code { get; set; } = "";
    public string Name { get; set; } = "";
    public List<Exam> Exams { get; set; } = [];
}

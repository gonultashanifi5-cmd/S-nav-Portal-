namespace OnlineSinavPortali.ViewModels;

public sealed record DashboardViewModel(int CourseCount, int ExamCount, int QuestionCount, IReadOnlyList<ExamRow> RecentExams);

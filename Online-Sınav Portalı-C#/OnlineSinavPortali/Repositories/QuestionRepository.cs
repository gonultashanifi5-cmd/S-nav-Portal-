using Microsoft.EntityFrameworkCore;
using OnlineSinavPortali.Models;

namespace OnlineSinavPortali.Repositories;

public sealed class QuestionRepository(AppDbContext context) : GenericRepository<Question>(context)
{
    public Task<List<Question>> ListForExamAsync(int examId) => Query()
        .Where(question => question.ExamId == examId)
        .Include(question => question.Options)
        .OrderBy(question => question.Id)
        .ToListAsync();

    public Task<Question?> GetForExamAsync(int examId, int questionId) => Entities
        .Include(question => question.Options)
        .SingleOrDefaultAsync(question => question.Id == questionId && question.ExamId == examId);
}

using QuizWebApp.Core.Entities;
using QuizWebApp.Shared.ApiResponses;
using QuizWebApp.Shared.DTOs.Common;

namespace QuizWebApp.Core.Interfaces.Repositories;

public interface IAttemptsRepository
{
    Task<PagedInfo<Attempt>> GetQuizAttemptsAsync(Guid quizId, PaginationDTO paginationData);
    
    Task<PagedInfo<Attempt>> GetParticipantAttemptsAsync(int participantId, PaginationDTO paginationData);
    
    Task<Attempt?> GetByIdWithQuizAndQuestionsAsync(int id);
    
    Task<Attempt?> GetByIdWithQuestionsOnlyAsync(int id);
    
    Task AddAsync(Attempt attempt);
    
    Task UpdateAsync(Attempt attempt);
    
    Task AddAttemptQuestionAsync(AttemptQuestion attemptQuestion);
    
    Task RemoveAttemptQuestionsRangeAsync(IEnumerable<AttemptQuestion> attemptQuestions);
}

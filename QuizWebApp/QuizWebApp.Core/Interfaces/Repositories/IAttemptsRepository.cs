using QuizWebApp.Core.Entities;
using QuizWebApp.Shared.ApiResponses;
using QuizWebApp.Shared.DTOs.Common;
using QuizWebApp.Shared.DTOs.Filter;

namespace QuizWebApp.Core.Interfaces.Repositories;

public interface IAttemptsRepository
{
    Task<PagedInfo<Attempt>> GetQuizAttemptsAsync(Guid quizId, PaginationDTO paginationData, AttemptFilterDTO filterData);
    
    Task<PagedInfo<Attempt>> GetParticipantAttemptsAsync(int participantId, PaginationDTO paginationData, AttemptFilterDTO filterData);
    
    Task<Attempt?> GetByIdWithQuizAndQuestionsAsync(int id);
    
    Task<Attempt?> GetByIdWithQuestionsOnlyAsync(int id);
    
    Task AddAsync(Attempt attempt);
    
    Task UpdateAsync(Attempt attempt);
    
    Task AddAttemptQuestionAsync(AttemptQuestion attemptQuestion);
    
    Task RemoveAttemptQuestionsRangeAsync(IEnumerable<AttemptQuestion> attemptQuestions);
}

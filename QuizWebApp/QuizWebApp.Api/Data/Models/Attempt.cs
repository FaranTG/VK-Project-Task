using System.ComponentModel.DataAnnotations;
using QuizWebApp.Api.Data.DataEnums;

namespace QuizWebApp.Api.Data.Models;

public class Attempt
{
    public int Id { get; set; }

    public int ParticipantId { get; set; }

    public User? Participant { get; set; }

    public Guid QuizId { get; set; }

    public Quiz? Quiz { get; set; }

    [AllowedValues(
        nameof(ParticipantQuizStatus.Started),
        nameof(ParticipantQuizStatus.Completed),
        nameof(ParticipantQuizStatus.Exited),
        nameof(ParticipantQuizStatus.AutoSubmitted)
    )]
    public required string Status { get; set; }

    public DateTime StartTime { get; set; }

    public DateTime EndTime { get; set; }

    public int Score { get; set; }

    public ICollection<AttemptQuestion>? AttemptQuestions { get; set; } 
}

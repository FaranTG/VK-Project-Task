using System.ComponentModel.DataAnnotations;
using QuizWebApp.Shared.Enums;

namespace QuizWebApp.Core.Entities;

public class Attempt
{
    public int Id { get; set; }

    public int ParticipantId { get; set; }

    public User? Participant { get; set; }

    public Guid QuizId { get; set; }

    public Quiz? Quiz { get; set; }

    [AllowedValues(
        nameof(AttemptStatus.Started),
        nameof(AttemptStatus.Completed),
        nameof(AttemptStatus.Exited),
        nameof(AttemptStatus.AutoSubmitted)
    )]
    public required string Status { get; set; }

    public DateTimeOffset StartTime { get; set; }

    public DateTimeOffset EndTime { get; set; }

    [Range(0, 20)]
    public int Score { get; set; }

    public List<AttemptQuestion>? AttemptQuestions { get; set; } 
}

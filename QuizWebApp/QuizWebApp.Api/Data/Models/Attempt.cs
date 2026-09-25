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
        nameof(AttemptStatus.Started),
        nameof(AttemptStatus.Completed),
        nameof(AttemptStatus.Exited),
        nameof(AttemptStatus.AutoSubmitted)
    )]
    public required string Status { get; set; }

    public DateTime StartTime { get; set; }

    public DateTime EndTime { get; set; }

    public int Score { get; set; }

    public ICollection<AttemptQuestion>? AttemptQuestions { get; set; } 
}

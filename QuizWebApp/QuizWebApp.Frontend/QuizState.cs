using QuizWebApp.Shared.DTOs.Quiz;

namespace QuizWebApp.Frontend;

public class QuizState
{
    public QuizBriefInfoDTO? SelectedQuiz { get; private set; }
    public int AttemptId { get; private set; }
    public bool IsQuizSelected => SelectedQuiz is not null && AttemptId != 0;

    public void SetSelectedQuiz(QuizBriefInfoDTO quiz, int attemptId)
    {
        SelectedQuiz = quiz;
        AttemptId = attemptId;
    }

    public void ResetSelectedQuiz()
    {
        SelectedQuiz = null;
        AttemptId = 0;
    }
}

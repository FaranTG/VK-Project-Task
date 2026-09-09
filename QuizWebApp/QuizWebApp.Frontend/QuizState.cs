using QuizWebApp.Shared.DTOs.Quiz;

namespace QuizWebApp.Frontend;

public class QuizState
{
    public QuizBriefInfoDTO? SelectedQuiz { get; private set; }

    public void SetSelectedQuiz(QuizBriefInfoDTO quiz)
    {
        SelectedQuiz = quiz;
    }

    public void ResetSelectedQuiz()
    {
        SelectedQuiz = null;
    }
}

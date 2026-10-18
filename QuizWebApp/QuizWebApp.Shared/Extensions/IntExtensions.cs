using QuizWebApp.Shared.Enums;

namespace QuizWebApp.Shared.Extensions;

public static class IntExtensions
{
    public static TopicFilterStatus ToFilterStatus(this int topicId)
    {
        return topicId < 0
            ? TopicFilterStatus.Invalid
            : topicId == 0
                ? TopicFilterStatus.Empty
                : TopicFilterStatus.Valid;
    }
}

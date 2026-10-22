using QuizWebApp.Shared.DTOs.Topic;

namespace QuizWebApp.Core.Validation;

public static class TopicSaveDTOValidator
{
    public static string? Validate(this TopicSaveDTO topicData)
    {
        if (string.IsNullOrWhiteSpace(topicData.Name))
        {
            return "Topic name is required.";
        }

        if (topicData.Name.Length > 20)
        {
            return "Topic name cannot exceed 20 characters.";
        }

        return null;
    }
}

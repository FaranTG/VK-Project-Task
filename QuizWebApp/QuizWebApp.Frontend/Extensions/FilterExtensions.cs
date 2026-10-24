using QuizWebApp.Shared.DTOs.Common;
using QuizWebApp.Shared.DTOs.Filter;

namespace QuizWebApp.Frontend.Extensions;

public static class FilterExtensions
{
    public static string CreateFilterParameters(this AttemptFilterDTO filterData, PaginationDTO paginationData)
    {
        string parameters = CreateBaseFilterParameters(paginationData);

        parameters = parameters.AddSearchFilterParameters(filterData.SearchFilter);

        if (filterData.StatusFilter is not null)
        {
            parameters += $"&statusFilter={filterData.StatusFilter}";
        }

        return parameters;
    }

    public static string CreateFilterParameters(this QuizFilterDTO filterData, PaginationDTO paginationData)
    {
        string parameters = CreateBaseFilterParameters(paginationData);

        parameters = parameters.AddSearchFilterParameters(filterData.SearchFilter);

        parameters += $"&topicFilterId={filterData.TopicFilterId}";

        return parameters;
    }

    public static string CreateFilterParameters(this UserFilterDTO filterData, PaginationDTO paginationData)
    {
        string parameters = CreateBaseFilterParameters(paginationData);

        parameters = parameters.AddSearchFilterParameters(filterData.SearchFilter);

        if (filterData.IsApprovedFilter is not null)
        {
            parameters += $"&isApprovedFilter={filterData.IsApprovedFilter.Value.ToString().ToLower()}";
        }

        return parameters;
    }

    public static string CreateFilterParameters(this TopicFilterDTO? filterData, PaginationDTO? paginationData)
    {
        string parameters = CreateBaseFilterParameters(paginationData);
        
        if (filterData is not null)
        {
            parameters = parameters.AddSearchFilterParameters(filterData.SearchFilter);
        }

        return parameters;
    }

    private static string AddSearchFilterParameters(this string parameters, string? searchFilter)
    {
        if (!string.IsNullOrWhiteSpace(searchFilter))
        {
            parameters += $"&searchFilter={Uri.EscapeDataString(searchFilter.Trim())}";
        }

        return parameters;
    }

    private static string CreateBaseFilterParameters(PaginationDTO? paginationData)
    {
        string parameters = string.Empty;

        if (paginationData is not null)
        {
            parameters += $"{paginationData.CreatePaginationParameters()}";
        }

        return parameters;
    }
}

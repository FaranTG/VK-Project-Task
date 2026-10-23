using System.Security.Claims;
using QuizWebApp.Api.Extensions;
using QuizWebApp.Core.Interfaces.Services;
using QuizWebApp.Shared.ApiResponses;
using QuizWebApp.Shared.DTOs.Common;
using QuizWebApp.Shared.DTOs.Question;
using QuizWebApp.Shared.Enums;

namespace QuizWebApp.Api.Endpoints;

public static class AttemptsEndpoints
{
    private const string ApiRoute = "/api/attempts";

    public static IEndpointRouteBuilder MapAttemptsEndpoints(this IEndpointRouteBuilder app)
    {
        MapOrganizerEndpoints(app);
        MapParticipantEndpoints(app);

        return app;
    }

    private static void MapOrganizerEndpoints(IEndpointRouteBuilder app)
    {
        RouteGroupBuilder organizerRouteGroup = app
            .MapGroup($"{ApiRoute}/organizer")
            .RequireAuthorization(policy => policy.RequireRole(nameof(UserRole.Organizer)));

        MapQuizAttemptsGetEndpoint(organizerRouteGroup);
    }

    private static void MapParticipantEndpoints(IEndpointRouteBuilder app)
    {
        RouteGroupBuilder participantRouteGroup = app
            .MapGroup($"{ApiRoute}/participant")
            .RequireAuthorization(policy => policy.RequireRole(nameof(UserRole.Participant)));

        MapParticipantAttemptsGetEndpoint(participantRouteGroup);

        RouteGroupBuilder takeQuizRouteGroup = participantRouteGroup
            .MapGroup("/take-quiz");
        
        MapQuizStartEndpoint(takeQuizRouteGroup);
        MapQuizNextQuestionGetEndpoint(takeQuizRouteGroup);
        MapQuizResponsePostEndpoint(takeQuizRouteGroup);
        MapAllQuizSubmitEndpoints(takeQuizRouteGroup);
    }

    private static void MapQuizAttemptsGetEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/quiz-info/{quizId:guid}", async (Guid quizId, [AsParameters] PaginationDTO paginationData, IAttemptsService attemptService) =>
            Results.Ok(await attemptService.GetQuizAttemptsAsync(quizId, paginationData))
        );
    }

    private static void MapParticipantAttemptsGetEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/", async ([AsParameters] PaginationDTO paginationData, ClaimsPrincipal principal, IAttemptsService attemptService) =>
            Results.Ok(await attemptService.GetParticipantAttemptsAsync(principal.GetParticipantId(), paginationData))
        );
    }

    private static void MapQuizStartEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/{quizId:guid}/start", async (Guid quizId, ClaimsPrincipal principal, IAttemptsService attemptService) =>
        {
            QuizApiResponse<int> response = await attemptService.StartQuizAsync(quizId, principal.GetParticipantId());

            return response.IsSuccess
                ? Results.Ok(response)
                : Results.BadRequest(response);
        });
    }

    private static void MapQuizNextQuestionGetEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/{attemptId:int}/next-question", async (int attemptId, ClaimsPrincipal principal, IAttemptsService attemptService) =>
        {
            QuizApiResponse<QuestionInfoDTO> response = await attemptService.GetNextQuizQuestionAsync(attemptId, principal.GetParticipantId());

            if (response.IsFailure)
            {
                return response.ErrorMessage == IAttemptsService.NotFoundMessage
                    ? Results.NotFound(response)
                    : Results.BadRequest(response);
            }

            return Results.Ok(response);
        });
    }

    private static void MapQuizResponsePostEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/{attemptId:int}/save-response", async (int attemptId, QuestionResponseSaveDTO responseData, ClaimsPrincipal principal, IAttemptsService attemptService) =>
        {
            if (responseData.AttemptId != attemptId)
            {
                return Results.BadRequest(QuizApiResponse.Fail("Invalid attempt id in Response Data."));
            }

            QuizApiResponse response = await attemptService.SaveQuestionResponseAsync(responseData, principal.GetParticipantId());

            if (response.IsFailure)
            {
                return response.ErrorMessage == IAttemptsService.NotFoundMessage
                    ? Results.NotFound(response)
                    : Results.BadRequest(response);
            }

            return Results.Ok(response);
        });
    }

    private static void MapAllQuizSubmitEndpoints(IEndpointRouteBuilder app)
    {
        MapQuizSubmitEndpoint(app, "complete", AttemptStatus.Completed);
        MapQuizSubmitEndpoint(app, "auto-submit", AttemptStatus.AutoSubmitted);
        MapQuizSubmitEndpoint(app, "exit", AttemptStatus.Exited);
    }

    private static void MapQuizSubmitEndpoint(IEndpointRouteBuilder app, string endpointName, AttemptStatus quizStatus)
    {
        string url = "/{attemptId:int}" + $"/{endpointName}";
        app.MapPost(url, async (int attemptId, ClaimsPrincipal principal, IAttemptsService attemptService) =>
        {
            QuizApiResponse response = await attemptService.SubmitQuizAsync(attemptId, quizStatus, principal.GetParticipantId());

            if (response.IsFailure)
            {
                return response.ErrorMessage == IAttemptsService.NotFoundMessage
                    ? Results.NotFound(response)
                    : Results.BadRequest(response);
            }

            return Results.Ok(response);
        });
    }
}

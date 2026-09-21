using System.Security.Claims;
using QuizWebApp.Api.Configuration;
using QuizWebApp.Api.Data.DataEnums;
using QuizWebApp.Api.Services.Interfaces;
using QuizWebApp.Shared.ApiResponses;
using QuizWebApp.Shared.DTOs.Question;
using QuizWebApp.Shared.Enums;

namespace QuizWebApp.Api.Endpoints;

public static class ParticipantQuizEndpoints
{
    private const string ApiRoute = "/api/participant";

    public static IEndpointRouteBuilder MapParticipantQuizEndpoints(this IEndpointRouteBuilder app)
    {
        RouteGroupBuilder routeGroup = app
            .MapGroup(ApiRoute)
            .RequireAuthorization(policy => policy.RequireRole(nameof(UserRole.Participant)));

        MapQuizGetActiveEndpoint(routeGroup);

        RouteGroupBuilder quizRouteGroup = routeGroup
            .MapGroup("/take-quiz");
        
        MapQuizStartEndpoint(quizRouteGroup);
        MapQuizNextQuestionGetEndpoint(quizRouteGroup);
        MapQuizResponsePostEndpoint(quizRouteGroup);
        MapAllQuizSubmitEndpoints(quizRouteGroup);

        return app;
    }

    private static void MapQuizGetActiveEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/available-quizzes", async (int topicIdFilter, IParticipantQuizService quizService) =>
            Results.Ok(await quizService.GetActiveQuizzesAsync(topicIdFilter))
        );
    }

    private static void MapQuizStartEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/{quizId:guid}/start", async (Guid quizId, ClaimsPrincipal principal, IParticipantQuizService quizService) =>
        {
            QuizApiResponse<int> response = await quizService.StartQuizAsync(quizId, principal.GetParticipantId());

            return response.IsSuccess
                ? Results.Ok(response)
                : Results.BadRequest(response);
        });
    }

    private static void MapQuizNextQuestionGetEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/{attemptId:int}/next-question", async (int attemptId, ClaimsPrincipal principal, IParticipantQuizService quizService) =>
        {
            QuizApiResponse<QuestionInfoDTO> response = await quizService.GetNextQuizQuestionAsync(attemptId, principal.GetParticipantId());

            if (response.IsFailure)
            {
                return response.ErrorMessage == IParticipantQuizService.NotFoundMessage
                    ? Results.NotFound(response)
                    : Results.BadRequest(response);
            }

            return Results.Ok(response);
        });
    }

    private static void MapQuizResponsePostEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/{attemptId:int}/save-response", async (int attemptId, QuestionResponseSaveDTO responseData, ClaimsPrincipal principal, IParticipantQuizService quizService) =>
        {
            if (responseData.AttemptId != attemptId)
            {
                return Results.BadRequest(QuizApiResponse.Fail("Invalid attempt id in Response Data."));
            }

            QuizApiResponse response = await quizService.SaveQuestionResponseAsync(responseData, principal.GetParticipantId());

            if (response.IsFailure)
            {
                return response.ErrorMessage == IParticipantQuizService.NotFoundMessage
                    ? Results.NotFound(response)
                    : Results.BadRequest(response);
            }

            return Results.Ok(response);
        });
    }

    private static void MapAllQuizSubmitEndpoints(IEndpointRouteBuilder app)
    {
        MapQuizSubmitEndpoint(app, "complete", ParticipantQuizStatus.Completed);
        MapQuizSubmitEndpoint(app, "auto-submit", ParticipantQuizStatus.AutoSubmitted);
        MapQuizSubmitEndpoint(app, "exit", ParticipantQuizStatus.Exited);
    }

    private static void MapQuizSubmitEndpoint(IEndpointRouteBuilder app, string endpointName, ParticipantQuizStatus quizStatus)
    {
        string url = "/{attemptId:int}" + $"/{endpointName}";
        app.MapPost(url, async (int attemptId, ClaimsPrincipal principal, IParticipantQuizService quizService) =>
        {
            QuizApiResponse response = await quizService.SubmitQuizAsync(attemptId, quizStatus, principal.GetParticipantId());

            if (response.IsFailure)
            {
                return response.ErrorMessage == IParticipantQuizService.NotFoundMessage
                    ? Results.NotFound(response)
                    : Results.BadRequest(response);
            }

            return Results.Ok(response);
        });
    }
}

using Microsoft.EntityFrameworkCore;
using QuizWebApp.Api.Data;
using QuizWebApp.Api.Data.Models;
using QuizWebApp.Api.Services.Interfaces;
using QuizWebApp.Shared.ApiResponses;
using QuizWebApp.Shared.DTOs.Common;
using QuizWebApp.Shared.DTOs.User;
using QuizWebApp.Shared.Enums;

namespace QuizWebApp.Api.Services;

public class UserService : IUserService
{
    private readonly QuizContext _dbContext;

    public UserService(QuizContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<QuizApiResponse<PagedInfoArray<UserInfoDTO>>> GetUsersAsync(UserApprovedFilter approvedFilter, PaginationDTO paginationData)
    {
        try
        {
            IQueryable<User> query = _dbContext.Users.AsNoTracking();
            query = approvedFilter switch
            {
                UserApprovedFilter.ApprovedOnly => query.Where(user => user.IsApproved),
                UserApprovedFilter.NotApprovedOnly => query.Where(user => !user.IsApproved),
                _ => query
            };

            int totalCount = await query.CountAsync();
            UserInfoDTO[] users = await query
                .OrderByDescending(user => user.Id)
                .Skip((paginationData.PageNumber - 1) * paginationData.PageSize)
                .Take(paginationData.PageSize)
                .Select(user => new UserInfoDTO
                (
                    user.Id,
                    user.Name,
                    user.Phone,
                    user.Email,
                    user.Role,
                    user.IsApproved
                ))
                .ToArrayAsync();
            
            return QuizApiResponse<PagedInfoArray<UserInfoDTO>>.Success(new PagedInfoArray<UserInfoDTO>(users, totalCount));
        }
        catch (Exception exception)
        {
            return QuizApiResponse<PagedInfoArray<UserInfoDTO>>.Fail(exception.Message);
        }
    }

    public async Task<QuizApiResponse> ToggleUserApprovedStatusAsync(int userId)
    {
        try
        {
            User? user = await _dbContext.Users.FindAsync(userId);

            if (user is null)
            {
                return QuizApiResponse.Fail(IUserService.NotFoundMessage);
            }

            user.IsApproved = !user.IsApproved;

            await _dbContext.SaveChangesAsync();

            return QuizApiResponse.Success();
        }
        catch (Exception exception)
        {
            return QuizApiResponse.Fail(exception.Message);
        }
    }
}

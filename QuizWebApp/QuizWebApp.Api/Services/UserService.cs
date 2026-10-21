using Microsoft.EntityFrameworkCore;
using QuizWebApp.Api.Configuration;
using QuizWebApp.Api.Data;
using QuizWebApp.Api.Data.Models;
using QuizWebApp.Api.Services.Interfaces;
using QuizWebApp.Infrastructure.Extensions;
using QuizWebApp.Shared.ApiResponses;
using QuizWebApp.Shared.DTOs.Common;
using QuizWebApp.Shared.DTOs.User;

namespace QuizWebApp.Api.Services;

public class UserService : IUserService
{
    private readonly QuizContext _dbContext;

    public UserService(QuizContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<QuizApiResponse<PagedInfo<UserInfoDTO>>> GetUsersAsync(PaginationDTO paginationData, bool? isApprovedFilter)
    {
        try
        {
            IQueryable<User> query = _dbContext.Users.AsNoTracking();

            if (isApprovedFilter is not null)
            {
                query = query.Where(user => user.IsApproved == isApprovedFilter);
            }

            int totalCount = await query.CountAsync();
            UserInfoDTO[] users = await query
                .OrderByDescending(user => user.Id)
                .Paginate(paginationData)
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
            
            return QuizApiResponse<PagedInfo<UserInfoDTO>>.Success(new PagedInfo<UserInfoDTO>(users, totalCount));
        }
        catch (Exception exception)
        {
            return QuizApiResponse<PagedInfo<UserInfoDTO>>.Fail(exception.Message);
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

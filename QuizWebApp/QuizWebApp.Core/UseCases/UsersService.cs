using QuizWebApp.Core.Entities;
using QuizWebApp.Core.Interfaces.Repositories;
using QuizWebApp.Core.Interfaces.Services;
using QuizWebApp.Shared.ApiResponses;
using QuizWebApp.Shared.DTOs.Common;
using QuizWebApp.Shared.DTOs.Filter;
using QuizWebApp.Shared.DTOs.User;

namespace QuizWebApp.Core.UseCases;

public class UsersService : IUsersService
{
    private readonly IUsersRepository _usersRepository;

    public UsersService(IUsersRepository usersRepository)
    {
        _usersRepository = usersRepository;
    }

    public async Task<QuizApiResponse<PagedInfo<UserInfoDTO>>> GetUsersAsync(PaginationDTO paginationData, UserFilterDTO filterData)
    {
        try
        {
            PagedInfo<User> pagedUsers = await _usersRepository
                .GetUsersAsync(paginationData, filterData);

            UserInfoDTO[] userDTOs = pagedUsers.Data
                .Select(CreateUserInfoDTO)
                .ToArray();
            
            PagedInfo<UserInfoDTO> result = new (userDTOs, pagedUsers.TotalCount);
            
            return QuizApiResponse<PagedInfo<UserInfoDTO>>.Success(result);
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
            User? user = await _usersRepository
                .GetByIdAsync(userId);

            if (user is null)
            {
                return QuizApiResponse.Fail(IUsersService.NotFoundMessage);
            }

            user.IsApproved = !user.IsApproved;

            await _usersRepository
                .UpdateAsync(user);

            return QuizApiResponse.Success();
        }
        catch (Exception exception)
        {
            return QuizApiResponse.Fail(exception.Message);
        }
    }

    private UserInfoDTO CreateUserInfoDTO(User user)
        => new 
        (
            user.Id,
            user.Name,
            user.Phone,
            user.Email,
            user.Role,
            user.IsApproved
        );
}

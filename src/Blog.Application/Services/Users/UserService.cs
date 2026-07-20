using Blog.Application.DTOs.Users;
using Blog.Application.Interfaces.Repositories;
using Blog.Application.Interfaces.Services;
using Blog.Domain.Entities;

namespace Blog.Application.Services.Users;

public class UserService : IUserService
{
    private readonly IUserRepository _users;
    public UserService(IUserRepository users) => _users = users;
    public async Task<UserProfileResponse> GetProfileAsync(Guid userId) => Map(await _users.GetByIdAsync(userId) ?? throw new InvalidOperationException("User not found."));
    public async Task<UserProfileResponse> UpdateProfileAsync(Guid userId, UpdateProfileRequest request)
    {
        var user = await _users.GetByIdAsync(userId) ?? throw new InvalidOperationException("User not found.");
        user.FirstName = request.FirstName.Trim(); user.Surname = request.Surname.Trim(); user.Bio = request.Bio?.Trim(); user.Avatar = request.Avatar?.Trim(); user.UpdatedAt = DateTime.UtcNow;
        await _users.UpdateAsync(user); await _users.SaveChangesAsync(); return Map(user);
    }
    private static UserProfileResponse Map(User user) => new() { Id = user.Id, FirstName = user.FirstName, Surname = user.Surname, Email = user.Email, Bio = user.Bio, Avatar = user.Avatar, Role = user.Role.ToString(), CreatedAt = user.CreatedAt, UpdatedAt = user.UpdatedAt };
}

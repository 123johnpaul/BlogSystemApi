using Blog.Application.DTOs.Users;
namespace Blog.Application.Interfaces.Services;
public interface IUserService { Task<UserProfileResponse> GetProfileAsync(Guid userId); Task<UserProfileResponse> UpdateProfileAsync(Guid userId, UpdateProfileRequest request); }

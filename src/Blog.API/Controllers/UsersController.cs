using System.Security.Claims;
using Blog.API.Models;
using Blog.Application.DTOs.Users;
using Blog.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Blog.API.Controllers;

[ApiController, Authorize, Route("api/users")]
public class UsersController : ControllerBase
{
    private readonly IUserService _users; public UsersController(IUserService users) => _users = users;
    [HttpGet("profile")] public async Task<ActionResult<ApiResponse<UserProfileResponse>>> GetProfileAsync() => Ok(ApiResponse<UserProfileResponse>.Create(await _users.GetProfileAsync(UserId()), "Profile retrieved."));
    [HttpPut("profile")] public async Task<ActionResult<ApiResponse<UserProfileResponse>>> UpdateProfileAsync(UpdateProfileRequest request) => Ok(ApiResponse<UserProfileResponse>.Create(await _users.UpdateProfileAsync(UserId(), request), "Profile updated."));
    private Guid UserId() => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
}

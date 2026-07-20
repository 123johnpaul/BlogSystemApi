using System.Security.Claims;
using Blog.API.Models;
using Blog.Application.DTOs.Common;
using Blog.Application.DTOs.Posts;
using Blog.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Blog.API.Controllers;

[ApiController, Route("api/posts")]
public class PostsController : ControllerBase
{
    private readonly IPostService _posts; public PostsController(IPostService posts) => _posts = posts;
    [Authorize, HttpPost] public async Task<ActionResult<ApiResponse<PostResponse>>> CreateAsync(CreatePostRequest request) => StatusCode(201, ApiResponse<PostResponse>.Create(await _posts.CreateAsync(UserId(), request), "Post created."));
    [HttpGet] public async Task<ActionResult<ApiResponse<PagedResponse<PostResponse>>>> GetAsync([FromQuery] int page = 1, [FromQuery] int pageSize = 10) => Ok(ApiResponse<PagedResponse<PostResponse>>.Create(await _posts.GetPublishedAsync(page, pageSize), "Posts retrieved."));
    [HttpGet("search")] public async Task<ActionResult<ApiResponse<PagedResponse<PostResponse>>>> SearchAsync([FromQuery] string query, [FromQuery] int page = 1, [FromQuery] int pageSize = 10) => Ok(ApiResponse<PagedResponse<PostResponse>>.Create(await _posts.SearchAsync(query, page, pageSize), "Posts retrieved."));
    [HttpGet("{id:guid}")] public async Task<ActionResult<ApiResponse<PostResponse>>> GetByIdAsync(Guid id) => Ok(ApiResponse<PostResponse>.Create(await _posts.GetByIdAsync(id, TryUserId()), "Post retrieved."));
    [Authorize, HttpPut("{id:guid}")] public async Task<ActionResult<ApiResponse<PostResponse>>> UpdateAsync(Guid id, UpdatePostRequest request) => Ok(ApiResponse<PostResponse>.Create(await _posts.UpdateAsync(id, UserId(), IsAdmin(), request), "Post updated."));
    [Authorize, HttpDelete("{id:guid}")] public async Task<IActionResult> DeleteAsync(Guid id) { await _posts.DeleteAsync(id, UserId(), IsAdmin()); return NoContent(); }
    [Authorize, HttpPatch("{id:guid}/publish")] public async Task<ActionResult<ApiResponse<PostResponse>>> PublishAsync(Guid id) => Ok(ApiResponse<PostResponse>.Create(await _posts.PublishAsync(id, UserId(), IsAdmin()), "Post published."));
    [Authorize, HttpPatch("{id:guid}/lock-comments")] public async Task<ActionResult<ApiResponse<PostResponse>>> LockAsync(Guid id) => Ok(ApiResponse<PostResponse>.Create(await _posts.SetCommentsLockedAsync(id, UserId(), IsAdmin(), true), "Comments locked."));
    [Authorize, HttpPatch("{id:guid}/unlock-comments")] public async Task<ActionResult<ApiResponse<PostResponse>>> UnlockAsync(Guid id) => Ok(ApiResponse<PostResponse>.Create(await _posts.SetCommentsLockedAsync(id, UserId(), IsAdmin(), false), "Comments unlocked."));
    private Guid UserId() => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!); private Guid? TryUserId() => Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var id) ? id : null; private bool IsAdmin() => User.IsInRole("Administrator");
}

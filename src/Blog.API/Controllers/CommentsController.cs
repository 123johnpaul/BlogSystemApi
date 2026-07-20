using System.Security.Claims;
using Blog.API.Models;
using Blog.Application.DTOs.Comments;
using Blog.Application.DTOs.Common;
using Blog.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Blog.API.Controllers;

[ApiController]
public class CommentsController : ControllerBase
{
    private readonly ICommentService _comments; public CommentsController(ICommentService comments) => _comments = comments;
    [Authorize, HttpPost("api/posts/{postId:guid}/comments")] public async Task<ActionResult<ApiResponse<CommentResponse>>> CreateAsync(Guid postId, CreateCommentRequest request) => StatusCode(201, ApiResponse<CommentResponse>.Create(await _comments.CreateAsync(postId, UserId(), request), "Comment created."));
    [Authorize, HttpPost("api/comments/{commentId:guid}/reply")] public async Task<ActionResult<ApiResponse<CommentResponse>>> ReplyAsync(Guid commentId, ReplyCommentRequest request) => StatusCode(201, ApiResponse<CommentResponse>.Create(await _comments.ReplyAsync(commentId, UserId(), request), "Reply created."));
    [HttpGet("api/posts/{postId:guid}/comments")] public async Task<ActionResult<ApiResponse<PagedResponse<CommentResponse>>>> GetAsync(Guid postId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10) => Ok(ApiResponse<PagedResponse<CommentResponse>>.Create(await _comments.GetByPostAsync(postId, page, pageSize), "Comments retrieved."));
    [Authorize, HttpDelete("api/comments/{commentId:guid}")] public async Task<IActionResult> DeleteAsync(Guid commentId) { await _comments.DeleteAsync(commentId, UserId(), User.IsInRole("Administrator")); return NoContent(); }
    private Guid UserId() => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
}

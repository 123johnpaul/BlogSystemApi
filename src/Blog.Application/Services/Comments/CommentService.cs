using Blog.Application.DTOs.Comments;
using Blog.Application.DTOs.Common;
using Blog.Application.Interfaces.Repositories;
using Blog.Application.Interfaces.Services;
using Blog.Domain.Entities;
using Blog.Domain.Enums;

namespace Blog.Application.Services.Comments;

public class CommentService : ICommentService
{
    private readonly ICommentRepository _comments; private readonly IPostRepository _posts;
    public CommentService(ICommentRepository comments, IPostRepository posts) { _comments = comments; _posts = posts; }
    public async Task<CommentResponse> CreateAsync(Guid postId, Guid userId, CreateCommentRequest request) { var post = await GetCommentablePostAsync(postId); var comment = new Comment { Id = Guid.NewGuid(), PostId = post.Id, UserId = userId, Content = request.Content.Trim(), CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow }; await _comments.AddAsync(comment); await _comments.SaveChangesAsync(); return Map(comment); }
    public async Task<CommentResponse> ReplyAsync(Guid commentId, Guid userId, ReplyCommentRequest request) { var parent = await _comments.GetByIdAsync(commentId) ?? throw new InvalidOperationException("Comment not found."); if (parent.ParentCommentId is not null) throw new InvalidOperationException("Replies may only be one level deep."); await GetCommentablePostAsync(parent.PostId); var comment = new Comment { Id = Guid.NewGuid(), PostId = parent.PostId, UserId = userId, ParentCommentId = parent.Id, Content = request.Content.Trim(), CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow }; await _comments.AddAsync(comment); await _comments.SaveChangesAsync(); return Map(comment); }
    public async Task<PagedResponse<CommentResponse>> GetByPostAsync(Guid postId, int page, int pageSize) { var list = (await _comments.GetByPostIdAsync(postId)).ToList(); page = Math.Max(page, 1); pageSize = Math.Clamp(pageSize, 1, 100); return new PagedResponse<CommentResponse> { Items = list.Skip((page - 1) * pageSize).Take(pageSize).Select(Map).ToList(), Page = page, PageSize = pageSize, TotalCount = list.Count }; }
    public async Task DeleteAsync(Guid commentId, Guid userId, bool admin) { var comment = await _comments.GetByIdAsync(commentId) ?? throw new InvalidOperationException("Comment not found."); if (!admin && comment.UserId != userId) throw new UnauthorizedAccessException("You do not own this comment."); await _comments.DeleteAsync(comment); await _comments.SaveChangesAsync(); }
    private async Task<Post> GetCommentablePostAsync(Guid postId) { var post = await _posts.GetByIdAsync(postId) ?? throw new InvalidOperationException("Post not found."); if (post.Status != PostStatus.Published) throw new InvalidOperationException("Comments can only be added to published posts."); if (post.CommentsLocked) throw new InvalidOperationException("Comments are locked for this post."); return post; }
    private static CommentResponse Map(Comment comment) => new() { Id = comment.Id, PostId = comment.PostId, UserId = comment.UserId, AuthorName = comment.User is null ? string.Empty : $"{comment.User.FirstName} {comment.User.Surname}", Content = comment.Content, ParentCommentId = comment.ParentCommentId, CreatedAt = comment.CreatedAt, Replies = comment.Replies.Select(Map).ToList() };
}

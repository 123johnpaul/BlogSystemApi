using Blog.Application.DTOs.Comments;
using Blog.Application.DTOs.Common;
namespace Blog.Application.Interfaces.Services;
public interface ICommentService { Task<CommentResponse> CreateAsync(Guid postId, Guid userId, CreateCommentRequest request); Task<CommentResponse> ReplyAsync(Guid commentId, Guid userId, ReplyCommentRequest request); Task<PagedResponse<CommentResponse>> GetByPostAsync(Guid postId, int page, int pageSize); Task DeleteAsync(Guid commentId, Guid userId, bool isAdministrator); }

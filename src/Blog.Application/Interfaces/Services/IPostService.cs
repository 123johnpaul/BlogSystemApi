using Blog.Application.DTOs.Common;
using Blog.Application.DTOs.Posts;
namespace Blog.Application.Interfaces.Services;
public interface IPostService { Task<PostResponse> CreateAsync(Guid userId, CreatePostRequest request); Task<PagedResponse<PostResponse>> GetPublishedAsync(int page, int pageSize); Task<PostResponse> GetByIdAsync(Guid id, Guid? userId); Task<PostResponse> UpdateAsync(Guid id, Guid userId, bool isAdministrator, UpdatePostRequest request); Task DeleteAsync(Guid id, Guid userId, bool isAdministrator); Task<PostResponse> PublishAsync(Guid id, Guid userId, bool isAdministrator); Task<PostResponse> SetCommentsLockedAsync(Guid id, Guid userId, bool isAdministrator, bool locked); Task<PagedResponse<PostResponse>> SearchAsync(string query, int page, int pageSize); }

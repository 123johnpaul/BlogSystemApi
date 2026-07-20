using Blog.Application.DTOs.Common;
using Blog.Application.DTOs.Posts;
using Blog.Application.Interfaces.Repositories;
using Blog.Application.Interfaces.Services;
using Blog.Domain.Entities;
using Blog.Domain.Enums;

namespace Blog.Application.Services.Posts;

public class PostService : IPostService
{
    private readonly IPostRepository _posts;
    public PostService(IPostRepository posts) => _posts = posts;
    public async Task<PostResponse> CreateAsync(Guid userId, CreatePostRequest request)
    {
        var post = new Post { Id = Guid.NewGuid(), AuthorId = userId, Title = request.Title.Trim(), Summary = request.Summary?.Trim() ?? string.Empty, Content = request.Content.Trim(), Status = PostStatus.Draft, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow };
        await _posts.AddAsync(post); await _posts.SaveChangesAsync();
        return Map(post);
    }
    public async Task<PagedResponse<PostResponse>> GetPublishedAsync(int page, int pageSize) => Page((await _posts.GetAllAsync()).Where(x => x.Status == PostStatus.Published), page, pageSize);
    public async Task<PostResponse> GetByIdAsync(Guid id, Guid? userId)
    {
        var post = await GetPostAsync(id);
        if (post.Status != PostStatus.Published && userId != post.AuthorId) throw new InvalidOperationException("Post not found.");
        return Map(post);
    }
    public async Task<PostResponse> UpdateAsync(Guid id, Guid userId, bool isAdministrator, UpdatePostRequest request)
    {
        var post = await GetOwnedAsync(id, userId, isAdministrator); post.Title = request.Title.Trim(); post.Summary = request.Summary?.Trim() ?? string.Empty; post.Content = request.Content.Trim(); post.UpdatedAt = DateTime.UtcNow;
        await _posts.UpdateAsync(post); await _posts.SaveChangesAsync(); return Map(post);
    }
    public async Task DeleteAsync(Guid id, Guid userId, bool isAdministrator) { var post = await GetOwnedAsync(id, userId, isAdministrator); await _posts.DeleteAsync(post); await _posts.SaveChangesAsync(); }
    public async Task<PostResponse> PublishAsync(Guid id, Guid userId, bool isAdministrator) { var post = await GetOwnedAsync(id, userId, isAdministrator); if (post.Status == PostStatus.Published) throw new InvalidOperationException("Post is already published."); post.Status = PostStatus.Published; post.UpdatedAt = DateTime.UtcNow; await _posts.UpdateAsync(post); await _posts.SaveChangesAsync(); return Map(post); }
    public async Task<PostResponse> SetCommentsLockedAsync(Guid id, Guid userId, bool isAdministrator, bool locked) { var post = await GetOwnedAsync(id, userId, isAdministrator); post.CommentsLocked = locked; post.UpdatedAt = DateTime.UtcNow; await _posts.UpdateAsync(post); await _posts.SaveChangesAsync(); return Map(post); }
    public async Task<PagedResponse<PostResponse>> SearchAsync(string query, int page, int pageSize) => Page(await _posts.SearchAsync(query.Trim()), page, pageSize);
    private async Task<Post> GetPostAsync(Guid id) => await _posts.GetByIdAsync(id) ?? throw new InvalidOperationException("Post not found.");
    private async Task<Post> GetOwnedAsync(Guid id, Guid userId, bool admin) { var post = await GetPostAsync(id); if (!admin && post.AuthorId != userId) throw new UnauthorizedAccessException("You do not own this post."); return post; }
    private static PagedResponse<PostResponse> Page(IEnumerable<Post> posts, int page, int pageSize) { page = Math.Max(1, page); pageSize = Math.Clamp(pageSize, 1, 100); var list = posts.ToList(); return new PagedResponse<PostResponse> { Items = list.Skip((page - 1) * pageSize).Take(pageSize).Select(Map).ToList(), Page = page, PageSize = pageSize, TotalCount = list.Count }; }
    private static PostResponse Map(Post post) => new() { Id = post.Id, Title = post.Title, Summary = post.Summary, Content = post.Content, Status = post.Status.ToString(), CommentsLocked = post.CommentsLocked, AuthorId = post.AuthorId, AuthorName = post.Author is null ? string.Empty : $"{post.Author.FirstName} {post.Author.Surname}", CreatedAt = post.CreatedAt, UpdatedAt = post.UpdatedAt };
}

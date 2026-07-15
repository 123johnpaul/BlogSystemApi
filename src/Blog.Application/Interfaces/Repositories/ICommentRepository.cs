using Blog.Domain.Entities;

namespace Blog.Application.Interfaces.Repositories;

public interface ICommentRepository
{
    Task<Comment?> GetByIdAsync(Guid id);

    Task<IEnumerable<Comment>> GetByPostIdAsync(Guid postId);

    Task AddAsync(Comment comment);

    Task DeleteAsync(Comment comment);

    Task SaveChangesAsync();
}
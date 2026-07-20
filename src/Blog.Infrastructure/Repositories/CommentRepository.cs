using Blog.Application.Interfaces.Repositories;
using Blog.Domain.Entities;
using Blog.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Blog.Infrastructure.Repositories;

public class CommentRepository : ICommentRepository
{
    private readonly ApplicationDbContext _context;
    public CommentRepository(ApplicationDbContext context) => _context = context;
    public Task<Comment?> GetByIdAsync(Guid id) => _context.Comments.Include(x => x.User).Include(x => x.Replies).ThenInclude(x => x.User).FirstOrDefaultAsync(x => x.Id == id);
    public async Task<IEnumerable<Comment>> GetByPostIdAsync(Guid postId) => await _context.Comments.Include(x => x.User).Include(x => x.Replies).ThenInclude(x => x.User).Where(x => x.PostId == postId && x.ParentCommentId == null).OrderBy(x => x.CreatedAt).ToListAsync();
    public Task AddAsync(Comment comment) => _context.Comments.AddAsync(comment).AsTask();
    public Task DeleteAsync(Comment comment) { _context.Comments.Remove(comment); return Task.CompletedTask; }
    public Task SaveChangesAsync() => _context.SaveChangesAsync();
}

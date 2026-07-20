using Blog.Application.Interfaces.Repositories;
using Blog.Domain.Entities;
using Blog.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Blog.Infrastructure.Repositories;

public class PostRepository : IPostRepository
{
    private readonly ApplicationDbContext _context;
    public PostRepository(ApplicationDbContext context) => _context = context;
    public Task<Post?> GetByIdAsync(Guid id) => _context.Posts.Include(x => x.Author).FirstOrDefaultAsync(x => x.Id == id);
    public async Task<IEnumerable<Post>> GetAllAsync() => await _context.Posts.Include(x => x.Author).OrderByDescending(x => x.CreatedAt).ToListAsync();
    public async Task<IEnumerable<Post>> SearchAsync(string keyword) => await _context.Posts.Include(x => x.Author).Where(x => EF.Functions.ILike(x.Title, $"%{keyword}%") || EF.Functions.ILike(x.Summary, $"%{keyword}%") || EF.Functions.ILike(x.Content, $"%{keyword}%")).OrderByDescending(x => x.CreatedAt).ToListAsync();
    public Task AddAsync(Post post) => _context.Posts.AddAsync(post).AsTask();
    public Task UpdateAsync(Post post) { _context.Posts.Update(post); return Task.CompletedTask; }
    public Task DeleteAsync(Post post) { _context.Posts.Remove(post); return Task.CompletedTask; }
    public Task SaveChangesAsync() => _context.SaveChangesAsync();
}

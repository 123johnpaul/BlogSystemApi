using Blog.Domain.Enums;

namespace Blog.Domain.Entities;

public class Post
{
    public Guid Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Summary { get; set; } = string.Empty;

    public string Content { get; set; } = string.Empty;

    public PostStatus Status { get; set; } = PostStatus.Draft;

    public bool CommentsLocked { get; set; } = false;

    public Guid AuthorId { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public User Author { get; set; } = null!;

    public ICollection<Comment> Comments { get; set; } = new List<Comment>();
}
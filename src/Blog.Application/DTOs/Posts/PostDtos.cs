namespace Blog.Application.DTOs.Posts;

public class CreatePostRequest { public string Title { get; set; } = string.Empty; public string? Summary { get; set; } public string Content { get; set; } = string.Empty; }
public class UpdatePostRequest { public string Title { get; set; } = string.Empty; public string? Summary { get; set; } public string Content { get; set; } = string.Empty; }
public class PostResponse { public Guid Id { get; init; } public string Title { get; init; } = string.Empty; public string Summary { get; init; } = string.Empty; public string Content { get; init; } = string.Empty; public string Status { get; init; } = string.Empty; public bool CommentsLocked { get; init; } public Guid AuthorId { get; init; } public string AuthorName { get; init; } = string.Empty; public DateTime CreatedAt { get; init; } public DateTime UpdatedAt { get; init; } }

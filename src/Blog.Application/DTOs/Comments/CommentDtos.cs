namespace Blog.Application.DTOs.Comments;

public class CreateCommentRequest { public string Content { get; set; } = string.Empty; }
public class ReplyCommentRequest { public string Content { get; set; } = string.Empty; }
public class CommentResponse { public Guid Id { get; init; } public Guid PostId { get; init; } public Guid UserId { get; init; } public string AuthorName { get; init; } = string.Empty; public string Content { get; init; } = string.Empty; public Guid? ParentCommentId { get; init; } public DateTime CreatedAt { get; init; } public IReadOnlyCollection<CommentResponse> Replies { get; init; } = Array.Empty<CommentResponse>(); }

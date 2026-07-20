using Blog.Application.DTOs.Comments;
using Blog.Application.DTOs.Posts;
using Blog.Application.DTOs.Users;
using FluentValidation;

namespace Blog.Application.Validators;

public class UpdateProfileRequestValidator : AbstractValidator<UpdateProfileRequest> { public UpdateProfileRequestValidator() { RuleFor(x => x.FirstName).NotEmpty().MaximumLength(100); RuleFor(x => x.Surname).NotEmpty().MaximumLength(100); RuleFor(x => x.Bio).MaximumLength(1000); RuleFor(x => x.Avatar).MaximumLength(2048).When(x => x.Avatar is not null); } }
public class CreatePostRequestValidator : AbstractValidator<CreatePostRequest> { public CreatePostRequestValidator() { RuleFor(x => x.Title).NotEmpty().MaximumLength(250); RuleFor(x => x.Content).NotEmpty().MaximumLength(50000); RuleFor(x => x.Summary).MaximumLength(1000); } }
public class UpdatePostRequestValidator : AbstractValidator<UpdatePostRequest> { public UpdatePostRequestValidator() { RuleFor(x => x.Title).NotEmpty().MaximumLength(250); RuleFor(x => x.Content).NotEmpty().MaximumLength(50000); RuleFor(x => x.Summary).MaximumLength(1000); } }
public class CreateCommentRequestValidator : AbstractValidator<CreateCommentRequest> { public CreateCommentRequestValidator() { RuleFor(x => x.Content).NotEmpty().MaximumLength(5000); } }
public class ReplyCommentRequestValidator : AbstractValidator<ReplyCommentRequest> { public ReplyCommentRequestValidator() { RuleFor(x => x.Content).NotEmpty().MaximumLength(5000); } }

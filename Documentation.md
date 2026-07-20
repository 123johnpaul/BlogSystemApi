# PROJECT COMPLETION SPECIFICATION
## Blogging System API
### Version 1.0

---

# 1. Introduction

## 1.1 Purpose

This document serves as the definitive implementation specification for completing the Blogging System API.

The project has already progressed beyond the planning phase. The domain model, persistence layer, solution architecture, and portions of the authentication module already exist. The remaining work consists primarily of implementing unfinished services, controllers, infrastructure configuration, testing, and production readiness.

This specification is intended to allow another engineer (human or AI) to continue development without redesigning the project.

The existing architecture must be preserved.

No architectural redesign should be introduced.

---

# 2. Project Objectives

The finished application shall provide a production-quality REST API supporting the following functionality.

Authentication

- User Registration
- Email Verification
- Login
- JWT Authentication
- Refresh Tokens
- Forgot Password
- Reset Password
- Change Password

User Management

- View Profile
- Update Profile
- Upload Avatar

Posts

- Create Post
- Update Post
- Delete Post
- Publish Draft
- Search Posts
- View Posts
- Lock Comments
- Unlock Comments

Comments

- Add Comment
- Reply
- View Comments
- Delete Comment

Infrastructure

- Swagger
- PostgreSQL
- EF Core
- FluentValidation
- Serilog
- OpenTelemetry
- SMTP Email
- Global Exception Handling

Testing

- Unit Tests
- Integration Tests
- Postman Collection

---

# 3. Existing Architecture

The project follows Clean Architecture.

Dependencies are one directional only.

```
API
↓

Application
↓

Domain

Infrastructure
```

Infrastructure depends on Application.

Application depends on Domain.

API depends on Application and Infrastructure.

Domain has no dependency on any other project.

This dependency flow must never be violated.

---

# 4. Current Project State

The project already contains the majority of its foundational code.

The implementation should continue from the current state rather than replacing existing work.

Current solution:

```
Blog.API

Blog.Application

Blog.Domain

Blog.Infrastructure

Blog.UnitTests

Blog.IntegrationTests
```

The overall project is approximately **65–75% structurally complete**.

Most remaining work is implementation rather than architecture.

---

# 5. Current Folder Analysis

## Blog.Domain

Status:
Approximately 95% complete.

Contains:

Entities

- User
- Post
- Comment
- RefreshToken
- Otp

Enums

- UserRole
- PostStatus
- OtpPurpose

Entity relationships already exist.

No additional entities should be introduced unless absolutely necessary.

Future modifications should only involve adding navigation properties when required.

---

## Blog.Infrastructure

Status:
Approximately 80% complete.

Already implemented:

ApplicationDbContext

Entity Configurations

Repositories

Password Hashing

JWT Generation

Refresh Token Generation

OTP Service

Email Service

Dependency Injection

Database Migrations

Infrastructure should remain responsible for:

Database

Email

Security

External services

Persistence

Infrastructure must never contain business logic.

---

## Blog.Application

Status:
Approximately 60% complete.

Contains

DTOs

Validators

Interfaces

Authentication folder

Post folder

Comment folder

User folder

Several folders currently contain placeholders only.

Business logic belongs here.

Controllers should never contain business logic.

---

## Blog.API

Status:
Approximately 30% complete.

Contains

Configuration

Middleware

Filters

Program.cs

Controllers folder

The majority of controllers remain to be implemented.

Program.cs requires additional middleware registration.

Authentication configuration is incomplete.

Swagger configuration requires expansion.

---

## Tests

Folders already exist.

Implementation is mostly empty.

Testing should occur after every completed feature rather than after the entire project is finished.

---

# 6. Engineering Principles

The following rules are mandatory.

## Rule 1

Controllers must remain thin.

Controllers should only:

Receive request

Validate model

Call service

Return response

No business logic.

---

## Rule 2

Services contain business logic.

Every workflow belongs inside a service.

Example

Registration

Login

Password Reset

Create Post

Publish Post

Delete Comment

Update Profile

---

## Rule 3

Repositories perform data access only.

Repositories never contain business rules.

Repositories never return DTOs.

Repositories return entities.

---

## Rule 4

DTOs are communication models.

Entities must never be exposed directly from controllers.

Every request uses DTOs.

Every response uses DTOs.

---

## Rule 5

Validation uses FluentValidation.

Controllers should never manually validate.

Business services should assume validated input.

---

## Rule 6

Every asynchronous database operation must use async/await.

Never use synchronous EF Core methods.

---

## Rule 7

Dependency Injection

Every service

Every repository

Every infrastructure component

must be registered through DependencyInjection.cs.

Program.cs should never manually instantiate services.

---

## Rule 8

Never duplicate business logic.

If functionality already exists inside another service, reuse it.

---

## Rule 9

Exceptions should be handled globally.

Business services throw exceptions.

Controllers never catch them.

Exception middleware translates them into HTTP responses.

---

## Rule 10

Every endpoint must be documented by Swagger automatically.

No undocumented endpoints.

---

# 7. Coding Standards

Namespaces

Follow project namespace.

One class per file.

One interface per file.

No nested classes.

Constructor injection only.

Nullable reference types enabled.

Implicit usings enabled.

Avoid static classes except utility classes.

Method names

Get...

Create...

Update...

Delete...

Verify...

Generate...

Send...

Never abbreviate names unnecessarily.

Bad

UpdUsr()

Good

UpdateUserProfileAsync()

Async suffix mandatory for asynchronous methods.

---

# 8. Dependency Order

The implementation order is critical.

Later services depend upon earlier services.

Correct order:

Authentication

↓

Users

↓

Posts

↓

Comments

↓

Testing

↓

Swagger

↓

Production

Changing this order creates unnecessary dependency issues.

---

END OF PART 1

# PART 2
# AUTHENTICATION MODULE IMPLEMENTATION SPECIFICATION

---

# 9. Authentication Module

## 9.1 Purpose

The Authentication Module is responsible for establishing and maintaining user identity within the Blogging System API.

It performs the following responsibilities:

- User Registration
- Email Verification
- User Login
- JWT Generation
- Refresh Token Management
- Password Recovery
- Password Reset
- Password Change
- Logout
- Authentication Validation

This module serves as the entry point into the application.

Every protected endpoint in the application ultimately depends upon this module functioning correctly.

---

# 10. Existing Authentication Status

The project already contains a significant portion of the authentication infrastructure.

## Completed Components

### Domain

✔ User Entity

✔ RefreshToken Entity

✔ OTP Entity

✔ OtpPurpose Enum

---

### Infrastructure

✔ PasswordHasher

✔ JwtService

✔ TokenGenerator

✔ EmailService

✔ OtpService

✔ UserRepository

✔ OtpRepository

✔ RefreshTokenRepository

---

### Application

✔ RegisterRequest

✔ RegisterResponse

✔ LoginRequest

✔ LoginResponse

✔ VerifyEmailRequest

✔ ForgotPasswordRequest

✔ ResetPasswordRequest

✔ ChangePasswordRequest

✔ Validators

✔ RegistrationService

✔ VerificationService

✔ LoginService (partial)

✔ AuthenticationService (partial)

---

### Persistence

✔ EF Core

✔ PostgreSQL

✔ RefreshToken Table

✔ OTP Table

✔ User Table

---

### Remaining Work

The following functionality remains incomplete.

Authentication Controller

Password Service

Refresh Token Endpoint

Logout Endpoint

Forgot Password Workflow

Reset Password Workflow

Change Password Workflow

JWT Configuration

Authorization Policies

Authentication Middleware Registration

Swagger Authentication Support

Unit Tests

Integration Tests

---

# 11. Authentication Design Principles

Authentication is designed around JWT Access Tokens and Refresh Tokens.

Access Tokens are short-lived.

Refresh Tokens are long-lived.

Passwords are never stored in plain text.

Passwords are always hashed using BCrypt.

OTP codes are temporary.

Email verification is mandatory before login.

Refresh Tokens are persisted in the database.

Access Tokens are not persisted.

---

# 12. Authentication Workflow

The authentication lifecycle follows this order.

```

Register

↓

Generate OTP

↓

Send Verification Email

↓

Verify Email

↓

Enable Account

↓

Login

↓

Generate JWT

↓

Generate Refresh Token

↓

Store Refresh Token

↓

Return Tokens

↓

Access Protected APIs

↓

Refresh Access Token

↓

Logout

```

Every step depends upon the previous one.

---

# 13. Registration Specification

## Objective

Create a new user account.

---

## Preconditions

Email must not already exist.

Password must satisfy policy.

Validation must succeed.

---

## Workflow

Receive RegisterRequest

↓

Validate DTO

↓

Check existing email

↓

Hash password

↓

Create User

↓

Persist User

↓

Generate OTP

↓

Persist OTP

↓

Send Email

↓

Return Success

---

## Repository Calls

UserRepository

ExistsByEmailAsync()

AddAsync()

SaveChangesAsync()

OTPRepository

AddAsync()

SaveChangesAsync()

---

## Security Requirements

Password never logged.

Password never returned.

Password always hashed.

Hashing algorithm:

BCrypt

---

## Response

Registration succeeds even though account is not yet verified.

User cannot login until verification completes.

---

# 14. Email Verification Specification

## Objective

Activate user account.

---

## Preconditions

OTP exists.

OTP not expired.

OTP unused.

User exists.

---

## Workflow

Receive VerifyEmailRequest

↓

Find OTP

↓

Validate Expiration

↓

Validate Purpose

↓

Validate User

↓

Mark OTP Used

↓

Update User.IsVerified

↓

Save Changes

↓

Return Success

---

## Error Conditions

OTP Not Found

Expired OTP

Already Used OTP

User Missing

Already Verified

Each condition should produce a meaningful domain exception.

---

# 15. Login Specification

## Objective

Authenticate an existing verified user.

---

## Preconditions

Email exists.

Password correct.

Account verified.

Account active.

---

## Workflow

Receive LoginRequest

↓

Validate DTO

↓

Retrieve User

↓

Verify Password

↓

Verify Email Confirmed

↓

Generate JWT

↓

Generate Refresh Token

↓

Persist Refresh Token

↓

Return LoginResponse

---

## Repository Usage

UserRepository

GetByEmailAsync()

RefreshTokenRepository

AddAsync()

SaveChangesAsync()

---

## LoginResponse

Contains

AccessToken

RefreshToken

Expiration

No additional sensitive information.

---

# 16. JWT Specification

JWT generation already exists.

The remaining work is configuration.

---

## Claims

Every JWT must contain

UserId

Email

Role

JWT ID

Issued Time

Expiration Time

Issuer

Audience

---

## Lifetime

Access Token

15–30 minutes.

Refresh Token

7–30 days.

Lifetime should come from configuration.

Never hardcode.

---

## Configuration

JWT settings belong in

appsettings.json

Never embed secrets inside source code.

Configuration should include

Issuer

Audience

Secret Key

Expiration Minutes

Refresh Token Days

---

# 17. Refresh Token Specification

Refresh Tokens provide seamless authentication without requiring another login.

---

## Workflow

Receive Refresh Token

↓

Find Token

↓

Check Exists

↓

Check Expiration

↓

Check Revoked

↓

Retrieve User

↓

Generate New JWT

↓

Generate New Refresh Token

↓

Revoke Old Token

↓

Persist New Token

↓

Return Tokens

---

## Refresh Token Rotation

Old Refresh Token

↓

Revoked

↓

New Refresh Token

↓

Persisted

↓

Returned

Never reuse refresh tokens.

Rotation is mandatory.

---

## Database Updates

Old Token

IsRevoked = true

RevokedAt = UTC Now

Create

New RefreshToken

Save

Return

---

# 18. Logout Specification

Logout invalidates refresh tokens.

JWTs cannot be revoked because they are stateless.

Therefore logout revokes all active refresh tokens.

---

Workflow

Receive Request

↓

Identify User

↓

Retrieve Refresh Tokens

↓

Revoke Tokens

↓

Save

↓

204 No Content

---

# 19. Forgot Password Specification

## Objective

Allow users to request password reset.

---

Workflow

Receive Email

↓

Find User

↓

Generate Password Reset OTP

↓

Persist OTP

↓

Send Email

↓

Return Success

Never reveal whether email exists.

Always return identical response.

---

# 20. Reset Password Specification

Workflow

Receive

Email

OTP

New Password

↓

Validate

↓

Retrieve OTP

↓

Validate Purpose

↓

Validate Expiration

↓

Retrieve User

↓

Hash Password

↓

Update Password

↓

Invalidate OTP

↓

Invalidate Refresh Tokens

↓

Save

↓

Return Success

---

# 21. Change Password Specification

Requires authentication.

Workflow

Receive

Current Password

New Password

↓

Retrieve User

↓

Verify Current Password

↓

Hash New Password

↓

Update Password

↓

Invalidate Refresh Tokens

↓

Save

↓

Return Success

---

# 22. Password Rules

Minimum Length

8

Contains Uppercase

Required

Contains Lowercase

Required

Contains Number

Required

Special Characters

Recommended

Maximum Length

128

Passwords must never be reversible.

---

# 23. Authorization

Role-based authorization should use JWT claims.

Roles currently include

Admin

Author

User

Future roles should not require controller modifications.

Policies should be configurable.

---

# 24. Auth Controller Specification

The controller should expose the following endpoints.

POST

/api/auth/register

POST

/api/auth/verify-email

POST

/api/auth/login

POST

/api/auth/refresh

POST

/api/auth/logout

POST

/api/auth/forgot-password

POST

/api/auth/reset-password

PUT

/api/auth/change-password

No business logic should exist inside this controller.

Every endpoint delegates immediately to an Application Service.

---

# 25. Error Handling

Authentication errors should produce consistent responses.

Examples include

400 Validation Failure

401 Invalid Credentials

401 Invalid Token

401 Expired Token

403 Email Not Verified

404 Resource Not Found

409 Email Already Exists

500 Internal Error

Responses should use the project's standardized API response model.

---

# 26. Authentication Logging

Log

Registration

Verification

Login Success

Login Failure

Password Reset Request

Password Reset Success

Password Change

Logout

Never log

Passwords

JWTs

Refresh Tokens

OTP Codes

Connection Strings

Secrets

---

# 27. Authentication Testing Requirements

## Unit Tests

RegistrationService

VerificationService

LoginService

PasswordService

AuthenticationService

JwtService

PasswordHasher

OTP Service

Refresh Token Service

Mock repositories.

Mock email service.

Mock clock if required.

---

## Integration Tests

Register

Verify

Login

Refresh

Forgot Password

Reset Password

Logout

Change Password

Run against a real PostgreSQL test database.

No mocked HTTP pipeline.

---

# 28. Authentication Definition of Done

Authentication is complete only when all conditions are satisfied.

□ Registration works

□ Duplicate emails rejected

□ Email verification works

□ Login succeeds

□ Invalid login rejected

□ JWT generated

□ Refresh Token generated

□ Refresh endpoint rotates tokens

□ Logout revokes tokens

□ Forgot Password works

□ Reset Password works

□ Change Password works

□ Password hashing verified

□ JWT middleware configured

□ Swagger authentication configured

□ Unit tests passing

□ Integration tests passing

□ No controller contains business logic

---

END OF PART 2

# PART 3
# USER MANAGEMENT AND POST MANAGEMENT IMPLEMENTATION SPECIFICATION

---

# 29. User Management Module

## 29.1 Purpose

The User Management Module is responsible for maintaining user account information after authentication.

Unlike the Authentication Module, which establishes identity, the User Module manages user data throughout the lifetime of the account.

Responsibilities include:

- Retrieve user profile
- Update profile
- View public profile
- Upload avatar (future)
- Account settings
- Author statistics
- User metadata

This module should never perform authentication.

Authentication should already have occurred before any endpoint within this module executes.

---

# 30. User Module Architecture

The User Module should follow the same architectural principles as every other module.

```

Controller

↓

Application Service

↓

Repository

↓

EF Core

↓

Database

```

No shortcuts should bypass the Application Layer.

---

# 31. Current Status

The project already contains:

User Entity

User DTO folder

GetProfile folder

UpdateProfile folder

Repository Interface

Repository Implementation

The remaining work consists primarily of implementing service logic, controller endpoints, DTO mappings, and tests.

---

# 32. User Profile Retrieval

## Objective

Allow authenticated users to retrieve their own profile.

---

Workflow

Receive Request

↓

Extract UserId from JWT

↓

Retrieve User

↓

Map Entity

↓

Return DTO

---

Repository

```
GetByIdAsync()
```

---

Response DTO

Should include

Id

First Name

Last Name

Email

Bio

Avatar URL

Role

Created Date

Updated Date

Never include

PasswordHash

Refresh Tokens

OTP History

Internal IDs

---

Authorization

Authenticated users only.

A user may retrieve only their own profile unless future administrative endpoints are introduced.

---

# 33. Public Profile

Future enhancement.

Public profile endpoint should expose

Display Name

Bio

Avatar

Number of Posts

Join Date

It must never expose

Email

Password

Refresh Tokens

Internal metadata

---

# 34. Update Profile

## Objective

Allow authenticated users to update profile information.

---

Editable Fields

First Name

Last Name

Bio

Avatar URL (temporary)

Future

Profile Picture Upload

---

Workflow

Receive DTO

↓

Validate

↓

Retrieve User

↓

Apply Changes

↓

Update Timestamp

↓

Save

↓

Return Updated DTO

---

Validation Rules

Names cannot be empty.

Bio maximum length should be configurable.

Email cannot be changed through this endpoint.

Role cannot be modified.

Password cannot be modified.

---

Repository

```
UpdateAsync()

SaveChangesAsync()
```

---

# 35. Avatar Upload

Not currently implemented.

The current Avatar field should accept a URL.

Future versions may support

Cloudinary

Azure Blob

AWS S3

Local Storage

The API should be designed such that replacing the avatar implementation does not require changes to business logic.

---

# 36. User Controller

Required endpoints

GET

/api/users/profile

PUT

/api/users/profile

Future

GET

/api/users/{id}

GET

/api/users/{username}

POST

/api/users/avatar

---

# 37. User Business Rules

Users cannot edit

Role

Email

Password

Verification Status

Creation Date

Only administrators should eventually be able to modify user roles.

---

# 38. User Module Testing

Unit Tests

Retrieve Profile

Update Profile

Invalid User

Unauthorized User

Validation Failure

---

Integration Tests

GET Profile

PUT Profile

JWT Authorization

Validation

Persistence

---

# 39. User Module Definition of Done

□ Retrieve profile works

□ Update profile works

□ Unauthorized users rejected

□ DTO mapping complete

□ Tests passing

□ Swagger documentation generated

---

# 40. Post Management Module

The Post Module is the primary business feature of the Blogging System.

Every other module ultimately supports this one.

This module manages the lifecycle of blog posts.

Responsibilities include

Create

Read

Update

Delete

Publish

Draft

Search

Pagination

Comment locking

Ownership validation

Status transitions

---

# 41. Current Status

Existing folders

CreatePost

UpdatePost

DeletePost

GetPosts

GetPostById

PublishPost

SearchPosts

LockComments

UnlockComments

Most folders currently contain structure only.

Business logic remains to be implemented.

---

# 42. Post Lifecycle

Every post follows this lifecycle.

```

Draft

↓

Updated

↓

Published

↓

Archived (Future)

↓

Deleted

```

Only Draft posts may be edited freely.

Published posts become visible publicly.

Deleted posts should no longer appear in search.

---

# 43. Create Post

## Objective

Allow authenticated users to create draft posts.

---

Workflow

Receive DTO

↓

Validate

↓

Extract UserId

↓

Create Entity

↓

Assign Author

↓

Set Status

Draft

↓

Persist

↓

Return Created DTO

---

Repository

```
AddAsync()

SaveChangesAsync()
```

---

Required Fields

Title

Content

Optional

Excerpt

Thumbnail

Tags (Future)

Category (Future)

---

Validation

Title required

Content required

Maximum title length

Maximum content length

---

# 44. Get Posts

Objective

Retrieve published posts.

---

Features

Pagination

Sorting

Filtering

Searching

Future category filtering

Future tag filtering

---

Default Sort

Newest First

---

Response

Paged result

Items

Page

Page Size

Total Count

Total Pages

---

# 45. Get Post By Id

Workflow

Receive Id

↓

Retrieve

↓

Exists?

↓

Published?

↓

Map DTO

↓

Return

---

Future Enhancement

Authenticated authors should be able to retrieve drafts.

Anonymous users should retrieve published posts only.

---

# 46. Update Post

Workflow

Receive DTO

↓

Retrieve Post

↓

Ownership Validation

↓

Status Validation

↓

Update Fields

↓

Save

↓

Return Updated DTO

---

Editable Fields

Title

Content

Excerpt

Thumbnail

---

Immutable Fields

Author

CreatedAt

Id

---

# 47. Ownership Rules

Only

Author

or

Administrator

may update a post.

No other user may modify another author's work.

Ownership should be validated within the Application Service.

Never inside repositories.

---

# 48. Delete Post

Decision

Soft Delete recommended.

Reasons

Audit history

Recovery

Analytics

Future moderation

If soft delete is implemented

Deleted posts should be excluded from every query automatically.

---

Workflow

Retrieve

↓

Ownership Validation

↓

Mark Deleted

↓

Save

↓

204 No Content

---

# 49. Publish Post

Posts begin as Draft.

Publishing changes

Status

Draft

↓

Published

Published posts become visible to public endpoints.

Published posts should receive

PublishedAt timestamp.

---

Workflow

Retrieve

↓

Ownership Check

↓

Draft?

↓

Publish

↓

Save

↓

Return Updated DTO

---

# 50. Search Posts

Search should support

Title

Content

Excerpt

Future

Tags

Categories

Author

---

Sorting

Newest

Oldest

Most Relevant (future)

---

Pagination mandatory.

Never return entire database.

---

# 51. Lock Comments

Authors may disable commenting.

Workflow

Retrieve Post

↓

Ownership Validation

↓

CommentsLocked

↓

True

↓

Save

---

Unlock

Same process.

CommentsLocked

↓

False

---

# 52. Post Repository Responsibilities

Repository should provide

Create

Update

Delete

Search

Pagination

Get By Id

Get By Slug (future)

Published Posts

Author Posts

---

Repository must not

Perform validation

Authorize users

Throw business exceptions

Map DTOs

---

# 53. Post DTOs

Separate DTOs required.

CreatePostRequest

UpdatePostRequest

PostResponse

PostSummaryResponse

PagedPostResponse

SearchPostRequest

Avoid reusing request DTOs as response DTOs.

---

# 54. Controller Endpoints

POST

/api/posts

GET

/api/posts

GET

/api/posts/{id}

PUT

/api/posts/{id}

DELETE

/api/posts/{id}

PATCH

/api/posts/{id}/publish

PATCH

/api/posts/{id}/lock-comments

PATCH

/api/posts/{id}/unlock-comments

GET

/api/posts/search

---

# 55. Authorization

Create

Authenticated

Update

Owner

Delete

Owner

Publish

Owner

Lock Comments

Owner

Search

Public

Read Published

Public

---

# 56. Validation

Title

Required

Content

Required

No empty strings

Maximum length validation

Reject whitespace-only values.

---

# 57. Logging

Log

Create

Update

Delete

Publish

Lock Comments

Unlock Comments

Search

Never log

Entire article content

JWT

Sensitive information

---

# 58. Unit Tests

Create Post

Update Post

Delete Post

Publish Post

Search

Lock Comments

Unlock Comments

Ownership Validation

Validation Failure

Repository Failure

---

# 59. Integration Tests

POST Create

GET Posts

GET Post

PUT Update

DELETE

PATCH Publish

PATCH Lock

PATCH Unlock

Search

Pagination

Authorization

---

# 60. Post Module Definition of Done

□ Create works

□ Update works

□ Delete works

□ Publish works

□ Search works

□ Pagination works

□ Lock comments works

□ Unlock comments works

□ Ownership enforced

□ DTO mapping complete

□ Validation complete

□ Swagger complete

□ Unit tests pass

□ Integration tests pass

---

END OF PART 3

# PART 4
# COMMENT MODULE, INFRASTRUCTURE, API DESIGN, TESTING, AND PRODUCTION SPECIFICATION

---

# 61. Comment Management Module

## 61.1 Purpose

The Comment Module provides discussion capabilities for published blog posts.

Unlike the Post Module, comments are always subordinate to a parent post.

A comment cannot exist independently.

Every comment belongs to:

- exactly one post
- exactly one author

A comment may optionally belong to another comment when acting as a reply.

---

# 62. Responsibilities

The Comment Module is responsible for

- Creating comments
- Replying to comments
- Viewing comments
- Deleting comments
- Pagination
- Authorization
- Comment locking enforcement

Future enhancements

- Edit comment
- Like comment
- Report comment
- Nested replies beyond one level
- Thread collapsing
- Moderation queue

---

# 63. Current Project Status

Existing folders include

CreateComment

DeleteComment

ReplyComment

GetComments

Repository interfaces already exist.

Repository implementation remains incomplete.

Business services are partially scaffolded.

Controllers have not been fully implemented.

---

# 64. Comment Entity Rules

Each comment must contain

Id

PostId

AuthorId

Content

CreatedAt

UpdatedAt

Optional ParentCommentId

A comment must always belong to a post.

A reply must always belong to an existing comment.

---

# 65. Comment Lifecycle

```

Create

↓

Visible

↓

Reply (optional)

↓

Delete

```

Comments are immediately visible after creation unless future moderation is introduced.

---

# 66. Create Comment

## Objective

Allow authenticated users to comment on published posts.

---

Workflow

Receive Request

↓

Validate DTO

↓

Retrieve Post

↓

Verify Exists

↓

Verify Published

↓

Verify Comments Not Locked

↓

Create Comment

↓

Persist

↓

Return DTO

---

Repository Calls

GetPostByIdAsync()

AddCommentAsync()

SaveChangesAsync()

---

Validation

Content required

No whitespace-only comments

Maximum comment length

Reject empty body

---

Authorization

Authenticated users only.

Anonymous users cannot comment.

---

# 67. Reply To Comment

Workflow

Receive DTO

↓

Retrieve Parent Comment

↓

Exists?

↓

Retrieve Post

↓

Comments Locked?

↓

Create Reply

↓

Persist

↓

Return DTO

---

Business Rules

Replies must belong to the same post.

Parent comment must exist.

Reply depth should initially be limited to one level unless recursive replies are intentionally supported.

---

# 68. Get Comments

Retrieve comments for a specific post.

Support

Pagination

Sorting

Future nested thread projection

Default sort

Oldest First

---

Response

```
{
    comments,

    page,

    pageSize,

    totalComments,

    totalPages
}
```

---

# 69. Delete Comment

Delete operations should respect authorization.

Allowed users

Comment Author

Administrator

Future Moderator

---

Workflow

Retrieve Comment

↓

Ownership Validation

↓

Delete

↓

Save

↓

204 No Content

---

Soft delete is recommended if moderation history is required.

Otherwise hard delete is acceptable.

---

# 70. Locked Comments

A post owner may disable commenting.

Whenever

CommentsLocked == true

The following operations must fail

Create Comment

Reply Comment

Existing comments remain visible.

Viewing comments is never disabled.

---

# 71. Repository Responsibilities

Provide

Create

Delete

Get By Id

Get By Post

Get Replies

Pagination

Count

Repositories never

Validate ownership

Validate authentication

Throw business exceptions

Map DTOs

---

# 72. Comment DTOs

Required

CreateCommentRequest

ReplyCommentRequest

CommentResponse

CommentSummaryResponse

PagedCommentResponse

---

# 73. Controller Endpoints

POST

/api/posts/{postId}/comments

POST

/api/comments/{commentId}/reply

GET

/api/posts/{postId}/comments

DELETE

/api/comments/{commentId}

---

# 74. Unit Tests

Create Comment

Reply

Delete

Locked Comments

Ownership

Validation

Repository Failure

---

# 75. Integration Tests

Comment Creation

Replies

Retrieval

Deletion

Authorization

Pagination

Comment Lock Enforcement

---

# 76. Comment Module Definition of Done

□ Create works

□ Reply works

□ Delete works

□ Retrieval works

□ Pagination works

□ Authorization enforced

□ Locked comments enforced

□ Swagger complete

□ Tests passing

---

# 77. Infrastructure Specification

Infrastructure already contains

EF Core

Repositories

Email

JWT

Password Hasher

OTP Service

Remaining work focuses on configuration.

Infrastructure should never contain business rules.

---

# 78. Dependency Injection

Every new implementation must be registered.

Examples

AuthenticationService

LoginService

PasswordService

Comment Services

Post Services

User Services

Repositories

Email

Security Services

Failure to register a dependency should result in startup failure rather than runtime discovery.

---

# 79. Configuration

Configuration belongs exclusively inside

appsettings.json

appsettings.Development.json

Environment Variables

Never hardcode

SMTP

JWT Secret

Database

Expiration Times

Connection Strings

API Keys

---

# 80. Swagger

Swagger should become the primary API documentation.

Requirements

JWT Authentication Button

Request Examples

Response Examples

Validation Messages

HTTP Status Codes

XML Documentation (optional)

Swagger should fully describe every endpoint.

---

# 81. Logging

Serilog should log

Startup

Shutdown

Unhandled Exceptions

Authentication Events

CRUD Operations

Validation Failures

Database Errors

Never log

Passwords

OTP

JWT

Refresh Tokens

Secrets

Connection Strings

---

# 82. Telemetry

OpenTelemetry should capture

HTTP Requests

Response Time

Exceptions

Database Calls

Dependency Calls

Request Duration

Future exporters

Jaeger

Grafana

Prometheus

Azure Monitor

---

# 83. Global Exception Middleware

The middleware should translate exceptions into consistent API responses.

Example

ValidationException

↓

400

UnauthorizedException

↓

401

ForbiddenException

↓

403

NotFoundException

↓

404

ConflictException

↓

409

Unhandled Exception

↓

500

All responses should use a common response model.

---

# 84. API Response Standard

Every endpoint should follow one response contract.

Example

```
{
    success,

    message,

    data,

    errors,

    timestamp
}
```

Never return inconsistent JSON structures.

---

# 85. Pagination Standard

All collection endpoints should support

Page Number

Page Size

Default Page

1

Default Size

10

Maximum

100

Never allow unlimited retrieval.

---

# 86. Search Standard

Search endpoints should

Trim whitespace

Ignore case

Support partial matching

Reject invalid parameters

Future

Full-text PostgreSQL search

---

# 87. Validation Strategy

Every request DTO should have a validator.

Controllers should not perform validation.

Services assume validated input.

Validation failures should never become exceptions.

---

# 88. Authorization Strategy

JWT Authentication

Role-based Authorization

Ownership Validation

Future Policy-based Authorization

Ownership belongs inside Application Services.

---

# 89. Testing Strategy

Testing occurs in three layers.

Layer One

Unit Tests

Business Logic

Layer Two

Integration Tests

Database

Authentication

Pipeline

Layer Three

Manual Testing

Swagger

Postman

End-to-end

---

# 90. Unit Testing Requirements

Every Application Service requires tests.

Every Validator requires tests.

Security components require tests.

Repositories should generally be tested through integration testing instead.

---

# 91. Integration Testing Requirements

Run against PostgreSQL.

Avoid InMemory provider for behavioral tests.

Test

Authentication

Users

Posts

Comments

Validation

Authorization

Persistence

---

# 92. Postman Collection

Collection should contain folders

Authentication

Users

Posts

Comments

Health

Every request should include

Sample Body

Expected Response

Authorization Examples

Environment Variables

---

# 93. Performance Goals

Average Response Time

<200ms

Authentication

<300ms

Search

<500ms

Large payload endpoints should implement pagination.

---

# 94. Security Checklist

Passwords Hashed

JWT Signed

Refresh Tokens Stored

OTP Expiration

Authorization Enabled

HTTPS Ready

Secrets Externalized

Validation Enabled

SQL Injection Protected through EF Core

No Sensitive Logging

---

# 95. Production Readiness Checklist

Before deployment

□ Build succeeds

□ No warnings

□ Tests pass

□ Swagger works

□ JWT configured

□ SMTP configured

□ PostgreSQL configured

□ Logging enabled

□ Telemetry enabled

□ Migrations applied

□ Environment variables configured

□ Health endpoint implemented

□ Postman collection verified

---

# 96. Recommended Implementation Order

The remaining project should be implemented in the following order.

Phase 1

Complete Authentication

- Password Service
- Refresh Tokens
- Logout
- Forgot Password
- Reset Password
- Change Password
- Auth Controller
- JWT Middleware

Phase 2

Complete User Module

- Profile Retrieval
- Profile Update
- Avatar Placeholder

Phase 3

Complete Post Module

- CRUD
- Publish
- Search
- Pagination
- Lock Comments

Phase 4

Complete Comment Module

- CRUD
- Replies
- Pagination
- Authorization

Phase 5

Infrastructure

- Swagger
- Logging
- Telemetry
- Middleware
- Configuration

Phase 6

Testing

- Unit Tests
- Integration Tests

Phase 7

Documentation

- Swagger Verification
- Postman
- README Updates

---

# 97. Definition of Done

The Blogging System API is considered complete only when the following conditions are simultaneously true.

Authentication

□ Register

□ Verify Email

□ Login

□ Refresh Token

□ Logout

□ Forgot Password

□ Reset Password

□ Change Password

Users

□ View Profile

□ Update Profile

Posts

□ Create

□ Read

□ Update

□ Delete

□ Publish

□ Search

□ Pagination

□ Lock Comments

□ Unlock Comments

Comments

□ Create

□ Reply

□ Retrieve

□ Delete

Infrastructure

□ Dependency Injection Complete

□ JWT Configured

□ Swagger Complete

□ Serilog Working

□ OpenTelemetry Working

□ Exception Middleware Working

Quality

□ No Business Logic in Controllers

□ Repository Pattern Followed

□ DTO Mapping Complete

□ Validation Complete

□ Consistent API Responses

□ Clean Build

□ Unit Tests Passing

□ Integration Tests Passing

□ Postman Collection Verified

□ Production Configuration Verified

---

# 98. Final Notes for the Implementing Engineer (Codex)

1. Preserve the existing Clean Architecture. Do not introduce MediatR, CQRS, Vertical Slice Architecture, or any other architectural paradigm unless explicitly requested.

2. Reuse the existing repositories, services, DTOs, validators, and entities whenever possible. Extend them instead of replacing them.

3. Avoid introducing unnecessary abstractions. Favor straightforward, maintainable code that aligns with the project's current structure.

4. Implement one feature completely before moving to the next. A feature is not complete until its service, controller, validation, dependency injection, Swagger documentation, and tests are all finished.

5. Maintain consistency in naming, asynchronous programming, exception handling, API responses, and dependency injection throughout the solution.

6. Ensure that every endpoint can be exercised successfully through Swagger and the accompanying Postman collection before considering the project complete.

---

END OF PART 4

PART 5
IMPLEMENTATION GUIDELINES FOR CODEX
Objective

Your goal is to complete the Blogging System API without altering its existing architecture.

The repository already contains a partially implemented solution. Treat the current implementation as the source of truth and extend it rather than replacing it.

Architectural Constraints

Do not:

Introduce CQRS
Introduce MediatR
Introduce Vertical Slice Architecture
Introduce Minimal APIs
Replace Entity Framework Core
Replace PostgreSQL
Replace JWT authentication
Rewrite existing repositories unless required to fix defects
Create duplicate services that overlap with existing implementations
Rename public APIs without necessity
Remove existing DTOs, entities, or validators

The project already follows a layered Clean Architecture. Preserve this structure throughout implementation.

Implementation Philosophy

When implementing a feature:

Reuse existing code whenever possible.
Extend existing services before creating new ones.
Avoid introducing unnecessary abstractions.
Prefer simple, maintainable solutions over clever patterns.
Keep business logic within the Application layer.
Keep controllers thin and focused on HTTP concerns.
Use repositories exclusively for persistence operations.
Preserve asynchronous programming throughout the codebase.
Coding Standards

All new code should:

Use nullable reference types.
Use constructor dependency injection.
Use async/await consistently.
Follow existing naming conventions.
Include XML documentation where appropriate.
Respect SOLID principles without excessive abstraction.
Error Handling

Continue using the project's standardized exception handling strategy.

Avoid returning ad hoc error responses.

All exceptions should flow through the global exception middleware.

Validation

Use FluentValidation for every request model.

Do not perform manual validation inside controllers.

Business services should assume validated input.

Logging

Log significant business events.

Do not log:

passwords
JWTs
refresh tokens
OTP values
connection strings
secrets
Security

Ensure all protected endpoints require authentication.

Enforce ownership checks for resources that belong to individual users.

Never trust client-supplied user identifiers when the authenticated identity is available through JWT claims.

Testing

Every completed feature should include:

unit tests for business logic
integration tests for API behavior where applicable

Existing tests should continue to pass after every change.

Completion Strategy

Implement the remaining work incrementally in the following order:

Authentication
User Management
Post Management
Comment Management
Infrastructure
Swagger
Testing
Final verification

Complete each feature before moving to the next.

Do not leave partially implemented functionality.

Definition of Success

The project is complete when:

all documented endpoints are implemented,
all business rules are enforced,
the solution builds successfully,
migrations execute successfully,
authentication is functional,
CRUD operations work correctly,
validation is comprehensive,
tests pass,
Swagger accurately documents the API,
and the application is ready for deployment without requiring architectural changes.
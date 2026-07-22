# Blog System API test guide

This guide covers manual and automated verification of the Blogging System API.

## 1. Prerequisites

1. Create a local secret file at `src/Blog.API/appsettings.Local.json` (it is ignored by Git), or provide the equivalent environment variables. Environment variables take precedence over the local file.
2. Use `src/Blog.API/appsettings.example.json` as the template.
3. Ensure PostgreSQL is running and the configured database exists.
4. Apply migrations:

```bash
dotnet ef database update --project src/Blog.Infrastructure --startup-project src/Blog.API
```

5. Start the API:

```bash
dotnet run --project src/Blog.API
```

6. Open Swagger in Development at `https://localhost:<port>/swagger`.

## 2. Automated tests

Run the complete suite:

```bash
dotnet test
```

Current automated coverage includes password-policy validation, BCrypt password hashing, refresh-token generation, and the HTTP health endpoint.

## 3. Health and API documentation

### Health

```http
GET /health
```

Expected: `200 OK` with a JSON status of `healthy`.

### OpenAPI and Swagger UI

- OpenAPI JSON: `/openapi/v1.json` in Development.
- Swagger UI: `/swagger` in Development.
- For protected endpoints, click **Authorize** and enter `Bearer <access-token>`.

## 4. Authentication

Use a disposable email address or a local SMTP sandbox such as smtp4dev while testing.

### Register

```http
POST /api/auth/register
Content-Type: application/json

{
  "firstName": "Jane",
  "surname": "Doe",
  "email": "jane@example.com",
  "password": "ValidPassword1!",
  "confirmPassword": "ValidPassword1!"
}
```

Expected: `201 Created`. The password must be at least eight characters and include uppercase, lowercase, number, and special character.

Test failures:

- Reuse the email: expect an error.
- Use a weak password: expect validation errors.
- Check the database: the password must be a BCrypt hash, never plain text.

### Verify email

Read the six-digit OTP from the received email or SMTP sandbox.

```http
POST /api/auth/verify-email
Content-Type: application/json

{
  "email": "jane@example.com",
  "code": "123456"
}
```

Expected: `200 OK`. The account becomes verified and the OTP is marked used. Repeat the request: expect an error.

### Login

```http
POST /api/auth/login
Content-Type: application/json

{
  "email": "jane@example.com",
  "password": "ValidPassword1!"
}
```

Expected: `200 OK`, with `accessToken`, `refreshToken`, and `expiresAt`.

Test failures:

- Wrong password: rejected.
- Unverified account: rejected.
- Verify the JWT includes the user id, email, and role claims.

Save the access token as `ACCESS_TOKEN` and refresh token as `REFRESH_TOKEN` for later requests.

### Refresh token

```http
POST /api/auth/refresh
Content-Type: application/json

{
  "refreshToken": "<REFRESH_TOKEN>"
}
```

Expected: `200 OK` with a new access token and refresh token. Reuse the original refresh token: it must be rejected because refresh-token rotation revokes it.

### Logout

```http
POST /api/auth/logout
Authorization: Bearer <ACCESS_TOKEN>
```

Expected: `204 No Content`. Attempt to refresh using any token belonging to that user: it must fail.

### Forgot and reset password

```http
POST /api/auth/forgot-password
Content-Type: application/json

{ "email": "jane@example.com" }
```

Expected: `200 OK` whether or not the account exists. This prevents account-enumeration attacks.

Use the received OTP:

```http
POST /api/auth/reset-password
Content-Type: application/json

{
  "email": "jane@example.com",
  "code": "123456",
  "newPassword": "AnotherValidPassword1!",
  "confirmPassword": "AnotherValidPassword1!"
}
```

Expected: `200 OK`. Existing refresh tokens must no longer work. Login using the new password.

### Change password

```http
PUT /api/auth/change-password
Authorization: Bearer <ACCESS_TOKEN>
Content-Type: application/json

{
  "currentPassword": "AnotherValidPassword1!",
  "newPassword": "FinalValidPassword1!",
  "confirmPassword": "FinalValidPassword1!"
}
```

Expected: `200 OK`, followed by refresh-token invalidation.

## 5. User profile

### Get profile

```http
GET /api/users/profile
Authorization: Bearer <ACCESS_TOKEN>
```

Expected: `200 OK`. Confirm the response does not contain `passwordHash`, OTPs, or refresh tokens.

### Update profile

```http
PUT /api/users/profile
Authorization: Bearer <ACCESS_TOKEN>
Content-Type: application/json

{
  "firstName": "Jane",
  "surname": "Smith",
  "bio": "Technical writer.",
  "avatar": "https://example.com/avatar.png"
}
```

Expected: `200 OK`; retrieve the profile again to confirm persistence. Email and role cannot be modified by this endpoint.

## 6. Posts

### Create draft

```http
POST /api/posts
Authorization: Bearer <ACCESS_TOKEN>
Content-Type: application/json

{
  "title": "My first post",
  "summary": "A short introduction.",
  "content": "The complete article content."
}
```

Expected: `201 Created`; status is `Draft`. Save the returned `id` as `POST_ID`.

### View and search published posts

```http
GET /api/posts?page=1&pageSize=10
GET /api/posts/search?query=first&page=1&pageSize=10
```

Expected: `200 OK` with `items`, `page`, `pageSize`, `totalCount`, and `totalPages`. Drafts must not appear publicly.

### Read a post

```http
GET /api/posts/<POST_ID>
```

Expected: public reads work only after publication. The author may retrieve their own draft while authenticated.

### Update, publish, and delete

```http
PUT /api/posts/<POST_ID>
PATCH /api/posts/<POST_ID>/publish
DELETE /api/posts/<POST_ID>
```

Use the access token of the post author. Expected: successful response. Repeat with another normal user: expect `403 Forbidden`. An administrator is allowed to manage any post.

### Lock comments

```http
PATCH /api/posts/<POST_ID>/lock-comments
PATCH /api/posts/<POST_ID>/unlock-comments
Authorization: Bearer <ACCESS_TOKEN>
```

Expected: only the owner or administrator can change this setting.

## 7. Comments

Publish the post before creating comments.

### Create comment

```http
POST /api/posts/<POST_ID>/comments
Authorization: Bearer <ACCESS_TOKEN>
Content-Type: application/json

{ "content": "Helpful article." }
```

Expected: `201 Created`. Confirm anonymous requests are rejected and comments on drafts or locked posts fail.

### Reply and list comments

```http
POST /api/comments/<COMMENT_ID>/reply
Authorization: Bearer <ACCESS_TOKEN>
Content-Type: application/json

{ "content": "Thanks!" }

GET /api/posts/<POST_ID>/comments?page=1&pageSize=10
```

Expected: `201 Created` for the reply and paginated results for the list. Attempting to reply to a reply must fail because only one reply level is supported.

### Delete comment

```http
DELETE /api/comments/<COMMENT_ID>
Authorization: Bearer <ACCESS_TOKEN>
```

Expected: `204 No Content` for the comment author or administrator; `403 Forbidden` for other users.

## 8. Validation and error responses

Send malformed request bodies, missing fields, whitespace-only content, invalid GUIDs, invalid page sizes, or invalid credentials.

Expected errors use the common structure:

```json
{
  "success": false,
  "message": "Validation failed.",
  "data": null,
  "errors": ["..."],
  "timestamp": "..."
}
```

Expected status codes include `400` for invalid input/business rules, `401` for missing or invalid authentication, and `403` for ownership or role failures.

## 9. Logging and telemetry

### Serilog

Run the API and exercise endpoints. Verify structured request logs contain method, path, status code, and duration. Never expect passwords, OTPs, JWTs, refresh tokens, connection strings, or SMTP credentials in logs.

### OpenTelemetry

The API instruments ASP.NET Core requests, EF Core database calls, and the `BlogSystem.Api` activity source. Configure an OTLP exporter destination using the standard OpenTelemetry environment variables for your collector/deployment. Send a request, then confirm a trace and its child database spans arrive at the collector.

## 10. Final checklist

- `dotnet restore`, `dotnet build`, and `dotnet test` succeed.
- Migrations apply to PostgreSQL.
- All authentication flows work with a non-production SMTP account or sandbox.
- Swagger UI authorizes protected endpoints with a JWT.
- Authorization checks prevent cross-user post/comment changes.
- No secret appears in Git status, configuration committed to Git, API responses, or logs.

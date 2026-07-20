# Codex implementation record

## Scope

This record documents implementation completed from `Documentation.md` on 2026-07-17. The existing Clean Architecture was preserved.

## Delivered API surface

### Authentication

- `POST /api/auth/register`, `/verify-email`, `/login`, `/refresh`, `/logout`, `/forgot-password`, `/reset-password`
- `PUT /api/auth/change-password`
- JWT bearer authentication, refresh-token rotation, logout revocation, and password-change/reset revocation.

### Users

- `GET /api/users/profile`
- `PUT /api/users/profile`

### Posts

- `POST`, `GET`, `GET /{id}`, `PUT /{id}`, and `DELETE /api/posts/{id}`
- `GET /api/posts/search`
- `PATCH /api/posts/{id}/publish`, `/lock-comments`, and `/unlock-comments`
- Post lists and searches are paginated; ownership and administrator access are enforced in application services. Deletion is hard-delete because the current schema has no soft-delete field, which the specification permits.

### Comments

- `POST /api/posts/{postId}/comments`
- `POST /api/comments/{commentId}/reply`
- `GET /api/posts/{postId}/comments`
- `DELETE /api/comments/{commentId}`

Comments require a published, unlocked post. Replies are limited to one level. Author/administrator authorization is enforced.

### Shared behavior

- FluentValidation for request DTOs.
- Common success response model and global exception middleware.
- Development OpenAPI document through existing `AddOpenApi` / `MapOpenApi`.
- `GET /health`.
- Cryptographically secure OTP generation; replacement OTPs invalidate old unused OTPs for the same purpose.
- Baseline unit test for password validation.

## Verification

The user verified `dotnet build` succeeds after the async-service correction. Codex's own runner cannot run compilation because its .NET workload resolver is incomplete. Run locally:

```bash
dotnet build
dotnet test
```

## Production requirements

Configure database, JWT, and SMTP values using environment variables or user secrets. Rotate the credentials currently present in tracked `appsettings.json` before sharing or deploying the project.

The existing build warns that `Microsoft.OpenApi` 2.0.0 has a high-severity advisory; update the OpenAPI dependency during dependency maintenance. Production rollout should additionally include real PostgreSQL integration tests, SMTP sandbox verification, Serilog/OpenTelemetry exporters, and (if desired) an interactive Swagger UI package.

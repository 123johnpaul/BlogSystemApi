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

## Security and operational configuration

- Sensitive connection-string and SMTP values are no longer tracked in `appsettings.json`.
- `appsettings.example.json` shows every required setting; configure actual values with environment variables, user secrets, or a local ignored `appsettings.Local.json`.
- `.gitignore` excludes local configuration, build outputs, IDE state, and test results.
- Swagger UI is available in Development at `/swagger`, with JWT bearer-token support.
- Serilog emits structured request and application logs; OpenTelemetry instruments HTTP requests, EF Core calls, and the API `ActivitySource`, with OTLP export configurable through standard OpenTelemetry environment settings.

## Verification

Verification completed on 2026-07-22:

- 3 unit tests passed.
- 1 integration test passed.
- `dotnet test` built all projects successfully.

Run locally after further changes:

```bash
dotnet build
dotnet test
```

## Production requirements

Configure database, JWT, and SMTP values using environment variables or user secrets. Rotate the credentials currently present in tracked `appsettings.json` before sharing or deploying the project.

Production rollout should additionally include real PostgreSQL integration tests, SMTP sandbox verification, and an OTLP collector/exporter endpoint appropriate for the deployment environment.

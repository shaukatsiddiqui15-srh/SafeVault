# SafeVault

SafeVault is a secure web application for managing sensitive data. This repository captures the security work completed across the three activities:

- Activity 1: input validation and SQL injection/XSS defenses
- Activity 2: authentication and role-based authorization
- Activity 3: debugging and hardening of the secure codebase

## Vulnerabilities Identified

- SQL injection risk in database access patterns
- Cross-site scripting (XSS) risk in user-facing output and form handling
- Missing authentication and authorization controls for protected features
- Build artifact clutter in source control from `bin/` and `obj/` output

## Fixes Applied

- Replaced unsafe query patterns with parameterized database commands
- Added input sanitization and HTML encoding for user-provided content
- Implemented password hashing and verification with PBKDF2
- Added authentication services to validate login credentials
- Added role-based authorization for admin-only access
- Added `.gitignore` rules to exclude generated build outputs
- Added automated NUnit tests covering SQL injection, XSS, login failure, and access control scenarios

## How Copilot Helped

Microsoft Copilot was used as a coding aid to:

- generate secure code patterns for validation, authentication, and authorization
- suggest test cases that simulate malicious input and unauthorized access attempts
- accelerate debugging by surfacing likely security weaknesses and guiding the replacement of unsafe code with safer alternatives

## Verification

The solution was validated with:

```bash
dotnet test SafeVault.sln
```

All tests passed after the fixes were applied.


# Security Policy

## Supported Versions

This project is a student systems-programming assignment. Security patches are not actively maintained.

| Version | Supported |
|---------|-----------|
| 1.0.0   | :white_check_mark: |

## Reporting a Vulnerability

If you discover a security issue, please:

1. **Do not** open a public issue.
2. Contact the maintainer directly via GitHub.
3. Include steps to reproduce, affected files, and potential impact.

We will acknowledge receipt within 7 business days and work on a resolution.

## Security Best Practices for Users

- **Run as Administrator** only when you need process/service control.
- Do not run this app on production systems without review.
- The app uses Win32 P/Invoke (`kernel32.dll`, `ntdll.dll`) to suspend/resume and kill processes — misuse can destabilize the OS.
- All OS interactions are wrapped in `try-catch` blocks; access-denied scenarios are handled gracefully.

## Disclaimer

This software is provided for educational purposes. The authors are not responsible for any system instability, data loss, or damage caused by misuse of process/service control features.
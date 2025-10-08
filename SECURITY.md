# Security Policy

## Supported Versions

ZPL2PDF follows semantic versioning and provides security updates for the following versions:

| Version | Supported          | End of Support |
| ------- | ------------------ | -------------- |
| 2.0.x   | :white_check_mark: | TBD            |
| 1.x.x   | :x:                | N/A            |
| < 1.0   | :x:                | N/A            |

**Note:** As ZPL2PDF v2.0.0 is the first major release, all security updates will be provided for the 2.x.x series. Support for v1.x.x and earlier versions is not available as these were pre-release versions.

## Security Updates

- **Critical vulnerabilities**: Patches released within 24-48 hours
- **High severity**: Patches released within 1 week
- **Medium severity**: Patches released within 2 weeks
- **Low severity**: Patches released in next scheduled release

## Reporting a Vulnerability

We take security seriously and appreciate your efforts to responsibly disclose vulnerabilities.

### How to Report

**Please do NOT report security vulnerabilities through public GitHub issues.**

Instead, please report security vulnerabilities via email:

**Email:** `brunoleocam@gmail.com`

**Subject:** `[SECURITY] ZPL2PDF Vulnerability Report`

### What to Include

Please include the following information in your report:

1. **Description** of the vulnerability
2. **Steps to reproduce** the issue
3. **Potential impact** and severity assessment
4. **Suggested mitigation** (if any)
5. **Your contact information** for follow-up

### Response Timeline

- **Acknowledgment**: Within 48 hours
- **Initial assessment**: Within 1 week
- **Regular updates**: Every 2 weeks until resolution
- **Resolution timeline**: Depends on severity (see Security Updates section)

### What to Expect

**If the vulnerability is accepted:**
- You will receive acknowledgment and regular updates
- A security patch will be developed and tested
- The vulnerability will be disclosed publicly after the patch is released
- You will be credited in the security advisory (unless you prefer to remain anonymous)

**If the vulnerability is declined:**
- You will receive an explanation of why it was not accepted
- You may choose to disclose it publicly after 90 days

### Scope

This security policy applies to:

- ✅ ZPL2PDF core application
- ✅ Docker images
- ✅ Installation packages
- ✅ Documentation website
- ❌ Third-party dependencies (please report to their respective maintainers)
- ❌ Issues with your local system configuration

### Security Best Practices

When using ZPL2PDF:

1. **Keep updated**: Always use the latest version
2. **Verify downloads**: Check SHA256 checksums before installation
3. **Sandbox execution**: Run in isolated environments when processing untrusted ZPL files
4. **File permissions**: Ensure proper file system permissions
5. **Network security**: Use HTTPS when downloading from official sources

### Responsible Disclosure

We follow responsible disclosure practices:

- **No public disclosure** until a patch is available
- **Coordinated disclosure** with security researchers
- **Timely patching** based on severity
- **Clear communication** throughout the process

### Security Contact

For general security questions or concerns:

- **Email**: `brunoleocam@gmail.com`
- **GitHub Issues**: For non-security related questions only
- **Documentation**: Check our [main documentation](README.md) for usage and best practices

---

**Thank you for helping keep ZPL2PDF secure!** 🔒

*Last updated: October 2025*

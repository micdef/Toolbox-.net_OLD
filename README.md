# Toolbox

A .NET 8 utility library providing common services for enterprise applications.

## Overview

Toolbox is a collection of reusable services designed to simplify common development tasks such as:

- **Active Directory** - User authentication, group management, and LDAP operations
- **Configuration** - JSON and INI configuration file management
- **Cryptography** - AES, RSA, and Base64 encryption/decryption
- **FTP/SFTP** - File transfer operations
- **Logging** - Structured logging with email alerts for critical errors
- **Mailing** - Email sending with attachments support

## Requirements

- .NET 8.0 SDK
- Windows (for Active Directory features)

## Installation

### From Source

```bash
git clone <repository-url>
cd Toolbox_OLD
dotnet restore
dotnet build
```

### NuGet Packages

The project uses the following NuGet packages:

| Package | Version | Purpose |
|---------|---------|---------|
| Microsoft.Extensions.Configuration | 8.0.0 | Configuration management |
| Microsoft.Extensions.Configuration.Json | 8.0.0 | JSON configuration support |
| Microsoft.Extensions.Configuration.Ini | 8.0.0 | INI configuration support |
| Microsoft.Extensions.FileProviders.Physical | 8.0.0 | File system access |
| Microsoft.Extensions.Logging.Abstractions | 8.0.2 | Logging abstractions |
| Newtonsoft.Json | 13.0.3 | JSON serialization |
| SSH.NET | 2024.2.0 | SFTP support |
| System.DirectoryServices | 8.0.0 | Active Directory (Windows) |
| System.DirectoryServices.AccountManagement | 8.0.0 | Active Directory (Windows) |

## Project Structure

```
Toolbox/
├── ActiveDirectory/
│   ├── IActiveDirectoryService.cs
│   └── ActiveDirectoryService.cs
├── Attributes/
│   ├── FromAttribute.cs
│   └── ToSqlParameterAttribute.cs
├── Configuration/
│   ├── IConfigurationService.cs
│   └── ConfigurationService.cs
├── Cryptography/
│   ├── ICryptographyService.cs
│   ├── AesCryptographyService.cs
│   ├── Base64Cryptography.cs
│   └── RsaCryptographyService.cs
├── Ftp/
│   ├── IFtpService.cs
│   └── FtpService.cs
├── Logs/
│   ├── ILoggingService.cs
│   └── LoggingService.cs
├── Mailing/
│   ├── IMailingService.cs
│   └── MailingService.cs
└── Extensers.cs
```

## Features

### Active Directory Service
- User/Group/Computer management
- Authentication and password management
- Organizational Unit operations
- Group membership management
- Password complexity validation (Belgian NISS support)

### Configuration Service
- JSON and INI file support
- Hierarchical configuration access
- Connection string management
- Auto-reload on file changes

### Cryptography Services
- **AES** - Symmetric encryption (128/196/256-bit keys)
- **RSA** - Asymmetric encryption (512 to 16384-bit keys)
- **Base64** - Multiple encoding tables (UTF-8, ASCII, Unicode, etc.)

### FTP/SFTP Service
- File upload and download
- Support for both FTP and SFTP protocols
- Certificate-based authentication for SFTP

### Logging Service
- Multiple log levels (Debug, Trace, Info, Warning, Error, Critical)
- Email notifications for critical errors
- Configurable via JSON/INI files

### Mailing Service
- HTML and plain text emails
- Multiple recipients (To, CC, BCC)
- File attachments support
- Async sending

### Extension Methods
- String validation and manipulation
- Date/Time parsing (Oracle/Bamboo formats)
- Belgian NISS validation
- Encryption/Decryption helpers

## Usage

See [usage.md](usage.md) for detailed examples and usage instructions.

## License

[Add your license here]

## Contributing

[Add contribution guidelines here]

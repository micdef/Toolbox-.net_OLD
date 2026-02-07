# Toolbox Usage Guide

This guide provides detailed examples for using each service in the Toolbox library.

## Table of Contents

1. [Configuration Service](#configuration-service)
2. [Cryptography Services](#cryptography-services)
3. [Active Directory Service](#active-directory-service)
4. [FTP/SFTP Service](#ftpsftp-service)
5. [Mailing Service](#mailing-service)
6. [Logging Service](#logging-service)
7. [Extension Methods](#extension-methods)
8. [Attributes](#attributes)

---

## Configuration Service

The `ConfigurationService` manages JSON and INI configuration files.

### Setup

```csharp
using Toolbox.Configuration;

// JSON configuration
var configService = new ConfigurationService(
    basePath: @"C:\Config",
    filename: "appsettings.json",
    reloadOnChange: true,
    isOptional: false,
    isIniFile: false
);

// INI configuration
var iniConfigService = new ConfigurationService(
    basePath: @"C:\Config",
    filename: "settings.ini",
    reloadOnChange: true,
    isOptional: false,
    isIniFile: true
);
```

### Reading Values

```csharp
// Get a string value from nested sections
string value = configService.GetValue(new List<string> { "Application", "Settings", "Name" });

// Get a typed value
int timeout = configService.GetValue<int>(new List<string> { "Application", "Settings", "Timeout" });

// Get a connection string
string connectionString = configService.GetConnectionString("DefaultConnection");

// Get a configuration section
IConfigurationSection section = configService.GetSection(new List<string> { "Application", "Settings" });
```

### Example Configuration File (appsettings.json)

```json
{
  "Application": {
    "Info": {
      "Name": "MyApp",
      "Code": "APP001",
      "Version": "1.0.0"
    },
    "Settings": {
      "Timeout": 30
    },
    "Security": {
      "Aes": {
        "Key": "path/to/key.bin",
        "Iv": "path/to/iv.bin"
      },
      "Base64": {
        "EncodingTable": "Utf8"
      }
    },
    "ActiveDirectory": {
      "ADDS": {
        "Username": "encrypted_username",
        "Password": "encrypted_password",
        "DomainName": "encrypted_domain"
      }
    }
  },
  "ConnectionStrings": {
    "DefaultConnection": "Server=...;Database=...;"
  }
}
```

---

## Cryptography Services

### Base64 Cryptography

Simple encoding/decoding with multiple character sets.

```csharp
using Toolbox.Cryptography;

// Create service with UTF-8 encoding
var base64Service = new Base64CryptographyService(Base64CryptographyService.Base64Tables.Utf8);

// Encrypt
string encrypted = base64Service.Encrypt("Hello World");
// Result: "SGVsbG8gV29ybGQ="

// Decrypt
string decrypted = base64Service.Decrypt(encrypted);
// Result: "Hello World"

// Async versions
string encryptedAsync = await base64Service.EncryptAsync("Hello World");
string decryptedAsync = await base64Service.DecryptAsync(encryptedAsync);
```

**Available Encoding Tables:**
- `Default` - System default encoding
- `Latin1` - ISO-8859-1
- `Unicode` - UTF-16 Little Endian
- `BigEndianUnicode` - UTF-16 Big Endian
- `Utf8` - UTF-8
- `Utf32` - UTF-32
- `Ascii` - ASCII

### AES Cryptography

Symmetric encryption for secure data storage.

```csharp
using Toolbox.Cryptography;

// Generate new keys
var (key, iv) = AesCryptographyService.GenerateKeys(AesCryptographyService.KeySizes.S256);

// Save keys to files for later use
File.WriteAllBytes("aes_key.bin", key);
File.WriteAllBytes("aes_iv.bin", iv);

// Create service with key and IV
var aesService = new AesCryptographyService(key, iv);

// Or use configuration
var aesServiceFromConfig = new AesCryptographyService(configService);

// Encrypt/Decrypt
string encrypted = aesService.Encrypt("Sensitive data");
string decrypted = aesService.Decrypt(encrypted);

// Async versions
string encryptedAsync = await aesService.EncryptAsync("Sensitive data");
string decryptedAsync = await aesService.DecryptAsync(encryptedAsync);
```

**Available Key Sizes:**
- `S128` - 128-bit
- `S196` - 196-bit
- `S256` - 256-bit (recommended)

### RSA Cryptography

Asymmetric encryption for secure key exchange.

```csharp
using Toolbox.Cryptography;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

// Generate new keys
var (publicKey, privateKey) = RsaCryptographyService.GenerateKeys(RsaCryptographyService.KeySizes.S2048);

// Create service with keys
var rsaService = new RsaCryptographyService(privateKey, publicKey, RSAEncryptionPadding.OaepSHA256);

// Or use a certificate
var cert = new X509Certificate2("certificate.pfx", "password");
var rsaServiceFromCert = new RsaCryptographyService(cert, RSAEncryptionPadding.OaepSHA256);

// Encrypt/Decrypt
string encrypted = rsaService.Encrypt("Secret message");
string decrypted = rsaService.Decrypt(encrypted);
```

**Available Key Sizes:**
- `S512` - 512-bit (weak, not recommended)
- `S1024` - 1024-bit (deprecated)
- `S2048` - 2048-bit (minimum recommended)
- `S4096` - 4096-bit (strong)
- `S8192` - 8192-bit (very strong)
- `S16384` - 16384-bit (maximum)

---

## Active Directory Service

> **Note:** Requires Windows and appropriate Active Directory permissions.

### Setup

```csharp
using Toolbox.ActiveDirectory;
using Toolbox.Configuration;
using Toolbox.Cryptography;
using System.DirectoryServices.AccountManagement;

// Using configuration (credentials are encrypted in config)
var configService = new ConfigurationService(@"C:\Config", "appsettings.json");
var cryptoService = new Base64CryptographyService(Base64CryptographyService.Base64Tables.Utf8);
var adService = new ActiveDirectoryService(configService, cryptoService);

// Direct instantiation (credentials are Base64 encrypted)
var adServiceDirect = new ActiveDirectoryService(
    contextType: ContextType.Domain,
    username: "ZW5jcnlwdGVkX3VzZXJuYW1l",  // Base64 encrypted
    password: "ZW5jcnlwdGVkX3Bhc3N3b3Jk",  // Base64 encrypted
    domainName: "ZW5jcnlwdGVkX2RvbWFpbg=="  // Base64 encrypted
);
```

### User Authentication

```csharp
// Login user
var (user, exception) = adService.Login("john.doe", "password123", authorizeGenericAccount: false);

if (exception == null)
{
    Console.WriteLine($"Welcome, {user.DisplayName}!");
}
else
{
    Console.WriteLine($"Login failed: {exception.Message}");
}
```

### User Management

```csharp
// Search for a user
var users = adService.SearchObjects<UserPrincipal>("SamAccountName", "john.doe");
var user = users.FirstOrDefault();

// Check if user exists
bool exists = adService.Exists(user);

// Check if user is enabled
bool isEnabled = adService.IsEnabled(user);

// Activate/Deactivate user
adService.Activate(user);
adService.Deactivate(user);

// Create a new user
var properties = new Dictionary<string, object?>
{
    { "GivenName", "John" },
    { "Surname", "Doe" },
    { "DisplayName", "John Doe" },
    { "EmailAddress", "john.doe@company.com" },
    { "Password", "SecureP@ssw0rd!" },
    { "PasswordNeverExpires", false },
    { "Enabled", true }
};

var newUser = adService.CreateUser("john.doe", properties, new List<string> { "Domain Users" });
```

### Password Management

```csharp
// Check password complexity
bool isComplex = adService.CheckPasswordComplexity(user, "NewP@ssw0rd!");

// Change password
adService.ChangePassword(
    user: user,
    newPassword: "NewP@ssw0rd!",
    currentPassword: "OldPassword",  // optional
    passwordMustBeChangedAtNextLogon: true
);
```

### Group Management

```csharp
// Get group
var groups = adService.SearchObjects<GroupPrincipal>("Name", "Developers");
var devGroup = groups.FirstOrDefault();

// Get group members
var allMembers = adService.GetGroupMembers(devGroup);
var userMembers = adService.GetGroupMembers<UserPrincipal>(devGroup);

// Check membership
bool isMember = adService.IsGroupMember(devGroup, user);

// Add/Remove member
adService.AddGroupMember(devGroup, user);
adService.RemoveGroupMember(devGroup, user);

// Create a new group
var groupProperties = new Dictionary<string, object?>
{
    { "Description", "Development Team" }
};
var newGroup = adService.CreateGroup("NewDevTeam", groupProperties);
```

### Organizational Unit Operations

```csharp
// Get objects from OU
string ouDN = "OU=Users,DC=company,DC=com";
var usersInOU = adService.GetObjectsFromOrganizationalUnit<UserPrincipal>(ouDN, checkAllTree: true);

// Move object to another OU
adService.MoveObject(user, "OU=Archived,DC=company,DC=com", newName: null);
```

### Account Type Detection

```csharp
// Get generic accounts (no valid Belgian NISS)
var genericAccounts = adService.GetGenericAccountsFromAList(users);

// Get nominated accounts (with valid Belgian NISS)
var nominatedAccounts = adService.GetNominatedAccountsFromAList(users);
```

---

## FTP/SFTP Service

### Setup

```csharp
using Toolbox.Ftp;

// FTP connection
var ftpService = new FtpService(
    host: "ftp://ftp.example.com",
    username: "ZnRwX3VzZXI=",     // Base64 encrypted
    password: "ZnRwX3Bhc3M=",     // Base64 encrypted
    port: 21,
    isUsernameSecured: true,
    isPasswordSecured: true,
    isSftp: false
);

// SFTP connection
var sftpService = new FtpService(
    host: "sftp.example.com",
    username: "c2Z0cF91c2Vy",     // Base64 encrypted
    password: "c2Z0cF9wYXNz",     // Base64 encrypted
    port: 22,
    isUsernameSecured: true,
    isPasswordSecured: true,
    isSftp: true,
    certFile: @"C:\Certs\private_key.pem"
);

// Unencrypted credentials
var ftpServicePlain = new FtpService(
    host: "ftp://ftp.example.com",
    username: "ftp_user",
    password: "ftp_pass",
    port: 21,
    isUsernameSecured: false,
    isPasswordSecured: false,
    isSftp: false
);
```

### File Operations

```csharp
// Download file
bool downloadSuccess = await ftpService.DownloadFile(
    localPath: @"C:\Downloads\file.txt",
    remoteFolder: "/uploads",
    remoteFile: "file.txt",
    bufferSize: 65536
);

// Upload file
bool uploadSuccess = await ftpService.UploadFile(
    localPath: @"C:\Documents\report.pdf",
    remoteFolder: "/reports"
);
```

---

## Mailing Service

### Setup

```csharp
using Toolbox.Mailing;

// Direct SMTP server
var mailService = new MailingService("smtp.company.com");

// Using configuration
var mailServiceFromConfig = new MailingService(configService);
```

### Sending Emails

```csharp
using System.Net.Mail;

// Send simple email
mailService.SendMail(
    from: new Tuple<string, string?>("noreply@company.com", "My Application"),
    to: new List<string> { "user@example.com" },
    cc: null,
    bcc: null,
    subject: "Test Email",
    body: new List<string> { "<h1>Hello!</h1>", "<p>This is a test.</p>" },
    isBodyHtml: true,
    mailPriority: MailPriority.Normal
);

// Send email with attachments
mailService.SendMail(
    from: new Tuple<string, string?>("reports@company.com", "Report System"),
    to: new List<string> { "manager@company.com" },
    cc: new List<string> { "team@company.com" },
    bcc: new List<string> { "archive@company.com" },
    subject: "Monthly Report",
    body: new List<string> { "Please find the monthly report attached." },
    isBodyHtml: false,
    mailPriority: MailPriority.High,
    attachments: new List<FileInfo> { new FileInfo(@"C:\Reports\monthly.pdf") }
);

// Async version
await mailService.SendMailAsync(
    from: new Tuple<string, string?>("noreply@company.com", null),
    to: new List<string> { "user@example.com" },
    cc: null,
    bcc: null,
    subject: "Async Email",
    body: new List<string> { "Sent asynchronously!" },
    isBodyHtml: false
);

// With completion handler
mailService.SendMail(
    from: new Tuple<string, string?>("noreply@company.com", null),
    to: new List<string> { "user@example.com" },
    cc: null,
    bcc: null,
    subject: "With Handler",
    body: new List<string> { "Check console for completion." },
    isBodyHtml: false,
    sendCompletedEventHandler: (sender, e) =>
    {
        if (e.Error != null)
            Console.WriteLine($"Error: {e.Error.Message}");
        else
            Console.WriteLine("Email sent successfully!");
    }
);
```

---

## Logging Service

### Setup

```csharp
using Toolbox.Logs;
using Microsoft.Extensions.Logging;

// Using configuration
var loggingService = new LoggingService(configService, mailService);

// Load logger instance (required before use)
ILogger<MyClass> logger = LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger<MyClass>();
loggingService.Load(logger, "MyModule");

// Direct instantiation
var loggingServiceDirect = new LoggingService(
    applicationName: "MyApp",
    applicationCode: "APP001",
    applicationVersion: "1.0.0",
    moduleName: "MainModule",
    logDebug: true,
    logTrace: true,
    fromEmail: "alerts@company.com",
    fromDisplay: "Alert System",
    to: new List<string> { "admin@company.com" },
    cc: new List<string>(),
    bcc: new List<string>(),
    logger: logger,
    events: new List<EventId>
    {
        new EventId(1000, "ApplicationStart"),
        new EventId(1001, "ApplicationStop"),
        new EventId(2000, "UserLogin"),
        new EventId(5000, "CriticalError")
    },
    svcMailing: mailService
);
```

### Writing Logs

```csharp
// Get event from list
EventId appStartEvent = loggingService.Events.First(e => e.Id == 1000);

// Log different levels
loggingService.WriteLog("Initialize", LogLevel.Information, appStartEvent, "Application started successfully");

loggingService.WriteLog("ProcessData", LogLevel.Debug, appStartEvent, "Processing data batch #123");

loggingService.WriteLog("ValidateInput", LogLevel.Warning, appStartEvent, "Invalid input detected, using default");

try
{
    // Some operation
    throw new InvalidOperationException("Database connection failed");
}
catch (Exception ex)
{
    // Critical errors automatically send email alerts
    loggingService.WriteLog(
        fctName: "ConnectDatabase",
        level: LogLevel.Critical,
        eventId: new EventId(5000, "CriticalError"),
        message: "Failed to connect to database",
        ex: ex
    );
}
```

### Configuration File Example

```json
{
  "Application": {
    "Info": {
      "Name": "MyApp",
      "Code": "APP001",
      "Version": "1.0.0"
    }
  },
  "LoggingBSCA": {
    "Logs": {
      "LogTrace": "true",
      "LogDebug": "true",
      "Mailing": {
        "FromEmail": "alerts@company.com",
        "FromDisplay": "Alert System",
        "To": [
          { "Email": "admin@company.com" }
        ],
        "Cc": [],
        "Bcc": []
      }
    },
    "Events": [
      { "Id": "1000", "Text": "ApplicationStart" },
      { "Id": "1001", "Text": "ApplicationStop" },
      { "Id": "5000", "Text": "CriticalError" }
    ]
  }
}
```

---

## Extension Methods

The `Extensers` class provides useful extension methods.

### String Extensions

```csharp
using Toolbox;

// Check if string is null, empty, or whitespace
bool isEmpty = myString.IsStringEmpty();

// Parse Oracle date format (YYYYMMDD)
DateOnly date = "20240115".ToDateOnly();

// Parse Bamboo date format (DD/MM/YYYY or DD-MM-YYYY)
DateOnly bambooDate = "15/01/2024".ToDateOnly(bamboo: true);

// Parse Oracle time format (HHMM)
TimeOnly time = "1430".ToTimeOnly();

// Parse to enum
MyEnum value = "EnumValue".ToEnum<MyEnum>();

// Extract numbers from string
string numbers = "ABC123DEF456".ExtractNumbers();
// Result: "123456"

// Validate Belgian NISS
bool isValidNiss = "85073100123".IsBelgiumNiss();
```

### Encryption Extensions

```csharp
using Toolbox;
using Toolbox.Cryptography;

var cryptoService = new AesCryptographyService(key, iv);

// Encrypt string
string encrypted = "Sensitive data".Encrypt(cryptoService);

// Decrypt string
string decrypted = encrypted.Decrypt(cryptoService);

// Encrypt object (serialized to JSON)
var myObject = new { Name = "John", Age = 30 };
string encryptedObject = myObject.Encrypt(cryptoService);

// Decrypt to object
var decryptedObject = encryptedObject.Decrypt<dynamic>(cryptoService);

// Async versions
string encryptedAsync = await "Sensitive data".EncryptAsync(cryptoService);
string decryptedAsync = await encryptedAsync.DecryptAsync(cryptoService);
```

---

## Attributes

### FromAttribute

Maps a property to a data source field.

```csharp
using Toolbox.Attributes;

public class UserDto
{
    [From("user_id")]
    public int Id { get; set; }

    [From("user_name")]
    public string Name { get; set; }

    [From("email_address")]
    public string Email { get; set; }
}
```

### ToSqlParameterAttribute

Maps a property to a SQL parameter for stored procedures.

```csharp
using Toolbox.Attributes;
using System.Data;

public class CreateUserRequest
{
    [ToSqlParameter("@UserName", ParameterDirection.Input, DbType.String, 100)]
    public string UserName { get; set; }

    [ToSqlParameter("@Email", ParameterDirection.Input, DbType.String, 255)]
    public string Email { get; set; }

    [ToSqlParameter("@UserId", ParameterDirection.Output, DbType.Int32)]
    public int UserId { get; set; }

    [ToSqlParameter("@ReturnValue", ParameterDirection.ReturnValue, DbType.Int32)]
    public int ReturnValue { get; set; }
}
```

---

## Best Practices

1. **Configuration**: Store sensitive data (passwords, keys) encrypted in configuration files.

2. **Cryptography**:
   - Use AES-256 for symmetric encryption
   - Use RSA-2048 or higher for asymmetric encryption
   - Store keys securely, not in source control

3. **Active Directory**:
   - Use a service account with minimal required permissions
   - Always encrypt credentials in configuration

4. **Logging**:
   - Use appropriate log levels
   - Critical errors trigger email alerts - use sparingly

5. **FTP/SFTP**:
   - Prefer SFTP over FTP for security
   - Use certificate-based authentication when possible

// Using
using Renci.SshNet;
using System.Net;
using Toolbox.Cryptography;

// Static using
// #N/A

// Named using
// #N/A

// Namespace
namespace Toolbox.Ftp;

/// <summary>
/// Service used to manage FTP download and upload
/// </summary>
public class FtpService : IFtpService
{
    #region Members

    #region Constants

    // #N/A

    #endregion

    #region Variables

    /// <summary>
    /// Is the service an SFTP one?
    /// </summary>
    private readonly bool _secured;

    /// <summary>
    /// Host name (or ip) of the FTP/SFTP server
    /// </summary>
    private readonly string _host;

    /// <summary>
    /// Port of the FTP/SFTP server
    /// </summary>
    private readonly int _port;

    /// <summary>
    /// Username to connect the FTP/SFTP server
    /// </summary>
    private readonly string _username;

    /// <summary>
    /// Password to connect the FTP/SFTP server
    /// </summary>
    private readonly string _password;

    /// <summary>
    /// Certification file full path
    /// </summary>
    private readonly string? _certFile;

    #endregion

    #region Delegates

    // #N/A

    #endregion

    #endregion

    #region Enumerations

    // #N/A

    #endregion

    #region Services

    // #N/A

    #endregion

    #region Accessors/Properties

    #region Variables

    // #N/A

    #endregion

    #region Delegates

    // #N/A

    #endregion

    #endregion

    #region Constructors

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="host">Host name (or ip) of the FTP/SFTP server</param>
    /// <param name="username">[Can be null] Username to connect the FTP/SFTP server</param>
    /// <param name="password">[Can be null] Password to connect the FTP/SFTP server</param>
    /// <param name="port">Port of the FTP/SFTP server [Default: 21]</param>
    /// <param name="isUsernameSecured">Is the username encrypted? [Default: true]</param>
    /// <param name="isPasswordSecured">Is the password encrypted? [Default: true]</param>
    /// <param name="isSftp">Is the server an SFTP one? [Default: false]</param>
    /// <param name="certFile">[Can be null] Certification file full path</param>
    public FtpService(string host, string? username, string? password, int port = 21, bool isUsernameSecured = true,
        bool isPasswordSecured = true, bool isSftp = false, string? certFile = null)
    {
        // Setting vars
        _host = host;
        _port = port;
        ICryptographyService svcCryptography = new Base64CryptographyService(Base64CryptographyService.Base64Tables.Utf8);
        _username = isUsernameSecured 
            ? svcCryptography.Decrypt(username ?? throw new ArgumentNullException(nameof(username), "In secure mode username cannot be null"))
            : username ?? string.Empty;
        _password = isPasswordSecured
            ? svcCryptography.Decrypt(password ?? throw new ArgumentNullException(nameof(password), "In secure mode password cannot be null"))
            : password ?? string.Empty;
        _secured = isSftp;
        _certFile = certFile;
    }

    #endregion

    #region Destructors

    // #N/A

    #endregion

    #region Methods

    #region Privates

    /// <summary>
    /// Method used to get requester
    /// </summary>
    /// <typeparam name="T">Type of requester [Must be one of these types: FtpWebRequest or SftpClient]</typeparam>
    /// <param name="remoteFolder">[Can be null] Remote folder where to work on the server</param>
    /// <returns>
    /// [T] A FTP/SFTP request
    /// </returns>
    /// <exception cref="InvalidCastException">If the T type is not one of these: FtpWebRequest or SftClient</exception>
    private T GetRequester<T>(string? remoteFolder = null)
    {
        if (typeof(T) == typeof(FtpWebRequest))
        {
            FtpWebRequest requester = (FtpWebRequest)WebRequest.Create($"{_host}:{_port}/{remoteFolder}");
            requester.Credentials = string.IsNullOrWhiteSpace(_username)
                ? new NetworkCredential()
                : new NetworkCredential(_username, _password);
            requester.KeepAlive = false;
            return (T) Convert.ChangeType(requester, typeof(T));
        }
        else if (typeof(T) == typeof(SftpClient))
        {
            PasswordAuthenticationMethod pwd = new PasswordAuthenticationMethod(_username, _password);
            PrivateKeyAuthenticationMethod cert =
                new PrivateKeyAuthenticationMethod(_username, new PrivateKeyFile(_certFile, _password));
            ConnectionInfo ci = new ConnectionInfo(_host, _port, _username, pwd, cert);
            return (T)Convert.ChangeType(new SftpClient(ci), typeof(T));
        }
        else
            throw new InvalidCastException("Type of requester must be FtpWebRequest or SftpClient");
    }

    #endregion

    #region Publics

    /// <summary>
    /// Method used to download a file from remote server
    /// </summary>
    /// <param name="localPath">Full path with filename where to store the file</param>
    /// <param name="remoteFolder">Path where file to download is stored</param>
    /// <param name="remoteFile">Filename of the file to download</param>
    /// <param name="bufferSize">Size of the buffer (in bytes))</param>
    /// <returns>
    /// [Task[bool]] An asynchronous task with result of bool
    /// True  ==> If everything is OK
    /// False ==> If the task is cancelled or faulted
    /// </returns>
    /// <exception cref="ArgumentNullException">If the local path is null or empty</exception>
    /// <exception cref="ArgumentNullException">If the remote folder is null or empty</exception>
    /// <exception cref="ArgumentNullException">If the remote filename is null or empty</exception>
    /// <exception cref="ArgumentNullException">If the task thrown an exception</exception>
    public async Task<bool> DownloadFile(string localPath, string remoteFolder, string remoteFile, uint bufferSize = 65536)
    {
        // Checks
        if (string.IsNullOrWhiteSpace(localPath)) throw new ArgumentNullException(nameof(localPath), $"The parameter \"{nameof(localPath)}\" value cannot be null or empty");
        if (string.IsNullOrWhiteSpace(remoteFolder)) throw new ArgumentNullException(nameof(remoteFolder), $"The parameter \"{nameof(remoteFolder)}\" value cannot be null or empty");
        if (string.IsNullOrWhiteSpace(remoteFile)) throw new ArgumentNullException(nameof(remoteFile), $"The parameter \"{nameof(remoteFile)}\" value cannot be null or empty");

        // Preparing task
        Task download;
        if (_secured)
        {
            SftpClient requester = GetRequester<SftpClient>();
            download = new Task(() =>
            {
                requester.Connect();
                requester.BufferSize = bufferSize * 1024;
                requester.DownloadFile(
                    $"{(remoteFolder.EndsWith('/') ? remoteFolder : $"{remoteFolder}/")}{remoteFile}",
                    File.OpenWrite(localPath));
                requester.Disconnect();
            });
        }
        else
        {
            FtpWebRequest requester = GetRequester<FtpWebRequest>($"{remoteFolder}/{remoteFile}");
            requester.Method = WebRequestMethods.Ftp.DownloadFile;
            download = new Task(() =>
            {
                using Stream reader = (requester.GetResponse()).GetResponseStream();
                FileStream file = File.Create(localPath);
                byte[] buffer = new byte[bufferSize];
                int read;
                while ((read = reader.Read(buffer, 0, buffer.Length)) > 0)
                    file.Write(buffer, 0, read);
                file.Close();
                reader.Close();
            });
        }

        // Starting task
        download.Start();
        await download;
        if (download.Exception is not null)
            throw download.Exception;

        // Function's return
        return !download.IsCanceled && !download.IsFaulted;
    }

    /// <summary>
    /// Method used to upload a file to a remote server
    /// </summary>
    /// <param name="localPath">Full path with filename where the file is stored</param>
    /// <param name="remoteFolder">Path where file to upload must be uploaded</param>
    /// <returns>
    /// [Task[bool]] An asynchronous task with result of bool
    /// True  ==> If everything is OK
    /// False ==> If the task is cancelled or faulted
    /// </returns>
    /// <exception cref="ArgumentNullException">If the local path is null or empty</exception>
    /// <exception cref="ArgumentNullException">If the remote folder is null or empty</exception>
    /// <exception cref="ArgumentNullException">If the task thrown an exception</exception>
    public async Task<bool> UploadFile(string localPath, string remoteFolder)
    {
        // Checks
        if (string.IsNullOrWhiteSpace(localPath))
            throw new ArgumentNullException(nameof(localPath), $"The parameter \"{nameof(localPath)}\" value cannot be null or empty");
        if (string.IsNullOrWhiteSpace(remoteFolder))
            throw new ArgumentNullException(nameof(remoteFolder), $"The parameter \"{nameof(remoteFolder)}\" value cannot be null or empty");

        // Preparing task
        Task upload;
        if (_secured)
        {
            SftpClient requester = GetRequester<SftpClient>();
            upload = new Task(() =>
            {
                requester.Connect();
                requester.ChangeDirectory(remoteFolder);
                requester.UploadFile(File.OpenRead(localPath.Replace('/', '\\')), Path.GetFileName(localPath.Replace('/', '\\')));
                requester.Disconnect();
            });
        }
        else
        {
            FtpWebRequest requester = GetRequester<FtpWebRequest>();
            requester.Method = WebRequestMethods.File.UploadFile;
            byte[] bFile = await File.ReadAllBytesAsync(localPath.Replace('/', '\\'));
            upload = (await requester.GetRequestStreamAsync()).WriteAsync(bFile, 0, bFile.Length);
        }

        // Starting task
        upload.Start();
        await upload;
        if (upload.Exception is not null)
            throw upload.Exception;

        // Function's return
        return !upload.IsCanceled && !upload.IsFaulted;
    }

    #endregion

    #endregion

    #region Overrides

    #region Methods

    // #N/A

    #endregion

    #region Operators

    // #N/A

    #endregion

    #endregion
}
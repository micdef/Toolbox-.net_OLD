// Using
// #N/A

// Static using
// #N/A

// Named using
// #N/A

// Namespace
namespace Toolbox.Ftp;

/// <summary>
/// Service used to manage FTP download and upload
/// </summary>
public interface IFtpService
{
    #region Enumerations

    // #N/A

    #endregion

    #region Accessors/Properties

    #region Members

    // #N/A

    #endregion

    #region Delegates

    // #N/A

    #endregion

    #endregion

    #region Methods

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
    Task<bool> DownloadFile(string localPath, string remoteFolder, string remoteFile, uint bufferSize = 65536);

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
    Task<bool> UploadFile(string localPath, string remoteFolder);

    #endregion
}
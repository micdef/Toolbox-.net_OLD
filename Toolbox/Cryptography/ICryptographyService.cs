// Using
// #N/A

// Static using
// #N/A

// Named using
// #N/A

// Namespace
namespace Toolbox.Cryptography;

/// <summary>
/// Cryptography service
/// </summary>
public interface ICryptographyService
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
    /// Method used to encrypt text (Asynchronous)
    /// </summary>
    /// <param name="text">Text to encrypt</param>
    /// <returns>
    /// Encrypted text
    /// </returns>
    Task<string> EncryptAsync(string text);

    /// <summary>
    /// Method used to encrypt text
    /// </summary>
    /// <param name="text">Text to encrypt</param>
    /// <returns>
    /// Encrypted text
    /// </returns>
    string Encrypt(string text);

    /// <summary>
    /// Method used to decrypt text (Asynchronous)
    /// </summary>
    /// <param name="text">Text to decrypt</param>
    /// <returns>
    /// Decrypted text
    /// </returns>
    Task<string> DecryptAsync(string text);

    /// <summary>
    /// Method used to decrypt text
    /// </summary>
    /// <param name="text">Text to decrypt</param>
    /// <returns>
    /// Decrypted text
    /// </returns>
    string Decrypt(string text);

    #endregion
}
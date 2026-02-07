// Using
using System.Text;
using Toolbox.Configuration;

// Static using
// #N/A

// Named using
// #N/A

// Namespace
namespace Toolbox.Cryptography;

/// <summary>
/// Base 64 cryptography service
/// </summary>
public class Base64CryptographyService : ICryptographyService
{
    #region Members

    #region Constants

    // #N/A

    #endregion

    #region Variables

    /// <summary>
    /// Encoding table used for crypting/decrypting
    /// </summary>
    private readonly Base64Tables _encodingTable;

    #endregion

    #region Delegates

    // #N/A

    #endregion

    #endregion

    #region Enumerations

    /// <summary>
    /// Encoding tables for Base64 conversion
    /// </summary>
    public enum Base64Tables
    {
        /// <summary>
        /// Default system encoding
        /// </summary>
        Default,

        /// <summary>
        /// Latin-1 (ISO-8859-1) encoding
        /// </summary>
        Latin1,

        /// <summary>
        /// Unicode (UTF-16 Little Endian) encoding
        /// </summary>
        Unicode,

        /// <summary>
        /// Unicode (UTF-16 Big Endian) encoding
        /// </summary>
        BigEndianUnicode,

        /// <summary>
        /// UTF-8 encoding
        /// </summary>
        Utf8,

        /// <summary>
        /// UTF-32 encoding
        /// </summary>
        Utf32,

        /// <summary>
        /// ASCII encoding
        /// </summary>
        Ascii
    }

    #endregion

    #region Services

    // #N/A

    #endregion

    #region Accessors/Properties

    #region Members & Properties

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
    /// <param name="svcConfiguration">Configuration service</param>
    public Base64CryptographyService(IConfigurationService svcConfiguration)
    {
        _encodingTable = svcConfiguration.GetValue(new List<string> { "Application", "Security", "Base64", "EncodingTable" }).ToEnum<Base64Tables>();
    }
    
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="encodingTable">Encoding table used for crypting/decrypting</param>
    public Base64CryptographyService(Base64Tables encodingTable)
    {
        _encodingTable = encodingTable;
    }

    #endregion

    #region Destructors

    // #N/A

    #endregion

    #region Methods

    #region Privates

    // #N/A

    #endregion

    #region Publics

    /// <summary>
    /// Method used to encrypt text (Asynchronous)
    /// </summary>
    /// <param name="text">Text to encrypt</param>
    /// <returns>
    /// Encrypted text
    /// </returns>
    public async Task<string> EncryptAsync(string text)
    {
        // Creating task
        Task<string> t = new Task<string>(() => Encrypt(text));

        // Executing task + Function's return
        t.Start();
        return await t;
    }

    /// <summary>
    /// Method used to encrypt text
    /// </summary>
    /// <param name="text">Text to encrypt</param>
    /// <returns>
    /// Encrypted text
    /// </returns>
    public string Encrypt(string text)
    {
        // Encrypt + Function's return
        switch (_encodingTable)
        {
            case Base64Tables.Ascii:
                return Convert.ToBase64String(Encoding.ASCII.GetBytes(text));

            case Base64Tables.BigEndianUnicode:
                return Convert.ToBase64String(Encoding.BigEndianUnicode.GetBytes(text));

            case Base64Tables.Latin1:
                return Convert.ToBase64String(Encoding.Latin1.GetBytes(text));

            case Base64Tables.Unicode:
                return Convert.ToBase64String(Encoding.Unicode.GetBytes(text));

            case Base64Tables.Utf8:
                return Convert.ToBase64String(Encoding.UTF8.GetBytes(text));

            case Base64Tables.Utf32:
                return Convert.ToBase64String(Encoding.UTF32.GetBytes(text));

            default:
                return Convert.ToBase64String(Encoding.Default.GetBytes(text));
        }
    }

    /// <summary>
    /// Method used to decrypt text (Asynchronous)
    /// </summary>
    /// <param name="text">Text to decrypt</param>
    /// <returns>
    /// Decrypted text
    /// </returns>
    public async Task<string> DecryptAsync(string text)
    {
        // Creating task
        Task<string> t = new Task<string>(() => Decrypt(text));

        // Executing task + Function's return
        t.Start();
        return await t;
    }

    /// <summary>
    /// Method used to decrypt text
    /// </summary>
    /// <param name="text">Text to decrypt</param>
    /// <returns>
    /// Decrypted text
    /// </returns>
    public string Decrypt(string text)
    {
        // Decrypt + Function's return
        switch (_encodingTable)
        {
            case Base64Tables.Ascii:
                return Encoding.ASCII.GetString(Convert.FromBase64String(text));

            case Base64Tables.BigEndianUnicode:
                return Encoding.BigEndianUnicode.GetString(Convert.FromBase64String(text));

            case Base64Tables.Latin1:
                return Encoding.Latin1.GetString(Convert.FromBase64String(text));

            case Base64Tables.Unicode:
                return Encoding.Unicode.GetString(Convert.FromBase64String(text));

            case Base64Tables.Utf8:
                return Encoding.UTF8.GetString(Convert.FromBase64String(text));

            case Base64Tables.Utf32:
                return Encoding.UTF32.GetString(Convert.FromBase64String(text));

            default:
                return Encoding.Default.GetString(Convert.FromBase64String(text));
        }
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
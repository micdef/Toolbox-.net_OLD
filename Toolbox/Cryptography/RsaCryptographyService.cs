//Using
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;

// Static using
// #N/A

// Named using
// #N/A

// Namespace
namespace Toolbox.Cryptography;

/// <summary>
/// RSA cryptography service
/// </summary>
public class RsaCryptographyService : ICryptographyService
{
    #region Members

    #region Constants

    // #N/A

    #endregion

    #region Variables

    /// <summary>
    /// Padding used for encryption/decryption
    /// </summary>
    private readonly RSAEncryptionPadding _padding;

    /// <summary>
    /// Cryptography service
    /// </summary>
    private readonly RSA _rsa;

    #endregion

    #region Delegates

    // #N/A

    #endregion

    #endregion

    #region Enumerations

    /// <summary>
    /// RSA key sizes in bits
    /// </summary>
    public enum KeySizes
    {
        /// <summary>
        /// 512-bit key size (weak, not recommended)
        /// </summary>
        S512 = 512,

        /// <summary>
        /// 1024-bit key size (deprecated)
        /// </summary>
        S1024 = 1024,

        /// <summary>
        /// 2048-bit key size (minimum recommended)
        /// </summary>
        S2048 = 2048,

        /// <summary>
        /// 4096-bit key size (strong)
        /// </summary>
        S4096 = 4096,

        /// <summary>
        /// 8192-bit key size (very strong)
        /// </summary>
        S8192 = 8192,

        /// <summary>
        /// 16384-bit key size (maximum strength)
        /// </summary>
        S16384 = 16384
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
    /// <param name="privateKey">Private key</param>
    /// <param name="publicKey">Public key</param>
    /// <param name="padding">Padding used for encryption/decryption</param>
    public RsaCryptographyService(byte[] privateKey, byte[] publicKey, RSAEncryptionPadding padding)
    {
        _padding = padding;
        _rsa = RSA.Create();
        _rsa.ImportRSAPrivateKey(privateKey, out _);
        _rsa.ImportRSAPublicKey(publicKey, out _);
    }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="cert">Certificate</param>
    /// <param name="padding">Padding used for encryption/decryption</param>
    public RsaCryptographyService(X509Certificate2 cert, RSAEncryptionPadding padding)
    {
        _padding = padding;
        _rsa = cert.HasPrivateKey ? cert.GetRSAPrivateKey()! : cert.GetRSAPublicKey()!;
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
        // Function's return
        return Convert.ToBase64String(_rsa.Encrypt(Encoding.Unicode.GetBytes(text), _padding));
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
        // Function's return
        return Encoding.Unicode.GetString(_rsa.Decrypt(Convert.FromBase64String(text), _padding));
    }
    
    /// <summary>
    /// Static method used to generate RSA keys
    /// </summary>
    /// <param name="keySize">Size of the key</param>
    /// <returns>
    /// A tuple of two elements
    /// --> 1st: Byte[] Public Key
    /// --> 2nd: Byte[] Private Key
    /// </returns>
    public static Tuple<byte[], byte[]> GenerateKeys(KeySizes keySize)
    {
        // Creating service
        RSA svcCrypto = RSA.Create((int)keySize);

        // Generating keys
        byte[] privateKey = svcCrypto.ExportRSAPrivateKey();
        byte[] publicKey = svcCrypto.ExportRSAPublicKey();
        
        // Function's return
        return new Tuple<byte[], byte[]>(publicKey, privateKey);
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
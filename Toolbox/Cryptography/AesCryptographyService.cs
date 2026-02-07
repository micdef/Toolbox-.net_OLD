//Using
using System.Security.Cryptography;
using System.Text;
using Toolbox.Configuration;

// Static using
// #N/A

// Named using
// #N/A

// Namespace
namespace Toolbox.Cryptography;

/// <summary>
/// AES cryptography service
/// </summary>
public class AesCryptographyService : ICryptographyService
{
    #region Members

    #region Constants

    // #N/A

    #endregion

    #region Variables

    /// <summary>
    /// AES cryptography holder
    /// </summary>
    private readonly Aes _aes;

    #endregion

    #region Delegates

    // #N/A

    #endregion

    #endregion

    #region Enumerations

    /// <summary>
    /// Possible AES key sizes in bits
    /// </summary>
    public enum KeySizes
    {
        /// <summary>
        /// 128-bit key size
        /// </summary>
        S128 = 128,

        /// <summary>
        /// 196-bit key size
        /// </summary>
        S196 = 196,

        /// <summary>
        /// 256-bit key size (strongest)
        /// </summary>
        S256 = 256
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
    public AesCryptographyService(IConfigurationService svcConfiguration)
    {
        _aes = Aes.Create();
        _aes.Key = File.ReadAllBytes(svcConfiguration.GetValue(new List<string> { "Application", "Security", "Aes", "Key" }));
        _aes.IV = File.ReadAllBytes(svcConfiguration.GetValue(new List<string> { "Application", "Security", "Aes", "Iv" }));
        _aes.Padding = PaddingMode.PKCS7;
        _aes.Mode = CipherMode.CBC;
    }
    
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="key">Aes Key</param>
    /// <param name="iv">Aes Initialization Vector</param>
    public AesCryptographyService(byte[] key, byte[] iv)
    {
        _aes = Aes.Create();
        _aes.Key = key;
        _aes.IV = iv;
        _aes.Padding = PaddingMode.PKCS7;
        _aes.Mode = CipherMode.CBC;
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
        // Encryption
        await using MemoryStream ms = new MemoryStream();
        await using CryptoStream cs = new CryptoStream(ms, _aes.CreateEncryptor(), CryptoStreamMode.Write);
        byte[] clearBytes = Encoding.Unicode.GetBytes(text);
        await cs.WriteAsync(clearBytes, 0, clearBytes.Length);
        cs.Close();

        // Function's return
        return Convert.ToBase64String(ms.ToArray());
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
        return EncryptAsync(text).Result;
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
        // Decryption
        await using MemoryStream ms = new MemoryStream(Convert.FromBase64String(text));
        await using CryptoStream cs = new CryptoStream(ms, _aes.CreateDecryptor(_aes.Key, _aes.IV), CryptoStreamMode.Read);
        using BinaryReader sr = new BinaryReader(cs);
        StringBuilder decryptedText = new StringBuilder();
        bool endWhile = false;
        while (!endWhile)
        {
            try
            {
                byte b = sr.ReadByte();
                if (b != 0)
                    decryptedText.Append(Convert.ToChar(b));
                endWhile = false;
            }
            catch (Exception e)
            {
                endWhile = true;
            }
        }
        
        // Function's return
        return decryptedText.ToString();
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
        return DecryptAsync(text).Result;
    }

    /// <summary>
    /// Static method used to generate AES keys
    /// </summary>
    /// <param name="keySize">Size of the key</param>
    /// <returns>
    /// A tuple of two elements
    /// --> 1st: Byte[] Key generated
    /// --> 2nd: Byte[] Vector (IV) generated
    /// </returns>
    public static Tuple<byte[], byte[]> GenerateKeys(KeySizes keySize)
    {
        // Creating service
        Aes aes = Aes.Create();
        aes.KeySize = (int)keySize;

        // Generating keys
        aes.GenerateKey();
        aes.GenerateIV();

        // Function's return
        return new Tuple<byte[], byte[]>(aes.Key, aes.IV);
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
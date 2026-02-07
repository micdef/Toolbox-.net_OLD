// Using
using Newtonsoft.Json;
using System.Text;
using Toolbox.Cryptography;

// Static using
// #N/A

// Named using
// #N/A

// Namespace
namespace Toolbox;

/// <summary>
/// Extensions class
/// </summary>
public static class Extensers
{
    #region Members

    #region Constants

    // #N/A

    #endregion

    #region Variables

    // #N/A

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

    #region Members & Properties

    // #N/A

    #endregion

    #region Delegates

    // #N/A

    #endregion

    #endregion

    #region Constructors

    // #N/A

    #endregion

    #region Destructors

    // #N/A

    #endregion

    #region Methods

    #region Privates

    // #N/A

    #endregion

    #region Publics

    #region Strings

    /// <summary>
    /// Method used to check if a string is null, empty or just white spaces
    /// </summary>
    /// <param name="value">[Can be null] String to check</param>
    /// <returns>
    /// True  ==> If the string is null, empty or just white spaces
    /// False ==> If the string contains at least one character
    /// </returns>
    public static bool IsStringEmpty(this string? value)
    {
        // Function's return
        return string.IsNullOrEmpty(value) || string.IsNullOrWhiteSpace(value);
    }

    /// <summary>
    /// Method used to transform ORACLE database date string to DateOnly object
    /// </summary>
    /// <param name="value">Value from ORACLE database</param>
    /// <param name="bamboo">Value from BAMBOO database?</param>
    /// <returns>
    /// The DateOnly object with all information
    /// </returns>
    /// <exception cref="ArgumentNullException">If the value is null or empty</exception>
    /// <exception cref="ArgumentOutOfRangeException">If the value's length is not exactly 8 characters</exception>
    /// <exception cref="InvalidCastException">If one digit of the value is not a number</exception>
    public static DateOnly ToDateOnly(this string value, bool bamboo = false)
    {
        // Check
        if (value.IsStringEmpty())
            throw new ArgumentNullException(nameof(value), $"The value of the parameter: {nameof(value)} cannot be null or empty");
        if (value.Length != (bamboo ? 10 : 8))
            throw new ArgumentOutOfRangeException(nameof(value), $"The value's length of the parameter: {nameof(value)} must have {(bamboo ? "10" : "8")} character");
        if (!bamboo && value.Any(c => !int.TryParse(c.ToString(), out _)))
            throw new InvalidCastException($"All characters of the parameter:{nameof(value)} must be a number between 0 and 9");
        
        // Transforming data
        int year = 0;
        int month = 0;
        int day = 0;
        if (bamboo)
        {
            char splitChar = value.Contains('-') ? '-' : '/';
            year = int.Parse(value.Split(splitChar)[2]);
            month = int.Parse(value.Split(splitChar)[1]);
            day = int.Parse(value.Split(splitChar)[0]);
        }
        else
        {
            year = int.Parse(value.Substring(0, 4));
            month = int.Parse(value.Substring(4, 2));
            day = int.Parse(value.Substring(6, 2));
        }
        
        // Function's return
        return new DateOnly(year, month, day);
    }

    /// <summary>
    /// Method used to transform ORACLE database time string to TimeOnly object
    /// </summary>
    /// <param name="value">Value from ORACLE database</param>
    /// <param name="bamboo">Value from BAMBOO database?</param>
    /// <returns>
    /// The TimeOnly object with all information
    /// </returns>
    /// <exception cref="ArgumentNullException">If the value is null or empty</exception>
    /// <exception cref="ArgumentOutOfRangeException">If the value's length is not exactly 4 characters</exception>
    /// <exception cref="InvalidCastException">If one digit of the value is not a number</exception>
    public static TimeOnly ToTimeOnly(this string value, bool bamboo = false)
    {
        // Check
        if (value.IsStringEmpty())
            throw new ArgumentNullException(nameof(value), $"The value of the parameter: {nameof(value)} cannot be null or empty");
        if (!bamboo && value.Length != 4)
            throw new ArgumentOutOfRangeException(nameof(value), $"The value's length of the parameter: {nameof(value)} must have 4 character");
        if (bamboo && value.Length >= 3 && value.Length <= 4)
            throw new ArgumentOutOfRangeException(nameof(value), $"The value's length of the parameter: {nameof(value)} must be between 3 and 4 characters long");
        if (value.Any(c => !int.TryParse(c.ToString(), out _)))
            throw new InvalidCastException($"All characters of the parameter: {nameof(value)} must be a number between 0 and 9");
        
        // Transforming data
        int hours = 0;
        int minutes = 0;
        if (!bamboo)
        {
            hours = int.Parse(value.Substring(0, value.Length == 3 ? 1 : 2));
            minutes = int.Parse(value.Substring(value.Length == 3 ? 1 : 2, 2));
        }
        else
        {
            hours = int.Parse(value.Substring(0, 2));
            minutes = int.Parse(value.Substring(2, 2));
        }
        
        // Function's return
        return new TimeOnly(hours, minutes, 0);
    }
    
    /// <summary>
    /// Method used to parse an enumeration value
    /// </summary>
    /// <param name="value">Value to parse</param>
    /// <typeparam name="T">Type of enumeration</typeparam>
    /// <returns>
    /// Enumeration value
    /// </returns>
    /// <exception cref="ArgumentNullException">If the value to parse is null or empty</exception>
    /// <exception cref="InvalidCastException">If the value cannot be parsed</exception>
    public static T ToEnum<T>(this string value)
    {
        // Checks
        if (value.IsStringEmpty())
            throw new ArgumentNullException(nameof(value), $"The value of the parameter: {nameof(value)} cannot be null or empty");
            
        // Parse to enum
        T el = (T)Enum.Parse(typeof(T), value);
        if (!Enum.IsDefined(typeof(T), el!))
            throw new InvalidCastException($"The value of the parameter: {nameof(value)} cannot be parsed into a enum value of {typeof(T)}");
            
        // Function's return
        return el;
    }

    /// <summary>
    /// Method used to extract number from a string
    /// </summary>
    /// <param name="value">Value with numbers to extract</param>
    /// <returns>
    /// All numbers of the string (in order)
    /// </returns>
    /// <exception cref="ArgumentNullException">If the value is null or empty</exception>
    public static string ExtractNumbers(this string value)
    {
        // Checks
        if (value.IsStringEmpty())
            throw new ArgumentNullException(nameof(value), $"The value of the parameter: {nameof(value)} cannot be null or empty");
        
        // Transforming data
        StringBuilder numberString = new StringBuilder();
        foreach (char c in value)
            switch (c)
            {
                case '0':
                case '1':
                case '2':
                case '3':
                case '4':
                case '5':
                case '6':
                case '7':
                case '8':
                case '9':
                    numberString.Append(c);
                    break;
            }
        
        // Function's return
        return numberString.ToString();
    }

    /// <summary>
    /// Method used to check if a number is a Belgium NISS
    /// </summary>
    /// <param name="niss">Number to check</param>
    /// <returns>
    /// If the number is a Belgium NISS
    /// </returns>
    /// <exception cref="ArgumentNullException">If the number to check is null or empty</exception>
    /// <exception cref="ArgumentOutOfRangeException">If the extracted number have not a length of 11 characters exactly</exception>
    public static bool IsBelgiumNiss(this string niss)
    {
        // Checks
        if (niss.IsStringEmpty())
            throw new ArgumentNullException(nameof(niss), $"The value of the parameter: {nameof(niss)} cannot be null or empty");
        
        // Getting numbers + checks
        string nissNumbers = niss.ExtractNumbers();
        if (nissNumbers.Length != 11)
            throw new ArgumentOutOfRangeException($"The niss number must contains 11 characters, no more, no less");
        
        // Get Datas
        uint control = uint.Parse(nissNumbers.Substring(9, 2));
        uint body19 = uint.Parse(nissNumbers.Substring(0, 9));
        uint check19 = 97 - (body19 % 97);
        ulong body20 = ulong.Parse($"2{nissNumbers.Substring(0, 9)}");
        ulong check20 = 97 - (body20 % 97);
        
        // Function's return
        return check19 == control || check20 == control;
    }

    /// <summary>
    /// Method used to decrypt a text
    /// </summary>
    /// <param name="text">Text to decrypt</param>
    /// <param name="svcCryptography">Cryptography service</param>
    /// <returns>
    /// Decrypted element
    /// </returns>
    /// <exception cref="ArgumentNullException">If the text to decrypt is null or empty</exception>
    public static string Decrypt(this string text, ICryptographyService svcCryptography)
    {
        // Checks
        if (text.IsStringEmpty())
            throw new ArgumentNullException(nameof(text), $"The value of the parameter: {nameof(text)} cannot be null or empty");
        
        // Function's return
        return svcCryptography.Decrypt(text);
    }

    /// <summary>
    /// Method used to decrypt a text
    /// </summary>
    /// <param name="text">Text to decrypt</param>
    /// <param name="svcCryptography">Cryptography service</param>
    /// <typeparam name="T">Type of decrypted element</typeparam>
    /// <returns>
    /// Decrypted element
    /// </returns>
    /// <exception cref="ArgumentNullException">If the text to decrypt is null or empty</exception>
    /// <exception cref="InvalidCastException">If the decrypted text cannot be casted in desired type</exception>
    public static T Decrypt<T>(this string text, ICryptographyService svcCryptography)
    {
        // Checks
        if (text.IsStringEmpty())
            throw new ArgumentNullException(nameof(text), $"The value of the parameter: {nameof(text)} cannot be null or empty");
        
        // Decrypt information
        string decrypted = svcCryptography.Decrypt(text);
        T? element = JsonConvert.DeserializeObject<T>(decrypted);
        if (element is null)
            throw new InvalidCastException($"The encrypted element is not json of type: {typeof(T)}");
        
        // Function's return
        return element;
    }

    /// <summary>
    /// [Async]
    /// Method used to decrypt a text
    /// </summary>
    /// <param name="text">Text to decrypt</param>
    /// <param name="svcCryptography">Cryptography service</param>
    /// <returns>
    /// Decrypted element
    /// </returns>
    /// <exception cref="ArgumentNullException">If the text to decrypt is null or empty</exception>
    public static async Task<string> DecryptAsync(this string text, ICryptographyService svcCryptography)
    {
        // Checks
        if (text.IsStringEmpty())
            throw new ArgumentNullException(nameof(text), $"The value of the parameter: {nameof(text)} cannot be null or empty");
        
        // Function's return
        return await svcCryptography.DecryptAsync(text);
    }

    /// <summary>
    /// [Async]
    /// Method used to decrypt a text
    /// </summary>
    /// <param name="text">Text to decrypt</param>
    /// <param name="svcCryptography">Cryptography service</param>
    /// <typeparam name="T">Type of decrypted element</typeparam>
    /// <returns>
    /// Decrypted element
    /// </returns>
    /// <exception cref="ArgumentNullException">If the text to decrypt is null or empty</exception>
    /// <exception cref="InvalidCastException">If the decrypted text cannot be casted in desired type</exception>
    public static async Task<T> DecryptAsync<T>(this string text, ICryptographyService svcCryptography)
    {
        // Checks
        if (text.IsStringEmpty())
            throw new ArgumentNullException(nameof(text), $"The value of the parameter: {nameof(text)} cannot be null or empty");
        
        // Decrypt information
        string decrypted = await svcCryptography.DecryptAsync(text);
        T? element = JsonConvert.DeserializeObject<T>(decrypted);
        if (element is null)
            throw new InvalidCastException($"The encrypted element is not json of type: {typeof(T)}");
        
        // Function's return
        return element;
    }

    #endregion

    #region Generic

    /// <summary>
    /// Method used to encrypt text
    /// </summary>
    /// <param name="text">Text to encrypt</param>
    /// <param name="svcCryptography">Cryptography Service</param>
    /// <returns>
    /// Encrypted string
    /// </returns>
    /// <exception cref="ArgumentNullException">If the text to encrypt is null or empty</exception>
    public static string Encrypt(this string text, ICryptographyService svcCryptography)
    {
        // Checks
        if (text.IsStringEmpty())
            throw new ArgumentNullException(nameof(text), $"The value of the parameter: {nameof(text)} cannot be null or empty");
        
        // Function's return
        return svcCryptography.Encrypt(text);
    }

    /// <summary>
    /// Method used to encrypt object
    /// </summary>
    /// <param name="element">Object to encrypt</param>
    /// <param name="svcCryptography">Cryptography service</param>
    /// <typeparam name="T">Type of object to encrypt</typeparam>
    /// <returns>
    /// Encrypted string
    /// </returns>
    public static string Encrypt<T>(this T element, ICryptographyService svcCryptography)
    {
        // Function's return
        return svcCryptography.Encrypt(JsonConvert.SerializeObject(element));
    }

    /// <summary>
    /// [Async]
    /// Method used to encrypt text
    /// </summary>
    /// <param name="text">Text to encrypt</param>
    /// <param name="svcCryptography">Cryptography Service</param>
    /// <returns>
    /// Encrypted string
    /// </returns>
    /// <exception cref="ArgumentNullException">If the text to encrypt is null or empty</exception>
    public static async Task<string> EncryptAsync(this string text, ICryptographyService svcCryptography)
    {
        // Checks
        if (text.IsStringEmpty())
            throw new ArgumentNullException(nameof(text), $"The value of the parameter: {nameof(text)} cannot be null or empty");
        
        // Function's return
        return await svcCryptography.EncryptAsync(text);
    }

    /// <summary>
    /// [Async]
    /// Method used to encrypt object
    /// </summary>
    /// <param name="element">Object to encrypt</param>
    /// <param name="svcCryptography">Cryptography service</param>
    /// <typeparam name="T">Type of object to encrypt</typeparam>
    /// <returns>
    /// Encrypted string
    /// </returns>
    public static async Task<string> EncryptAsync<T>(this T element, ICryptographyService svcCryptography)
    {
        // Function's return
        return await svcCryptography.EncryptAsync(JsonConvert.SerializeObject(element));
    }

    #endregion

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
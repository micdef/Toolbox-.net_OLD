// Using
using Microsoft.Extensions.Configuration;

// Static using
// #N/A

// Named using
// #N/A

// Namespace
namespace Toolbox.Configuration;

/// <summary>
/// Service used to manage configuration file
/// </summary>
public interface IConfigurationService
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
    /// Method used to get an IConfigurationSection object
    /// </summary>
    /// <param name="nodes">List of nodes where the section is</param>
    /// <returns>
    /// The retrieved IConfigurationSection object
    /// </returns>
    /// <exception cref="ArgumentNullException">If the list 'nodes' is null, empty or one of its element is null or empty</exception>
    /// <exception cref="NullReferenceException">If the configuration cannot find the node in sections</exception>
    /// <exception cref="NullReferenceException">If the section is not found in the configuration</exception>
    IConfigurationSection GetSection(IEnumerable<string> nodes);

    /// <summary>
    /// Method used to get a connection string
    /// </summary>
    /// <param name="key">Key used to store connection string</param>
    /// <returns>
    /// The retrieved connection string
    /// </returns>
    /// <exception cref="ArgumentNullException">If the value of the 'key' is null or empty</exception>
    /// <exception cref="NullReferenceException">If the connection string cannot be found</exception>
    string GetConnectionString(string key);

    /// <summary>
    /// Method used to get a value (string)
    /// </summary>
    /// <param name="section">IConfigurationSection where value is stored</param>
    /// <returns>
    /// The value stored (String.Empty if null)
    /// </returns>
    string GetValue(IConfigurationSection section);

    /// <summary>
    /// Method used to get a value (object)
    /// </summary>
    /// <param name="section">IConfigurationSection where value is stored</param>
    /// <typeparam name="T">Object type to transform</typeparam>
    /// <returns>
    /// The object value retrieved (Default if null)
    /// </returns>
    T? GetValue<T>(IConfigurationSection section);

    /// <summary>
    /// Method used to get a value (string)
    /// </summary>
    /// <param name="nodes">List of nodes to find the section</param>
    /// <returns>
    /// The value retrieved (String.Empty if null
    /// </returns>
    /// <exception cref="ArgumentNullException">If the list 'nodes' is null, empty or one of its element is null or empty</exception>
    /// <exception cref="NullReferenceException">If the configuration cannot find the node in sections</exception>
    /// <exception cref="NullReferenceException">If the section is not found in the configuration</exception>
    string GetValue(IEnumerable<string> nodes);

    /// <summary>
    /// Method used to get a value (object)
    /// </summary>
    /// <param name="nodes">List of nodes to find the section</param>
    /// <typeparam name="T">Object type to transform</typeparam>
    /// <returns>
    /// The object value retrieved (Default if null)
    /// </returns>
    /// <exception cref="ArgumentNullException">If the list 'nodes' is null, empty or one of its element is null or empty</exception>
    /// <exception cref="NullReferenceException">If the configuration cannot find the node in sections</exception>
    /// <exception cref="NullReferenceException">If the section is not found in the configuration</exception>
    T? GetValue<T>(IEnumerable<string> nodes);

    #endregion
}
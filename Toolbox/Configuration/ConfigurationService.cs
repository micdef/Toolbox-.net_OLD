// Using
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Ini;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.FileProviders;

// Static using
// #N/A

// Named using
// #N/A

// Namespace
namespace Toolbox.Configuration;

/// <summary>
/// Service used to manage configuration file
/// </summary>
public class ConfigurationService : IConfigurationService
{
    #region Members

    #region Constants

    // #N/A

    #endregion

    #region Variables

    /// <summary>
    /// Configuration holder
    /// </summary>
    private readonly IConfiguration _configuration;

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

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="basePath">Where the configuration file is stored</param>
    /// <param name="filename">Configuration's filename</param>
    /// <param name="reloadOnChange">Do we have to reload the file in case of changes [Default: true]</param>
    /// <param name="isOptional">Is the configuration optional? [Default: false]</param>
    /// <param name="isIniFile">Is the configuration file an INI one? [Default: false]</param>
    /// <exception cref="ArgumentNullException">If the value of the 'basePath' is null or empty</exception>
    /// <exception cref="ArgumentNullException">If the value of the 'filename' is null or emtpy</exception>
    public ConfigurationService(string basePath, string filename, bool reloadOnChange = true, bool isOptional = false, bool isIniFile = false)
    {
        // Checks
        if (string.IsNullOrEmpty(basePath) || string.IsNullOrWhiteSpace(basePath)) throw new ArgumentNullException(nameof(basePath), $"The value of the parameter: {nameof(basePath)} cannot be null or empty");
        if (string.IsNullOrEmpty(filename) || string.IsNullOrWhiteSpace(filename)) throw new ArgumentNullException(nameof(filename), $"The value of the parameter: {nameof(filename)} cannot be null or empty");
        
        // Setting up configuration source
        FileConfigurationSource source = isIniFile ? new IniConfigurationSource() : new JsonConfigurationSource();
        source.Path = filename;
        source.FileProvider = new PhysicalFileProvider(basePath);
        source.ReloadOnChange = reloadOnChange;
        source.Optional = isOptional;
        
        // Build configuration
        IConfigurationBuilder builder = new ConfigurationBuilder();
        builder.Sources.Add(source);
        _configuration = builder.Build();
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
    /// Method used to get an IConfigurationSection object
    /// </summary>
    /// <param name="nodes">List of nodes where the section is</param>
    /// <returns>
    /// The retrieved IConfigurationSection object
    /// </returns>
    /// <exception cref="ArgumentNullException">If the list 'nodes' is null, empty or one of its element is null or empty</exception>
    /// <exception cref="NullReferenceException">If the configuration cannot find the node in sections</exception>
    /// <exception cref="NullReferenceException">If the section is not found in the configuration</exception>
    public IConfigurationSection GetSection(IEnumerable<string> nodes)
    {
        // Checks
        if (!nodes.Any() || nodes.Any(string.IsNullOrEmpty) || nodes.Any(string.IsNullOrWhiteSpace)) throw new ArgumentNullException(nameof(nodes), $"The value of the parameter: {nameof(nodes)} cannot be null, empty or none of its elements can be empty or null ");
        
        // Getting section
        IConfigurationSection? section = null;
        foreach (string node in nodes)
        {
            IConfigurationSection? newSection = section is null ? _configuration.GetSection(node) : section.GetSection(node);
            section = newSection ?? throw new NullReferenceException(section is null ? $"The section {node} doesn't exist in the configuration" : $"The section {node} doesn't exists in the section {section.Key} in the configuration");
        }
        
        // Function's return
        return section ?? throw new NullReferenceException("The desired section was not found in the configuration");
    }

    /// <summary>
    /// Method used to get a connection string
    /// </summary>
    /// <param name="key">Key used to store connection string</param>
    /// <returns>
    /// The retrieved connection string
    /// </returns>
    /// <exception cref="ArgumentNullException">If the value of the 'key' is null or empty</exception>
    /// <exception cref="NullReferenceException">If the connection string cannot be found</exception>
    public string GetConnectionString(string key)
    {
        // Checks
        if (string.IsNullOrEmpty(key) || string.IsNullOrWhiteSpace(key)) throw new ArgumentNullException(nameof(key), $"The value of the parameter: {nameof(key)} cannot be null or empty");
        
        // Getting connections string
        string connStr = _configuration.GetConnectionString(key) ?? string.Empty;
        if (string.IsNullOrEmpty(connStr) || string.IsNullOrWhiteSpace(connStr)) throw new NullReferenceException($"The desired connection string with key: {key} was not found in the configuration");
        
        // Function's return
        return connStr;
    }
    
    /// <summary>
    /// Method used to get a value (string)
    /// </summary>
    /// <param name="section">IConfigurationSection where value is stored</param>
    /// <returns>
    /// The value stored (String.Empty if null)
    /// </returns>
    public string GetValue(IConfigurationSection section)
    {
        // Function's return
        return section.Value ?? string.Empty;
    }

    /// <summary>
    /// Method used to get a value (object)
    /// </summary>
    /// <param name="section">IConfigurationSection where value is stored</param>
    /// <typeparam name="T">Object type to transform</typeparam>
    /// <returns>
    /// The object value retrieved (Default if null)
    /// </returns>
    public T? GetValue<T>(IConfigurationSection section)
    {
        // Getting value from configuration
        string sectionValue = section.Value ?? string.Empty;
        
        // Function's return
        return string.IsNullOrEmpty(sectionValue) || string.IsNullOrWhiteSpace(sectionValue) ? default : (T)Convert.ChangeType(sectionValue, typeof(T));
    }

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
    public string GetValue(IEnumerable<string> nodes)
    {
        // Function's return
        return GetSection(nodes).Value ?? string.Empty;
    }

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
    public T? GetValue<T>(IEnumerable<string> nodes)
    {
        // Getting value from configuration
        string sectionValue = GetSection(nodes).Value ?? string.Empty;
        
        // Function's return
        return string.IsNullOrEmpty(sectionValue) || string.IsNullOrWhiteSpace(sectionValue) ? default : (T)Convert.ChangeType(sectionValue, typeof(T));
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
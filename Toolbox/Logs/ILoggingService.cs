// Using
using Microsoft.Extensions.Logging;

// Static using
// #N/A

// Named using
// #N/A

// Namespace
namespace Toolbox.Logs;

/// <summary>
/// Service used to log events
/// </summary>
public interface ILoggingService
{
    #region Enumerations

    // #N/A

    #endregion

    #region Accessors/Properties

    #region Members

    /// <summary>
    /// List of available events
    /// </summary>
    List<EventId> Events { get; init; }

    #endregion

    #region Delegates

    // #N/A

    #endregion

    #endregion

    #region Methods

    /// <summary>
    /// Method used to load final information (in case of use of configuration file)
    /// </summary>
    /// <param name="logger">Logger used to log</param>
    /// <param name="moduleName">Module name</param>
    /// <typeparam name="T">Type of element of the logger</typeparam>
    void Load<T>(ILogger<T> logger, string moduleName);

    /// <summary>
    /// Method used to write log
    /// </summary>
    /// <param name="fctName">Name of transmitting function</param>
    /// <param name="level">Logging level</param>
    /// <param name="eventId">Event ID</param>
    /// <param name="message">Message emitted</param>
    /// <param name="ex">[Can be null] Exception occurred</param>
    /// <exception cref="ArgumentNullException">If the name of transmitting function is null or empty</exception>
    /// <exception cref="ArgumentNullException">If the message emitted is null or empty</exception>
    void WriteLog(string fctName, LogLevel level, EventId eventId, string message, Exception? ex = null);

    #endregion
}
// Using
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net.Mail;
using System.Text;
using Toolbox.Configuration;
using Toolbox.Mailing;

// Static using
// #N/A

// Named using
// #N/A

// Namespace
namespace Toolbox.Logs;

/// <summary>
/// Service used to log events
/// </summary>
public class LoggingService : ILoggingService
{
    #region Members

    #region Constants

    // #N/A

    #endregion

    #region Variables

    /// <summary>
    /// Application name
    /// </summary>
    private string _applicationName;

    /// <summary>
    /// Application development code
    /// </summary>
    private string _applicationCode;

    /// <summary>
    /// Application version
    /// </summary>
    private string _applicationVersion;

    /// <summary>
    /// Module name
    /// </summary>
    private string? _moduleName;

    /// <summary>
    /// Email address that send critical alerts
    /// </summary>
    private string _fromEmail;

    /// <summary>
    /// Email display that send critical alerts
    /// </summary>
    private string _fromDisplay;

    /// <summary>
    /// Emails addresses that receive critical alerts
    /// </summary>
    private IEnumerable<string> _to;

    /// <summary>
    /// Emails addresses that receive in cc critical alerts
    /// </summary>
    private IEnumerable<string> _cc;

    /// <summary>
    /// Emails addresses that receive in bcc critical alerts
    /// </summary>
    private IEnumerable<string> _bcc;

    /// <summary>
    /// Can the system log debug ones?
    /// </summary>
    private readonly bool _logDebug;

    /// <summary>
    /// Can the system log trace ones?
    /// </summary>
    private readonly bool _logTrace;

    #endregion

    #region Delegates

    // #N/A

    #endregion

    #endregion

    #region Enumerations

    // #N/A

    #endregion

    #region Services

    /// <summary>
    /// Logger used to log
    /// </summary>
    private ILogger? _logger;

    /// <summary>
    /// Mailing service
    /// </summary>
    private readonly IMailingService _svcMailing;
    
    #endregion

    #region Accessors/Properties

    #region Members & Properties

    /// <summary>
    /// List of available events
    /// </summary>
    public List<EventId> Events { get; init; }

    #endregion

    #region Delegates

    // #N/A

    #endregion

    #endregion

    #region Constructors

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="applicationName">Application name</param>
    /// <param name="applicationCode">Application code</param>
    /// <param name="applicationVersion">Application version</param>
    /// <param name="moduleName">Module name</param>
    /// <param name="logDebug">Can the system log debug ones?</param>
    /// <param name="logTrace">Can the system log trace ones?</param>
    /// <param name="fromEmail">Email address that send critical alerts</param>
    /// <param name="fromDisplay">Email display that send critical alerts</param>
    /// <param name="to">Emails addresses that receive critical alerts</param>
    /// <param name="cc">Emails addresses that receive in cc critical alerts</param>
    /// <param name="bcc">Emails addresses that receive in bcc critical alerts</param>
    /// <param name="logger">Logger used to log</param>
    /// <param name="events">List of available events</param>
    /// <param name="svcMailing">Mailing service</param>
    /// <exception cref="ArgumentNullException">If the application name is null or empty</exception>
    /// <exception cref="ArgumentNullException">If the application code is null or empty</exception>
    /// <exception cref="ArgumentNullException">If the application version is null or empty</exception>
    /// <exception cref="ArgumentNullException">If the module name is null or empeyt</exception>
    /// <exception cref="ArgumentNullException">If the list of event is null or empty</exception>
    public LoggingService(string applicationName, string applicationCode, string applicationVersion, string moduleName, bool logDebug, bool logTrace, string fromEmail,
        string fromDisplay, IEnumerable<string> to, IEnumerable<string> cc, IEnumerable<string> bcc, ILogger logger, List<EventId> events, IMailingService svcMailing)
    {
        // Checks
        if (string.IsNullOrEmpty(applicationName) || string.IsNullOrWhiteSpace(applicationName))
            throw new ArgumentNullException(nameof(applicationName), $"The value of the parameter: {nameof(applicationName)} cannot be null or empty");
        if (string.IsNullOrEmpty(applicationCode) || string.IsNullOrWhiteSpace(applicationCode))
            throw new ArgumentNullException(nameof(applicationCode), $"The value of the parameter: {nameof(applicationCode)} cannot be null or empty");
        if (string.IsNullOrEmpty(applicationVersion) || string.IsNullOrWhiteSpace(applicationVersion))
            throw new ArgumentNullException(nameof(applicationVersion), $"The value of the parameter: {nameof(applicationVersion)} cannot be null or empty");
        if (string.IsNullOrEmpty(moduleName) || string.IsNullOrWhiteSpace(moduleName))
            throw new ArgumentNullException(nameof(moduleName), $"The value of the parameter: {nameof(moduleName)} cannot be null or empty");
        if (!events.Any())
            throw new ArgumentNullException(nameof(events), $"The list: {nameof(events)} must contains at least 1 element");
        
        // Setting application info
        _applicationName = applicationName;
        _applicationCode = applicationCode;
        _applicationVersion = applicationVersion;
        _moduleName = moduleName;
        _logDebug = logDebug;
        _logTrace = logTrace;
        _to = to;
        _cc = cc;
        _bcc = bcc;
        _fromDisplay = fromDisplay;
        _fromEmail = fromEmail;
        
        // Setting services
        _logger = logger;
        _svcMailing = svcMailing;
        
        // Setting events
        Events = events;
    }

    /// <summary>
    /// Constructor (Use for dependency injection - Use method load in element to set logger) 
    /// </summary>
    /// <param name="svcConfiguration">Configuration service</param>
    /// <param name="svcMailing">Mailing service</param>
    public LoggingService(IConfigurationService svcConfiguration, IMailingService svcMailing)
    {
        // Setting application info
        _applicationName = svcConfiguration.GetValue(new List<string> { "Application", "Info", "Name" });
        _applicationCode = svcConfiguration.GetValue(new List<string> { "Application", "Info", "Code" });
        _applicationVersion = svcConfiguration.GetValue(new List<string> { "Application", "Info", "Version" });
        _logTrace = bool.Parse(svcConfiguration.GetValue(new List<string> { "LoggingBSCA", "Logs", "LogTrace" }));
        _logDebug = bool.Parse(svcConfiguration.GetValue(new List<string> { "LoggingBSCA", "Logs", "LogDebug" }));
        _fromEmail = svcConfiguration.GetValue(new List<string> { "LoggingBSCA", "Logs", "Mailing", "FromEmail" });
        _fromDisplay = svcConfiguration.GetValue(new List<string> { "LoggingBSCA", "Logs", "Mailing", "FromDisplay" });
        _to = svcConfiguration.GetSection(new List<string> { "LoggingBSCA", "Logs", "Mailing", "To" }).GetChildren()
            .Select(s => s["Email"]!)
            .ToList();
        _cc = svcConfiguration.GetSection(new List<string> { "LoggingBSCA", "Logs", "Mailing", "Cc" }).GetChildren()
            .Select(s => s["Email"]!)
            .ToList();
        _bcc = svcConfiguration.GetSection(new List<string> { "LoggingBSCA", "Logs", "Mailing", "Bcc" }).GetChildren()
            .Select(s => s["Email"]!)
            .ToList();
        _moduleName = string.Empty;
        
        // Setting services
        _svcMailing = svcMailing;
        _logger = null;
        
        // Setting events
        Events = new List<EventId>();
        Events = svcConfiguration.GetSection(new List<string> { "LoggingBSCA", "Events" }).GetChildren()
            .Select(e => new EventId(int.Parse(e["Id"]!), e["Text"]))
            .ToList();
    }

    #endregion

    #region Destructors

    // #N/A

    #endregion

    #region Methods

    #region Privates

    /// <summary>
    /// Method used to prepare mail's body
    /// </summary>
    /// <param name="fctName">Name of transmitting function</param>
    /// <param name="level">Logging level</param>
    /// <param name="message">Message emitted</param>
    /// <param name="ex">[Can be null] Exception occurred</param>
    /// <returns>
    /// Mail message
    /// </returns>
    /// <exception cref="ArgumentNullException">If the name of transmitting function is null or empty</exception>
    /// <exception cref="ArgumentNullException">If the message emitted is null or empty</exception>
    private string PrepareMailBody(string fctName, LogLevel level, string message, Exception? ex = null)
    {
        // Checks
        if (string.IsNullOrWhiteSpace(fctName))
            throw new ArgumentNullException(nameof(fctName), $"The value of the parameter {nameof(fctName)} cannot be null or empty");
        if (string.IsNullOrWhiteSpace(message))
            throw new ArgumentNullException(nameof(message), $"The value of the parameter {nameof(message)} cannot be null or empty");

        // Preparing message
        StringBuilder mailBody = new StringBuilder();
        mailBody.Append($"<HTML>{Environment.NewLine}");
        mailBody.Append($"   <BODY>{Environment.NewLine}");
        mailBody.Append($"       <DIV>{Environment.NewLine}");
        mailBody.Append($"           <H3><B><U>Application Information:</U></B></H3>{Environment.NewLine}");
        mailBody.Append($"           <DL>{Environment.NewLine}");
        mailBody.Append($"               <DT><B>Code:</B></DT>{Environment.NewLine}");
        mailBody.Append($"               <DD>{_applicationCode}</DD>{Environment.NewLine}");
        mailBody.Append($"               <DT><B>Name:</B></DT>{Environment.NewLine}");
        mailBody.Append($"               <DD>{_applicationName}</DD>{Environment.NewLine}");
        mailBody.Append($"               <DT><B>Version:</B></DT>{Environment.NewLine}");
        mailBody.Append($"               <DD>{_applicationVersion}</DD>{Environment.NewLine}");
        mailBody.Append($"               <DT><B>Module:</B></DT>{Environment.NewLine}");
        mailBody.Append($"               <DD>{_moduleName}</DD>{Environment.NewLine}");
        mailBody.Append($"               <DT><B>Function:</B></DT>{Environment.NewLine}");
        mailBody.Append($"               <DD>{fctName}</DD>{Environment.NewLine}");
        mailBody.Append($"           </DL>{Environment.NewLine}");
        mailBody.Append($"       </DIV>{Environment.NewLine}");
        mailBody.Append($"       <DIV>{Environment.NewLine}");
        mailBody.Append($"           <H3><B><U>Log Level:</U></B></H3>{Environment.NewLine}");
        mailBody.Append($"           <P>{level}</P>{Environment.NewLine}");
        mailBody.Append($"       </DIV>{Environment.NewLine}");
        mailBody.Append($"       <DIV>{Environment.NewLine}");
        mailBody.Append($"           <H3><B><U>Message:</U></B></H3>{Environment.NewLine}");
        mailBody.Append($"           <P>{message}</P>{Environment.NewLine}");
        mailBody.Append($"       </DIV>{Environment.NewLine}");
        mailBody.Append($"       <DIV>{Environment.NewLine}");
        mailBody.Append($"           <H3><B><U>Exception JSON:</U></B></H3>{Environment.NewLine}");
        mailBody.Append($"           <P>{JsonConvert.SerializeObject(ex)}</P>{Environment.NewLine}");
        mailBody.Append($"       </DIV>{Environment.NewLine}");
        mailBody.Append($"   </BODY>{Environment.NewLine}");
        mailBody.Append($"</HTML>{Environment.NewLine}");

        // Function's return
        return mailBody.ToString();
    }
    
    /// <summary>
    /// Method used to prepare logging message
    /// </summary>
    /// <param name="fctName">Name of transmitting function</param>
    /// <param name="level">Logging level</param>
    /// <param name="message">Message emitted</param>
    /// <param name="ex">[Can be null] Exception occurred</param>
    /// <returns>
    /// Logging message
    /// </returns>
    /// <exception cref="ArgumentNullException">If the name of transmitting function is null or empty</exception>
    /// <exception cref="ArgumentNullException">If the message emitted is null or empty</exception>
    private string PrepareLoggingMessage(string fctName, LogLevel level, string message, Exception? ex = null)
    {
        // Checks
        if (string.IsNullOrWhiteSpace(fctName))
            throw new ArgumentNullException(nameof(fctName),
                $"The value of the parameter {nameof(fctName)} cannot be null or empty");
        if (string.IsNullOrWhiteSpace(message))
            throw new ArgumentNullException(nameof(message),
                $"The value of the parameter {nameof(message)} cannot be null or empty");

        // Preparing message
        StringBuilder loggingMessage = new StringBuilder();
        loggingMessage.Append($"Application Information:{Environment.NewLine}");
        loggingMessage.Append($"--> Application Code: {_applicationCode}{Environment.NewLine}");
        loggingMessage.Append($"--> Application Name: {_applicationName}{Environment.NewLine}");
        loggingMessage.Append($"--> Application Version: {_applicationVersion}{Environment.NewLine}");
        loggingMessage.Append($"--> Module Name: {_moduleName}{Environment.NewLine}");
        loggingMessage.Append($"--> Function Name: {fctName}{Environment.NewLine}{Environment.NewLine}");
        loggingMessage.Append($"Log:{Environment.NewLine}");
        loggingMessage.Append($"--> Level: {level.ToString().ToUpperInvariant()}{Environment.NewLine}{Environment.NewLine}");
        loggingMessage.Append($"Message:{Environment.NewLine}");
        loggingMessage.Append($"{message}{Environment.NewLine}{Environment.NewLine}");
        
        // Adding error
        if (ex is not null)
        {
            loggingMessage.Append($"Exception Message:{Environment.NewLine}");
            loggingMessage.Append($"{ex.Message}{Environment.NewLine}{Environment.NewLine}");
            loggingMessage.Append($"Exception Stack Trace:{Environment.NewLine}");
            loggingMessage.Append($"{ex.StackTrace}{Environment.NewLine}{Environment.NewLine}");
            loggingMessage.Append($"Error (Jsonify):{Environment.NewLine}");
            loggingMessage.Append($"{JsonConvert.SerializeObject(ex)}");
        }

        // Function's return
        return loggingMessage.ToString();
    }
    
    #endregion

    #region Publics

    /// <summary>
    /// Method used to load final information (in case of use of configuration file)
    /// </summary>
    /// <param name="logger">Logger used to log</param>
    /// <param name="moduleName">Module name</param>
    /// <typeparam name="T">Type of element of the logger</typeparam>
    public void Load<T>(ILogger<T> logger, string moduleName)
    {
        // Setting missing elements
        _logger = logger;
        _moduleName = moduleName;
    }
    
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
    public void WriteLog(string fctName, LogLevel level, EventId eventId, string message, Exception? ex = null)
    {
        // Checks
        if (string.IsNullOrWhiteSpace(fctName))
            throw new ArgumentNullException(nameof(fctName),
                $"The value of the parameter {nameof(fctName)} cannot be null or empty");
        if (string.IsNullOrWhiteSpace(message))
            throw new ArgumentNullException(nameof(message),
                $"The value of the parameter {nameof(message)} cannot be null or empty");
        
        // Log
        string logMessage = PrepareLoggingMessage(fctName, level, message, ex);
        switch (level)
        {
            case LogLevel.Debug:
                if (_logDebug)
                    _logger!.Log(level, eventId, ex, logMessage);
                break;
            
            case LogLevel.Trace:
                if (_logTrace)
                    _logger!.Log(level, eventId, ex, logMessage);
                break;

            case LogLevel.Critical:
                _logger!.Log(level, eventId, ex, logMessage);
                _svcMailing.SendMail(new Tuple<string, string?>(_fromEmail, _fromDisplay), _to, _cc, _bcc,
                    $"A critical error has occurred on application {_applicationCode} - {_applicationName} - {_applicationVersion}",
                    new List<string> { PrepareMailBody(fctName, level, message, ex) }, true, MailPriority.High);
                break;

            default:
                _logger!.Log(level, eventId, ex, logMessage);
                break;
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
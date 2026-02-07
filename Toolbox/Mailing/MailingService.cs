// Using
using System.ComponentModel.DataAnnotations;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using Toolbox.Configuration;

// Static using
// #N/A

// Named using
// #N/A

// Namespace
namespace Toolbox.Mailing;

/// <summary>
/// Service used to send mail
/// </summary>
public class MailingService : IMailingService
{
    #region Members

    #region Constants

    // #N/A

    #endregion

    #region Variables

    /// <summary>
    /// SMTP client
    /// </summary>
    private readonly SmtpClient _client;

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
    /// <param name="serverAddress">Server address</param>
    /// <exception cref="ArgumentNullException">If the server address is null or empty</exception>
    public MailingService(string serverAddress)
    {
        // Checks
        if (string.IsNullOrWhiteSpace(serverAddress)) throw new ArgumentNullException(nameof(serverAddress), $"The value of the parameter: {nameof(serverAddress)} cannot be null or empty");
        
        // Setting up client
        _client = new SmtpClient(serverAddress);
    }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="svcConfiguration">Configuration service</param>
    public MailingService(IConfigurationService svcConfiguration)
    {
        // Setting up client
        _client = new SmtpClient(svcConfiguration.GetValue(new List<string> { "Mailing", "ServerAddress" }));
    }

    #endregion

    #region Destructors

    // #N/A

    #endregion

    #region Methods

    #region Privates

    /// <summary>
    /// Method used to prepare mail message
    /// </summary>
    /// <param name="from">From information (1st email, 2nd display name)</param>
    /// <param name="to">List of emails for the "to" field</param>
    /// <param name="cc">[Can be null] List of emails for the "cc" field</param>
    /// <param name="bcc">[Can be null] List of emails for the "bcc" field</param>
    /// <param name="subject">Subject of the mail</param>
    /// <param name="body">Lines of the body</param>
    /// <param name="isBodyHtml">Is the body in HTML format</param>
    /// <param name="mailPriority">Mail priority</param>
    /// <param name="attachments">[Can be null] List of attachments</param>
    /// <returns>
    /// Mail object with all information
    /// </returns>
    /// <exception cref="ArgumentNullException">If the subject is null or empty</exception>
    /// <exception cref="ArgumentNullException">If the email 'From' is null or empty</exception>
    /// <exception cref="ArgumentNullException">If the body is null or empty</exception>
    /// <exception cref="ArgumentNullException">If the to, cc and bcc is null and empty</exception>
    private MailMessage PrepareMail(Tuple<string, string?> from, IEnumerable<string>? to, IEnumerable<string>? cc, IEnumerable<string>? bcc, string subject, 
        IEnumerable<string> body, bool isBodyHtml, MailPriority mailPriority = MailPriority.Normal, IEnumerable<FileInfo>? attachments = null)
    {
        // Checks
        if (string.IsNullOrEmpty(subject) || string.IsNullOrWhiteSpace(subject)) throw new ArgumentNullException(nameof(subject), $"The subject cannot be null or empty");
        if (string.IsNullOrEmpty(from.Item1) || string.IsNullOrWhiteSpace(from.Item1)) throw new ArgumentNullException(nameof(from), $"The email 'From' cannot be null or empty");
        if (!body.Any() || body.Any(string.IsNullOrEmpty) || body.Any(string.IsNullOrWhiteSpace)) throw new ArgumentNullException(nameof(body), $"The body cannot be null or empty");
        if ((to is null || !to.Any()) && (cc is null || !cc.Any()) && (bcc is null || !bcc.Any())) throw new ArgumentNullException("to, cc, bcc", "At least one of these: to, cc, bcc, must contains one email address");
        
        // Preparing mail
        MailMessage mail = new MailMessage();
        mail.IsBodyHtml = isBodyHtml;
        mail.Priority = mailPriority;
        mail.Subject = subject;
        
        // Adding mails addresses
        mail.From = GetMailFromString(from.Item1, from.Item2);
        if (to is not null && to.Any()) 
            foreach (string toEmail in to) 
                mail.To.Add(GetMailFromString(toEmail));
        if (cc is not null && cc.Any()) 
            foreach (string ccEmail in cc) 
                mail.CC.Add(GetMailFromString(ccEmail));
        if (bcc is not null && bcc.Any()) 
            foreach (string bccEmail in bcc) 
                mail.Bcc.Add(GetMailFromString(bccEmail));
        
        // Adding body
        StringBuilder bodyText = new StringBuilder();
        foreach (string bodyLine in body)
            bodyText.Append(bodyLine);
        mail.Body = bodyText.ToString();
        
        // Adding attachments
        if (attachments is not null && attachments.Any()) 
            foreach (FileInfo attach in attachments)
                mail.Attachments.Add(new Attachment(attach.Name, MediaTypeNames.Application.Octet));
        
        // Function's return
        return mail;
    }

    /// <summary>
    /// Method used to transform text to email element
    /// </summary>
    /// <param name="mail">Mail address</param>
    /// <param name="display">Display text</param>
    /// <returns>
    /// The mail object with all information
    /// </returns>
    /// <exception cref="ValidationException">If the email address is not in a correct format</exception>
    private MailAddress GetMailFromString(string mail, string? display = null)
    {
        // Checks
        EmailAddressAttribute emailAttribute = new EmailAddressAttribute();
        if (!emailAttribute.IsValid(mail)) throw new ValidationException($"Email address: {mail} is not correct");
        
        // Function's return
        return new MailAddress(mail, display);
    }
    
    #endregion

    #region Publics

    /// <summary>
    /// Method used to send mail
    /// </summary>
    /// <param name="from">From information (1st email, 2nd display name)</param>
    /// <param name="to">[Can be null] List of emails for the "to" field</param>
    /// <param name="cc">[Can be null] List of emails for the "cc" field</param>
    /// <param name="bcc">[Can be null] List of emails for the "bcc" field</param>
    /// <param name="subject">Subject of the mail</param>
    /// <param name="body">Lines of the body</param>
    /// <param name="isBodyHtml">Is the body in HTML format</param>
    /// <param name="mailPriority">Mail priority</param>
    /// <param name="attachments">[Can be null] List of attachments</param>
    /// <param name="sendCompletedEventHandler">[Can be null] Handler used to determine what to do after sending mail</param>
    /// <exception cref="ArgumentNullException">If the subject is null or empty</exception>
    /// <exception cref="ArgumentNullException">If the email 'From' is null or empty</exception>
    /// <exception cref="ArgumentNullException">If the body is null or empty</exception>
    /// <exception cref="ArgumentNullException">If the to, cc and bcc is null and empty</exception>
    public void SendMail(Tuple<string, string?> from, IEnumerable<string>? to, IEnumerable<string>? cc, IEnumerable<string>? bcc, string subject, IEnumerable<string> body, 
        bool isBodyHtml, MailPriority mailPriority = MailPriority.Normal, IEnumerable<FileInfo>? attachments = null, SendCompletedEventHandler? sendCompletedEventHandler = null)
    {
        // Preparing mail
        MailMessage mail = PrepareMail(from, to, cc, bcc, subject, body, isBodyHtml, mailPriority, attachments);
        if (sendCompletedEventHandler is not null) _client.SendCompleted += sendCompletedEventHandler;
        
        // Sending mail
        _client.Send(mail);
    }

    /// <summary>
    /// Method used to send mail (Asynchronous)
    /// </summary>
    /// <param name="from">From information (1st email, 2nd display name)</param>
    /// <param name="to">[Can be null] List of emails for the "to" field</param>
    /// <param name="cc">[Can be null] List of emails for the "cc" field</param>
    /// <param name="bcc">[Can be null] List of emails for the "bcc" field</param>
    /// <param name="subject">Subject of the mail</param>
    /// <param name="body">Lines of the body</param>
    /// <param name="isBodyHtml">Is the body in HTML format</param>
    /// <param name="mailPriority">Mail priority</param>
    /// <param name="attachments">[Can be null] List of attachments</param>
    /// <param name="sendCompletedEventHandler">[Can be null] Handler used to determine what to do after sending mail</param>
    /// <exception cref="ArgumentNullException">If the subject is null or empty</exception>
    /// <exception cref="ArgumentNullException">If the email 'From' is null or empty</exception>
    /// <exception cref="ArgumentNullException">If the body is null or empty</exception>
    /// <exception cref="ArgumentNullException">If the to, cc and bcc is null and empty</exception>
    public async Task SendMailAsync(Tuple<string, string?> from, IEnumerable<string>? to, IEnumerable<string>? cc, IEnumerable<string>? bcc, string subject, 
        IEnumerable<string> body, bool isBodyHtml, MailPriority mailPriority = MailPriority.Normal, IEnumerable<FileInfo>? attachments = null, 
        SendCompletedEventHandler? sendCompletedEventHandler = null)
    {
        // Preparing mail
        MailMessage mail = PrepareMail(from, to, cc, bcc, subject, body, isBodyHtml, mailPriority, attachments);
        if (sendCompletedEventHandler is not null) _client.SendCompleted += sendCompletedEventHandler;
        
        // Sending mail
        await _client.SendMailAsync(mail);
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
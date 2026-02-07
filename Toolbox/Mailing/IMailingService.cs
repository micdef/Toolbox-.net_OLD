// Using
using System.Net.Mail;

// Static using
// #N/A

// Named using
// #N/A

// Namespace
namespace Toolbox.Mailing;

/// <summary>
/// Service interface used to send emails
/// </summary>
public interface IMailingService
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
    void SendMail(Tuple<string, string?> from, IEnumerable<string>? to, IEnumerable<string>? cc, IEnumerable<string>? bcc, string subject, IEnumerable<string> body, 
        bool isBodyHtml, MailPriority mailPriority = MailPriority.Normal, IEnumerable<FileInfo>? attachments = null, SendCompletedEventHandler? sendCompletedEventHandler = null);

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
    Task SendMailAsync(Tuple<string, string?> from, IEnumerable<string>? to, IEnumerable<string>? cc, IEnumerable<string>? bcc, string subject, IEnumerable<string> body, 
        bool isBodyHtml, MailPriority mailPriority = MailPriority.Normal, IEnumerable<FileInfo>? attachments = null, SendCompletedEventHandler? sendCompletedEventHandler = null);

    #endregion
}
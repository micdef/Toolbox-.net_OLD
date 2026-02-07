// Using
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.DirectoryServices.ActiveDirectory;
using System.Security.Authentication;

// Static using
// #N/A

// Named using
// #N/A

// Namespace
namespace Toolbox.ActiveDirectory;

/// <summary>
/// Active Directory Service
/// </summary>
public interface IActiveDirectoryService
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
    /// Method used to retrieve full Active Directory object
    /// </summary>
    /// <param name="obj">Base Active Directory object</param>
    /// <typeparam name="T">Type derived from "Principal" class</typeparam>
    /// <returns>
    /// [Can be null] Full Active Directory object
    /// </returns>
    DirectoryEntry? GetDirectoryEntry<T>(T obj)
        where T : Principal;

    /// <summary>
    /// Method used to create a base Active Directory object
    /// </summary>
    /// <param name="propName">Property name</param>
    /// <param name="propValue">[Can be null] Property value</param>
    /// <typeparam name="T">Type derived from "Principal" class</typeparam>
    /// <returns>
    /// Base Active Directory object
    /// </returns>
    /// <exception cref="ArgumentNullException">If the property name is null or empty</exception>
    /// <exception cref="NullReferenceException">If no properties can be found with given name (Case insensitive)</exception>
    T CreateObject<T>(string propName, object? propValue = null)
        where T : Principal;

    /// <summary>
    /// Method used to search one or many elements
    /// </summary>
    /// <param name="propName">Property name</param>
    /// <param name="propValue">Property value</param>
    /// <typeparam name="T">Type derived from "Principal" class</typeparam>
    /// <returns>
    /// The list of retrieved Active Directory objects
    /// </returns>
    /// <exception cref="ArgumentNullException">If the property name is null or empty</exception>
    /// <exception cref="NullReferenceException">If no properties can be found with given name (Case insensitive)</exception>
    IEnumerable<T> SearchObjects<T>(string propName, object? propValue)
        where T : Principal;

    /// <summary>
    /// Method used to compare 2 Active Directory objects
    /// </summary>
    /// <param name="obj1">1st object of the comparison</param>
    /// <param name="obj2">2nd object of the comparison</param>
    /// <param name="byName">Compare by name? (If not ist compared by Distinguished Names)</param>
    /// <typeparam name="T">Type derived from "Principal" class</typeparam>
    /// <returns>
    /// If the objects are the same or not
    /// </returns>
    bool Equals<T>(T obj1, T obj2, bool byName)
        where T : Principal;

    /// <summary>
    /// Method used to check if an object exist
    /// </summary>
    /// <param name="obj">Object to check</param>
    /// <typeparam name="T">Type derived from "Principal" class</typeparam>
    /// <returns>
    /// If the object exist or not
    /// </returns>
    bool Exists<T>(T obj)
        where T : Principal;

    /// <summary>
    /// Method used to check if an object is enabled or not
    /// </summary>
    /// <param name="obj">Object to check</param>
    /// <typeparam name="T">Type derived from "Principal" class</typeparam>
    /// <returns>
    /// If the object is enabled or not
    /// </returns>
    /// <exception cref="ActiveDirectoryObjectNotFoundException">If the object does not exist in the Active Directory context</exception>
    /// <exception cref="InvalidCastException">If the object is not one of the following types: UserPrincipal or ComputerPrincipal</exception>
    bool IsEnabled<T>(T obj)
        where T : Principal;

    /// <summary>
    /// Method used to activate an Active Directory object
    /// </summary>
    /// <param name="obj">Object to activate</param>
    /// <typeparam name="T">Type derived from "Principal" class</typeparam>
    /// <exception cref="ActiveDirectoryObjectNotFoundException">If the object does not exist in the Active Directory context</exception>
    /// <exception cref="InvalidCastException">If the object is not one of the following types: UserPrincipal or ComputerPrincipal</exception>
    void Activate<T>(T obj)
        where T : Principal;

    /// <summary>
    /// Method used to deactivate an Active Directory object
    /// </summary>
    /// <param name="obj">Object to deactivate</param>
    /// <typeparam name="T">Type derived from "Principal" class</typeparam>
    /// <exception cref="ActiveDirectoryObjectNotFoundException">If the object does not exist in the Active Directory context</exception>
    /// <exception cref="InvalidCastException">If the object is not one of the following types: UserPrincipal or ComputerPrincipal</exception>
    void Deactivate<T>(T obj)
        where T : Principal;

    /// <summary>
    /// Method used to move an Active Directory object from an Organizational Unit to another one
    /// </summary>
    /// <param name="obj">Object to move</param>
    /// <param name="newDistinguishedName">New distinguished name of the object</param>
    /// <param name="newName">[Can be null] New name of the object</param>
    /// <typeparam name="T">Type derived from "Principal" class</typeparam>
    /// <exception cref="ActiveDirectoryObjectNotFoundException">If the object does not exist in the Active Directory context</exception>
    /// <exception cref="ArgumentNullException">If the new Distinguished Name of the object is null or empty</exception>
    void MoveObject<T>(T obj, string newDistinguishedName, string? newName = null)
        where T : Principal;

    /// <summary>
    /// Method used to retrieve Active Directory objects from Organizational Unit
    /// </summary>
    /// <param name="ouDistinguishedName">Organizational Unit Distinguished Name</param>
    /// <param name="checkAllTree">Check in all the selected tree (below the OU)</param>
    /// <typeparam name="T">Type derived from "Principal" class</typeparam>
    /// <returns>
    /// List of retrieved Active Directory objects
    /// </returns>
    /// <exception cref="ArgumentNullException">f the new Distinguished Name of the Organizational Unit is null or empty</exception>
    IEnumerable<T> GetObjectsFromOrganizationalUnit<T>(string ouDistinguishedName, bool checkAllTree)
        where T : Principal;

    /// <summary>
    /// Method used to retrieve all members in an Active Directory Group
    /// </summary>
    /// <param name="group">Group to check</param>
    /// <returns>
    /// List of members
    /// </returns>
    /// <exception cref="ActiveDirectoryObjectNotFoundException">If the group does not exist in the Active Directory context</exception>
    IEnumerable<Principal> GetGroupMembers(GroupPrincipal group);

    /// <summary>
    /// Method used to retrieve all members of a type in an Active Directory Group
    /// </summary>
    /// <param name="group">Group to check</param>
    /// <typeparam name="T">Type derived from "Principal" class</typeparam>
    /// <returns>
    /// List of members
    /// </returns>
    /// <exception cref="ActiveDirectoryObjectNotFoundException">If the group does not exist in the Active Directory context</exception>
    IEnumerable<T> GetGroupMembers<T>(GroupPrincipal group)
        where T : Principal;

    /// <summary>
    /// Method used to check if an Active Directory object is a member of an Active Directory group
    /// </summary>
    /// <param name="group">Group to check</param>
    /// <param name="obj">Object to check</param>
    /// <typeparam name="T">Type derived from "Principal" class</typeparam>
    /// <returns>
    /// If the object is a member of the group
    /// </returns>
    /// <exception cref="ActiveDirectoryObjectNotFoundException">If the group does not exist in the Active Directory context</exception>
    /// <exception cref="ActiveDirectoryObjectNotFoundException">If the object does not exist in the Active Directory context</exception>
    bool IsGroupMember<T>(GroupPrincipal group, T obj)
        where T : Principal;

    /// <summary>
    /// Method used to add an Active Directory Object in an Active Directory Group
    /// </summary>
    /// <param name="group">Group where member will be added</param>
    /// <param name="obj">Object to be added to the group</param>
    /// <typeparam name="T">Type derived from "Principal" class</typeparam>
    /// <exception cref="ActiveDirectoryObjectNotFoundException">If the group does not exist in the Active Directory context</exception>
    /// <exception cref="ActiveDirectoryObjectNotFoundException">If the object does not exist in the Active Directory context</exception>
    /// <exception cref="Exception">If the object to be added is already a member of the group</exception>
    void AddGroupMember<T>(GroupPrincipal group, T obj)
        where T : Principal;

    /// <summary>
    /// Method used to remove an Active Directory Object in an Active Directory Group
    /// </summary>
    /// <param name="group">Group where member will be removed</param>
    /// <param name="obj">Object to be removed to the group</param>
    /// <typeparam name="T">Type derived from "Principal" class</typeparam>
    /// <exception cref="ActiveDirectoryObjectNotFoundException">If the group does not exist in the Active Directory context</exception>
    /// <exception cref="ActiveDirectoryObjectNotFoundException">If the object does not exist in the Active Directory context</exception>
    /// <exception cref="Exception">If the object to be added is already a member of the group</exception>
    void RemoveGroupMember<T>(GroupPrincipal group, T obj)
        where T : Principal;

    /// <summary>
    /// Method used to retrieve generics accounts from a list
    /// </summary>
    /// <param name="list">List to check</param>
    /// <typeparam name="T">Type derived from "Principal" class</typeparam>
    /// <returns>
    /// List of generic accounts founded
    /// </returns>
    /// <exception cref="ArgumentNullException">If the list is null or empty</exception>
    IEnumerable<UserPrincipal> GetGenericAccountsFromAList<T>(IEnumerable<T> list)
        where T : Principal;

    /// <summary>
    /// Method used to retrieve nominated accounts from a list
    /// </summary>
    /// <param name="list">List to check</param>
    /// <typeparam name="T">Type derived from "Principal" class</typeparam>
    /// <returns>
    /// List of nominated accounts founded
    /// </returns>
    /// <exception cref="ArgumentNullException">If the list is null or empty</exception>
    IEnumerable<UserPrincipal> GetNominatedAccountsFromAList<T>(IEnumerable<T> list)
        where T : Principal;

    /// <summary>
    /// Method used check password complexity
    /// Rules :
    /// 1° : Password must contain at least 12 characters
    /// 2° : Password must contain 3 of theses (1 Capital letter, 1 Normal letter, 1 Number, 1 Special Char)
    /// 3° : Password must not contain one these word (Username, Firstname, Lastname, BSCA (in all formats))
    /// 4° : Password must not contain any spaces
    /// </summary>
    /// <param name="user">User to check password</param>
    /// <param name="password">Password to check</param>
    /// <returns>
    /// If the password have necessary complexity or not
    /// </returns>
    /// <exception cref="ActiveDirectoryObjectNotFoundException">If the user does not exist in the Active Directory context</exception>
    /// <exception cref="ArgumentNullException">If the password to check is null or empty</exception>
    bool CheckPasswordComplexity(UserPrincipal user, string password);

    /// <summary>
    /// Method used to change password of a user
    /// </summary>
    /// <param name="user">User to change password</param>
    /// <param name="newPassword">New password to set</param>
    /// <param name="currentPassword">[Can be null] Current password (Default: null)</param>
    /// <param name="passwordMustBeChangedAtNextLogon">User must change his password at next logon? (Default: true)</param>
    /// <exception cref="ActiveDirectoryObjectNotFoundException">If the user does not exist in the Active Directory context</exception>
    /// <exception cref="ArgumentNullException">If the new password to set is null or empty</exception>
    /// <exception cref="AuthenticationException">If the old password is set and the Active Directory cannot log on the user with this password</exception>
    /// <exception cref="Exception">If the password is not meeting BSCA complexity requirements</exception>
    void ChangePassword(UserPrincipal user, string newPassword, string? currentPassword = null, bool passwordMustBeChangedAtNextLogon = true);

    /// <summary>
    /// Method used to log in Active Directory
    /// </summary>
    /// <param name="username">Username of the account</param>
    /// <param name="password">Password of the account</param>
    /// <param name="authorizeGenericAccount">Is the generic account authorized to log in</param>
    /// <returns>
    /// A tuple with 2 elements:
    /// --> 1st: [Can be null] Active Directory User
    /// --> 2nd: [Can be null] Exception thrown
    /// </returns>
    /// <exception cref="ArgumentNullException">If the username of the account is null or empty</exception>
    /// <exception cref="ArgumentNullException">If the password of the account is null or empty</exception>
    /// <exception cref="ActiveDirectoryObjectNotFoundException">If the account is not found in the Active Directory</exception>
    /// <exception cref="AuthenticationException">If the account is locked</exception>
    /// <exception cref="AuthenticationException">If the account is not active (Disable in Active Directory)</exception>
    /// <exception cref="AuthenticationException">If the user's account is a generic on and generic accounts are not authorized to log in</exception>
    /// <exception cref="AuthenticationException">If the username/password combination is incorrect</exception>
    Tuple<UserPrincipal?, Exception?> Login(string username, string password, bool authorizeGenericAccount = false);

    /// <summary>
    /// Method used to create an Active Directory User
    /// </summary>
    /// <param name="samAccountName">Active Directory User login name</param>
    /// <param name="properties">Properties of the account</param>
    /// <param name="groupNames">[Can be null] Group names (Default: null)</param>
    /// <returns>
    /// The Active Directory User created
    /// </returns>
    /// <exception cref="ArgumentNullException">If the Active Directory User login name is null or empty</exception>
    /// <exception cref="Exception">If an Active Directory User already exist with the same login name</exception>
    /// <exception cref="ArgumentNullException">If the properties does not contain a key named "Password" or its value is set to null or empty</exception>
    /// <exception cref="ArgumentNullException">If the properties does not contain a key named "PasswordNeverExpires" or its value is set to null or empty</exception>
    /// <exception cref="ActiveDirectoryObjectNotFoundException">If the group list is not null, contains group names and one of these group names cannot be found in the Active Directory</exception>
    /// <exception cref="Exception">If the password not meeting BSCA password complexity</exception>
    UserPrincipal CreateUser(string samAccountName, Dictionary<string, object?> properties, List<string>? groupNames = null);

    /// <summary>
    /// Method used to create an Active Directory Group
    /// </summary>
    /// <param name="name">Active Directory Group name</param>
    /// <param name="properties">Properties of the group</param>
    /// <param name="groupNames">[Can be null] Group names (Default: null)</param>
    /// <returns>
    /// The Active Directory Group created
    /// </returns>
    /// <exception cref="ArgumentNullException">If the Active Directory Group name is null or empty</exception>
    /// <exception cref="Exception">If an Active Directory Group already exist with the same name</exception>
    /// <exception cref="ActiveDirectoryObjectNotFoundException">If the group list is not null, contains group names and one of these group names cannot be found in the Active Directory</exception>
    GroupPrincipal CreateGroup(string name, Dictionary<string, object?> properties, List<string>? groupNames = null);

    /// <summary>
    /// Method used to create an Active Directory Computer
    /// </summary>
    /// <param name="name">Active Directory Computer name</param>
    /// <param name="properties">Properties of the Computer</param>
    /// <param name="groupNames">[Can be null] Group names (Default: null)</param>
    /// <returns>
    /// The Active Directory Group created
    /// </returns>
    /// <exception cref="ArgumentNullException">If the Active Directory Computer name is null or empty</exception>
    /// <exception cref="Exception">If an Active Directory Computer already exist with the same name</exception>
    /// <exception cref="ActiveDirectoryObjectNotFoundException">If the group list is not null, contains group names and one of these group names cannot be found in the Active Directory</exception>
    ComputerPrincipal CreateComputer(string name, Dictionary<string, object?> properties, List<string>? groupNames = null);

    #endregion
}
// Using
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.DirectoryServices.ActiveDirectory;
using System.Reflection;
using System.Security.Authentication;
using System.Text.RegularExpressions;
using Toolbox.Configuration;
using Toolbox.Cryptography;

// Static using
// #N/A

// Named using
// #N/A

// Pragma START
#pragma warning disable CA1416 // Validate platform compatibility

// Namespace
namespace Toolbox.ActiveDirectory;

/// <summary>
/// Active Directory service
/// </summary>
public class ActiveDirectoryService : IActiveDirectoryService
{
    #region Members

    #region Constants

    // #N/A

    #endregion

    #region Variables

    /// <summary>
    /// Context used to connect Active Directory
    /// </summary>
    private readonly PrincipalContext _context;

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
    /// <param name="svcConfiguration">Configuration service</param>
    /// <param name="svcCryptography">Cryptography service</param>
    /// <exception cref="NullReferenceException">If the section "Application->ActiveDirectory->ADDS->Username" in configuration file is null or empty</exception>
    /// <exception cref="NullReferenceException">If the section "Application->ActiveDirectory->ADDS->Password" in configuration file is null or empty</exception>
    /// <exception cref="NullReferenceException">If the section "Application->ActiveDirectory->ADDS->DomainName" in configuration file is null or empty</exception>
    public ActiveDirectoryService(IConfigurationService svcConfiguration, Base64CryptographyService svcCryptography)
    {
        // Getting elements from configuration
        string username = svcConfiguration.GetValue(new List<string> { "Application", "ActiveDirectory", "ADDS", "Username" });
        string password = svcConfiguration.GetValue(new List<string> { "Application", "ActiveDirectory", "ADDS", "Password" });
        string domainName = svcConfiguration.GetValue(new List<string> { "Application", "ActiveDirectory", "ADDS", "DomainName" });
        
        // Checks
        if (username.IsStringEmpty()) throw new NullReferenceException("The value of Application->ActiveDirectory->ADDS->Username in configuration file cannot be null or empty");
        if (password.IsStringEmpty()) throw new NullReferenceException("The value of Application->ActiveDirectory->ADDS->Password in configuration file cannot be null or empty");
        if (domainName.IsStringEmpty()) throw new NullReferenceException("The value of Application->ActiveDirectory->ADDS->DomainName in configuration file cannot be null or empty");
        
        // Decrypting information
        username = username.Decrypt(svcCryptography);
        password = password.Decrypt(svcCryptography);
        domainName = domainName.Decrypt(svcCryptography);
        
        // Setting up context
        _context = new PrincipalContext(ContextType.Domain, domainName, username, password);
    }
    
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="contextType">Type of context</param>
    /// <param name="username">Username use to connect AD</param>
    /// <param name="password">Password of the username</param>
    /// <param name="domainName">Domain to contact</param>
    /// <exception cref="ArgumentNullException">If username use to connect AD is null or empty</exception>
    /// <exception cref="ArgumentNullException">If password of the username is null or empty</exception>
    /// <exception cref="ArgumentNullException">If the domain name is null or empty</exception>
    public ActiveDirectoryService(ContextType contextType, string username, string password, string domainName)
    {
        // Setting Base64 cryptography service
        ICryptographyService svcCryptography = new Base64CryptographyService(Base64CryptographyService.Base64Tables.Utf8);

        // Checks
        if (username.IsStringEmpty()) throw new ArgumentNullException(nameof(username), $"The value of the parameter {nameof(username)} cannot be null or empty");
        if (password.IsStringEmpty()) throw new ArgumentNullException(nameof(password), $"The value of the parameter {nameof(password)} cannot be null or empty");
        if (domainName.IsStringEmpty()) throw new ArgumentNullException(nameof(domainName), $"The value of the parameter {nameof(domainName)} cannot be null or empty");

        // Decrypting information
        username = username.Decrypt(svcCryptography);
        password = password.Decrypt(svcCryptography);
        domainName = domainName.Decrypt(svcCryptography);

        // Setting up context
        _context = new PrincipalContext(contextType, domainName, username, password);
    }

    #endregion

    #region Destructors

    // #N/A

    #endregion

    #region Methods

    #region Privates

    /// <summary>
    /// Method used to activate or deactivate an Active Directory object
    /// </summary>
    /// <param name="obj">Object to activate/deactivate</param>
    /// <param name="isActive">Is the object active or not?</param>
    /// <typeparam name="T">Type derived from "Principal" class</typeparam>
    /// <exception cref="ActiveDirectoryObjectNotFoundException">If the object does not exist in the Active Directory context</exception>
    /// <exception cref="InvalidCastException">If the object is not one of the following types: UserPrincipal or ComputerPrincipal</exception>
    private void ActivateDeactivate<T>(T obj, bool isActive)
        where T : Principal
    {
        // Checks
        if (!Exists(obj)) throw new ActiveDirectoryObjectNotFoundException($"Object with name: {obj.Name} is not existing in the Active Directory context");
        
        // Setting value
        switch (obj)
        {
            case UserPrincipal u:
                u.Enabled = isActive;
                u.Save();
                break;
            
            case ComputerPrincipal c:
                c.Enabled = isActive;
                c.Save();
                break;
            
            default:
                throw new InvalidCastException($"The type used is not one of theses: {typeof(UserPrincipal)} or {typeof(ComputerPrincipal)}");
        }
    }

    /// <summary>
    /// Method used to check if an object is in a given Organizational Unit
    /// </summary>
    /// <param name="obj">Object to check</param>
    /// <param name="ouDistinguishedName">Organizational Unit Distinguished Name</param>
    /// <typeparam name="T">Type derived from "Principal" class</typeparam>
    /// <returns>
    /// If the object is in Organizational Unit or not
    /// </returns>
    /// <exception cref="ActiveDirectoryObjectNotFoundException">If the object does not exist in the Active Directory context</exception>
    private bool CheckOrganizationalUnit<T>(T obj, string ouDistinguishedName)
        where T : Principal
    {
        // Checks
        if (!Exists(obj)) throw new ActiveDirectoryObjectNotFoundException($"Object with name: {obj.Name} is not existing in the Active Directory context");
        
        // Function's return
        return obj.DistinguishedName.EndsWith(ouDistinguishedName) &&
               obj.DistinguishedName.Split(',').Count(element => element.StartsWith("OU=")) == ouDistinguishedName.Split(',').Count(element => element.StartsWith("OU="));
    }
    
    /// <summary>
    /// Method used to check if an Active Directory User is a generic account or not
    /// </summary>
    /// <param name="user">User to check</param>
    /// <returns>
    /// If the Active Directory User is a generic account or not
    /// </returns>
    /// <exception cref="ActiveDirectoryObjectNotFoundException">If the user does not exist in the Active Directory context</exception>
    private bool IsGenericAccount(UserPrincipal user)
    {
        // Checks
        if (!Exists(user)) throw new ActiveDirectoryObjectNotFoundException($"User with name: {user.Name} is not existing in the Active Directory context");
        
        // Function's return
        return user.EmployeeId.IsStringEmpty() || !user.EmployeeId.IsBelgiumNiss();
    }
    
    #endregion

    #region Publics
    
    /// <summary>
    /// Method used to retrieve full Active Directory object
    /// </summary>
    /// <param name="obj">Base Active Directory object</param>
    /// <typeparam name="T">Type derived from "Principal" class</typeparam>
    /// <returns>
    /// [Can be null] Full Active Directory object
    /// </returns>
    public DirectoryEntry? GetDirectoryEntry<T>(T obj)
        where T : Principal
    {
        // Function's return
        return obj.GetUnderlyingObject() as DirectoryEntry;
    }
    
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
    public T CreateObject<T>(string propName, object? propValue = null)
        where T : Principal
    {
        // Checks
        if (propName.IsStringEmpty()) throw new ArgumentNullException(nameof(propName), $"The value of the parameter: {nameof(propName)} cannot be null or empty");
        
        // Creating object
        Principal? p;
        if (typeof(T) == typeof(ComputerPrincipal)) 
            p = new ComputerPrincipal(_context);
        else if (typeof(T) == typeof(GroupPrincipal)) 
            p = new GroupPrincipal(_context);
        else 
            p = new UserPrincipal(_context);
        
        // Setting property value
        if (typeof(T).GetProperties().All(pi => pi.Name.ToLower() != propName.ToLower()))
            throw new NullReferenceException($"No property found with name: {propName} in type {typeof(T)}");
        typeof(T).GetProperties().First(pi => pi.Name.ToLower() == propName.ToLower()).SetValue(p, propValue);
        
        // Function's return
        return (T)p;
    }

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
    public IEnumerable<T> SearchObjects<T>(string propName, object? propValue)
        where T : Principal
    {
        // Checks
        if (propName.IsStringEmpty()) throw new ArgumentNullException(nameof(propName), $"The value of the parameter: {nameof(propName)} cannot be null or empty");
        
        // Creating search object
        Principal p = CreateObject<T>(propName, propValue);
        using PrincipalSearcher s = new PrincipalSearcher(p);
        
        // Function's return
        return s.FindAll().OfType<T>();
    }

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
    public bool Equals<T>(T obj1, T obj2, bool byName)
        where T : Principal
    {
        // Function's return
        if (byName)
            return obj1.Name == obj2.Name;
        return obj1.DistinguishedName == obj2.DistinguishedName;
    }

    /// <summary>
    /// Method used to check if an object exist
    /// </summary>
    /// <param name="obj">Object to check</param>
    /// <typeparam name="T">Type derived from "Principal" class</typeparam>
    /// <returns>
    /// If the object exist or not
    /// </returns>
    public bool Exists<T>(T obj)
        where T : Principal
    {
        // Function's return
        using PrincipalSearcher s = new PrincipalSearcher(obj);
        return s.FindAll().FirstOrDefault() is not null;
    }

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
    public bool IsEnabled<T>(T obj)
        where T : Principal
    {
        // Checks
        if (!Exists(obj)) throw new ActiveDirectoryObjectNotFoundException($"Object with name: {obj.Name} is not existing in the Active Directory context");
        
        // Function's return
        return typeof(T) == typeof(UserPrincipal)
            ? ((UserPrincipal)Convert.ChangeType(obj, typeof(UserPrincipal))).Enabled ?? false
            : typeof(T) == typeof(ComputerPrincipal)
                ? ((ComputerPrincipal)Convert.ChangeType(obj, typeof(ComputerPrincipal))).Enabled ?? false
                : throw new InvalidCastException($"The type used is not one of theses: {typeof(UserPrincipal)} or {typeof(ComputerPrincipal)}");
    }

    /// <summary>
    /// Method used to activate an Active Directory object
    /// </summary>
    /// <param name="obj">Object to activate</param>
    /// <typeparam name="T">Type derived from "Principal" class</typeparam>
    /// <exception cref="ActiveDirectoryObjectNotFoundException">If the object does not exist in the Active Directory context</exception>
    /// <exception cref="InvalidCastException">If the object is not one of the following types: UserPrincipal or ComputerPrincipal</exception>
    public void Activate<T>(T obj)
        where T : Principal
    {
        ActivateDeactivate(obj, true);
    }

    /// <summary>
    /// Method used to deactivate an Active Directory object
    /// </summary>
    /// <param name="obj">Object to deactivate</param>
    /// <typeparam name="T">Type derived from "Principal" class</typeparam>
    /// <exception cref="ActiveDirectoryObjectNotFoundException">If the object does not exist in the Active Directory context</exception>
    /// <exception cref="InvalidCastException">If the object is not one of the following types: UserPrincipal or ComputerPrincipal</exception>
    public void Deactivate<T>(T obj)
        where T : Principal
    {
        ActivateDeactivate(obj, false);
    }

    /// <summary>
    /// Method used to move an Active Directory object from an Organizational Unit to another one
    /// </summary>
    /// <param name="obj">Object to move</param>
    /// <param name="newDistinguishedName">New distinguished name of the object</param>
    /// <param name="newName">[Can be null] New name of the object</param>
    /// <typeparam name="T">Type derived from "Principal" class</typeparam>
    /// <exception cref="ActiveDirectoryObjectNotFoundException">If the object does not exist in the Active Directory context</exception>
    /// <exception cref="ArgumentNullException">If the new Distinguished Name of the object is null or empty</exception>
    public void MoveObject<T>(T obj, string newDistinguishedName, string? newName = null)
        where T : Principal
    {
        // Checks
        if (!Exists(obj)) throw new ActiveDirectoryObjectNotFoundException($"Object with name: {obj.Name} is not existing in the Active Directory context");
        if (newDistinguishedName.IsStringEmpty()) throw new ArgumentNullException(nameof(newDistinguishedName), $"The value of the parameter: {nameof(newDistinguishedName)} cannot be null or empty");
        
        // Move object
        using DirectoryEntry from = new DirectoryEntry($"LDAP://{obj.DistinguishedName}");
        using DirectoryEntry to = new DirectoryEntry($"LDAP://{newDistinguishedName}");
        from.MoveTo(to, newName.IsStringEmpty() ? from.Name : newName);
        to.Close();
        from.Close();
    }

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
    public IEnumerable<T> GetObjectsFromOrganizationalUnit<T>(string ouDistinguishedName, bool checkAllTree)
        where T : Principal
    {
        // Checks
        if (ouDistinguishedName.IsStringEmpty()) throw new ArgumentNullException(nameof(ouDistinguishedName), $"The value of the parameter: {nameof(ouDistinguishedName)} cannot be null or empty");
        
        // Creating searcher
        Principal p = CreateObject<T>("Name");
        using PrincipalSearcher s = new PrincipalSearcher(p);
        
        // Function's return
        return s.FindAll()
            .Where(pr => checkAllTree ? pr.DistinguishedName.EndsWith(ouDistinguishedName) : CheckOrganizationalUnit(pr, ouDistinguishedName))
            .OfType<T>();
    }

    /// <summary>
    /// Method used to retrieve all members in an Active Directory Group
    /// </summary>
    /// <param name="group">Group to check</param>
    /// <returns>
    /// List of members
    /// </returns>
    /// <exception cref="ActiveDirectoryObjectNotFoundException">If the group does not exist in the Active Directory context</exception>
    public IEnumerable<Principal> GetGroupMembers(GroupPrincipal group)
    {
        // Checks
        if (!Exists(group)) throw new ActiveDirectoryObjectNotFoundException($"Group with name: {group.Name} is not existing in the Active Directory context");
        
        // Function's return
        return group.Members;
    }
    
    /// <summary>
    /// Method used to retrieve all members of a type in an Active Directory Group
    /// </summary>
    /// <param name="group">Group to check</param>
    /// <typeparam name="T">Type derived from "Principal" class</typeparam>
    /// <returns>
    /// List of members
    /// </returns>
    /// <exception cref="ActiveDirectoryObjectNotFoundException">If the group does not exist in the Active Directory context</exception>
    public IEnumerable<T> GetGroupMembers<T>(GroupPrincipal group)
        where T : Principal
    {
        // Checks
        if (!Exists(group)) throw new ActiveDirectoryObjectNotFoundException($"Group with name: {group.Name} is not existing in the Active Directory context");
        
        // Function's return
        return group.Members.OfType<T>();
    }

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
    public bool IsGroupMember<T>(GroupPrincipal group, T obj)
        where T : Principal
    {
        // Checks
        if (!Exists(group)) throw new ActiveDirectoryObjectNotFoundException($"Group with name: {group.Name} is not existing in the Active Directory context");
        if (!Exists(obj)) throw new ActiveDirectoryObjectNotFoundException($"Group with name: {obj.Name} is not existing in the Active Directory context");
        
        // Function's return
        return group.Members.OfType<T>().Any(m => m.Name == obj.Name);
    }

    /// <summary>
    /// Method used to add an Active Directory Object in an Active Directory Group
    /// </summary>
    /// <param name="group">Group where member will be added</param>
    /// <param name="obj">Object to be added to the group</param>
    /// <typeparam name="T">Type derived from "Principal" class</typeparam>
    /// <exception cref="ActiveDirectoryObjectNotFoundException">If the group does not exist in the Active Directory context</exception>
    /// <exception cref="ActiveDirectoryObjectNotFoundException">If the object does not exist in the Active Directory context</exception>
    /// <exception cref="Exception">If the object to be added is already a member of the group</exception>
    public void AddGroupMember<T>(GroupPrincipal group, T obj)
        where T : Principal
    {
        // Checks
        if (!Exists(group)) throw new ActiveDirectoryObjectNotFoundException($"Group with name: {group.Name} is not existing in the Active Directory context");
        if (!Exists(obj)) throw new ActiveDirectoryObjectNotFoundException($"Group with name: {obj.Name} is not existing in the Active Directory context");
        if (IsGroupMember(group, obj)) throw new Exception($"Element: {obj.Name} already existing in group: {group.Name}");
        
        // Adding member
        group.Members.Add(obj);
        group.Save();
    }

    /// <summary>
    /// Method used to remove an Active Directory Object in an Active Directory Group
    /// </summary>
    /// <param name="group">Group where member will be removed</param>
    /// <param name="obj">Object to be removed to the group</param>
    /// <typeparam name="T">Type derived from "Principal" class</typeparam>
    /// <exception cref="ActiveDirectoryObjectNotFoundException">If the group does not exist in the Active Directory context</exception>
    /// <exception cref="ActiveDirectoryObjectNotFoundException">If the object does not exist in the Active Directory context</exception>
    /// <exception cref="Exception">If the object to be added is already a member of the group</exception>
    public void RemoveGroupMember<T>(GroupPrincipal group, T obj)
        where T : Principal
    {
        // Checks
        if (!Exists(group)) throw new ActiveDirectoryObjectNotFoundException($"Group with name: {group.Name} is not existing in the Active Directory context");
        if (!Exists(obj)) throw new ActiveDirectoryObjectNotFoundException($"Group with name: {obj.Name} is not existing in the Active Directory context");
        if (!IsGroupMember(group, obj)) throw new Exception($"Element: {obj.Name} not existing in group: {group.Name}");
        
        // Removing member
        group.Members.Remove(obj);
        group.Save();
    }

    /// <summary>
    /// Method used to retrieve generics accounts from a list
    /// </summary>
    /// <param name="list">List to check</param>
    /// <typeparam name="T">Type derived from "Principal" class</typeparam>
    /// <returns>
    /// List of generic accounts founded
    /// </returns>
    /// <exception cref="ArgumentNullException">If the list is null or empty</exception>
    public IEnumerable<UserPrincipal> GetGenericAccountsFromAList<T>(IEnumerable<T> list)
        where T : Principal
    {
        // Checks
        if (!list.Any()) throw new ArgumentNullException(nameof(list), $"The list: {nameof(list)} cannot be null or empty");
        
        // Function's return
        return list.OfType<UserPrincipal>().Where(IsGenericAccount);
    }

    /// <summary>
    /// Method used to retrieve nominated accounts from a list
    /// </summary>
    /// <param name="list">List to check</param>
    /// <typeparam name="T">Type derived from "Principal" class</typeparam>
    /// <returns>
    /// List of nominated accounts founded
    /// </returns>
    /// <exception cref="ArgumentNullException">If the list is null or empty</exception>
    public IEnumerable<UserPrincipal> GetNominatedAccountsFromAList<T>(IEnumerable<T> list)
        where T : Principal
    {
        // Checks
        if (!list.Any()) throw new ArgumentNullException(nameof(list), $"The list: {nameof(list)} cannot be null or empty");

        // Function's return
        return list.OfType<UserPrincipal>().Where(u => !IsGenericAccount(u));
    }

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
    public bool CheckPasswordComplexity(UserPrincipal user, string password)
    {
        // Checks
        if (!Exists(user)) throw new ActiveDirectoryObjectNotFoundException($"Group with name: {user.Name} is not existing in the Active Directory context");
        if (password.IsStringEmpty()) throw new ArgumentNullException(nameof(password), $"The value of the parameter: {nameof(password)} cannot be null or empty");
        
        // Check password
        bool passwordLength = password.Length >= 12;
        bool passwordContainsWords = !(password.Contains(user.SamAccountName) || password.Contains(user.GivenName) || password.Contains(user.Surname) || password.ToLower().Contains("bsca"));
        int charSpecs = 0;
        if (Regex.Match(password, @"(?=.*[a-z])").Success)
            charSpecs++;
        if (Regex.Match(password, @"(?=.*[A-Z])").Success)
            charSpecs++;
        if (Regex.Match(password, @"(?=.*\d)").Success)
            charSpecs++;
        if (Regex.Match(password, @"(?=.*\W)").Success)
            charSpecs++;
        if (Regex.Match(password, @"(?=.*\s)").Success)
            charSpecs = 0;
        
        // Function's return
        return passwordLength && passwordContainsWords && charSpecs >= 3;
    }
    
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
    public void ChangePassword(UserPrincipal user, string newPassword, string? currentPassword = null, bool passwordMustBeChangedAtNextLogon = true)
    {
        // Checks
        if (!Exists(user)) throw new ActiveDirectoryObjectNotFoundException($"Group with name: {user.Name} is not existing in the Active Directory context");
        if (newPassword.IsStringEmpty()) throw new ArgumentNullException(nameof(newPassword), $"The value of the parameter: {nameof(newPassword)} cannot be null or empty");
        
        // Authenticate if necessary
        if (!currentPassword.IsStringEmpty())
            if (!_context.ValidateCredentials(user.SamAccountName, currentPassword))
                throw new AuthenticationException($"Given old password is not the current password for the account {user.SamAccountName}"); 
        
        // Change password
        if (!CheckPasswordComplexity(user, newPassword)) throw new Exception($"New password not meeting BSCA password complexity requirements"); 
        user.SetPassword(newPassword);
        if (passwordMustBeChangedAtNextLogon) user.ExpirePasswordNow();
        user.Save();
    }

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
    public Tuple<UserPrincipal?, Exception?> Login(string username, string password, bool authorizeGenericAccount = false)
    {
        // Getting user
        UserPrincipal? user = default;
        try
        {
            // Checks
            if (username.IsStringEmpty()) throw new ArgumentNullException(nameof(username), $"The value of the parameter: {nameof(username)} cannot be null or empty");
            if (password.IsStringEmpty()) throw new ArgumentNullException(nameof(password), $"The value of the parameter: {nameof(password)} cannot be null or empty");
            
            // Login user
            user = SearchObjects<UserPrincipal>("SamAccountName", username).FirstOrDefault();
            if (user is null) throw new ActiveDirectoryObjectNotFoundException($"No user found with SamAccountName: {username}");
            if (user.IsAccountLockedOut()) throw new AuthenticationException("Account is locked");
            if (user.Enabled is null or false) throw new AuthenticationException("Account is not active");
            if (!authorizeGenericAccount && IsGenericAccount(user)) throw new AuthenticationException($"User's account is a generic one");
            if (!_context.ValidateCredentials(username, password)) throw new AuthenticationException("Incorrect username/password combination");
            
            // Function's return
            return new Tuple<UserPrincipal?, Exception?>(user, null);
        }
        catch (Exception ex)
        {
            // Exception's return
            return new Tuple<UserPrincipal?, Exception?>(user, ex);
        }
    }
    
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
    public UserPrincipal CreateUser(string samAccountName, Dictionary<string, object?> properties, List<string>? groupNames = null)
    {
        // Checks
        if (samAccountName.IsStringEmpty()) throw new ArgumentNullException(nameof(samAccountName), $"The value of the parameter: {nameof(samAccountName)} cannot be null or empty");
        if (Exists(CreateObject<UserPrincipal>("SamAccountName", samAccountName))) throw new Exception($"User with SamAccountName: {samAccountName} already exists in the Active Directory");
        if (properties.Keys.All(k => k.ToLower() != "password") && properties[properties.Keys.First(k => k.ToLower() == "password")] is null) throw new ArgumentNullException($"The dictionary: {nameof(properties)} must contains a key named 'Password' with a value");
        if (properties.Keys.All(k => k.ToLower() != "passwordneverexpires") && properties[properties.Keys.First(k => k.ToLower() == "passwordneverexpires")] is null) throw new ArgumentNullException($"The dictionary: {nameof(properties)} must contains a key named 'PasswordNeverExpires' with a value");
        if (groupNames is not null && groupNames.Any() && groupNames.Any(g => !Exists(CreateObject<GroupPrincipal>("Name", g)))) throw new ActiveDirectoryObjectNotFoundException("At least one group is not existing in the Active Directory");
        
        // Creating user
        UserPrincipal user = CreateObject<UserPrincipal>("SamAccountName", samAccountName);
        foreach (KeyValuePair<string, object?> property in properties)
        {
            PropertyInfo? pi = typeof(UserPrincipal).GetProperties().FirstOrDefault(p => p.Name.ToLower() == property.Key.ToLower());
            if (pi is not null)
                pi.SetValue(user, property.Value);
        }
        user.Save();
        
        // Setting password
        string password = properties.First(prop => prop.Key.ToLower() == "password").Value!.ToString()!;
        if (!CheckPasswordComplexity(user, password)) throw new Exception("Password not meets the BSCA password complexity");
        user.SetPassword(password);
        if (!user.PasswordNeverExpires) user.ExpirePasswordNow();
        user.Save();
        
        // Adding in groups
        if (groupNames is not null && groupNames.Any())
            foreach (string groupName in groupNames)
                AddGroupMember(SearchObjects<GroupPrincipal>("Name", groupName).First(), user);
        user.Save();
        
        // Function's return
        return user;
    }

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
    public GroupPrincipal CreateGroup(string name, Dictionary<string, object?> properties, List<string>? groupNames = null)
    {
        // Checks
        if (name.IsStringEmpty()) throw new ArgumentNullException(nameof(name), $"The value of the parameter: {nameof(name)} cannot be null or empty");
        if (Exists(CreateObject<GroupPrincipal>("SamAccountName", name))) throw new Exception($"Group with Name: {name} already exists in the Active Directory");
        if (groupNames is not null && groupNames.Any() && groupNames.Any(g => !Exists(CreateObject<GroupPrincipal>("Name", g)))) throw new ActiveDirectoryObjectNotFoundException("At least one group is not existing in the Active Directory");
        
        // Creating group
        GroupPrincipal group = CreateObject<GroupPrincipal>("Name", name);
        foreach (KeyValuePair<string, object?> property in properties)
        {
            PropertyInfo? pi = typeof(GroupPrincipal).GetProperties().FirstOrDefault(p => p.Name.ToLower() == property.Key.ToLower());
            if (pi is not null)
                pi.SetValue(group, property.Value);
        }
        group.Save();
        
        // Adding in groups
        if (groupNames is not null && groupNames.Any())
            foreach (string groupName in groupNames)
                AddGroupMember(SearchObjects<GroupPrincipal>("Name", groupName).First(), group);
        group.Save();
        
        // Function's return
        return group;
    }

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
    public ComputerPrincipal CreateComputer(string name, Dictionary<string, object?> properties, List<string>? groupNames = null)
    {
        // Checks
        if (name.IsStringEmpty()) throw new ArgumentNullException(nameof(name), $"The value of the parameter: {nameof(name)} cannot be null or empty");
        if (Exists(CreateObject<GroupPrincipal>("SamAccountName", name))) throw new Exception($"Group with Name: {name} already exists in the Active Directory");
        if (groupNames is not null && groupNames.Any() && groupNames.Any(g => !Exists(CreateObject<GroupPrincipal>("Name", g)))) throw new ActiveDirectoryObjectNotFoundException("At least one group is not existing in the Active Directory");
        
        // Creating group
        ComputerPrincipal computer = CreateObject<ComputerPrincipal>("Name", name);
        foreach (KeyValuePair<string, object?> property in properties)
        {
            PropertyInfo? pi = typeof(ComputerPrincipal).GetProperties().FirstOrDefault(p => p.Name.ToLower() == property.Key.ToLower());
            if (pi is not null)
                pi.SetValue(computer, property.Value);
        }
        computer.Save();
        
        // Adding in groups
        if (groupNames is not null && groupNames.Any())
            foreach (string groupName in groupNames)
                AddGroupMember(SearchObjects<GroupPrincipal>("Name", groupName).First(), computer);
        computer.Save();
        
        // Function's return
        return computer;
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

// Pragma END
#pragma warning restore CA1416 // Validate platform compatibility
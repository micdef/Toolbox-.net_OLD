// Using
using System.Data;

// Static using
// #N/A

// Named using
// #N/A

// Namespace
namespace Toolbox.Attributes;

/// <summary>
/// Attribute used to map a property to a SQL parameter
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class ToSqlParameterAttribute : Attribute
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

    /// <summary>
    /// Name of the parameter
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Direction of the parameter
    /// </summary>
    public ParameterDirection Direction { get; set; }

    /// <summary>
    /// SQL database type
    /// </summary>
    public DbType Type { get; set; }

    /// <summary>
    /// [In case of string type] Size of the string
    /// </summary>
    public int Size { get; set; }

    #endregion

    #region Delegates

    // #N/A

    #endregion

    #endregion

    #region Constructors

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="name">Name of the parameter</param>
    /// <param name="direction">Direction of the parameter</param>
    /// <param name="type">SQL database type</param>
    /// <param name="size">[In case of string type] Size of the string</param>
    public ToSqlParameterAttribute(string name, ParameterDirection direction, DbType type, int size = 0)
    {
        Name = name;
        Direction = direction;
        Type = type;
        Size = size;
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

    // #N/A

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
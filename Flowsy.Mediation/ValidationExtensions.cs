using System.Linq.Expressions;
using System.Reflection;
using FluentValidation;

namespace Flowsy.Mediation;

/// <summary>
/// Extension methods used in validations.
/// </summary>
public static class ValidationExtensions
{
    /// <summary>
    /// Gets the property name for validation.
    /// </summary>
    /// <param name="memberInfo">
    /// An instance of <see cref="MemberInfo"/>.
    /// </param>
    /// <param name="expression">
    /// An instance of <see cref="LambdaExpression"/>.
    /// </param>
    /// <returns>
    /// A string representing the property name.
    /// </returns>
    public static string GetPropertyNameForValidation(this MemberInfo memberInfo, LambdaExpression? expression = null)
    {
        try
        {
            return ValidatorOptions.Global.PropertyNameResolver(memberInfo.DeclaringType, memberInfo, expression);
        }
        catch (Exception exception)
        {
            Console.WriteLine(exception);
            return memberInfo.Name;
        }
    }

    /// <summary>
    /// Gets the property name for validation.
    /// </summary>
    /// <param name="type">
    /// An instance of <see cref="Type"/>.
    /// </param>
    /// <param name="propertyName">
    /// The name of a property belonging to the type.
    /// </param>
    /// <param name="expression">
    /// An instance of <see cref="LambdaExpression"/>.
    /// </param>
    /// <returns>
    /// A string representing the property name.
    /// </returns>
    public static string GetPropertyNameForValidation(this Type type, string propertyName, LambdaExpression? expression = null)
    {
        var propertyInfo = type.GetProperty(propertyName);
        return propertyInfo is not null
            ? propertyInfo.GetPropertyNameForValidation(expression)
            : propertyName;
    }
}
using System.Linq.Expressions;
using System.Reflection;
using FluentValidation;
using Serilog;

namespace Flowsy.Mediation;

public static class ValidationExtensions
{
    private static ILogger? _logger;
    private static ILogger Logger => _logger ??= Log.ForContext(typeof(ValidationExtensions));

    public static string GetPropertyNameForValidation(this MemberInfo memberInfo, LambdaExpression? expression = null)
    {
        try
        {
            return ValidatorOptions.Global.PropertyNameResolver(memberInfo.DeclaringType, memberInfo, expression);
        }
        catch (Exception exception)
        {
            Logger.Error(
                exception,
                "Could not resolve property name for validation: {Type}.{PropertyName}",
                memberInfo.DeclaringType?.Name,
                memberInfo.Name
            );
            return memberInfo.Name;
        }
    }

    public static string GetPropertyNameForValidation(this Type type, string propertyName, LambdaExpression? expression = null)
    {
        var propertyInfo = type.GetProperty(propertyName);
        return propertyInfo is not null
            ? propertyInfo.GetPropertyNameForValidation(expression)
            : propertyName;
    }
}
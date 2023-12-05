using System.Linq.Expressions;
using System.Reflection;
using FluentValidation;

namespace Flowsy.Mediation;

public static class ValidationExtensions
{
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

    public static string GetPropertyNameForValidation(this Type type, string propertyName, LambdaExpression? expression = null)
    {
        var propertyInfo = type.GetProperty(propertyName);
        return propertyInfo is not null
            ? propertyInfo.GetPropertyNameForValidation(expression)
            : propertyName;
    }
}
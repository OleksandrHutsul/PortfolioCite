using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.AspNetCore.Components;

namespace PortfolioCite.App.Components.Shared;

public partial class RequiredIndicatorComponent<TValue> : ComponentBase
{
    private static readonly Dictionary<PropertyInfo, bool> RequiredByProperty = [];

    [Parameter, EditorRequired] public Expression<Func<TValue>> For { get; set; } = default!;

    private bool IsRequired => IsRequiredProperty(For);

    private static bool IsRequiredProperty(LambdaExpression expression)
    {
        var property = GetProperty(expression);

        if (property is null)
            return false;

        if (RequiredByProperty.TryGetValue(property, out var required))
            return required;

        required = Attribute.IsDefined(property, typeof(RequiredAttribute)) || IsRequiredDate(property);
        RequiredByProperty[property] = required;

        return required;
    }

    private static bool IsRequiredDate(PropertyInfo property)
    {
        if (Nullable.GetUnderlyingType(property.PropertyType) is not null)
            return false;

        return property.PropertyType == typeof(DateOnly) || property.PropertyType == typeof(DateTime) || property.PropertyType == typeof(DateTimeOffset);
    }

    private static PropertyInfo? GetProperty(LambdaExpression expression)
    {
        var body = expression.Body;

        while (body is UnaryExpression { NodeType: ExpressionType.Convert or ExpressionType.ConvertChecked } unary)
            body = unary.Operand;

        return body is MemberExpression { Member: PropertyInfo property } ? property : null;
    }
}

using System.Reflection;

internal static class Extensions
{
    public static T? GetCustomAttribute<T>(this MemberInfo info, bool inherit = true)
        where T : Attribute
    {
        var attributes = info.GetCustomAttributes(typeof(T), inherit);
        return attributes.SingleOrDefault() as T;
    }
}

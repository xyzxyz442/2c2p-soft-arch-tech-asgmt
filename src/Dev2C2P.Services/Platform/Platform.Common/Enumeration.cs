using System.Reflection;

namespace Dev2C2P.Services.Platform.Common;

public abstract class Enumeration : IComparable
{
    protected static string Error => "Possible values for {0}: {1}";

    public string Name { get; }

    public int Id { get; }

    protected Enumeration(int id, string name) => (Id, Name) = (id, name);

    public static int AbsoluteDifference(Enumeration firstValue, Enumeration secondValue)
        => Math.Abs(firstValue.Id - secondValue.Id);

    public static T FromValue<T>(int value) where T : Enumeration
        => Parse<T, int>(value, "value", item => item.Id == value);

    public static T FromDisplayName<T>(string displayName) where T : Enumeration
        => Parse<T, string>(displayName, "display name", item => item.Name == displayName);

    public static IEnumerable<T> GetAll<T>() where T : Enumeration
        => typeof(T).GetFields(BindingFlags.Public |
                            BindingFlags.Static |
                            BindingFlags.DeclaredOnly)
                    .Select(f => f.GetValue(null))
                    .Cast<T>();

    private static T Parse<T, K>(K value, string description, Func<T, bool> predicate) where T : Enumeration
    {
        var matchingItem = GetAll<T>().FirstOrDefault(predicate);

        if (matchingItem == null)
            throw new InvalidOperationException($"'{value}' is not a valid {description} in {typeof(T)}");

        return matchingItem;
    }

    public override int GetHashCode() => Id.GetHashCode();

    public override string ToString() => Name;

    public int CompareTo(object? obj) => Id.CompareTo(((Enumeration)obj!).Id);

    public override bool Equals(object obj)
    {
        if (obj is not Enumeration otherValue)
            return false;

        var typeMatches = GetType().Equals(obj.GetType());
        var valueMatches = Id.Equals(otherValue.Id);

        return typeMatches && valueMatches;
    }

    public static bool operator ==(Enumeration? left, Enumeration? right)
    {
        if (left is null && right is null)
            return true;

        if (left is null || right is null)
            return false;

        return left.Equals(right);
    }

    public static bool operator !=(Enumeration? left, Enumeration? right) => !(left == right);
}

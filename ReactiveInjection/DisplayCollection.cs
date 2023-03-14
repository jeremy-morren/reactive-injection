namespace ReactiveInjection;

public class DisplayCollection<T>
{
    private readonly List<T> _source;

    public DisplayCollection(IEnumerable<T> source) => _source = source.ToList();

    public override string ToString() => $"[ {string.Join(", ", _source.Select(s => s?.ToString() ?? string.Empty))} ]";
}

public static class DisplayCollectionHelpers
{
    public static DisplayCollection<T> ToDisplayCollection<T>(this IEnumerable<T> source) => new (source);
}
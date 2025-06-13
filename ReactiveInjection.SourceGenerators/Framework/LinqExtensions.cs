namespace ReactiveInjection.SourceGenerators.Framework;

internal static class LinqExtensions
{
    public static IEnumerable<T> WhereNotNull<T>(this IEnumerable<T?> source) => source.Where(e => e != null)!;
    
    public static IEnumerable<TKey> Duplicates<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> selector) =>
        source.GroupBy(selector)
            .Where(g => g.Count() > 1)
            .Select(g => g.Key);
    
    public static void AddRange<T>(this HashSet<T> set, IEnumerable<T> items)
    {
        foreach (var item in items)
            set.Add(item);
    }
}
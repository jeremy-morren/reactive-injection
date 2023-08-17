﻿namespace ReactiveInjection;

public static class LinqExtensions
{
    public static IEnumerable<T> WhereNotNull<T>(this IEnumerable<T?> source) => source.Where(e => e != null)!;
}
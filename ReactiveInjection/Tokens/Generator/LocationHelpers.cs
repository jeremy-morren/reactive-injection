using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

namespace ReactiveInjection.Tokens.Generator;

internal static class LocationHelpers
{
    public static Location GetLocation(this ImmutableArray<Location> locations)
    {
        if (locations.IsEmpty) return Location.None;
        return locations.FirstOrDefault(l => l.IsInSource) ?? locations.First();
    }
}
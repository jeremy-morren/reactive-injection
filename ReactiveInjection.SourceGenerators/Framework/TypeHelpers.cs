﻿using Microsoft.CodeAnalysis;
using ReactiveInjection.SourceGenerators.Symbols;

namespace ReactiveInjection.SourceGenerators.Framework;

internal static class TypeHelpers
{
    public static bool IsTask(this IType type, out IType argument)
    {
        if (type is { Name: "Task", Namespace: "System.Threading.Tasks" } && type.GenericArguments.Count() == 1)
        {
            argument = type.GenericArguments.First();
            return true;
        }

        argument = null!;
        return false;
    }
    
    public static bool IsCancellationToken(this IType type)
    {
        return type is { Name: "CancellationToken", Namespace: "System.Threading" };
    }

    public static bool IsPublic(this Accessibility accessibility) =>
        accessibility is Accessibility.Public or Accessibility.Internal or Accessibility.ProtectedOrInternal;
}
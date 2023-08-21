﻿using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ReactiveInjection.Framework;

internal static class SyntaxHelpers
{
    public static bool IsPartial(this TypeDeclarationSyntax type) =>
        type.Modifiers.Any(t => t.IsKind(SyntaxKind.PartialKeyword));
}
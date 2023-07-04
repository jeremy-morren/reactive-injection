using System;
using System.Linq;
using System.Reflection;
using Microsoft.CodeAnalysis;

namespace ReactiveInjection.Tokens.Generator;

internal class TypeSymbol : IType
{
    private readonly ITypeSymbol _source;
    private readonly bool? _isPartial;

    public TypeSymbol(ITypeSymbol source, bool? isPartial = null)
    {
        _source = source;
        _isPartial = isPartial;
    }

    public Location Location => _source.Locations.GetLocation();

    public IAssembly Assembly => new AssemblySymbol(_source.ContainingAssembly);

    public string? Namespace => _source.ContainingNamespace?.ToString();

    public string Name => _source.Name;

    public string FullName
    {
        get
        {
            var name = _source.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
            const string global = "global::";
            if (name.StartsWith(global))
                name = name.Substring(global.Length);
            return name;
        }
    }

    public bool IsValueType => _source.IsValueType;
    public bool IsAbstract => _source.IsAbstract;

    public bool IsPartial => _isPartial ?? throw new NotImplementedException();

    public IType[] GetGenericArguments() => throw new NotImplementedException();

    public IMethod[] GetMethods() => _source.GetMembers()
        .Where(s => s is IMethodSymbol
            {
                MethodKind: MethodKind.Ordinary,
                DeclaredAccessibility: Accessibility.Public
            })
        .Select(m => (IMethod) new MethodSymbol(m))
        .ToArray();

    public IConstructor[] GetConstructors() => _source.GetMembers()
        .Where(s => s is IMethodSymbol {MethodKind: MethodKind.Constructor})
        .Select(m => (IConstructor) new ConstructorSymbol(m))
        .ToArray();

    public string CSharpName => _source.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);

    public override string ToString() => _source.ToDisplayString(SymbolDisplayFormat.CSharpErrorMessageFormat);

    #region Equality

    public bool Equals(IType? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        if (other is not TypeSymbol o) return false;
        return o._source.Equals(_source, SymbolEqualityComparer.Default);
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj is not TypeSymbol o) return false;
        return o._source.Equals(_source, SymbolEqualityComparer.Default);
    }

    public override int GetHashCode() => SymbolEqualityComparer.Default.GetHashCode(_source);

    #endregion
}
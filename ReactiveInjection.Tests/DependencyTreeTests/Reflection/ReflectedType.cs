using System.Reflection;
using System.Text;
using ReactiveInjection.Tokens;

namespace ReactiveInjection.Tests.DependencyTreeTests.Reflection;

internal class ReflectedType : ReflectedTokenBase, IType
{
    private readonly Type _type;

    public ReflectedType(Type type) => _type = type;
    public IAssembly Assembly => new ReflectedAssembly(_type.Assembly);

    public string? Namespace => _type.Namespace;
    
    public string Name => _type.Name;
    
    public string FullName => _type.FullName ?? _type.Name;
    
    public IType? DeclaringType => _type.DeclaringType != null ? new ReflectedType(_type.DeclaringType) : null;
    
    public bool IsValueType => _type.IsValueType;
    
    public bool IsAbstract => _type.IsAbstract;

    public bool IsPartial => false;

    public IType[] GetGenericArguments() => _type.IsGenericType
        ? _type.GetGenericArguments().Select(t => (IType)new ReflectedType(t)).ToArray()
        : throw new InvalidOperationException("Type is not a generic type");

    
    public IMethod[] GetMethods() => _type.GetMethods(BindingFlags.Public | BindingFlags.Instance)
        .Select(m => (IMethod)new ReflectedMethod(m))
        .ToArray();


    public IConstructor[] GetConstructors() => _type.GetConstructors(BindingFlags.Public | BindingFlags.Instance)
        .Select(c => (IConstructor) new ReflectedConstructor(_type, c))
        .ToArray();

    public override string ToString() => $"{FullName}, {Assembly}";

    public string CSharpName
    {
        get
        {
            if (!_type.IsGenericType)
                return $"global::{_type.Namespace}.{_type.Name}";
            var n = _type.Name.Substring(0, _type.Name.IndexOf('`')); //Get name without `
            var sb = new StringBuilder($"global::{_type.Namespace}.{n}");
            sb.Append('<');
            foreach (var param in _type.GetGenericArguments())
                sb.Append($"{new ReflectedType(param).CSharpName},");
            sb[sb.Length - 1] = '>'; //Replace trailing ',' with '>'
            return sb.ToString();
        }
    }
    
    
    #region Equality

    public bool Equals(IType? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        if (other is not ReflectedType t) return false;
        return t._type == _type;
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj is not ReflectedType other) return false;
        return other._type == _type;
    }

    public override int GetHashCode() => ToString().GetHashCode();
    
    #endregion
}
using System.Reflection;
using System.Text;
using ReactiveInjection.Tokens;

namespace ReactiveInjection.Tests.DependencyTreeTests.Reflection;

internal class ReflectedType : ReflectedTokenBase, IType
{
    private readonly Type _type;
    private readonly bool? _isPartial;

    public ReflectedType(Type type, bool? isPartial = null)
    {
        _type = type;
        _isPartial = isPartial;
    }

    public IAssembly Assembly => new ReflectedAssembly(_type.Assembly);

    public string? Namespace => _type.Namespace;
    
    public string Name => _type.Name;

    public string FullName => $"{_type.Namespace}.{_type.Name}";
    
    public bool IsValueType => _type.IsValueType;
    
    public bool IsAbstract => _type.IsAbstract;

    public bool IsPartial => _isPartial ?? throw new NotImplementedException();

    public IEnumerable<IAttribute> GetAttributes() => _type.GetCustomAttributes(false)
        .Select(a => (IAttribute)new ReflectedAttribute(a))
        .ToArray();

    public IConstructor[] GetConstructors() => _type.GetConstructors(BindingFlags.Public | BindingFlags.Instance)
        .Select(c => (IConstructor) new ReflectedConstructor(_type, c))
        .ToArray();

    public override string ToString() => CSharpName;

    public string CSharpName
    {
        get
        {
            if (!_type.IsGenericType)
                return $"global::{_type.FullName}";
            var n = _type.Name[.._type.Name.IndexOf('`')]; //Get name without `
            var sb = new StringBuilder($"global::{_type.Namespace}.{n}");
            sb.Append('<');
            foreach (var param in _type.GetGenericArguments())
                sb.Append($"{new ReflectedType(param).CSharpName},");
            sb[^1] = '>'; //Replace trailing ',' with '>'
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
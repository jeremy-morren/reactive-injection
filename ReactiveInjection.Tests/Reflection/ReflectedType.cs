using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Text;
using ReactiveInjection.Symbols;

namespace ReactiveInjection.Tests.Reflection;

internal class ReflectedType : ReflectedTokenBase, IType
{
    private readonly Type _type;
    private readonly bool? _isPartial;
    private readonly bool? _isNullable;

    public ReflectedType(Type type, 
        bool? isPartial = null,
        bool? isNullable = null)
    {
        _type = type;
        _isPartial = isPartial;
        _isNullable = isNullable;
    }

    public IAssembly Assembly => new ReflectedAssembly(_type.Assembly);

    public IType? ContainingType => _type.DeclaringType != null ? new ReflectedType(_type.DeclaringType) : null;

    public string? Namespace => _type.Namespace;
    
    public string Name => _type.Name;

    public string FullName => $"{_type.Namespace}.{_type.Name}";

    public bool IsReferenceType => !_type.IsValueType;
    
    public bool IsAbstract => _type.IsAbstract;

    public bool IsPartial => _isPartial ?? throw new NotImplementedException($"{nameof(_isPartial)} not set");

    public bool IsGenericType => _type.IsGenericType;

    public bool IsNullable => _isNullable ?? throw new NotImplementedException($"{nameof(_isNullable)} not set");

    public IEnumerable<IAttribute> Attributes => _type.GetCustomAttributes(false)
        .Select(a => new ReflectedAttribute(a));

    public IEnumerable<IConstructor> Constructors => _type.GetConstructors(BindingFlags.Public | BindingFlags.Instance)
        .Select(c => new ReflectedConstructor(_type, c));

    public IEnumerable<IProperty> Properties
    {
        get
        {
            var instance = _type
                .GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            
            var @static = _type
                .GetProperties(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
                
            return instance.Concat(@static).Select(p => new ReflectedProperty(p));
        }
    }

    public override string ToString() => CSharpName;

    public string CSharpName
    {
        get
        {
            if (!_type.IsGenericType || Nullable.GetUnderlyingType(_type) != null)
                return LangKeyword(_type);
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

    [SuppressMessage("ReSharper", "ConvertIfStatementToReturnStatement")]
    private static string LangKeyword(Type type)
    {
        var nullable = Nullable.GetUnderlyingType(type) != null ? "?" : string.Empty;
        var nt = Nullable.GetUnderlyingType(type) ?? type;
        
        if (nt == typeof(int)) return $"int{nullable}";
        if (nt == typeof(string)) return $"string{nullable}";
        if (nt == typeof(double)) return $"double{nullable}";
        if (nt == typeof(bool)) return $"bool{nullable}";
        return $"global::{type.FullName}";
    }
}
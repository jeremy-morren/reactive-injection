using ReactiveInjection.Symbols;

namespace ReactiveInjection.ModelInjection;

internal class ModelInjectionTree
{
    public required IType ViewModel { get; init; }

    public required List<Model> Models { get; init; }
}

internal class Model
{
    
    public required IProperty VmProperty { get; init; }
    
    public required List<IProperty> Properties { get; init; }
    public string Name => VmProperty.Name;
    public IType Type => VmProperty.Type;

    public string CSharpName => VmProperty.IsNullable ? $"{Type.CSharpName}?" : Type.CSharpName;
}
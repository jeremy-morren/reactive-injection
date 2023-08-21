// ReSharper disable all
// ReSharper disable InvalidXmlDocComment

#nullable enable

using ReactiveInjection;

namespace Models.ModelItems;


public record Model1(int Int)
{
    public required bool InitRequired { get; init; }
}

public class Model2 {}

public class Model3
{
    /// <summary>
    /// This is a comment
    /// </summary>
    /// <remarks>These are remarks</remarks>
    public required string RequiredInit { get; init; }

    /// <summary>
    /// Invalid xml
    /// <summary>
    public string? GetOnly { get; }

    public required double Required { get; set; }
    
    public Model2? M3ClassProp { get; set; }
}

public record Model4(int? Constructed)
{
    public required string OtherProperty { get; set; }

    public Model2? M4ClassProp { get; set; }
}

public partial class InjectedViewModel1
{
    [BackingModel] public Model1? Model1 { get; }

    [BackingModel] public Model2 Model2 { get; set; } = null!;
    
    [BackingModel] public Model3 Model3 { get; private set; } = null!;
    
    [BackingModel] public Model4? Model4 { get; set; }

    public Model4 Other { get; } = null!;
}

public partial class InjectedViewModel2
{
    [BackingModel] public Model1? Model1 { get; private set; }

    [BackingModel] public Model3? Model3 { get; }

    public Model4 Other { get; } = null!;
    
    public partial class InjectedViewModel3
    {
        [BackingModel(nameof(Models.ModelItems.Model3.M3ClassProp))] 
        public Model3? Model3 { get; }
    }
}

using ReactiveInjection.Abstractions;
using ReactiveInjection.Tests.Setup.ViewModels;

namespace ReactiveInjection.Tests;

public static partial class ViewModelFactories
{
    [ReactiveFactory]
    public static ParentViewModel CreateParentViewModel(IServiceProvider services) =>
        throw new NotImplementedException();
}
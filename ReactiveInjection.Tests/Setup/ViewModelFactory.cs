using ReactiveInjection.Tests.Setup.Models;
using ReactiveInjection.Tests.Setup.ViewModels;

namespace ReactiveInjection.Tests.Setup;

public class ViewModelFactory
{
    [ReactiveFactory]
    public ParentViewModel NewParent() => throw new NotImplementedException();

    [ReactiveFactory]
    public ChildViewModel NewChild(ChildModel model) => throw new NotImplementedException();
}
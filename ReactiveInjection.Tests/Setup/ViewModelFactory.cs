using ReactiveInjection.Tests.Setup.Models;
using ReactiveInjection.Tests.Setup.ViewModels;

namespace ReactiveInjection.Tests.Setup;

public class ViewModelFactory
{
    public ParentViewModel NewParent() => throw new NotImplementedException();

    public ChildViewModel NewChild(ChildModel model) => throw new NotImplementedException();
}
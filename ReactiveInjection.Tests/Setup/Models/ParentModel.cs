namespace ReactiveInjection.Tests.Setup.Models;

public class ParentModel
{
    public ChildModel[] ChildModels { get; } = Enumerable.Range(0, 10).Select(i => new ChildModel(i)).ToArray();
}
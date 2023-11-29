using FluentAssertions;
using FluentAssertions.Collections;
using ReactiveInjection.SourceGenerators.Symbols;
using ReactiveInjection.Tests.Reflection;

namespace ReactiveInjection.Tests;

internal static class AssertionHelpers
{
    public static AndConstraint<GenericCollectionAssertions<IType>> Contain(this GenericCollectionAssertions<IType> assertions,
        IEnumerable<Type> expected,
        string because = "", params object[] becauseArgs)
    {
        return assertions.Contain(expected.Select(t => new ReflectedType(t)), because, becauseArgs);
    }
}
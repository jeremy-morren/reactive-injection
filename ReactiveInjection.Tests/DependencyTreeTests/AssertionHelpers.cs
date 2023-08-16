using FluentAssertions;
using FluentAssertions.Collections;
using ReactiveInjection.Tests.DependencyTreeTests.Reflection;
using ReactiveInjection.Tokens;

namespace ReactiveInjection.Tests.DependencyTreeTests;

internal static class AssertionHelpers
{
    public static AndConstraint<GenericCollectionAssertions<IType>> Contain(this GenericCollectionAssertions<IType> assertions,
        IEnumerable<Type> expected,
        string because = "", params object[] becauseArgs)
    {
        return assertions.Contain(expected.Select(t => new ReflectedType(t)), because, becauseArgs);
    }
}
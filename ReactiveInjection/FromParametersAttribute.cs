using JetBrains.Annotations;
using ReactiveInjection.Loader;

namespace ReactiveInjection;

/// <summary>
/// Marks that a parameter should be loaded from <c>parameters</c> array on <see cref="IReactiveViewModelLoader.Load"/>
/// </summary>
/// <remarks>
/// This is mainly used for custom objects that cannot be converted to/from a string
/// </remarks>
[PublicAPI, MeansImplicitUse(ImplicitUseTargetFlags.Itself)]
[AttributeUsage(AttributeTargets.Parameter)]
public class FromParametersAttribute : Attribute;
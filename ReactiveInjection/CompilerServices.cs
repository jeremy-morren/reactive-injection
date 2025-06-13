// ReSharper disable All

namespace System.Runtime.CompilerServices;

internal class IsExternalInit : Attribute {}

internal class CompilerFeatureRequiredAttribute : Attribute
{
    public CompilerFeatureRequiredAttribute(string name) { }
}

internal class RequiredMemberAttribute : Attribute {}
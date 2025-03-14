using System;

namespace BetterGlobals.Attributes;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
internal sealed class PatchDescriptionAttribute : Attribute
{
    public string Description { get; }
    public bool DefaultValue { get; }

    public PatchDescriptionAttribute(string description, bool defaultValue = true)
    {
        Description = description;
        DefaultValue = defaultValue;
    }
}
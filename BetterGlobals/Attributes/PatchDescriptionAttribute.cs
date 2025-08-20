using System;

namespace BetterGlobals.Attributes;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
internal sealed class PatchDescriptionAttribute(string description, bool defaultValue = true) : Attribute
{
    public string Description { get; } = description;
    public bool DefaultValue { get; } = defaultValue;
}
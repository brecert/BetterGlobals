using Elements.Core;
using HarmonyLib;
using ResoniteModLoader;
using System;
using System.Linq;
using System.Reflection;
using BetterGlobals.Attributes;

namespace BetterGlobals;

using System.Collections.Generic;


#if DEBUG
using ResoniteHotReloadLib;
#endif

public class ResoniteBetterGlobalsMod : ResoniteMod
{
    private static Assembly ModAssembly => typeof(ResoniteBetterGlobalsMod).Assembly;

    public override string Name => ModAssembly.GetCustomAttribute<AssemblyTitleAttribute>().Title;
    public override string Author => ModAssembly.GetCustomAttribute<AssemblyCompanyAttribute>().Company;
    public override string Version => ModAssembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion;
    public override string Link => ModAssembly.GetCustomAttributes<AssemblyMetadataAttribute>().First(meta => meta.Key == "RepositoryUrl").Value;

    internal static string HarmonyId => $"dev.bree.{ModAssembly.GetName()}";

    private static readonly Harmony harmony = new(HarmonyId);

    private static readonly Dictionary<string, ModConfigurationKey<bool>> patchCategoryKeys = [];

    static ResoniteBetterGlobalsMod()
    {
        DebugFunc(() => $"Static Initializing {nameof(ResoniteBetterGlobalsMod)}...");

        var keys = from t in AccessTools.GetTypesFromAssembly(ModAssembly)
                   select (t.GetCustomAttribute<HarmonyPatchCategory>(), t.GetCustomAttribute<TweakDescriptionAttribute>()) into t
                   where t.Item1 is not null && t.Item2 is not null
                   select new ModConfigurationKey<bool>(t.Item1.info.category, t.Item2.Description, computeDefault: () => t.Item2.DefaultValue);

        foreach (var key in keys)
        {
            DebugFunc(() => $"Registering patch category {key.Name}...");
            patchCategoryKeys[key.Name] = key;
        }
    }

    public override void DefineConfiguration(ModConfigurationDefinitionBuilder builder)
    {
        if (builder is null)
        {
            throw new ArgumentNullException(nameof(builder), "builder is null.");
        }

        foreach (var key in patchCategoryKeys.Values)
        {
            DebugFunc(() => $"Adding configuration key for {key.Name}...");
            builder.Key(key);
        }
    }


    public override void OnEngineInit()
    {
        OnHotReload(this);

#if DEBUG
        HotReloader.RegisterForHotReload(this);
#endif
    }


#if DEBUG
    static void BeforeHotReload()
    {
        foreach (var category in patchCategoryKeys.Keys)
        {
            UpdatePatch(category, false);
        }
    }

    static void OnHotReload(ResoniteMod modInstance)
    {
        foreach (var category in patchCategoryKeys.Keys)
        {
            UpdatePatch(category, true);
        }
    }
#endif

    private static void UpdatePatch(string category, bool enabled)
    {
        try
        {

            if (enabled)
            {
                DebugFunc(() => $"Patching {category}...");
                harmony.PatchCategory(category.ToString());
            }
            else
            {
                DebugFunc(() => $"Unpatching {category}...");
                harmony.UnpatchAll(HarmonyId);
            }
        }
        catch (Exception e)
        {
            Error(e);
        }
    }
}
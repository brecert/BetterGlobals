using System;
using FrooxEngine;
using FrooxEngine.ProtoFlux;
using FrooxEngine.UIX;

using BetterGlobals.Attributes;
using HarmonyLib;

namespace BetterGlobals.Patches;

[HarmonyPatchCategory("ProtoFluxGlobalRefProxy UI"), TweakDescription("Makes it easier to work with global references.")]
[HarmonyPatch(typeof(ProtoFluxGlobalRefProxy))]
internal static class ProtoFluxGlobalRefProxy_BuildUI_Patch
{
    // TODO: remove orphaned globals that are on the same slot as the root node when the global changes to avoid hanging global garbage

    [HarmonyPostfix]
    [HarmonyPatch("BuildUI")]
    internal static void BuildUI_Postifx(ProtoFluxGlobalRefProxy __instance, Text label, UIBuilder ui, ISyncRef globalRef)
    {
        var proxyVisual = __instance.GetSyncMember("_proxyVisual") as ISyncRef<Button>;
        label.Slot.AttachComponent<ReferenceReceiver>().TargetReference.Target = globalRef;
        proxyVisual.Target.Slot.AttachComponent<ReferenceReceiver>().TargetReference.Target = globalRef;
    }

    [HarmonyPatch("Clear")]
    [HarmonyPrefix]
    internal static void Clear_Prefix(ProtoFluxGlobalRefProxy __instance)
    {
        var globalRef = __instance.TargetGlobalRef;
        var globalProxy = globalRef.Target?.Target;
        if (globalRef is not null && globalProxy is IGlobalValueProxy)
        {
            globalRef.Target?.Clear();
        }
    }
}
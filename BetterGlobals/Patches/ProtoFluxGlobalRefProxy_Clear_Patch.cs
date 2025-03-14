using System;
using FrooxEngine;
using FrooxEngine.ProtoFlux;
using FrooxEngine.UIX;

using BetterGlobals.Attributes;
using HarmonyLib;

namespace BetterGlobals.Patches;

[HarmonyPatchCategory("ProtoFlux Global Clear Removes Reference"), PatchDescription("Configures whether or not the Global reference should be cleared rather than resetting the Global Value/Reference itself. Note this can make it easy to leave unused Globals arounds.")]
[HarmonyPatch(typeof(ProtoFluxGlobalRefProxy), "Clear")]
internal static class ProtoFluxGlobalRefProxy_Clear_Patch
{
    // TODO: remove orphaned globals that are on the same slot as the root node when the global changes to avoid hanging global garbage
    internal static void Postfix(ProtoFluxGlobalRefProxy __instance)
    {
        var globalRef = __instance.TargetGlobalRef;
        var globalProxy = globalRef.Target?.Target;
        if (globalRef is not null && globalProxy is IGlobalValueProxy)
        {
            globalRef.Target?.Clear();
        }
    }
}
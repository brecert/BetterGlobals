using System;
using System.Collections.Generic;
using Elements.Core;
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
    [HarmonyPrefix]
    [HarmonyPatch("BuildUI")]
    internal static void BuildUI_Prefix(Text label, UIBuilder ui, ISyncRef globalRef)
    {
        label.Slot.AttachComponent<ReferenceReceiver>().TargetReference.Target = globalRef;
    }

    [HarmonyPatch("Clear")]
    [HarmonyPrefix]
    internal static void Clear_Prefix(ProtoFluxGlobalRefProxy __instance)
    {
        var globalRef = __instance.TargetGlobalRef;
        var globalProxy = globalRef.Target?.Target;
        if (globalRef is not null && globalProxy is IGlobalValueProxy) {
            // globalRef;
            globalRef.Target?.Clear();
        }
    }
}
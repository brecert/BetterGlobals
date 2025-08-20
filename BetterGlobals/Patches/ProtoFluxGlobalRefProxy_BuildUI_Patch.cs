using System;
using FrooxEngine;
using FrooxEngine.ProtoFlux;
using FrooxEngine.UIX;

using BetterGlobals.Attributes;
using HarmonyLib;

namespace BetterGlobals.Patches;

[HarmonyPatchCategory("ProtoFlux Global Drop Support"), PatchDescription("Allows for Globals to be dropped into the ProtoFlux global references UI.")]
[HarmonyPatch(typeof(ProtoFluxGlobalRefProxy), "BuildUI")]
internal static class ProtoFluxGlobalRefProxy_BuildUI_Patch
{
    // TODO: remove orphaned globals that are on the same slot as the root node when the global changes to avoid hanging global garbage

    internal static void Postfix(ProtoFluxGlobalRefProxy __instance, Text label, UIBuilder ui, ISyncRef globalRef)
    {
        var proxyVisual = (ISyncRef<Button>)__instance.GetSyncMember("_proxyVisual");
        label.Slot.AttachComponent<ReferenceReceiver>().TargetReference.Target = globalRef;
        proxyVisual.Target.Slot.AttachComponent<ReferenceReceiver>().TargetReference.Target = globalRef;
    }
}
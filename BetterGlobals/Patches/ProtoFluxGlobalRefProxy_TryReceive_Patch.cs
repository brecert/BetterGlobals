using FrooxEngine;
using FrooxEngine.ProtoFlux;
using FrooxEngine.UIX;

using BetterGlobals.Attributes;
using HarmonyLib;
using System.Collections.Generic;
using Elements.Core;

namespace BetterGlobals.Patches;

[HarmonyPatchCategory("ProtoFlux ProtoFluxGlobalRefProxy Drop Support"), PatchDescription("Allows for ProtoFluxGlobalRefProxy to be dropped into the ProtoFlux global references UI.")]
[HarmonyPatch(typeof(ProtoFluxGlobalRefProxy), nameof(ProtoFluxGlobalRefProxy.TryReceive))]
internal static class ProtoFluxGlobalRefProxy_TryReceive_Patch
{
    // TODO: remove orphaned globals that are on the same slot as the root node when the global changes to avoid hanging global garbage

    internal static void Postfix(ProtoFluxGlobalRefProxy __instance, ref bool __result, IEnumerable<IGrabbable> items, Component grabber, Canvas.InteractionData eventData, in float3 globalPoint)
    {
        __result = false;
        foreach (var grabbable in items)
        {
            foreach (var refProxy in grabbable.Slot.GetComponentsInChildren<ReferenceProxy>())
            {
                if (refProxy.Reference.Target is ProtoFluxGlobalRefProxy globalRefProxy)
                {
                    __instance.TargetGlobalRef.Target.Target = globalRefProxy.TargetGlobalRef.Target.Target;
                    __result = true;
                }
            }
        }
    }
}

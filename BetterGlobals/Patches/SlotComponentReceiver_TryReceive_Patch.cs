using System;
using System.Collections.Generic;
using Elements.Core;
using FrooxEngine;
using FrooxEngine.ProtoFlux;
using FrooxEngine.UIX;

using BetterGlobals.Attributes;
using HarmonyLib;

namespace BetterGlobals.Patches;

[HarmonyPatchCategory("SlotComponentReceiver TryReceive"), TweakDescription("Makes it easier to work with global references.")]
[HarmonyPatch(typeof(SlotComponentReceiver), nameof(SlotComponentReceiver.TryReceive))]
internal static class SlotComponentReceiver_TryReceive_Patch
{
    static void Postfix(SlotComponentReceiver __instance, IEnumerable<IGrabbable> items, Component grabber, Canvas.InteractionData eventData, in float3 globalPoint)
    {
        foreach (var grabbable in items)
        {
            foreach (var refProxy in grabbable.Slot.GetComponentsInChildren<ReferenceProxy>())
            {
                if (refProxy.Reference.Target is ProtoFluxGlobalRefProxy globalRefProxy)
                {
                    if (globalRefProxy.TargetGlobalRef.Target.Target is Component component)
                    {
                        var menu = __instance.LocalUser.GetUserContextMenu();
                        var item = menu.AddItem("Move Global", (Uri)null, RadiantUI_Constants.Hero.CYAN);
                        item.Button.LocalPressed += (_, _) =>
                        {
                            __instance.Target.Target.MoveComponent(component);
                            __instance.LocalUser.CloseContextMenu(__instance);
                        };
                    }

                }
            }
        }
    }
}

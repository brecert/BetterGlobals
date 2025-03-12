using System;
using System.Collections.Generic;
using Elements.Core;
using FrooxEngine;
using FrooxEngine.ProtoFlux;
using FrooxEngine.UIX;

using BetterGlobals.Attributes;
using HarmonyLib;
using FrooxEngine.ProtoFlux.Runtimes.Execution;
using ProtoFlux.Runtimes.Execution;
using ProtoFlux.Runtimes.Execution.Nodes;

namespace BetterGlobals.Patches;

[HarmonyPatchCategory("ProtoFluxTool Generate Global to Output"), TweakDescription("Makes it easier to work with global references.")]
[HarmonyPatch(typeof(ProtoFluxTool))]
internal static class ProtoFluxTool_GenerateMenuItems_Patch
{
    [HarmonyPostfix]
    [HarmonyPatch(nameof(ProtoFluxTool.GenerateMenuItems))]
    internal static void GenerateMenuItems_Postfix(ProtoFluxTool __instance, InteractionHandler tool, ContextMenu menu)
    {
        var GetHit = Traverse.Create(__instance).Method("GetHit").GetValue<RaycastHit?>();
        var grabbedReference = __instance.GetGrabbedReference();

        if (grabbedReference is ProtoFluxGlobalRefProxy globalRefProxy)
        {
            var label = "Reference".AsLocaleKey();
            var drive = OfficialAssets.Graphics.Icons.ProtoFlux.Reference;
            var item = menu.AddItem(in label, drive, null);
            item.Button.LocalPressed += (button, data) =>
            {
                var globalValueProxy = globalRefProxy.EnsureProxy();
                var globalToOutputNode = GetGlobalToOutputNode(globalValueProxy.ValueType);
                var globalRef = globalRefProxy.TargetGlobalRef.Target;
                __instance.SpawnNode(globalToOutputNode, n =>
                {
                    n.GetGlobalRef(0).Target = globalValueProxy;
                    __instance.ActiveHandler.CloseContextMenu();
                });
            };
        }
    }

    static Type GetGlobalToOutputNode(Type inputType)
    {
        try
        {
            return ProtoFluxHelper.GetBindingForNode(typeof(GlobalToValueOutput<>)).MakeGenericType(inputType);
        }
        catch { }

        return ProtoFluxHelper.GetBindingForNode(typeof(GlobalToObjectOutput<>)).MakeGenericType(inputType);
    }

}
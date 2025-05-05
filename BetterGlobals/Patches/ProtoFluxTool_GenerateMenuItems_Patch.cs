using System;
using Elements.Core;
using FrooxEngine;
using FrooxEngine.ProtoFlux;

using BetterGlobals.Attributes;
using HarmonyLib;
using ProtoFlux.Runtimes.Execution.Nodes;
using ProtoFlux.Runtimes.Execution;

namespace BetterGlobals.Patches;

[HarmonyPatchCategory("GlobalToOutput Context Menu Item"), PatchDescription("Adds a context menu item to create a GlobalToOutput for the held Global when holding a ProtoFluxGlobalRefProxy with the ProtoFluxTool.")]
[HarmonyPatch(typeof(ProtoFluxTool), nameof(ProtoFluxTool.GenerateMenuItems))]
internal static class ProtoFluxTool_GenerateMenuItems_Patch
{
    static readonly Uri Icon_Color_Output = new("resdb:///e0a4e5f5dd6c0fc7e2b089b873455f908a8ede7de4fd37a3430ef71917a543ec.png");


    internal static void Postfix(ProtoFluxTool __instance, InteractionHandler tool, ContextMenu menu)
    {
        // var GetHit = Traverse.Create(__instance).Method("GetHit").GetValue<RaycastHit?>();
        var grabbedReference = __instance.GetGrabbedReference();

        if (grabbedReference is ProtoFluxGlobalRefProxy globalRefProxy)
        {
            var label = "Output".AsLocaleKey();
            var item = menu.AddItem(in label, Icon_Color_Output, RadiantUI_Constants.Hero.ORANGE);
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

        if (grabbedReference is IGlobalValueProxy globalValue)
        {
            var label = "Output".AsLocaleKey();
            var item = menu.AddItem(in label, Icon_Color_Output, RadiantUI_Constants.Hero.ORANGE);
            item.Button.LocalPressed += (button, data) =>
            {
                var globalToOutputNode = GetGlobalToOutputNode(globalValue.ValueType);
                __instance.SpawnNode(globalToOutputNode, n =>
                {
                    n.GetGlobalRef(0).Target = globalValue;
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
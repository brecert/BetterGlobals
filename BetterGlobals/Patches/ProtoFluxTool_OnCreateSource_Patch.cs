using FrooxEngine;
using FrooxEngine.ProtoFlux;
using BetterGlobals.Attributes;
using HarmonyLib;
using Elements.Core;

namespace BetterGlobals.Patches;

[HarmonyPatchCategory("Sourceable GlobalReference<IValue> References"), PatchDescription("Allows GlobalReference<IValue> components to be sourced by using the global as the source.")]
[HarmonyPatch(typeof(ProtoFluxTool), "OnCreateSource")]
internal static class ProtoFluxTool_OnCreateSource_Patch
{
    internal static bool Prefix(ProtoFluxTool __instance, IButton button, ButtonEventData eventData, IWorldElement target)
    {
        UniLog.Log(target.GetType());
        if (target is IGlobalValueProxy globalValueProxy)
        {
            var valueType = globalValueProxy.ValueType;
            if (valueType.IsGenericType && typeof(IValue<>).IsAssignableFrom(valueType.GetGenericTypeDefinition()))
            {
                var sourceNode = ProtoFluxHelper.GetSourceNode(valueType.GenericTypeArguments[0]);
                __instance.SpawnNode(sourceNode, (n) =>
                {
                    n.GetGlobalRef(0).TrySet(target);
                });
                return false;
            }
        }
        return true;
    }
}
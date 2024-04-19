using System;
using HarmonyLib;

namespace ControlCompanyAddons.Patches.AI;

[HarmonyPatch(typeof(RadMechAI))]
public static class RadMechAIPatch {
    [HarmonyPatch(nameof(RadMechAI.SetExplosion))]
    [HarmonyFinalizer]
    // ReSharper disable once InconsistentNaming
    private static Exception SetExplosionFinalizer(Exception __exception) {
        // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
        if (__exception is null)
            return null!;

        return __exception.GetType() == typeof(NullReferenceException)? null! : __exception;
    }

    [HarmonyPatch(nameof(RadMechAI.StartExplosion))]
    [HarmonyPrefix]
    // ReSharper disable once InconsistentNaming
    private static bool StartExplosionPrefix(RadMechAI __instance) {
        try {
            return __instance.enabled;
        } catch (NullReferenceException) {
            return false;
        }
    }
}
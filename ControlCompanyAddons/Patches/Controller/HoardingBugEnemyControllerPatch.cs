using ControlCompany.Core;
using ControlCompany.Core.Enemy;
using ControlCompanyAddons.Additions;
using ControlCompanyAddons.Helpers;
using HarmonyLib;

namespace ControlCompanyAddons.Patches.Controller;

[HarmonyPatch(typeof(HoardingBugEnemyController))]
public class HoardingBugEnemyControllerPatch {
    // ReSharper disable InconsistentNaming

    [HarmonyPatch(nameof(HoardingBugEnemyController.GetPrimarySkillName))]
    [HarmonyPrefix]
    public static bool SetPrimarySkillName(HoardingBugEnemyController __instance, ref string __result) {
        var hoardingBugAI = __instance.hoarderBugAI;

        if (hoardingBugAI == null) return true;

        if (hoardingBugAI.heldItem != null) return true;

        var dataHelper = hoardingBugAI.GetComponent<DataHelper>() ?? hoardingBugAI.gameObject.AddComponent<DataHelper>();

        if (!dataHelper.HasData("IsAttacking")) dataHelper.SetData("IsAttacking", false);

        var isAttacking = (bool) dataHelper.GetData("IsAttacking");

        __result = isAttacking? "Stop Attacking" : "Start Attacking";
        return false;
    }

    [HarmonyPatch(nameof(HoardingBugEnemyController.UsePrimarySkillAction))]
    [HarmonyPrefix]
    public static bool UsePrimarySkillAction(HoardingBugEnemyController __instance) {
        if (__instance.isAIControlled) return true;

        var hoardingBugAI = __instance.hoarderBugAI;

        if (hoardingBugAI == null) return true;

        if (hoardingBugAI.heldItem != null) return true;

        var dataHelper = hoardingBugAI.GetComponent<DataHelper>() ?? hoardingBugAI.gameObject.AddComponent<DataHelper>();

        if (!dataHelper.HasData("IsAttacking")) dataHelper.SetData("IsAttacking", false);

        var isAttacking = (bool) dataHelper.GetData("IsAttacking");

        isAttacking = !isAttacking;

        HoardingBugAdditions.HandleAttackLogic(isAttacking);

        dataHelper.SetData("IsAttacking", isAttacking);
        return false;
    }
}

[HarmonyPatch(typeof(EnemyController))]
public class HoardingBugControllerDestroyPatch {
    // ReSharper disable InconsistentNaming

    [HarmonyPatch(nameof(EnemyController.DestroyAndCleanUp))]
    [HarmonyPrefix]
    public static void DestroyAndCleanUp(EnemyController __instance) {
        if (__instance is not HoardingBugEnemyController) return;

        HoardingBugAdditions.HandleAttackLogic(false);
    }
}
using System.Reflection;
using ControlCompanyAddons.Additions;
using ControlCompanyAddons.Helpers;
using HarmonyLib;

namespace ControlCompanyAddons.Patches.Controller;

[HarmonyPatch]
public class HoardingBugGetPrimarySkillNamePatch {
    public static MethodBase TargetMethod() {
        var enemyControllerType = HoardingBugEnemyControllerHelper.GetHoardingBugEnemyControllerType();

        return AccessTools.FirstMethod(enemyControllerType, method => method.Name.Contains("GetPrimarySkillName"));
    }

    public static bool Prefix(object __instance, ref string __result) {
        var hoardingBugAI = HoardingBugEnemyControllerHelper.GetHoardingBugAI(__instance);

        if (hoardingBugAI is null)
            return true;

        if (hoardingBugAI.heldItem is not null)
            return true;

        var dataHelper = hoardingBugAI.GetComponent<DataHelper>() ??
                         hoardingBugAI.gameObject.AddComponent<DataHelper>();

        if (!dataHelper.HasData("IsAttacking"))
            dataHelper.SetData("IsAttacking", false);

        var isAttacking = (bool)dataHelper.GetData("IsAttacking");

        __result = isAttacking ? "Stop Attacking" : "Start Attacking";
        return false;
    }
}

[HarmonyPatch]
public class HoardingBugUsePrimarySkillActionPatch {
    public static MethodBase TargetMethod() {
        var enemyControllerType = HoardingBugEnemyControllerHelper.GetHoardingBugEnemyControllerType();

        return AccessTools.FirstMethod(enemyControllerType, method => method.Name.Contains("UsePrimarySkillAction"));
    }

    public static bool Prefix(object __instance) {
        if (EnemyControllerHelper.IsAIControlled(__instance))
            return true;

        var hoardingBugAI = HoardingBugEnemyControllerHelper.GetHoardingBugAI(__instance);

        if (hoardingBugAI is null)
            return true;

        if (hoardingBugAI.heldItem != null)
            return true;

        var dataHelper = hoardingBugAI.GetComponent<DataHelper>() ??
                         hoardingBugAI.gameObject.AddComponent<DataHelper>();

        if (!dataHelper.HasData("IsAttacking"))
            dataHelper.SetData("IsAttacking", false);

        var isAttacking = (bool)dataHelper.GetData("IsAttacking");

        isAttacking = !isAttacking;

        HoardingBugAdditions.HandleAttackLogic(isAttacking);

        dataHelper.SetData("IsAttacking", isAttacking);
        return false;
    }
}

[HarmonyPatch]
public class HoardingBugControllerDestroyPatch {
    public static MethodBase TargetMethod() {
        //The destroy method we're looking for is actually not being overriden, so we need to search in the EnemyController class
        var enemyControllerType = EnemyControllerHelper.GetEnemyControllerType();

        return AccessTools.FirstMethod(enemyControllerType, method => method.Name.Contains("DestroyAndCleanUp"));
    }

    public static void Prefix(object __instance) {
        if (!HoardingBugEnemyControllerHelper.IsHoardingBugController(__instance))
            return;

        HoardingBugAdditions.HandleAttackLogic(false);
    }
}
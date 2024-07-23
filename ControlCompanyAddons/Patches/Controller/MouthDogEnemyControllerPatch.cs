using ControlCompany.Core;
using ControlCompany.Core.Enemy;
using ControlCompanyAddons.Additions;
using ControlCompanyAddons.Helpers;
using HarmonyLib;

namespace ControlCompanyAddons.Patches.Controller;

[HarmonyPatch(typeof(EnemyController))]
public class MouthDogPrimarySkillPatch {
    // ReSharper disable InconsistentNaming

    [HarmonyPatch(nameof(EnemyController.GetPrimarySkillName))]
    [HarmonyPrefix]
    public static bool SetPrimarySkillName(EnemyController __instance, ref string __result) {
        if (__instance is not MouthDogEnemyController mouthDogEnemyController) return true;

        var mouthDogAI = mouthDogEnemyController.mouthDogAI;

        if (mouthDogAI == null) return true;

        var dataHelper = mouthDogAI.GetComponent<DataHelper>() ?? mouthDogAI.gameObject.AddComponent<DataHelper>();

        if (!dataHelper.HasData("WasAngered")) dataHelper.SetData("WasAngered", false);

        var wasAngered = (bool) dataHelper.GetData("WasAngered");

        __result = wasAngered? "Calm Down" : "Investigate";
        return false;
    }

    [HarmonyPatch(nameof(EnemyController.UsePrimarySkillAction))]
    [HarmonyPrefix]
    public static bool UsePrimarySkillAction(EnemyController __instance) {
        if (__instance is not MouthDogEnemyController mouthDogEnemyController) return true;

        if (mouthDogEnemyController.isAIControlled) return true;

        var mouthDogAI = mouthDogEnemyController.mouthDogAI;

        if (mouthDogAI == null) return true;

        var dataHelper = mouthDogAI.GetComponent<DataHelper>() ?? mouthDogAI.gameObject.AddComponent<DataHelper>();

        if (!dataHelper.HasData("HasScreamed")) dataHelper.SetData("HasScreamed", false);

        if (!dataHelper.HasData("WasAngered")) dataHelper.SetData("WasAngered", false);

        var wasAngered = (bool) dataHelper.GetData("WasAngered");

        const bool hasScreamed = false;

        wasAngered = !wasAngered;

        MouthDogAdditions.HandleAngerState(mouthDogAI, wasAngered);

        dataHelper.SetData("HasScreamed", hasScreamed);
        dataHelper.SetData("WasAngered", wasAngered);
        return false;
    }
}

[HarmonyPatch(typeof(MouthDogEnemyController))]
public class MouthDogSecondarySkillPatch {
    // ReSharper disable InconsistentNaming

    [HarmonyPatch(nameof(MouthDogEnemyController.GetSecondarySkillName))]
    [HarmonyPrefix]
    public static bool SetSecondarySkillName(MouthDogEnemyController __instance, ref string __result) {
        var mouthDogAI = __instance.mouthDogAI;

        if (mouthDogAI == null) return true;

        var dataHelper = mouthDogAI.GetComponent<DataHelper>() ?? mouthDogAI.gameObject.AddComponent<DataHelper>();

        if (!dataHelper.HasData("HasScreamed")) dataHelper.SetData("HasScreamed", false);

        if (!dataHelper.HasData("WasAngered")) dataHelper.SetData("WasAngered", false);

        var hasScreamed = (bool) dataHelper.GetData("HasScreamed");

        var wasAngered = (bool) dataHelper.GetData("WasAngered");

        __result = hasScreamed? "Lunge" : "Scream";
        return false;
    }

    [HarmonyPatch(nameof(MouthDogEnemyController.UseSecondarySkillAction))]
    [HarmonyPrefix]
    public static bool UseSecondarySkillAction(MouthDogEnemyController __instance) {
        if (__instance.isAIControlled) return true;

        var mouthDogAI = __instance.mouthDogAI;

        if (mouthDogAI == null) return true;

        var dataHelper = mouthDogAI.GetComponent<DataHelper>() ?? mouthDogAI.gameObject.AddComponent<DataHelper>();

        if (!dataHelper.HasData("HasScreamed")) dataHelper.SetData("HasScreamed", false);

        if (!dataHelper.HasData("WasAngered")) dataHelper.SetData("WasAngered", false);

        var hasScreamed = (bool) dataHelper.GetData("HasScreamed");

        var wasAngered = (bool) dataHelper.GetData("WasAngered");

        hasScreamed = !hasScreamed;

        if (!hasScreamed) wasAngered = true;

        MouthDogAdditions.HandleScreamOrLunge(mouthDogAI, hasScreamed);

        dataHelper.SetData("HasScreamed", hasScreamed);
        dataHelper.SetData("WasAngered", wasAngered);
        return false;
    }
}
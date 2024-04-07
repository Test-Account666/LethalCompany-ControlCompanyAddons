using System.Reflection;
using ControlCompanyAddons.Additions;
using ControlCompanyAddons.Helpers;
using HarmonyLib;

namespace ControlCompanyAddons.Patches.Controller;

// Primary Skill
[HarmonyPatch]
public class MouthDogGetPrimarySkillNamePatch {
    // ReSharper disable InconsistentNaming
    public static MethodBase TargetMethod() {
        var enemyControllerType = EnemyControllerHelper.GetEnemyControllerType();

        return AccessTools.FirstMethod(enemyControllerType, method => method.Name.Contains("GetPrimarySkillName"));
    }

    public static bool Prefix(object __instance, ref string __result) {
        if (!MouthDogEnemyControllerHelper.IsMouthDogController(__instance))
            return true;

        var mouthDogAI = MouthDogEnemyControllerHelper.GetMouthDogAI(__instance);

        if (mouthDogAI is null)
            return true;

        var dataHelper = mouthDogAI.GetComponent<DataHelper>() ??
                         mouthDogAI.gameObject.AddComponent<DataHelper>();

        if (!dataHelper.HasData("WasAngered"))
            dataHelper.SetData("WasAngered", false);

        var wasAngered = (bool) dataHelper.GetData("WasAngered");

        __result = wasAngered? "Calm Down" : "Investigate";
        return false;
    }
}

[HarmonyPatch]
public class MouthDogUsePrimarySkillActionPatch {
    public static MethodBase TargetMethod() {
        var enemyControllerType = EnemyControllerHelper.GetEnemyControllerType();

        return AccessTools.FirstMethod(enemyControllerType, method => method.Name.Contains("UsePrimarySkillAction"));
    }

    public static bool Prefix(object __instance) {
        if (!MouthDogEnemyControllerHelper.IsMouthDogController(__instance))
            return true;

        if (EnemyControllerHelper.IsAIControlled(__instance))
            return true;

        var mouthDogAI = MouthDogEnemyControllerHelper.GetMouthDogAI(__instance);

        if (mouthDogAI is null)
            return true;

        var dataHelper = mouthDogAI.GetComponent<DataHelper>() ??
                         mouthDogAI.gameObject.AddComponent<DataHelper>();

        if (!dataHelper.HasData("HasScreamed"))
            dataHelper.SetData("HasScreamed", false);

        if (!dataHelper.HasData("WasAngered"))
            dataHelper.SetData("WasAngered", false);

        var wasAngered = (bool) dataHelper.GetData("WasAngered");

        const bool hasScreamed = false;

        wasAngered = !wasAngered;

        MouthDogAdditions.HandleAngerState(mouthDogAI, wasAngered);

        dataHelper.SetData("HasScreamed", hasScreamed);
        dataHelper.SetData("WasAngered", wasAngered);
        return false;
    }
}

// Secondary Skill

[HarmonyPatch]
public class MouthDogGetSecondarySkillNamePatch {
    // ReSharper disable InconsistentNaming
    public static MethodBase TargetMethod() {
        var enemyControllerType = MouthDogEnemyControllerHelper.GetMouthDogEnemyControllerType();

        return AccessTools.FirstMethod(enemyControllerType, method => method.Name.Contains("GetSecondarySkillName"));
    }

    public static bool Prefix(object __instance, ref string __result) {
        var mouthDogAI = MouthDogEnemyControllerHelper.GetMouthDogAI(__instance);

        if (mouthDogAI is null)
            return true;

        var dataHelper = mouthDogAI.GetComponent<DataHelper>() ?? mouthDogAI.gameObject.AddComponent<DataHelper>();

        if (!dataHelper.HasData("HasScreamed"))
            dataHelper.SetData("HasScreamed", false);

        if (!dataHelper.HasData("WasAngered"))
            dataHelper.SetData("WasAngered", false);

        var hasScreamed = (bool) dataHelper.GetData("HasScreamed");

        var wasAngered = (bool) dataHelper.GetData("WasAngered");

        __result = hasScreamed? "Lunge" : "Scream";
        return false;
    }
}

[HarmonyPatch]
public class MouthDogUseSecondarySkillActionPatch {
    public static MethodBase TargetMethod() {
        var enemyControllerType = MouthDogEnemyControllerHelper.GetMouthDogEnemyControllerType();

        return AccessTools.FirstMethod(enemyControllerType, method => method.Name.Contains("UseSecondarySkillAction"));
    }

    public static bool Prefix(object __instance) {
        if (EnemyControllerHelper.IsAIControlled(__instance))
            return true;

        var mouthDogAI = MouthDogEnemyControllerHelper.GetMouthDogAI(__instance);

        if (mouthDogAI is null)
            return true;

        var dataHelper = mouthDogAI.GetComponent<DataHelper>() ?? mouthDogAI.gameObject.AddComponent<DataHelper>();

        if (!dataHelper.HasData("HasScreamed"))
            dataHelper.SetData("HasScreamed", false);

        if (!dataHelper.HasData("WasAngered"))
            dataHelper.SetData("WasAngered", false);

        var hasScreamed = (bool) dataHelper.GetData("HasScreamed");

        var wasAngered = (bool) dataHelper.GetData("WasAngered");

        hasScreamed = !hasScreamed;

        if (!hasScreamed)
            wasAngered = true;

        MouthDogAdditions.HandleScreamOrLunge(mouthDogAI, hasScreamed);

        dataHelper.SetData("HasScreamed", hasScreamed);
        dataHelper.SetData("WasAngered", wasAngered);
        return false;
    }
}
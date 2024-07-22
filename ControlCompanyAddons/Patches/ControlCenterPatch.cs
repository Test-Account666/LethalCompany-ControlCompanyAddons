using System.Diagnostics;
using System.Reflection;
using ControlCompanyAddons.Helpers;
using HarmonyLib;

namespace ControlCompanyAddons.Patches;

[HarmonyPatch]
public class ControlCenterPatch {
    public static MethodBase TargetMethod() {
        var controlCenterType = ControlCenterHelper.GetControlCenterType();

        Debug.Assert(controlCenterType != null, nameof(controlCenterType) + " != null");
        return AccessTools.FirstMethod(controlCenterType, method => method.Name.Contains("SpawnControllableEnemy"));
    }

    // ReSharper disable once InconsistentNaming
    public static void Postfix(object __result) {
        var enemyGameObject = EnemyControllerHelper.GetEnemyGameObject(__result);

        // ReSharper disable once UseNullPropagation
        if (enemyGameObject is null) return;

        enemyGameObject.AddComponent<DataHelper>();
    }
}
using ControlCompany.Core;
using ControlCompanyAddons.Helpers;
using HarmonyLib;

namespace ControlCompanyAddons.Patches;

[HarmonyPatch(typeof(ControlCenter))]
public class ControlCenterPatch {
    [HarmonyPatch(nameof(ControlCenter.SpawnControllableEnemy))]
    [HarmonyPostfix]
    // ReSharper disable once InconsistentNaming
    public static void AddDataHelper(EnemyController __result) {
        var enemyGameObject = __result.enemyGameObject;

        if (enemyGameObject == null) return;

        enemyGameObject.AddComponent<DataHelper>();
    }
}
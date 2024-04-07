using ControlCompanyAddons.Helpers;
using HarmonyLib;
using UnityEngine;

namespace ControlCompanyAddons.Patches.AI;

[HarmonyPatch(typeof(EnemyAI))]
public static class HoarderBugAIPatch {
    // ReSharper disable InconsistentNaming
    [HarmonyPatch("SwitchToBehaviourServerRpc")]
    [HarmonyPrefix]
    public static void SwitchToBehaviourServerRpcPrefix(EnemyAI __instance, ref int stateIndex) =>
        SwitchToBehaviourStateHandler(__instance, ref stateIndex);

    [HarmonyPatch("SwitchToBehaviourState")]
    [HarmonyPrefix]
    public static void SwitchToBehaviourStatePrefix(EnemyAI __instance, ref int stateIndex) =>
        SwitchToBehaviourStateHandler(__instance, ref stateIndex);

    [HarmonyPatch("SwitchToBehaviourStateOnLocalClient")]
    [HarmonyPrefix]
    public static void SwitchToBehaviourStateOnLocalClientPrefix(EnemyAI __instance, ref int stateIndex) =>
        SwitchToBehaviourStateHandler(__instance, ref stateIndex);

    private static void SwitchToBehaviourStateHandler(Component enemyAI, ref int stateIndex) {
        if (enemyAI is not HoarderBugAI)
            return;

        if (!ControlCenterHelper.IsEnemyMode())
            return;

        var currentControlledEnemy = ControlCenterHelper.GetCurrentControlledEnemy();

        if (currentControlledEnemy is null)
            return;

        if (EnemyControllerHelper.IsAIControlled(currentControlledEnemy))
            return;

        var dataHelper = enemyAI.GetComponent<DataHelper>();

        if (dataHelper is null)
            return;

        if (!dataHelper.HasData("IsAttacking"))
            return;

        var isAttacking = (bool) dataHelper.GetData("IsAttacking");

        if (isAttacking)
            return;

        if (stateIndex != 2)
            return;

        stateIndex = 0;
    }
}
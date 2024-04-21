using ControlCompanyAddons.Helpers;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using Debug = System.Diagnostics.Debug;

namespace ControlCompanyAddons.Input;

internal static class InputHandler {
    internal static void Initialize() =>
        ControlCompanyAddons.inputKeys.ReleaseControlKey.performed += OnReleaseKey;

    private static void OnReleaseKey(InputAction.CallbackContext callbackContext) {
        if (!callbackContext.performed)
            return;

        if (ControlCenterHelper.GetCurrentControlMode() != ControlMode.ENEMY)
            return;

        var enemyController = ControlCenterHelper.GetCurrentControlledEnemy();

        Debug.Assert(enemyController != null, nameof(enemyController) + " != null");

        var currentControlledEnemy = EnemyControllerHelper.GetEnemyGameObject(enemyController);

        Debug.Assert(currentControlledEnemy != null, nameof(currentControlledEnemy) + " != null");

        var position = currentControlledEnemy.transform.position;

        ControlCenterHelper.EnableGhost(true, position);
        GhostControllerHelper.EnableLight(ControlCenterHelper.IsGhostIndoors());

        EnemyControllerHelper.EnableAIControl(enemyController, true);

        var enemyControllerBehaviour = enemyController as MonoBehaviour;

        if (enemyControllerBehaviour != null)
            Object.Destroy(enemyControllerBehaviour.gameObject);

        ControlCenterHelper.SetCurrentControlMode(ControlMode.GHOST);
    }
}
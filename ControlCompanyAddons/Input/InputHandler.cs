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

        var rotation = currentControlledEnemy.transform.rotation;

        var copyObject = Object.Instantiate(currentControlledEnemy, position, rotation);

        copyObject.GetComponent<NetworkObject>().Spawn(true);

        ControlCenterHelper.EnableGhost(true, position);
        GhostControllerHelper.EnableLight(ControlCenterHelper.IsGhostIndoors());
        EnemyControllerHelper.DestroyAndCleanUp(enemyController);
        ControlCenterHelper.SetCurrentControlMode(ControlMode.GHOST);
    }
}
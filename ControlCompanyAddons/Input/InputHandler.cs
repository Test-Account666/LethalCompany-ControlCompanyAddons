using ControlCompanyAddons.Helpers;
using UnityEngine;
using UnityEngine.InputSystem;
using Debug = System.Diagnostics.Debug;

namespace ControlCompanyAddons.Input;

internal static class InputHandler {
    internal static bool espEnabled;

    internal static void Initialize() {
        ControlCompanyAddons.inputKeys.ReleaseControlKey.performed += OnEnemyReleaseKey;

        ControlCompanyAddons.inputKeys.ToggleEspKey.performed += OnToggleEspKey;
    }

    private static void OnToggleEspKey(InputAction.CallbackContext callbackContext) {
        if (!callbackContext.performed) return;

        if (StartOfRound.Instance == null) return;

        if (!StartOfRound.Instance.IsHost) return;

        espEnabled = !espEnabled;

        foreach (var allPlayerScript in StartOfRound.Instance.allPlayerScripts) {
            if (allPlayerScript == null) continue;

            if (espEnabled) PlayerEsp.ApplyEsp(allPlayerScript);
            else PlayerEsp.UnApplyEsp(allPlayerScript);
        }
    }

    private static void OnEnemyReleaseKey(InputAction.CallbackContext callbackContext) {
        if (!callbackContext.performed) return;

        if (ControlCenterHelper.GetCurrentControlMode() != ControlMode.ENEMY) return;

        var enemyController = ControlCenterHelper.GetCurrentControlledEnemy();

        Debug.Assert(enemyController != null, nameof(enemyController) + " != null");

        var currentControlledEnemy = EnemyControllerHelper.GetEnemyGameObject(enemyController);

        Debug.Assert(currentControlledEnemy != null, nameof(currentControlledEnemy) + " != null");

        var position = currentControlledEnemy.transform.position;

        ControlCenterHelper.EnableGhost(true, position);
        GhostControllerHelper.EnableLight(ControlCenterHelper.IsGhostIndoors());

        EnemyControllerHelper.EnableAIControl(enemyController, true);

        var enemyControllerBehaviour = enemyController as MonoBehaviour;

        if (enemyControllerBehaviour != null) Object.Destroy(enemyControllerBehaviour.gameObject);

        ControlCenterHelper.SetCurrentControlMode(ControlMode.GHOST);
    }
}
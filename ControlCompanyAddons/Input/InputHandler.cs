using ControlCompany.Core;
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

        var controlCenter = ControlCenter.Instance;

        if (controlCenter.currentMode != ControlCenter.Mode.ENEMY) return;

        var enemyController = controlCenter.currentControlledEnemy;

        Debug.Assert(enemyController != null, nameof(enemyController) + " != null");

        var currentControlledEnemy = enemyController.enemyGameObject;

        Debug.Assert(currentControlledEnemy != null, nameof(currentControlledEnemy) + " != null");

        var position = currentControlledEnemy.transform.position;

        controlCenter.EnableGhost(true, position);
        controlCenter.ghostController.EnableLight(controlCenter.isGhostIndoors);

        enemyController.EnableAIControl(true);

        var enemyControllerBehaviour = enemyController as MonoBehaviour;

        if (enemyControllerBehaviour != null) Object.Destroy(enemyControllerBehaviour.gameObject);

        controlCenter.currentMode = ControlCenter.Mode.GHOST;
    }
}
using System;
using System.Reflection;
using HarmonyLib;

namespace ControlCompanyAddons.Helpers;

public static class GhostControllerHelper {
    private static Type? _ghostControllerType;
    private static MethodInfo? _enableLightMethod;

    internal static bool FetchGhostControllerType() {
        if (_ghostControllerType is not null)
            return true;

        var controlCompanyAssembly = ControlCompanyHelper.GetControlCompanyAssembly();

        if (controlCompanyAssembly is null)
            return false;

        var tries = 0;

        while (tries < 3) {
            foreach (var type in AccessTools.GetTypesFromAssembly(controlCompanyAssembly)) {
                if (type?.Namespace?.Contains("ControlCompany.Core") is not true)
                    continue;

                if (type?.Name?.Contains("CustomPlayerController") is not true)
                    continue;

                _ghostControllerType = type;
                return true;
            }

            tries += 1;
        }

        ControlCompanyAddons.Logger.LogError("Couldn't find ControlCompany 'CustomPlayerController' type!");
        return false;
    }

    public static Type? GetGhostControllerType() =>
        !FetchGhostControllerType()? null : _ghostControllerType;

    internal static bool FetchEnableGhostMethod() {
        var ghostControllerType = GetGhostControllerType();

        if (ghostControllerType is null)
            return false;

        _enableLightMethod = AccessTools.DeclaredMethod(ghostControllerType, "EnableLight", [
            typeof(bool),
        ]);

        if (_enableLightMethod is not null)
            return true;

        ControlCompanyAddons.Logger.LogError("Couldn't find ControlCompany 'CustomPlayerController' EnableLight method!");
        return false;
    }

    public static void EnableLight(bool enable) {
        var ghostControllerInstance = ControlCenterHelper.GetGhostController();

        if (ghostControllerInstance is null)
            return;

        if (!FetchEnableGhostMethod())
            return;

        _enableLightMethod?.Invoke(ghostControllerInstance, [
            enable,
        ]);
    }
}
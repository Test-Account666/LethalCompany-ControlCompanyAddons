using BepInEx;
using BepInEx.Logging;
using ControlCompanyAddons.Input;
using HarmonyLib;

namespace ControlCompanyAddons;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
[BepInDependency("ControlCompany.ControlCompany")]
[BepInDependency("com.rune580.LethalCompanyInputUtils")]
public class ControlCompanyAddons : BaseUnityPlugin {
    internal static InputKeys inputKeys = null!;
    public static ControlCompanyAddons Instance { get; private set; } = null!;
    internal new static ManualLogSource Logger { get; private set; } = null!;
    internal static Harmony? Harmony { get; set; }

    private void Awake() {
        Logger = base.Logger;
        Instance = this;

        inputKeys = new();

        InputHandler.Initialize();

        Patch();

        Logger.LogInfo($"{MyPluginInfo.PLUGIN_GUID} v{MyPluginInfo.PLUGIN_VERSION} has loaded!");
    }

    internal static void Patch() {
        Harmony ??= new(MyPluginInfo.PLUGIN_GUID);

        Logger.LogDebug("Patching...");

        Harmony.PatchAll();

        Logger.LogDebug("Finished patching!");
    }

    internal static void Unpatch() {
        Logger.LogDebug("Unpatching...");

        Harmony?.UnpatchSelf();

        Logger.LogDebug("Finished unpatching!");
    }
}
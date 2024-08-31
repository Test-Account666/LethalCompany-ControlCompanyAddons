using System.IO;
using System.Reflection;
using GameNetcodeStuff;
using HarmonyLib;
using UnityEngine;
using Debug = System.Diagnostics.Debug;

namespace ControlCompanyAddons;

[HarmonyPatch(typeof(PlayerControllerB))]
public static class PlayerEsp {
    private static AssetBundle _assetBundle = null!;
    private static GameObject _playerEspPrefab = null!;

    internal static void LoadMaterial() {
        var assemblyLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        Debug.Assert(assemblyLocation != null, nameof(assemblyLocation) + " != null");
        _assetBundle = AssetBundle.LoadFromFile(Path.Combine(assemblyLocation, "controlcompanyaddons"));

        _playerEspPrefab = _assetBundle.LoadAsset<GameObject>("Assets/LethalCompany/Mods/ControlCompanyAddons/PlayerESP.prefab");
    }

    internal static void UnApplyEsp(PlayerControllerB playerControllerB) {
        var scavengerModel = playerControllerB.transform.Find("ScavengerModel");

        if (scavengerModel == null) {
            ControlCompanyAddons.Logger.LogFatal("Couldn't find scavenger model!");
            return;
        }

        var playerEsp = scavengerModel.Find("PlayerESP(Clone)");

        if (playerEsp == null) return;

        Object.Destroy(playerEsp.gameObject);
    }

    internal static void ApplyEsp(PlayerControllerB playerControllerB) {
        var scavengerModel = playerControllerB.transform.Find("ScavengerModel");

        if (scavengerModel == null) {
            ControlCompanyAddons.Logger.LogFatal("Couldn't find scavenger model!");
            return;
        }

        UnApplyEsp(playerControllerB);

        Object.Instantiate(_playerEspPrefab, scavengerModel, false);

        //y = 1.5
        //z = -0.197
    }
}
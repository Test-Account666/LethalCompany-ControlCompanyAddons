using System.IO;
using System.Linq;
using System.Reflection;
using GameNetcodeStuff;
using HarmonyLib;
using UnityEngine;
using Debug = System.Diagnostics.Debug;

namespace ControlCompanyAddons;

[HarmonyPatch(typeof(PlayerControllerB))]
public static class PlayerEsp {
    private static AssetBundle _assetBundle = null!;
    private static Material _material = null!;

    internal static void LoadMaterial() {
        var assemblyLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        Debug.Assert(assemblyLocation != null, nameof(assemblyLocation) + " != null");
        _assetBundle = AssetBundle.LoadFromFile(Path.Combine(assemblyLocation, "controlcompanyaddons"));

        _material = _assetBundle.LoadAllAssets<Material>()[0];
    }

    internal static void UnApplyEsp(PlayerControllerB playerControllerB) {
        var scavengerModel = playerControllerB.transform.Find("ScavengerModel");

        if (scavengerModel == null) {
            ControlCompanyAddons.Logger.LogFatal("Couldn't find scavenger model!");
            return;
        }

        var skinnedMeshRenderers = scavengerModel.GetComponentsInChildren<SkinnedMeshRenderer>();

        skinnedMeshRenderers ??= [
        ];

        foreach (var skinnedMeshRenderer in skinnedMeshRenderers) {
            var materials = skinnedMeshRenderer.materials.ToList();

            materials.RemoveAt(materials.Count - 1);

            skinnedMeshRenderer.materials = materials.ToArray();
            skinnedMeshRenderer.sharedMaterials = materials.ToArray();
        }
    }

    internal static void ApplyEsp(PlayerControllerB playerControllerB) {
        var scavengerModel = playerControllerB.transform.Find("ScavengerModel");

        if (scavengerModel == null) {
            ControlCompanyAddons.Logger.LogFatal("Couldn't find scavenger model!");
            return;
        }

        var skinnedMeshRenderers = scavengerModel.GetComponentsInChildren<SkinnedMeshRenderer>();

        skinnedMeshRenderers ??= [
        ];

        foreach (var skinnedMeshRenderer in skinnedMeshRenderers) {
            var materials = skinnedMeshRenderer.materials.ToList();

            if (materials.Contains(_material)) continue;

            materials.Add(_material);

            skinnedMeshRenderer.materials = materials.ToArray();
            skinnedMeshRenderer.sharedMaterials = materials.ToArray();
        }
    }
}
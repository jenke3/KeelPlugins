﻿using BepInEx;
using HarmonyLib;
using UnityEngine;

[assembly: System.Reflection.AssemblyFileVersion(CameraFrameMask.Koikatu.CameraFrameMask.Version)]

namespace CameraFrameMask.Koikatu
{
    [BepInPlugin(GUID, PluginName, Version)]
    public class CameraFrameMask : BaseUnityPlugin
    {
        public const string GUID = "keelhauled.cameraframemask";
        public const string PluginName = "CameraFrameMask";
        public const string Version = "1.0.1." + BuildNumber.Version;

        private static MaskComponent maskComponent;
        private static bool inStudio = Paths.ProcessName == "CharaStudio";

        private void Awake()
        {
            Log.SetLogSource(Logger);
            Harmony.CreateAndPatchAll(GetType());
        }

        [HarmonyPrefix, HarmonyPatch(typeof(CustomScene), "Start")]
        private static void MakerEntrypoint() => maskComponent = Camera.main.GetOrAddComponent<MaskComponent>();
        [HarmonyPrefix, HarmonyPatch(typeof(CustomScene), "OnDestroy")]
        private static void MakerEnd() => maskComponent = null;
        [HarmonyPrefix, HarmonyPatch(typeof(StudioScene), "Start")]
        private static void StudioEntrypoint() => maskComponent = Camera.main.GetOrAddComponent<MaskComponent>();
        [HarmonyPrefix, HarmonyPatch(typeof(HSceneProc), "SetShortcutKey")]
        private static void HSceneEntrypoint() => maskComponent = Camera.main.GetOrAddComponent<MaskComponent>();
        [HarmonyPrefix, HarmonyPatch(typeof(HSceneProc), "OnDestroy")]
        private static void HSceneEnd() => maskComponent = null;

        [HarmonyPrefix, HarmonyPatch(typeof(ChaControl), nameof(ChaControl.ChangeCoordinateType), typeof(ChaFileDefine.CoordinateType), typeof(bool))]
        private static void ChangeCoordinateTypePrefix()
        {
            MaskFrames(inStudio ? 3 : 1);
        }

        public static void MaskFrames(int count)
        {
            if(maskComponent)
                maskComponent.MaskFrames(count);
        }
    }
}

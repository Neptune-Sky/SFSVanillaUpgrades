using HarmonyLib;
using JetBrains.Annotations;
using SFS;
using SFS.Builds;

namespace VanillaUpgrades.Build
{
    [HarmonyPatch(typeof(BuildManager), "Start")]
    internal class SkipBuildNewPrompt
    {
        [UsedImplicitly]
        private static void Prefix()
        {
            if (Config.settings.skipBuildNewPrompt) Base.sceneLoader.sceneSettings.askBuildNew = false;
        }
    }
}
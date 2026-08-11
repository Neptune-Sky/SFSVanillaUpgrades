using System.Collections;
using HarmonyLib;
using ModLoader;
using SFS.UI;
using UnityEngine;

namespace VanillaUpgrades.Patches
{
    [HarmonyPatch(typeof(BuildVersionText), "LoadVersionText")]
    internal static class BuildVersionTextPatch
    {
        [HarmonyPostfix]
        private static void Postfix(
            BuildVersionText __instance,
            ref IEnumerator __result)
        {
            __result = ModifyVersionText(__instance, __result);
        }

        private static IEnumerator ModifyVersionText(
            BuildVersionText instance,
            IEnumerator original)
        {
            // Let the game's original coroutine finish first.
            yield return original;

            if (instance.textAdapter == null)
                yield break;

            Main.modCount = Loader.main.GetLoadedMods().Length;

            GameObject version = GameObject.Find("VersionText");
            version.transform.localScale = new Vector3(1.2f, 1.2f, 0f);
            version.GetComponent<TextAdapter>().Text += " - Modded\n(" + Main.modCount + " Mods Loaded)";
        }
    }
}
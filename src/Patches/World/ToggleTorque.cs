﻿using HarmonyLib;
using SFS.UI;
using SFS.World;

// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Local

namespace VanillaUpgrades
{
    [HarmonyPatch(typeof(Rocket), "GetTorque")]
    public static class ToggleTorque
    {
        private static bool disableTorque;

        public static void Set(bool setting = true)
        {
            disableTorque = setting;
        }

        public static void Toggle()
        {
            if (!PlayerController.main.HasControl(MsgDrawer.main)) return;
            Set(!disableTorque);
            MsgDrawer.main.Log("Torque " + (disableTorque ? "Disabled" : "Enabled"));
        }

        private static bool Prefix(ref float __result)
        {
            if (!disableTorque) return true;
            __result = 0f;
            return false;
        }
    }
}
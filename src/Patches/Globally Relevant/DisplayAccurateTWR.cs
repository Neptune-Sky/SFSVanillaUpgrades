﻿using HarmonyLib;
using SFS;
using SFS.Parts.Modules;
using SFS.UI;
using SFS.World;
using UnityEngine;
using SFS.Builds;
using SFS.WorldBase;

// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Local

namespace VanillaUpgrades
{
    [HarmonyPatch(typeof(FlightInfoDrawer), "Update")]
    internal static class DisplayAccurateThrustAndLocalTWR
    {
        // This method modifies the way thrust and TWR are displayed on the flight information panel:
        // - Thrust now takes into account engine orientation and stretching - also works with boosters
        // - TWR is now the local Thrust-To-Weight ratio (takes into account the local gravity)
        private static void Postfix(ref TextAdapter ___thrustText, ref TextAdapter ___thrustToWeightText)
        {
            if (PlayerController.main.player.Value is not Rocket rocket) return;
            var mass = rocket.rb2d.mass;
            var localGravity = (float)rocket.location.planet.Value.GetGravity(rocket.location.position.Value.magnitude);

            // Calculate thrust, taking into account engines state, stretching and orientation
            var thrust = new Vector2(0.0f, 0.0f);

            foreach (EngineModule engineModule in rocket.partHolder.GetModules<EngineModule>())
            {
                if (!engineModule.engineOn.Value) continue;
                var direction = (Vector2)engineModule.transform.TransformVector(engineModule.thrustNormal.Value);

                // For cheaters...
                var stretchFactor = 1.0f;
                if (Base.worldBase.AllowsCheats) stretchFactor = direction.magnitude;

                Vector2 engineThrust = engineModule.thrust.Value * direction.normalized * stretchFactor *
                                       engineModule.throttle_Out.Value;
                thrust += engineThrust;
            }

            foreach (BoosterModule boosterModule in rocket.partHolder.GetModules<BoosterModule>())
            {
                if (!boosterModule.enabled) continue;
                var direction =
                    (Vector2)boosterModule.transform.TransformVector(boosterModule.thrustVector
                        .Value); // equal to thrust * stretch factor in magnitude

                // For cheaters...
                var stretchFactor = 1.0f;
                if (Base.worldBase.AllowsCheats)
                    stretchFactor = direction.magnitude / boosterModule.thrustVector.Value.magnitude;

                // adjusting direction to what it should be now
                direction = direction.normalized;

                thrust += boosterModule.thrustVector.Value.magnitude * direction.normalized * stretchFactor;
            }

            var thrustVal = thrust.magnitude;

            ___thrustText.Text = thrustVal.ToThrustString().Split(':')[1];
            ___thrustToWeightText.Text = (mass != 0 ? thrustVal / mass * 9.8f / localGravity : 0).ToTwrString().Split(':')[1];
        }
    }
    
    // TWR in build mode will simply be calculated using the gravity of the planet the launchpad is on.
    [HarmonyPatch(typeof(BuildStatsDrawer), "Draw")]
    internal class DisplayCorrectTWRInBuild
    {
        private static void Postfix(float ___mass, float ___thrust, TextAdapter ___thrustToWeightText)
        {
            SpaceCenterData spaceCenter = Base.planetLoader.spaceCenter;
            var gravityAtLaunchpad = spaceCenter.address.GetPlanet()
                .GetGravity(spaceCenter.LaunchPadLocation.position.magnitude);
            var TWR = ___mass != 0 ? ___thrust * 9.8 / (___mass * gravityAtLaunchpad) : 0;
            ___thrustToWeightText.Text = TWR.ToString(2, true);
        }
    }
}
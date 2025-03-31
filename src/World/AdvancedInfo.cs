﻿using System;
using System.Collections.Generic;
using System.Globalization;
using ModLoader.Helpers;
using SFS.UI;
using SFS.UI.ModGUI;
using SFS.World;
using UnityEngine;
using VanillaUpgrades.Utility;
using UIExtensions = VanillaUpgrades.Utility.UIExtensions;

namespace VanillaUpgrades
{
    public static partial class AdvancedInfo
    {
        private const string PositionKey = "VU.AdvancedInfoWindow";

        private static Dictionary<string, TextAdapter> newStats = new()
        {
            { "Apoapsis", null },
            { "Periapsis", null },
            { "Eccentricity", null },
            { "Angle", null },
            { "AngleTitle", null }
        };

        private static readonly List<GameObject> infoObjects = new();

        public static GameObject windowHolder;
        private static Window advancedInfoWindow;
        private static Container horizontal;

        private static Dictionary<string, Label> infoLabels = new()
        {
            { "apopsisHorizontal", null },
            { "apoapsisVertical", null },
            { "periapsisHorizontal", null },
            { "periapsisVertical", null },
            { "eccentricityHorizontal", null },
            { "eccentricityVertical", null },
            { "angleHorizontal", null },
            { "angleVertical", null },
            { "angleTitleHorizontal", null },
            { "angleTitleVertical", null }
        };

        private static Container vertical;

        public static void Setup()
        {
            windowHolder = UIExtensions.ZeroedHolder(Builder.SceneToAttach.CurrentScene, "AdvancedInfoHolder");

            AddToVanillaGUI();
            CreateWindow();
            VerticalGUI();
            HorizontalGUI();
            CheckHorizontalToggle();


            PlayerController.main.player.OnChange += OnPlayerChange;
            Config.settings.horizontalMode.OnChange += CheckHorizontalToggle;
            Config.settings.persistentVars.windowScale.OnChange += () => advancedInfoWindow.ScaleWindow();
            Config.settings.showAdvanced.OnChange += OnToggle;
            Config.settings.showAdvancedInSeparateWindow.OnChange += OnToggle;

            OnToggle();

            UpdateInGame.execute += Update;
            SceneHelper.OnWorldSceneUnloaded += OnDestroy;
        }

        private static void Update()
        {
            if (WorldManager.currentRocket == null) return;
            if (Config.settings.showAdvancedInSeparateWindow)
                RefreshLabels(infoLabels);
            else RefreshLabels(newStats);
        }

        private static void OnDestroy()
        {
            infoObjects.Clear();
            PlayerController.main.player.OnChange -= OnPlayerChange;
            Config.settings.horizontalMode.OnChange -= CheckHorizontalToggle;
            Config.settings.persistentVars.windowScale.OnChange -= () => advancedInfoWindow.ScaleWindow();
            Config.settings.showAdvanced.OnChange -= OnToggle;
            Config.settings.showAdvancedInSeparateWindow.OnChange -= OnToggle;

            infoLabels = new Dictionary<string, Label>
            {
                { "apopsisHorizontal", null },
                { "apoapsisVertical", null },
                { "periapsisHorizontal", null },
                { "periapsisVertical", null },
                { "eccentricityHorizontal", null },
                { "eccentricityVertical", null },
                { "angleHorizontal", null },
                { "angleVertical", null },
                { "angleTitleHorizontal", null },
                { "angleTitleVertical", null }
            };
            newStats = new Dictionary<string, TextAdapter>
            {
                { "Apoapsis", null },
                { "Periapsis", null },
                { "Eccentricity", null },
                { "Angle", null },
                { "AngleTitle", null }
            };
        }

        private static void OnPlayerChange()
        {
            if (PlayerController.main == null) return;
            OnToggle();
            ToggleTorque.Set(false);
        }

        private static void OnToggle()
        {
            if (PlayerController.main == null) return;
            var value = WorldManager.currentRocket != null && Config.settings.showAdvanced;
            windowHolder.SetActive(value && Config.settings.showAdvancedInSeparateWindow);
            infoObjects.ForEach(e => e.SetActive(value && !Config.settings.showAdvancedInSeparateWindow));
        }

        private static void CheckHorizontalToggle()
        {
            if (Config.settings.horizontalMode)
            {
                vertical.gameObject.SetActive(false);
                horizontal.gameObject.SetActive(true);
                advancedInfoWindow.Size = new Vector2(350, 230);
            }
            else
            {
                horizontal.gameObject.SetActive(false);
                vertical.gameObject.SetActive(true);
                advancedInfoWindow.Size = new Vector2(220, 350);
            }

            advancedInfoWindow.ClampWindow();
        }

        private static void GetValues(out string apoapsis, out string periapsis, out string eccentricity,
            out string angleTitle, out string angle)
        {
            Rocket rocket = WorldManager.currentRocket;
            if (rocket.physics.GetTrajectory().paths[0] is Orbit orbit)
            {
                apoapsis = (orbit.apoapsis - rocket.location.planet.Value.Radius)
                    .ToDistanceString();

                var truePeriapsis =
                    orbit.periapsis < rocket.location.planet.Value.Radius
                        ? 0
                        : orbit.periapsis - rocket.location.planet.Value.Radius;
                periapsis = truePeriapsis.ToDistanceString();

                eccentricity = orbit.ecc.ToString("F3", CultureInfo.InvariantCulture);
            }
            else
            {
                apoapsis = "0.0m";
                periapsis = "0.0m";
                eccentricity = "0.000";
            }

            var globalAngle = rocket.partHolder.transform.eulerAngles.z;
            Location location = rocket.location.Value;

            Vector2 orbitAngleVector =
                new Vector2(Mathf.Cos((float)location.position.AngleRadians),
                    Mathf.Sin((float)location.position.AngleRadians)).Rotate_Radians(270 * Mathf.Deg2Rad);
            var facing = new Vector2(Mathf.Cos(globalAngle * Mathf.Deg2Rad), Mathf.Sin(globalAngle * Mathf.Deg2Rad));
            var trueAngle = Vector2.SignedAngle(facing, orbitAngleVector);

            if (location.TerrainHeight < location.planet.TimewarpRadius_Ascend - rocket.location.planet.Value.Radius)
            {
                angle = trueAngle.ToString("F1", CultureInfo.InvariantCulture) + "°";
                angleTitle = "Local Angle";
                return;
            }

            if (globalAngle > 180) angle = (360 - globalAngle).ToString("F1", CultureInfo.InvariantCulture) + "°";
            else angle = (-globalAngle).ToString("F1", CultureInfo.InvariantCulture) + "°";

            angleTitle = "Angle";
        }

        private static void RefreshLabels<T>(Dictionary<string, T> texts)
        {
            GetValues(out var apo, out var peri, out var ecc, out var angTitle, out var ang);

            if (texts is Dictionary<string, Label> labelDict)
            {
                labelDict["apoapsisVertical"].Text = labelDict["apoapsisHorizontal"].Text = apo;
                labelDict["periapsisVertical"].Text = labelDict["periapsisHorizontal"].Text = peri;
                labelDict["eccentricityVertical"].Text = labelDict["eccentricityHorizontal"].Text = ecc;
                labelDict["angleVertical"].Text = labelDict["angleHorizontal"].Text = ang;
                labelDict["angleTitleVertical"].Text = labelDict["angleTitleHorizontal"].Text = angTitle;
                return;
            }

            if (texts is not Dictionary<string, TextAdapter> adapterDict)
                throw new ArgumentNullException(nameof(adapterDict));
            adapterDict["Apoapsis"].Text = apo;
            adapterDict["Periapsis"].Text = peri;
            adapterDict["Eccentricity"].Text = ecc;
            adapterDict["Angle"].Text = ang;
            adapterDict["AngleTitle"].Text = angTitle;
        }
    }
}
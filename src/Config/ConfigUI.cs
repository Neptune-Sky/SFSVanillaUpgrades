using System;
using System.Globalization;
using SFS.UI.ModGUI;
using UITools;
using UnityEngine;
using static SFS.UI.ModGUI.Builder;
using Type = SFS.UI.ModGUI.Type;
using UIExtensions = VanillaUpgrades.Utility.UIExtensions;

// ReSharper disable StringLiteralTypo

namespace VanillaUpgrades
{
    public static class ConfigUI
    {
        private const int ToggleHeight = 32;

        public static void Setup()
        {
            ConfigurationMenu.Add("VU", new (string, Func<Transform, GameObject>)[]
            {
                ("Display", transform1 => GetGUISettings(transform1, ConfigurationMenu.ContentSize)),
                ("Units", transform1 => GetUnitsSettings(transform1, ConfigurationMenu.ContentSize)),
                ("Misc", transform1 => GetMiscSettings(transform1, ConfigurationMenu.ContentSize))
            });
            // ("Windows", transform1 => GetWindowSettings(transform1, ConfigurationMenu.ContentSize))
        }

        private static GameObject GetGUISettings(Transform parent, Vector2Int size)
        {
            Box box = CreateBox(parent, size.x, size.y);
            box.CreateLayoutGroup(Type.Vertical, TextAnchor.UpperCenter, 35, new RectOffset(15, 15, 15, 15));

            var elementWidth = size.x - 60;

            CreateLabel(box, elementWidth, 50, 0, 0, "Display");

            Container container = CreateContainer(box);
            container.CreateLayoutGroup(Type.Horizontal, TextAnchor.MiddleLeft, 0);

            UIExtensions.AlignedLabel(container, elementWidth - 225, ToggleHeight, "Window Scale");

            Container SliderLabel = CreateContainer(container);
            SliderLabel.CreateLayoutGroup(Type.Horizontal, TextAnchor.MiddleRight);

            CreateSlider(SliderLabel, 225, Config.settings.persistentVars.windowScale.Value, (0.5f, 1.5f),
                false,
                val => { Config.settings.persistentVars.windowScale.Value = val; },
                val => Math.Round(val * 100) + "%");

            CreateSeparator(box, elementWidth - 20);

            if (!Main.buildSettingsPresent)
            {
                CreateToggleWithLabel(box, elementWidth, ToggleHeight, () => Config.settings.showBuildGui,
                    () => Config.settings.showBuildGui.Value ^= true, 0, 0, "Show Build Settings");
                CreateSeparator(box, elementWidth - 20);
            }

            CreateToggleWithLabel(box, elementWidth, ToggleHeight, () => Config.settings.showAdvanced,
                () => Config.settings.showAdvanced.Value ^= true, 0, 0, "Show Advanced Info");
            CreateToggleWithLabel(box, elementWidth, ToggleHeight, () => Config.settings.showAdvancedInSeparateWindow,
                () => Config.settings.showAdvancedInSeparateWindow.Value ^= true, 0, 0,
                "Separate Advanced Info Window");
            CreateToggleWithLabel(box, elementWidth, ToggleHeight, () => Config.settings.horizontalMode,
                () => Config.settings.horizontalMode.Value ^= true, 0, 0, "Horizontal Mode");
            CreateSeparator(box, elementWidth - 20);
            CreateToggleWithLabel(box, elementWidth, ToggleHeight, () => Config.settings.showTime,
                () => Config.settings.showTime.Value ^= true, 0, 0, "Show Clock While Timewarping");
            CreateToggleWithLabel(box, elementWidth, ToggleHeight, () => Config.settings.showWorldTime,
                () => Config.settings.showWorldTime.Value ^= true, 0, 0, "Show World Time in Clock");
            CreateToggleWithLabel(box, elementWidth, ToggleHeight, () => Config.settings.alwaysShowTime,
                () => Config.settings.alwaysShowTime.Value ^= true, 0, 0, "Always Show World Clock");
            CreateSeparator(box, elementWidth - 20);

            CreateToggleWithLabel(box, elementWidth, ToggleHeight, () => Config.settings.explosions,
                () => Config.settings.explosions ^= true, 0, 0, "Explosion Effects");
            CreateToggleWithLabel(box, elementWidth, ToggleHeight, () => Config.settings.darkenDebris,
                () => Config.settings.darkenDebris ^= true, 0, 0, "Darken Debris Map Icons");
            CreateToggleWithLabel(box, elementWidth, ToggleHeight, () => Config.settings.morePrecisePercentages,
                () => Config.settings.morePrecisePercentages ^= true, 0, 0, "More Precise Percentages");
            CreateToggleWithLabel(box, elementWidth, ToggleHeight, () => Config.settings.hideTopLeftButtonText,
                () => Config.settings.hideTopLeftButtonText.Value ^= true, 0, 0, "Hide Top Left Button Text In World");


            return box.gameObject;
        }

        private static GameObject GetUnitsSettings(Transform parent, Vector2Int size)
        {
            Box box = CreateBox(parent, size.x, size.y);
            box.CreateLayoutGroup(Type.Vertical, TextAnchor.UpperCenter, 35, new RectOffset(15, 15, 15, 15));

            var elementWidth = size.x - 60;

            CreateLabel(box, elementWidth, 50, 0, 0, "Units");

            CreateToggleWithLabel(box, elementWidth, ToggleHeight, () => Config.settings.mmUnits,
                () => Config.settings.mmUnits ^= true, 0, 0, "Megameters (Mm)");
            CreateToggleWithLabel(box, elementWidth, ToggleHeight, () => Config.settings.gmUnits,
                () => Config.settings.gmUnits ^= true, 0, 0, "Gigameters (Gm)");
            CreateToggleWithLabel(box, elementWidth, ToggleHeight, () => Config.settings.lyUnits,
                () => Config.settings.lyUnits ^= true, 0, 0, "Lightyears (ly)");
            CreateSeparator(box, elementWidth - 20);
            CreateToggleWithLabel(box, elementWidth, ToggleHeight, () => Config.settings.kmsUnits,
                () => Config.settings.kmsUnits ^= true, 0, 0, "Kilometers/Second (km/s)");
            CreateToggleWithLabel(box, elementWidth, ToggleHeight, () => Config.settings.cUnits,
                () => Config.settings.cUnits ^= true, 0, 0, "% Speed of Light (c)");
            CreateSeparator(box, elementWidth - 20);
            CreateToggleWithLabel(box, elementWidth, ToggleHeight, () => Config.settings.ktUnits,
                () => Config.settings.ktUnits ^= true, 0, 0, "Kilotonnes (kt)");

            return box.gameObject;
        }

        private static GameObject GetMiscSettings(Transform parent, Vector2Int size)
        {
            Box box = CreateBox(parent, size.x, size.y);
            box.CreateLayoutGroup(Type.Vertical, TextAnchor.UpperCenter, 35, new RectOffset(15, 15, 15, 15));

            var elementWidth = size.x - 60;

            CreateLabel(box, elementWidth, 50, 0, 0, "Miscellaneous");
            Container autosaveContainer = CreateContainer(box);
            autosaveContainer.CreateLayoutGroup(Type.Horizontal, TextAnchor.MiddleLeft, 0);

            UIExtensions.AlignedLabel(autosaveContainer, elementWidth - 225, ToggleHeight, "Autosave Timer");

            Container autosaveSliderLabel = CreateContainer(autosaveContainer);
            autosaveSliderLabel.CreateLayoutGroup(Type.Horizontal, TextAnchor.MiddleRight);

            CreateSlider(autosaveSliderLabel, 225, Config.settings.persistentVars.minutesUntilAutosave, (5, 60),
                true,
                val => { Config.settings.persistentVars.minutesUntilAutosave = val; },
                val => val.Round(0).ToString(CultureInfo.InvariantCulture) + " Min");

            Container autosaveSlotsContainer = CreateContainer(box);
            autosaveSlotsContainer.CreateLayoutGroup(Type.Horizontal, TextAnchor.MiddleLeft, 0);

            UIExtensions.AlignedLabel(autosaveSlotsContainer, elementWidth - 225, ToggleHeight, "Allowed Autosaves");

            Container autosaveSlotsSliderLabel = CreateContainer(autosaveSlotsContainer);
            autosaveSlotsSliderLabel.CreateLayoutGroup(Type.Horizontal, TextAnchor.MiddleRight);

            CreateSlider(autosaveSlotsSliderLabel, 225, Config.settings.persistentVars.allowedAutosaveSlots, (0, 10),
                true,
                val => { Config.settings.persistentVars.allowedAutosaveSlots = (int)val; },
                val => val.Round(0).ToString(CultureInfo.InvariantCulture) + "");

            CreateSeparator(box, elementWidth - 20);

            CreateToggleWithLabel(box, elementWidth, ToggleHeight, () => !Config.settings.allowBackgroundProcess,
                () => Config.settings.allowBackgroundProcess.Value ^= true, 0, 0, "Pause Game When Focus is Lost");
            CreateSeparator(box, elementWidth - 20);

            CreateToggleWithLabel(box, elementWidth, ToggleHeight,
                () => Config.settings.stopTimewarpOnEncounter,
                () => Config.settings.stopTimewarpOnEncounter ^= true, 0, 0, "Stop Timewarp On Encounter");
            CreateSeparator(box, elementWidth - 20);

            CreateToggleWithLabel(box, elementWidth, ToggleHeight, () => Config.settings.moreCameraZoom,
                () => Config.settings.moreCameraZoom ^= true, 0, 0, "More Camera Zoom");
            CreateToggleWithLabel(box, elementWidth, ToggleHeight, () => Config.settings.moreBuildCameraZoom,
                () => Config.settings.moreBuildCameraZoom ^= true, 0, 0, "More Build Camera Zoom");
            CreateToggleWithLabel(box, elementWidth, ToggleHeight, () => Config.settings.moreCameraMove,
                () => Config.settings.moreCameraMove ^= true, 0, 0, "More Camera Movement");
            CreateSeparator(box, elementWidth - 20);

            CreateToggleWithLabel(box, elementWidth, ToggleHeight, () => Config.settings.allowTimeSlowdown,
                () => Config.settings.allowTimeSlowdown.Value ^= true, 0, 0, "Allow Time Slowdown");
            CreateToggleWithLabel(box, elementWidth, ToggleHeight, () => Config.settings.higherPhysicsWarp,
                () => Config.settings.higherPhysicsWarp ^= true, 0, 0, "Higher Physics Timewarps");

            return box.gameObject;
        }
    }
}
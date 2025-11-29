using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using JetBrains.Annotations;
using SFS.Input;
using SFS.Logs;
using SFS.UI;
using SFS.UI.ModGUI;
using SFS.World;
using SFS.World.Maps;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VanillaUpgrades.Utility;
using Button = SFS.UI.Button;

namespace VanillaUpgrades.World
{
    public static class MissionLogButton
    {
        private static readonly IconButton MissionLog = new();

        public static void Create()
        {
            GameObject parent = GameSelector.main.focusButton.button.gameObject;
            MissionLog.button = Object.Instantiate(parent).GetComponent<Button>();
            GameObject gameObject = MissionLog.button.gameObject;
            gameObject.transform.SetParent(parent.transform.parent);
            gameObject.transform.localScale = Vector3.one;
            MissionLog.text = MissionLog.button.gameObject.GetComponentInChildren<TextAdapter>();
            MissionLog.text.Text = "Mission Log";
            MissionLog.Show = false;
            GameSelector.main.selected.OnChange += selected =>
            {
                GameObject endMissionButton = GameObject.Find("End Mission Button");
                var endMissionActive = endMissionButton != null && endMissionButton.activeSelf &&
                                       endMissionButton.GetComponentInChildren<TextAdapter>().Text == "Destroy";
                if (selected is MapPlayer && !endMissionActive)
                {
                    MissionLog.Show = true;
                    return;
                }

                MissionLog.Show = false;
            };
            MissionLog.button.onClick += OpenMenu;
        }

        public static void OpenMenu()
        {
            var mapRocket = GameSelector.main.selected.Value as MapRocket;
            Rocket rocket = mapRocket != null ? mapRocket.rocket : PlayerController.main.player.Value as Rocket;
            if (rocket == null) return;
            MethodInfo logsMethod = EndMissionMenu.main.GetType()
                .GetMethod("ReplayMission", BindingFlags.NonPublic | BindingFlags.Static);

            if (logsMethod == null) return;

            OpenMissionLog((List<(string, double, LogId)>)logsMethod.Invoke(EndMissionMenu.main,
                new object[] { rocket.stats.branch, rocket.location.Value, null, null, null }));
        }

        private static void OpenMissionLog(List<(string, double, LogId)> missions)
        {
            var menuElement = new MenuElement(delegate(GameObject root)
            {
                // Create the window.
                var containerObject = new GameObject("ModGUI Container");
                var rectTransform = containerObject.AddComponent<RectTransform>();
                rectTransform.sizeDelta = new Vector2(0, 0);

                Vector2 canvasSize = UIExtensions.ActualCanvasSize;
                Vector2Int windowSize = Vector2Int.Min(new Vector2Int((int)(canvasSize.x * 0.7f),
                    (int)canvasSize.y - 200), new Vector2Int(1200, 1000));

                Window scroll = Builder.CreateWindow(rectTransform, Builder.GetRandomID(), windowSize.x, windowSize.y,
                    0, windowSize.y / 2, false, false,
                    1, "Mission Log");

                // Populate the window with the mission entries.
                scroll.CreateLayoutGroup(Type.Vertical, TextAnchor.MiddleCenter, 5, null, false);
                scroll.EnableScrolling(Type.Vertical);
                containerObject.transform.SetParent(root.transform);

                foreach ((string, double, LogId) line in missions)
                    UIExtensions.AlignedLabel(scroll, (int)scroll.Size.x - 30, 35,
                        "- " + line.Item1, TextAlignmentOptions.Left,
                        false, Mathf.Clamp(50 * UIExtensions.GetCanvasRect().lossyScale.x, 10, 30));

                Builder.CreateLabel(scroll, 1, (int)(scroll.Size.y / 50), text: " ");

                scroll.gameObject.GetComponentInChildren<RectMask2D>().padding = new Vector4(0, 70, 0, 0);
                Container okayButton =
                    Builder.CreateContainer(scroll.gameObject.transform, 0, (int)(-scroll.Size.y + scroll.Size.y / 23));
                okayButton.CreateLayoutGroup(Type.Horizontal);

                // Create the "Okay" button to close the window.
                Builder.CreateButton(okayButton, (int)(scroll.Size.x / 6), (int)(scroll.Size.y / 19.5f),
                    onClick: ScreenManager.main.CloseCurrent, text: "Okay");
            });
            MenuGenerator.OpenMenu(CancelButton.Cancel, CloseMode.Current, menuElement);
        }
    }

    [HarmonyPatch(typeof(EndMissionMenu), "OpenEndMissionMenu")]
    internal class EndMissionMenuHook
    {
        private static SFS.UI.ModGUI.Button missionLogButton;

        [UsedImplicitly]
        private static void Postfix(EndMissionMenu __instance)
        {
            if (missionLogButton == null)
            {
                GameObject completeButton =
                    GameObject.Find("Complete Buttons")?.transform.Find("Complete Button").gameObject;
                if (completeButton != null)
                {
                    missionLogButton = Builder.CreateButton(completeButton.transform.parent,
                        (int)completeButton.Rect().sizeDelta.x, (int)completeButton.Rect().sizeDelta.y,
                        onClick: MissionLogButton.OpenMenu, text: "Mission Log");
                    missionLogButton.gameObject.name = "Mission Log Button";
                }
            }

            missionLogButton?.gameObject.SetActive(__instance.titleText.Text.ToLower().Contains("challenges"));
        }
    }
}
using System;
using System.Collections.Generic;
using HarmonyLib;
using ModLoader;
using ModLoader.Helpers;
using SFS.IO;
using SFS.UI;
using TMPro;
using UITools;
using UnityEngine;
using VanillaUpgrades.Build;
using VanillaUpgrades.World;
using Console = ModLoader.IO.Console;
using Object = UnityEngine.Object;

namespace VanillaUpgrades
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class Main : Mod, IUpdatable
    {
        public static Main inst;

        public static bool buildSettingsPresent;

        private static GameObject mainObject;

        private static Harmony patcher;

        private static int modCount = 2;
        public override string ModNameID => "VanUp";
        public override string DisplayName => "Vanilla Upgrades";
        public override string Author => "NeptuneSky";
        public override string MinimumGameVersionNecessary => "1.6";
        public override string ModVersion => "v5.3.1.1";

        public override string Description =>
            "Upgrades the vanilla experience with quality-of-life features and keybinds. See the GitHub repository for a list of features. Credits can be found in the wiki.";

        public override Action LoadKeybindings => VuKeybindings.LoadKeybindings;

        public override Dictionary<string, string> Dependencies => new() { { "UITools", "1.0" } };

        public Dictionary<string, FilePath> UpdatableFiles => new()
        {
            {
                "https://github.com/Neptune-Sky/SFSVanillaUpgrades/releases/latest/download/VanillaUpgrades.dll",
                new FolderPath(ModFolder).ExtendToFile("VanillaUpgrades.dll")
            }
        };
        
        public static IFolder modFolder;

        public override void Early_Load()
        {
            try
            {
                inst = this;
                modFolder = new DefaultFolder(ModFolder);
                patcher = new Harmony("mods.NeptuneSky.VanUp");
                patcher.PatchAll();
                SubscribeToScenes();
                Application.quitting += OnQuit;
                Config.Load();
                ConfigUI.Setup();
                Application.runInBackground = Config.settings.allowBackgroundProcess;
                Config.settings.allowBackgroundProcess.OnChange +=
                    () => Application.runInBackground = Config.settings.allowBackgroundProcess;
            }
            catch (Exception e)
            {
                ErrorNotification.Error(e.Message);
            }
        }

        public override void Load()
        {
            Console.commands.Add(Command);
            
            mainObject = new GameObject("NeptuneMainObject", typeof(ErrorNotification));
            Object.DontDestroyOnLoad(mainObject);
            mainObject.SetActive(true);
            
            UpdateVersionText();
        }

        private static void SubscribeToScenes()
        {
            SceneHelper.OnHomeSceneLoaded += UpdateVersionText;
            SceneHelper.OnBuildSceneLoaded += BuildSettings.Setup;
            SceneHelper.OnWorldSceneLoaded += WorldManager.Setup;
        }

        private static void OnQuit()
        {
            Config.Save();
        }

        private static bool Command(string str)
        {
            if (!str.StartsWith("reset")) return false;
            ApplicationUtility.Relaunch();

            return true;
        }

        private static void UpdateVersionText()
        {
            GameObject version = GameObject.Find("Version");
            if (!version) return;
            modCount = Loader.main.GetLoadedMods().Length;

            version.GetComponent<TextMeshProUGUI>().autoSizeTextContainer = true;
            version.GetComponent<TextAdapter>().Text += " - Modded\n(" + modCount + " Mods Loaded)";
        }
    }

    [HarmonyPatch(typeof(Loader), "Initialize_Load")]
    internal class ModCheck
    {
        // ReSharper disable once InconsistentNaming
        // ReSharper disable once UnusedMember.Local
        private static void Postfix(List<Mod> ___loadedMods)
        {
            if (___loadedMods.FindIndex(e => e.ModNameID == "BuildSettings") == -1) return;
            Main.buildSettingsPresent = true;
            Debug.Log(
                "[VanillaUpgrades] BuildSettings mod was detected, disabling own Build Settings features to avoid conflicts.");
        }
    }
}
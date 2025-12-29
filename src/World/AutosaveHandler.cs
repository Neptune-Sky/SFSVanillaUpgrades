using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using SFS;
using SFS.Parsers.Json;
using SFS.World;
using UnityEngine;


namespace VanillaUpgrades.World
{
    [HarmonyPatch(typeof(GameManager), "Update")]
    internal static class AutosaveHandler
    {
        private static float _timeSinceLastAutosave;

        private static void Postfix()
        {
            if (Time.timeScale < 0.01f) return;
            if (Config.settings.persistentVars.allowedAutosaveSlots == 0)
            {
                _timeSinceLastAutosave = 0f;
                return;
            }

            _timeSinceLastAutosave += Time.unscaledDeltaTime;
            if (_timeSinceLastAutosave < Config.settings.persistentVars.minutesUntilAutosave * 60) return;

            var fileList = new AutosaveFile();
            var rootPath = Base.worldBase.paths.path;
            var quicksavesPath = Base.worldBase.paths.quicksavesPath;

            if (rootPath.GetFileUnsafe("Autosaves.txt").Exists())
            {
                var filePath = rootPath.GetFileUnsafe("Autosaves.txt");
                try
                {
                    var jsonContent = filePath.ReadText();
                    fileList = JsonUtility.FromJson<AutosaveFile>(jsonContent);
                }
                catch (Exception)
                {
                    filePath.Delete();
                }
            }

            var toSave = Traverse.Create(GameManager.main).Method("CreateWorldSave").GetValue() as WorldSave;
            var name = "Autosave " + DateTime.Now.ToString("yyyy-MM-dd HH_mm_ss");
            IFolder where = quicksavesPath.GetFolderUnsafe(name).Create();

            WorldSave.Save(where, true, toSave, Base.worldBase.IsCareer);

            fileList.fileNames.Add(name);

            // Check if directories exist and remove non-existent ones
            fileList.fileNames = fileList.fileNames
                .Where(dir => quicksavesPath.GetFolderUnsafe(dir).Exists())
                .ToList();

            // If the number of directories exceeds the limit, delete the oldest ones
            if (fileList.fileNames.Count > Config.settings.persistentVars.allowedAutosaveSlots)
            {
                // Get directories with their last modified times
                var directoryInfos = fileList.fileNames
                    .Select(dir => new
                    {
                        Name = dir,
                        Folder = quicksavesPath.GetFolderUnsafe(dir),
                        LastWriteTime = quicksavesPath.GetFolderUnsafe(dir).GetLastModifiedTime()
                    })
                    .Where(x => x.Folder.Exists())
                    .OrderBy(x => x.LastWriteTime)
                    .ToList();

                int toDelete = directoryInfos.Count - Config.settings.persistentVars.allowedAutosaveSlots;

                for (int i = 0; i < toDelete; i++)
                {
                    directoryInfos[i].Folder.Delete();
                    fileList.fileNames.Remove(directoryInfos[i].Name);
                }
            }


            JsonWrapper.SaveAsJson(rootPath.GetFileUnsafe("Autosaves.txt"), fileList, false);

            _timeSinceLastAutosave = 0f;
        }

        [Serializable]
        private class AutosaveFile
        {
            public List<string> fileNames = new();
        }
    }
}
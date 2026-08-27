using System;
using System.IO;
using UnityEngine;

namespace SmartElectric.Domain
{
    /// <summary>Save/load RoomModel JSON under persistentDataPath/projects.</summary>
    public static class RoomModelStore
    {
        const string FolderName = "projects";

        public static string ProjectsDirectory =>
            Path.Combine(Application.persistentDataPath, FolderName);

        public static string GetPath(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                fileName = "room.json";
            if (!fileName.EndsWith(".json", StringComparison.OrdinalIgnoreCase))
                fileName += ".json";
            return Path.Combine(ProjectsDirectory, fileName);
        }

        public static void Save(RoomModel model, string fileName = "current_room.json")
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            model.TouchUpdated();
            Directory.CreateDirectory(ProjectsDirectory);
            var path = GetPath(fileName);
            var json = RoomModelJsonSerializer.ToJson(model, prettyPrint: true);
            File.WriteAllText(path, json);
            Debug.Log($"[SmartElectric] Saved RoomModel → {path}");
        }

        public static bool TryLoad(string fileName, out RoomModel model)
        {
            model = null;
            var path = GetPath(fileName);
            if (!File.Exists(path))
            {
                Debug.LogWarning($"[SmartElectric] No save at {path}");
                return false;
            }

            var json = File.ReadAllText(path);
            model = RoomModelJsonSerializer.FromJson(json);
            Debug.Log($"[SmartElectric] Loaded RoomModel ← {path}");
            return true;
        }
    }
}

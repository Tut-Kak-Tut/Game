using System;
using System.IO;
using UnityEngine;

namespace Game.CoreRuntime
{
    public sealed class GraveyardService
    {
        public static string GraveyardDir => Path.Combine(Application.persistentDataPath, "saves", "graveyard");

        public string ArchiveSave(string characterId, string saveJson)
        {
            if (string.IsNullOrWhiteSpace(saveJson))
                return string.Empty;

            string safeId = string.IsNullOrWhiteSpace(characterId) ? "anonymous" : characterId;
            foreach (char invalid in Path.GetInvalidFileNameChars())
                safeId = safeId.Replace(invalid, '_');

            try
            {
                Directory.CreateDirectory(GraveyardDir);
                string filePath = Path.Combine(GraveyardDir, $"{safeId}-{DateTime.UtcNow.Ticks}.json");
                File.WriteAllText(filePath, saveJson);
                return filePath;
            }
            catch (Exception ex)
            {
                Debug.LogError($"[GraveyardService] Failed to archive save: {ex.Message}");
                return string.Empty;
            }
        }

        public string[] ListGraveyardFiles()
        {
            if (!Directory.Exists(GraveyardDir))
                return Array.Empty<string>();

            try
            {
                return Directory.GetFiles(GraveyardDir, "*.json");
            }
            catch (Exception ex)
            {
                Debug.LogError($"[GraveyardService] Failed to list graveyard: {ex.Message}");
                return Array.Empty<string>();
            }
        }
    }
}

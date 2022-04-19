namespace RoomBuildingStarterKit.Common
{
    using System;
    using System.IO;
    using UnityEngine;

    /// <summary>
    /// The save game helper class.
    /// </summary>
    public class SaveHelper
    {
        /// <summary>
        /// The persistent data path.
        /// </summary>
        public static string PersistentDataPath { get => $"{Application.dataPath}/RoomBuildingStarterKit/Save/GameData"; }

        /// <summary>
        /// Saves game content.
        /// </summary>
        /// <param name="folderName">The folder name.</param>
        /// <param name="fileName">The file name.</param>
        public static void Save(string fileName, string content)
        {
            try
            {
                string path = SaveHelper.PersistentDataPath;

                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                string filePath = $@"{path}/{fileName}.json";

                if (!File.Exists(filePath))
                {
                    File.Create(filePath).Dispose();
                }

                File.WriteAllText(filePath, content);
            }
            catch (Exception e)
            {
                Debug.Log($"Save game content failed: {e.Message}");
                throw new SaveGameContentException(e.Message);
            }
        }

        /// <summary>
        /// Loads game content.
        /// </summary>
        /// <param name="folderName">The folder name.</param>
        /// <param name="fileName">The file name.</param>
        /// <returns>The json string.</returns>
        public static string Load(string fileName)
        {
            try
            {
                return File.ReadAllText($@"{SaveHelper.PersistentDataPath}/{fileName}.json");
            }
            catch (Exception e)
            {
                Debug.Log($"Load game content failed: {e.Message}");
                throw new LoadGameContentException(e.Message);
            }
        }
    }
}
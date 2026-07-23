using System;
using System.IO;
using UnityEngine;

namespace Eric.Save
{
        public static class SatelliteJsonSaveSystem
        {
                private const string FileName = "SatelliteSave.json";

                public static string SavePath =>
                        Path.Combine(Application.persistentDataPath, FileName);

                public static void Save(SatelliteSaveData saveData)
                {
                        if (saveData == null) return;

                        Directory.CreateDirectory(Application.persistentDataPath);

                        string json = JsonUtility.ToJson(saveData, true);
                        File.WriteAllText(SavePath, json);
                }

                public static bool TryLoad(out SatelliteSaveData saveData)
                {
                        saveData = null;

                        if (!File.Exists(SavePath))
                                return false;

                        try
                        {
                                string json = File.ReadAllText(SavePath);

                                if (string.IsNullOrWhiteSpace(json))
                                        return false;

                                saveData = JsonUtility.FromJson<SatelliteSaveData>(json);

                                if (saveData == null)
                                        return false;

                                saveData.unlockedSatellites ??= new();
                                return true;
                        }
                        catch (Exception exception) { return false; }
                }

                public static bool HasSave()
                {
                        return File.Exists(SavePath);
                }

                public static bool DeleteSave()
                {
                        if (!File.Exists(SavePath))
                                return true;

                        try
                        {
                                File.Delete(SavePath);
                                return true;
                        }
                        catch (Exception exception) { return false; }
                }
        }
}
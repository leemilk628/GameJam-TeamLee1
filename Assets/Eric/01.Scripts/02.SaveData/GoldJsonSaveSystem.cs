using System;
using System.IO;
using UnityEngine;

namespace Eric.Save
{
        public static class GoldJsonSaveSystem
        {
                private const string FileName = "GoldSave.json";

                public static string SavePath =>
                        Path.Combine(Application.persistentDataPath, FileName);

                public static bool Save(GoldSaveData saveData)
                {
                        if (saveData == null) return false;

                        try
                        {
                                Directory.CreateDirectory(Application.persistentDataPath);

                                string json = JsonUtility.ToJson(saveData, true);
                                File.WriteAllText(SavePath, json);

                                return true;
                        }
                        catch (Exception exception) {return false;}
                }

                public static bool TryLoad(out GoldSaveData saveData)
                {
                        saveData = null;

                        if (!File.Exists(SavePath))
                                return false;

                        try
                        {
                                string json = File.ReadAllText(SavePath);

                                if (string.IsNullOrWhiteSpace(json))
                                        return false;

                                saveData = JsonUtility.FromJson<GoldSaveData>(json);

                                return saveData != null;
                        }
                        catch (Exception exception) {return false;}
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
                        catch (Exception exception)  {return false;}
                }
        }
}
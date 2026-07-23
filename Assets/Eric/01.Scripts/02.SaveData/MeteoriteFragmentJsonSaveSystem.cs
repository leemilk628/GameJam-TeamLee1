using System;
using System.IO;
using UnityEngine;

namespace Eric.Save
{
        public static class MeteoriteFragmentJsonSaveSystem
        {
                private const string FileName =
                        "MeteoriteFragmentSave.json";

                private const string LegacyGoldFileName =
                        "GoldSave.json";

                public static string SavePath =>
                        Path.Combine(
                                Application.persistentDataPath,
                                FileName
                        );

                private static string LegacyGoldSavePath =>
                        Path.Combine(
                                Application.persistentDataPath,
                                LegacyGoldFileName
                        );

                public static void Save(
                        MeteoriteFragmentSaveData saveData
                )
                {
                        if (saveData == null)
                                return;

                        try
                        {
                                Directory.CreateDirectory(
                                        Application.persistentDataPath
                                );

                                string json =
                                        JsonUtility.ToJson(
                                                saveData,
                                                true
                                        );

                                File.WriteAllText(
                                        SavePath,
                                        json
                                );
                        }
                        catch (Exception)
                        {
                        }
                }

                public static bool TryLoad(
                        out MeteoriteFragmentSaveData saveData
                )
                {
                        saveData = null;

                        if (!File.Exists(SavePath))
                                return false;

                        try
                        {
                                string json =
                                        File.ReadAllText(SavePath);

                                if (string.IsNullOrWhiteSpace(json))
                                        return false;

                                saveData =
                                        JsonUtility.FromJson
                                                <MeteoriteFragmentSaveData>(
                                                        json
                                                );

                                return saveData != null;
                        }
                        catch (Exception)
                        {
                                saveData = null;
                                return false;
                        }
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
                                return !File.Exists(SavePath);
                        }
                        catch (Exception)
                        {
                                return false;
                        }
                }

                public static void DeleteLegacyGoldSave()
                {
                        if (!File.Exists(LegacyGoldSavePath))
                                return;

                        try
                        {
                                File.Delete(LegacyGoldSavePath);
                        }
                        catch (Exception)
                        {
                        }
                }
        }
}
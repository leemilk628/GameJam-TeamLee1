using System;
using System.Collections.Generic;
using Eric.ModuleSystem;
using Eric.Save;
using Eric.ScriptableScripts;
using UnityEngine;

namespace Eric.Satellite
{
        public class SatelliteUnlockModule : MonoBehaviour, IModule, ISaveable
        {
                private ModuleOwner Owner{get;set;}
                private readonly HashSet<SatelliteType> _unlockedSatellites = new();

                public event Action OnSatelliteUnlockChanged;

                public void Init(ModuleOwner owner)
                {
                        Owner = owner;
                        OnLoad();
                }

                public void AfterInit()
                {
                }

                public bool Unlock(SatelliteType satelliteType)
                {
                        if (satelliteType == SatelliteType.None)
                                return false;

                        if (!_unlockedSatellites.Add(satelliteType))
                                return false;

                        OnSave();
                        OnSatelliteUnlockChanged?.Invoke();
                        return true;
                }

                public bool Lock(SatelliteType satelliteType)
                {
                        if (satelliteType == SatelliteType.None)
                                return false;

                        if (!_unlockedSatellites.Remove(satelliteType))
                                return false;

                        OnSave();
                        OnSatelliteUnlockChanged?.Invoke();
                        return true;
                }

                public bool IsUnlocked(SatelliteType satelliteType)
                {
                        if (satelliteType == SatelliteType.None)
                                return false;

                        return _unlockedSatellites.Contains(satelliteType);
                }

                public void RebuildUnlockData(IEnumerable<SkillTreeSO> skillTrees)
                {
                        _unlockedSatellites.Clear();

                        if (skillTrees != null)
                        {
                                foreach (SkillTreeSO skillTree in skillTrees)
                                {
                                        if (skillTree == null)
                                                continue;

                                        if (!skillTree.IsUpgrade)
                                                continue;

                                        if (skillTree.SkillTreeType != SkillTreeType.Satellite)
                                                continue;

                                        if (skillTree.SatelliteType == SatelliteType.None)
                                                continue;

                                        _unlockedSatellites.Add(
                                                skillTree.SatelliteType
                                        );
                                }
                        }

                        OnSave();
                        OnSatelliteUnlockChanged?.Invoke();
                }

                public void ResetUnlockData()
                {
                        _unlockedSatellites.Clear();
                        SatelliteJsonSaveSystem.DeleteSave();
                        OnSatelliteUnlockChanged?.Invoke();
                }

                public void OnSave()
                {
                        SatelliteSaveData saveData = new();

                        foreach (SatelliteType satelliteType in _unlockedSatellites)
                        {
                                if (satelliteType == SatelliteType.None)
                                        continue;

                                saveData.unlockedSatellites.Add(
                                        satelliteType.ToString()
                                );
                        }

                        SatelliteJsonSaveSystem.Save(saveData);
                }

                public void OnLoad()
                {
                        _unlockedSatellites.Clear();

                        if (!SatelliteJsonSaveSystem.TryLoad(
                                    out SatelliteSaveData saveData
                            ))
                        {
                                SatelliteJsonSaveSystem.DeleteSave();
                                OnSatelliteUnlockChanged?.Invoke();
                                return;
                        }

                        foreach (string satelliteName in saveData.unlockedSatellites)
                        {
                                if (string.IsNullOrWhiteSpace(satelliteName))
                                        continue;

                                if (!Enum.TryParse(
                                            satelliteName,
                                            out SatelliteType satelliteType
                                    ))
                                {
                                        continue;
                                }

                                if (satelliteType == SatelliteType.None)
                                        continue;

                                _unlockedSatellites.Add(satelliteType);
                        }

                        OnSatelliteUnlockChanged?.Invoke();
                }
        }
}
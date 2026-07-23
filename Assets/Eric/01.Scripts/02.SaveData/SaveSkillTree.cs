using System.Collections.Generic;
using Eric.Currency;
using Eric.ModuleSystem;
using Eric.Satellite;
using Eric.ScriptableScripts;
using Eric.StageUpgrade;
using Eric.Upgrade;
using UnityEngine;

namespace Eric.Save
{
        public class SaveSkillTree : MonoBehaviour, IModule, ISaveable
        {
                private ModuleOwner Owner{get;set;}
                private SkillTreeUpgradeModule _skillTreeUpgradeModule;
                private SatelliteUnlockModule _satelliteUnlockModule;
                private GoldModule _goldModule;

                [field:SerializeField] public SkillTreeSO[] SkillTree{get;private set;}

                public Dictionary<string, bool> SkillTreeIsUpgraded{get;private set;} = new();

                public void Init(ModuleOwner owner)
                {
                        Owner = owner;
                }

                public void AfterInit()
                {
                        _skillTreeUpgradeModule = Owner.GetModule<SkillTreeUpgradeModule>();
                        _satelliteUnlockModule = Owner.GetModule<SatelliteUnlockModule>();
                        _goldModule = Owner.GetModule<GoldModule>();
                        
                        OnLoad();
                }

                public void OnSave()
                {
                        if (SkillTree == null) return;

                        SkillTreeSaveData saveData = new();

                        SkillTreeIsUpgraded.Clear();

                        foreach (SkillTreeSO skillTree in SkillTree)
                        {
                                if (skillTree == null)
                                        continue;

                                if (string.IsNullOrWhiteSpace(skillTree.NodeId)) continue;

                                SkillTreeIsUpgraded[skillTree.NodeId] = skillTree.IsUpgrade;

                                SkillTreeSaveEntry saveEntry = new()
                                {
                                        nodeId = skillTree.NodeId,
                                        isUpgrade = skillTree.IsUpgrade
                                };

                                saveData.skillTreeSaveEntries.Add(saveEntry);
                        }

                        SkillTreeJsonSaveSystem.Save(saveData);
                }

                public void OnLoad()
                {
                        SkillTreeIsUpgraded.Clear();

                        if (!SkillTreeJsonSaveSystem.TryLoad(out SkillTreeSaveData saveData))
                        {
                                ResetAllSkillTreeState();

                                if (_skillTreeUpgradeModule != null)
                                        _skillTreeUpgradeModule.RebuildMultiply(SkillTree);

                                return;
                        }

                        LoadSaveData(saveData);

                        if (!SatelliteJsonSaveSystem.HasSave())
                        {
                                ResetSatelliteSkillTreeState();
                                OnSave();
                        }

                        if (_skillTreeUpgradeModule != null)
                                _skillTreeUpgradeModule.RebuildMultiply(SkillTree);
                }

                public bool IsSatelliteUnlocked(SatelliteType satelliteType)
                {
                        if (_satelliteUnlockModule == null)
                                return false;

                        return _satelliteUnlockModule.IsUnlocked(satelliteType);
                }

                public void ResetSaveData()
                {
                        ResetAllSkillTreeState();

                        if (_satelliteUnlockModule != null)
                                _satelliteUnlockModule.ResetUnlockData();
                        else
                                SatelliteJsonSaveSystem.DeleteSave();

                        if (_skillTreeUpgradeModule != null)
                                _skillTreeUpgradeModule.RebuildMultiply(SkillTree);

                        if (_goldModule != null)
                                _goldModule.DeleteGoldSave();

                        ResetStageData();

                        bool isDeleted = SkillTreeJsonSaveSystem.DeleteSave();

                        if (!isDeleted) OnSave();
                }

                private void LoadSaveData(SkillTreeSaveData saveData)
                {
                        if (SkillTree == null) return;

                        if (saveData == null || saveData.skillTreeSaveEntries == null)
                        {
                                ResetAllSkillTreeState();
                                return;
                        }

                        foreach (SkillTreeSaveEntry saveEntry in saveData.skillTreeSaveEntries)
                        {
                                if (saveEntry == null)
                                        continue;

                                if (string.IsNullOrWhiteSpace(saveEntry.nodeId))
                                        continue;

                                SkillTreeIsUpgraded[saveEntry.nodeId] = saveEntry.isUpgrade;
                        }

                        foreach (SkillTreeSO skillTree in SkillTree)
                        {
                                if (skillTree == null)
                                        continue;

                                if (string.IsNullOrWhiteSpace(skillTree.NodeId))
                                {
                                        skillTree.SetUpgradeState(false);
                                        continue;
                                }

                                if (SkillTreeIsUpgraded.TryGetValue(skillTree.NodeId, out bool isUpgrade))
                                {
                                        skillTree.SetUpgradeState(isUpgrade);
                                }
                                else
                                {
                                        skillTree.SetUpgradeState(false);
                                        SkillTreeIsUpgraded[skillTree.NodeId] = false;
                                }
                        }
                }

                private void ResetAllSkillTreeState()
                {
                        SkillTreeIsUpgraded.Clear();

                        if (SkillTree == null) return;

                        foreach (SkillTreeSO skillTree in SkillTree)
                        {
                                if (skillTree == null)
                                        continue;

                                skillTree.SetUpgradeState(false);

                                if (!string.IsNullOrWhiteSpace(skillTree.NodeId))
                                        SkillTreeIsUpgraded[skillTree.NodeId] = false;
                        }
                }

                private void ResetSatelliteSkillTreeState()
                {
                        if (SkillTree == null) return;

                        foreach (SkillTreeSO skillTree in SkillTree)
                        {
                                if (skillTree == null)
                                        continue;

                                if (skillTree.SkillTreeType != SkillTreeType.Satellite)
                                        continue;

                                skillTree.SetUpgradeState(false);

                                if (!string.IsNullOrWhiteSpace(skillTree.NodeId))
                                        SkillTreeIsUpgraded[skillTree.NodeId] = false;
                        }

                        if (_satelliteUnlockModule != null)
                                _satelliteUnlockModule.ResetUnlockData();
                }

                private void ResetStageData()
                {
                        if (StageModuleOwner.Instance == null)
                                return;

                        StageUpgradeModule stageUpgradeModule =
                                StageModuleOwner.Instance.GetModule<StageUpgradeModule>();

                        if (stageUpgradeModule != null)
                        {
                                stageUpgradeModule.ResetStageData();
                                return;
                        }

                        MeteoriteFragmentModule meteoriteFragmentModule =
                                StageModuleOwner.Instance.GetModule<MeteoriteFragmentModule>();

                        if (meteoriteFragmentModule != null)
                                meteoriteFragmentModule.ResetMeteoriteFragment();
                }
        }
}
using System;
using System.Collections.Generic;
using Eric.Currency;
using Eric.ModuleSystem;
using Eric.Satellite;
using Eric.Save;
using Eric.ScriptableScripts;
using UnityEngine;

namespace Eric.Upgrade
{
        public class SkillTreeUpgradeModule : MonoBehaviour, IModule
        {
                private ModuleOwner Owner{get;set;}
                private SaveSkillTree _saveSkillTree;
                private SatelliteUnlockModule _satelliteUnlockModule;
                private GoldModule _goldModule;

                private readonly Dictionary<SkillTreeType, float> _currentMultiply = new();

                public event Action OnMultiplyChanged;

                public void Init(ModuleOwner owner)
                {
                        Owner = owner;
                        ResetMultiply();
                }

                public void AfterInit()
                {
                        _saveSkillTree = Owner.GetModule<SaveSkillTree>();
                        _satelliteUnlockModule = Owner.GetModule<SatelliteUnlockModule>();
                        _goldModule = Owner.GetModule<GoldModule>();
                }

                public bool TryUpgrade(SkillTreeSO skillTree)
                {
                        if (skillTree == null) return false;

                        if (_saveSkillTree == null) return false;

                        if (_goldModule == null)return false;

                        if (skillTree.IsUpgrade)return false;

                        if (skillTree.BeforeNode != null && !skillTree.BeforeNode.IsUpgrade) return false;

                        if (skillTree.SkillTreeType == SkillTreeType.Satellite)
                        {
                                if (_satelliteUnlockModule == null) return false;
                                if (skillTree.SatelliteType == SatelliteType.None) return false;
                        }

                        if (!_goldModule.TrySpendGold(skillTree.NeedGold)) return false;

                        skillTree.Upgrade();

                        if (skillTree.SkillTreeType == SkillTreeType.Satellite) _satelliteUnlockModule.Unlock(skillTree.SatelliteType);
                        else
                        {
                                ApplyMultiply(skillTree);
                                OnMultiplyChanged?.Invoke();
                        }

                        _saveSkillTree.OnSave();

                        return true;
                }

                public bool CanUpgrade(SkillTreeSO skillTree)
                {
                        if (skillTree == null || _goldModule == null||skillTree.IsUpgrade||skillTree.BeforeNode != null && !skillTree.BeforeNode.IsUpgrade)
                                return false;

                        if (skillTree.SkillTreeType == SkillTreeType.Satellite)
                        {
                                if (_satelliteUnlockModule == null||skillTree.SatelliteType == SatelliteType.None)
                                        return false;
                        }

                        return _goldModule.HasGold(skillTree.NeedGold);
                }

                public void Raise()
                {
                        OnMultiplyChanged?.Invoke();
                }

                public void RebuildMultiply(IEnumerable<SkillTreeSO> skillTrees)
                {
                        ResetMultiply();

                        if (skillTrees != null)
                        {
                                foreach (SkillTreeSO skillTree in skillTrees)
                                {
                                        OnMultiplyChanged?.Invoke();
                                        
                                        if (skillTree == null||!skillTree.IsUpgrade||skillTree.SkillTreeType == SkillTreeType.Satellite)
                                                continue;

                                        ApplyMultiply(skillTree);
                                }
                        }

                        OnMultiplyChanged?.Invoke();
                }

                public float GetMultiply(SkillTreeType skillTreeType)
                {
                        if (_currentMultiply.TryGetValue(skillTreeType, out float multiply))
                                return multiply;

                        return 1f;
                }

                private void ApplyMultiply(SkillTreeSO skillTree)
                {
                        SkillTreeType skillTreeType = skillTree.SkillTreeType;

                        if (!_currentMultiply.TryGetValue(skillTreeType, out float currentMultiply))
                                currentMultiply = 1f;

                        _currentMultiply[skillTreeType] = currentMultiply * skillTree.Multiply;
                }

                private void ResetMultiply()
                {
                        _currentMultiply.Clear();

                        foreach (SkillTreeType skillTreeType in Enum.GetValues(typeof(SkillTreeType)))
                        {
                                _currentMultiply[skillTreeType] = 1f;
                        }
                }
        }
}
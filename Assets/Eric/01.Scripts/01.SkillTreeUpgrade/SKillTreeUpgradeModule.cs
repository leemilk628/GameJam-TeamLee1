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
                private MeteoriteFragmentModule _meteoriteFragmentModule;
                private readonly Dictionary<SkillTreeType, int> _currentIncreaseValues = new();

                public event Action OnMultiplyChanged;

                public void Init(ModuleOwner owner)
                {
                        Owner = owner;
                        ResetUpgradeValues();
                }

                public void AfterInit()
                {
                        _saveSkillTree = Owner.GetModule<SaveSkillTree>();
                        _satelliteUnlockModule = Owner.GetModule<SatelliteUnlockModule>();
                        _meteoriteFragmentModule = Owner.GetModule<MeteoriteFragmentModule>();
                }

                public bool TryUpgrade(SkillTreeSO skillTree)
                {
                        if (!CanUpgrade(skillTree))
                                return false;

                        if (!_meteoriteFragmentModule.TrySpendMeteoriteFragment(skillTree.NeedMeteoriteFragment))
                                return false;

                        skillTree.Upgrade();

                        if (skillTree.SkillTreeType == SkillTreeType.Satellite)
                                _satelliteUnlockModule.Unlock(skillTree.SatelliteType);
                        else
                                ApplyUpgradeValue(skillTree);

                        OnMultiplyChanged?.Invoke();
                        _saveSkillTree.OnSave();

                        return true;
                }

                public bool CanUpgrade(SkillTreeSO skillTree)
                {
                        if (skillTree == null || _saveSkillTree == null || _meteoriteFragmentModule == null)
                                return false;

                        if (skillTree.IsUpgrade)
                                return false;

                        if (skillTree.BeforeNode != null && !skillTree.BeforeNode.IsUpgrade)
                                return false;

                        if (skillTree.SkillTreeType == SkillTreeType.Satellite)
                        {
                                if (_satelliteUnlockModule == null)
                                        return false;

                                if (skillTree.SatelliteType == SatelliteType.None)
                                        return false;
                        }

                        return _meteoriteFragmentModule.HasMeteoriteFragment(skillTree.NeedMeteoriteFragment);
                }

                public void Raise()
                {
                        OnMultiplyChanged?.Invoke();
                }

                public void RebuildMultiply(IEnumerable<SkillTreeSO> skillTrees)
                {
                        ResetUpgradeValues();

                        if (skillTrees != null)
                        {
                                foreach (SkillTreeSO skillTree in skillTrees)
                                {
                                        if (skillTree == null || !skillTree.IsUpgrade)
                                                continue;

                                        if (skillTree.SkillTreeType == SkillTreeType.Satellite)
                                                continue;

                                        ApplyUpgradeValue(skillTree);
                                }
                        }

                        OnMultiplyChanged?.Invoke();
                }

                public int GetAddValue(SkillTreeType skillTreeType)
                {
                        if (IsPercentType(skillTreeType))
                                return 0;

                        return GetIncreaseValue(skillTreeType);
                }

                public int GetPercentIncrease(SkillTreeType skillTreeType)
                {
                        if (!IsPercentType(skillTreeType))
                                return 0;

                        return GetIncreaseValue(skillTreeType);
                }

                public float GetMultiply(SkillTreeType skillTreeType)
                {
                        return 1f + GetPercentIncrease(skillTreeType) / 100f;
                }

                private int GetIncreaseValue(SkillTreeType skillTreeType)
                {
                        if (_currentIncreaseValues.TryGetValue(skillTreeType, out int increaseValue))
                                return increaseValue;

                        return 0;
                }

                private void ApplyUpgradeValue(SkillTreeSO skillTree)
                {
                        SkillTreeType skillTreeType = skillTree.SkillTreeType;

                        if (!_currentIncreaseValues.ContainsKey(skillTreeType))
                                _currentIncreaseValues[skillTreeType] = 0;

                        _currentIncreaseValues[skillTreeType] += skillTree.IncreaseValue;
                }

                private bool IsPercentType(SkillTreeType skillTreeType)
                {
                        return skillTreeType == SkillTreeType.GetGold ||
                               skillTreeType == SkillTreeType.GetMeteoriteFragment;
                }

                private void ResetUpgradeValues()
                {
                        _currentIncreaseValues.Clear();

                        foreach (SkillTreeType skillTreeType in Enum.GetValues(typeof(SkillTreeType)))
                                _currentIncreaseValues[skillTreeType] = 0;
                }
        }
}
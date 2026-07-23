using System;
using System.Collections.Generic;
using Eric.Currency;
using Eric.ModuleSystem;
using Eric.ScriptableScripts;
using Eric.Upgrade;
using UnityEngine;

namespace Eric.StageUpgrade
{
        public class StageUpgradeModule : MonoBehaviour, IModule
        {
                private ModuleOwner Owner{get;set;}
                private MeteoriteFragmentModule _meteoriteFragmentModule;
                private SkillTreeUpgradeModule _skillTreeUpgradeModule;

                private readonly Dictionary<StageUpgradeType, int> _upgradeLevels = new();
                private readonly Dictionary<StageUpgradeType, StageUpgradeSO> _upgradeData = new();

                [field:SerializeField] public StageUpgradeSO[] StageUpgrades{get;private set;}

                public int CurrentMeteoriteFragment =>
                        _meteoriteFragmentModule != null
                                ? _meteoriteFragmentModule.CurrentMeteoriteFragment
                                : 0;

                public event Action OnStageUpgradeDataChanged;

                public void Init(ModuleOwner owner)
                {
                        Owner = owner;
                        InitializeUpgradeData();
                        ResetUpgradeLevels();
                }

                public void AfterInit()
                {
                        _meteoriteFragmentModule = Owner.GetModule<MeteoriteFragmentModule>();

                        if (GameModuleOwner.Instance != null)
                                _skillTreeUpgradeModule = GameModuleOwner.Instance.GetModule<SkillTreeUpgradeModule>();
                }

                public bool TryUpgrade(StageUpgradeSO stageUpgrade)
                {
                        if (stageUpgrade == null||_meteoriteFragmentModule == null) return false;
                        int currentLevel = GetLevel(stageUpgrade.StageUpgradeType);
                        if (currentLevel >= stageUpgrade.MaxLevel) return false;
                        int needMF = GetNeedMF(stageUpgrade);

                        if (!_meteoriteFragmentModule.TrySpendMeteoriteFragment(needMF)) return false;

                        _upgradeLevels[stageUpgrade.StageUpgradeType] = currentLevel + 1;
                        OnStageUpgradeDataChanged?.Invoke();

                        return true;
                }

                public bool CanUpgrade(StageUpgradeSO stageUpgrade)
                {
                        if (stageUpgrade == null || _meteoriteFragmentModule == null)
                                return false;

                        int currentLevel = GetLevel(stageUpgrade.StageUpgradeType);

                        if (currentLevel >= stageUpgrade.MaxLevel)
                                return false;

                        return _meteoriteFragmentModule.HasMeteoriteFragment(GetNeedMF(stageUpgrade));
                }

                public int GetLevel(StageUpgradeType stageUpgradeType)
                {
                        if (_upgradeLevels.TryGetValue(stageUpgradeType, out int level))
                                return level;

                        return 0;
                }

                public int GetNeedMF(StageUpgradeSO stageUpgrade)
                {
                        if (stageUpgrade == null)
                                return 0;

                        int currentLevel = GetLevel(stageUpgrade.StageUpgradeType);

                        return Mathf.RoundToInt(
                                stageUpgrade.BaseNeedMF *
                                Mathf.Pow(stageUpgrade.NeedMFMultiply, currentLevel)
                        );
                }

                public float GetStageMultiply(StageUpgradeType stageUpgradeType)
                {
                        if (!_upgradeData.TryGetValue(stageUpgradeType, out StageUpgradeSO stageUpgrade))
                                return 1f;

                        return Mathf.Pow(stageUpgrade.MultiplyPerLevel, GetLevel(stageUpgradeType));
                }

                public float GetPermanentMultiply(StageUpgradeType stageUpgradeType)
                {
                        if (_skillTreeUpgradeModule == null)
                                return 1f;

                        return _skillTreeUpgradeModule.GetMultiply(ConvertSkillTreeType(stageUpgradeType));
                }

                public float GetTotalMultiply(StageUpgradeType stageUpgradeType)
                {
                        return GetPermanentMultiply(stageUpgradeType) * GetStageMultiply(stageUpgradeType);
                }

                public float GetCurrentStat(StageUpgradeSO stageUpgrade)
                {
                        if (stageUpgrade == null)
                                return 0f;

                        return stageUpgrade.BaseStat * GetTotalMultiply(stageUpgrade.StageUpgradeType);
                }

                public float GetAfterUpgradeStat(StageUpgradeSO stageUpgrade)
                {
                        if (stageUpgrade == null)
                                return 0f;

                        int currentLevel = GetLevel(stageUpgrade.StageUpgradeType);

                        if (currentLevel >= stageUpgrade.MaxLevel)
                                return GetCurrentStat(stageUpgrade);

                        float afterStageMultiply = Mathf.Pow(
                                stageUpgrade.MultiplyPerLevel,
                                currentLevel + 1
                        );

                        return stageUpgrade.BaseStat *
                               GetPermanentMultiply(stageUpgrade.StageUpgradeType) *
                               afterStageMultiply;
                }

                public float ApplyStat(StageUpgradeType stageUpgradeType, float baseValue)
                {
                        return baseValue * GetTotalMultiply(stageUpgradeType);
                }

                public void AddMeteoriteFragment(int baseAmount)
                {
                        if (baseAmount <= 0 || _meteoriteFragmentModule == null)
                                return;

                        int finalAmount = Mathf.RoundToInt(
                                baseAmount *
                                GetTotalMultiply(StageUpgradeType.MeteoriteFragment)
                        );

                        _meteoriteFragmentModule.AddMeteoriteFragment(finalAmount);
                }

                public void ResetStageData()
                {
                        ResetUpgradeLevels();

                        if (_meteoriteFragmentModule != null)
                                _meteoriteFragmentModule.ResetMeteoriteFragment();

                        OnStageUpgradeDataChanged?.Invoke();
                }

                private void ResetUpgradeLevels()
                {
                        _upgradeLevels.Clear();

                        foreach (StageUpgradeType stageUpgradeType in Enum.GetValues(typeof(StageUpgradeType)))
                                _upgradeLevels[stageUpgradeType] = 0;
                }

                private void InitializeUpgradeData()
                {
                        _upgradeData.Clear();

                        if (StageUpgrades == null)
                                return;

                        foreach (StageUpgradeSO stageUpgrade in StageUpgrades)
                        {
                                if (stageUpgrade == null)
                                        continue;

                                StageUpgradeType stageUpgradeType = stageUpgrade.StageUpgradeType;

                                if (_upgradeData.ContainsKey(stageUpgradeType)) continue;

                                _upgradeData.Add(stageUpgradeType, stageUpgrade);
                        }
                }

                private SkillTreeType ConvertSkillTreeType(StageUpgradeType stageUpgradeType)
                {
                        return stageUpgradeType switch
                        {
                                StageUpgradeType.Health => SkillTreeType.PlayerHealth,
                                StageUpgradeType.Attack => SkillTreeType.PlayerAttack,
                                StageUpgradeType.MeteoriteFragment => SkillTreeType.GetMeteoriteFragment,
                                StageUpgradeType.SatelliteAttackSpeed => SkillTreeType.SatelliteAttackSpeed,
                                _ => throw new ArgumentOutOfRangeException(
                                        nameof(stageUpgradeType),
                                        stageUpgradeType,
                                        null
                                )
                        };
                }
        }
}
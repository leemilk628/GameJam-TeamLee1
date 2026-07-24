using System;
using System.Collections.Generic;
using Eric.Currency;
using Eric.ModuleSystem;
using Eric.ScriptableScripts;
using Eric.Upgrade;
using Key.Scripts.Singletone;
using UnityEngine;

namespace Eric.StageUpgrade
{
        public class StageUpgradeModule : MonoBehaviour, IModule
        {
                private ModuleOwner Owner{get;set;}
                private GoldModule _goldModule;
                private SkillTreeUpgradeModule _skillTreeUpgradeModule;
                private readonly Dictionary<StageUpgradeType, int> _upgradeLevels = new();
                private readonly Dictionary<StageUpgradeType, StageUpgradeSO> _upgradeData = new();

                [field:SerializeField] public StageUpgradeSO[] StageUpgrades{get;private set;}

                public int CurrentGold => _goldModule != null ? _goldModule.CurrentGold : 0;

                public event Action OnStageUpgradeDataChanged;
                public event Action<int> OnPlayerHealthRecoveryRequested;

                public void Init(ModuleOwner owner)
                {
                        Owner = owner;
                        InitializeUpgradeData();
                        ResetUpgradeLevels();
                }

                public void AfterInit()
                {
                        _goldModule = Owner.GetModule<GoldModule>();

                        if (GameModuleOwner.Instance != null)
                                _skillTreeUpgradeModule = GameModuleOwner.Instance.GetModule<SkillTreeUpgradeModule>();

                        if (_skillTreeUpgradeModule != null)
                                _skillTreeUpgradeModule.OnMultiplyChanged += LobbyUpgradeChanged;

                        OnStageUpgradeDataChanged?.Invoke();
                }

                private void OnDestroy()
                {
                        if (_skillTreeUpgradeModule != null)
                                _skillTreeUpgradeModule.OnMultiplyChanged -= LobbyUpgradeChanged;
                }

                public bool TryUpgrade(StageUpgradeSO stageUpgrade)
                {
                        if (!CanUpgrade(stageUpgrade))
                        {
                                SoundManager.Instance.PlaySFX(SoundType.NotEnoughMoney);
                                return false;
                        }

                        int needGold = GetNeedGold(stageUpgrade);

                        if (!_goldModule.TrySpendGold(needGold))
                        {
                                SoundManager.Instance.PlaySFX(SoundType.NotEnoughMoney);
                                return false;
                        }

                        StageUpgradeType stageUpgradeType = stageUpgrade.StageUpgradeType;
                        _upgradeLevels[stageUpgradeType] = GetLevel(stageUpgradeType) + 1;

                        if (stageUpgradeType == StageUpgradeType.HealthRecovery)
                                OnPlayerHealthRecoveryRequested?.Invoke(GetCurrentStat(stageUpgrade));

                        OnStageUpgradeDataChanged?.Invoke();
                        SoundManager.Instance.PlaySFX(SoundType.Upgrade);
                        return true;
                }

                public bool CanUpgrade(StageUpgradeSO stageUpgrade)
                {
                        if (stageUpgrade == null || _goldModule == null)
                                return false;

                        if (GetLevel(stageUpgrade.StageUpgradeType) >= stageUpgrade.MaxLevel)
                                return false;

                        return _goldModule.HasGold(GetNeedGold(stageUpgrade));
                }

                public int GetLevel(StageUpgradeType stageUpgradeType)
                {
                        return _upgradeLevels.TryGetValue(stageUpgradeType, out int level) ? level : 0;
                }

                public int GetNeedGold(StageUpgradeSO stageUpgrade)
                {
                        if (stageUpgrade == null)
                                return 0;

                        int currentLevel = GetLevel(stageUpgrade.StageUpgradeType);

                        return Mathf.Max(
                                0,
                                Mathf.RoundToInt(
                                        stageUpgrade.BaseNeedGold *
                                        Mathf.Pow(stageUpgrade.NeedGoldMultiply, currentLevel)
                                )
                        );
                }

                public int GetCurrentStat(StageUpgradeSO stageUpgrade)
                {
                        if (stageUpgrade == null)
                                return 0;

                        int currentLevel = GetLevel(stageUpgrade.StageUpgradeType);
                        int stageIncrease = stageUpgrade.AddValuePerLevel * currentLevel;

                        if (stageUpgrade.StageUpgradeType == StageUpgradeType.MeteoriteFragment)
                                return GetLobbyMeteoriteFragmentPercentIncrease() + stageIncrease;

                        return stageUpgrade.BaseStat +
                               GetLobbyAddValue(stageUpgrade.StageUpgradeType) +
                               stageIncrease;
                }

                public int GetAfterUpgradeStat(StageUpgradeSO stageUpgrade)
                {
                        if (stageUpgrade == null)
                                return 0;

                        int currentLevel = GetLevel(stageUpgrade.StageUpgradeType);

                        if (currentLevel >= stageUpgrade.MaxLevel)
                                return GetCurrentStat(stageUpgrade);

                        int stageIncrease = stageUpgrade.AddValuePerLevel * (currentLevel + 1);

                        if (stageUpgrade.StageUpgradeType == StageUpgradeType.MeteoriteFragment)
                                return GetLobbyMeteoriteFragmentPercentIncrease() + stageIncrease;

                        return stageUpgrade.BaseStat +
                               GetLobbyAddValue(stageUpgrade.StageUpgradeType) +
                               stageIncrease;
                }

                public int GetPlayerMaxHealth(int baseValue)
                {
                        return Mathf.Max(
                                1,
                                baseValue +
                                GetLobbyAddValue(StageUpgradeType.Health) +
                                GetStageAddValue(StageUpgradeType.Health)
                        );
                }

                public int GetPlayerAttack(int baseValue)
                {
                        return Mathf.Max(
                                0,
                                baseValue +
                                GetLobbyAddValue(StageUpgradeType.Attack) +
                                GetStageAddValue(StageUpgradeType.Attack)
                        );
                }

                public int GetPlayerAttackSpeed(int baseValue)
                {
                        return Mathf.Max(
                                1,
                                baseValue +
                                GetLobbyAddValue(StageUpgradeType.AttackSpeed) +
                                GetStageAddValue(StageUpgradeType.AttackSpeed)
                        );
                }

                public int GetPlayerBarrier(int baseValue)
                {
                        return Mathf.Max(
                                0,
                                baseValue +
                                GetLobbyAddValue(StageUpgradeType.Barrier) +
                                GetStageAddValue(StageUpgradeType.Barrier)
                        );
                }

                public int GetPlayerBarrierRecoverySpeed(int baseValue)
                {
                        int lobbyIncrease = _skillTreeUpgradeModule != null
                                ? _skillTreeUpgradeModule.GetAddValue(SkillTreeType.BarrierRecoverySpeed)
                                : 0;

                        return Mathf.Max(0, baseValue + lobbyIncrease);
                }

                public int GetSatelliteAttack(int baseValue)
                {
                        return Mathf.Max(
                                0,
                                baseValue +
                                GetLobbyAddValue(StageUpgradeType.SatelliteAttack) +
                                GetStageAddValue(StageUpgradeType.SatelliteAttack)
                        );
                }

                public int GetSatelliteAttackSpeed(int baseValue)
                {
                        return Mathf.Max(
                                1,
                                baseValue +
                                GetLobbyAddValue(StageUpgradeType.SatelliteAttackSpeed) +
                                GetStageAddValue(StageUpgradeType.SatelliteAttackSpeed)
                        );
                }

                public int GetMaxSatelliteCount(int baseValue)
                {
                        return Mathf.Max(
                                0,
                                baseValue +
                                GetLobbyAddValue(StageUpgradeType.MaxSatelliteCount) +
                                GetStageAddValue(StageUpgradeType.MaxSatelliteCount)
                        );
                }

                public int GetStageMeteoriteFragmentPercentIncrease()
                {
                        return Mathf.Max(0, GetStageAddValue(StageUpgradeType.MeteoriteFragment));
                }

                public void ResetStageData()
                {
                        ResetUpgradeLevels();

                        if (_goldModule != null)
                                _goldModule.ResetForStage();

                        OnStageUpgradeDataChanged?.Invoke();
                }

                private int GetStageAddValue(StageUpgradeType stageUpgradeType)
                {
                        if (!_upgradeData.TryGetValue(stageUpgradeType, out StageUpgradeSO stageUpgrade))
                                return 0;

                        return stageUpgrade.AddValuePerLevel * GetLevel(stageUpgradeType);
                }

                private int GetLobbyAddValue(StageUpgradeType stageUpgradeType)
                {
                        if (_skillTreeUpgradeModule == null)
                                return 0;

                        return stageUpgradeType switch
                        {
                                StageUpgradeType.Health =>
                                        _skillTreeUpgradeModule.GetAddValue(SkillTreeType.PlayerHealth),

                                StageUpgradeType.Attack =>
                                        _skillTreeUpgradeModule.GetAddValue(SkillTreeType.PlayerAttack),

                                StageUpgradeType.AttackSpeed =>
                                        _skillTreeUpgradeModule.GetAddValue(SkillTreeType.PlayerAttackSpeed),

                                StageUpgradeType.Barrier =>
                                        _skillTreeUpgradeModule.GetAddValue(SkillTreeType.PlayerBarrier),

                                StageUpgradeType.SatelliteAttack =>
                                        _skillTreeUpgradeModule.GetAddValue(SkillTreeType.SatelliteAttack),

                                StageUpgradeType.SatelliteAttackSpeed =>
                                        _skillTreeUpgradeModule.GetAddValue(SkillTreeType.SatelliteAttackSpeed),

                                StageUpgradeType.MaxSatelliteCount =>
                                        _skillTreeUpgradeModule.GetAddValue(SkillTreeType.MaxSatelliteCount),

                                _ => 0
                        };
                }

                private int GetLobbyMeteoriteFragmentPercentIncrease()
                {
                        if (_skillTreeUpgradeModule == null)
                                return 0;

                        return _skillTreeUpgradeModule.GetPercentIncrease(SkillTreeType.GetMeteoriteFragment);
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
                                if (stageUpgrade == null || _upgradeData.ContainsKey(stageUpgrade.StageUpgradeType))
                                        continue;

                                _upgradeData.Add(stageUpgrade.StageUpgradeType, stageUpgrade);
                        }
                }

                private void LobbyUpgradeChanged()
                {
                        OnStageUpgradeDataChanged?.Invoke();
                }
        }
}
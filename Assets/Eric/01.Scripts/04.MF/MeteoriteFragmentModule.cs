using System;
using System.Collections.Generic;
using Eric.ModuleSystem;
using Eric.Save;
using Eric.ScriptableScripts;
using Eric.StageUpgrade;
using Eric.Upgrade;
using UnityEngine;

namespace Eric.Currency
{
        public class MeteoriteFragmentModule : MonoBehaviour, IModule, ISaveable
        {
                private static readonly HashSet<MeteoriteFragmentModule> Instances = new();

                private static int _sharedMeteoriteFragment;
                private static bool _isLoaded;

                private ModuleOwner Owner{get;set;}
                private SkillTreeUpgradeModule _skillTreeUpgradeModule;

                public int CurrentMeteoriteFragment => _sharedMeteoriteFragment;

                public event Action<int> OnMeteoriteFragmentChanged;

                [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
                private static void ResetStaticData()
                {
                        Instances.Clear();
                        _sharedMeteoriteFragment = 0;
                        _isLoaded = false;
                }

                public void Init(ModuleOwner owner)
                {
                        Owner = owner;
                        Instances.Add(this);
                }

                public void AfterInit()
                {
                        _skillTreeUpgradeModule =
                                FindSkillTreeUpgradeModule();

                        EnsureLoaded();
                        NotifyCurrentInstance();
                }

                private void OnDestroy()
                {
                        Instances.Remove(this);
                }

                public void AddMeteoriteFragment(int baseAmount)
                {
                        if (baseAmount <= 0)
                                return;

                        EnsureLoaded();

                        int finalAmount =
                                CalculateMeteoriteFragmentAmount(baseAmount);

                        SetSharedMeteoriteFragment(
                                _sharedMeteoriteFragment + finalAmount,
                                true
                        );
                }

                public bool TrySpendMeteoriteFragment(int amount)
                {
                        if (amount < 0)
                                return false;

                        EnsureLoaded();

                        if (_sharedMeteoriteFragment < amount)
                                return false;

                        if (amount == 0)
                                return true;

                        SetSharedMeteoriteFragment(
                                _sharedMeteoriteFragment - amount,
                                true
                        );

                        return true;
                }

                public bool HasMeteoriteFragment(int amount)
                {
                        if (amount < 0)
                                return false;

                        EnsureLoaded();

                        return _sharedMeteoriteFragment >= amount;
                }

                public void SetMeteoriteFragment(int amount)
                {
                        SetSharedMeteoriteFragment(
                                Mathf.Max(0, amount),
                                true
                        );
                }

                public void ResetMeteoriteFragment()
                {
                        ResetAllMeteoriteFragmentData();
                }

                public void DeleteMeteoriteFragmentSave()
                {
                        ResetAllMeteoriteFragmentData();
                }

                public static void ResetAllMeteoriteFragmentData()
                {
                        _sharedMeteoriteFragment = 0;
                        _isLoaded = true;

                        SaveSharedMeteoriteFragment();
                        MeteoriteFragmentJsonSaveSystem.DeleteLegacyGoldSave();

                        NotifyAllInstances();
                }

                public void OnSave()
                {
                        EnsureLoaded();
                        SaveSharedMeteoriteFragment();
                }

                public void OnLoad()
                {
                        _isLoaded = false;

                        EnsureLoaded();
                        NotifyAllInstances();
                }

                private int CalculateMeteoriteFragmentAmount(int baseAmount)
                {
                        int lobbyPercent = 0;
                        int stagePercent = 0;

                        if (_skillTreeUpgradeModule == null)
                        {
                                _skillTreeUpgradeModule =
                                        FindSkillTreeUpgradeModule();
                        }

                        if (_skillTreeUpgradeModule != null)
                        {
                                lobbyPercent =
                                        _skillTreeUpgradeModule
                                                .GetPercentIncrease(
                                                        SkillTreeType
                                                                .GetMeteoriteFragment
                                                );
                        }

                        if (StageModuleOwner.Instance != null)
                        {
                                StageUpgradeModule stageUpgradeModule =
                                        StageModuleOwner.Instance
                                                .GetModule<StageUpgradeModule>();

                                if (stageUpgradeModule != null)
                                {
                                        stagePercent =
                                                stageUpgradeModule
                                                        .GetStageMeteoriteFragmentPercentIncrease();
                                }
                        }

                        int totalPercent =
                                lobbyPercent + stagePercent;

                        float multiplier =
                                Mathf.Max(
                                        0f,
                                        1f + totalPercent / 100f
                                );

                        return Mathf.Max(
                                1,
                                Mathf.RoundToInt(
                                        baseAmount * multiplier
                                )
                        );
                }

                private SkillTreeUpgradeModule FindSkillTreeUpgradeModule()
                {
                        if (GameModuleOwner.Instance != null)
                        {
                                SkillTreeUpgradeModule gameModule =
                                        GameModuleOwner.Instance
                                                .GetModule<SkillTreeUpgradeModule>();

                                if (gameModule != null)
                                        return gameModule;
                        }

                        if (Owner != null)
                        {
                                return Owner
                                        .GetModule<SkillTreeUpgradeModule>();
                        }

                        return null;
                }

                private static void EnsureLoaded()
                {
                        if (_isLoaded)
                                return;

                        _isLoaded = true;

                        bool hasSave =
                                MeteoriteFragmentJsonSaveSystem.TryLoad(
                                        out MeteoriteFragmentSaveData saveData
                                );

                        if (!hasSave || saveData == null)
                        {
                                _sharedMeteoriteFragment = 0;
                                SaveSharedMeteoriteFragment();
                                return;
                        }

                        _sharedMeteoriteFragment =
                                Mathf.Max(
                                        0,
                                        saveData.meteoriteFragment
                                );
                }

                private static void SetSharedMeteoriteFragment(
                        int amount,
                        bool save
                )
                {
                        _sharedMeteoriteFragment =
                                Mathf.Max(0, amount);

                        _isLoaded = true;

                        if (save)
                                SaveSharedMeteoriteFragment();

                        NotifyAllInstances();
                }

                private static void SaveSharedMeteoriteFragment()
                {
                        MeteoriteFragmentSaveData saveData = new()
                        {
                                meteoriteFragment =
                                        _sharedMeteoriteFragment
                        };

                        MeteoriteFragmentJsonSaveSystem.Save(
                                saveData
                        );
                }

                private static void NotifyAllInstances()
                {
                        List<MeteoriteFragmentModule> modules =
                                new(Instances);

                        foreach (MeteoriteFragmentModule module in modules)
                        {
                                if (module == null)
                                {
                                        Instances.Remove(module);
                                        continue;
                                }

                                module.NotifyCurrentInstance();
                        }
                }

                private void NotifyCurrentInstance()
                {
                        OnMeteoriteFragmentChanged?.Invoke(
                                _sharedMeteoriteFragment
                        );
                }
        }
}
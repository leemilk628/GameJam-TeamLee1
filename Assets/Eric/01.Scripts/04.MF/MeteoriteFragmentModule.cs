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
    public class MeteoriteFragmentModule :
        MonoBehaviour,
        IModule,
        ISaveable
    {
        private static readonly
            HashSet<MeteoriteFragmentModule>
            Instances = new();

        private static int
            _sharedMeteoriteFragment;

        private static bool _isLoaded;

        private ModuleOwner Owner { get; set; }

        private SkillTreeUpgradeModule
            _skillTreeUpgradeModule;

        private SaveManager SaveManager
        {
            get;
            set;
        }

        public int CurrentMeteoriteFragment =>
            _sharedMeteoriteFragment;

        public event Action<int>
            OnMeteoriteFragmentChanged;

        [RuntimeInitializeOnLoadMethod(
            RuntimeInitializeLoadType
                .SubsystemRegistration
        )]
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

            SaveManager =
                Owner.GetModule<SaveManager>();

            SaveManager.RaiseSave(OnSave);
            SaveManager.RaiseLoad(OnLoad);

            EnsureLoaded();
            NotifyCurrentInstance();
        }

        private void OnDestroy()
        {
            Instances.Remove(this);
        }

        public void AddMeteoriteFragment(
            int baseAmount)
        {
            AddCalculatedMeteoriteFragment(
                CalculateFinalMeteoriteFragmentAmount(
                    baseAmount
                )
            );
        }

        public int
            CalculateFinalMeteoriteFragmentAmount(
                int baseAmount)
        {
            if (baseAmount <= 0)
                return 0;

            EnsureLoaded();

            return CalculateMeteoriteFragmentAmount(
                baseAmount
            );
        }

        public void
            AddCalculatedMeteoriteFragment(
                int finalAmount)
        {
            if (finalAmount <= 0)
                return;

            EnsureLoaded();

            SetSharedMeteoriteFragment(
                _sharedMeteoriteFragment +
                finalAmount,
                true
            );
        }

        public bool TrySpendMeteoriteFragment(
            int amount)
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

        public bool HasMeteoriteFragment(
            int amount)
        {
            if (amount < 0)
                return false;

            EnsureLoaded();

            return
                _sharedMeteoriteFragment >= amount;
        }

        public void SetMeteoriteFragment(
            int amount)
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

        public static void
            ResetAllMeteoriteFragmentData()
        {
            _sharedMeteoriteFragment = 0;
            _isLoaded = true;

            SaveSharedMeteoriteFragment();

            MeteoriteFragmentJsonSaveSystem
                .DeleteLegacyGoldSave();

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

        private int
            CalculateMeteoriteFragmentAmount(
                int baseAmount)
        {
            if (_skillTreeUpgradeModule == null)
            {
                _skillTreeUpgradeModule =
                    FindSkillTreeUpgradeModule();
            }

            int lobbyPercent =
                _skillTreeUpgradeModule == null
                    ? 0
                    : _skillTreeUpgradeModule
                        .GetPercentIncrease(
                            SkillTreeType
                                .GetMeteoriteFragment
                        );

            int stagePercent = 0;

            if (StageModuleOwner.Instance != null)
            {
                StageUpgradeModule stageModule =
                    StageModuleOwner.Instance
                        .GetModule<
                            StageUpgradeModule
                        >();

                if (stageModule != null)
                {
                    stagePercent =
                        stageModule
                            .GetStageMeteoriteFragmentPercentIncrease();
                }
            }

            float multiplier = Mathf.Max(
                0f,
                1f +
                (lobbyPercent + stagePercent) /
                100f
            );

            return Mathf.Max(
                1,
                Mathf.RoundToInt(
                    baseAmount * multiplier
                )
            );
        }

        private SkillTreeUpgradeModule
            FindSkillTreeUpgradeModule()
        {
            if (GameModuleOwner.Instance != null)
            {
                SkillTreeUpgradeModule gameModule =
                    GameModuleOwner.Instance
                        .GetModule<
                            SkillTreeUpgradeModule
                        >();

                if (gameModule != null)
                    return gameModule;
            }

            return Owner == null
                ? null
                : Owner.GetModule<
                    SkillTreeUpgradeModule
                >();
        }

        private static void EnsureLoaded()
        {
            if (_isLoaded)
                return;

            _isLoaded = true;

            bool loaded =
                MeteoriteFragmentJsonSaveSystem
                    .TryLoad(
                        out MeteoriteFragmentSaveData
                            saveData
                    );

            if (!loaded || saveData == null)
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

        private static void
            SetSharedMeteoriteFragment(
                int amount,
                bool save)
        {
            _sharedMeteoriteFragment =
                Mathf.Max(0, amount);

            _isLoaded = true;

            if (save)
                SaveSharedMeteoriteFragment();

            NotifyAllInstances();
        }

        private static void
            SaveSharedMeteoriteFragment()
        {
            MeteoriteFragmentJsonSaveSystem.Save(
                new MeteoriteFragmentSaveData
                {
                    meteoriteFragment =
                        _sharedMeteoriteFragment
                }
            );
        }

        private static void NotifyAllInstances()
        {
            List<MeteoriteFragmentModule>
                modules = new(Instances);

            foreach (
                MeteoriteFragmentModule module
                in modules)
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
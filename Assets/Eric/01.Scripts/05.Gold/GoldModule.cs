using System;
using Eric.ModuleSystem;
using Eric.ScriptableScripts;
using Eric.Upgrade;
using UnityEngine;

namespace Eric.Currency
{
    public class GoldModule : MonoBehaviour, IModule
    {
        private ModuleOwner Owner { get; set; }
        [SerializeField]private MeteoriteFragmentModule meteoriteFragmentModule;
        private SkillTreeUpgradeModule _skillTreeUpgradeModule;

        public int CurrentGold { get; private set; }

        public event Action<int> OnGoldChanged;

        public void Init(ModuleOwner owner)
        {
            Owner = owner;
            CurrentGold = 0;
        }

        public void AfterInit()
        {
            if (GameModuleOwner.Instance != null)
            {
                _skillTreeUpgradeModule =
                    GameModuleOwner.Instance
                        .GetModule<SkillTreeUpgradeModule>();
            }

            ResetForStage();
        }

        public void AddGold(int baseAmount)
        {
            AddCalculatedGold(
                CalculateFinalGoldAmount(baseAmount)
            );
        }

        public int CalculateFinalGoldAmount(
            int baseAmount)
        {
            return baseAmount <= 0
                ? 0
                : CalculateGoldAmount(baseAmount);
        }

        public void AddCalculatedGold(
            int finalAmount)
        {
            if (finalAmount <= 0)
                return;

            CurrentGold += finalAmount;
            NotifyChanged();
        }

        public bool TrySpendGold(int amount)
        {
            if (amount < 0 ||
                CurrentGold < amount)
            {
                return false;
            }

            if (amount == 0)
                return true;

            CurrentGold -= amount;
            NotifyChanged();

            return true;
        }

        public bool HasGold(int amount)
        {
            return amount >= 0 &&
                   CurrentGold >= amount;
        }

        public void SetGold(int amount)
        {
            CurrentGold = Mathf.Max(0, amount);
            NotifyChanged();
        }

        public void ResetGold()
        {
            _meteoriteFragmentModule.AddMeteoriteFragment(CurrentGold/10);
            CurrentGold = 0;
            NotifyChanged();
        }

        public void ResetForStage()
        {
            CurrentGold = 0;

            if (_skillTreeUpgradeModule != null)
            {
                CurrentGold = Mathf.Max(
                    0,
                    _skillTreeUpgradeModule
                        .GetAddValue(
                            SkillTreeType.StartingGold
                        )
                );
            }

            NotifyChanged();
        }

        public void DeleteGoldSave()
        {
            ResetGold();
        }

        private int CalculateGoldAmount(
            int baseAmount)
        {
            int percentIncrease =
                _skillTreeUpgradeModule == null
                    ? 0
                    : _skillTreeUpgradeModule
                        .GetPercentIncrease(
                            SkillTreeType.GetGold
                        );

            float multiplier = Mathf.Max(
                0f,
                1f + percentIncrease / 100f
            );

            return Mathf.Max(
                1,
                Mathf.RoundToInt(
                    baseAmount * multiplier
                )
            );
        }

        private void NotifyChanged()
        {
            OnGoldChanged?.Invoke(CurrentGold);
        }
    }
}
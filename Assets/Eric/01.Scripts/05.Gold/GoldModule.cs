using System;
using Eric.ModuleSystem;
using Eric.ScriptableScripts;
using Eric.Upgrade;
using UnityEngine;

namespace Eric.Currency
{
        public class GoldModule : MonoBehaviour, IModule
        {
                private ModuleOwner Owner{get;set;}
                private SkillTreeUpgradeModule _skillTreeUpgradeModule;

                public int CurrentGold{get;private set;}

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
                        if (baseAmount <= 0)
                                return;

                        int finalAmount = CalculateGoldAmount(baseAmount);

                        CurrentGold += finalAmount;
                        NotifyChanged();
                }

                public bool TrySpendGold(int amount)
                {
                        if (amount < 0)
                                return false;

                        if (CurrentGold < amount)
                                return false;

                        if (amount == 0)
                                return true;

                        CurrentGold -= amount;
                        NotifyChanged();

                        return true;
                }

                public bool HasGold(int amount)
                {
                        if (amount < 0)
                                return false;

                        return CurrentGold >= amount;
                }

                public void SetGold(int amount)
                {
                        CurrentGold = Mathf.Max(0, amount);
                        NotifyChanged();
                }

                public void ResetGold()
                {
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
                                        _skillTreeUpgradeModule.GetAddValue(
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

                private int CalculateGoldAmount(int baseAmount)
                {
                        int percentIncrease = 0;

                        if (_skillTreeUpgradeModule != null)
                        {
                                percentIncrease =
                                        _skillTreeUpgradeModule.GetPercentIncrease(
                                                SkillTreeType.GetGold
                                        );
                        }

                        float multiply =
                                Mathf.Max(
                                        0f,
                                        1f + percentIncrease / 100f
                                );

                        return Mathf.Max(
                                1,
                                Mathf.RoundToInt(baseAmount * multiply)
                        );
                }

                private void NotifyChanged()
                {
                        OnGoldChanged?.Invoke(CurrentGold);
                }
        }
}
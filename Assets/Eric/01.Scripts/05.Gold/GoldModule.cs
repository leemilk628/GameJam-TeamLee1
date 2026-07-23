using System;
using Eric.ModuleSystem;
using Eric.Save;
using UnityEngine;

namespace Eric.Currency
{
        public class GoldModule : MonoBehaviour, IModule, ISaveable
        {
                private ModuleOwner Owner{get;set;}

                public int CurrentGold{get;private set;}

                public event Action<int> OnGoldChanged;

                public void Init(ModuleOwner owner)
                {
                        Owner = owner;
                }

                public void AfterInit()
                {
                        OnLoad();
                }

                public void AddGold(int amount)
                {
                        if (amount <= 0)
                                return;

                        CurrentGold += amount;
                        NotifyChanged();
                        OnSave();
                }

                public bool TrySpendGold(int amount)
                {
                        if (amount <= 0)
                                return false;

                        if (CurrentGold < amount)
                                return false;

                        CurrentGold -= amount;
                        NotifyChanged();
                        OnSave();

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
                        OnSave();
                }

                public void ResetGold()
                {
                        CurrentGold = 0;
                        NotifyChanged();
                        OnSave();
                }

                public void DeleteGoldSave()
                {
                        if (!GoldJsonSaveSystem.DeleteSave())
                                return;

                        CurrentGold = 0;
                        NotifyChanged();
                }

                public void OnSave()
                {
                        GoldSaveData saveData = new()
                        {
                                gold = CurrentGold
                        };

                        GoldJsonSaveSystem.Save(saveData);
                }

                public void OnLoad()
                {
                        if (!GoldJsonSaveSystem.TryLoad(out GoldSaveData saveData))
                        {
                                CurrentGold = 0;
                                NotifyChanged();
                                return;
                        }

                        CurrentGold = Mathf.Max(0, saveData.gold);
                        NotifyChanged();
                }

                private void NotifyChanged()
                {
                        OnGoldChanged?.Invoke(CurrentGold);
                }
        }
}
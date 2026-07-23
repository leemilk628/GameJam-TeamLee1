using System;
using Eric.ModuleSystem;
using UnityEngine;

namespace Eric.Currency
{
        public class MeteoriteFragmentModule : MonoBehaviour, IModule
        {
                private ModuleOwner Owner{get;set;}

                public int CurrentMeteoriteFragment{get;private set;}

                public event Action<int> OnMeteoriteFragmentChanged;

                public void Init(ModuleOwner owner)
                {
                        Owner = owner;
                        CurrentMeteoriteFragment = 0;
                }

                public void AfterInit()
                {
                        NotifyChanged();
                }

                public void AddMeteoriteFragment(int amount)
                {
                        if (amount <= 0)
                                return;

                        CurrentMeteoriteFragment += amount;
                        NotifyChanged();
                }

                public bool TrySpendMeteoriteFragment(int amount)
                {
                        if (amount <= 0)
                                return false;

                        if (CurrentMeteoriteFragment < amount)
                                return false;

                        CurrentMeteoriteFragment -= amount;
                        NotifyChanged();

                        return true;
                }

                public bool HasMeteoriteFragment(int amount)
                {
                        if (amount < 0)
                                return false;

                        return CurrentMeteoriteFragment >= amount;
                }

                public void SetMeteoriteFragment(int amount)
                {
                        CurrentMeteoriteFragment = Mathf.Max(0, amount);
                        NotifyChanged();
                }

                public void ResetMeteoriteFragment()
                {
                        CurrentMeteoriteFragment = 0;
                        NotifyChanged();
                }

                private void NotifyChanged()
                {
                        OnMeteoriteFragmentChanged?.Invoke(CurrentMeteoriteFragment);
                }
        }
}
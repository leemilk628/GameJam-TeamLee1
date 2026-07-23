using Eric.ModuleSystem;
using Eric.StageUpgrade;
using TMPro;
using UnityEngine;

namespace Eric.Currency
{
        public class CurrencyUI : MonoBehaviour, IModule
        {
                private ModuleOwner Owner{get;set;}
                private GoldModule _goldModule;
                private MeteoriteFragmentModule _meteoriteFragmentModule;

                [field:SerializeField] public TextMeshProUGUI GoldText{get;private set;}
                [field:SerializeField] public TextMeshProUGUI MeteoriteFragmentText{get;private set;}

                public void Init(ModuleOwner owner)
                {
                        Owner = owner;
                }

                public void AfterInit()
                {
                        FindModules();
                        SubscribeEvents();
                        UIUpdate();
                }

                private void OnDestroy()
                {
                        UnsubscribeEvents();
                }

                public void UIUpdate()
                {
                        FindModules();

                        if (_goldModule != null)
                        {
                                UpdateGoldUI(
                                        _goldModule.CurrentGold
                                );
                        }

                        if (_meteoriteFragmentModule != null)
                        {
                                UpdateMeteoriteFragmentUI(
                                        _meteoriteFragmentModule
                                                .CurrentMeteoriteFragment
                                );
                        }
                        else if (MeteoriteFragmentText != null)
                        {
                                MeteoriteFragmentText.text = "0";
                        }
                }

                private void FindModules()
                {
                        if (_goldModule == null)
                        {
                                if (StageModuleOwner.Instance != null)
                                {
                                        _goldModule =
                                                StageModuleOwner.Instance
                                                        .GetModule<GoldModule>();
                                }

                                if (_goldModule == null && Owner != null)
                                {
                                        _goldModule =
                                                Owner.GetModule<GoldModule>();
                                }
                        }

                        if (_meteoriteFragmentModule == null)
                        {
                                if (GameModuleOwner.Instance != null)
                                {
                                        _meteoriteFragmentModule =
                                                GameModuleOwner.Instance
                                                        .GetModule<MeteoriteFragmentModule>();
                                }

                                if (_meteoriteFragmentModule == null &&
                                    Owner != null)
                                {
                                        _meteoriteFragmentModule =
                                                Owner.GetModule
                                                        <MeteoriteFragmentModule>();
                                }
                        }
                }

                private void SubscribeEvents()
                {
                        if (_goldModule != null)
                        {
                                _goldModule.OnGoldChanged -=
                                        UpdateGoldUI;

                                _goldModule.OnGoldChanged +=
                                        UpdateGoldUI;
                        }

                        if (_meteoriteFragmentModule != null)
                        {
                                _meteoriteFragmentModule
                                        .OnMeteoriteFragmentChanged -=
                                        UpdateMeteoriteFragmentUI;

                                _meteoriteFragmentModule
                                        .OnMeteoriteFragmentChanged +=
                                        UpdateMeteoriteFragmentUI;
                        }
                }

                private void UnsubscribeEvents()
                {
                        if (_goldModule != null)
                        {
                                _goldModule.OnGoldChanged -=
                                        UpdateGoldUI;
                        }

                        if (_meteoriteFragmentModule != null)
                        {
                                _meteoriteFragmentModule
                                        .OnMeteoriteFragmentChanged -=
                                        UpdateMeteoriteFragmentUI;
                        }
                }

                private void UpdateGoldUI(int gold)
                {
                        if (GoldText == null)
                                return;

                        GoldText.text = gold.ToString("N0");
                }

                private void UpdateMeteoriteFragmentUI(
                        int meteoriteFragment
                )
                {
                        if (MeteoriteFragmentText == null)
                                return;

                        MeteoriteFragmentText.text =
                                meteoriteFragment.ToString("N0");
                }
        }
}
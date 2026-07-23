using Eric.ModuleSystem;
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
                        _meteoriteFragmentModule = Owner.GetModule<MeteoriteFragmentModule>();

                        if (GameModuleOwner.Instance != null)
                                _goldModule = GameModuleOwner.Instance.GetModule<GoldModule>();

                        SubscribeEvents();
                        UIUpdate();
                }

                private void OnDestroy()
                {
                        UnsubscribeEvents();
                }

                public void UIUpdate()
                {
                        if (_goldModule != null)
                                UpdateGoldUI(_goldModule.CurrentGold);

                        if (_meteoriteFragmentModule != null)
                                UpdateMeteoriteFragmentUI(_meteoriteFragmentModule.CurrentMeteoriteFragment);
                }

                private void SubscribeEvents()
                {
                        if (_goldModule != null)
                        {
                                _goldModule.OnGoldChanged -= UpdateGoldUI;
                                _goldModule.OnGoldChanged += UpdateGoldUI;
                        }

                        if (_meteoriteFragmentModule != null)
                        {
                                _meteoriteFragmentModule.OnMeteoriteFragmentChanged -= UpdateMeteoriteFragmentUI;
                                _meteoriteFragmentModule.OnMeteoriteFragmentChanged += UpdateMeteoriteFragmentUI;
                        }
                }

                private void UnsubscribeEvents()
                {
                        if (_goldModule != null)
                                _goldModule.OnGoldChanged -= UpdateGoldUI;

                        if (_meteoriteFragmentModule != null)
                                _meteoriteFragmentModule.OnMeteoriteFragmentChanged -= UpdateMeteoriteFragmentUI;
                }

                private void UpdateGoldUI(int gold)
                {
                        if (GoldText != null)
                                GoldText.text = gold.ToString("N0");
                }

                private void UpdateMeteoriteFragmentUI(int meteoriteFragment)
                {
                        if (MeteoriteFragmentText != null)
                                MeteoriteFragmentText.text = meteoriteFragment.ToString("N0");
                }
        }
}
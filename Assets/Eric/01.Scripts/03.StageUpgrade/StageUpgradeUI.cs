using Eric.Currency;
using Eric.ModuleSystem;
using Eric.ScriptableScripts;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Eric.StageUpgrade
{
        public class StageUpgradeUI : MonoBehaviour, IModule
        {
                private ModuleOwner Owner{get;set;}
                private StageUpgradeModule _stageUpgradeModule;
                private GoldModule _goldModule;

                [field:SerializeField] public StageUpgradeSO StageUpgrade{get;private set;}
                [field:SerializeField] public TextMeshProUGUI Name{get;private set;}
                [field:SerializeField] public TextMeshProUGUI CurStat{get;private set;}
                [field:SerializeField] public TextMeshProUGUI AfterUpgradeStat{get;private set;}
                [field:SerializeField] public TextMeshProUGUI Level{get;private set;}

                [field:FormerlySerializedAs("<NeedMF>k__BackingField")]
                [field:SerializeField] public TextMeshProUGUI NeedGold{get;private set;}

                [field:SerializeField] public Button BuyButton{get;private set;}
                [field:SerializeField] public Image Icon{get;private set;}

                public void Init(ModuleOwner owner)
                {
                        Owner = owner;
                }

                public void AfterInit()
                {
                        _stageUpgradeModule = Owner.GetModule<StageUpgradeModule>();
                        _goldModule = Owner.GetModule<GoldModule>();

                        if (_stageUpgradeModule == null || _goldModule == null)
                                return;

                        if (BuyButton != null)
                                BuyButton.onClick.AddListener(Buy);

                        _stageUpgradeModule.OnStageUpgradeDataChanged += UIUpdate;
                        _goldModule.OnGoldChanged += GoldChanged;

                        UIUpdate();
                }

                private void OnDestroy()
                {
                        if (BuyButton != null)
                                BuyButton.onClick.RemoveListener(Buy);

                        if (_stageUpgradeModule != null)
                                _stageUpgradeModule.OnStageUpgradeDataChanged -= UIUpdate;

                        if (_goldModule != null)
                                _goldModule.OnGoldChanged -= GoldChanged;
                }

                public void Buy()
                {
                        if (_stageUpgradeModule == null || StageUpgrade == null)
                                return;

                        _stageUpgradeModule.TryUpgrade(StageUpgrade);
                }

                public void UIUpdate()
                {
                        if (_stageUpgradeModule == null || StageUpgrade == null)
                                return;

                        int currentLevel = _stageUpgradeModule.GetLevel(StageUpgrade.StageUpgradeType);
                        bool isMaxLevel = currentLevel >= StageUpgrade.MaxLevel;

                        if (Name != null)
                                Name.text = StageUpgrade.UpgradeName;

                        if (CurStat != null)
                                CurStat.text = FormatStat(_stageUpgradeModule.GetCurrentStat(StageUpgrade));

                        if (AfterUpgradeStat != null)
                        {
                                AfterUpgradeStat.text = isMaxLevel
                                        ? "MAX"
                                        : $"=> {FormatStat(_stageUpgradeModule.GetAfterUpgradeStat(StageUpgrade))}";
                        }

                        if (Level != null)
                                Level.text = $"{currentLevel} / {StageUpgrade.MaxLevel}";

                        if (NeedGold != null)
                        {
                                NeedGold.text = isMaxLevel
                                        ? "MAX"
                                        : _stageUpgradeModule.GetNeedGold(StageUpgrade).ToString("N0");
                        }

                        if (Icon != null)
                        {
                                Icon.sprite = StageUpgrade.Icon;
                                Icon.enabled = StageUpgrade.Icon != null;
                        }

                        if (BuyButton != null)
                                BuyButton.interactable = _stageUpgradeModule.CanUpgrade(StageUpgrade);
                }

                private void GoldChanged(int amount)
                {
                        UIUpdate();
                }

                private string FormatStat(int value)
                {
                        if (StageUpgrade.StageUpgradeType == StageUpgradeType.MeteoriteFragment)
                                return $"{value:N0}%";

                        return value.ToString("N0");
                }
        }
}
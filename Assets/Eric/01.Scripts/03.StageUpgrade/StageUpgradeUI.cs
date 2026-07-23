using Eric.Currency;
using Eric.ModuleSystem;
using Eric.ScriptableScripts;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Eric.StageUpgrade
{
        public class StageUpgradeUI : MonoBehaviour, IModule
        {
                private ModuleOwner Owner{get;set;}
                private StageUpgradeModule _stageUpgradeModule;
                private MeteoriteFragmentModule _meteoriteFragmentModule;

                [field:Header("Upgrade Data")]
                [field:SerializeField] public StageUpgradeSO StageUpgrade{get;private set;}

                [field:Header("Text")]
                [field:SerializeField] public TextMeshProUGUI Name{get;private set;}
                [field:SerializeField] public TextMeshProUGUI CurStat{get;private set;}
                [field:SerializeField] public TextMeshProUGUI AfterUpgradeStat{get;private set;}
                [field:SerializeField] public TextMeshProUGUI Level{get;private set;}
                [field:SerializeField] public TextMeshProUGUI NeedMF{get;private set;}

                [field:Header("UI")]
                [field:SerializeField] public Button BuyButton{get;private set;}
                [field:SerializeField] public RawImage Icon{get;private set;}

                public void Init(ModuleOwner owner)
                {
                        Owner = owner;
                }

                public void AfterInit()
                {
                        _stageUpgradeModule = Owner.GetModule<StageUpgradeModule>();
                        _meteoriteFragmentModule = Owner.GetModule<MeteoriteFragmentModule>();

                        if (BuyButton != null)
                                BuyButton.onClick.AddListener(Buy);

                        _stageUpgradeModule.OnStageUpgradeDataChanged += UIUpdate;
                        _meteoriteFragmentModule.OnMeteoriteFragmentChanged += MeteoriteFragmentChanged;

                        UIUpdate();
                }

                private void OnDestroy()
                {
                        if (BuyButton != null)
                                BuyButton.onClick.RemoveListener(Buy);

                        if (_stageUpgradeModule != null)
                                _stageUpgradeModule.OnStageUpgradeDataChanged -= UIUpdate;

                        if (_meteoriteFragmentModule != null)
                                _meteoriteFragmentModule.OnMeteoriteFragmentChanged -= MeteoriteFragmentChanged;
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
                        float currentStat = _stageUpgradeModule.GetCurrentStat(StageUpgrade);
                        float afterStat = _stageUpgradeModule.GetAfterUpgradeStat(StageUpgrade);
                        bool isMaxLevel = currentLevel >= StageUpgrade.MaxLevel;

                        if (Name != null)
                                Name.text = StageUpgrade.UpgradeName;

                        if (CurStat != null)
                                CurStat.text = FormatStat(currentStat);

                        if (AfterUpgradeStat != null)
                                AfterUpgradeStat.text = isMaxLevel ? "MAX" : $"=> {FormatStat(afterStat)}";

                        if (Level != null)
                                Level.text = $"{currentLevel} / {StageUpgrade.MaxLevel}";

                        if (NeedMF != null)
                                NeedMF.text = isMaxLevel ? "MAX" : _stageUpgradeModule.GetNeedMF(StageUpgrade).ToString();

                        if (Icon != null)
                                Icon.texture = StageUpgrade.Icon;

                        if (BuyButton != null)
                                BuyButton.interactable = _stageUpgradeModule.CanUpgrade(StageUpgrade);
                }

                private void MeteoriteFragmentChanged(int amount)
                {
                        UIUpdate();
                }

                private string FormatStat(float value)
                {
                        if (StageUpgrade.IsIntStat)
                                return Mathf.RoundToInt(value).ToString();

                        return value.ToString("0.##");
                }
        }
}
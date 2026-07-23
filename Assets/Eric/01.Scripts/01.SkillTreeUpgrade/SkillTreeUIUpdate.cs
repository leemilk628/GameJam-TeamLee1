using Eric.ModuleSystem;
using Eric.ScriptableScripts;
using Eric.SkillTreeUpgrade;
using TMPro;
using UnityEngine;

namespace Eric.Upgrade
{
        public class SkillTreeUIUpdate : MonoBehaviour, IModule
        {
                private ModuleOwner Owner{get;set;}
                private SkillTreeUpgradeModule _skillTreeUpgradeModule;
                private ChangeSkillTreeImage[] _changeSkillTreeImages;

                [field:SerializeField] public TextMeshProUGUI Stats{get;private set;}

                public void Init(ModuleOwner owner)
                {
                        Owner = owner;
                        _changeSkillTreeImages = GetComponentsInChildren<ChangeSkillTreeImage>(true);
                }

                public void AfterInit()
                {
                        _skillTreeUpgradeModule = Owner.GetModule<SkillTreeUpgradeModule>();

                        if (_skillTreeUpgradeModule == null)
                                return;

                        _skillTreeUpgradeModule.OnMultiplyChanged += UIUpdate;
                        UIUpdate();
                }

                private void OnDestroy()
                {
                        if (_skillTreeUpgradeModule != null)
                                _skillTreeUpgradeModule.OnMultiplyChanged -= UIUpdate;
                }

                private void UIUpdate()
                {
                        if (_skillTreeUpgradeModule == null)
                                return;

                        foreach (ChangeSkillTreeImage image in _changeSkillTreeImages)
                        {
                                if (image != null)
                                        image.UpdateImages();
                        }

                        if (Stats == null)
                                return;

                        Stats.text =
                                $"PlayerHealth = +{_skillTreeUpgradeModule.GetAddValue(SkillTreeType.PlayerHealth):N0}\n" +
                                $"PlayerBarrier = +{_skillTreeUpgradeModule.GetAddValue(SkillTreeType.PlayerBarrier):N0}\n" +
                                $"PlayerAttack = +{_skillTreeUpgradeModule.GetAddValue(SkillTreeType.PlayerAttack):N0}\n" +
                                $"PlayerAttackSpeed = +{_skillTreeUpgradeModule.GetAddValue(SkillTreeType.PlayerAttackSpeed):N0}\n" +
                                $"SatelliteAttack = +{_skillTreeUpgradeModule.GetAddValue(SkillTreeType.SatelliteAttack):N0}\n" +
                                $"SatelliteAttackSpeed = +{_skillTreeUpgradeModule.GetAddValue(SkillTreeType.SatelliteAttackSpeed):N0}\n" +
                                $"GetGold = +{_skillTreeUpgradeModule.GetPercentIncrease(SkillTreeType.GetGold):N0}%\n" +
                                $"GetMeteoriteFragment = +{_skillTreeUpgradeModule.GetPercentIncrease(SkillTreeType.GetMeteoriteFragment):N0}%\n" +
                                $"MaxSatelliteCount = +{_skillTreeUpgradeModule.GetAddValue(SkillTreeType.MaxSatelliteCount):N0}\n" +
                                $"StartingGold = +{_skillTreeUpgradeModule.GetAddValue(SkillTreeType.StartingGold):N0}\n" +
                                $"BarrierRecoverySpeed = +{_skillTreeUpgradeModule.GetAddValue(SkillTreeType.BarrierRecoverySpeed):N0}";
                }
        }
}
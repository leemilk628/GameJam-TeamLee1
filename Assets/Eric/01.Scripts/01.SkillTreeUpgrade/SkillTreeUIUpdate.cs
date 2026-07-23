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
                        _changeSkillTreeImages = GetComponentsInChildren<ChangeSkillTreeImage>();
                }

                public void AfterInit()
                {
                        _skillTreeUpgradeModule = Owner.GetModule<SkillTreeUpgradeModule>();
                        if (_skillTreeUpgradeModule == null) return;
                        _skillTreeUpgradeModule.OnMultiplyChanged += UIUpdate;
                        UIUpdate();
                }

                private void OnDestroy()
                {
                        if (_skillTreeUpgradeModule != null) _skillTreeUpgradeModule.OnMultiplyChanged -= UIUpdate;
                }

                private void UIUpdate()
                {
                        if (Stats == null || _skillTreeUpgradeModule == null) return;
                        foreach (ChangeSkillTreeImage image in _changeSkillTreeImages)
                        {
                                image.UpdateImages();
                        }

                        float playerHealth = _skillTreeUpgradeModule.GetMultiply(SkillTreeType.PlayerHealth);
                        float playerBarrier = _skillTreeUpgradeModule.GetMultiply(SkillTreeType.PlayerBarrier);
                        float playerAttack = _skillTreeUpgradeModule.GetMultiply(SkillTreeType.PlayerAttack);
                        float playerAttackSpeed = _skillTreeUpgradeModule.GetMultiply(SkillTreeType.PlayerAttackSpeed);
                        float satelliteAttack = _skillTreeUpgradeModule.GetMultiply(SkillTreeType.SatelliteAttack);
                        float satelliteAttackSpeed = _skillTreeUpgradeModule.GetMultiply(SkillTreeType.SatelliteAttackSpeed);
                        float getMeteoriteFragment = _skillTreeUpgradeModule.GetMultiply(SkillTreeType.GetMeteoriteFragment);
                        float getGold = _skillTreeUpgradeModule.GetMultiply(SkillTreeType.GetGold);

                        Stats.text = $"PlayerHealth = {playerHealth:0.##}\n" +
                                     $"PlayerBarrier = {playerBarrier:0.##}\n" +
                                     $"PlayerAttack = {playerAttack:0.##}\n" +
                                     $"PlayerAttackSpeed = {playerAttackSpeed:0.##}\n" +
                                     $"SatelliteAttack = {satelliteAttack:0.##}\n" +
                                     $"SatelliteAttackSpeed = {satelliteAttackSpeed:0.##}\n" +
                                     $"GetMeteoriteFragment = {getMeteoriteFragment:0.##}\n" +
                                     $"GetGold = {getGold:0.##}";
                }
        }
}
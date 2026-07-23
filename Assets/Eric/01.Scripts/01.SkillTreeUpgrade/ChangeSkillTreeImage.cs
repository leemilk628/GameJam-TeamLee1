using System;
using Eric.ModuleSystem;
using Eric.ScriptableScripts;
using Eric.Upgrade;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Eric.SkillTreeUpgrade
{
        [Serializable]
        public enum BackgroundType
        {
                Opened,
                CanOpened,
                Locked
        }

        public class ChangeSkillTreeImage : MonoBehaviour, IModule
        {
                private ModuleOwner Owner{get;set;}

                [SerializeField] private Image _background;
                [SerializeField] private Image _icon;

                [field:SerializeField] private SkillTreeImageSO Background{get;set;}
                [field:SerializeField] private SkillTreeImageSO Icon{get;set;}
                [field:SerializeField] private DetectClick DetectClick{get;set;}
                [field:SerializeField] private TextMeshProUGUI Description{get;set;}
                [field:SerializeField] private TextMeshProUGUI Cost{get;set;}

                public void Init(ModuleOwner owner)
                {
                        Owner = owner;

                        if (DetectClick == null)
                                DetectClick = GetComponent<DetectClick>();

                        if (_background == null)
                                _background = GetComponent<Image>();

                        if (_icon == null && transform.childCount > 0)
                                _icon = transform.GetChild(0).GetComponent<Image>();
                }

                public void AfterInit()
                {
                        UpdateImages();
                }

                public void UpdateImages()
                {
                        Description.text = "<color=red>" + DetectClick.SkillTree.Description;
                        Cost.text = "<color=red>" + DetectClick.SkillTree.NeedMeteoriteFragment.ToString() + "MF";
                        if (DetectClick == null || DetectClick.SkillTree == null)
                                return;

                        BackgroundType backgroundType = GetBackgroundType();
                        Sprite backgroundSprite = SetBackground(backgroundType);

                        if (_background != null)
                                _background.sprite = backgroundSprite;

                        if (_icon == null)
                                return;

                        _icon.sprite = backgroundType == BackgroundType.Locked
                                ? backgroundSprite
                                : SetIcon(DetectClick.SkillTree.SkillTreeType);
                }

                private BackgroundType GetBackgroundType()
                {
                        SkillTreeSO skillTree = DetectClick.SkillTree;

                        if (skillTree.IsUpgrade)
                                return BackgroundType.Opened;

                        if (skillTree.BeforeNode == null || skillTree.BeforeNode.IsUpgrade)
                                return BackgroundType.CanOpened;

                        return BackgroundType.Locked;
                }

                private Sprite SetBackground(BackgroundType backgroundType)
                {
                        if (Background == null)
                                return null;

                        return backgroundType switch
                        {
                                BackgroundType.Opened => Background.GetNode(0),
                                BackgroundType.CanOpened => Background.GetNode(1),
                                BackgroundType.Locked => Background.GetNode(2),
                                _ => null
                        };
                }

                private Sprite SetIcon(SkillTreeType skillTreeType)
                {
                        if (Icon == null)
                                return null;

                        return skillTreeType switch
                        {
                                SkillTreeType.PlayerHealth => Icon.GetNode(0),
                                SkillTreeType.PlayerBarrier => Icon.GetNode(1),
                                SkillTreeType.PlayerAttack => Icon.GetNode(2),
                                SkillTreeType.PlayerAttackSpeed => Icon.GetNode(3),
                                SkillTreeType.GetGold => Icon.GetNode(4),
                                SkillTreeType.Satellite => Icon.GetNode(5),
                                SkillTreeType.SatelliteAttack => Icon.GetNode(6),
                                SkillTreeType.SatelliteAttackSpeed => Icon.GetNode(7),
                                SkillTreeType.GetMeteoriteFragment => Icon.GetNode(8),
                                SkillTreeType.MaxSatelliteCount => Icon.GetNode(9),
                                SkillTreeType.StartingGold => Icon.GetNode(10),
                                SkillTreeType.BarrierRecoverySpeed => Icon.GetNode(11),
                                _ => null
                        };
                }
        }
}
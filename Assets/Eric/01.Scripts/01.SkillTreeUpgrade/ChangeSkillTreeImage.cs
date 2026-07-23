using System;
using Eric.ModuleSystem;
using Eric.ScriptableScripts;
using Eric.Upgrade;
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
                private ModuleOwner Owner { get; set; }
                [field: SerializeField] private Image _background;
                [field: SerializeField] private Image _icon;
                [field: SerializeField] private SkillTreeImageSO Background { get; set; }

                [field: SerializeField] private SkillTreeImageSO Icon { get; set; }
                [field: SerializeField] private DetectClick DetectClick { get; set; }

                private Sprite SetBackground(BackgroundType type)
                {
                        Sprite result = type switch
                        {
                                BackgroundType.Opened => Background.GetNode(0),
                                BackgroundType.CanOpened => Background.GetNode(1),
                                BackgroundType.Locked => Background.GetNode(2),
                        };
                        return result;
                }

                private Sprite SetIcon(SkillTreeType type)
                {
                        Sprite result = type switch
                        {
                                SkillTreeType.PlayerHealth => Icon.GetNode(0),
                                SkillTreeType.PlayerBarrier => Icon.GetNode(1),
                                SkillTreeType.PlayerAttack => Icon.GetNode(2),
                                SkillTreeType.PlayerAttackSpeed => Icon.GetNode(3),
                                SkillTreeType.GetGold => Icon.GetNode(4),
                                SkillTreeType.Satellite => Icon.GetNode(5),
                                SkillTreeType.SatelliteAttack => Icon.GetNode(6),
                                SkillTreeType.SatelliteAttackSpeed => Icon.GetNode(7)
                        };
                        return result;
                }

                public void UpdateImages()
                {
                        BackgroundType backgroudType;
                        if (DetectClick.SkillTree.BeforeNode != null)
                        {
                                if(DetectClick.SkillTree.IsUpgrade) backgroudType = BackgroundType.Opened;
                                else if(DetectClick.SkillTree.BeforeNode.IsUpgrade) backgroudType = BackgroundType.CanOpened;
                                else backgroudType = BackgroundType.Locked;
                        }
                        else
                        {
                                if(DetectClick.SkillTree.IsUpgrade) backgroudType = BackgroundType.Opened;
                                else backgroudType = BackgroundType.CanOpened;
                        }
                        SkillTreeType skillTreeType = DetectClick.SkillTree.SkillTreeType;
                        _background.sprite = SetBackground(backgroudType);
                        if (backgroudType == BackgroundType.Locked) _icon.sprite = SetBackground(backgroudType);
                        else _icon.sprite = SetIcon(skillTreeType);
                }

                public void Init(ModuleOwner owner)
                {
                        Owner = owner;
                        DetectClick = transform.GetComponent<DetectClick>();
                        _background = transform.GetComponent<Image>();
                        _icon = transform.GetChild(0).GetComponent<Image>();
                }

                public void AfterInit()
                {                        
                        UpdateImages();
                }
        }
}